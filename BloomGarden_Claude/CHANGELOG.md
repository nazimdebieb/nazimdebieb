# Changelog

## 2026-10-12 — P0 (partiel) + P1 (logique pure, testée hors Unity)
- Structure du projet Unity, `.gitignore`, kit de référence copié dans `Kit/`.
- Moteur C# pur `BloomGarden.Core` : chargement JSON, validation, `GameState`, `TurnResolver`, `UndoHistory`, `GameSession`.
- 49 tests EditMode (dont les 15 invariants du GDD et une comparaison coup par coup avec le solveur Python) : 49/49 réussis avec `dotnet test`.
- Script éditeur `ProjectSetup` (portrait, scène Game, URP) : écrit, pas encore exécuté dans Unity.
