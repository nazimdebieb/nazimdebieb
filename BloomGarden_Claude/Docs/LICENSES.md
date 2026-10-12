# Journal des assets et licences

Mis à jour : 2026-10-12 (P4). Aucun asset tiers n'est utilisé dans `BloomGarden_Claude`.

| Élément | Origine | Licence / statut |
|---|---|---|
| Modèles 3D (fleurs, arrosoirs, arbres, pierres, estrade) | Primitives Unity (sphère, cylindre, cube, capsule) assemblées par code (`FlowerFactory`, `CanView`, `GardenView`) | Création originale du projet |
| Matériaux | URP Lit, couleurs de la palette du GDD (`SceneBuilder`) | Création originale |
| Sons | Synthétisés par code au démarrage (`GardenAudio`) | Création originale, aucun fichier audio |
| Police | `LegacyRuntime.ttf`, police intégrée à Unity | Fournie avec Unity, utilisée selon les conditions d'Unity |
| Niveaux BG-001 à BG-020 | Kit du projet (`Kit/levels`), générés par `solver_reference.py` | Fournis par le porteur du projet |
| Images `Kit/assets` | Références conceptuelles du kit | **Non utilisées dans le jeu** (inspiration seulement) |
| Packages Unity | URP, uGUI, Test Framework, Pipeline (outil éditeur, exclu des builds non-développement) | Packages Unity |
