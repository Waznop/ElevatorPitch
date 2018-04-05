using System.Collections;
using System.Collections.Generic;

public class Constants
{

    public static bool GameOn = false;
    public static bool Endless = true;
    public static int[] Level;
    public static string LevelKey = L0Key;

    public static int MinNote = 48; // C3
    public static int MaxNote = 72; // C5

    public const string VolumeKey = "volume";
    public const string VoiceRangeKey = "voicerange";

    public const string L0Key = "Endless Mode";
    public const string L1Key = "Level 1";
    public const string L2Key = "Level 2";
    public const string L3Key = "Level 3";
    public const string L4Key = "Level 4";
    public const string L5Key = "Level 5 (Random)";

    public const int PPU = 16;

    public const float StopDelay = 0.2f;

    public const float VecPrecision = 0.01f;

    public const float NotePrecision = 0.5f;

    public const float PersonOffset = -0.5f;

    public const float EndlessPatience = 30f;
    public const float NormalPatience = 15f;

    public const float CriticalTimer = 0.3f;

    public const float EndlessSpawnDecay = 0.95f;
    public const float EndlessInitSpawnTime = 7f;

    public const float Version = 0.01f;
    public const string VersionKey = "version";
}
