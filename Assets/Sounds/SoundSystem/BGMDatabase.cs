using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoundSystem
{
    [System.Serializable]
    public class BGMEntry
    {
        public string bgmName;
        public AudioClip audioClip;
        [Range(0f, 2f)] public float volumeMultiplier = 1f;
    }

    [CreateAssetMenu(menuName = "Sound/BGM Database")]
    public class BGMDatabase : ScriptableObject
    {
        public List<BGMEntry> bgmList;

        public BGMEntry GetEntryByName(string bgmName)
        {
            return bgmList.FirstOrDefault(b => b.bgmName == bgmName);
        }

        public AudioClip GetClipByName(string bgmName)
        {
            return GetEntryByName(bgmName)?.audioClip;
        }
    }
}
