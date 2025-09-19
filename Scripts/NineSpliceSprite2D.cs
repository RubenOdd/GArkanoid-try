// (c) 2025 Sardorbek Mukhudinov <sardorbekmukhudinov@gmail.com>
// License 3-clause BSD License
using Godot;
using System;

namespace GA.GArkanoid
{
    [Tool]
    /// <summary>
    /// A helper class to update 9-slice sprite shader parameters
    /// </summary>
    public partial class NineSpliceSprite2D : Sprite2D
    {
        /// <summary>
        /// Updates the node every time transform changes (globally)
        /// </summary>
        public override void _Ready()
        {
            SetNotifyTransform(enable: true);
        }

        public override void _Notification(int what)
        {
            if (what == NotificationTransformChanged)
            {
                Material material = GetMaterial();
                material?.Set("shader_parameter/scale", GlobalScale);
            }
        }
    }
}