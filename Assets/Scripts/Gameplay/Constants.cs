using System.Collections;
using System.Collections.Generic;

public class Constants {

    public static bool GameOn = false;
    public static bool Endless = true;
    public static int[] Level;

    public const int PPU = 16;

    public const int MinNote = 48; // C3
    public const int MaxNote = 72; // C5

    public const float VecPrecision = 0.01f;

    public const float NormalPrecision = 0.1f;
    public const float EndlessPrecision = 0.25f;

    public const float PersonOffset = -0.5f;

    public const float EndlessPatience = 30f;
    public const float NormalPatience = 15f;

    public const float EndlessSpawnDecay = 0.95f;
    public const float EndlessInitSpawnTime = 7f;
}
