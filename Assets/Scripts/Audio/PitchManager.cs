using UnityEngine;
using UnityEngine.Audio;
using Pitch;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class PitchManager : MonoBehaviour
{
    public static float PitchValue;
    public static int MidiNote;

    public bool FFT;
    public bool ShowGUI;

    public float Pitch1H;
    public float PitchFFT;
    public float PitchAC;

    public AudioMixer output;

    public const int SampleSize = 2048;

    // AC
    const int HistorySize = 10;
    const float Threshold = 0.05f;

    // FFT
    const int Harmonics = 5;
    const float ThresholdFFT = 0.01f;

    int sampleFreq;
    float[] samples;
    public static float[] spectrum;

    AudioSource source;
    PitchTracker pitchTracker;

    void Start()
    {
        samples = new float[SampleSize];
        spectrum = new float[SampleSize];

        // Project Settings > Audio > DSP Buffer Size = Best latency
        sampleFreq = AudioSettings.outputSampleRate;

        pitchTracker = new PitchTracker();
        pitchTracker.SampleRate = sampleFreq;
        pitchTracker.PitchDetected += PitchDetected;
        pitchTracker.DetectLevelThreshold = Threshold;
        pitchTracker.SampleRate = sampleFreq;
        pitchTracker.RecordPitchRecords = true;
        pitchTracker.PitchRecordHistorySize = HistorySize;

        output.SetFloat("MasterVolume", -80f);
        AudioMixerGroup master = output.FindMatchingGroups("Master")[0];

        source = GetComponent<AudioSource>();
        source.clip = Microphone.Start(null, true, 1, sampleFreq);
        source.outputAudioMixerGroup = master;
        source.loop = true;
        while (Microphone.GetPosition(null) <= 0) { }
        source.Play();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) {
            ShowGUI = !ShowGUI;
        }

        if (!Constants.GameOn) return;

        if (FFT)
        {
            ProcessSpectrum();
        }

        source.GetOutputData(samples, 0);
        pitchTracker.ProcessBuffer(samples);
    }

    /*
	void OnGUI()
	{
        if (ShowGUI)
        {
            GUI.skin.textField.fontSize = 50;
            GUI.TextField(
                new Rect(50, 200, 630, 180),
                "Basic FFT:          " + Pitch1H.ToString() + "\n" +
                "Harmonics Sum: " + PitchFFT.ToString() + "\n" +
                "Autocorrelation:  " + PitchAC.ToString()
            );
        }
	}
	*/

	void ProcessSpectrum()
    {
        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        int maxI_1h = 0;
        float maxEnergy_1h = 0;

        int maxI = 0;
        float maxEnergy = 0;
        for (int i = 0; i < SampleSize; i++)
        {
            if (spectrum[i] < ThresholdFFT) continue;

            float energy = 0;
            for (int h = 1; h <= Harmonics; h++)
            {
                int idx = i * h;
                if (idx >= SampleSize) break;
                energy += spectrum[idx];
            }

            if (energy > maxEnergy)
            {
                maxEnergy = energy;
                maxI = i;
            }

            if (spectrum[i] > maxEnergy_1h)
            {
                maxEnergy_1h = spectrum[i];
                maxI_1h = i;
            }
        }

        float maxFreqBin = maxI;
        if (maxI > 0 && maxI < SampleSize - 1) // neighbours interpolation
        {
            float dL = spectrum[maxI - 1] / spectrum[maxI];
            float dR = spectrum[maxI + 1] / spectrum[maxI];
            maxFreqBin += 0.5f * (dR * dR - dL * dL);
        }

        float pitch = maxFreqBin * (sampleFreq / 2) / SampleSize;
        if (pitch > 0)
        {
            PitchFFT = pitch;
        }

        float maxFreqBin_1h = maxI_1h;
        if (maxI_1h > 0 && maxI_1h < SampleSize - 1)
        {
            float dL = spectrum[maxI_1h - 1] / spectrum[maxI_1h];
            float dR = spectrum[maxI_1h + 1] / spectrum[maxI_1h];
            maxFreqBin_1h += 0.5f * (dR * dR - dL * dL);
        }

        float pitch_1h = maxFreqBin_1h * (sampleFreq / 2) / SampleSize;
        if (pitch_1h > 0)
        {
            Pitch1H = pitch_1h;
        }
    }

    void PitchDetected(PitchTracker sender, PitchTracker.PitchRecord record)
    {
        if (!Constants.GameOn) return;

        if (record.Pitch > 0)
        {
            bool consistentNote = true;
            foreach (PitchTracker.PitchRecord rec in pitchTracker.PitchRecords)
            {
                if (rec.MidiNote != 0 && record.MidiNote != rec.MidiNote)
                {
                    consistentNote = false;
                    break;
                }
            }

            if (consistentNote)
            {
                MidiNote = record.MidiNote;
                PitchValue = record.Pitch;
                PitchAC = record.Pitch;
            }
        }
    }

    public static float PitchToNote(float pitch)
    {
        return 12 * Mathf.Log(pitch / 440f, 2) + 69;
    }

    public static float NoteToPitch(int note)
    {
        return Mathf.Pow(2, (note - 69) / 12f) * 440;
    }

    public static string NoteToName(int note)
    {
        return PitchDsp.GetNoteName(note, true, true);
    }
}
