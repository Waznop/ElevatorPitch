using UnityEngine;
using System.Collections;

public class LevelManager
{
    public static int[] GenerateLevel(int n)
    {
        int[] notes = new int[n];

        notes[0] = Random.Range(Constants.MinNote+1, Constants.MaxNote+1);

        for (int i = 1; i < n; i++) {
            notes[i] = Random.Range(Constants.MinNote, Constants.MaxNote+1);
            while (notes[i] == notes[i-1]) {
                notes[i] = Random.Range(Constants.MinNote, Constants.MaxNote+1);
            }
        }

        return notes;
    }

}
