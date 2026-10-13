using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BloomGarden.Presentation
{
    public enum GraphicsMode
    {
        /// <summary>Starts with high quality and drops to economy if the phone cannot keep a smooth frame rate.</summary>
        Auto = 0,
        High = 1,
        /// <summary>No shadows, no MSAA, fewer particles: for entry-level phones (P7-04).</summary>
        Economy = 2,
    }

    /// <summary>
    /// Applies the graphics level per camera and light (no change to the URP asset, so the editor project is never
    /// modified at runtime), sets the target frame rate (Android defaults to 30 fps), and measures frame times
    /// for the automatic mode and the playtest log (P7-03). Never touches game rules.
    /// </summary>
    public sealed class GraphicsQuality : MonoBehaviour
    {
        public const int TargetFps = 60;
        /// <summary>Auto mode switches to economy when the average stays below this for <see cref="AutoWindow"/> seconds.</summary>
        public const float AutoMinFps = 40f;
        public const float AutoWindow = 10f;
        private const float WarmUp = 5f;

        private Camera _camera;
        private Light _light;
        private GameSettings _settings;
        private readonly Queue<float> _frames = new Queue<float>();
        private float _slowSince = -1f, _started;

        /// <summary>Effective level after applying the Auto rule.</summary>
        public bool Economy { get; private set; }
        public bool AutoDowngraded { get; private set; }

        public event System.Action<string> AutoSwitched;

        public void Init(Camera camera, GameSettings settings)
        {
            _camera = camera;
            _light = FindAnyObjectByType<Light>();
            _settings = settings;
            _started = Time.realtimeSinceStartup;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = TargetFps;
            settings.Changed += Apply;
            Apply();
        }

        public void Apply()
        {
            Economy = _settings.Graphics == GraphicsMode.Economy || (_settings.Graphics == GraphicsMode.Auto && AutoDowngraded);
            if (_camera != null)
            {
                _camera.allowMSAA = !Economy;
                var data = _camera.GetUniversalAdditionalCameraData();
                if (data != null) data.renderShadows = !Economy;
            }
            if (_light != null) _light.shadows = Economy ? LightShadows.None : LightShadows.Soft;
        }

        private void Update()
        {
            float dt = Time.unscaledDeltaTime;
            _frames.Enqueue(dt);
            while (_frames.Count > 120) _frames.Dequeue();

            // The editor's frame rate (often throttled when unfocused) says nothing about a phone: no automatic switch there.
            if (_settings.Graphics != GraphicsMode.Auto || AutoDowngraded || Application.isEditor) return;
            float now = Time.realtimeSinceStartup;
            if (now - _started < WarmUp || _frames.Count < 60) return;
            if (AverageFps < AutoMinFps)
            {
                if (_slowSince < 0f) _slowSince = now;
                else if (now - _slowSince > AutoWindow) ForceAutoDowngrade();
            }
            else _slowSince = -1f;
        }

        /// <summary>Auto mode gave up on high quality (also callable from tests).</summary>
        public void ForceAutoDowngrade()
        {
            if (AutoDowngraded) return;
            AutoDowngraded = true;
            Apply();
            AutoSwitched?.Invoke("economy");
        }

        public float AverageFps
        {
            get
            {
                if (_frames.Count == 0) return 0f;
                float sum = 0f;
                foreach (float f in _frames) sum += f;
                return sum > 0f ? _frames.Count / sum : 0f;
            }
        }

        public float FrameP95Ms
        {
            get
            {
                if (_frames.Count == 0) return 0f;
                var sorted = new List<float>(_frames);
                sorted.Sort();
                return sorted[Mathf.Min(sorted.Count - 1, (int)(sorted.Count * 0.95f))] * 1000f;
            }
        }

        public static int ManagedMemoryMb => (int)(System.GC.GetTotalMemory(false) / (1024 * 1024));

        /// <summary>Total memory used by the app as seen by Unity (0 when the platform does not report it).</summary>
        public static int AllocatedMemoryMb => (int)(UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024));

        /// <summary>Device RAM rounded to whole gigabytes: enough to compare phone classes, not to identify a phone.</summary>
        public static int DeviceRamGb => Mathf.RoundToInt(SystemInfo.systemMemorySize / 1024f);
    }
}
