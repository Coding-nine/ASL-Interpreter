using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandUDPReceiver : MonoBehaviour
{
    public HandController handController; // Assign your HandController

    [Header("UDP Settings")]
    public int listenPort = 5005;
    private UdpClient client;
    private Thread receiveThread;
    private bool running = true;

    [Header("Wrist Rotation")]
    private Quaternion imuRotation = Quaternion.identity;
    private Quaternion neutralRot = Quaternion.identity;
    private Quaternion smoothedRotation = Quaternion.identity;
    [Range(0f, 1f)] public float rotationSmooth = 0.2f;
    public float rotationAmplifier = 2.0f;
    private bool calibrated = false;

    [Header("Finger Smoothing")]
    [Range(0f, 1f)] public float fingerSmooth = 0.1f;

    // --- Calibration values (set per sensor/joint) ---
    [Header("Thumb")]
    public int Thumb_CMC_Straight = 830;
    public int Thumb_CMC_Bent = 790;
    public int Thumb_IP_Straight = 900;
    public int Thumb_IP_Bent = 830;
    public int Thumb_CMC_MaxAngle = 45;   // CMC 0-45
    public int Thumb_IP_MaxAngle = 90;   // IP 0-90

    [Header("Index")]
    public int Index_MCP_Straight = 840;
    public int Index_MCP_Bent = 790;
    public int Index_PIP_Straight = 860;
    public int Index_PIP_Bent = 815;
    public int Index_MCP_MaxAngle = 90;
    public int Index_PIP_MaxAngle = 110;

    [Header("Middle")]
    public int Middle_MCP_Straight = 620;
    public int Middle_MCP_Bent = 570;
    public int Middle_PIP_Straight = 800;
    public int Middle_PIP_Bent = 740;
    public int Middle_MCP_MaxAngle = 90;
    public int Middle_PIP_MaxAngle = 110;

    [Header("Ring")]
    public int Ring_MCP_Straight = 18000;
    public int Ring_MCP_Bent = 8000;
    public int Ring_PIP_Straight = 15000;
    public int Ring_PIP_Bent = 7000;
    public int Ring_MCP_MaxAngle = 90;
    public int Ring_PIP_MaxAngle = 110;

    [Header("Pinky")]
    public int Pinky_MCP_Straight = 6000;
    public int Pinky_MCP_Bent = 0;
    public int Pinky_PIP_Straight = 12000;
    public int Pinky_PIP_Bent = 6000;
    public int Pinky_MCP_MaxAngle = 90;
    public int Pinky_PIP_MaxAngle = 110;

    void Start()
    {
        client = new UdpClient(listenPort);
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();

        smoothedRotation = transform.rotation;
    }

    void Update()
    {
        // --- Calibrate neutral wrist pose ---
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            neutralRot = imuRotation;
            calibrated = true;
            Debug.Log("Neutral wrist pose calibrated.");
        }

        // --- Apply wrist rotation ---
        if (calibrated)
        {
            Quaternion delta = imuRotation * Quaternion.Inverse(neutralRot);
            Quaternion amplified = Quaternion.Slerp(Quaternion.identity, delta, rotationAmplifier);
            smoothedRotation = Quaternion.Slerp(smoothedRotation, amplified, rotationSmooth);
            transform.rotation = smoothedRotation;
        }
        else
        {
            transform.rotation = imuRotation;
        }
    }

    private void ReceiveData()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);

        while (running)
        {
            try
            {
                byte[] data = client.Receive(ref remoteEP);
                string message = Encoding.ASCII.GetString(data).Trim();
                string[] parts = message.Split(',');

                if (parts.Length >= 14)
                {
                    // --- IMU quaternion (first 4 values) ---
                    if (float.TryParse(parts[0], out float q0) &&
                        float.TryParse(parts[1], out float q1) &&
                        float.TryParse(parts[2], out float q2) &&
                        float.TryParse(parts[3], out float q3))
                    {
                        Quaternion raw = new Quaternion(q1, q2, q3, q0);
                        imuRotation = new Quaternion(raw.y, raw.x, raw.z, raw.w);
                    }

                    // // --- Thumb ---
                    // ApplyFingerAngle(parts, 4, ref handController.thumbCMC, Thumb_CMC_Straight, Thumb_CMC_Bent, Thumb_CMC_MaxAngle);
                    // ApplyFingerAngle(parts, 5, ref handController.thumbIP, Thumb_IP_Straight, Thumb_IP_Bent, Thumb_IP_MaxAngle);

                    // --- Index ---
                    ApplyFingerAngle(parts, 6, ref handController.indexMCP, Index_MCP_Straight, Index_MCP_Bent, Index_MCP_MaxAngle);
                    ApplyFingerAngle(parts, 7, ref handController.indexPIP, Index_PIP_Straight, Index_PIP_Bent, Index_PIP_MaxAngle);

                    // --- Middle ---
                    ApplyFingerAngle(parts, 8, ref handController.middleMCP, Middle_MCP_Straight, Middle_MCP_Bent, Middle_MCP_MaxAngle);
                    ApplyFingerAngle(parts, 9, ref handController.middlePIP, Middle_PIP_Straight, Middle_PIP_Bent, Middle_PIP_MaxAngle);

                    // --- Ring ---
                    ApplyFingerAngle(parts, 10, ref handController.ringMCP, Ring_MCP_Straight, Ring_MCP_Bent, Ring_MCP_MaxAngle);
                    ApplyFingerAngle(parts, 11, ref handController.ringPIP, Ring_PIP_Straight, Ring_PIP_Bent, Ring_PIP_MaxAngle);

                    // --- Pinky ---
                    ApplyFingerAngle(parts, 12, ref handController.pinkyMCP, Pinky_MCP_Straight, Pinky_MCP_Bent, Pinky_MCP_MaxAngle);
                    ApplyFingerAngle(parts, 13, ref handController.pinkyPIP, Pinky_PIP_Straight, Pinky_PIP_Bent, Pinky_PIP_MaxAngle);
                }
            }
            catch { }
        }
    }

    private void ApplyFingerAngle(string[] parts, int index, ref float fingerValue, int straight, int bent, int maxAngle)
    {
        if (float.TryParse(parts[index], out float raw))
        {
            raw = Mathf.Clamp(raw, bent, straight);
            float angle = Mathf.Lerp(0f, maxAngle, (straight - raw) / (float)(straight - bent));
            fingerValue = Mathf.Lerp(fingerValue, angle, fingerSmooth);
        }
    }

    void OnApplicationQuit()
    {
        running = false;
        receiveThread?.Abort();
        client.Close();
    }
}
