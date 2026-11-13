using UnityEngine;

public class HandController : MonoBehaviour
{
    [Header("FINGERS")]
    [Range(0, 90)] public float indexMCP;
    [Range(0, 110)] public float indexPIP;

    [Range(0, 90)] public float middleMCP;
    [Range(0, 110)] public float middlePIP;

    [Range(0, 90)] public float ringMCP;
    [Range(0, 110)] public float ringPIP;

    [Range(0, 90)] public float pinkyMCP;
    [Range(0, 110)] public float pinkyPIP;

    [Header("Thumb")]
    [Range(0, 45)] public float thumbCMC; // base curl
    [Range(0, 90)] public float thumbIP;  // independent tip

    [Header("BONES")]
    public Transform index1, index2, index3;
    public Transform middle1, middle2, middle3;
    public Transform ring1, ring2, ring3;
    public Transform pinky1, pinky2, pinky3;
    public Transform thumb1, thumb2, thumb3;

    // Store neutral rotations
    Quaternion i1R, i2R, i3R;
    Quaternion m1R, m2R, m3R;
    Quaternion r1R, r2R, r3R;
    Quaternion p1R, p2R, p3R;
    Quaternion t1R, t2R, t3R;

    const float DIP_RATIO = 0.65f; // finger DIP follows PIP
    const float IP_RATIO = 0.9f;   // thumb IP proportional to PIP if needed

    void Awake()
    {
        // Fingers neutral rotations
        i1R = index1.localRotation; i2R = index2.localRotation; i3R = index3.localRotation;
        m1R = middle1.localRotation; m2R = middle2.localRotation; m3R = middle3.localRotation;
        r1R = ring1.localRotation; r2R = ring2.localRotation; r3R = ring3.localRotation;
        p1R = pinky1.localRotation; p2R = pinky2.localRotation; p3R = pinky3.localRotation;

        // Thumb neutral rotations
        t1R = thumb1.localRotation; t2R = thumb2.localRotation; t3R = thumb3.localRotation;
    }

    void Update()
    {
        // Fingers
        ApplyFinger(indexMCP, indexPIP, index1, index2, index3, i1R, i2R, i3R);
        ApplyFinger(middleMCP, middlePIP, middle1, middle2, middle3, m1R, m2R, m3R);
        ApplyFinger(ringMCP, ringPIP, ring1, ring2, ring3, r1R, r2R, r3R);
        ApplyFinger(pinkyMCP, pinkyPIP, pinky1, pinky2, pinky3, p1R, p2R, p3R);

        // Thumb
        ApplyThumb();
    }

    void ApplyFinger(float mcp, float pip, Transform mcpBone, Transform pipBone, Transform dipBone, Quaternion mcpBase, Quaternion pipBase, Quaternion dipBase)
    {
        mcpBone.localRotation = mcpBase * Quaternion.Euler(mcp, 0, 0);
        pipBone.localRotation = pipBase * Quaternion.Euler(pip, 0, 0);
        dipBone.localRotation = dipBase * Quaternion.Euler(pip * DIP_RATIO, 0, 0);
    }

    void ApplyThumb()
    {
        // --- CMC (thumb1) ---
        float cmcFlex = thumbCMC;               // base curl slider
        float cmcAbd = thumbCMC * 0.5f;        // lateral circular motion
        thumb1.localRotation = t1R *
                               Quaternion.AngleAxis(-cmcFlex, thumb1.right) *
                               Quaternion.AngleAxis(-cmcAbd, thumb1.up);

        // --- PIP/MCP (thumb2) ---
        // Amplify PIP curl relative to CMC for realistic reach
        float pipMultiplier = 1.3f;             // increase bending
        float pipAngle = cmcFlex * pipMultiplier;
        thumb2.localRotation = t2R * Quaternion.AngleAxis(-pipAngle, thumb2.up);

        float ipAngle = Mathf.Clamp(thumbIP, 0f, 90f);

        // -- IP --
        // Compute the desired bend axis
        Vector3 pipAxisWorld = thumb2.right;   // PIP flexion axis
        Vector3 ipAxisWorld = thumb3.right;    // IP neutral axis

        // Interpolated axis (toward PIP direction)
        Vector3 flexAxis = (pipAxisWorld + ipAxisWorld).normalized;

        // Apply rotation along interpolated axis
        thumb3.localRotation = t3R * Quaternion.AngleAxis(-ipAngle, thumb3.InverseTransformDirection(flexAxis));
    }
}
