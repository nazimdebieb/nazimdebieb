using System.Collections;
using System.Collections.Generic;
using BloomGarden.Core.Model;
using BloomGarden.Core.Rules;
using UnityEngine;

namespace BloomGarden.Presentation
{
    /// <summary>
    /// 2.5D garden built from soft primitives (P3; art pass in P4). The view never decides rules:
    /// <see cref="ShowState"/> rebuilds everything from a <see cref="GameState"/>, and <see cref="PlayEvents"/>
    /// only animates the events returned by the TurnResolver before the caller snaps to the final state.
    /// World layout: XZ plane, Y up; board at the top of the screen (+Z), places in the middle, lanes at the bottom.
    /// </summary>
    public sealed class GardenView : MonoBehaviour
    {
        public const float MaxCellSize = 1.4f;
        /// <summary>The board grows downward from this edge and never exceeds <see cref="MaxBoardDepth"/>.</summary>
        public const float BoardTopZ = 5.8f;
        public const float MaxBoardDepth = 5.2f;
        public const float SlotZ = -0.75f;
        public const float LaneHeadZ = -3.0f;
        public const float LaneStep = 1.15f;
        public const float ColumnSpacing = 2.0f;
        public const int VisibleCansPerLane = 4;

        [SerializeField] private GardenPalette palette;
        [Tooltip("Duration of one logical step animation, in seconds.")]
        [SerializeField] private float stepDuration = 0.16f;

        private LevelDefinition _level;
        private float _cell = MaxCellSize;
        private float _boardCenterZ;
        private Transform _cellsRoot, _slotsRoot, _cansRoot;
        private readonly List<Transform> _cellContent = new List<Transform>();
        private readonly Dictionary<string, CanView> _cans = new Dictionary<string, CanView>();
        private readonly List<GameObject> _highlights = new List<GameObject>();

        public GardenPalette Palette { get => palette; set => palette = value; }

        /// <summary>Raised during <see cref="PlayEvents"/> each time a bloom becomes visible (for the HUD counter).</summary>
        public event System.Action BloomShown;

        public static float ColumnX(int index) => (index - 1) * ColumnSpacing;
        public static Vector3 SlotPosition(int slot) => new Vector3(ColumnX(slot), 0f, SlotZ);
        public static Vector3 LanePosition(int lane, int depth) => new Vector3(ColumnX(lane), 0f, LaneHeadZ - depth * LaneStep);

        public Vector3 CellPosition(int cell)
        {
            int w = _level.Width, h = _level.Height;
            float x = (_level.ColumnOf(cell) - (w - 1) / 2f) * _cell;
            float z = _boardCenterZ + ((h - 1) / 2f - _level.RowOf(cell)) * _cell;
            return new Vector3(x, 0f, z);
        }

        // ------------------------------------------------------------------ build

        public void Build(LevelDefinition level)
        {
            _level = level;
            _cell = Mathf.Min(MaxCellSize, MaxBoardDepth / Mathf.Max(level.Width, level.Height));
            _boardCenterZ = BoardTopZ - level.Height * _cell / 2f;
            foreach (Transform child in transform) Destroy(child.gameObject);
            _cellContent.Clear();
            _cans.Clear();
            _highlights.Clear();

            _cellsRoot = NewRoot("Board");
            _slotsRoot = NewRoot("Places");
            _cansRoot = NewRoot("Cans");

            var bed = Prim(PrimitiveType.Cube, _cellsRoot, palette.leaf, new Vector3(0f, -0.18f, _boardCenterZ),
                new Vector3(level.Width * _cell + 0.35f, 0.2f, level.Height * _cell + 0.35f));
            bed.name = "Bed";
            for (int i = 0; i < level.CellCount; i++)
            {
                var tile = Prim(PrimitiveType.Cylinder, _cellsRoot, palette.soil, CellPosition(i) + Vector3.down * 0.05f,
                    new Vector3(_cell * 0.86f, 0.05f, _cell * 0.86f));
                tile.name = $"Cell {level.RowOf(i)},{level.ColumnOf(i)}";
                var content = new GameObject("Content").transform;
                content.SetParent(tile.transform.parent, false);
                content.localPosition = CellPosition(i);
                content.localScale = Vector3.one * (_cell / MaxCellSize);
                _cellContent.Add(content);
            }

            for (int s = 0; s < level.SlotCount; s++)
            {
                var pad = Prim(PrimitiveType.Cylinder, _slotsRoot, palette.slot, SlotPosition(s) + Vector3.down * 0.06f, new Vector3(1.5f, 0.04f, 1.5f));
                pad.name = $"Place {s + 1}";
            }

            for (int l = 0; l < level.Lanes.Count; l++)
            {
                var lanePad = Prim(PrimitiveType.Cube, _slotsRoot, palette.lane,
                    LanePosition(l, 0) + new Vector3(0f, -0.1f, -(VisibleCansPerLane - 1) * LaneStep / 2f),
                    new Vector3(1.5f, 0.06f, VisibleCansPerLane * LaneStep));
                lanePad.name = $"Lane {l + 1}";
                foreach (CanDefinition can in level.Lanes[l])
                {
                    var view = CanView.Create(can, palette, _cansRoot);
                    _cans[can.Id] = view;
                }
            }
        }

