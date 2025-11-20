using Godot;
using System;

namespace GA.GArkanoid;

public partial class Effect : Node2D
{
    [Export] public GpuParticles2D ParticleEffect { get; private set; }
    [Export] public AudioStreamPlayer Audio { get; private set; }
    private int _effectCount = 0;

    // TODO: Add audio effect

    public void Play()
    {
        if (ParticleEffect != null)
        {
            ParticleEffect.OneShot = true;
            ParticleEffect.Emitting = true;

            // TODO: handle destroying
            ParticleEffect.Finished += OnParticleEffectFinished;
        }

        if (Audio != null)
        {
            _effectCount++;
            Audio.Play();

            Audio.Finished += OnAudioEffectFinished;
        }
    }

    private void OnAudioEffectFinished()
    {
        Audio.Finished -= OnAudioEffectFinished;
        OnEffectFinished();
    }

    private void OnParticleEffectFinished()
    {
        ParticleEffect.Finished -= OnParticleEffectFinished;
        OnEffectFinished();
    }
    
    private void OnEffectFinished()
    {
        _effectCount--;

        if (_effectCount <= 0)
        {
            QueueFree();
        }
    }
}
