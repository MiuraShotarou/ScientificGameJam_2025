using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoundSystem
{
    [System.Serializable]
    public class VoiceEntry
    {
        public string voiceName;
        public AudioClip audioClip;
        [Range(0f, 2f)] public float volumeMultiplier = 1f;
    }

    [CreateAssetMenu(menuName = "Sound/Voice Database")]
    public class VoiceDatabase : ScriptableObject
    {
        public List<VoiceEntry> voiceList;

        public VoiceEntry GetEntryByName(string voiceName)
        {
            return voiceList.FirstOrDefault(v => v.voiceName == voiceName);
        }

        public AudioClip GetClipByName(string voiceName)
        {
            return GetEntryByName(voiceName)?.audioClip;
        }
    }
}