        // ------------------------------------------------------------------ snap to state

        /// <summary>Authoritative display of a state. Cancels nothing by itself; the caller stops animations first.</summary>
        public void ShowState(GameState state)
        {
            for (int i = 0; i < _level.CellCount; i++) ShowCell(i, state.BloomedInCell(i), pop: false);

            var placed = new HashSet<string>();
            for (int s = 0; s < state.SlotCount; s++)
            {
                SlotState slot = state.GetSlot(s);
                if (!slot.IsOccupied) continue;
                CanView can = _cans[slot.CanId];
                can.gameObject.SetActive(true);
                can.transform.localPosition = SlotPosition(s);
                can.transform.localScale = Vector3.one;
                can.transform.localRotation = Quaternion.identity;
                can.SetRemaining(slot.Remaining);
                can.SetDimmed(false);
                placed.Add(slot.CanId);
            }

            for (int l = 0; l < _level.Lanes.Count; l++)
            {
                var lane = _level.Lanes[l];
                for (int k = 0; k < lane.Count; k++)
                {
                    CanView can = _cans[lane[k].Id];
                    if (placed.Contains(lane[k].Id)) continue;
                    int depth = k - state.LaneHead(l);
                    bool waiting = depth >= 0 && depth < VisibleCansPerLane;
                    can.gameObject.SetActive(waiting);
                    if (!waiting) continue;
                    can.transform.localPosition = LanePosition(l, depth);
                    can.transform.localScale = Vector3.one * (depth == 0 ? 1f : 0.82f);
                    can.transform.localRotation = Quaternion.identity;
                    can.SetRemaining(lane[k].Capacity);
                    can.SetDimmed(depth > 0);
                }
            }
            ShowHighlights(state);
        }

        private void ShowHighlights(GameState state)
        {
            foreach (var h in _highlights) Destroy(h);
            _highlights.Clear();
            foreach (var target in _cansRoot.GetComponentsInChildren<LaneTapTarget>(true)) Destroy(target.gameObject);

            for (int l = 0; l < _level.Lanes.Count; l++)
            {
                if (!state.TryGetLaneHead(_level, l, out CanDefinition head)) continue;
                bool selectable = TurnResolver.CanSelect(_level, state, l);
                if (selectable)
                {
                    var ring = Prim(PrimitiveType.Cylinder, _cansRoot, palette.highlight, LanePosition(l, 0) + Vector3.down * 0.04f,
                        new Vector3(1.6f, 0.03f, 1.6f));
                    ring.name = $"Selectable lane {l + 1}";
                    _highlights.Add(ring);
                }
                // Tap target on every head (also when places are full, so the tap can be acknowledged).
                var hit = new GameObject($"Tap lane {l + 1}");
                hit.transform.SetParent(_cansRoot, false);
                hit.transform.localPosition = LanePosition(l, 0) + Vector3.up * 0.5f;
                var box = hit.AddComponent<BoxCollider>();
                box.size = new Vector3(ColumnSpacing * 0.95f, 1.6f, LaneStep * 1.2f);
                hit.AddComponent<LaneTapTarget>().lane = l;
            }
        }

