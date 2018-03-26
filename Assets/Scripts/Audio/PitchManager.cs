﻿using UnityEngine;
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
    float[] samples;

    AudioSource source;
    PitchTracker pitchTracker;

    void Start()
    {
        samples = new float[SampleSize];

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
