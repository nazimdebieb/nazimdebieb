# Niveaux du MVP1 — progression pédagogique (P2-06)

Généré à partir de `Docs/QA/P2_difficulty_report.csv` (menu **Bloom Garden ▸ P2 - Exporter le rapport de difficulte**). Les 20 niveaux sont ceux du kit, **non modifiés** (décision D-107 : même contenu que Codex pour le MVP1, pièges reportés au MVP2).

Tranches et contraintes : GDD chapitre 08. Chaque niveau est vérifié automatiquement : couleurs et couches dans les limites de sa tranche, solution trouvée par le solveur C#, replay gagnant (test `Report_AllLevelsPublishable_AndInTheirTeachingBand`).

Colonnes : **coups** = longueur de la solution la plus courte ; **états** = positions atteignables ; **choix** = positions où au moins 2 files sont jouables ; **pièges** = coups qui rendent le niveau impossible à gagner.

## Niveaux 1-3 — Tap, couleurs, éclosion automatique

| Niveau | Grille | Couleurs | Couches max | Fleurs | Arrosoirs | Coups | États | Choix | Pièges |
|---|---|---|---|---|---|---|---|---|---|
| BG-001 | 3×3 | 2 | 1 | 6 | 2 | 2 | 4 | 1 | 0 |
| BG-002 | 3×3 | 2 | 1 | 7 | 3 | 3 | 6 | 2 | 0 |
| BG-003 | 3×3 | 2 | 1 | 8 | 4 | 4 | 9 | 4 | 0 |

## Niveaux 4-6 — Fleurs cachées dans les piles

| Niveau | Grille | Couleurs | Couches max | Fleurs | Arrosoirs | Coups | États | Choix | Pièges |
|---|---|---|---|---|---|---|---|---|---|
| BG-004 | 3×3 | 3 | 2 | 9 | 7 | 7 | 25 | 14 | 0 |
| BG-005 | 3×3 | 3 | 2 | 9 | 5 | 5 | 18 | 12 | 0 |
| BG-006 | 3×3 | 3 | 2 | 10 | 7 | 7 | 25 | 16 | 0 |

## Niveaux 7-10 — Gestion des 3 places et files

| Niveau | Grille | Couleurs | Couches max | Fleurs | Arrosoirs | Coups | États | Choix | Pièges |
|---|---|---|---|---|---|---|---|---|---|
| BG-007 | 3×3 | 3 | 2 | 10 | 7 | 7 | 32 | 20 | 0 |
| BG-008 | 3×3 | 3 | 2 | 11 | 7 | 7 | 42 | 31 | 0 |
| BG-009 | 3×3 | 3 | 2 | 11 | 6 | 6 | 28 | 17 | 0 |
| BG-010 | 4×3 | 3 | 2 | 12 | 7 | 7 | 37 | 26 | 0 |

## Niveaux 11-20 — Anticiper plusieurs coups

| Niveau | Grille | Couleurs | Couches max | Fleurs | Arrosoirs | Coups | États | Choix | Pièges |
|---|---|---|---|---|---|---|---|---|---|
| BG-011 | 4×3 | 4 | 2 | 12 | 8 | 8 | 56 | 44 | 0 |
| BG-012 | 4×3 | 4 | 2 | 13 | 7 | 7 | 57 | 35 | 0 |
| BG-013 | 4×3 | 4 | 3 | 13 | 9 | 9 | 124 | 88 | 0 |
| BG-014 | 4×3 | 4 | 3 | 14 | 7 | 7 | 31 | 18 | 0 |
| BG-015 | 4×4 | 4 | 3 | 14 | 8 | 8 | 58 | 44 | 0 |
| BG-016 | 4×4 | 4 | 3 | 15 | 9 | 9 | 86 | 66 | 0 |
| BG-017 | 4×4 | 4 | 2 | 15 | 8 | 8 | 62 | 48 | 0 |
| BG-018 | 4×4 | 4 | 3 | 16 | 8 | 8 | 74 | 50 | 5 |
| BG-019 | 4×4 | 4 | 3 | 16 | 9 | 9 | 55 | 41 | 0 |
| BG-020 | 4×4 | 4 | 3 | 17 | 10 | 10 | 106 | 89 | 0 |

## Constats

- **Progression respectée :** 2 couleurs et 1 couche pour 1–3, piles à 2 couches à partir de 4, 3 couleurs pour 7–10, 4 couleurs et jusqu'à 3 couches pour 11–20. La longueur des solutions monte de 2 à 10 coups et l'espace d'états de 4 à 124 positions.
- **Aucun piège sauf BG-018** (5 coups perdants) : sur 19 niveaux, toute suite de taps mène à la victoire. Le rapport les marque `trivial`. Conforme à D-107 ; à traiter au MVP2.
- **Conforme à l'exemple du GDD (chapitre 06)** : BG-001 contient bien 3 boutons `coral`, 3 boutons `blue` et 2 files avec un arrosoir de capacité 3 chacune (« Première floraison »).
- **Tutoriel (P5-03)** : il s'appuiera sur BG-001 à BG-003 (2 couleurs, 1 couche, 2 à 4 coups).