        /// <summary>Cell content: the visible bud (+ one dormant marker per hidden layer), an open flower once
        /// the stack is done, or grass for a cell that never had buds.</summary>
        private void ShowCell(int cell, int bloomed, bool pop)
        {
            Transform root = _cellContent[cell];
            foreach (Transform child in root) Destroy(child.gameObject);
            var stack = _level.Cells[cell];

            GameObject shown;
            if (bloomed < stack.Count)
            {
                shown = FlowerFactory.Bud(stack[bloomed], palette, root);
                int hidden = stack.Count - bloomed - 1;
                for (int k = 0; k < hidden; k++)
                {
                    var pip = Prim(PrimitiveType.Sphere, root, palette.dormant,
                        new Vector3(-0.32f + k * 0.2f, 0.06f, -0.38f), Vector3.one * 0.14f);
                    pip.name = "Hidden layer";
                }
            }
            else if (stack.Count > 0)
            {
                shown = FlowerFactory.Bloom(stack[stack.Count - 1], palette, root);
            }
            else
            {
                shown = FlowerFactory.Grass(palette, root);
            }
            if (pop) StartCoroutine(Pop(shown.transform));
        }

        /// <summary>
        /// Verifies that what is on screen matches the state: cell contents, cans in places with their water,
        /// lane heads, and no stray can. Used by the PlayMode tests (no desync after taps, undo or restart).
        /// </summary>
        public bool MatchesState(GameState state, out string error)
        {
            for (int i = 0; i < _level.CellCount; i++)
            {
                var stack = _level.Cells[i];
                int bloomed = state.BloomedInCell(i);
                string expected = bloomed < stack.Count ? "Bud " + stack[bloomed].ToId()
                    : stack.Count > 0 ? "Bloom " + stack[stack.Count - 1].ToId() : "Grass";
                string shown = null;
                foreach (Transform child in _cellContent[i])
                    if (child.name.StartsWith("Bud") || child.name.StartsWith("Bloom") || child.name == "Grass") shown = child.name;
                if (shown != expected) { error = $"cell {i}: shows '{shown}', expected '{expected}'"; return false; }
            }

            var expectedActive = new HashSet<string>();
            for (int s = 0; s < state.SlotCount; s++)
            {
                SlotState slot = state.GetSlot(s);
                if (!slot.IsOccupied) continue;
                expectedActive.Add(slot.CanId);
                CanView can = _cans[slot.CanId];
                if (!can.gameObject.activeSelf || (can.transform.localPosition - SlotPosition(s)).sqrMagnitude > 1e-4f)
                { error = $"{slot.CanId} not shown in place {s + 1}"; return false; }
                if (can.ShownRemaining != slot.Remaining)
                { error = $"{slot.CanId} shows {can.ShownRemaining} water, expected {slot.Remaining}"; return false; }
            }
            for (int l = 0; l < _level.Lanes.Count; l++)
            {
                var lane = _level.Lanes[l];
                int head = state.LaneHead(l);
                for (int k = head; k < lane.Count && k - head < VisibleCansPerLane; k++)
                {
                    expectedActive.Add(lane[k].Id);
                    if ((_cans[lane[k].Id].transform.localPosition - LanePosition(l, k - head)).sqrMagnitude > 1e-4f)
                    { error = $"{lane[k].Id} not at its lane position"; return false; }
                }
            }
            foreach (var pair in _cans)
            {
                if (pair.Value.gameObject.activeSelf != expectedActive.Contains(pair.Key))
                { error = $"{pair.Key} visible={pair.Value.gameObject.activeSelf}, expected {expectedActive.Contains(pair.Key)}"; return false; }
            }
            error = null;
            return true;
        }

        // ------------------------------------------------------------------ animation

        /// <summary>Animates one move from its events. Purely cosmetic: the state was already resolved.</summary>
        public IEnumerator PlayEvents(GameState before, IReadOnlyList<GameEvent> events)
        {
            var bloomed = new int[_level.CellCount];
            for (int i = 0; i < bloomed.Length; i++) bloomed[i] = before.BloomedInCell(i);

            foreach (GameEvent e in events)
            {
                switch (e.Type)
                {
                    case GameEventType.CanSelected:
                        yield return SendToPlace(e, before);
                        break;
                    case GameEventType.FlowerBloomed:
                    {
                        CanView can = _cans[e.CanId];
                        bloomed[e.CellIndex]++;
                        can.SetRemaining(e.RemainingAfter);
                        StartCoroutine(Tilt(can.transform));
                        StartCoroutine(Drop(can.transform.position, CellWorld(e.CellIndex)));
                        yield return Shrink(_cellContent[e.CellIndex], stepDuration * 0.5f);
                        _cellContent[e.CellIndex].localScale = Vector3.one * (_cell / MaxCellSize);
                        ShowCell(e.CellIndex, bloomed[e.CellIndex], pop: true);
                        BloomShown?.Invoke();
                        yield return new WaitForSeconds(stepDuration * 0.5f);
                        break;
                    }
                    case GameEventType.CanEmptied:
                        yield return Shrink(_cans[e.CanId].transform, stepDuration);
                        _cans[e.CanId].gameObject.SetActive(false);
                        break;
                }
            }
        }

