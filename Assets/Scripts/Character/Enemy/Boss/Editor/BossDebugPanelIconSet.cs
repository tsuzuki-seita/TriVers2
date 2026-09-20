using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BossDebugPanelIconSet", menuName = "Debug/Boss Debug Panel Icon Set")]
public class BossDebugPanelIconSet : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public string BehaviorName;
        public Texture2D Icon;
    }

    [SerializeField] private Entry[] _entries;

    public Texture2D GetIcon(string behaviorName)
    {
        if (_entries == null) return null;

        foreach (Entry entry in _entries)
        {
            if (entry.BehaviorName == behaviorName) return entry.Icon;
        }
        return null;
    }
}
