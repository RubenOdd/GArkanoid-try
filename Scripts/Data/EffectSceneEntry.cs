using Godot;

namespace GA.GArkanoid.Data;

[Tool, GlobalClass]
public partial class EffectSceneEntry : Resource
{
    [Export] public EffectType EffectType { get; set; }
    [Export] public PackedScene Scene{ get; set; }
}