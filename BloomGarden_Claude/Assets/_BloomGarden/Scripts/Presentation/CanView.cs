using System.Collections.Generic;
using BloomGarden.Core.Model;
using UnityEngine;

namespace BloomGarden.Presentation
{
    /// <summary>
    /// A watering can built from primitives: rounded body, top handle, spout with a rose, color symbol, and one
    /// white drop per unit of water left, so a waiting can always shows its remaining capacity (GDD chapter 03).
    /// The drops are created once (up to the capacity) and only shown or hidden afterwards.
    /// </summary>
    public sealed class CanView : MonoBehaviour
    {
        public string CanId { get; private set; }
        public ColorId Color { get; private set; }
        public int ShownRemaining => _shownRemaining;

        /// <summary>Spout tip in the can's local space: where the water jet starts.</summary>
        public static readonly Vector3 SpoutTip = new Vector3(0.78f, 0.82f, 0f);

        /// <summary>Animated part (bob, tilt, squash) so the root position stays exact for layout checks.</summary>
        public Transform Body { get; private set; }

        private GardenPalette _palette;
        private readonly List<GameObject> _drops = new List<GameObject>();
        private readonly List<Renderer> _bodyRenderers = new List<Renderer>();
        private Material _dimmed;
        private int _shownRemaining = -1;

        public static CanView Create(CanDefinition can, GardenPalette palette, Transform parent)
        {
            var go = new GameObject("Can " + can.Id);
            go.transform.SetParent(parent, false);
            var view = go.AddComponent<CanView>();
            view.CanId = can.Id;
            view.Color = can.Color;
            view._palette = palette;
            view.Build(can.Capacity);
            view.SetRemaining(can.Capacity);
            go.SetActive(false);
            return view;
        }

        private void Build(int capacity)
        {
            Body = new GameObject("Body").transform;
            Body.SetParent(transform, false);
            Material color = _palette.Flower(Color);

            Keep(GardenView.Prim(PrimitiveType.Cylinder, Body, color, new Vector3(0f, 0.4f, 0f), new Vector3(0.74f, 0.38f, 0.74f), true));
            Keep(GardenView.Prim(PrimitiveType.Sphere, Body, color, new Vector3(0f, 0.78f, 0f), new Vector3(0.74f, 0.16f, 0.74f), true));
            var spout = Keep(GardenView.Prim(PrimitiveType.Cylinder, Body, color, new Vector3(0.5f, 0.55f, 0f), new Vector3(0.11f, 0.3f, 0.11f), true));
            spout.transform.localRotation = Quaternion.Euler(0f, 0f, -52f);
            var rose = Keep(GardenView.Prim(PrimitiveType.Cylinder, Body, color, SpoutTip - new Vector3(0.04f, 0.03f, 0f), new Vector3(0.22f, 0.04f, 0.22f), true));
            rose.transform.localRotation = Quaternion.Euler(0f, 0f, -52f);
            // Top handle: two posts and a bar.
            Keep(GardenView.Prim(PrimitiveType.Cylinder, Body, color, new Vector3(-0.2f, 0.95f, 0f), new Vector3(0.08f, 0.13f, 0.08f), true));
            Keep(GardenView.Prim(PrimitiveType.Cylinder, Body, color, new Vector3(0.2f, 0.95f, 0f), new Vector3(0.08f, 0.13f, 0.08f), true));
            var bar = Keep(GardenView.Prim(PrimitiveType.Cylinder, Body, color, new Vector3(0f, 1.08f, 0f), new Vector3(0.08f, 0.22f, 0.08f), true));
            bar.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

            FlowerFactory.AddSymbol(Color, _palette, Body, new Vector3(0f, 0.42f, -0.38f), 0.16f);

            var dropsRoot = new GameObject("Water").transform;
            dropsRoot.SetParent(Body, false);
            dropsRoot.localPosition = new Vector3(0f, 0.9f, -0.12f);
            for (int i = 0; i < capacity; i++)
            {
                var drop = GardenView.Prim(PrimitiveType.Sphere, dropsRoot, _palette.marker, Vector3.zero, new Vector3(0.14f, 0.17f, 0.14f));
                drop.name = "Drop " + (i + 1);
                _drops.Add(drop);
            }
        }

        private GameObject Keep(GameObject part)
        {
            _bodyRenderers.Add(part.GetComponent<Renderer>());
            return part;
        }

        /// <summary>One drop per unit of water left, centred in a row on top of the can.</summary>
        public void SetRemaining(int remaining)
        {
            if (remaining == _shownRemaining) return;
            _shownRemaining = remaining;
            const float spacing = 0.19f;
            for (int i = 0; i < _drops.Count; i++)
            {
                bool on = i < remaining;
                _drops[i].SetActive(on);
                if (on) _drops[i].transform.localPosition = new Vector3((i - (remaining - 1) / 2f) * spacing, 0f, 0f);
            }
        }

        /// <summary>Cans waiting behind the head of a lane are shown paler: visible but not selectable.</summary>
        public void SetDimmed(bool dimmed)
        {
            Material color = _palette.Flower(Color);
            if (dimmed && _dimmed == null)
            {
                _dimmed = new Material(color);
                if (_dimmed.HasProperty("_BaseColor"))
                    _dimmed.SetColor("_BaseColor", UnityEngine.Color.Lerp(color.GetColor("_BaseColor"), UnityEngine.Color.white, 0.45f));
            }
            foreach (var r in _bodyRenderers) r.sharedMaterial = dimmed ? _dimmed : color;
        }

        public void ResetPose()
        {
            Body.localPosition = Vector3.zero;
            Body.localRotation = Quaternion.identity;
            Body.localScale = Vector3.one;
        }

        private void OnDestroy()
        {
            if (_dimmed != null) Destroy(_dimmed);
        }
    }
}
