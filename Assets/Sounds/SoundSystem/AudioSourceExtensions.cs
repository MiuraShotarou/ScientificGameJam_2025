// AudioSourceExtensions.cs（切り替え可能なフェード方式対応）
using System.Collections;
using UnityEngine;

namespace SoundSystem
{
    public enum FadeCurveType
    {
        Linear,
        ConstantPower
    }

    public static class AudioSourceExtensions
    {
        public static void Play(this AudioSource audioSource, AudioClip audioClip = null, float volume = 1f)
        {
            if (audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.volume = Mathf.Clamp01(volume);
                audioSource.Play();
            }
        }

        public static IEnumerator PlayRandomStart(this AudioSource audioSource, AudioClip audioClip, float volume = 1f)
        {
            if (audioClip == null) yield break;

            audioSource.clip = audioClip;
            audioSource.volume = Mathf.Clamp01(volume);
            audioSource.time = UnityEngine.Random.Range(0f, audioClip.length - 0.01f);

            yield return PlayWithFadeIn(audioSource, audioClip, 0.1f, volume);
        }

        public static IEnumerator PlayWithFadeIn(this AudioSource audioSource, AudioClip audioClip, float fadeTime = 0.1f, float endVolume = 1.0f, FadeCurveType curveType = FadeCurveType.Linear)
        {
            float targetVolume = Mathf.Clamp01(endVolume);
            fadeTime = fadeTime < 0.1f ? 0.1f : fadeTime;
            audioSource.Play(audioClip, 0f);

            for (float t = 0f; t < fadeTime; t += Time.deltaTime)
            {
                float nt = Mathf.Clamp01(t / fadeTime);
                audioSource.volume = Mathf.Lerp(0f, targetVolume, GetFadeValue(nt, curveType));
                yield return null;
            }
            audioSource.volume = targetVolume;
        }

        public static IEnumerator StopWithFadeOut(this AudioSource audioSource, float fadeTime = 0.1f, FadeCurveType curveType = FadeCurveType.Linear)
        {
            float startVolume = audioSource.volume;
            fadeTime = fadeTime < 0.1f ? 0.1f : fadeTime;

            for (float t = 0f; t < fadeTime; t += Time.deltaTime)
            {
                float nt = Mathf.Clamp01(t / fadeTime);
                audioSource.volume = Mathf.Lerp(startVolume, 0f, GetFadeValue(nt, curveType));
                yield return null;
            }

            audioSource.volume = 0f;
            audioSource.Stop();
            audioSource.clip = null;
        }

        private static float GetFadeValue(float t, FadeCurveType type)
        {
            switch (type)
            {
                case FadeCurveType.Linear:
                    return t;
                case FadeCurveType.ConstantPower:
                    return Mathf.Sin(t * Mathf.PI / 2f);
                default:
                    return t;
            }
        }
    }
}
