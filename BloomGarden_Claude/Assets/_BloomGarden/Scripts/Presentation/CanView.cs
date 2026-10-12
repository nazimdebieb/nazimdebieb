using System.Collections.Generic;
using BloomGarden.Core.Model;
using UnityEngine;

namespace BloomGarden.Presentation
{
    /// <summary>
    /// A watering can built from primitives: colored body, spout, handle, color symbol, and one white drop per
    /// unit of water left, so a waiting can always shows its remaining capacity (GDD chapter 03).
    /// </summary>
    public sealed class CanView : MonoBehaviour
    {
        public string CanId { get; private set; }
        public ColorId Color { get; private set; }
        public int ShownRemaining => _shownRemaining;

        private GardenPalette _palette;
        private Transform _drops;
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
            view.BuildBody();
            view.SetRemaining(can.Capacity);
            go.SetActive(false);
            return view;
        }

        private void BuildBody()
        {
            Material color = _palette.Flower(Color);
            var body = GardenView.Prim(PrimitiveType.Cylinder, transform, color, new Vector3(0f, 0.42f, 0f), new Vector3(0.78f, 0.4f, 0.78f));
            var spout = GardenView.Prim(PrimitiveType.Cylinder, transform, color, new Vector3(0.5f, 0.6f, 0f), new Vector3(0.12f, 0.3f, 0.12f));
            spout.transform.localRotation = Quaternion.Euler(0f, 0f, -55f);
            var rose = GardenView.Prim(PrimitiveType.Sphere, transform, color, new Vector3(0.72f, 0.78f, 0f), new Vector3(0.2f, 0.2f, 0.2f));
            var handle = GardenView.Prim(PrimitiveType.Cylinder, transform, color, new Vector3(-0.4f, 0.62f, 0f), new Vector3(0.1f, 0.22f, 0.1f));
            handle.transform.localRotation = Quaternion.Euler(0f, 0f, 30f);
            _bodyRenderers.Add(body.GetComponent<Renderer>());
            _bodyRenderers.Add(spout.GetComponent<Renderer>());
            _bodyRenderers.Add(rose.GetComponent<Renderer>());
            _bodyRenderers.Add(handle.GetComponent<Renderer>());
            FlowerFactory.AddSymbol(Color, _palette, transform, new Vector3(0f, 0.45f, -0.4f), 0.16f);

            _drops = new GameObject("Water").transform;
            _drops.SetParent(transform, false);
            _drops.localPosition = new Vector3(0f, 0.86f, 0f);
        }

        /// <summary>One drop per unit of water left, in a row on top of the can.</summary>
        public void SetRemaining(int remaining)
        {
            if (remaining == _shownRemaining) return;
            _shownRemaining = remaining;
            foreach (Transform child in _drops) Destroy(child.gameObject);
            float spacing = 0.2f;
            for (int i = 0; i < remaining; i++)
            {
                var drop = GardenView.Prim(PrimitiveType.Sphere, _drops, _palette.marker,
                    new Vector3((i - (remaining - 1) / 2f) * spacing, 0f, 0f), new Vector3(0.15f, 0.18f, 0.15f));
                drop.name = "Drop " + (i + 1);
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

        private void OnDestroy()
        {
            if (_dimmed != null) Destroy(_dimmed);
        }
    }
}
