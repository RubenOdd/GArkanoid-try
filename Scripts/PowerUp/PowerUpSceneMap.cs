using Godot;

namespace GA.GArkanoid;

[GlobalClass]
public partial class PowerUpSceneMap : Resource
{
    [Export] public PowerUpType PowerUpType {get; set;} = PowerUpType.None;
    [Export] public PackedScene PowerUpScene {get; set;} = null;    
}
