# PROMPTS PAR PHASE — À ENVOYER APRÈS LE PROMPT MAÎTRE

Les prompts ci-dessous doivent être utilisés **dans l'ordre**, seulement si la phase précédente a été démontrée. Ils sont identiques pour Codex et Claude Code.

## P0 — Préparer l'environnement

« Vérifie l'environnement Unity 6+, Android Build Support (SDK/NDK/OpenJDK), le plugin Unity officiel et la capacité réelle de piloter l'éditeur via CLI/MCP. Crée la structure Git et les dossiers de projet sans installer de services payants. Écris `Docs/PLAN.md`, `Docs/ENVIRONMENT.md`, `Docs/QA/P0.md`. Précise exactement ce que tu as pu tester. »

**Acceptation :** scène vierge ouverte et enregistrée, projet compilable, version exacte documentée, Git propre.

## P1 — Écrire le moteur de règles pur C#

« Implémente les structures `LevelDefinition`, `GameState`, `Can`, `CellStack`, `TurnResolver`, `UndoHistory`, `ValidationResult` sans dépendance UnityEngine si possible. Implémente l'ordre déterministe des cibles et la stabilisation automatique tels que décrits au chapitre 05. Écris des tests EditMode pour les 15 invariants du GDD. N'ajoute pas encore de visuel complexe. »

**Acceptation :** 15 cas logiques documentés ; état sérialisable ; undo hash identique ; résultat reproductible.

## P2 — Données, solveur et niveaux

« Implémente lecture des JSON du dossier `levels/`, validation du schéma, solveur BFS exhaustif pour les petits niveaux, génération/replay de solutions. Compare les solutions du solveur Unity à `tools/solver_reference.py` ; fournis 20 niveaux solvables dans une progression pédagogique et les logs associés. Tout état inconnu par limite d'exploration doit être déclaré, jamais marqué solvable. »

**Acceptation :** 20 fichiers chargés et replays exécutés ; aucune impossibilité détectée ; rapports archivés.

## P3 — Premier vertical slice jouable

« Construis la scène portrait de jeu en Unity, caméra orthographique, jardin 2,5D, fleur/arrosoir générés en primitives stylisées, 3 files et 3 places, HUD, annuler et recommencer. Branche la vue sur `TurnResolver` : l'animation ne change jamais la logique. Teste un niveau de bout en bout avec captures de l'éditeur et, si possible, Android. »

**Acceptation :** lancement niveau 1 → sélections → floraisons → victoire → niveau 2, aucun crash ni dépendance externe.

## P4 — Visuels et animations premium

« À partir des images de référence du dossier `assets/`, crée une identité artistique originale pastel et cohérente. Ajoute fleurs en bouton et ouvertes, croissance, gouttes d'eau, mouvements des arrosoirs, végétation légère et petits sons. Implémente mode animations réduites et vitesse 2x. Optimise sans sacrifier la lisibilité. Compare captures avant/après. »

**Acceptation :** expérience claire, élégante, jouable sur petit écran, pas de baisse majeure de FPS.

## P5 — UX, progression, sauvegarde

« Ajoute onboarding interactif, menu, sélection des niveaux, résultat de victoire, paramètres, FR/EN et persistance locale versionnée. Les modes pause, redémarrage et annulation doivent conserver la cohérence de l'état. Corrige tous les problèmes bloquants remontés par QA. »

**Acceptation :** parcours complet sur Android ; sortie/reprise ; sauvegarde ; options ; localisation sans débordement.

## P6 — Alpha et boucle de retours

« Prépare une version interne pour tests de 20 niveaux avec instrumentation locale minimaliste. Fournis formulaire d'observation et instrumentation `level_started`, `can_selected`, `undo_used`, `deadlock_detected`, `level_completed`. Produis un tableau des échecs et recommande les corrections basées sur de vraies données. Ne simule pas des testeurs humains. »

**Acceptation :** protocole de test exécutable, aucun faux résultat, corrections priorisées.

## P7 — Contenu et performance

« Après validation humaine, prépare 50 à 100 niveaux supplémentaires seulement si leur qualité est contrôlée ; profile sur Android, optimises draw calls, mémoire et animations ; prépare la configuration iOS sans prétendre compiler iOS sans Mac/Xcode ou service adapté. »

**Acceptation :** nouveaux niveaux validés, performances documentées, plan iOS concret.

## P8 — Monétisation et stores (après accord explicite)

« Prépare seulement une proposition de monétisation respectueuse et une checklist de conformité Google Play/App Store. N'intègre aucune publicité, SDK invasif, achats intégrés ou compte payant sans mon accord. Distingue consentement, analytics, publicité et achats. »

**Acceptation :** plan chiffré de test, politique de confidentialité et conformité vérifiées au moment de lancer.

## P9 — Boucle de maintenance

« À chaque release, compare les données réelles, écris les problèmes corrigés, mets à jour tests automatiques, fais un build de validation et versionne les niveaux. Ajoute une date de revue des performances à J1/J7/J30 après publication. »

**Acceptation :** releases reproductibles et décisions basées sur mesures vérifiables.
