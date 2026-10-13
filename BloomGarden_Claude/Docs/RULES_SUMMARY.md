# Résumé des règles verrouillées — rules 0.1.0

Lu avant de coder : `Kit/docs/00_BLOOM_GARDEN_GDD_FR.md` (ch. 05, 16, 24), `Kit/README_DEMARRAGE.md`, `Kit/docs/DECISIONS.md`, `Kit/docs/PROMPTS_PAR_PHASE.md`, `Kit/levels/README.md`, `Kit/tools/solver_reference.py`.

| # | Règle (verrouillée, D-001 à D-004) | Où dans le code |
|---|---|---|
| R1 | Plateau 1×1 à 7×7 (MVP 3×3 à 5×5), cases en ordre de lecture ligne par ligne | `LevelDefinition.Cells`, `RowOf/ColumnOf` |
| R2 | Pile de 0 à 3 boutons ; seul le premier non éclos est visible et ciblable | `GameState.TryGetVisibleBud`, `TurnResolver.FindTarget` |
| R3 | 4 couleurs au maximum : coral, blue, yellow, purple | `ColorId` |
| R4 | 3 files ; seul l'arrosoir de tête est sélectionnable | `TurnResolver.CheckSelection` |
| R5 | 3 places ; l'arrosoir choisi prend la place libre la plus à gauche | `TurnResolver.SelectLane` |
| R6 | 1 bouton arrosé = 1 unité ; pas de recharge ; capacité entière ≥ 1 | `LevelValidator`, `SlotState.Remaining` |
| R7 | Arrosage automatique : places évaluées de la plus ancienne à la plus récente ; cible = plus petit (ligne, colonne) de sa couleur ; **une fleur par étape**, puis réévaluation complète | `TurnResolver.Stabilize` |
| R8 | Arrosoir vide → quitte sa place ; arrosoir sans cible → attend et garde sa place | `Stabilize` |
| R9 | Victoire = toutes les piles vides (les arrosoirs restants n'importent pas) | `EvaluateStatus` |
| R10 | Blocage = état stable (après stabilisation), fleurs restantes, aucune sélection possible | `EvaluateStatus` |
| R11 | Annulation gratuite et illimitée jusqu'au début du niveau ; restaure tout l'état | `UndoHistory`, `GameSession.Undo` |
| R12 | Aucun chronomètre ; la logique ne dépend jamais du temps ou des animations | Aucun temps dans `Core` |
| R13 | Validation des niveaux : eau par couleur = boutons par couleur (exactement, comme le solveur Python) | `LevelValidator` |

Hors périmètre du MVP (D-007) : boutique, publicité, achats intégrés, réseau, notifications.
