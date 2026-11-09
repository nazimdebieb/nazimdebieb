"""Charte graphique Artronic + synchronisation voix/animation pour Manim.

Toutes les scènes d'épisodes héritent de ArtronicScene : mêmes couleurs,
même police, même logo, et la méthode `segment()` qui cale chaque bloc
d'animation sur la durée réelle de la voix off (audio/timings.json).
"""
import json
import os

from manim import (Scene, Text, VGroup, Line, RoundedRectangle, FadeIn, FadeOut,
                   UP, DOWN, LEFT, RIGHT, UL, config)

# --- Charte -----------------------------------------------------------------
BG = "#0E1116"        # fond
FG = "#E8ECF1"        # texte principal
MUTED = "#8A94A6"     # texte secondaire
CYAN = "#22D3EE"      # couleur Artronic (électrons, accents)
AMBER = "#F5B83D"     # tension, valeurs clés
RED = "#F87171"       # charges +
COPPER = "#C98B5B"    # fils, plaques
FONT = "Inter"

config.background_color = BG


def T(text, size=36, color=FG, weight="NORMAL", **kw):
    """Texte à la charte Artronic."""
    return Text(text, font=FONT, font_size=size, color=color, weight=weight, **kw)


def logo(scale=1.0):
    """Petit logo : une résistance en zigzag + ARTRONIC."""
    pts = [[-0.9, 0, 0], [-0.55, 0, 0]]
    x = -0.55
    for i in range(6):
        x += 0.12
        pts.append([x, 0.18 if i % 2 == 0 else -0.18, 0])
    pts += [[x + 0.12, 0, 0], [0.9, 0, 0]]
    zig = VGroup(*[Line(pts[i], pts[i + 1], color=CYAN, stroke_width=5) for i in range(len(pts) - 1)])
    word = T("ARTRONIC", 44, FG, weight="BOLD").next_to(zig, RIGHT, buff=0.3)
    return VGroup(zig, word).scale(scale)


class ArtronicScene(Scene):
    """Scène de base : charge les durées de la voix off et fournit segment()/finish()."""

    episode_dir = None  # à définir dans la scène de l'épisode

    def setup(self):
        adir = os.path.join(self.episode_dir, "audio")
        self.audio_dir = adir
        self.timings = {t["id"]: t for t in json.load(open(os.path.join(adir, "timings.json")))}
        self._seg = None
        self.starts = {}
        self.watermark = logo(0.32).to_corner(UL, buff=0.35).set_opacity(0.55)

    def chapter(self, label):
        """Bandeau de chapitre en haut à droite."""
        tag = T(label, 24, MUTED)
        box = RoundedRectangle(corner_radius=0.12, width=tag.width + 0.5, height=tag.height + 0.3,
                               stroke_color=MUTED, stroke_width=1.5)
        g = VGroup(box, tag).to_edge(UP, buff=0.35).to_edge(RIGHT, buff=0.4)
        tag.move_to(box)
        return g

    def segment(self, seg_id):
        """Démarre un segment : joue sa voix off et retient l'heure de départ."""
        t = self.timings[seg_id]
        self.add_sound(os.path.join(self.audio_dir, t["file"]))
        self._seg = (self.renderer.time, t["duration"])
        self.starts[seg_id] = round(self.renderer.time, 3)

    def tear_down(self):
        # heures de début réelles -> utilisées par tools/subtitles.py
        json.dump(self.starts, open(os.path.join(self.audio_dir, "starts.json"), "w"), indent=2)

    def finish(self, pad=0.4):
        """Attend la fin de la voix du segment courant (+ une petite respiration)."""
        start, dur = self._seg
        remaining = start + dur + pad - self.renderer.time
        if remaining > 0.01:
            self.wait(remaining)
