using System;
using UnityEngine;

namespace BloomGarden.Presentation
{
    /// <summary>
    /// Player preferences that only affect presentation (GDD chapter 04: speed normal/fast, reduced motion; sound).
    /// None of them can change a game result: the logic is resolved before any animation or sound.
    /// Stored in PlayerPrefs for now; the versioned save of P5 will take over.
    /// </summary>
    public sealed class GameSettings
    {
        private const string SoundKey = "bg.sound.volume";
        private const string FastKey = "bg.anim.fast";
        private const string ReducedKey = "bg.anim.reduced";

        public float SoundVolume { get; private set; } = 0.8f;
        public bool FastAnimations { get; private set; }
        public bool ReducedMotion { get; private set; }

        public bool SoundOn => SoundVolume > 0.001f;

        /// <summary>Multiplier applied to every animation duration (1 = normal, 0.5 = fast).</summary>
        public float DurationScale => FastAnimations ? 0.5f : 1f;

        public event Action Changed;

        public static GameSettings Load()
        {
            return new GameSettings
            {
                SoundVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(SoundKey, 0.8f)),
                FastAnimations = PlayerPrefs.GetInt(FastKey, 0) == 1,
                ReducedMotion = PlayerPrefs.GetInt(ReducedKey, 0) == 1,
            };
        }

        public void SetSoundVolume(float volume) { SoundVolume = Mathf.Clamp01(volume); Save(); }
        public void SetFastAnimations(bool fast) { FastAnimations = fast; Save(); }
        public void SetReducedMotion(bool reduced) { ReducedMotion = reduced; Save(); }

        private void Save()
        {
            PlayerPrefs.SetFloat(SoundKey, SoundVolume);
            PlayerPrefs.SetInt(FastKey, FastAnimations ? 1 : 0);
            PlayerPrefs.SetInt(ReducedKey, ReducedMotion ? 1 : 0);
            PlayerPrefs.Save();
            Changed?.Invoke();
        }
    }
}
