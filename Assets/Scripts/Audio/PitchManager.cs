using UnityEngine;
using UnityEngine.Audio;
using Pitch;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class PitchManager : MonoBehaviour
{
    public static float PitchValue;
    public static int MidiNote;

    public AudioMixer output;

    const int SampleSize = 2048;
    const int HistorySize = 10;
    const float Threshold = 0.02f;

    int sampleFreq;
    float[] samples = new float[SampleSize];

    AudioSource source;
    PitchTracker pitchTracker;

    void Start()
    {
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
        source.GetOutputData(samples, 0);
        pitchTracker.ProcessBuffer(samples);
    }

    void PitchDetected(PitchTracker sender, PitchTracker.PitchRecord record)
    {
        if (!GameLogic.GameStarted) return;

        if (record.Pitch > 0)
        {
            PitchValue = record.Pitch;

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
            }
        }
    }

    static public int FreqToMidi(float freq)
    {
        return 12 * (int)Mathf.Log(freq / 440f, 2) + 69;
    }

    static public float MidiToFreq(int midi)
    {
        return Mathf.Pow(2, (midi - 69) / 12f) * 440;
    }
}
