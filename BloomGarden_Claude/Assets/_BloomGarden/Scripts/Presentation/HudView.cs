using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BloomGarden.Presentation
{
    /// <summary>
    /// Portrait HUD built at runtime with uGUI (GDD screen C): objective and bud counter at the top,
    /// Undo / Restart at the bottom, and the end-of-level panel (win or deadlock). French texts for now;
    /// localization comes in P5.
    /// </summary>
    public sealed class HudView : MonoBehaviour
    {
        private static readonly Color Ink = new Color32(0x25, 0x48, 0x3B, 0xFF);
        private static readonly Color Ivory = new Color32(0xFF, 0xF9, 0xEE, 0xFF);
        private static readonly Color Leaf = new Color32(0x6D, 0xBF, 0x93, 0xFF);
        private static readonly Color Paper = new Color32(0xF3, 0xEA, 0xDA, 0xFF);

        private Font _font;
        private Text _title, _counter, _panelTitle, _panelBody;
        private Button _undo, _restart, _panelPrimary, _panelSecondary;
        private Text _panelPrimaryLabel, _panelSecondaryLabel;
        private GameObject _panel;

        public event Action UndoClicked, RestartClicked, PanelPrimaryClicked, PanelSecondaryClicked;

        public void Build()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            if (FindAnyObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                es.transform.SetParent(transform, false);
            }

            var canvasGo = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
            RectTransform root = canvasGo.GetComponent<RectTransform>();

            var safe = Rect(root, "Safe area", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            safe.gameObject.AddComponent<SafeArea>();

            _title = Label(safe, "Title", 64, FontStyle.Bold, TextAnchor.UpperCenter);
            Place(_title.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -150f), new Vector2(-80f, 90f));
            _counter = Label(safe, "Counter", 44, FontStyle.Normal, TextAnchor.UpperCenter);
            Place(_counter.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -235f), new Vector2(-80f, 60f));

            _undo = MakeButton(safe, "Annuler", Paper, out _);
            Place((RectTransform)_undo.transform, new Vector2(0.27f, 0f), new Vector2(0.27f, 0f), new Vector2(0f, 110f), new Vector2(400f, 130f));
            _undo.onClick.AddListener(() => UndoClicked?.Invoke());
            _restart = MakeButton(safe, "Recommencer", Paper, out _);
            Place((RectTransform)_restart.transform, new Vector2(0.73f, 0f), new Vector2(0.73f, 0f), new Vector2(0f, 110f), new Vector2(400f, 130f));
            _restart.onClick.AddListener(() => RestartClicked?.Invoke());

            _panel = new GameObject("End panel", typeof(RectTransform), typeof(Image));
            var panelRt = (RectTransform)_panel.transform;
            panelRt.SetParent(safe, false);
            Place(panelRt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 120f), new Vector2(880f, 640f));
            _panel.GetComponent<Image>().color = new Color(Ivory.r, Ivory.g, Ivory.b, 0.96f);
            _panelTitle = Label(panelRt, "Title", 72, FontStyle.Bold, TextAnchor.MiddleCenter);
            Place(_panelTitle.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -110f), new Vector2(-60f, 120f));
            _panelBody = Label(panelRt, "Body", 42, FontStyle.Normal, TextAnchor.MiddleCenter);
            Place(_panelBody.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -240f), new Vector2(-80f, 110f));
            _panelPrimary = MakeButton(panelRt, "", Leaf, out _panelPrimaryLabel);
            Place((RectTransform)_panelPrimary.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 230f), new Vector2(600f, 130f));
            _panelPrimary.onClick.AddListener(() => PanelPrimaryClicked?.Invoke());
            _panelSecondary = MakeButton(panelRt, "", Paper, out _panelSecondaryLabel);
            Place((RectTransform)_panelSecondary.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 85f), new Vector2(600f, 110f));
            _panelSecondary.onClick.AddListener(() => PanelSecondaryClicked?.Invoke());
            _panel.SetActive(false);
        }

        public void SetTitle(string title) => _title.text = title;

        public void SetCounter(int bloomed, int total) => _counter.text = $"Fleurs écloses : {bloomed} / {total}";

        public void SetUndoInteractable(bool value) => _undo.interactable = value;

        public bool PanelVisible => _panel.activeSelf;

        public void ShowPanel(string title, string body, string primary, string secondary)
        {
            _panelTitle.text = title;
            _panelBody.text = body;
            _panelPrimaryLabel.text = primary;
            _panelSecondaryLabel.text = secondary;
            _panelSecondary.gameObject.SetActive(!string.IsNullOrEmpty(secondary));
            _panel.SetActive(true);
        }

        public void HidePanel() => _panel.SetActive(false);

        // ------------------------------------------------------------------ helpers

        private static RectTransform Rect(Transform parent, string name, Vector2 min, Vector2 max, Vector2 offMin, Vector2 offMax)
        {
            var rt = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.offsetMin = offMin;
            rt.offsetMax = offMax;
            return rt;
        }

        private static void Place(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
        }

        private Text Label(Transform parent, string name, int size, FontStyle style, TextAnchor anchor)
        {
            var text = new GameObject(name, typeof(RectTransform), typeof(Text)).GetComponent<Text>();
            text.transform.SetParent(parent, false);
            text.font = _font;
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = anchor;
            text.color = Ink;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private Button MakeButton(Transform parent, string label, Color color, out Text text)
        {
            var go = new GameObject("Button " + label, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            go.GetComponent<Image>().color = color;
            var button = go.GetComponent<Button>();
            text = Label(go.transform, "Label", 46, FontStyle.Bold, TextAnchor.MiddleCenter);
            Place(text.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            text.text = label;
            return button;
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
