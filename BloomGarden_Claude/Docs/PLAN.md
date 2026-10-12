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

## P4 — Visuels et animations

| ID | Tâche | Statut | Preuve |
|---|---|---|---|
| P4-01 | Palette / matériaux | **Partiel** | Palette GDD + diorama (île, pierres, arbres, estrade bois), ombres douces ; captures `P4_*.png` ; « homogène sur 2 téléphones » non vérifiable sans appareil (H7) |
| P4-02 | Gouttes et jet | **Validé** | Jet de 5 gouttes en arc depuis le bec ; test `Logic_IsResolved_BeforeAnimationsPlay` (état final avant les effets) |
| P4-03 | Éclosion 4 couleurs | **Validé** | Bouton tulipe → coupe de 6 pétales qui s'ouvre + étamines + étincelles ; capture `P4_level20_win.png` (4 couleurs) |
| P4-04 | Déplacement arrosoir | **Validé** | Arc + inclinaison + tassement à l'arrivée ; la file avance en même temps ; capture `P4_level1_watering.png` |
| P4-05 | Sons | **Validé** | 8 sons synthétisés ; son muet en 0.4.0 (AudioListener manquant) corrigé en 0.4.1 ; **confirmé à l'écoute par l'utilisateur sur téléphone** ; test `Sounds_AreAudible_AndFollowTheSetting` |
| P4-06 | Animation de victoire | **Validé** | Vague sur le jardin + pétales qui tombent, panneau en bas qui laisse le jardin visible ; `P4_level20_win.png` |
| P4-07 | Mode 2× / animations réduites | **Validé** | Test `SameFinalState_InEveryAnimationMode` : même hash dans les 3 modes ; BG-020 joué en 13,9 s / 7,2 s / 4,5 s |
| P4-08 | Pooling / perf | **Validé (éditeur)** | `VisualPool` : rejouer BG-013 crée 0 objet ; p50 4,7 ms, p95 6,4 ms, max 14,3 ms dans l'éditeur (`P4_perf.txt`) ; mesure téléphone en P7-03 |

Tests : PlayMode **9/9**, EditMode **62/62**. APK 0.4.1 testé par l'utilisateur sur son téléphone : jeu fonctionnel, son OK.

## P5 — UX, progression, sauvegarde

| ID | Tâche | Statut | Preuve |
|---|---|---|---|
| P5-01 | Menu principal et niveau suivant | **Validé** | Accueil avec diorama vivant, « Jouer » / « Continuer : niveau N » ; « Niveau suivant » après victoire ; `P5_home_fr.png` |
| P5-02 | Sélection des niveaux | **Validé** | Grille 20 niveaux (réussis en vert, suivant en jaune, verrouillés grisés) ; test `Levels_AreLocked_UntilThePreviousOneIsWon` |
| P5-03 | Tutoriel intégré | **Partiel** | Bulles niveaux 1–4 + chevron sur l'arrosoir à toucher ; tests `FreshStart_ShowsHome_ThenLevel1WithTutorial`, `Tutorial_OneIdeaPerFirstLevel_AndSilentOnceCompleted` ; « test sans explication » = test humain (P6) |
| P5-04 | Paramètres | **Validé** | Son, vitesse ×2, animations réduites, langue ; sauvegardés dans le fichier de sauvegarde |
| P5-05 | Localisation FR/EN | **Validé** | `Loc` (38 clés FR+EN) ; test `BothLanguages_AllScreens_NoTextOverflow` (accueil, niveaux, jeu + bulle, options, blocage, victoire) |
| P5-06 | Sauvegarde versionnée | **Validé** | `SaveService` v1 ; 6 tests (aller-retour, écriture interrompue, fichier corrompu → secours, illisible → départ propre, migration PlayerPrefs, version future) ; reprise exacte après rechargement avec annulation |
| P5-07 | Pause / arrière-plan | **Validé** | `OnApplicationPause` : animation figée sur l'état réel + sauvegarde ; test `Pause_DuringAnimation_SnapsAndSaves` |
| P5-08 | QA sur appareil Android | **À faire** | APK 0.5.0 envoyé à l'utilisateur ; liste de vérification dans `Docs/QA/P5.md` |

Tests : PlayMode **14/14**, EditMode **70/70**.

## P6 — Alpha et retours joueurs (suivante)

| ID | Tâche | Critère d'acceptation |
|---|---|---|
| P6-01 | Instrumentation locale anonymisée | Événements visibles localement, aucun SDK publicitaire ni envoi réseau |
| P6-02 | Protocole d'observation | Formulaire reproductible (Kit/benchmark/PROTOCOLE_TEST_HUMAIN.md) |
| P6-03 à P6-05 | Tests avec 5 puis 20–30 joueurs | Frictions et abandons notés ; **nécessite l'utilisateur et des testeurs** |

## Interventions humaines nécessaires

| # | Quoi | Pourquoi | Bloque |
|---|---|---|---|
| H7 | Brancher un téléphone Android en USB avec le **débogage USB** activé (Paramètres > À propos > appuyer 7 fois sur « Numéro de build », puis Options pour les développeurs > Débogage USB) et accepter l'empreinte de l'ordinateur | adb ne voit aucun appareil ; l'installation et le test réel en dépendent | P3-10, P5-08 |

H1, H2, H3, H5 et H6 sont **résolus** (session locale sur le PC, 2026-10-12 ; aucune erreur de compilation liée à `UxmlCloneTreeGenerator.dll`).

## Risques (par sévérité)

1. **Niveaux de référence sans défi** (D-107, accepté pour le MVP1) : impossible de perdre sur 19 niveaux sur 20 ; pièges prévus au MVP2.
2. **Outils Unity en bêta / expérimental** (plugin 0.1.8-beta, CLI 1.0.0-beta.13, Pipeline 0.8.0-exp.1) : la commande `build` asynchrone n'a rien produit, contournée par `BuildPipeline` direct ; repli possible sur le batchmode.
3. **Aucun test sur appareil réel** pour l'instant : performance, lisibilité des tailles et taps tactiles restent à confirmer (H7).
