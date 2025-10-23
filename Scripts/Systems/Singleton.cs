// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using Godot;

namespace GA.Common.Godot;

/// <summary>
/// A generic class reprecenting a singleton. The where keyword is used to limit what
/// the type T can be.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract partial class Singleton<T> : Node
    where T : Node
{
    public static T Instance { get; private set; }

    public sealed override void _Ready()
    {
        if (Instance == null)
        {
            // The instance doesn't exits yet. Let this be the one and only instance.
            Instance = this as T;
        }
        else if (Instance != this)
        {
            // The instance already exists and we are creating another one. Destroy
            // the newly created instance to make sure, that singleton pattern is followed corrently.
            GD.PrintErr($"A singleton of type {typeof(T).Name} is already created!");
            QueueFree(); // Queues the destruction of this node.
            return;
        }

        Initialize();
    }

    protected virtual void Initialize()
    {
    }
}