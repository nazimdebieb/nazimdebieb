using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloomGarden.Presentation
{
    public enum GardenSound
    {
        Select,
        Water,
        Bloom,
        Empty,
        Win,
        Deadlock,
        Undo,
        Refused,
    }

    /// <summary>
    /// Small, calm sound set synthesized at startup (no audio files, no license to track). Blooms use a
    /// pentatonic scale so a cascade sounds like a short melody. Volume comes from <see cref="GameSettings"/>.
    /// </summary>
    public sealed class GardenAudio : MonoBehaviour
    {
        private const int SampleRate = 44100;
        private static readonly float[] Pentatonic = { 523.25f, 587.33f, 659.25f, 783.99f, 880.00f, 1046.5f, 1174.7f, 1318.5f };

        private readonly Dictionary<GardenSound, AudioClip> _clips = new Dictionary<GardenSound, AudioClip>();
        private readonly List<AudioClip> _bloomNotes = new List<AudioClip>();
        private AudioSource[] _sources;
        private int _next;
        private GameSettings _settings;

        /// <summary>True when sounds can actually be heard (an enabled AudioListener exists).</summary>
        public bool HasListener => FindAnyObjectByType<AudioListener>() != null;

        /// <summary>Number of sounds started since Init (tests).</summary>
        public int PlayedCount { get; private set; }

        public void Init(GameSettings settings)
        {
            _settings = settings;
            // Safety net: a scene without AudioListener plays every sound silently (bug found on device, 0.4.0).
            if (!HasListener)
            {
                Camera target = Camera.main;
                if (target != null) target.gameObject.AddComponent<AudioListener>();
                else gameObject.AddComponent<AudioListener>();
                Debug.LogWarning("[BloomGarden] No AudioListener in the scene: one was added at runtime.");
            }
            _sources = new AudioSource[6];
            for (int i = 0; i < _sources.Length; i++)
            {
                _sources[i] = gameObject.AddComponent<AudioSource>();
                _sources[i].playOnAwake = false;
                _sources[i].spatialBlend = 0f;
            }
            _clips[GardenSound.Select] = Make("select", 0.09f, t => Sine(Sweep(620f, 420f, t, 0.09f), t) * Env(t, 0.004f, 0.09f) * 0.55f);
            _clips[GardenSound.Water] = Make("water", 0.16f, (t, rng) => Noise(rng) * Env(t, 0.01f, 0.16f) * 0.22f, lowPass: 0.18f);
            _clips[GardenSound.Empty] = Make("empty", 0.12f, t => Sine(Sweep(440f, 660f, t, 0.12f), t) * Env(t, 0.005f, 0.12f) * 0.3f);
            _clips[GardenSound.Undo] = Make("undo", 0.1f, t => Sine(Sweep(420f, 620f, t, 0.1f), t) * Env(t, 0.004f, 0.1f) * 0.45f);
            _clips[GardenSound.Refused] = Make("refused", 0.12f, t => Sine(180f, t) * Env(t, 0.004f, 0.12f) * 0.35f);
            _clips[GardenSound.Deadlock] = Make("deadlock", 0.7f, t =>
                (Bell(330f, t, 0.35f) + Bell(262f, t - 0.25f, 0.45f)) * 0.35f);
            _clips[GardenSound.Win] = Make("win", 1.3f, t =>
                (Bell(523.25f, t, 0.5f) + Bell(659.25f, t - 0.12f, 0.5f) + Bell(783.99f, t - 0.24f, 0.6f) + Bell(1046.5f, t - 0.36f, 0.9f)) * 0.3f);
            foreach (float f in Pentatonic)
                _bloomNotes.Add(Make("bloom" + f, 0.45f, t => Bell(f, t, 0.4f) * 0.4f));
        }

        public void Play(GardenSound sound, float pitch = 1f)
        {
            if (_settings == null || !_settings.SoundOn || !_clips.TryGetValue(sound, out AudioClip clip)) return;
            Emit(clip, pitch);
        }

        /// <summary>Bloom note: successive blooms of one move climb the scale.</summary>
        public void PlayBloom(int indexInCascade)
        {
            if (_settings == null || !_settings.SoundOn) return;
            Emit(_bloomNotes[Mathf.Clamp(indexInCascade, 0, _bloomNotes.Count - 1)], 1f);
        }

        private void Emit(AudioClip clip, float pitch)
        {
            AudioSource source = _sources[_next];
            _next = (_next + 1) % _sources.Length;
            source.pitch = pitch;
            source.volume = _settings.SoundVolume;
            source.PlayOneShot(clip);
            PlayedCount++;
        }

        // ------------------------------------------------------------------ synthesis

        private static AudioClip Make(string name, float seconds, Func<float, float> wave) =>
            Make(name, seconds, (t, _) => wave(t));

        private static AudioClip Make(string name, float seconds, Func<float, System.Random, float> wave, float lowPass = 1f)
        {
            int n = Mathf.CeilToInt(seconds * SampleRate);
            var data = new float[n];
            var rng = new System.Random(name.GetHashCode());
            float smooth = 0f;
            for (int i = 0; i < n; i++)
            {
                float v = wave(i / (float)SampleRate, rng);
                smooth += (v - smooth) * lowPass;
                data[i] = Mathf.Clamp(smooth, -1f, 1f);
            }
            var clip = AudioClip.Create(name, n, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static float Sine(float frequency, float t) => Mathf.Sin(2f * Mathf.PI * frequency * t);

        private static float Sweep(float from, float to, float t, float duration) => Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));

        private static float Noise(System.Random rng) => (float)(rng.NextDouble() * 2.0 - 1.0);

        /// <summary>Attack then linear release to zero at <paramref name="length"/>.</summary>
        private static float Env(float t, float attack, float length) =>
            t < attack ? t / attack : Mathf.Clamp01(1f - (t - attack) / Mathf.Max(0.0001f, length - attack));

        /// <summary>Soft bell: fundamental + octave, quick attack, exponential decay. Silent before t = 0.</summary>
        private static float Bell(float frequency, float t, float decay)
        {
            if (t < 0f) return 0f;
            float env = Mathf.Min(1f, t / 0.005f) * Mathf.Exp(-t / (decay * 0.35f));
            return (Sine(frequency, t) * 0.8f + Sine(frequency * 2f, t) * 0.2f) * env;
        }
    }
}
