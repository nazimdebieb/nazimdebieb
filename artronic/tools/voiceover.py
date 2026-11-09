"""Génère la voix off d'un épisode Artronic avec ElevenLabs.

Usage : python tools/voiceover.py episodes/01-condensateur

Lit script.json, crée un MP3 par segment dans <episode>/audio/,
puis écrit audio/timings.json (durée de chaque segment) que la scène
Manim utilise pour caler les animations sur la voix.
Un segment déjà généré n'est pas regénéré (pour ne pas consommer de crédits) :
supprime son MP3 pour le refaire.
"""
import json, os, subprocess, sys, urllib.request

API = "https://api.elevenlabs.io/v1/text-to-speech/{voice}?output_format=mp3_44100_128"


def tts(text, voice, model, out):
    body = json.dumps({
        "text": text,
        "model_id": model,
        "voice_settings": {"stability": 0.5, "similarity_boost": 0.8, "style": 0.3, "use_speaker_boost": True},
    }).encode()
    req = urllib.request.Request(API.format(voice=voice), data=body, method="POST", headers={
        "Content-Type": "application/json",
        # La clé est injectée par le proxy ; en local, mets ta clé dans ELEVENLABS_API_KEY.
        "xi-api-key": os.environ.get("ELEVENLABS_API_KEY", "dummy"),
    })
    with urllib.request.urlopen(req) as r, open(out, "wb") as f:
        f.write(r.read())


def duration(path):
    out = subprocess.check_output(["ffprobe", "-v", "error", "-show_entries", "format=duration",
                                   "-of", "csv=p=0", path])
    return float(out)


def main(ep):
    spec = json.load(open(os.path.join(ep, "script.json"), encoding="utf-8"))
    adir = os.path.join(ep, "audio")
    os.makedirs(adir, exist_ok=True)
    timings = []
    for seg in spec["segments"]:
        mp3 = os.path.join(adir, f"{seg['id']}.mp3")
        if not os.path.exists(mp3):
            print("voix :", seg["id"])
            tts(seg["text"], spec["voice_id"], spec["model_id"], mp3)
        timings.append({"id": seg["id"], "file": f"{seg['id']}.mp3", "duration": duration(mp3)})
    json.dump(timings, open(os.path.join(adir, "timings.json"), "w"), indent=2)
    print("total : %.1f s" % sum(t["duration"] for t in timings))


if __name__ == "__main__":
    main(sys.argv[1])
