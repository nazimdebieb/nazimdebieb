# Notes de version et procédure de publication (P9-01)

Numérotation : `MAJEUR.MINEUR.CORRECTIF` (`PlayerSettings.bundleVersion`) + code de version entier croissant (`bundleVersionCode`), tous deux fixés dans `Editor/ProjectSetup.cs` (`ConfigureAndroid`). Chaque APK testé est tracé ici et dans `Docs/QA/P3_android_build.txt` (dernier build).

| Version | Code | Date | Contenu | Testé sur téléphone |
|---|---|---|---|---|
| 0.3.0 | 3 | 2026-10-12 | Premier niveau jouable (primitives) | Non (pas d'appareil branché) |
| 0.4.0 | 4 | 2026-10-12 | Diorama, animations, sons, options | Oui (utilisateur) : tout OK sauf le son |
| 0.4.1 | 5 | 2026-10-12 | Correctif du son (AudioListener) | Oui (utilisateur) : son OK |
| 0.5.0 | 6 | 2026-10-12 | Accueil, niveaux, tutoriel, FR/EN, sauvegarde | Oui (utilisateur) : « ça marche » |
| 0.6.0 | 7 | 2026-10-13 | Journal de test local, protocole d'observation | Non |
| 0.7.0 | 8 | 2026-10-13 | 60 i/s, graphismes auto/élevés/économie, mesures de fluidité, thèmes Étang et Zen, icône, statistiques Unity désactivées, correctif de l'écran des niveaux | En attente du test final |

## Notes pour les testeurs — 0.7.0

- Le jeu vise maintenant 60 images par seconde (Android en imposait 30 par défaut).
- Options ▸ Graphismes : « auto » passe tout seul en mode économie si le téléphone rame ; « économie » coupe les ombres et allège les effets.
- Les niveaux 11 à 15 se jouent au bord d'un étang, les niveaux 16 à 20 dans un jardin zen.
- Nouvelle icône d'application.
- Corrigé : l'écran « Choix du jardin » laissait voir l'interface de jeu en transparence.

## Publier une version sur Google Play (bloqué : actions de ta part)

Le build actuel est un **APK signé avec la clé de débogage d'Unity** : parfait pour tester, **refusé par Google Play**. Pour publier :

1. **Clé de téléversement (à créer par toi)** : Unity ▸ Project Settings ▸ Player ▸ Android ▸ Publishing Settings ▸ Keystore Manager ▸ Create New. Choisis toi-même le mot de passe. Range le fichier `.keystore` **hors du dossier du projet** (le `.gitignore` exclut déjà `*.keystore` et `*.jks`) et fais-en une sauvegarde : perdue, elle bloque les mises à jour (sauf réinitialisation via Play App Signing).
2. Activer **Play App Signing** dans la Play Console (Google garde la clé de signature finale).
3. Construire un **AAB** : `EditorUserBuildSettings.buildAppBundle = true` puis Build (je peux ajouter le menu « Build AAB de publication » ; la saisie du mot de passe de la clé reste à faire par toi dans Unity).
4. Augmenter `bundleVersionCode` à chaque envoi.

P9-02 à P9-05 (retours utilisateurs, régressions, coût d'acquisition, décision de portefeuille) demandent un jeu publié et de vraies données : rien n'est simulé.
