using GA.Common;
using Godot;
using System;
using System.Collections.Generic;

namespace GA.GArkanoid.Editor;

[Tool]
public partial class ResetGUIDs : Node2D
{
    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            EnsureUniqueGUIDs();
        }
    }

    private void EnsureUniqueGUIDs()
    {
        HashSet<string> guids = [];
        IList<Block> blocks = this.GetNodesInChildren<Block>(recursive: true);

        foreach (Block block in blocks)
        {
            if (block == null)
            {
                // Bug, shouldn't happen
                continue;
            }

            string guid = block.GUID;
            if (string.IsNullOrWhiteSpace(guid) || guids.Contains(guid))
            {
                // GUID either does not exist or is a duplicate. Create a new one.
                string newGUID;

                do
                {
                    newGUID = Guid.NewGuid().ToString();
                } while (guids.Contains(newGUID));

                guid = newGUID;
                block.SetGUID(guid);
            }

            guids.Add(guid);
        }
    }
}
