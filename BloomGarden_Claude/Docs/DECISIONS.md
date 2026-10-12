# Journal des décisions — Bloom Garden (copie Claude)

Décisions héritées du kit (non modifiées) :

| ID | Statut | Décision | Date | Justification |
|---|---|---|---|---|
| D-001 | VERROUILLÉE | Jeu puzzle 2,5D portrait sans limite de temps | 2026-10-12 | Expérience relaxante |
| D-002 | VERROUILLÉE | 3 files, 3 places, 4 couleurs, arrosage automatique | 2026-10-12 | Prototype comparable |
| D-003 | VERROUILLÉE | Undo gratuit, victoire par fleurs toutes écloses | 2026-10-12 | Éviter frustration |
| D-004 | VERROUILLÉE | Unity 6+, C# logique pure, Android puis iOS | 2026-10-12 | Déploiement et test |
| D-005 | PROVISOIRE | Nom « Bloom Garden » | 2026-10-12 | Vérification marque nécessaire |
| D-006 | PROVISOIRE | UI Toolkit ou UI classique selon preuve de stabilité | 2026-10-12 | Arbitrage documenté par agent |
| D-007 | REPORTÉE | Boutique, publicité, achats | 2026-10-12 | Après test du gameplay |

Décisions secondaires prises par Claude (aucune ne change une règle verrouillée) :

| ID | Statut | Décision | Date | Options envisagées | Impact | Validation utilisateur |
|---|---|---|---|---|---|---|
| D-101 | PRISE | Places à position fixe (gauche → droite) avec un numéro d'ordre de sélection ; l'ancienneté décide de la priorité d'arrosage | 2026-10-12 | Liste compactée comme le Python | Même résultat que le Python (vérifié sur 1242 coups) ; la vue sait dans quelle place dessiner chaque arrosoir | Non requise |
| D-102 | PRISE | Blocage déclaré aussi quand des places sont libres mais toutes les files sont vides (aucun coup possible) | 2026-10-12 | Ne déclarer que « 3 places pleines » | Impossible avec des niveaux à eau = fleurs, mais évite qu'une partie reste sans issue ni message | Non requise |
| D-103 | PRISE | Une sélection est refusée avec une raison (`InvalidLane`, `GameOver`, `LaneEmpty`, `SlotsFull`) et l'état reste identique | 2026-10-12 | Exceptions | La vue peut donner un retour discret sans planter | Non requise |
| D-104 | PRISE | Lecteur JSON maison (`MiniJson`) dans `Core`, sans dépendance | 2026-10-12 | `JsonUtility` (ne lit pas les tableaux imbriqués `cells`), Newtonsoft (package en plus) | `Core` reste du C# pur, testable hors Unity | Non requise |
| D-105 | PRISE | Toute la logique pure (modèle, niveaux, règles) dans une seule assembly `BloomGarden.Core` (`noEngineReferences`) sous `Scripts/Core/{Model,Levels,Rules,Json}` | 2026-10-12 | Une assembly par dossier du GDD | `Scripts/Levels` est réservé au chargement côté Unity (TextAsset) | Non requise |
| D-106 | PRISE | Tests EditMode aussi exécutables hors Unity avec `dotnet test` (`Tools/DotnetTests`, C# 9 comme Unity 6) | 2026-10-12 | Seulement Unity Test Runner | Preuve de tests disponible sans éditeur ; le passage dans Unity Test Runner reste à faire | Non requise |
| D-107 | OUVERTE | 19 niveaux de référence sur 20 n'ont **aucun** état de blocage atteignable (n'importe quelle suite de taps gagne) | 2026-10-12 | Garder / régénérer avec un critère « pièges » | Le défi est nul hors BG-018 ; à traiter en P2-06 | **Oui** : à décider avant de retoucher les niveaux |
