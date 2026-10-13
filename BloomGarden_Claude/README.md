# BloomGarden_Claude

Version **Claude Code** du jeu mobile Bloom Garden (Unity 6, portrait, Android). Elle est réalisée à part de la version Codex pour comparer les deux résultats.

- `Kit/` : kit de départ reçu, **non modifié** (GDD, niveaux, solveur Python, benchmark).
- `Assets/_BloomGarden/` : projet Unity (scripts, niveaux, tests).
- `Docs/` : plan, environnement, décisions, rapports QA.
- `Tools/DotnetTests/` : lance les tests de la logique sans Unity (`dotnet test`).
- `Tools/python/` : scripts d'oracle et d'analyse basés sur le solveur de référence.

## Ouvrir dans Unity (première fois)

1. Unity Hub > **Add** > **Add project from disk** > choisir ce dossier `BloomGarden_Claude`. Utiliser la même version Unity 6 que pour la copie Codex.
2. Menu **Bloom Garden > P0 - Configurer le projet** : orientation portrait, scène `Game`, installation d'URP.
3. **Window > General > Test Runner > EditMode > Run All**.

État d'avancement : voir `Docs/PLAN.md`.
