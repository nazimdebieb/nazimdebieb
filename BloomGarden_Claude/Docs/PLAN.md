# Plan — Bloom Garden (copie Claude Code)

Statuts : **Validé** (preuve jointe) · **Partiel** (fait ici, vérification Unity en attente) · **Bloqué** (besoin de l'utilisateur) · **À faire**.
Une seule phase à la fois. Le suivi des 71 tâches est dans `Docs/PLAN_DE_TRAVAIL.csv`.

## P0 — Cadrage / installation

| ID | Tâche | Priorité | Dépend de | Statut | Résultat attendu | Preuve |
|---|---|---|---|---|---|---|
| P0-01 | Vérifier Unity Hub / Editor 6+ | Haute | — | **Validé** | Version exacte notée | Unity **6000.6.5f1** (3ff58d469c8a), `ProjectSettings/ProjectVersion.txt` |
| P0-02 | Modules Android | Haute | P0-01 | **Validé** | SDK/NDK/OpenJDK détectés | `PlaybackEngines/AndroidPlayer/{SDK,NDK,OpenJDK}` présents |
| P0-03 | CLI / plugin Unity MCP | Haute | P0-01 | **Validé** | Lecture réelle d'une scène | Unity CLI 1.0.0-beta.13 + package Pipeline : `get_scene_hierarchy` sur l'éditeur ouvert a renvoyé Main Camera / Directional Light de `Game.unity` |
| P0-04 | Git / .gitignore Unity | Haute | — | **Validé** | Commit propre sans caches ni secrets | `.gitignore`, `Library/` `Logs/` exclus |
| P0-05 | Structure des dossiers | Haute | — | **Validé** | Scenes/Scripts/Tests/Docs présents | `Assets/_BloomGarden/` (GDD ch. 10) |
| P0-06 | Portrait / URP | Haute | P0-01 | **Validé** | Player Settings et scène enregistrés | `ProjectSetup.ConfigureAll` en batchmode : portrait (`defaultScreenOrientation: 0`), `Settings/URP_Mobile.asset` assigné (Graphics + Quality), `Scenes/Game.unity` ; voir `Docs/QA/P0.md` |
| P0-07 | Docs/PLAN et ENVIRONMENT | Moyenne | — | **Validé** | Versions, risques, prochaine phase | ce fichier, `Docs/ENVIRONMENT.md`, `Docs/QA/P0.md` |

Critère de sortie P0 (GDD) : « ouvre, compile, documentation exacte » → **atteint** (ouverture + compilation sans erreur sous Unity 6000.6.5f1, pilotage en direct démontré).

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

## P3 — Premier vertical slice jouable

| ID | Tâche | Statut | Preuve |
|---|---|---|---|
| P3-01 | Scène Game orthographique | **Validé** | Caméra orthographique inclinée 55°, portrait 1080×1920 ; `Docs/QA/captures/P3_level1_start.png` |
| P3-02 | Vue grille et piles | **Validé** | Seul le bouton du dessus est visible, une pastille grise par couche cachée ; taille des cases adaptée à la grille (3×3 à 4×4) |
| P3-03 | Fleur bouton / éclose | **Validé** | Bouton = ovale fermé, éclose = 6 pétales (forme différente) ; transition liée à l'événement `FlowerBloomed` |
| P3-04 | Arrosoir 4 couleurs | **Validé** | Couleur + symbole + une goutte blanche par unité d'eau restante |
| P3-05 | Files et têtes | **Validé** | Anneau vert sous les têtes sélectionnables ; seules les têtes ont un collider ; arrosoirs suivants plus pâles |
| P3-06 | Places et attente | **Validé** | Arrosoir en attente reste sur sa place avec sa charge |
| P3-07 | Tap → TurnResolver | **Validé** | Test PlayMode `TapDuringAnimation_IsExactlyOneMoreMove` (tap écran → rayon → file → 1 coup) |
| P3-08 | Undo / Restart | **Validé** | Test `UndoAndRestart_RestoreExactStates` (hash identique + vue synchronisée) |
| P3-09 | Victoire / blocage | **Validé** | Tests `Level1_WinByTaps_ThenLevel2`, `Bg018_Deadlock_ShowsPanel_AndUndoRecovers`, `AllTwentyLevels_PlayToWin_ViewInSync` |
| P3-10 | APK debug | **Partiel** | APK construit : `Builds/BloomGarden_Claude_0.3.0.apk` (27 Mo, IL2CPP ARM64, 0 erreur, `Docs/QA/P3_android_build.txt`) ; **installation sur téléphone non faite** : aucun appareil détecté par adb (H7) |

Critère de sortie P3 (GDD) : « niveau 1 → sélections → floraisons → victoire → niveau 2, aucun crash ni dépendance externe » → **atteint dans l'éditeur** (PlayMode 5/5, EditMode 62/62). Reste la vérification sur téléphone.

## P4 — Visuels et animations (suivante)

| ID | Tâche | Critère d'acceptation |
|---|---|---|
| P4-01 | Palette / matériaux cohérents | Rendu homogène, contraste vérifié, captures avant/après |
| P4-02 | Gouttes et jet | La logique n'attend jamais les particules (tests PlayMode inchangés) |
| P4-03 / P4-04 | Éclosion et trajectoire de l'arrosoir | États différenciés, trajectoire lisible |
| P4-05 | Sons | Volume réglable |
| P4-06 à P4-08 | Victoire, mode 2× / animations réduites, pooling | Même replay quel que soit le mode, pas de pic de CPU |

## Interventions humaines nécessaires

| # | Quoi | Pourquoi | Bloque |
|---|---|---|---|
| H7 | Brancher un téléphone Android en USB avec le **débogage USB** activé (Paramètres > À propos > appuyer 7 fois sur « Numéro de build », puis Options pour les développeurs > Débogage USB) et accepter l'empreinte de l'ordinateur | adb ne voit aucun appareil ; l'installation et le test réel en dépendent | P3-10, P5-08 |

H1, H2, H3, H5 et H6 sont **résolus** (session locale sur le PC, 2026-10-12 ; aucune erreur de compilation liée à `UxmlCloneTreeGenerator.dll`).

## Risques (par sévérité)

1. **Niveaux de référence sans défi** (D-107, accepté pour le MVP1) : impossible de perdre sur 19 niveaux sur 20 ; pièges prévus au MVP2.
2. **Outils Unity en bêta / expérimental** (plugin 0.1.8-beta, CLI 1.0.0-beta.13, Pipeline 0.8.0-exp.1) : la commande `build` asynchrone n'a rien produit, contournée par `BuildPipeline` direct ; repli possible sur le batchmode.
3. **Aucun test sur appareil réel** pour l'instant : performance, lisibilité des tailles et taps tactiles restent à confirmer (H7).
