# Plan — Bloom Garden (copie Claude Code)

Statuts : **Validé** (preuve jointe) · **Partiel** (fait ici, vérification Unity en attente) · **Bloqué** (besoin de l'utilisateur) · **À faire**.
Une seule phase à la fois. Le suivi des 71 tâches est dans `Docs/PLAN_DE_TRAVAIL.csv`.

## P0 — Cadrage / installation

| ID | Tâche | Priorité | Dépend de | Statut | Résultat attendu | Preuve |
|---|---|---|---|---|---|---|
| P0-01 | Vérifier Unity Hub / Editor 6+ | Haute | — | **Bloqué** | Version exacte notée | Besoin : version affichée dans Unity Hub (intervention H1) |
| P0-02 | Modules Android | Haute | P0-01 | **Bloqué** | SDK/NDK/OpenJDK détectés | Besoin : capture de Unity Hub > Installs > Modules (H1) |
| P0-03 | CLI / plugin Unity MCP | Haute | P0-01 | **Bloqué** | Lecture réelle d'une scène | Impossible depuis le cloud ; nécessite une session Claude Code locale (H2) |
| P0-04 | Git / .gitignore Unity | Haute | — | **Validé** | Commit propre sans caches ni secrets | `.gitignore`, premier commit de `BloomGarden_Claude/` |
| P0-05 | Structure des dossiers | Haute | — | **Validé** | Scenes/Scripts/Tests/Docs présents | arborescence `Assets/_BloomGarden/` (GDD ch. 10) |
| P0-06 | Portrait / URP | Haute | P0-01 | **Partiel** | Player Settings et scène enregistrés | Script `Assets/_BloomGarden/Editor/ProjectSetup.cs` écrit, **pas encore exécuté dans Unity** |
| P0-07 | Docs/PLAN et ENVIRONMENT | Moyenne | — | **Validé** | Versions, risques, prochaine phase | ce fichier, `Docs/ENVIRONMENT.md`, `Docs/QA/P0.md` |

Critère de sortie P0 (GDD) : « ouvre, compile, documentation exacte » → **non atteint** tant que le projet n'a pas été ouvert dans Unity.

## P1 — Cœur logique (C# pur)

| ID | Tâche | Statut | Preuve |
|---|---|---|---|
| P1-01 | `LevelDefinition` versionné | **Partiel** | `Core/Model/LevelDefinition.cs`, `LevelLoader`, `LevelValidator` ; tests de rejet verts (dotnet) |
| P1-02 | `GameState` immuable / hash déterministe | **Partiel** | `ComputeHash` FNV-1a ; test valeur de référence |
| P1-03 | Files et sélection de tête | **Partiel** | I01 |
| P1-04 | Trois places | **Partiel** | I08, `Can_TakesLeftmostFreePlace…` |
| P1-05 | Cibles visibles par pile | **Partiel** | I04, `Target_IsSmallestRowThenColumn` |
| P1-06 | `TurnResolver` stable | **Partiel** | I05 + comparaison Python sur 1242 coups |
| P1-07 | Arrosoir en attente / départ | **Partiel** | I06, I07, I09 |
| P1-08 | Victoire et blocage | **Partiel** | I11, I12 |
| P1-09 | `UndoHistory` | **Partiel** | I10 (hash identique à chaque coup, sur les 20 niveaux) |
| P1-10 | Tests EditMode | **Partiel** | 49/49 verts avec `dotnet test` (`Docs/QA/P1_dotnet_test.log`) ; à relancer dans Unity Test Runner |

« Partiel » = code écrit et testé hors Unity ; passe à « Validé » après l'exécution des mêmes tests dans Unity Test Runner (H3).

## P2 — Solveur / contenu (suivante)

| ID | Tâche | Critère d'acceptation |
|---|---|---|
| P2-01 | 20 JSON dans le projet | Déjà copiés dans `Assets/_BloomGarden/Levels/` et validés par les tests ; reste le chargement Unity (TextAsset) |
| P2-02 | Validateur de données | Déjà fait (`LevelValidator`) ; ajouter un rapport par fichier |
| P2-03 | Solveur BFS C# | Utiliser `GameState.ComputePositionKey` ; mêmes `solutionLanes` que le Python sur les 20 niveaux |
| P2-04 | Replays | Déjà vert (I15) ; ajouter le replay des solutions du solveur C# |
| P2-05 | Limites | Statut `Unknown` quand le plafond d'états est atteint, jamais `Solvable` |
| P2-06 | Niveaux tutoriels 1–10 | Dépend de la décision D-107 (niveaux sans piège) |
| P2-07 | Rapport difficulté | CSV : états explorés, profondeur, nombre d'états de blocage |

## Interventions humaines nécessaires

| # | Quoi | Pourquoi | Bloque |
|---|---|---|---|
| H1 | M'envoyer la **version exacte d'Unity** (Unity Hub > Installs) et une capture des modules Android installés | Je ne peux pas voir ton PC | P0-01, P0-02 |
| H2 | Ouvrir une session Claude Code **sur ton PC** (Claude Desktop ou `claude` dans un terminal), installer le plugin Unity, récupérer cette branche | Le MCP Unity est local ; depuis le cloud je ne peux pas piloter l'éditeur | P0-03, P0-06, P3 et suivantes |
| H3 | Ou bien, à la main : Unity Hub > **Add > Add project from disk** > `BloomGarden_Claude`, puis menu **Bloom Garden > P0 - Configurer le projet**, puis **Window > General > Test Runner > EditMode > Run All**, et m'envoyer la Console + le résultat | Première ouverture, génération de `ProjectSettings/` et des `.meta` | Validation P0/P1 |
| H4 | Décider pour D-107 : garder les 20 niveaux tels quels ou les régénérer avec des pièges réels | Changement de contenu, pas de règle | P2-06 |
| H5 | (Si la compilation Unity échoue à cause de Sécurité Windows) me copier l'erreur de la Console | Un fichier de Unity est bloqué par Windows | Compilation |

## Risques (par sévérité)

1. **Pas d'accès à l'éditeur depuis le cloud** : tout ce qui est scène, rendu, build Android reste invérifié tant que H2 ou H3 n'est pas fait.
2. **Niveaux de référence sans défi** (D-107) : impossible de perdre sur 19 niveaux sur 20 ; le jeu risque d'être jugé trop facile.
3. **Blocage Windows d'une DLL Unity** : peut empêcher la compilation de scripts UI Toolkit si la console signale une erreur.
