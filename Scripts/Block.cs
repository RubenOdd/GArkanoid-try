// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using System;
using GA.Common;
using GA.GArkanoid.Systems;
using Godot;

namespace GA.GArkanoid;
[Tool]
public partial class Block : StaticBody2D
{
    [Signal] public delegate void BlockDestroyedEventHandler(Block block);

    [Export] private Color _blockColor = Colors.White;

    [Export] private int _score = 1;
    [Export] private PowerUpType _guaranteedPowerUp = PowerUpType.None;

    private bool _isEnabled = true;

    public bool IsEnabled
    {
        get { return _isEnabled; }
        set
        {
            _isEnabled = value;
            _sprite.Visible = value;
            _collisionShape.Disabled = !value;
        }
    }

    public PowerUpType GuaranteedPowerUp => _guaranteedPowerUp;

    [Export] public string GUID { get; private set; } = null;

    private Sprite2D _sprite = null;
    private CollisionShape2D _collisionShape = null;

    public override void _EnterTree()
    {
        // TODO: To make sure all ID's are unique and not copied from the patent scene, create an
        // editor tool to ensure GUID uniqueness!
        if (Engine.IsEditorHint())
        {
            if (string.IsNullOrWhiteSpace(GUID))
            {
                SetGUID(Guid.NewGuid().ToString());
                GD.Print($"Created a new GUID for the object {this.Name}");
            }
        }
    }

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        if (_sprite != null)
        {
            _sprite.Modulate = _blockColor;
        }

        _collisionShape = this.GetNode<CollisionShape2D>();
    }

    public void Hit()
    {
        IsEnabled = false;

        // Access GameManager singleton using its Instance property.
        GameManager.Instance.AddScore(_score);

        EmitSignal(SignalName.BlockDestroyed, this);
    }

    public void SetGUID(string guid)
    {
        GUID = guid;
        NotifyPropertyListChanged();
    }
}