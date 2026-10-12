# Changelog

## 2026-10-12 — 0.4.1 : correctif du son
- Son muet sur téléphone (signalé sur 0.4.0) : `AudioListener` manquant sur la caméra. Ajouté dans la scène + filet de sécurité au démarrage + test PlayMode dédié (9/9).

## 2026-10-12 — P4 : visuels, animations, sons
- Diorama : île de gazon avec pierres, arbres ronds et buissons, estrade en bois pour les places et les files, ombres douces (1 cascade, MSAA 2×), palette retravaillée.
- Fleurs : bouton tulipe à sépales, fleur en coupe qui s'ouvre (6 pétales, cœur, étamines), étincelles ; arrosoirs avec anse, bec et pomme.
- Animations : trajectoire en arc vers la place, jet de gouttes depuis le bec, inclinaison d'arrosage, départ de l'arrosoir vide, têtes sélectionnables qui respirent, vague et pluie de pétales à la victoire.
- Sons synthétisés par code (aucune licence) ; bouton Options : son, vitesse ×2, animations réduites (préférences sauvegardées).
- Pooling des visuels : rejouer un niveau ne crée aucun objet.
- Tests PlayMode 8/8 (dont : même résultat dans les 3 modes d'animation), EditMode 62/62.

## 2026-10-12 — P3 : premier niveau jouable (vertical slice)
- Scène `Game` portrait : jardin 2,5D en primitives (palette du GDD), 3 places, 3 files, HUD (titre, compteur, Annuler, Recommencer), écrans de victoire et de blocage.
- Tap écran → rayon caméra → tête de file → un seul coup du `TurnResolver` ; animations (déplacement, gouttes, éclosion) sans influence sur la logique ; un tap pendant une animation recale l'affichage puis joue le coup.
- Accessibilité : forme différente bouton / fleur éclose, symbole par couleur, gouttes = eau restante.
- Tests PlayMode 5/5 (dont les 20 niveaux gagnés avec vérification vue = état), EditMode 62/62.
- APK de test 0.3.0 (IL2CPP ARM64, 27 Mo) ; installation sur téléphone en attente d'un appareil.
- Pilotage en direct de l'éditeur (Unity CLI + package Pipeline) ; P0-03 validé.

## 2026-10-12 — P2 validé (solveur, catalogue, rapport)
- `BloomGarden.Solvers` (C# pur) : `LevelSolver` (BFS, statuts Solvable / Unsolvable / Unknown), `LevelAnalyzer` (graphe complet : blocages, choix, coups perdants), `DifficultyReport` (CSV) et tranches pédagogiques du GDD ch. 08.
- `BloomGarden.Levels` : `LevelCatalog` (ScriptableObject, 20 TextAsset) ; outils éditeur `Bloom Garden ▸ P2 - …` utilisables en batchmode.
- Oracle Python `Tools/python/generate_solver_fixture.py` → `Fixtures/python_solver_reference.json` : le solveur C# retrouve les mêmes solutions, états explorés et analyses sur les 20 niveaux.
- Décision D-107 tranchée : niveaux du kit conservés pour le MVP1, pièges au MVP2.
- Tests Unity EditMode : 62/62.

## 2026-10-12 — P0 validé + P1 validé dans Unity (session locale)
- Projet ouvert et compilé sous Unity 6000.6.5f1 ; `ProjectSettings/` et `.meta` générés et versionnés.
- `Packages/manifest.json` : URP 17.6.0, Test Framework 1.8.0, uGUI 2.6.0 ; IAP et Analytics retirés (hors MVP).
- `ProjectSetup` : URP créé et assigné de façon synchrone (`Settings/URP_Mobile.asset`), scène `Game` orthographique, portrait.
- Unity Test Runner EditMode : 49/49 (`Docs/QA/P1_unity_editmode_results.xml`).
- Plugin `unity@unity-agent-plugin` 0.1.8-beta installé ; démonstration MCP à faire.

## 2026-10-12 — P0 (partiel) + P1 (logique pure, testée hors Unity)
- Structure du projet Unity, `.gitignore`, kit de référence copié dans `Kit/`.
- Moteur C# pur `BloomGarden.Core` : chargement JSON, validation, `GameState`, `TurnResolver`, `UndoHistory`, `GameSession`.
- 49 tests EditMode (dont les 15 invariants du GDD et une comparaison coup par coup avec le solveur Python) : 49/49 réussis avec `dotnet test`.
- Script éditeur `ProjectSetup` (portrait, scène Game, URP) : écrit, pas encore exécuté dans Unity.
