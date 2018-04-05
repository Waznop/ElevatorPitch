﻿using UnityEngine;using System.Collections;public class LevelManager{    public static int[] Twinkle = { 57, 64, 66, 64, 62, 61, 59, 57 };    public static int[] HeyMister = { 67, 69, 71, 69, 62, 69, 67, 69, 62, 64, 62, 59 };    public static int[] Shindlers = { 57, 69, 62, 69, 62, 70, 69, 65, 57, 65, 55, 65, 67, 69 };    public static int[] Challenge = { 57, 69, 52, 64, 53, 65, 52, 64, 50, 62, 48, 60, 72, 71, 69, 57 };    public static int[] GenerateLevel(int n)    {        int[] notes = new int[n];        notes[0] = Random.Range(Constants.MinNote + 1, Constants.MaxNote + 1);        for (int i = 1; i < n; i++)        {            notes[i] = Random.Range(Constants.MinNote, Constants.MaxNote + 1);            while (notes[i] == notes[i - 1])            {                notes[i] = Random.Range(Constants.MinNote, Constants.MaxNote + 1);            }        }        return notes;    }}