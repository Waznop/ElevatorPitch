using UnityEngine;
using System.Collections;

public class LevelManager
{
    public static int[] GenerateLevel(int n)
    {
        int[] notes = new int[n];

        notes[0] = Random.Range(GameLogic.MinNote+1, GameLogic.MaxNote+1);

        for (int i = 1; i < n; i++) {
            notes[i] = Random.Range(GameLogic.MinNote, GameLogic.MaxNote+1);
            while (notes[i] == notes[i-1]) {
                notes[i] = Random.Range(GameLogic.MinNote, GameLogic.MaxNote+1);
            }
        }

        return notes;
    }

}
