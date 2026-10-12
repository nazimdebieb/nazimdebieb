using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BloomGarden.Presentation
{
    /// <summary>
    /// Home screen (GDD screen A: name, living diorama behind, "Play") and level selection (screen B: completed,
    /// current and locked levels). Built on the HUD canvas, above the game layer. Texts come from <see cref="Loc"/>.
    /// </summary>
    public sealed class MenuView : MonoBehaviour
    {
        public const int Columns = 4;

        private UiKit _ui;
        private RectTransform _home, _levels;
        private Text _appTitle, _subtitle, _playLabel, _levelsLabel, _optionsLabel, _doneLabel;
        private Text _levelsTitle, _progress, _backLabel;
        private readonly List<(Button button, Image image, Text label)> _levelButtons = new List<(Button, Image, Text)>();

        private int _continueLevel;
        private bool _allDone;
        private int _count, _unlocked, _completedCount;
        private Func<int, bool> _isCompleted;

        public bool HomeVisible => _home.gameObject.activeSelf;
        public bool LevelsVisible => _levels.gameObject.activeSelf;
        public bool AnyVisible => HomeVisible || LevelsVisible;

        public event Action PlayClicked, LevelsClicked, OptionsClicked, BackClicked;
        public event Action<int> LevelChosen;

        public void Build(UiKit ui, RectTransform parent, int levelCount)
        {
            _ui = ui;
            _count = levelCount;
            BuildHome(parent);
            BuildLevels(parent);
            Loc.Changed += ApplyTexts;
        }

        private void OnDestroy() => Loc.Changed -= ApplyTexts;

        private void BuildHome(RectTransform parent)
        {
            _home = UiKit.Stretch(parent, "Home");
            var top = UiKit.Panel(_home, "Title band", new Color(UiKit.Ivory.r, UiKit.Ivory.g, UiKit.Ivory.b, 0.9f));
            UiKit.Place(top.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -230f), new Vector2(0f, 460f));
            _appTitle = _ui.Label(top.transform, "App title", 120, FontStyle.Bold, TextAnchor.MiddleCenter);
            UiKit.Place(_appTitle.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -170f), new Vector2(-60f, 160f));
            _subtitle = _ui.Label(top.transform, "Subtitle", 46, FontStyle.Italic, TextAnchor.MiddleCenter);
            UiKit.Place(_subtitle.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -330f), new Vector2(-80f, 130f));

            var bottom = UiKit.Panel(_home, "Button band", new Color(UiKit.Ivory.r, UiKit.Ivory.g, UiKit.Ivory.b, 0.9f));
            UiKit.Place(bottom.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 330f), new Vector2(0f, 660f));
            _doneLabel = _ui.Label(bottom.transform, "All done", 44, FontStyle.Bold, TextAnchor.MiddleCenter);
            UiKit.Place(_doneLabel.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -60f), new Vector2(-80f, 70f));
            var play = _ui.Button(bottom.transform, "Play", UiKit.Leaf, 58, out _playLabel);
            UiKit.At((RectTransform)play.transform, 0.5f, 0f, new Vector2(0f, 470f), new Vector2(780f, 160f));
            play.onClick.AddListener(() => PlayClicked?.Invoke());
            var levels = _ui.Button(bottom.transform, "Levels", UiKit.Paper, 48, out _levelsLabel);
            UiKit.At((RectTransform)levels.transform, 0.5f, 0f, new Vector2(0f, 300f), new Vector2(780f, 130f));
            levels.onClick.AddListener(() => LevelsClicked?.Invoke());
            var options = _ui.Button(bottom.transform, "Options", UiKit.Paper, 48, out _optionsLabel);
            UiKit.At((RectTransform)options.transform, 0.5f, 0f, new Vector2(0f, 140f), new Vector2(780f, 130f));
            options.onClick.AddListener(() => OptionsClicked?.Invoke());
        }

        private void BuildLevels(RectTransform parent)
        {
            _levels = UiKit.Stretch(parent, "Levels");
            UiKit.Panel(_levels, "Background", new Color(UiKit.Ivory.r, UiKit.Ivory.g, UiKit.Ivory.b, 0.95f)).rectTransform.anchorMax = Vector2.one;
            var bg = (RectTransform)_levels.GetChild(0);
            bg.anchorMin = Vector2.zero;
            bg.offsetMin = bg.offsetMax = Vector2.zero;

            _levelsTitle = _ui.Label(_levels, "Title", 72, FontStyle.Bold, TextAnchor.MiddleCenter);
            UiKit.Place(_levelsTitle.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -150f), new Vector2(-80f, 110f));
            _progress = _ui.Label(_levels, "Progress", 44, FontStyle.Normal, TextAnchor.MiddleCenter);
            UiKit.Place(_progress.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -240f), new Vector2(-80f, 70f));

            const float size = 200f, gap = 28f;
            int rows = Mathf.CeilToInt(_count / (float)Columns);
            for (int i = 0; i < _count; i++)
            {
                int index = i;
                var button = _ui.Button(_levels, $"Level {i + 1}", UiKit.Paper, 64, out Text label);
                int row = i / Columns, col = i % Columns;
                float x = (col - (Columns - 1) / 2f) * (size + gap);
                float y = 330f - row * (size + gap) - (rows - 5) * (size + gap) / 2f;
                UiKit.At((RectTransform)button.transform, 0.5f, 0.5f, new Vector2(x, y - 80f), new Vector2(size, size));
                button.onClick.AddListener(() => LevelChosen?.Invoke(index));
                _levelButtons.Add((button, button.GetComponent<Image>(), label));
            }

            var back = _ui.Button(_levels, "Back", UiKit.Paper, 46, out _backLabel);
            UiKit.At((RectTransform)back.transform, 0.5f, 0f, new Vector2(0f, 120f), new Vector2(520f, 130f));
            back.onClick.AddListener(() => BackClicked?.Invoke());
            _levels.gameObject.SetActive(false);
        }

        public void ShowHome(int continueLevel, bool allDone)
        {
            _continueLevel = continueLevel;
            _allDone = allDone;
            _levels.gameObject.SetActive(false);
            _home.gameObject.SetActive(true);
            ApplyTexts();
        }

        /// <summary>Level grid: completed levels in leaf green, the next level to play highlighted, the rest locked.</summary>
        public void ShowLevels(int unlocked, Func<int, bool> isCompleted)
        {
            _unlocked = unlocked;
            _isCompleted = isCompleted;
            _completedCount = 0;
            for (int i = 0; i < _count; i++) if (isCompleted(i)) _completedCount++;
            for (int i = 0; i < _levelButtons.Count; i++)
            {
                var (button, image, label) = _levelButtons[i];
                bool open = i < unlocked;
                bool done = isCompleted(i);
                button.interactable = open;
                image.color = done ? UiKit.Leaf : open ? new Color32(0xFF, 0xE3, 0x9A, 0xFF) : UiKit.Muted;
                label.text = (i + 1).ToString();
                label.color = open ? UiKit.Ink : new Color(UiKit.Ink.r, UiKit.Ink.g, UiKit.Ink.b, 0.35f);
            }
            _home.gameObject.SetActive(false);
            _levels.gameObject.SetActive(true);
            ApplyTexts();
        }

        public void Hide()
        {
            _home.gameObject.SetActive(false);
            _levels.gameObject.SetActive(false);
        }

        public bool IsLevelButtonInteractable(int index) => _levelButtons[index].button.interactable;

        private void ApplyTexts()
        {
            _appTitle.text = Loc.T("app_title");
            _subtitle.text = Loc.T("home_subtitle");
            _playLabel.text = _continueLevel > 0 ? Loc.T("home_continue", _continueLevel) : Loc.T("home_play");
            _doneLabel.text = _allDone ? Loc.T("home_all_done") : "";
            _levelsLabel.text = Loc.T("home_levels");
            _optionsLabel.text = Loc.T("options");
            _levelsTitle.text = Loc.T("levels_title");
            _progress.text = Loc.T("levels_progress", _completedCount, _count);
            _backLabel.text = Loc.T("back");
        }
    }
}
