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

## Versions en darija (dialecte algérien)

Même animation, autre voix : `script.dz.json` (texte en darija, termes techniques en français
comme on parle en Algérie) → `audio-dz/` → `export/Condensateur-dz.mp4` + `sous-titres.dz.srt`.

Voix algériennes natives trouvées dans la bibliothèque ElevenLabs (changer `voice_id`) :

| Voix | voice_id |
|------|----------|
| Amin – Algerian Darija (utilisée) | `wZ6E1XvJnMjmRvpvp2vY` |
| Achraf – Darija Accent | `NVRXBDmA6I8ZAldiUWuB` |
| Ben Chamsou – Podcast Host | `dbXZnF5pfQqN00XGwlRF` |
| Ilyass – Calm, Clear and Smooth | `jpofSqItAIlT4TLP5CrK` |

`"max_pause": 0.3` dans le script raccourcit les pauses trop longues de la voix
(les MP3 bruts `*.raw.mp3` sont gardés : changer ce réglage ne consomme pas de crédits).

## Épisodes

| # | Sujet | Statut |
|---|-------|--------|
| 01 | Le condensateur (structure, charge, τ = RC) | démo FR + darija |

## Idées pour la suite

- **Format vertical 9:16** (TikTok / Reels / Shorts) : même scène, `-r 1080,1920` et une mise en page adaptée.
- **Ta propre voix** : ElevenLabs peut cloner ta voix à partir d'un enregistrement propre —
  la chaîne garde ton identité sans les heures de reprises.
- **Schémas** : la skill CircuiTikZ peut produire des schémas normalisés pour les miniatures ou les posts.
