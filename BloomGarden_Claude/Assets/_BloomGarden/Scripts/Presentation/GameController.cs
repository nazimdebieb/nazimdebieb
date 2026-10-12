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
    public enum GameScreen
    {
        Home,
        Levels,
        Playing,
    }

    /// <summary>
    /// Scene entry point and game flow (P5): home → level selection → game, progress and save.
    /// A tap becomes exactly one logical move (<see cref="GameSession.SelectLane"/>), then the returned events are
    /// animated. The logical state is updated before any animation, so animations never gate rules: a tap during
    /// an animation first snaps the view to the current state, then plays the new move.
    /// Every accepted move, undo and restart is saved as the list of lanes played, so an interrupted game resumes
    /// exactly (deterministic replay), undo history included.
    /// </summary>
    public sealed class GameController : MonoBehaviour
    {
        /// <summary>Tests point this to a temporary folder so they never touch the player's real save.</summary>
        public static string SaveDirectoryOverride;

        [SerializeField] private LevelCatalog catalog;
        [SerializeField] private GardenPalette palette;
        [SerializeField] private Camera viewCamera;

        private GardenView _garden;
        private HudView _hud;
        private MenuView _menu;
        private GardenAudio _audio;
        private SaveService _saves;
        private SaveData _save;
        private GameSession _session;
        private readonly List<int> _lanes = new List<int>();
        private int _levelIndex;
        private Coroutine _animation;
        private bool _endShown;
        private int _displayedBloomed;

        public GameSettings Settings { get; private set; }
        public GardenAudio Audio => _audio;
        public GameSession Session => _session;
        public int LevelIndex => _levelIndex;
        public bool IsAnimating => _animation != null;
        public bool EndPanelVisible => _hud != null && _hud.PanelVisible;
        public GardenView Garden => _garden;
        public HudView Hud => _hud;
        public MenuView Menu => _menu;
        public SaveData SaveData => _save;
        public SaveService Saves => _saves;
        public GameScreen CurrentScreen { get; private set; }
        public int LevelCount => catalog.Count;

        private void Awake()
        {
            if (viewCamera == null) viewCamera = Camera.main;
            _saves = new SaveService(SaveDirectoryOverride ?? Application.persistentDataPath);
            _save = _saves.Load();
            _save.unlocked = Mathf.Clamp(_save.unlocked, 1, catalog.Count);
            Debug.Log($"[BloomGarden] Save {_saves.LastLoadSource}: unlocked={_save.unlocked} completed={_save.records.Count}");

            Settings = new GameSettings(_save.settings);
            Settings.Changed += Persist;

            _audio = new GameObject("Audio").AddComponent<GardenAudio>();
            _audio.transform.SetParent(transform, false);
            _audio.Init(Settings);
            _garden = new GameObject("Garden").AddComponent<GardenView>();
            _garden.transform.SetParent(transform, false);
            _garden.Palette = palette;
            _garden.Init(Settings, _audio);
            _garden.BloomShown += () => _hud.SetCounter(++_displayedBloomed, _session.Level.TotalBuds);

            _hud = new GameObject("UI").AddComponent<HudView>();
            _hud.transform.SetParent(transform, false);
            _hud.Build(Settings, LaneHeadScreenPosition);
            _hud.UndoClicked += Undo;
            _hud.RestartClicked += Restart;
            _hud.MenuClicked += ShowHome;
            _hud.PanelPrimaryClicked += OnPanelPrimary;
            _hud.PanelSecondaryClicked += OnPanelSecondary;

            _menu = _hud.gameObject.AddComponent<MenuView>();
            _menu.Build(_hud.Ui, _hud.Safe, catalog.Count);
            _menu.PlayClicked += () => Play(ContinueLevelIndex());
            _menu.LevelsClicked += ShowLevels;
            _menu.OptionsClicked += _hud.ToggleSettings;
            _menu.BackClicked += ShowHome;
            _menu.LevelChosen += index => { if (index < _save.unlocked) Play(index); else _audio.Play(GardenSound.Refused); };
        }

        private void Start() => ShowHome();

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
            if (CurrentScreen != GameScreen.Playing || _hud.PanelVisible || _hud.SettingsVisible) return false;
            Ray ray = viewCamera.ScreenPointToRay(screenPosition);
            return Physics.Raycast(ray, out RaycastHit hit, 200f) && hit.collider.TryGetComponent(out LaneTapTarget target)
                && SelectLane(target.lane);
        }

        /// <summary>Screen position of a lane head (tests, captures and the tutorial arrow).</summary>
        public Vector2 LaneHeadScreenPosition(int lane) =>
            viewCamera.WorldToScreenPoint(_garden.transform.TransformPoint(GardenView.LanePosition(lane, 0) + Vector3.up * 0.5f));

        // ------------------------------------------------------------------ screens

        /// <summary>Level to offer on "Play": the game in progress, else the first unfinished unlocked level.</summary>
        public int ContinueLevelIndex()
        {
            int inProgress = IndexOf(_save.progress.levelId);
            if (inProgress >= 0) return inProgress;
            for (int i = 0; i < _save.unlocked; i++)
                if (!_save.IsCompleted(LevelId(i))) return i;
            return Mathf.Min(_save.unlocked, catalog.Count) - 1;
        }

        public void ShowHome()
        {
            SnapAndPersist();
            CurrentScreen = GameScreen.Home;
            int next = ContinueLevelIndex();
            bool allDone = _save.records.Count >= catalog.Count;
            bool resumable = IndexOf(_save.progress.levelId) >= 0 || _save.records.Count > 0;
            // The living diorama behind the home screen is the level the player will continue.
            if (_session == null || _levelIndex != next) LoadLevelInternal(next, resume: true);
            _hud.SetGameLayerVisible(false);
            _hud.ShowHint(TutorialHint.None);
            _menu.ShowHome(resumable ? next + 1 : 0, allDone);
        }

        public void ShowLevels()
        {
            CurrentScreen = GameScreen.Levels;
            _menu.ShowLevels(_save.unlocked, i => _save.IsCompleted(LevelId(i)));
        }

        /// <summary>Starts (or resumes) a level and shows the game screen.</summary>
        public void Play(int index)
        {
            bool sameLevel = _session != null && _levelIndex == index;
            if (!sameLevel) LoadLevelInternal(index, resume: true);
            CurrentScreen = GameScreen.Playing;
            _menu.Hide();
            _hud.SetGameLayerVisible(true);
            Refresh();
        }

        /// <summary>Loads a level fresh and goes straight to the game screen (ignores any saved game of that level).</summary>
        public void LoadLevel(int index)
        {
            LoadLevelInternal(index, resume: false);
            CurrentScreen = GameScreen.Playing;
            _menu.Hide();
            _hud.SetGameLayerVisible(true);
            Refresh();
            PersistProgress();
        }

        private void LoadLevelInternal(int index, bool resume)
        {
            StopAnimation();
            _garden.CancelAnimations();
            _levelIndex = Mathf.Clamp(index, 0, catalog.Count - 1);
            if (!catalog.TryLoad(_levelIndex, out LevelDefinition level, out ValidationResult result))
            {
                Debug.LogError($"[BloomGarden] Level {_levelIndex + 1} is invalid: {result}");
                return;
            }
            _session = new GameSession(level);
            _completedAtLoad = _save.IsCompleted(level.Id);
            _lanes.Clear();
            if (resume && _save.progress.levelId == level.Id && _save.progress.lanes.Count > 0)
            {
                if (_session.Replay(_save.progress.lanes)) _lanes.AddRange(_save.progress.lanes);
                else
                {
                    Debug.LogWarning($"[BloomGarden] Saved game of {level.Id} could not be replayed; starting fresh.");
                    _session.Restart();
                }
            }
            _garden.Build(level);
            _hud.SetLevel(_levelIndex + 1);
            Refresh();
            Debug.Log($"[BloomGarden] Loaded {level.Id} ({level.Width}x{level.Height}, {level.TotalBuds} buds, {_lanes.Count} moves restored)");
        }

        // ------------------------------------------------------------------ moves

        /// <summary>One tap = at most one logical move. Returns true when the move was accepted.</summary>
        public bool SelectLane(int lane)
        {
            if (_session == null || _endShown || CurrentScreen != GameScreen.Playing) return false;
            if (StopAnimation()) _garden.ShowState(_session.State); // snap the interrupted move first
            GameState before = _session.State;
            MoveResult result = _session.SelectLane(lane);
            if (!result.Accepted)
            {
                _audio.Play(GardenSound.Refused);
                Debug.Log($"[BloomGarden] Lane {lane + 1} refused: {result.Rejection}");
                return false;
            }
            _lanes.Add(lane);
            if (_session.State.IsWon) RecordWin();
            else PersistProgress();
            _displayedBloomed = _session.Level.TotalBuds - before.RemainingBuds(_session.Level);
            _hud.ShowHint(Tutorial.For(_levelIndex, _session.Level, _session.State, LevelCompletedBefore()));
            _animation = StartCoroutine(Animate(before, result.Events));
            return true;
        }

        public void Undo()
        {
            if (_session == null || !_session.Undo()) return;
            StopAnimation();
            if (_lanes.Count > 0) _lanes.RemoveAt(_lanes.Count - 1);
            PersistProgress();
            _audio.Play(GardenSound.Undo);
            Refresh();
        }

        public void Restart()
        {
            if (_session == null) return;
            if (StopAnimation()) _garden.ShowState(_session.State);
            _session.Restart();
            _lanes.Clear();
            PersistProgress();
            Refresh();
        }

        public void NextLevel()
        {
            if (_levelIndex + 1 < catalog.Count) Play(_levelIndex + 1);
            else ShowHome();
        }

        private void OnPanelPrimary()
        {
            if (_session.State.IsWon) NextLevel();
            else Undo();
        }

        private void OnPanelSecondary() => Restart();

        /// <summary>Tutorial hints are shown until the level has been completed once (state when the level was loaded).</summary>
        private bool LevelCompletedBefore() => _completedAtLoad;

        private bool _completedAtLoad;

        private void RecordWin()
        {
            _save.RecordWin(_session.Level.Id, _session.State.MovesMade);
            _save.unlocked = Mathf.Clamp(Mathf.Max(_save.unlocked, _levelIndex + 2), 1, catalog.Count);
            _save.progress = new ProgressData();
            _lanes.Clear();
            Persist();
        }

        // ------------------------------------------------------------------ save

        private void PersistProgress()
        {
            if (_session == null) return;
            _save.progress.levelId = _lanes.Count > 0 ? _session.Level.Id : "";
            _save.progress.lanes = new List<int>(_lanes);
            Persist();
        }

        private void Persist()
        {
            try { _saves.Save(_save); }
            catch (System.Exception e) { Debug.LogError("[BloomGarden] Save failed: " + e.Message); }
        }

        /// <summary>Background / pause (P5-07): no animation is left half-way and the game is saved.</summary>
        private void OnApplicationPause(bool paused)
        {
            if (paused) SnapAndPersist();
        }

        private void OnApplicationQuit() => SnapAndPersist();

        public void SnapAndPersist()
        {
            if (_session == null) return;
            if (StopAnimation()) Refresh();
            if (!_session.State.IsWon) PersistProgress();
        }

        // ------------------------------------------------------------------ view sync

        private IEnumerator Animate(GameState before, IReadOnlyList<GameEvent> events)
        {
            _hud.HidePanel();
            yield return _garden.PlayEvents(before, events);
            if (_session.State.IsWon) yield return _garden.Celebrate();
            else if (_session.State.IsDeadlocked) _audio.Play(GardenSound.Deadlock);
            _animation = null;
            Refresh();
        }

        /// <summary>Stops the running animation, if any. Returns true when one was interrupted.</summary>
        private bool StopAnimation()
        {
            if (_animation == null) return false;
            StopCoroutine(_animation);
            _garden.CancelAnimations();
            _animation = null;
            return true;
        }

        /// <summary>Snaps every visual to the authoritative state and shows the end panel / tutorial when relevant.</summary>
        private void Refresh()
        {
            if (_session == null) return;
            GameState state = _session.State;
            LevelDefinition level = _session.Level;
            _garden.ShowState(state);
            _displayedBloomed = level.TotalBuds - state.RemainingBuds(level);
            _hud.SetCounter(_displayedBloomed, level.TotalBuds);
            _hud.SetUndoInteractable(_session.CanUndo);
            bool playing = CurrentScreen == GameScreen.Playing;
            _hud.ShowHint(playing ? Tutorial.For(_levelIndex, level, state, LevelCompletedBefore()) : TutorialHint.None);

            _endShown = false;
            if (!playing)
            {
                _hud.HidePanel();
            }
            else if (state.IsWon)
            {
                _endShown = true;
                bool last = _levelIndex == catalog.Count - 1;
                int best = _save.Record(level.Id)?.bestMoves ?? state.MovesMade;
                _hud.ShowPanel(new PanelContent("win_title",
                    best < state.MovesMade ? "win_body_best" : "win_body",
                    best < state.MovesMade ? new object[] { state.MovesMade, best } : new object[] { state.MovesMade },
                    last ? "back_home" : "next", "replay"));
            }
            else if (state.IsDeadlocked)
            {
                _endShown = true;
                _hud.ShowPanel(new PanelContent("dead_title", "dead_body", null, "dead_undo", "retry"));
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

        private string[] _ids;

        private string LevelId(int index)
        {
            if (_ids == null)
            {
                _ids = new string[catalog.Count];
                for (int i = 0; i < _ids.Length; i++)
                    _ids[i] = catalog.TryLoad(i, out LevelDefinition level, out _) ? level.Id : $"BG-{i + 1:000}";
            }
            return _ids[index];
        }

        private int IndexOf(string levelId)
        {
            if (string.IsNullOrEmpty(levelId)) return -1;
            for (int i = 0; i < catalog.Count; i++) if (LevelId(i) == levelId) return i;
            return -1;
        }
    }
}
