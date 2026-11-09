#!/usr/bin/env bash
# Construit un épisode Artronic : voix off -> animation -> vidéo finale.
#   ./build.sh episodes/01-condensateur Condensateur        (1080p, YouTube)
#   ./build.sh episodes/01-condensateur Condensateur dz     (voix off en darija)
#   QUALITY=-ql ./build.sh episodes/01-condensateur Condensateur   (aperçu rapide)
set -euo pipefail
EP=${1:?dossier épisode}; SCENE=${2:?nom de la scène}; ARTRONIC_LANG=${3:-fr}
export ARTRONIC_LANG
SUFFIX=$([ "$ARTRONIC_LANG" = fr ] && echo "" || echo "-$ARTRONIC_LANG")
MANIM=${MANIM:-manim}
QUALITY=${QUALITY:--r 1920,1080 --frame_rate 30}
cd "$(dirname "$0")"
python3 tools/voiceover.py "$EP" "$ARTRONIC_LANG"
$MANIM $QUALITY --media_dir "build/media$SUFFIX" "$EP/scene.py" "$SCENE"
python3 tools/subtitles.py "$EP" "$ARTRONIC_LANG"
mkdir -p "$EP/export"
cp "$(ls -t "build/media$SUFFIX"/videos/scene/*/"$SCENE".mp4 | head -1)" "$EP/export/$SCENE$SUFFIX.mp4"
echo "Vidéo : $EP/export/$SCENE$SUFFIX.mp4"
