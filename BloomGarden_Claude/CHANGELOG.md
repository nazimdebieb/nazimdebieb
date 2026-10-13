# Changelog

## 2026-10-13 — P7 / P8 / P9 : performance, thèmes, préparation store (0.7.0)
- 60 images/s (Android imposait 30) ; Options ▸ Graphismes auto / élevés / économie ; mesures de fluidité dans le journal de test.
- Thèmes Étang (niveaux 11–15) et Jardin zen (16–20).
- 30 niveaux candidats MVP2 avec vrais pièges, vérifiés par les deux solveurs, hors du jeu livré.
- Réglages iOS préparés ; plan iOS documenté (Mac requis).
- Icône de l'application et visuels du store rendus depuis le jeu ; captures FR/EN ; fiche store ; politique de confidentialité ; proposition de monétisation ; plan de soft launch ; notes de version.
- Statistiques matérielles Unity désactivées.
- Corrigé : l'écran des niveaux laissait voir l'interface de jeu ; l'icône n'était pas incluse dans l'APK.
- Alerte : le nom « Bloom Garden » est déjà utilisé par un jeu publié (décision D-124).
- Tests PlayMode 19/19, EditMode 72/72.

## 2026-10-13 — P6 : instrumentation de test et protocole (0.6.0)
- Journal de test local et anonyme (`PlaytestLog`) : début/fin de niveau, arrosoir choisi, annulation, recommencer, blocage, abandon, bulles du tutoriel, réglages ; aucun réseau, aucune donnée personnelle.
- Options → Données de test : résumé par niveau sur le téléphone, bouton Copier pour envoyer ses données, Effacer.
- Protocole d'observation, fiche par testeur, message prêt à envoyer, script de rapport `playtest_report.py`.
- Tests PlayMode 16/16, EditMode 70/70. P5 validé sur téléphone par l'utilisateur.

## 2026-10-12 — P5 : accueil, niveaux, tutoriel, FR/EN, sauvegarde (0.5.0)
- Écran d'accueil (diorama vivant derrière, « Jouer » / « Continuer : niveau N »), sélection des 20 niveaux avec progression verrouillée, bouton Menu en jeu.
- Tutoriel en bulles sur les niveaux 1 à 4 avec chevron vers l'arrosoir à toucher ; jamais bloquant, ne masque aucune cible.
- Français / anglais (langue de l'appareil par défaut, réglable dans Options), sans débordement de texte.
- Sauvegarde JSON versionnée, robuste (fichier temporaire + secours), migration des anciens réglages ; reprise exacte d'une partie interrompue, annulation comprise.
- Pause / arrière-plan : animation figée sur l'état réel et sauvegarde.
- Tests PlayMode 14/14, EditMode 70/70.

## 2026-10-12 — 0.4.1 : correctif du son
- Son muet sur téléphone (signalé sur 0.4.0) : `AudioListener` manquant sur la caméra. Ajouté dans la scène + filet de sécurité au démarrage + test PlayMode dédié (9/9).

## 2026-10-12 — P4 : visuels, animations, sons
- Diorama : île de gazon avec pierres, arbres ronds et buissons, estrade en bois pour les places et les files, ombres douces (1 cascade, MSAA 2×), palette retravaillée.
- Fleurs : bouton tulipe à sépales, fleur en coupe qui s'ouvre (6 pétales, cœur, étamines), étincelles ; arrosoirs avec anse, bec et pomme.
- Animations : trajectoire en arc vers la place, jet de gouttes depuis le bec, inclinaison d'arrosage, départ de l'arrosoir vide, têtes sélectionnables qui respirent, vague et pluie de pétales à la victoire.
- Sons synthétisés par code (aucune licence) ; bouton Options : son, vitesse ×2, animations réduites (préférences sauvegardées).
- Pooling des visuels : rejouer un niveau ne crée aucun objet.
- Tests PlayMode 8/8 (dont : même résultat dans les 3 modes d'animation), EditMode 62/62.

## 2026-10-12 — P3 : premier niveau jouable (vertical slice)
- Scène `Game` portrait : jardin 2,5D en primitives (palette du GDD), 3 places, 3 files, HUD (titre, compteur, Annuler, Recommencer), écrans de victoire et de blocage.
- Tap écran → rayon caméra → tête de file → un seul coup du `TurnResolver` ; animations (déplacement, gouttes, éclosion) sans influence sur la logique ; un tap pendant une animation recale l'affichage puis joue le coup.
- Accessibilité : forme différente bouton / fleur éclose, symbole par couleur, gouttes = eau restante.
- Tests PlayMode 5/5 (dont les 20 niveaux gagnés avec vérification vue = état), EditMode 62/62.
- APK de test 0.3.0 (IL2CPP ARM64, 27 Mo) ; installation sur téléphone en attente d'un appareil.
- Pilotage en direct de l'éditeur (Unity CLI + package Pipeline) ; P0-03 validé.

## 2026-10-12 — P2 validé (solveur, catalogue, rapport)
- `BloomGarden.Solvers` (C# pur) : `LevelSolver` (BFS, statuts Solvable / Unsolvable / Unknown), `LevelAnalyzer` (graphe complet : blocages, choix, coups perdants), `DifficultyReport` (CSV) et tranches pédagogiques du GDD ch. 08.
- `BloomGarden.Levels` : `LevelCatalog` (ScriptableObject, 20 TextAsset) ; outils éditeur `Bloom Garden ▸ P2 - …` utilisables en batchmode.
- Oracle Python `Tools/python/generate_solver_fixture.py` → `Fixtures/python_solver_reference.json` : le solveur C# retrouve les mêmes solutions, états explorés et analyses sur les 20 niveaux.
- Décision D-107 tranchée : niveaux du kit conservés pour le MVP1, pièges au MVP2.
- Tests Unity EditMode : 62/62.

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
