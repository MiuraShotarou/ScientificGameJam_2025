using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoundSystem
{
    [System.Serializable]
    public class EnvironmentEntry
    {
        public string envName;
        public AudioClip audioClip;
        [Range(0f, 2f)] public float volumeMultiplier = 1f;
    }

    [CreateAssetMenu(menuName = "Sound/Environment Database")]
    public class EnvDatabase : ScriptableObject
    {
        public List<EnvironmentEntry> envList;

        public EnvironmentEntry GetEntryByName(string envName)
        {
            return envList.FirstOrDefault(e => e.envName == envName);
        }

        public AudioClip GetClipByName(string envName)
        {
            return GetEntryByName(envName)?.audioClip;
        }
    }
}
