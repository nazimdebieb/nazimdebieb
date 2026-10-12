using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BloomGarden.Presentation
{
    /// <summary>Content of the end-of-level panel, kept as localization keys so a language change re-renders it.</summary>
    public readonly struct PanelContent
    {
        public readonly string TitleKey, BodyKey, PrimaryKey, SecondaryKey;
        public readonly object[] BodyArgs;

        public PanelContent(string titleKey, string bodyKey, object[] bodyArgs, string primaryKey, string secondaryKey)
        {
            TitleKey = titleKey;
            BodyKey = bodyKey;
            BodyArgs = bodyArgs;
            PrimaryKey = primaryKey;
            SecondaryKey = secondaryKey;
        }
    }

    /// <summary>
    /// Portrait HUD built at runtime with uGUI (GDD screen C): level and bud counter at the top, Menu and Options,
    /// Undo / Restart at the bottom, end-of-level panel, Options panel and the tutorial bubble with its arrow.
    /// All texts come from <see cref="Loc"/> and are re-rendered when the language changes.
    /// </summary>
    public sealed class HudView : MonoBehaviour
    {
        private UiKit _ui;
        private RectTransform _safe, _gameLayer, _arrow;
        private Text _title, _counter, _panelTitle, _panelBody, _hintText;
        private Text _undoLabel, _restartLabel, _menuLabel, _optionsLabel, _primaryLabel, _secondaryLabel;
        private Text _soundLabel, _speedLabel, _motionLabel, _languageLabel, _settingsTitle, _closeLabel;
        private Button _undo, _panelSecondary;
        private GameObject _panel, _settingsPanel, _hint;
        private GameSettings _settings;

        private int _levelNumber, _bloomed, _total;
        private PanelContent _panelContent;
        private TutorialHint _hintContent;
        private Func<int, Vector2> _laneScreenPosition;

        public UiKit Ui => _ui;
        public RectTransform Safe => _safe;
        public bool PanelVisible => _panel.activeSelf;
        public bool SettingsVisible => _settingsPanel != null && _settingsPanel.activeSelf;
        public bool HintVisible => _hint.activeSelf;
        public string HintKey => _hint.activeSelf ? _hintContent.Key : null;
        public bool ArrowVisible => _arrow.gameObject.activeSelf;

        public event Action UndoClicked, RestartClicked, MenuClicked, PanelPrimaryClicked, PanelSecondaryClicked;

        public void Build(GameSettings settings, Func<int, Vector2> laneScreenPosition)
        {
            _settings = settings;
            _laneScreenPosition = laneScreenPosition;
            _ui = new UiKit();

            if (FindAnyObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                es.transform.SetParent(transform, false);
            }

            var canvasGo = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);
            canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            _safe = UiKit.Stretch(canvasGo.transform, "Safe area");
            _safe.gameObject.AddComponent<SafeArea>();
            _gameLayer = UiKit.Stretch(_safe, "Game layer");

            var menu = _ui.Button(_gameLayer, "Menu", UiKit.Paper, 36, out _menuLabel);
            UiKit.At((RectTransform)menu.transform, 0f, 1f, new Vector2(130f, -50f), new Vector2(220f, 80f));
            menu.onClick.AddListener(() => MenuClicked?.Invoke());

            _title = _ui.Label(_gameLayer, "Title", 64, FontStyle.Bold, TextAnchor.MiddleCenter);
            UiKit.Place(_title.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -150f), new Vector2(-80f, 90f));
            _counter = _ui.Label(_gameLayer, "Counter", 44, FontStyle.Normal, TextAnchor.MiddleCenter);
            UiKit.Place(_counter.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -232f), new Vector2(-80f, 60f));

            _undo = _ui.Button(_gameLayer, "Undo", UiKit.Paper, 46, out _undoLabel);
            UiKit.At((RectTransform)_undo.transform, 0.27f, 0f, new Vector2(0f, 110f), new Vector2(420f, 130f));
            _undo.onClick.AddListener(() => UndoClicked?.Invoke());
            var restart = _ui.Button(_gameLayer, "Restart", UiKit.Paper, 46, out _restartLabel);
            UiKit.At((RectTransform)restart.transform, 0.73f, 0f, new Vector2(0f, 110f), new Vector2(420f, 130f));
            restart.onClick.AddListener(() => RestartClicked?.Invoke());

            BuildHint();
            BuildEndPanel();
            BuildSettings();

            _settings.Changed += ApplyTexts;
            Loc.Changed += ApplyTexts;
            ApplyTexts();
        }

        private void OnDestroy() => Loc.Changed -= ApplyTexts;

        private void BuildHint()
        {
            var bubble = UiKit.Panel(_gameLayer, "Tutorial hint", new Color(1f, 1f, 1f, 0.94f));
            _hint = bubble.gameObject;
            // Low on the screen, above Undo / Restart: it may cover queued cans (not tappable) but never a flower or a lane head.
            UiKit.Place(bubble.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 285f), new Vector2(980f, 140f));
            bubble.raycastTarget = false;
            _hintText = _ui.Label(bubble.transform, "Text", 42, FontStyle.Bold, TextAnchor.MiddleCenter);
            UiKit.Place(_hintText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(-50f, -20f));
            _hint.SetActive(false);

            // Chevron "^" under the lane head, bouncing up towards it.
            _arrow = new GameObject("Tutorial arrow", typeof(RectTransform)).GetComponent<RectTransform>();
            _arrow.SetParent(_gameLayer, false);
            _arrow.sizeDelta = new Vector2(120f, 80f);
            foreach (float side in new[] { -1f, 1f })
            {
                var bar = UiKit.Panel(_arrow, "Bar", UiKit.Ink);
                bar.raycastTarget = false;
                UiKit.At(bar.rectTransform, 0.5f, 0.5f, new Vector2(side * 22f, 0f), new Vector2(22f, 78f));
                bar.rectTransform.localRotation = Quaternion.Euler(0f, 0f, side * 38f);
            }
            _arrow.gameObject.SetActive(false);
        }

        private void BuildEndPanel()
        {
            var panel = UiKit.Panel(_safe, "End panel", new Color(UiKit.Ivory.r, UiKit.Ivory.g, UiKit.Ivory.b, 0.96f));
            _panel = panel.gameObject;
            UiKit.At(panel.rectTransform, 0.5f, 0.5f, new Vector2(0f, -420f), new Vector2(900f, 660f));
            _panelTitle = _ui.Label(panel.transform, "Title", 70, FontStyle.Bold, TextAnchor.MiddleCenter);
            UiKit.Place(_panelTitle.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -100f), new Vector2(-60f, 110f));
            _panelBody = _ui.Label(panel.transform, "Body", 42, FontStyle.Normal, TextAnchor.MiddleCenter);
            UiKit.Place(_panelBody.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -235f), new Vector2(-80f, 130f));
            var primary = _ui.Button(panel.transform, "Primary", UiKit.Leaf, 46, out _primaryLabel);
            UiKit.At((RectTransform)primary.transform, 0.5f, 0f, new Vector2(0f, 230f), new Vector2(680f, 130f));
            primary.onClick.AddListener(() => PanelPrimaryClicked?.Invoke());
            _panelSecondary = _ui.Button(panel.transform, "Secondary", UiKit.Paper, 44, out _secondaryLabel);
            UiKit.At((RectTransform)_panelSecondary.transform, 0.5f, 0f, new Vector2(0f, 85f), new Vector2(680f, 110f));
            _panelSecondary.onClick.AddListener(() => PanelSecondaryClicked?.Invoke());
            _panel.SetActive(false);
        }

        private void BuildSettings()
        {
            var open = _ui.Button(_gameLayer, "Options", UiKit.Paper, 36, out _optionsLabel);
            UiKit.At((RectTransform)open.transform, 1f, 1f, new Vector2(-130f, -50f), new Vector2(220f, 80f));
            open.onClick.AddListener(ToggleSettings);

            var panel = UiKit.Panel(_safe, "Settings panel", new Color(UiKit.Ivory.r, UiKit.Ivory.g, UiKit.Ivory.b, 0.98f));
            _settingsPanel = panel.gameObject;
            UiKit.At(panel.rectTransform, 0.5f, 0.5f, new Vector2(0f, 60f), new Vector2(900f, 900f));
            _settingsTitle = _ui.Label(panel.transform, "Title", 64, FontStyle.Bold, TextAnchor.MiddleCenter);
            UiKit.Place(_settingsTitle.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -90f), new Vector2(-60f, 100f));

            float y = -210f;
            Button Row(string name, out Text label, Action onClick)
            {
                var b = _ui.Button(panel.transform, name, UiKit.Paper, 44, out label);
                UiKit.At((RectTransform)b.transform, 0.5f, 1f, new Vector2(0f, y), new Vector2(760f, 110f));
                b.onClick.AddListener(() => onClick());
                y -= 130f;
                return b;
            }
            Row("Sound", out _soundLabel, () => _settings.SetSoundVolume(_settings.SoundOn ? 0f : 0.8f));
            Row("Speed", out _speedLabel, () => _settings.SetFastAnimations(!_settings.FastAnimations));
            Row("Motion", out _motionLabel, () => _settings.SetReducedMotion(!_settings.ReducedMotion));
            Row("Language", out _languageLabel, () => _settings.SetLanguage(_settings.Language == Language.French ? Language.English : Language.French));

            var close = _ui.Button(panel.transform, "Close", UiKit.Leaf, 46, out _closeLabel);
            UiKit.At((RectTransform)close.transform, 0.5f, 0f, new Vector2(0f, 90f), new Vector2(480f, 110f));
            close.onClick.AddListener(() => _settingsPanel.SetActive(false));
            _settingsPanel.SetActive(false);
        }

        public void ToggleSettings()
        {
            _settingsPanel.SetActive(!_settingsPanel.activeSelf);
            _settingsPanel.transform.SetAsLastSibling();
        }

        /// <summary>Shows or hides the in-game layer (hidden behind the home and level screens).</summary>
        public void SetGameLayerVisible(bool visible)
        {
            _gameLayer.gameObject.SetActive(visible);
            if (!visible) HidePanel();
        }

        // ------------------------------------------------------------------ dynamic content

        public void SetLevel(int levelNumber) { _levelNumber = levelNumber; ApplyTexts(); }

        public void SetCounter(int bloomed, int total)
        {
            _bloomed = bloomed;
            _total = total;
            _counter.text = Loc.T("counter", bloomed, total);
        }

        public void SetUndoInteractable(bool value) => _undo.interactable = value;

        public void ShowPanel(PanelContent content)
        {
            _panelContent = content;
            _panel.SetActive(true);
            ApplyTexts();
        }

        public void HidePanel() => _panel.SetActive(false);

        public void ShowHint(TutorialHint hint)
        {
            _hintContent = hint;
            _hint.SetActive(!hint.IsNone);
            _arrow.gameObject.SetActive(!hint.IsNone && hint.Lane >= 0);
            ApplyTexts();
        }

        /// <summary>The arrow bounces above the lane head it points at.</summary>
        private void Update()
        {
            if (!_arrow.gameObject.activeSelf || _laneScreenPosition == null) return;
            Vector2 screen = _laneScreenPosition(_hintContent.Lane);
            var canvasRt = (RectTransform)_arrow.parent;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRt, screen, null, out Vector2 local)) return;
            float bounce = (_settings != null && _settings.ReducedMotion) ? 0f : Mathf.Abs(Mathf.Sin(Time.time * 4f)) * 30f;
            _arrow.anchorMin = _arrow.anchorMax = new Vector2(0.5f, 0.5f);
            _arrow.anchoredPosition = local - canvasRt.rect.center + new Vector2(0f, -150f + bounce);
        }

        private void ApplyTexts()
        {
            if (_ui == null) return;
            _menuLabel.text = Loc.T("menu");
            _optionsLabel.text = Loc.T("options");
            _title.text = Loc.T("level", _levelNumber);
            _counter.text = Loc.T("counter", _bloomed, _total);
            _undoLabel.text = Loc.T("undo");
            _restartLabel.text = Loc.T("restart");

            if (_panelContent.TitleKey != null)
            {
                _panelTitle.text = Loc.T(_panelContent.TitleKey);
                _panelBody.text = Loc.T(_panelContent.BodyKey, _panelContent.BodyArgs);
                _primaryLabel.text = Loc.T(_panelContent.PrimaryKey);
                _secondaryLabel.text = _panelContent.SecondaryKey == null ? "" : Loc.T(_panelContent.SecondaryKey);
                _panelSecondary.gameObject.SetActive(_panelContent.SecondaryKey != null);
            }
            if (!_hintContent.IsNone) _hintText.text = Loc.T(_hintContent.Key);

            _settingsTitle.text = Loc.T("options");
            _soundLabel.text = Loc.T(_settings.SoundOn ? "sound_on" : "sound_off");
            _speedLabel.text = Loc.T(_settings.FastAnimations ? "speed_fast" : "speed_normal");
            _motionLabel.text = Loc.T(_settings.ReducedMotion ? "motion_on" : "motion_off");
            _languageLabel.text = Loc.T("language");
            _closeLabel.text = Loc.T("close");
        }
    }

    /// <summary>Keeps the HUD inside the device safe area (notches, rounded corners).</summary>
    public sealed class SafeArea : MonoBehaviour
    {
        private Rect _applied;

        private void Update()
        {
            Rect area = Screen.safeArea;
            if (area == _applied || Screen.width == 0 || Screen.height == 0) return;
            _applied = area;
            var rt = (RectTransform)transform;
            rt.anchorMin = new Vector2(area.xMin / Screen.width, area.yMin / Screen.height);
            rt.anchorMax = new Vector2(area.xMax / Screen.width, area.yMax / Screen.height);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }
    }
}