        private Vector3 CellWorld(int cell) => transform.TransformPoint(CellPosition(cell));

        private IEnumerator SendToPlace(GameEvent e, GameState before)
        {
            foreach (var h in _highlights) Destroy(h);
            _highlights.Clear();

            CanView moving = _cans[e.CanId];
            moving.SetDimmed(false);
            var lane = _level.Lanes[e.Lane];
            int head = before.LaneHead(e.Lane);
            var followers = new List<(Transform t, Vector3 from, Vector3 to, float s0, float s1)>();
            for (int k = head + 1; k < lane.Count && k - head <= VisibleCansPerLane; k++)
            {
                CanView c = _cans[lane[k].Id];
                int depth = k - head - 1;
                c.gameObject.SetActive(depth < VisibleCansPerLane);
                c.SetDimmed(depth > 0);
                followers.Add((c.transform, c.transform.localPosition, LanePosition(e.Lane, depth),
                    c.transform.localScale.x, depth == 0 ? 1f : 0.82f));
            }

            Vector3 from = moving.transform.localPosition, to = SlotPosition(e.Slot);
            float duration = stepDuration * 1.6f;
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                float k = Mathf.SmoothStep(0f, 1f, t);
                moving.transform.localPosition = Vector3.Lerp(from, to, k) + Vector3.up * Mathf.Sin(k * Mathf.PI) * 0.6f;
                foreach (var f in followers)
                {
                    f.t.localPosition = Vector3.Lerp(f.from, f.to, k);
                    f.t.localScale = Vector3.one * Mathf.Lerp(f.s0, f.s1, k);
                }
                yield return null;
            }
            moving.transform.localPosition = to;
        }

        private static IEnumerator Tilt(Transform can)
        {
            const float duration = 0.18f;
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                can.localRotation = Quaternion.Euler(-Mathf.Sin(t * Mathf.PI) * 25f, 0f, 0f);
                yield return null;
            }
            can.localRotation = Quaternion.identity;
        }

        private IEnumerator Drop(Vector3 from, Vector3 to)
        {
            var drop = Prim(PrimitiveType.Sphere, transform, palette.flowers[(int)ColorId.Blue], Vector3.zero, Vector3.one * 0.16f);
            drop.name = "Drop";
            float duration = stepDuration;
            for (float t = 0f; t < 1f && drop != null; t += Time.deltaTime / duration)
            {
                drop.transform.position = Vector3.Lerp(from + Vector3.up * 0.8f, to + Vector3.up * 0.5f, t) + Vector3.up * Mathf.Sin(t * Mathf.PI) * 1.2f;
                yield return null;
            }
            if (drop != null) Destroy(drop);
        }

        private static IEnumerator Shrink(Transform target, float duration)
        {
            Vector3 start = target.localScale;
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                target.localScale = start * (1f - t);
                yield return null;
            }
            target.localScale = Vector3.zero;
        }

        private static IEnumerator Pop(Transform target)
        {
            const float duration = 0.2f;
            for (float t = 0f; t < 1f && target != null; t += Time.deltaTime / duration)
            {
                target.localScale = Vector3.one * (1f + Mathf.Sin(t * Mathf.PI) * 0.25f) * Mathf.Min(1f, t * 2.5f);
                yield return null;
            }
            if (target != null) target.localScale = Vector3.one;
        }

        // ------------------------------------------------------------------ helpers

        private Transform NewRoot(string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            return go.transform;
        }

        internal static GameObject Prim(PrimitiveType type, Transform parent, Material material, Vector3 localPosition, Vector3 localScale)
        {
            var go = GameObject.CreatePrimitive(type);
            Object.Destroy(go.GetComponent<Collider>());
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = localScale;
            var renderer = go.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            return go;
        }
    }
}
