// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

// Used a little help from this video for resolution settings: https://youtu.be/YsdkcPV0BAo?si=pAw6lTV-BBnG2lGp

using GA.GArkanoid.States;
using GA.GArkanoid.Systems;
using Godot;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace GA.GArkanoid.UI;
public partial class UISettings : Control
{
    private struct Resolution
    {
        public int Width;
        public int Height;
        public int Multiplyer;

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }

        public static explicit operator Vector2I(Resolution resolution)
        {
            return new Vector2I(resolution.Width, resolution.Height);
        }
    }

    [Export] private Button _closeButton = null;
    [Export] private Button _menuButon = null;
    [Export] private UIAudioControl _masterControl = null;
    [Export] private UIAudioControl _musicControl = null;
    [Export] private UIAudioControl _sfxControl = null;
    [Export] private string _masterBusName = null;
    [Export] private string _musicBusName = null;
    [Export] private string _sfxBusName = null;
    [Export] private string _masterBusDisplayName = null;
    [Export] private string _musicBusDisplayName = null;
    [Export] private string _sfxBusDisplayName = null;
    [Export] private OptionButton _windowSizeOption = null;
    [Export] private CheckBox _fullscreenCheck = null;
    private Dictionary<int, Resolution> _resolutions = [];

    public override void _Ready()
    {
        InitializeAudioControls();
        InitializeVideoControl();
    }

    public override void _EnterTree()
    {
        _closeButton.Pressed += OnClose;
        _menuButon.Pressed += OnMenu;

        _masterControl.VolumeChanged += OnVolumeChanged;
        _musicControl.VolumeChanged += OnVolumeChanged;
        _sfxControl.VolumeChanged += OnVolumeChanged;

        _fullscreenCheck.Toggled += OnFullscreenToggled;
    }

    public override void _ExitTree()
    {
        _closeButton.Pressed -= OnClose;

        _masterControl.VolumeChanged -= OnVolumeChanged;
        _musicControl.VolumeChanged -= OnVolumeChanged;
        _sfxControl.VolumeChanged -= OnVolumeChanged;

        _fullscreenCheck.Toggled -= OnFullscreenToggled;
    }

    private void InitializeVideoControl()
    {
        Vector2I viewportSize = GameManager.MinWindowSize;
        int currentScreen = DisplayServer.WindowGetCurrentScreen();
        Vector2I screnSize = DisplayServer.ScreenGetSize();
        int multiplyer = 1;
        int id = 0;

        while (GetWidth(viewportSize, multiplyer) <= screnSize.X &&
            GetHeight(viewportSize, multiplyer) <= screnSize.Y)
        {
            Resolution resolution = new()
            {
                Width = GetWidth(viewportSize, multiplyer),
                Height = GetHeight(viewportSize, multiplyer),
                Multiplyer = multiplyer
            };

            _resolutions.Add(id, resolution);
            _windowSizeOption.AddItem(resolution.ToString(), id);

            multiplyer++;
            id++;
        }
    }

    private static int GetWidth(Vector2I viewportSize, int multiplyer)
    {
        return viewportSize.X * multiplyer;
    }
    private static int GetHeight(Vector2I viewportSize, int multiplyer)
    {
        return viewportSize.Y * multiplyer;
    }


    private void InitializeAudioControls()
    {
        bool result = true;
        result = result && SetupAudioControl(_masterControl, _masterBusName, _masterBusDisplayName);
        result = result && SetupAudioControl(_musicControl, _musicBusName, _musicBusDisplayName);
        result = result && SetupAudioControl(_sfxControl, _sfxBusName, _sfxBusDisplayName);

        if (!result)
        {
            GD.PrintErr("Error while initializing Audio Controls");
        }
    }

    private bool SetupAudioControl(UIAudioControl uiAudio, string busName, string displayName)
    {
        int index = AudioServer.GetBusIndex(busName);
        if (index >= 0)
        {
            float volume = AudioServer.GetBusVolumeDb(index);
            uiAudio.Setup(busName, displayName, volume);
        }

        return index >= 0;
    }

    private void OnVolumeChanged(string busName, float decibel)
    {
        int index = AudioServer.GetBusIndex(busName);
        if (index >= 0)
        {
            AudioServer.SetBusVolumeDb(index, decibel);
        }
    }

    private void OnFullscreenToggled(bool toggle)
    {
        _windowSizeOption.Disabled = toggle;

        // if (toggle)
        // {
        //     DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        //     DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
        // }
        // else
        // {
        //     DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        //     DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
        // }
    }

    // private void OnWindowResolutionSelected(long index)
    // {
    //     if (index < 0 || index > 2)
    //     {
    //         GD.PrintErr($"Something is worng with resolution selection. Index it gives is {index}");
    //     }
    //     switch(index)
    //     {
    //         case 0:
    //             DisplayServer.WindowSetSize(Config.Window640);
    //             break;
    //         case 1:
    //             DisplayServer.WindowSetSize(Config.Window1280);
    //             break;
    //         case 2:
    //             DisplayServer.WindowSetSize(Config.Window1920);
    //             break;
    //     }
    // }

    private void ApplySettings()
    {
        // Set resolution
        int resolutionIndex = _windowSizeOption.Selected;
        if (resolutionIndex < 0)
        {
            // nothing selected
            return;
        }

        Resolution resolution = _resolutions[resolutionIndex];
        DisplayServer.WindowSetSize((Vector2I)resolution);

        // Set window mode
        DisplayServer.WindowMode windowMode = _fullscreenCheck.ButtonPressed
            ? DisplayServer.WindowMode.Fullscreen
            : DisplayServer.WindowMode.Windowed;

        DisplayServer.WindowSetMode(windowMode);
    }

    private void OnClose()
    {
        ApplySettings();

        GameManager.Instance.ActivatePreviousState();
    }

    private void OnMenu()
    {
        GameManager.Instance.ChangeState(StateType.MainMenu);
    }

}
