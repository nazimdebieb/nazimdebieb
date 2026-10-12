# Changelog

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
