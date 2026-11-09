"""Crée un fichier .srt (sous-titres YouTube/TikTok) depuis script.json + audio/timings.json.
Usage : python tools/subtitles.py episodes/01-condensateur [langue]
"""
import json, os, sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from voiceover import paths  # noqa: E402

PAD = 0.4  # utilisé seulement si audio/starts.json n'existe pas encore (avant le premier rendu)


def ts(s):
    ms = int(round(s * 1000)); h, ms = divmod(ms, 3600000); m, ms = divmod(ms, 60000); sec, ms = divmod(ms, 1000)
    return f"{h:02}:{m:02}:{sec:02},{ms:03}"


ep = sys.argv[1]
lang = sys.argv[2] if len(sys.argv) > 2 else None
script, adir = paths(ep, lang)
segs = json.load(open(script, encoding="utf-8"))["segments"]
tim = {t["id"]: t["duration"] for t in json.load(open(os.path.join(adir, "timings.json")))}
sp = os.path.join(adir, "starts.json")
starts = json.load(open(sp)) if os.path.exists(sp) else None  # écrit par la scène au rendu
t, out = 0.0, []
for i, s in enumerate(segs, 1):
    if starts:
        t = starts[s["id"]]
    out.append(f"{i}\n{ts(t)} --> {ts(t + tim[s['id']])}\n{s['text']}\n")
    t += tim[s["id"]] + PAD + 0.6
open(os.path.join(ep, "sous-titres.srt" if lang in (None, "", "fr") else f"sous-titres.{lang}.srt"), "w", encoding="utf-8").write("\n".join(out))
print("ok")
