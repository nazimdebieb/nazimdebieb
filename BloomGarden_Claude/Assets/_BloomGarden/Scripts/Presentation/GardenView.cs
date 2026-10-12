using System.Collections;
using System.Collections.Generic;
using BloomGarden.Core.Model;
using BloomGarden.Core.Rules;
using UnityEngine;

namespace BloomGarden.Presentation
{
    /// <summary>
    /// 2.5D miniature garden built from soft primitives. The view never decides rules:
    /// <see cref="ShowState"/> rebuilds the display from a <see cref="GameState"/>, and <see cref="PlayEvents"/>
    /// only animates the events returned by the TurnResolver before the caller snaps to the final state.
    /// Timings follow <see cref="GameSettings"/> (fast mode, reduced motion) and never feed back into the logic.
    /// World layout: XZ plane, Y up; board at the top of the screen (+Z), places in the middle, lanes at the bottom.
    /// </summary>
    public sealed class GardenView : MonoBehaviour
    {
        public const float MaxCellSize = 1.4f;
        /// <summary>The board grows downward from this edge and never exceeds <see cref="MaxBoardDepth"/>.</summary>
        public const float BoardTopZ = 5.8f;
        public const float MaxBoardDepth = 4.9f;
        public const float SlotZ = -0.75f;
        public const float LaneHeadZ = -3.0f;
        public const float LaneStep = 1.15f;
        public const float ColumnSpacing = 2.0f;
        public const int VisibleCansPerLane = 4;
        /// <summary>Flowers are drawn larger than the soil patch scale so they read well on a phone.</summary>
        public const float FlowerScale = 1.3f;

        [SerializeField] private GardenPalette palette;
        [Tooltip("Base duration of one logical step animation, in seconds (normal speed).")]
        [SerializeField] private float stepDuration = 0.2f;

        private LevelDefinition _level;
        private GameSettings _settings;
        private GardenAudio _audio;
        private VisualPool _pool;
        private float _cell = MaxCellSize;
        private float _boardCenterZ;
        private Transform _cellsRoot, _slotsRoot, _cansRoot, _effectsRoot, _poolRoot;
        private readonly List<Transform> _cellContent = new List<Transform>();
        private readonly Dictionary<string, CanView> _cans = new Dictionary<string, CanView>();
        private readonly List<GameObject> _rings = new List<GameObject>();
        private readonly List<BoxCollider> _tapTargets = new List<BoxCollider>();
        private readonly List<CanView> _selectableHeads = new List<CanView>();

        public GardenPalette Palette { get => palette; set => palette = value; }

        /// <summary>Raised during <see cref="PlayEvents"/> each time a bloom becomes visible (for the HUD counter).</summary>
        public event System.Action BloomShown;

        /// <summary>GameObjects created by the visual pool for the current level (P4-08 measurement).</summary>
        public int PooledObjectsCreated => _pool?.Created ?? 0;

        private float ContentScale => _cell / MaxCellSize * FlowerScale;

        private bool Reduced => _settings != null && _settings.ReducedMotion;
        private float Step => stepDuration * (_settings?.DurationScale ?? 1f) * (Reduced ? 0.5f : 1f);

        public void Init(GameSettings settings, GardenAudio audio)
        {
            _settings = settings;
            _audio = audio;
        }

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
            StopAllCoroutines();
            foreach (Transform child in transform) Destroy(child.gameObject);
            _cellContent.Clear();
            _cans.Clear();
            _rings.Clear();
            _tapTargets.Clear();
            _selectableHeads.Clear();

            _cellsRoot = NewRoot("Board");
            _slotsRoot = NewRoot("Places");
            _cansRoot = NewRoot("Cans");
            _effectsRoot = NewRoot("Effects");
            _poolRoot = NewRoot("Pool");
            _poolRoot.gameObject.SetActive(false);
            _pool = new VisualPool(_poolRoot);

            BuildDiorama(level);

