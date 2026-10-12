using System.Collections;
using System.Collections.Generic;
using BloomGarden.Core.Levels;
using BloomGarden.Core.Model;
using BloomGarden.Core.Rules;
using BloomGarden.Levels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BloomGarden.Presentation
{
    /// <summary>
    /// Scene entry point: loads a level from the catalog, turns taps into exactly one logical move
    /// (<see cref="GameSession.SelectLane"/>), then animates the returned events. The logical state is updated
    /// before any animation, so animations never gate rules: a tap during an animation first snaps the view
    /// to the current state, then plays the new move.
    /// </summary>
    public sealed class GameController : MonoBehaviour
    {
        [SerializeField] private LevelCatalog catalog;
        [SerializeField] private GardenPalette palette;
        [SerializeField] private int startLevelIndex;
        [SerializeField] private Camera viewCamera;

        private GardenView _garden;
        private HudView _hud;
        private GameSession _session;
        private int _levelIndex;
        private Coroutine _animation;
        private bool _endShown;
        private int _displayedBloomed;

        public GameSession Session => _session;
        public int LevelIndex => _levelIndex;
        public bool IsAnimating => _animation != null;
        public bool EndPanelVisible => _hud != null && _hud.PanelVisible;
        public GardenView Garden => _garden;

        private void Awake()
        {
            if (viewCamera == null) viewCamera = Camera.main;
            _garden = new GameObject("Garden").AddComponent<GardenView>();
            _garden.transform.SetParent(transform, false);
            _garden.Palette = palette;
            _garden.BloomShown += () => _hud.SetCounter(++_displayedBloomed, _session.Level.TotalBuds);

            _hud = new GameObject("UI").AddComponent<HudView>();
            _hud.transform.SetParent(transform, false);
            _hud.Build();
            _hud.UndoClicked += Undo;
            _hud.RestartClicked += Restart;
            _hud.PanelPrimaryClicked += OnPanelPrimary;
            _hud.PanelSecondaryClicked += OnPanelSecondary;
        }

        private void Start() => LoadLevel(startLevelIndex);

        private void Update()
        {
            FitCamera();
            if (!Input.GetMouseButtonDown(0)) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            if (Input.touchCount > 0 && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;
            TapAt(Input.mousePosition);
        }

        /// <summary>Screen tap on the garden: only a lane head (its collider) maps to a move. Returns the move result.</summary>
        public bool TapAt(Vector2 screenPosition)
        {
            if (_hud.PanelVisible) return false;
            Ray ray = viewCamera.ScreenPointToRay(screenPosition);
            return Physics.Raycast(ray, out RaycastHit hit, 200f) && hit.collider.TryGetComponent(out LaneTapTarget target)
                && SelectLane(target.lane);
        }

        /// <summary>Screen position of a lane head (used by tests and captures to tap like a player).</summary>
        public Vector2 LaneHeadScreenPosition(int lane) =>
            viewCamera.WorldToScreenPoint(_garden.transform.TransformPoint(GardenView.LanePosition(lane, 0) + Vector3.up * 0.5f));

        // ------------------------------------------------------------------ level flow

        public void LoadLevel(int index)
        {
            StopAnimation();
            if (catalog == null || catalog.Count == 0)
            {
                Debug.LogError("[BloomGarden] No level catalog assigned to GameController");
                return;
            }
            _levelIndex = ((index % catalog.Count) + catalog.Count) % catalog.Count;
            if (!catalog.TryLoad(_levelIndex, out LevelDefinition level, out ValidationResult result))
            {
                Debug.LogError($"[BloomGarden] Level {_levelIndex + 1} is invalid: {result}");
                return;
            }
            _session = new GameSession(level);
            _garden.Build(level);
            _hud.SetTitle($"Niveau {_levelIndex + 1} — {level.Title}");
            Refresh();
            Debug.Log($"[BloomGarden] Loaded {level.Id} ({level.Width}x{level.Height}, {level.TotalBuds} buds)");
        }

        /// <summary>One tap = at most one logical move. Returns true when the move was accepted.</summary>
        public bool SelectLane(int lane)
        {
            if (_session == null || _endShown) return false;
            if (StopAnimation()) _garden.ShowState(_session.State); // snap the interrupted move first
            GameState before = _session.State;
            MoveResult result = _session.SelectLane(lane);
            if (!result.Accepted)
            {
                Debug.Log($"[BloomGarden] Lane {lane + 1} refused: {result.Rejection}");
                return false;
            }
            _displayedBloomed = _session.Level.TotalBuds - before.RemainingBuds(_session.Level);
            _animation = StartCoroutine(Animate(before, result.Events));
            return true;
        }

        public void Undo()
        {
            if (_session == null || !_session.Undo()) return;
            StopAnimation();
            Refresh();
        }

        public void Restart()
        {
            if (_session == null) return;
            StopAnimation();
            _session.Restart();
            Refresh();
        }

        public void NextLevel() => LoadLevel(_levelIndex + 1);

        private void OnPanelPrimary()
        {
            if (_session.State.IsWon) NextLevel();
            else Undo();
        }

        private void OnPanelSecondary() => Restart();

        // ------------------------------------------------------------------ view sync

        private IEnumerator Animate(GameState before, IReadOnlyList<GameEvent> events)
        {
            _hud.HidePanel();
            yield return _garden.PlayEvents(before, events);
            _animation = null;
            Refresh();
        }

        /// <summary>Stops the running animation, if any. Returns true when one was interrupted.</summary>
        private bool StopAnimation()
        {
            if (_animation == null) return false;
            StopCoroutine(_animation);
            _garden.StopAllCoroutines();
            _animation = null;
            // Leftover transient objects (water drops) belong to the stopped animation.
            foreach (Transform child in _garden.transform)
                if (child.name == "Drop") Destroy(child.gameObject);
            return true;
        }

        /// <summary>Snaps every visual to the authoritative state and shows the end panel when relevant.</summary>
        private void Refresh()
        {
            GameState state = _session.State;
            LevelDefinition level = _session.Level;
            _garden.ShowState(state);
            _displayedBloomed = level.TotalBuds - state.RemainingBuds(level);
            _hud.SetCounter(_displayedBloomed, level.TotalBuds);
            _hud.SetUndoInteractable(_session.CanUndo);

            _endShown = false;
            if (state.IsWon)
            {
                _endShown = true;
                bool last = _levelIndex == catalog.Count - 1;
                _hud.ShowPanel("Jardin en fleurs !", $"{level.Title} terminé en {state.MovesMade} coups.",
                    last ? "Recommencer au niveau 1" : "Niveau suivant", "Rejouer");
            }
            else if (state.IsDeadlocked)
            {
                _endShown = true;
                _hud.ShowPanel("Plus de place", "Les trois places sont occupées et aucune fleur ne peut être arrosée.",
                    "Annuler le dernier coup", "Réessayer");
            }
            else
            {
                _hud.HidePanel();
            }
        }

        /// <summary>Orthographic framing that keeps the whole garden visible on any portrait aspect ratio.</summary>
        private void FitCamera()
        {
            if (viewCamera == null || !viewCamera.orthographic) return;
            const float neededHalfWidth = 3.35f;
            const float neededHalfHeight = 7.4f;
            float aspect = Mathf.Max(0.01f, viewCamera.aspect);
            viewCamera.orthographicSize = Mathf.Max(neededHalfHeight, neededHalfWidth / aspect);
        }
    }
}
