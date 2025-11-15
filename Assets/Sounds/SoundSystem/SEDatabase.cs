using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoundSystem
{
    [System.Serializable]
    public class SEEntry
    {
        public string seName;
        public AudioClip audioClip;
        [Range(0f, 2f)] public float volumeMultiplier = 1f;
    }

    [CreateAssetMenu(menuName = "Sound/SE Database")]
    public class SEDatabase : ScriptableObject
    {
        public List<SEEntry> seList;

        public SEEntry GetEntryByName(string seName)
        {
            return seList.FirstOrDefault(se => se.seName == seName);
        }

        public AudioClip GetClipByName(string seName)
        {
            return GetEntryByName(seName)?.audioClip;
        }
    }
}
