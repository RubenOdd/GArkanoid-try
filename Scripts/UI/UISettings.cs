// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

// Used a little help from this video for resolution settings: https://youtu.be/YsdkcPV0BAo?si=pAw6lTV-BBnG2lGp

using GA.GArkanoid.States;
using GA.GArkanoid.Systems;
using Godot;
using System;
using System.Text.RegularExpressions;

namespace GA.GArkanoid.UI;
public partial class UISettings : Control
{
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
    [Export] private CheckBox _fullscreenToggle = null;

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

        _windowSizeOption.ItemSelected += OnWindowResolutionSelected;
        _fullscreenToggle.Toggled += OnWindowModeSelected;
    }

    public override void _ExitTree()
    {
        _closeButton.Pressed -= OnClose;

        _masterControl.VolumeChanged -= OnVolumeChanged;
        _musicControl.VolumeChanged -= OnVolumeChanged;
        _sfxControl.VolumeChanged -= OnVolumeChanged;

        _windowSizeOption.ItemSelected -= OnWindowResolutionSelected;
        _fullscreenToggle.Toggled -= OnWindowModeSelected;
    }

    private void InitializeVideoControl()
    {
        throw new NotImplementedException();
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

    private void OnWindowModeSelected(bool toggle)
    {
        if (toggle)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
            DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
        }
        else
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
        }
    }

    private void OnWindowResolutionSelected(long index)
    {
        if (index < 0 || index > 2)
        {
            GD.PrintErr($"Something is worng with resolution selection. Index it gives is {index}");
        }
        switch(index)
        {
            case 0:
                DisplayServer.WindowSetSize(Config.Window640);
                break;
            case 1:
                DisplayServer.WindowSetSize(Config.Window1280);
                break;
            case 2:
                DisplayServer.WindowSetSize(Config.Window1920);
                break;
        }
    }

    private void OnClose()
    {
        GameManager.Instance.ActivatePreviousState();
    }

    private void OnMenu()
    {
        GameManager.Instance.ChangeState(StateType.MainMenu);
    }

}
