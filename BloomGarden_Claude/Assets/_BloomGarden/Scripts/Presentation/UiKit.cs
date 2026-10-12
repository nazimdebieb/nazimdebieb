using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BloomGarden.Presentation
{
    /// <summary>
    /// Small uGUI toolkit shared by the HUD and the menus: palette, labels, buttons, anchoring, and a registry of
    /// every label so tests can check that no text overflows its box in either language.
    /// </summary>
    public sealed class UiKit
    {
        public static readonly Color Ink = new Color32(0x25, 0x48, 0x3B, 0xFF);
        public static readonly Color Ivory = new Color32(0xFF, 0xF9, 0xEE, 0xFF);
        public static readonly Color Leaf = new Color32(0x6D, 0xBF, 0x93, 0xFF);
        public static readonly Color Paper = new Color32(0xF3, 0xEA, 0xDA, 0xFF);
        public static readonly Color Muted = new Color32(0xD8, 0xD0, 0xC2, 0xFF);

        private readonly Font _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        private readonly List<Text> _labels = new List<Text>();

        public IReadOnlyList<Text> Labels => _labels;

        public static RectTransform Stretch(Transform parent, string name)
        {
            var rt = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            return rt;
        }

        public static void Place(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
        }

        /// <summary>Anchored at a point of the parent (0..1), fixed size.</summary>
        public static void At(RectTransform rt, float ax, float ay, Vector2 position, Vector2 size) =>
            Place(rt, new Vector2(ax, ay), new Vector2(ax, ay), position, size);

        public static Image Panel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.color = color;
            return image;
        }

        public Text Label(Transform parent, string name, int size, FontStyle style, TextAnchor anchor)
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
            _labels.Add(text);
            return text;
        }

        public Button Button(Transform parent, string name, Color color, int fontSize, out Text label)
        {
            var go = new GameObject("Button " + name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            go.GetComponent<Image>().color = color;
            var button = go.GetComponent<Button>();
            label = Label(go.transform, "Label", fontSize, FontStyle.Bold, TextAnchor.MiddleCenter);
            Place(label.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(-24f, -8f));
            return button;
        }

        /// <summary>Active labels whose wrapped text is taller than their box (P5-05 overflow check).</summary>
        public List<string> FindOverflows()
        {
            var problems = new List<string>();
            Canvas.ForceUpdateCanvases();
            foreach (Text text in _labels)
            {
                if (text == null || !text.gameObject.activeInHierarchy || string.IsNullOrEmpty(text.text)) continue;
                Rect rect = text.rectTransform.rect;
                if (rect.width <= 1f) continue;
                float needed = text.preferredHeight;
                if (needed > rect.height + 2f)
                    problems.Add($"{Path(text.transform)} '{text.text}' needs {needed:0}px, box {rect.height:0}px");
            }
            return problems;
        }

        private static string Path(Transform t)
        {
            string path = t.name;
            for (Transform p = t.parent; p != null && p.GetComponent<Canvas>() == null; p = p.parent) path = p.name + "/" + path;
            return path;
        }
    }
}
