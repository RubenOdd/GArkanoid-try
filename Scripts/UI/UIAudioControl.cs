// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using Godot;
using System;

namespace GA.GArkanoid.UI;
public partial class UIAudioControl : HBoxContainer
{
    [Signal] public delegate void VolumeChangedEventHandler(string busName, float decibel);


    [Export] private Label _nameLabel = null;
    [Export] private Slider _volumeSlider = null;
    [Export] private Label _valueLabel = null;
    private string _busName;

    public override void _EnterTree()
    {
        _volumeSlider.ValueChanged += OnVolumeChanged;
    }

    public override void _ExitTree()
    {
        _volumeSlider.ValueChanged -= OnVolumeChanged;
    }

    public void Setup(string busName, string displayName, float dbVolume)
    {
        _busName = busName;
        _nameLabel.Text = displayName;
        SetVolume(dbVolume);
    }

    public void SetVolume(float dbVolume)
    {
        float linear = Mathf.DbToLinear(dbVolume);
        _volumeSlider.Value = linear;
    }

    private void OnVolumeChanged(double value)
    {
        UpdateVolume();
    }

    private void UpdateVolume()
    {
        float linear = (float)_volumeSlider.Value;

        float dbVolume = Mathf.LinearToDb(linear);

        _valueLabel.Text = ((int) (linear * 100)).ToString();

        EmitSignal(SignalName.VolumeChanged, _busName, dbVolume);
    }
}
