using System.Collections;
using System.Collections.Generic;

public class Constants
{

    public static bool GameOn = false;
    public static bool Endless = true;
    public static int[] Level;

    public const int PPU = 16;

    public const int MinNote = 48; // C3
    public const int MaxNote = 72; // C5

    public const float StopDelay = 0.2f;

    public const float VecPrecision = 0.01f;

    public const float NotePrecision = 0.5f;

    public const float PersonOffset = -0.5f;

    public const float EndlessPatience = 30f;
    public const float NormalPatience = 15f;

    public const float EndlessSpawnDecay = 0.95f;
    public const float EndlessInitSpawnTime = 7f;

    public const float Version = 0.01f;
    public const string VersionKey = "version";
}
