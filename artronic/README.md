# Artronic — chaîne de production des vidéos

Objectif : passer de **1–2 semaines par vidéo** (PowerPoint + voix refaite des dizaines de fois)
à **quelques heures**, avec une qualité constante.

## Comment ça marche

```
script.json  ──►  voix off ElevenLabs (1 MP3 par phrase)  ──►  durées réelles
     │                                                              │
     └────────────►  scene.py (animation Manim)  ◄──────────────────┘
                              │
                              ▼
                  export/<Scene>.mp4  +  sous-titres.srt
```

1. **Le texte** (`script.json`) : découpé en segments courts. Tu écris ou corriges le texte,
   Claude propose la structure pédagogique.
2. **La voix** (`tools/voiceover.py`) : chaque segment est lu par une voix ElevenLabs.
   Une phrase ne te plaît pas ? Tu modifies son texte, tu supprimes son MP3, tu relances :
   seule cette phrase est regénérée. Fini les heures de ré-enregistrement.
3. **L'animation** (`scene.py`, [Manim](https://www.manim.community/), l'outil des vidéos
   de maths/sciences type 3Blue1Brown) : chaque bloc d'animation est calé automatiquement sur
   la durée réelle de sa phrase (`segment()` / `finish()`).
4. **La charte** (`tools/artronic_style.py`) : couleurs, police, logo, bandeaux de chapitre —
   identiques sur toutes les vidéos.

## Lancer un épisode

```bash
pip install manim            # + paquets système : libcairo2-dev libpango1.0-dev pkg-config ffmpeg
export ELEVENLABS_API_KEY=... # en local seulement
./build.sh episodes/01-condensateur Condensateur
QUALITY=-ql ./build.sh episodes/01-condensateur Condensateur   # aperçu rapide 480p
./build.sh episodes/01-condensateur Condensateur dz             # version darija algérienne
```

## Deux chaînes, une seule animation

| Chaîne | Voix off | Texte à l'écran | Commande |
|--------|----------|-----------------|----------|
| **Artronic Electronics** (anglais) | IA ElevenLabs, voix « Eric » (`script.en.json`) | anglais | `./build.sh … Condensateur en` |
| **Artronic** (darija) | **ta voix** (`script.dz.json`) | anglais | `./build.sh … Condensateur dz` |
| (français, démo d'origine) | IA (`script.json`) | français | `./build.sh … Condensateur` |

En darija, les mots techniques restent en anglais (capacitor, plates, switch, voltage…),
donc l'écran est en anglais pour les deux chaînes. Les textes à l'écran sont dans `LABELS`
au début de chaque `scene.py`.

### Ta voix en darija

1. Lis `ENREGISTREMENT-darija.md` de l'épisode : une phrase = un fichier (`1-intro.m4a`, `2-plaques.m4a`…).
2. `python tools/import_voice.py episodes/01-condensateur dz <dossier des enregistrements>` :
   nettoyage (souffle, bruit), volume normalisé, silences de début et de fin coupés.
3. `./build.sh episodes/01-condensateur Condensateur dz` : les pauses trop longues sont raccourcies
   (`max_pause`) et l'animation se cale sur ta voix.

Une phrase ratée ? Réenregistre seulement ce fichier et relance les étapes 2 et 3.

## Épisodes

| # | Sujet | Statut |
|---|-------|--------|
| 01 | Le condensateur / the capacitor (structure, charge, τ = RC) | EN prête · darija : en attente de ta voix · FR démo |

## Idées pour la suite

- **Format vertical 9:16** (TikTok / Reels / Shorts) : même scène, `-r 1080,1920` et une mise en page adaptée.
- **Ta propre voix** : ElevenLabs peut cloner ta voix à partir d'un enregistrement propre —
  la chaîne garde ton identité sans les heures de reprises.
- **Schémas** : la skill CircuiTikZ peut produire des schémas normalisés pour les miniatures ou les posts.
