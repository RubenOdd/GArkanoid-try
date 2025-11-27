// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using System.IO;
using Godot;

namespace GA.GArkanoid;

public static class Config
{
    public static StringName MoveRightAction = "MoveRight";
    public static StringName MoveLeftAction = "MoveLeft";
    public static StringName LaunchAction = "LaunchBall";
    public static StringName PauseAction = "Pause";

    #region Settings
    public static Vector2I Window640 = new(640, 360);
    public static Vector2I Window1280 = new(1280, 720);
    public static Vector2I Window1920 = new(1920, 1080);
    #endregion

    #region Level Data

    public const int LevelCount = 2;
    public const float PowerUpSpawnChance = 0.3f;

    #endregion Level Data

    #region Player data
    public static int MaxLives = 100;
    public static string DefaultPlayerDataPath = "res://Config/DefaultPlayerData.tres";
    #endregion Player data

    #region Audio
    public static StringName MasterBusName = "Master";
    public static StringName MusicBusName = "Music";
    public static StringName SFXBusName = "SFX";
    public static string MusicDataPath = "res://Config/MusicData.tres";
    #endregion Audio

    #region Save
    public static string QuickSaveName = "QuickSave";
    public static string SettingsSaveName = "PlayerSettings";
    public static string SaveFolderName = "Save";
    public static string SaveFileExtension = ".json";
    public static string SaveSettingsFileExtension = ".ini";
    public static string PlayerDataKey = "PlayerData";
    public static string LevelDataKey = "LevelData";
    public static StringName QuickSaveAction = "QuickSave";

    public static string GetSaveFolderPath()
    {
        // The path on user's disk where save files can be written.
        string path = ProjectSettings.GlobalizePath("user://");
        // Path.Combine selects corrent path separation character (/ or \) based on the user's platform.
        path = Path.Combine(path, SaveFolderName);
        return path;
    }
    #endregion Save
}
