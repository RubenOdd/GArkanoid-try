// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using System.Collections.Generic;
using Godot;

namespace GA.GArkanoid.Data;

[Tool, GlobalClass]
public partial class EffectMap : Resource
{
    [Export] private Godot.Collections.Array<EffectSceneEntry> _entries = null;
    private Dictionary<EffectType, PackedScene> _sceneCache = [];

    private void RebuildCache()
    {
        _sceneCache.Clear();
        if (_entries == null || _entries.Count == 0)
        {
            return;
        }

        foreach (EffectSceneEntry entry in _entries)
        {
            if (entry == null)
            {
                GD.PushWarning("_entries array contains null values.");
                continue;
            }

            if (entry.Scene == null)
            {
                GD.PushWarning($"EffectSceneMap: Scene missing for entry type {entry.EffectType}");
                continue;
            }

            if (_sceneCache.ContainsKey(entry.EffectType))
            {
                GD.PushWarning($"EffectSceneMap: Duplicate mapping for {entry.EffectType} (will use last)");
            }

            _sceneCache[entry.EffectType] = entry.Scene;
        }

    }
    
    public PackedScene GetSceneResource(EffectType effectType)
    {
        if (_sceneCache.Count == 0)
        {
            RebuildCache();
        }

        return _sceneCache.TryGetValue(effectType, out PackedScene scene) ? scene : null;
    }
}
