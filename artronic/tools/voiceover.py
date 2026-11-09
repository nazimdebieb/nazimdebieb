"""Génère la voix off d'un épisode Artronic avec ElevenLabs.

Usage : python tools/voiceover.py episodes/01-condensateur [langue]
        (langue : rien = français ; "en" ; "dz" = script.dz.json -> audio-dz/)

Si script.json contient "voice": "human", rien n'est généré : les enregistrements
viennent de tools/import_voice.py (ta propre voix).

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


def tighten(src, dst, max_pause):
    """Ramène chaque silence à max_pause secondes au plus (voix plus rythmée)."""
    f = (f"silenceremove=start_periods=1:start_threshold=-45dB:stop_periods=-1:"
         f"stop_duration={max_pause / 2}:stop_threshold=-45dB:stop_silence={max_pause / 2}")
    subprocess.check_call(["ffmpeg", "-v", "error", "-y", "-i", src, "-af", f, "-b:a", "128k", dst])


def duration(path):
    out = subprocess.check_output(["ffprobe", "-v", "error", "-show_entries", "format=duration",
                                   "-of", "csv=p=0", path])
    return float(out)


def paths(ep, lang):
    if lang in (None, "", "fr"):
        return os.path.join(ep, "script.json"), os.path.join(ep, "audio")
    return os.path.join(ep, f"script.{lang}.json"), os.path.join(ep, f"audio-{lang}")


def main(ep, lang=None):
    script, adir = paths(ep, lang)
    spec = json.load(open(script, encoding="utf-8"))
    os.makedirs(adir, exist_ok=True)
    timings = []
    max_pause = spec.get("max_pause")  # ex. 0.3 : raccourcit les pauses plus longues (en secondes)
    for seg in spec["segments"]:
        mp3 = os.path.join(adir, f"{seg['id']}.mp3")
        raw = os.path.join(adir, f"{seg['id']}.raw.mp3") if max_pause else mp3
        if not os.path.exists(raw):
            if spec.get("voice") == "human":
                sys.exit(f"Il manque l'enregistrement « {seg['id']} » : importe-le avec tools/import_voice.py")
            print("voix :", seg["id"])
            tts(seg["text"], spec["voice_id"], spec["model_id"], raw)
        if max_pause:
            tighten(raw, mp3, max_pause)
        timings.append({"id": seg["id"], "file": f"{seg['id']}.mp3", "duration": duration(mp3)})
    json.dump(timings, open(os.path.join(adir, "timings.json"), "w"), indent=2)
    print("total : %.1f s" % sum(t["duration"] for t in timings))


if __name__ == "__main__":
    main(sys.argv[1], sys.argv[2] if len(sys.argv) > 2 else None)
