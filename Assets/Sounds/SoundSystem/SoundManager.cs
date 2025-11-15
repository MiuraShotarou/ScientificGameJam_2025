using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundSystem
{
    public class SoundManager : MonoBehaviour
    {
        // ★★★ シングルトンインスタンス ★★★
        public static SoundManager Instance { get; private set; }

        [Header("Audio Mixer")]
        public AudioMixer audioMixer;
        public AudioMixerGroup bgmAMG, menuSeAMG, envAMG, voiceAMG;
        public AudioMixer effectAudioMixer;

        [Header("BGM Database")]
        [SerializeField] private BGMDatabase bgmDatabase;

        [Header("SE Database")]
        [SerializeField] private SEDatabase seDatabase;

        [Header("Voice Database")]
        [SerializeField] private VoiceDatabase voiceDatabase;

        [Header("Environment Database")]
        [SerializeField] private EnvDatabase environmentDatabase;

        [Header("Fade Settings")]
        [SerializeField] private FadeCurveType fadeCurveType = FadeCurveType.ConstantPower;

        private AudioSource menuSeAudioSource;
        private AudioSource environmentAudioSource;
        private AudioSource voiceAudioSource;

        private List<AudioSource> bgmAudioSourceList = new List<AudioSource>();
        private const int BGMAudiosourceNum = 2;

        private List<IEnumerator> fadeCoroutines = new List<IEnumerator>();

        public bool IsPaused { get; private set; }

        private const string MasterVolumeParamName = "MasterVolume";
        private const string GameSeVolumeParamName = "GameSEVolume";
        private const string BGMVolumeParamName = "BGMVolume";
        private const string EnvVolumeParamName = "EnvironmentVolume";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.Log($"[SoundManager] シーンに既にSoundManagerのインスタンスが存在します ({Instance.name})。この新しいインスタンス ({gameObject.name}) を破棄します。", gameObject);
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Debug.Log($"[SoundManager] SoundManagerのインスタンスが設定されました: {gameObject.name}", gameObject);

            DontDestroyOnLoad(gameObject);

            menuSeAudioSource = InitializeAudioSource(gameObject, false, menuSeAMG);
            bgmAudioSourceList = InitializeAudioSources(gameObject, true, bgmAMG, BGMAudiosourceNum);
            environmentAudioSource = InitializeAudioSource(gameObject, true, envAMG);
            voiceAudioSource = InitializeAudioSource(gameObject, false, voiceAMG);

            IsPaused = false;
        }

        // ★★★ GetInstance() ★★★
        public static SoundManager GetInstance()
        {
            if (Instance != null)
                return Instance;

            // シーンに手動配置されたSoundManagerを探す
            Instance = FindObjectOfType<SoundManager>();
            if (Instance != null)
            {
                Debug.Log("[SoundManager] シーン上のSoundManagerを使用します。");
                return Instance;
            }

            // Resources からロード
            GameObject prefab = Resources.Load<GameObject>("SoundManager");
            if (prefab != null)
            {
                GameObject obj = Instantiate(prefab);
                obj.name = "SoundManager";
                Instance = obj.GetComponent<SoundManager>();
                DontDestroyOnLoad(obj);
                Debug.Log("[SoundManager] Resources から SoundManager を生成しました。");
                return Instance;
            }

            // 最終手段：空GameObjectを生成
            GameObject fallback = new GameObject("SoundManager (Auto Generated)");
            Instance = fallback.AddComponent<SoundManager>();
            DontDestroyOnLoad(fallback);
            Debug.LogWarning("[SoundManager] Resources に SoundManager.prefab が見つかりません。空の SoundManager を生成しました。Inspector 設定が必要です。");
            return Instance;
        }

        public float MasterVolume
        {
            get => audioMixer.GetVolumeByLinear(MasterVolumeParamName);
            set => audioMixer.SetVolumeByLinear(MasterVolumeParamName, value);
        }

        public float GameSeVolume
        {
            get => audioMixer.GetVolumeByLinear(GameSeVolumeParamName);
            set => audioMixer.SetVolumeByLinear(GameSeVolumeParamName, value);
        }

        public float BGMVolume
        {
            get => audioMixer.GetVolumeByLinear(BGMVolumeParamName);
            set => audioMixer.SetVolumeByLinear(BGMVolumeParamName, value);
        }

        public float EnvironmentVolume
        {
            get => audioMixer.GetVolumeByLinear(EnvVolumeParamName);
            set => audioMixer.SetVolumeByLinear(EnvVolumeParamName, value);
        }

        public void ChangeSnapshot(string snapshotName, float transitionTime = 1f)
        {
            AudioMixerSnapshot snapshot = effectAudioMixer.FindSnapshot(snapshotName);
            if (snapshot == null)
            {
                Debug.Log($"{snapshotName} は見つかりません");
                return;
            }
            snapshot.TransitionTo(transitionTime);
        }

        public void Pause()
        {
            IsPaused = true;
            fadeCoroutines.ForEach(StopCoroutine);
            environmentAudioSource.Pause();
            voiceAudioSource.Pause();
            bgmAudioSourceList.ForEach(bas => bas.Pause());
        }

        public void Resume()
        {
            IsPaused = false;
            environmentAudioSource.UnPause();
            voiceAudioSource.UnPause();
            bgmAudioSourceList.ForEach(bas => bas.UnPause());
        }

        private List<AudioSource> InitializeAudioSources(GameObject parent, bool isLoop, AudioMixerGroup amg, int count)
        {
            var sources = new List<AudioSource>();
            for (int i = 0; i < count; i++)
            {
                sources.Add(InitializeAudioSource(parent, isLoop, amg));
            }
            return sources;
        }

        private AudioSource InitializeAudioSource(GameObject parent, bool isLoop, AudioMixerGroup amg)
        {
            var audioSource = parent.AddComponent<AudioSource>();
            audioSource.loop = isLoop;
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = amg;
            return audioSource;
        }

        public void PlayBGMByName(string bgmName, float fadeTime = 2f)
        {
            if (bgmDatabase == null)
            {
                Debug.LogWarning("BGMDatabase が未設定です");
                return;
            }

            var entry = bgmDatabase.GetEntryByName(bgmName);
            if (entry == null || entry.audioClip == null)
            {
                Debug.LogWarning($"{bgmName} に対応するBGMが見つかりません");
                return;
            }

            if (IsPaused) return;

            if (bgmAudioSourceList.Any(s => s.clip == entry.audioClip && s.isPlaying))
                return;

            StopBGMWithFadeOut(fadeTime);

            var audioSource = bgmAudioSourceList.FirstOrDefault(s => !s.isPlaying);
            if (audioSource != null)
            {
                var routine = audioSource.PlayWithFadeIn(entry.audioClip, fadeTime, entry.volumeMultiplier, fadeCurveType);
                fadeCoroutines.Add(routine);
                StartCoroutine(routine);
            }
            else
            {
                Debug.LogWarning("BGM用の利用可能なAudioSourceが見つかりません。BGMAudiosourceNumを増やしてください。");
            }
        }

        public void StopBGMWithFadeOut(float fadeTime = 2f)
        {
            if (IsPaused) return;

            fadeCoroutines.ForEach(StopCoroutine);
            fadeCoroutines.Clear();

            fadeCoroutines = bgmAudioSourceList
                .Where(s => s.isPlaying)
                .Select(s =>
                {
                    var routine = s.StopWithFadeOut(fadeTime, fadeCurveType);
                    StartCoroutine(routine);
                    return routine;
                }).ToList();
        }

        public void PlaySeByName(string seName)
        {
            if (IsPaused) return;

            var entry = seDatabase?.GetEntryByName(seName);
            if (entry == null || entry.audioClip == null)
            {
                Debug.LogWarning($"{seName} に対応するSEが見つかりません");
                return;
            }

            float volume = Mathf.Clamp01(entry.volumeMultiplier * GameSeVolume);
            menuSeAudioSource?.PlayOneShot(entry.audioClip, volume);
        }

        public void PlayVoiceByName(string voiceName, float delayTime = 0f)
        {
            if (IsPaused) return;

            var entry = voiceDatabase?.GetEntryByName(voiceName);
            if (entry == null || entry.audioClip == null)
            {
                Debug.LogWarning($"{voiceName} に対応するVoiceが見つかりません");
                return;
            }

            if (voiceAudioSource != null)
            {
                voiceAudioSource.clip = entry.audioClip;
                voiceAudioSource.volume = Mathf.Clamp01(entry.volumeMultiplier * GameSeVolume);

                if (delayTime > 0)
                    voiceAudioSource.PlayDelayed(delayTime);
                else
                    voiceAudioSource.Play();
            }
        }

        public void PlayEnvironmentByName(string envName)
        {
            if (IsPaused) return;

            var entry = environmentDatabase?.GetEntryByName(envName);
            if (entry == null || entry.audioClip == null)
            {
                Debug.LogWarning($"{envName} に対応する環境音が見つかりません");
                return;
            }

            if (environmentAudioSource != null)
            {
                environmentAudioSource.volume = Mathf.Clamp01(entry.volumeMultiplier * EnvironmentVolume);
                StartCoroutine(PlayRandomStartCoroutine(environmentAudioSource, entry.audioClip));
            }
        }

        private IEnumerator PlayRandomStartCoroutine(AudioSource source, AudioClip clip)
        {
            source.clip = clip;
            source.time = Random.Range(0f, clip.length);
            source.Play();
            yield break;
        }

        public void StopEnvironment()
        {
            if (IsPaused) return;
            if (environmentAudioSource?.isPlaying == true)
            {
                environmentAudioSource.Stop();
            }
        }
    }
}