            for (int i = 0; i < level.CellCount; i++)
            {
                var tile = Prim(PrimitiveType.Cylinder, _cellsRoot, palette.soil, CellPosition(i) + Vector3.down * 0.04f,
                    new Vector3(_cell * 0.84f, 0.05f, _cell * 0.84f));
                tile.name = $"Cell {level.RowOf(i)},{level.ColumnOf(i)}";
                var content = new GameObject("Content").transform;
                content.SetParent(_cellsRoot, false);
                content.localPosition = CellPosition(i);
                content.localScale = Vector3.one * ContentScale;
                _cellContent.Add(content);
            }

            for (int s = 0; s < level.SlotCount; s++)
            {
                var pad = Prim(PrimitiveType.Cylinder, _slotsRoot, palette.woodDark, SlotPosition(s) + Vector3.down * 0.06f, new Vector3(1.5f, 0.05f, 1.5f));
                pad.name = $"Place {s + 1}";
            }

            for (int l = 0; l < level.Lanes.Count; l++)
            {
                var lanePad = Prim(PrimitiveType.Cube, _slotsRoot, palette.lane,
                    LanePosition(l, 0) + new Vector3(0f, -0.1f, -(VisibleCansPerLane - 1) * LaneStep / 2f),
                    new Vector3(1.5f, 0.06f, VisibleCansPerLane * LaneStep));
                lanePad.name = $"Lane {l + 1}";

                var ring = Prim(PrimitiveType.Cylinder, _slotsRoot, palette.highlight, LanePosition(l, 0) + Vector3.down * 0.03f, new Vector3(1.6f, 0.03f, 1.6f));
                ring.name = $"Selectable lane {l + 1}";
                ring.SetActive(false);
                _rings.Add(ring);

                var hit = new GameObject($"Tap lane {l + 1}");
                hit.transform.SetParent(_cansRoot, false);
                hit.transform.localPosition = LanePosition(l, 0) + Vector3.up * 0.5f;
                var box = hit.AddComponent<BoxCollider>();
                box.size = new Vector3(ColumnSpacing * 0.95f, 1.6f, LaneStep * 1.2f);
                hit.AddComponent<LaneTapTarget>().lane = l;
                _tapTargets.Add(box);

                foreach (CanDefinition can in level.Lanes[l]) _cans[can.Id] = CanView.Create(can, palette, _cansRoot);
            }
        }

        /// <summary>Floating island under the board, stones, trees and a wooden stand for the cans. Decoration only.</summary>
        private void BuildDiorama(LevelDefinition level)
        {
            var decor = NewRoot("Decor");
            float halfW = level.Width * _cell / 2f + 0.45f, halfD = level.Height * _cell / 2f + 0.45f;

            Prim(PrimitiveType.Cube, decor, palette.sand, new Vector3(0f, -1.2f, 0f), new Vector3(40f, 0.2f, 40f)).name = "Ground";
            Prim(PrimitiveType.Cube, decor, palette.earth, new Vector3(0f, -0.62f, _boardCenterZ), new Vector3(halfW * 2f, 0.8f, halfD * 2f), true).name = "Island";
            Prim(PrimitiveType.Cube, decor, palette.grass, new Vector3(0f, -0.16f, _boardCenterZ), new Vector3(halfW * 2f + 0.1f, 0.14f, halfD * 2f + 0.1f)).name = "Lawn";

            // Stones along the island edge, deterministic sizes.
            int index = 0;
            for (float x = -halfW; x <= halfW + 0.01f; x += 0.62f)
            {
                PlaceStone(decor, new Vector3(x, -0.08f, _boardCenterZ + halfD), index++);
                PlaceStone(decor, new Vector3(x, -0.08f, _boardCenterZ - halfD), index++);
            }
            for (float z = -halfD + 0.62f; z < halfD - 0.3f; z += 0.62f)
            {
                PlaceStone(decor, new Vector3(-halfW, -0.08f, _boardCenterZ + z), index++);
                PlaceStone(decor, new Vector3(halfW, -0.08f, _boardCenterZ + z), index++);
            }

            float top = _boardCenterZ + halfD, bottom = _boardCenterZ - halfD;
            FlowerFactory.Tree(palette, palette.blossom, decor, 1.0f).transform.localPosition = new Vector3(-halfW - 0.7f, -0.15f, _boardCenterZ - 0.2f);
            FlowerFactory.Tree(palette, palette.leaf, decor, 0.85f).transform.localPosition = new Vector3(halfW + 0.75f, -0.15f, bottom + 0.9f);
            FlowerFactory.Bush(palette, decor).transform.localPosition = new Vector3(-halfW - 0.5f, -0.15f, bottom + 0.3f);
            FlowerFactory.Bush(palette, decor).transform.localPosition = new Vector3(halfW + 0.5f, -0.15f, top - 0.9f);

            // Wooden stand for places and lanes.
            float standTop = SlotZ + 0.85f, standBottom = LaneHeadZ - (VisibleCansPerLane - 0.4f) * LaneStep;
            Prim(PrimitiveType.Cube, decor, palette.wood, new Vector3(0f, -0.38f, (standTop + standBottom) / 2f),
                new Vector3(ColumnSpacing * 3f + 0.6f, 0.5f, standTop - standBottom), true).name = "Stand";
        }

