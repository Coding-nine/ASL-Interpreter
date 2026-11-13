using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class ASLUIController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI outputText;

    [Header("ASL")]
    public ASLTranslator aslTranslator;

    private bool translating = false;
    private char lastLetter = '\0';

    void Start()
    {
        outputText.text = "...";
    }

    void Update()
    {
        // START
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            translating = true;
            outputText.text = "Listening...";
            Debug.Log("ASL STARTED");
        }

        // STOP
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            translating = false;
            outputText.text = "Stopped";
            Debug.Log("ASL STOPPED");
        }

        if (!translating) return;

        char detected = aslTranslator.RecognizeLetter();
        Debug.Log("Detected: " + detected);

        if (detected == '\0' || detected == '-' || detected == lastLetter)
            return;

        lastLetter = detected;
        outputText.text += detected.ToString();
    }
}
