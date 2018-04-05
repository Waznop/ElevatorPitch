using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Transform Cube;

    Transform[] cubes;

    void Start()
    {
        cubes = new Transform[PitchManager.SampleSize];
        Vector3 scale = Vector3.one * 2f / PitchManager.SampleSize;
        scale.y *= 20;
        for (int i = 0; i < PitchManager.SampleSize; i++)
        {
            Transform tf = Instantiate(Cube);
            tf.localPosition = Vector3.right * ((i + 0.5f) * 2f / PitchManager.SampleSize - 1f);
            tf.localScale = scale;
            cubes[i] = tf;
        }
        Constants.GameOn = true;
    }

    void Update()
    {
        for (int i = 0; i < PitchManager.SampleSize; i++)
        {
            float val = PitchManager.spectrum[i];
            Vector3 pos = cubes[i].localPosition;
            pos.y = val * 5;
            cubes[i].localPosition = pos;
        }
    }
}
