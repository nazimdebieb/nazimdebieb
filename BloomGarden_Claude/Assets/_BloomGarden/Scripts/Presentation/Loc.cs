using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloomGarden.Presentation
{
    public enum Language
    {
        French = 0,
        English = 1,
    }

    /// <summary>
    /// FR/EN text table (P5-05). Every player-facing string goes through <see cref="T"/>; a missing key is shown
    /// as "#key" so it cannot go unnoticed. The default language follows the device (French if the system is
    /// French, English otherwise) until the player picks one in Options.
    /// </summary>
    public static class Loc
    {
        private static readonly Dictionary<string, string[]> Table = new Dictionary<string, string[]>
        {
            ["app_title"] = new[] { "Bloom Garden", "Bloom Garden" },
            ["home_subtitle"] = new[] { "Un jardin à faire fleurir,\nune goutte à la fois.", "A garden to bring to bloom,\none drop at a time." },
            ["home_play"] = new[] { "Jouer", "Play" },
            ["home_continue"] = new[] { "Continuer : niveau {0}", "Continue: level {0}" },
            ["home_levels"] = new[] { "Niveaux", "Levels" },
            ["home_all_done"] = new[] { "Les 20 jardins sont en fleurs !", "All 20 gardens are in bloom!" },
            ["levels_title"] = new[] { "Choisis un jardin", "Choose a garden" },
            ["levels_progress"] = new[] { "{0} / {1} jardins fleuris", "{0} / {1} gardens in bloom" },
            ["back"] = new[] { "Retour", "Back" },
            ["menu"] = new[] { "Menu", "Menu" },
            ["options"] = new[] { "Options", "Options" },
            ["level"] = new[] { "Niveau {0}", "Level {0}" },
            ["counter"] = new[] { "Fleurs écloses : {0} / {1}", "Flowers in bloom: {0} / {1}" },
            ["undo"] = new[] { "Annuler", "Undo" },
            ["restart"] = new[] { "Recommencer", "Restart" },
            ["win_title"] = new[] { "Jardin en fleurs !", "Garden in bloom!" },
            ["win_body"] = new[] { "Terminé en {0} coups.", "Completed in {0} moves." },
            ["win_body_best"] = new[] { "Terminé en {0} coups. Record : {1}.", "Completed in {0} moves. Best: {1}." },
            ["next"] = new[] { "Niveau suivant", "Next level" },
            ["replay"] = new[] { "Rejouer", "Play again" },
            ["back_home"] = new[] { "Retour à l'accueil", "Back to home" },
            ["dead_title"] = new[] { "Plus de place", "No room left" },
            ["dead_body"] = new[] { "Les trois places sont occupées et aucune fleur ne peut être arrosée.", "All three spots are taken and no flower can be watered." },
            ["dead_undo"] = new[] { "Annuler le dernier coup", "Undo last move" },
            ["retry"] = new[] { "Réessayer", "Try again" },
            ["sound_on"] = new[] { "Son : activé", "Sound: on" },
            ["sound_off"] = new[] { "Son : coupé", "Sound: off" },
            ["speed_normal"] = new[] { "Vitesse : normale", "Speed: normal" },
            ["speed_fast"] = new[] { "Vitesse : rapide (×2)", "Speed: fast (×2)" },
            ["motion_off"] = new[] { "Animations réduites : non", "Reduced motion: off" },
            ["motion_on"] = new[] { "Animations réduites : oui", "Reduced motion: on" },
            ["language"] = new[] { "Langue : Français", "Language: English" },
            ["close"] = new[] { "Fermer", "Close" },
            ["playtest_button"] = new[] { "Données de test", "Playtest data" },
            ["playtest_privacy"] = new[] { "Enregistré seulement sur ce téléphone, sans nom ni identifiant. Rien n'est envoyé.", "Stored on this phone only, with no name or ID. Nothing is sent." },
            ["playtest_header"] = new[] { "Niveau : parties / gagnées / abandons / blocages / annulations / temps médian", "Level: plays / wins / quits / deadlocks / undos / median time" },
            ["playtest_empty"] = new[] { "Aucune donnée pour l'instant.", "No data yet." },
            ["playtest_copy"] = new[] { "Copier les données", "Copy data" },
            ["playtest_copied"] = new[] { "Copié ! Colle-le dans un message.", "Copied! Paste it into a message." },
            ["playtest_clear"] = new[] { "Effacer", "Clear" },
            ["playtest_perf"] = new[] { "Fluidité : {0} i/s (p95 {1} ms) · mémoire {2} Mo · graphismes {3}", "Smoothness: {0} fps (p95 {1} ms) · memory {2} MB · graphics {3}" },
            ["graphics_auto"] = new[] { "Graphismes : auto", "Graphics: auto" },
            ["graphics_auto_eco"] = new[] { "Graphismes : auto (économie)", "Graphics: auto (economy)" },
            ["graphics_high"] = new[] { "Graphismes : élevés", "Graphics: high" },
            ["graphics_economy"] = new[] { "Graphismes : économie", "Graphics: economy" },
            ["quality_high"] = new[] { "élevés", "high" },
            ["quality_economy"] = new[] { "économie", "economy" },
            ["tut_tap"] = new[] { "Touche un arrosoir pour l'envoyer sur une place.", "Tap a watering can to send it to a spot." },
            ["tut_auto"] = new[] { "Il arrose tout seul les fleurs de sa couleur.", "It waters every flower of its color by itself." },
            ["tut_places"] = new[] { "Il n'y a que 3 places. Un arrosoir sans fleur à arroser attend et garde sa place.", "There are only 3 spots. A can with nothing to water waits and keeps its spot." },
            ["tut_undo"] = new[] { "Une erreur ? « Annuler » est gratuit et illimité.", "Made a mistake? \"Undo\" is free and unlimited." },
            ["tut_hidden"] = new[] { "Les billes grises sont des fleurs cachées sous celle du dessus.", "Grey beads are flowers hidden under the top one." },
        };

        public static Language Current { get; private set; } = DefaultForDevice();

        public static event Action Changed;

        public static IEnumerable<string> Keys => Table.Keys;

        public static Language DefaultForDevice() =>
            Application.systemLanguage == SystemLanguage.French ? Language.French : Language.English;

        public static void SetLanguage(Language language)
        {
            if (language == Current) return;
            Current = language;
            Changed?.Invoke();
        }

        public static string T(string key, params object[] args)
        {
            if (!Table.TryGetValue(key, out string[] texts)) return "#" + key;
            string text = texts[(int)Current];
            return args == null || args.Length == 0 ? text : string.Format(text, args);
        }

        public static string In(Language language, string key) => Table.TryGetValue(key, out string[] t) ? t[(int)language] : null;

        public static string Code(Language language) => language == Language.French ? "fr" : "en";

        public static Language FromCode(string code, Language fallback) =>
            code == "fr" ? Language.French : code == "en" ? Language.English : fallback;
    }
}
