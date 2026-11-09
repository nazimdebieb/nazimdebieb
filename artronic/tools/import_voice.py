"""Importe TES enregistrements (téléphone, micro…) dans un épisode.

Usage : python tools/import_voice.py episodes/01-condensateur dz ~/Téléchargements/enregistrements

Le dossier source contient un fichier par phrase, nommé avec l'identifiant du segment
(intro.m4a, plaques.m4a, charge.ogg… : m4a, mp3, wav, ogg, opus, aac acceptés ;
un préfixe numéroté comme « 1-intro.m4a » est accepté aussi).

Pour chaque fichier : conversion en mono 44,1 kHz, coupure des basses (souffle, vibrations),
réduction de bruit légère, volume normalisé (-16 LUFS, standard YouTube/podcast),
silence coupé au début et à la fin. Résultat : audio-<langue>/<id>.raw.mp3
Lance ensuite build.sh : il raccourcit les longues pauses (max_pause) et cale l'animation.
"""
import glob, json, os, re, subprocess, sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from voiceover import paths  # noqa: E402

EXTS = (".m4a", ".mp3", ".wav", ".ogg", ".opus", ".aac", ".flac", ".webm")
CLEAN = ",".join([
    "highpass=f=80",                       # souffle / vibrations
    "afftdn=nf=-25",                       # réduction de bruit légère
    "silenceremove=start_periods=1:start_threshold=-45dB:start_silence=0.05",  # début
    "areverse",
    "silenceremove=start_periods=1:start_threshold=-45dB:start_silence=0.1",   # fin
    "areverse",
    "loudnorm=I=-16:TP=-1.5:LRA=11",       # volume régulier entre les phrases
])


def find(src, seg_id):
    for f in sorted(glob.glob(os.path.join(src, "*"))):
        name, ext = os.path.splitext(os.path.basename(f))
        if ext.lower() in EXTS and re.sub(r"^\d+[-_ .]*", "", name).lower() == seg_id.lower():
            return f
    return None


def main(ep, lang, src):
    script, adir = paths(ep, lang)
    segs = json.load(open(script, encoding="utf-8"))["segments"]
    os.makedirs(adir, exist_ok=True)
    missing = []
    for seg in segs:
        f = find(src, seg["id"])
        if not f:
            missing.append(seg["id"])
            continue
        out = os.path.join(adir, f"{seg['id']}.raw.mp3")
        subprocess.check_call(["ffmpeg", "-v", "error", "-y", "-i", f, "-ac", "1", "-ar", "44100",
                               "-af", CLEAN, "-b:a", "160k", out])
        print("importé :", seg["id"], "<-", os.path.basename(f))
    if missing:
        print("manquants :", ", ".join(missing))


if __name__ == "__main__":
    main(*sys.argv[1:4])
