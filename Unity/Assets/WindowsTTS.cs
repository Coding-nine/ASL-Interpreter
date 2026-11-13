using UnityEngine;
using System.Speech.Synthesis;

public class WindowsTTS : MonoBehaviour
{
    private SpeechSynthesizer synth;

    void Awake()
    {
        synth = new SpeechSynthesizer();
        synth.SetOutputToDefaultAudioDevice();
    }

    public void Speak(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        synth.SpeakAsync(text); // speak asynchronously so it doesn't block Unity
    }

    void OnApplicationQuit()
    {
        synth.Dispose();
    }
}
