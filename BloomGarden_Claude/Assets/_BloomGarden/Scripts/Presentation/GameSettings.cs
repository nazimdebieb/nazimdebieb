using System;
using UnityEngine;

namespace BloomGarden.Presentation
{
    /// <summary>
    /// Player preferences that only affect presentation (GDD chapter 04: speed normal/fast, reduced motion; sound;
    /// language). None of them can change a game result: the logic is resolved before any animation or sound.
    /// Backed by the versioned save (<see cref="SettingsData"/>); <see cref="Changed"/> tells the owner to save.
    /// </summary>
    public sealed class GameSettings
    {
        private readonly SettingsData _data;

        public GameSettings(SettingsData data)
        {
            _data = data ?? new SettingsData();
            Loc.SetLanguage(Language);
        }

        public float SoundVolume => _data.volume;
        public bool FastAnimations => _data.fast;
        public bool ReducedMotion => _data.reduced;
        public Language Language => Loc.FromCode(_data.language, Loc.DefaultForDevice());

        public bool SoundOn => SoundVolume > 0.001f;

        public GraphicsMode Graphics =>
            _data.graphics == "high" ? GraphicsMode.High : _data.graphics == "economy" ? GraphicsMode.Economy : GraphicsMode.Auto;

        public void SetGraphics(GraphicsMode mode)
        {
            _data.graphics = mode == GraphicsMode.High ? "high" : mode == GraphicsMode.Economy ? "economy" : "auto";
            Notify("graphics", _data.graphics);
        }

        /// <summary>Multiplier applied to every animation duration (1 = normal, 0.5 = fast).</summary>
        public float DurationScale => FastAnimations ? 0.5f : 1f;

        public event Action Changed;

        /// <summary>Which setting changed and its new value (playtest event <c>settings_changed</c>).</summary>
        public event Action<string, string> SettingChanged;

        public void SetSoundVolume(float volume) { _data.volume = Mathf.Clamp01(volume); Notify("sound", SoundOn ? "on" : "off"); }
        public void SetFastAnimations(bool fast) { _data.fast = fast; Notify("speed", fast ? "fast" : "normal"); }
        public void SetReducedMotion(bool reduced) { _data.reduced = reduced; Notify("reduced_motion", reduced ? "on" : "off"); }

        public void SetLanguage(Language language)
        {
            _data.language = Loc.Code(language);
            Loc.SetLanguage(language);
            Notify("language", Loc.Code(language));
        }

        private void Notify(string name, string value)
        {
            Changed?.Invoke();
            SettingChanged?.Invoke(name, value);
        }
    }
}
