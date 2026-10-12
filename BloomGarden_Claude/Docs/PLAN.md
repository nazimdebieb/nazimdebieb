# Plan — Bloom Garden (copie Claude Code)

Statuts : **Validé** (preuve jointe) · **Partiel** (fait ici, vérification Unity en attente) · **Bloqué** (besoin de l'utilisateur) · **À faire**.
Une seule phase à la fois. Le suivi des 71 tâches est dans `Docs/PLAN_DE_TRAVAIL.csv`.

## P0 — Cadrage / installation

| ID | Tâche | Priorité | Dépend de | Statut | Résultat attendu | Preuve |
|---|---|---|---|---|---|---|
| P0-01 | Vérifier Unity Hub / Editor 6+ | Haute | — | **Validé** | Version exacte notée | Unity **6000.6.5f1** (3ff58d469c8a), `ProjectSettings/ProjectVersion.txt` |
| P0-02 | Modules Android | Haute | P0-01 | **Validé** | SDK/NDK/OpenJDK détectés | `PlaybackEngines/AndroidPlayer/{SDK,NDK,OpenJDK}` présents |
| P0-03 | CLI / plugin Unity MCP | Haute | P0-01 | **Partiel** | Lecture réelle d'une scène | Plugin `unity@unity-agent-plugin` 0.1.8-beta installé ; Unity CLI (batchmode) opérationnel ; lecture de scène via MCP à démontrer dans la session suivante (Unity ouvert sur ce projet) |
| P0-04 | Git / .gitignore Unity | Haute | — | **Validé** | Commit propre sans caches ni secrets | `.gitignore`, `Library/` `Logs/` exclus |
| P0-05 | Structure des dossiers | Haute | — | **Validé** | Scenes/Scripts/Tests/Docs présents | `Assets/_BloomGarden/` (GDD ch. 10) |
| P0-06 | Portrait / URP | Haute | P0-01 | **Validé** | Player Settings et scène enregistrés | `ProjectSetup.ConfigureAll` en batchmode : portrait (`defaultScreenOrientation: 0`), `Settings/URP_Mobile.asset` assigné (Graphics + Quality), `Scenes/Game.unity` ; voir `Docs/QA/P0.md` |
| P0-07 | Docs/PLAN et ENVIRONMENT | Moyenne | — | **Validé** | Versions, risques, prochaine phase | ce fichier, `Docs/ENVIRONMENT.md`, `Docs/QA/P0.md` |

Critère de sortie P0 (GDD) : « ouvre, compile, documentation exacte » → **atteint** (ouverture + compilation sans erreur sous Unity 6000.6.5f1). Seule la démonstration MCP (P0-03) reste à faire.

## P1 — Cœur logique (C# pur)

| ID | Tâche | Statut | Preuve |
|---|---|---|---|
| P1-01 | `LevelDefinition` versionné | **Validé** | `Core/Model/LevelDefinition.cs`, `LevelLoader`, `LevelValidator` ; tests de rejet |
| P1-02 | `GameState` immuable / hash déterministe | **Validé** | `ComputeHash` FNV-1a ; test valeur de référence |
| P1-03 | Files et sélection de tête | **Validé** | I01 |
| P1-04 | Trois places | **Validé** | I08, `Can_TakesLeftmostFreePlace…` |
| P1-05 | Cibles visibles par pile | **Validé** | I04, `Target_IsSmallestRowThenColumn` |
| P1-06 | `TurnResolver` stable | **Validé** | I05 + comparaison Python sur 1242 coups |
| P1-07 | Arrosoir en attente / départ | **Validé** | I06, I07, I09 |
| P1-08 | Victoire et blocage | **Validé** | I11, I12 |
| P1-09 | `UndoHistory` | **Validé** | I10 (hash identique à chaque coup, sur les 20 niveaux) |
| P1-10 | Tests EditMode | **Validé** | **Unity Test Runner EditMode : 49/49** (`Docs/QA/P1_unity_editmode_results.xml`) ; dotnet 49/49 (`Docs/QA/P1_dotnet_test.log`) |

## P2 — Solveur / contenu

| ID | Tâche | Statut | Preuve |
|---|---|---|---|
| P2-01 | 20 JSON dans le projet | **Validé** | `ScriptableObjects/LevelCatalog.asset` (20 TextAsset, inclus dans le build) ; test Unity `Catalog_LoadsTwentyValidLevels_InOrder` |
| P2-02 | Validateur de données | **Validé** | `LevelValidator` + colonne `valid` par fichier dans le rapport ; fichiers invalides listés dans `Docs/QA/P2_solver_log.txt` |
| P2-03 | Solveur BFS C# | **Validé** | `Scripts/Solvers/LevelSolver.cs` ; mêmes solutions et mêmes `statesExplored` que le Python sur les 20 niveaux |
| P2-04 | Replays | **Validé** | 20 `referenceSolution` + 20 solutions du solveur rejouées jusqu'à la victoire via `GameSession` |
| P2-05 | Limites | **Validé** | `Unsolvable` sur un niveau à blocage forcé ; `Unknown` (jamais `Solvable`) quand la limite d'états est atteinte |
| P2-06 | Niveaux tutoriels 1–10 | **Validé** | Niveaux non modifiés (D-107) ; progression documentée dans `Docs/LEVELS.md` et vérifiée par test (tranches du GDD ch. 08) |
| P2-07 | Rapport difficulté | **Validé** | `Docs/QA/P2_difficulty_report.csv` (24 colonnes : coups, états explorés/atteignables, blocages, choix, pièges, replays, avertissements) |

Critère de sortie P2 (GDD) : « 20 fichiers JSON versionnés, solveur/replay confirme leur solvabilité, niveaux classés par notion, analyses de difficulté » → **atteint**. Tests Unity EditMode : **62/62** (`Docs/QA/P2_unity_editmode_results.xml`).

## P3 — Premier vertical slice jouable (suivante)

| ID | Tâche | Critère d'acceptation |
|---|---|---|
| P3-01 | Scène Game orthographique | Jeu portrait visible sous Unity, capture 1080×1920 |
| P3-02 | Vue grille et piles | Dessus de chaque pile correctement affiché |
| P3-03 / P3-04 | Prefabs fleur et arrosoir (primitives douces) | Couleur, état bouton/éclos et capacité lisibles |
| P3-05 / P3-06 | Files, têtes, places | Seules les têtes cliquables ; occupation et charge lisibles |
| P3-07 à P3-09 | Tap → `TurnResolver`, Undo/Restart, victoire/blocage | Niveau 1 → victoire → niveau 2 sans erreur console |
| P3-10 | APK debug | Installé sur téléphone ou limitation documentée |

## Interventions humaines nécessaires

| # | Quoi | Pourquoi | Bloque |
|---|---|---|---|
| H6 | Pour la démonstration MCP : fermer le projet Codex dans Unity (ou l'autre instance) et ouvrir `BloomGarden_Claude` dans Unity Hub, puis démarrer une nouvelle session Claude (le plugin se charge au démarrage) | Le MCP pilote l'éditeur ouvert ; on ne touche pas au projet Codex | P0-03, P3 (vérification visuelle) |

H1, H2, H3 et H5 sont **résolus** (session locale sur le PC, 2026-10-12 ; aucune erreur de compilation liée à `UxmlCloneTreeGenerator.dll`).

## Risques (par sévérité)

1. **Niveaux de référence sans défi** (D-107, accepté pour le MVP1) : impossible de perdre sur 19 niveaux sur 20 ; pièges prévus au MVP2.
2. **Plugin Unity en bêta (0.1.8-beta)** : si le MCP est instable, repli sur Unity CLI en batchmode (déjà fonctionnel pour la configuration et les tests).
3. **Deux projets Unity en parallèle** (Codex / Claude) : une seule instance d'éditeur par projet ; ne jamais ouvrir le projet Codex depuis cette session.