        private void PlaceStone(Transform parent, Vector3 position, int index)
        {
            float size = 0.3f + ((index * 37) % 10) * 0.018f;
            var stone = FlowerFactory.Stone(palette, parent, size);
            stone.transform.localPosition = position;
            stone.transform.localRotation = Quaternion.Euler(0f, (index * 53) % 360, 0f);
        }

        // ------------------------------------------------------------------ snap to state

        /// <summary>Stops every running animation and clears transient effects (drops, sparkles, petals).</summary>
        public void CancelAnimations()
        {
            StopAllCoroutines();
            if (_effectsRoot != null) _pool.ReleaseChildren(_effectsRoot);
        }

        /// <summary>Authoritative display of a state. Call <see cref="CancelAnimations"/> first when one may be running.</summary>
        public void ShowState(GameState state)
        {
            for (int i = 0; i < _level.CellCount; i++)
            {
                _cellContent[i].localScale = Vector3.one * ContentScale;
                ShowCell(i, state.BloomedInCell(i), animate: false);
            }

            var placed = new HashSet<string>();
            for (int s = 0; s < state.SlotCount; s++)
            {
                SlotState slot = state.GetSlot(s);
                if (!slot.IsOccupied) continue;
                CanView can = _cans[slot.CanId];
                Place(can, SlotPosition(s), 1f, dimmed: false);
                can.SetRemaining(slot.Remaining);
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
                    if (!waiting) { can.gameObject.SetActive(false); continue; }
                    Place(can, LanePosition(l, depth), depth == 0 ? 1f : 0.82f, dimmed: depth > 0);
                    can.SetRemaining(lane[k].Capacity);
                }
            }
            ShowHighlights(state);
        }

        private static void Place(CanView can, Vector3 position, float scale, bool dimmed)
        {
            can.gameObject.SetActive(true);
            can.transform.localPosition = position;
            can.transform.localScale = Vector3.one * scale;
            can.transform.localRotation = Quaternion.identity;
            can.ResetPose();
            can.SetDimmed(dimmed);
        }

        private void ShowHighlights(GameState state)
        {
            _selectableHeads.Clear();
            for (int l = 0; l < _level.Lanes.Count; l++)
            {
                bool hasHead = state.TryGetLaneHead(_level, l, out CanDefinition head);
                bool selectable = hasHead && TurnResolver.CanSelect(_level, state, l);
                _rings[l].SetActive(selectable);
                _rings[l].transform.localScale = new Vector3(1.6f, 0.03f, 1.6f);
                // Tap target on every head (also when places are full, so the tap can be acknowledged).
                _tapTargets[l].enabled = hasHead;
                if (selectable) _selectableHeads.Add(_cans[head.Id]);
            }
        }

        private void HideHighlights()
        {
            foreach (var ring in _rings) ring.SetActive(false);
            foreach (var head in _selectableHeads) head.ResetPose();
            _selectableHeads.Clear();
        }

        /// <summary>Gentle idle motion: selectable heads bob and their ring breathes (off in reduced motion).</summary>
        private void Update()
        {
            if (Reduced || _selectableHeads.Count == 0) return;
            float t = Time.time;
            for (int i = 0; i < _selectableHeads.Count; i++)
            {
                CanView can = _selectableHeads[i];
                if (can == null) continue;
                can.Body.localPosition = new Vector3(0f, (Mathf.Sin(t * 3f + i) * 0.5f + 0.5f) * 0.08f, 0f);
            }
            float pulse = 1.6f + Mathf.Sin(t * 3f) * 0.05f;
            foreach (var ring in _rings)
                if (ring.activeSelf) ring.transform.localScale = new Vector3(pulse, 0.03f, pulse);
        }

        /// <summary>Cell content: the visible bud (+ one dormant marker per hidden layer), an open flower once
        /// the stack is done, or grass for a cell that never had buds. Objects come from the pool.</summary>
        private GameObject ShowCell(int cell, int bloomed, bool animate)
        {
            Transform root = _cellContent[cell];
            _pool.ReleaseChildren(root);
            var stack = _level.Cells[cell];

            GameObject shown;
            if (bloomed < stack.Count)
            {
                ColorId color = stack[bloomed];
                shown = _pool.Get("Bud " + color.ToId(), p => FlowerFactory.Bud(color, palette, p), root);
                int hidden = stack.Count - bloomed - 1;
                for (int k = 0; k < hidden; k++)
                {
                    var pip = _pool.Get("Hidden", p =>
                    {
                        var go = Prim(PrimitiveType.Sphere, p, palette.dormant, Vector3.zero, Vector3.one * 0.14f);
                        go.name = "Hidden layer";
                        return go;
                    }, root);
                    pip.transform.localPosition = new Vector3(-0.34f + k * 0.2f, 0.07f, -0.4f);
                    pip.transform.localScale = Vector3.one * 0.14f;
                }
            }
            else if (stack.Count > 0)
            {
                ColorId color = stack[stack.Count - 1];
                shown = _pool.Get("Bloom " + color.ToId(), p => FlowerFactory.Bloom(color, palette, p), root);
                FlowerFactory.SetBloomOpen(shown, 1f);
            }
            else
            {
                shown = _pool.Get("Grass", p => FlowerFactory.Grass(palette, p), root);
            }
            if (animate && !Reduced) StartCoroutine(Pop(shown.transform, Step * 1.2f));
            return shown;
        }

        /// <summary>
        /// Verifies that what is on screen matches the state: cell contents, hidden-layer markers, cans in places
        /// with their water, lane heads and highlights, and no stray can. Used by the PlayMode tests.
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
                int hiddenShown = 0;
                foreach (Transform child in _cellContent[i])
                {
                    if (!child.gameObject.activeSelf) continue;
                    if (child.name.StartsWith("Bud") || child.name.StartsWith("Bloom") || child.name == "Grass") shown = child.name;
                    if (child.name == "Hidden layer") hiddenShown++;
                }
                if (shown != expected) { error = $"cell {i}: shows '{shown}', expected '{expected}'"; return false; }
                int hiddenExpected = Mathf.Max(0, stack.Count - bloomed - 1);
                if (hiddenShown != hiddenExpected) { error = $"cell {i}: {hiddenShown} hidden markers, expected {hiddenExpected}"; return false; }
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
                bool selectable = TurnResolver.CanSelect(_level, state, l);
                if (_rings[l].activeSelf != selectable) { error = $"lane {l + 1} highlight={_rings[l].activeSelf}, expected {selectable}"; return false; }
            }
            foreach (var pair in _cans)
            {
                if (pair.Value.gameObject.activeSelf != expectedActive.Contains(pair.Key))
                { error = $"{pair.Key} visible={pair.Value.gameObject.activeSelf}, expected {expectedActive.Contains(pair.Key)}"; return false; }
            }
            error = null;
            return true;
        }

        // ------------------------------------------------------------------ move animation

        /// <summary>Animates one move from its events. Purely cosmetic: the state was already resolved.</summary>
        public IEnumerator PlayEvents(GameState before, IReadOnlyList<GameEvent> events)
        {
            var bloomed = new int[_level.CellCount];
            for (int i = 0; i < bloomed.Length; i++) bloomed[i] = before.BloomedInCell(i);
            int cascade = 0;
            HideHighlights();

            foreach (GameEvent e in events)
            {
                switch (e.Type)
                {
                    case GameEventType.CanSelected:
                        _audio?.Play(GardenSound.Select);
                        yield return SendToPlace(e, before);
                        break;

                    case GameEventType.FlowerBloomed:
                    {
                        CanView can = _cans[e.CanId];
                        bloomed[e.CellIndex]++;
                        can.SetRemaining(e.RemainingAfter);
                        _audio?.Play(GardenSound.Water, 1f + cascade * 0.03f);
                        if (!Reduced)
                        {
                            StartCoroutine(Pour(can));
                            yield return Jet(can, e.CellIndex);
                        }
                        yield return Shrink(FirstVisible(_cellContent[e.CellIndex]), Step * 0.35f);

                        bool stackDone = bloomed[e.CellIndex] >= _level.Cells[e.CellIndex].Count;
                        GameObject shown = ShowCell(e.CellIndex, bloomed[e.CellIndex], animate: !stackDone);
                        if (stackDone && !Reduced) StartCoroutine(Unfold(shown, Step * 1.8f));
                        if (!Reduced) Burst(CellWorld(e.CellIndex) + Vector3.up * 0.5f, e.Color, 6);
                        _audio?.PlayBloom(cascade++);
                        BloomShown?.Invoke();
                        yield return Wait(Step * 0.6f);
                        break;
                    }

                    case GameEventType.CanEmptied:
                        _audio?.Play(GardenSound.Empty);
                        yield return Leave(_cans[e.CanId]);
                        _cans[e.CanId].gameObject.SetActive(false);
                        break;
                }
            }
        }

        /// <summary>Win celebration: a wave over the finished garden, then falling petals. Never touches the state.</summary>
        public IEnumerator Celebrate()
        {
            _audio?.Play(GardenSound.Win);
            if (Reduced)
            {
                yield return Wait(0.3f);
                yield break;
            }
            for (int i = 0; i < _cellContent.Count; i++)
            {
                Transform flower = FirstVisible(_cellContent[i]);
                if (flower != null && flower.name.StartsWith("Bloom")) StartCoroutine(Pop(flower, Step * 1.5f));
                if (i % _level.Width == _level.Width - 1) yield return Wait(Step * 0.35f);
            }
            for (int i = 0; i < 28; i++)
            {
                ColorId color = (ColorId)(i % ColorIds.Count);
                var petal = _pool.Get("Petal " + color.ToId(), p =>
                {
                    var go = Prim(PrimitiveType.Sphere, p, palette.Flower(color), Vector3.zero, Vector3.one);
                    go.name = "Petal";
                    return go;
                }, _effectsRoot);
                float x = ((i * 47) % 100) / 100f * 6f - 3f;
                float z = _boardCenterZ + ((i * 31) % 100) / 100f * 4f - 2f;
                StartCoroutine(Fall(petal.transform, new Vector3(x, 4.5f + (i % 5) * 0.4f, z), i));
            }
            yield return Wait(1.1f * (_settings?.DurationScale ?? 1f));
        }

        private IEnumerator SendToPlace(GameEvent e, GameState before)
        {
            CanView moving = _cans[e.CanId];
            moving.SetDimmed(false);
            var lane = _level.Lanes[e.Lane];
            int head = before.LaneHead(e.Lane);
            var followers = new List<(CanView c, Vector3 from, Vector3 to, float s0, float s1)>();
            for (int k = head + 1; k < lane.Count && k - head <= VisibleCansPerLane; k++)
            {
                CanView c = _cans[lane[k].Id];
                int depth = k - head - 1;
                c.gameObject.SetActive(true);
                c.SetDimmed(depth > 0);
                followers.Add((c, c.transform.localPosition, LanePosition(e.Lane, depth), c.transform.localScale.x, depth == 0 ? 1f : 0.82f));
            }

            Vector3 from = moving.transform.localPosition, to = SlotPosition(e.Slot);
            float duration = Step * 1.4f;
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                float k = Mathf.SmoothStep(0f, 1f, t);
                float arc = Reduced ? 0f : Mathf.Sin(k * Mathf.PI) * 0.9f;
                moving.transform.localPosition = Vector3.Lerp(from, to, k) + Vector3.up * arc;
                if (!Reduced) moving.Body.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(k * Mathf.PI) * -12f);
                foreach (var f in followers)
                {
                    f.c.transform.localPosition = Vector3.Lerp(f.from, f.to, k);
                    f.c.transform.localScale = Vector3.one * Mathf.Lerp(f.s0, f.s1, k);
                }
                yield return null;
            }
            moving.transform.localPosition = to;
            moving.ResetPose();
            foreach (var f in followers)
            {
                f.c.transform.localPosition = f.to;
                f.c.transform.localScale = Vector3.one * f.s1;
            }
            if (!Reduced) yield return Squash(moving.Body, Step * 0.5f);
        }

        /// <summary>Water drops flying in an arc from the spout to the bud. The logic does not wait for them.</summary>
        private IEnumerator Jet(CanView can, int cell)
        {
            Vector3 start = can.transform.TransformPoint(CanView.SpoutTip);
            Vector3 end = CellWorld(cell) + Vector3.up * 0.6f;
            const int drops = 5;
            for (int i = 0; i < drops; i++)
            {
                var drop = _pool.Get("Drop", p => FlowerFactory.Drop(palette, p), _effectsRoot);
                StartCoroutine(Fly(drop, start, end, Step * 0.9f));
                yield return Wait(Step * 0.08f);
            }
            yield return Wait(Step * 0.5f);
        }

        private IEnumerator Fly(GameObject drop, Vector3 start, Vector3 end, float duration)
        {
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                drop.transform.position = Vector3.Lerp(start, end, t) + Vector3.up * Mathf.Sin(t * Mathf.PI) * 1.4f;
                yield return null;
            }
            _pool.Release(drop);
        }

        private static IEnumerator Pour(CanView can)
        {
            const float duration = 0.3f;
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                can.Body.localRotation = Quaternion.Euler(0f, 0f, -Mathf.Sin(t * Mathf.PI) * 28f);
                yield return null;
            }
            can.Body.localRotation = Quaternion.identity;
        }

        private IEnumerator Leave(CanView can)
        {
            float duration = Step;
            Vector3 start = can.transform.localScale;
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                if (!Reduced) can.Body.localPosition = Vector3.up * Mathf.Sin(t * Mathf.PI) * 0.4f;
                can.transform.localScale = start * (1f - t);
                yield return null;
            }
            can.ResetPose();
        }

        private void Burst(Vector3 origin, ColorId color, int count)
        {
            Material material = palette.Flower(color);
            for (int i = 0; i < count; i++)
            {
                var spark = _pool.Get("Sparkle " + color.ToId(), p => FlowerFactory.Sparkle(material, p), _effectsRoot);
                Vector3 dir = Quaternion.Euler(0f, i * 360f / count + 15f, 0f) * new Vector3(0f, 1.2f, 0.7f);
                StartCoroutine(Spark(spark, origin, dir, Step * 2f));
            }
        }

        private IEnumerator Spark(GameObject spark, Vector3 origin, Vector3 velocity, float duration)
        {
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                spark.transform.position = origin + velocity * t + Vector3.down * t * t * 0.8f;
                spark.transform.localScale = Vector3.one * 0.1f * (1f - t);
                yield return null;
            }
            _pool.Release(spark);
        }

        private IEnumerator Fall(Transform petal, Vector3 start, int seed)
        {
            float duration = 1.6f + (seed % 4) * 0.15f;
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                petal.position = start + new Vector3(Mathf.Sin(t * 6f + seed) * 0.35f, -t * 5.5f, 0f);
                petal.localRotation = Quaternion.Euler(t * 360f + seed * 20f, seed * 30f, t * 180f);
                petal.localScale = new Vector3(0.16f, 0.04f, 0.24f) * (t < 0.85f ? 1f : (1f - t) / 0.15f);
                yield return null;
            }
            _pool.Release(petal.gameObject);
        }

        private static IEnumerator Unfold(GameObject bloom, float duration)
        {
            FlowerFactory.SetBloomOpen(bloom, 0f);
            for (float t = 0f; t < 1f && bloom != null; t += Time.deltaTime / duration)
            {
                // Ease-out with a slight overshoot, then settle open.
                float k = 1f - Mathf.Pow(1f - t, 3f);
                FlowerFactory.SetBloomOpen(bloom, k + Mathf.Sin(t * Mathf.PI) * 0.12f);
                yield return null;
            }
            if (bloom != null) FlowerFactory.SetBloomOpen(bloom, 1f);
        }

        private static IEnumerator Squash(Transform body, float duration)
        {
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                float s = Mathf.Sin(t * Mathf.PI) * 0.12f;
                body.localScale = new Vector3(1f + s, 1f - s, 1f + s);
                yield return null;
            }
            body.localScale = Vector3.one;
        }

        private static IEnumerator Shrink(Transform target, float duration)
        {
            if (target == null) yield break;
            Vector3 start = target.localScale;
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                target.localScale = start * (1f - t);
                yield return null;
            }
            target.localScale = Vector3.zero;
        }

        private static IEnumerator Pop(Transform target, float duration)
        {
            for (float t = 0f; t < 1f && target != null; t += Time.deltaTime / duration)
            {
                target.localScale = Vector3.one * (1f + Mathf.Sin(t * Mathf.PI) * 0.25f) * Mathf.Min(1f, 0.3f + t * 2.5f);
                yield return null;
            }
            if (target != null) target.localScale = Vector3.one;
        }

        private static IEnumerator Wait(float seconds)
        {
            for (float t = 0f; t < seconds; t += Time.deltaTime) yield return null;
        }

        private static Transform FirstVisible(Transform root)
        {
            foreach (Transform child in root)
                if (child.gameObject.activeSelf && child.name != "Hidden layer") return child;
            return null;
        }

        private Vector3 CellWorld(int cell) => transform.TransformPoint(CellPosition(cell));

        // ------------------------------------------------------------------ helpers

        private Transform NewRoot(string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            return go.transform;
        }

        /// <summary>Primitive without collider. <paramref name="castShadows"/> for volumes; flat pads only receive shadows.</summary>
        internal static GameObject Prim(PrimitiveType type, Transform parent, Material material, Vector3 localPosition, Vector3 localScale,
            bool castShadows = false)
        {
            var go = GameObject.CreatePrimitive(type);
            Object.Destroy(go.GetComponent<Collider>());
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = localScale;
            var renderer = go.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = castShadows ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
            return go;
        }
    }
}
