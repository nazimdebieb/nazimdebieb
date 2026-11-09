"""Épisode 01 — Le condensateur, expliqué simplement.

Rendu : voir ../../build.sh (ou README).
"""
import math
import os
import sys

import numpy as np
from manim import *

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, os.path.join(HERE, "..", "..", "tools"))
from artronic_style import (ArtronicScene, T, logo, BG, FG, MUTED, CYAN, AMBER, RED, COPPER)  # noqa: E402

# Textes à l'écran (la version darija utilise "en" : termes techniques en anglais)
LABELS = {
    "fr": {
        "title": "Le condensateur",
        "subtitle": "expliqué simplement",
        "chap1": "1 · La structure",
        "plates": "plaques métalliques",
        "insulator": "isolant",
        "symbol": "symbole",
        "chap2": "2 · La charge",
        "switch": "interrupteur",
        "chap3": "3 · La tension",
        "time": "temps",
        "vc": "tension Vc",
        "vsrc": "tension de la pile",
        "chap4": "4 · La constante de temps",
        "pct63": "63 %",
        "full": "≈ 99 %  plein !",
        "chap5": "5 · Exemple",
        "calc": "τ = 10 000 Ω × 0,000 1 F = 1 s",
        "next": "Prochain épisode : la bobine",
        "subscribe": "Abonne-toi  ▶",
    },
    "en": {
        "title": "The Capacitor",
        "subtitle": "explained simply",
        "chap1": "1 · Structure",
        "plates": "metal plates",
        "insulator": "insulator",
        "symbol": "symbol",
        "chap2": "2 · Charging",
        "switch": "switch",
        "chap3": "3 · Voltage",
        "time": "time",
        "vc": "voltage Vc",
        "vsrc": "battery voltage",
        "chap4": "4 · Time constant",
        "pct63": "63%",
        "full": "≈ 99%  full!",
        "chap5": "5 · Example",
        "calc": "τ = 10,000 Ω × 0.0001 F = 1 s",
        "next": "Next episode: the inductor",
        "subscribe": "Subscribe  ▶",
    },
}

PLATE_X = 0.45      # demi-écart entre les plaques
PLATE_H = 3.0
CY = 0.6            # hauteur du centre des plaques
LOOP_X = 3.6        # largeur du circuit
LOOP_Y = -2.4       # fil du bas


def battery(center):
    """Pile : grand trait (+) et petit trait (−), verticaux sur le fil du bas."""
    long_ = Line(UP * 0.55, DOWN * 0.55, color=FG, stroke_width=6).move_to(center + LEFT * 0.15)
    short = Line(UP * 0.3, DOWN * 0.3, color=FG, stroke_width=10).move_to(center + RIGHT * 0.15)
    plus = T("+", 28, RED).next_to(long_, UP, buff=0.1)
    minus = T("−", 28, CYAN).next_to(short, UP, buff=0.1)
    return VGroup(long_, short, plus, minus)


class Condensateur(ArtronicScene):
    episode_dir = HERE
    labels = LABELS

    def construct(self):
        # ------------------------------------------------------------ INTRO
        self.segment("intro")
        lg = logo(1.3).move_to(UP * 0.8)
        title = T(self.tr("title"), 64, FG, weight="BOLD").next_to(lg, DOWN, buff=0.7)
        sub = T(self.tr("subtitle"), 34, CYAN).next_to(title, DOWN, buff=0.25)
        self.play(Create(lg[0]), FadeIn(lg[1], shift=RIGHT * 0.3), run_time=1.0)
        self.play(Write(title), run_time=1.0)
        self.play(FadeIn(sub, shift=UP * 0.2), run_time=0.6)
        self.finish(pad=0.2)
        self.play(FadeOut(VGroup(title, sub)), ReplacementTransform(lg, self.watermark), run_time=0.7)

        # ---------------------------------------------------------- PLAQUES
        self.segment("plaques")
        chap = self.chapter(self.tr("chap1"))
        left = Rectangle(width=0.18, height=PLATE_H, fill_color=COPPER, fill_opacity=1, stroke_width=0)
        right = left.copy()
        left.move_to([-PLATE_X, CY, 0])
        right.move_to([PLATE_X, CY, 0])
        diel = Rectangle(width=2 * PLATE_X - 0.18, height=PLATE_H, fill_color=CYAN, fill_opacity=0.12,
                         stroke_width=0).move_to([0, CY, 0])
        lab_plate = T(self.tr("plates"), 26, COPPER).move_to([-3.2, CY + 1.0, 0])
        arr1 = Arrow(lab_plate.get_right(), left.get_left() + UP * 0.6, buff=0.15, color=COPPER,
                     stroke_width=3, max_tip_length_to_length_ratio=0.12)
        arr1b = Arrow(lab_plate.get_right(), right.get_top() + DOWN * 0.3 + RIGHT * 0.0, buff=0.15,
                      color=COPPER, stroke_width=3, max_tip_length_to_length_ratio=0.08)
        lab_diel = T(self.tr("insulator"), 26, CYAN).move_to([3.0, CY - 1.0, 0])
        arr2 = Arrow(lab_diel.get_left(), diel.get_center() + DOWN * 0.5, buff=0.15, color=CYAN,
                     stroke_width=3, max_tip_length_to_length_ratio=0.15)
        # symbole normalisé
        sym = VGroup(Line(LEFT * 0.6, LEFT * 0.12), Line(LEFT * 0.12 + UP * 0.3, LEFT * 0.12 + DOWN * 0.3),
                     Line(RIGHT * 0.12 + UP * 0.3, RIGHT * 0.12 + DOWN * 0.3), Line(RIGHT * 0.12, RIGHT * 0.6))
        sym.set_stroke(FG, 4)
        sym_box = VGroup(sym, T(self.tr("symbol"), 20, MUTED).next_to(sym, DOWN, buff=0.15)).move_to([4.6, -2.3, 0])

        self.play(FadeIn(chap), GrowFromCenter(left), GrowFromCenter(right), run_time=0.9)
        self.play(FadeIn(lab_plate), GrowArrow(arr1), GrowArrow(arr1b), run_time=0.8)
        self.play(FadeIn(diel), FadeIn(lab_diel), GrowArrow(arr2), run_time=0.8)
        # « le courant ne traverse pas » : une flèche qui bute sur l'isolant
        block = Arrow([-2.2, CY - 0.6, 0], [-PLATE_X - 0.1, CY - 0.6, 0], buff=0, color=AMBER, stroke_width=5)
        cross = Cross(scale_factor=0.18, stroke_color=RED, stroke_width=6).move_to([0, CY - 0.6, 0])
        self.play(GrowArrow(block), run_time=0.6)
        self.play(Create(cross), run_time=0.4)
        self.play(FadeIn(sym_box, shift=UP * 0.2), run_time=0.5)
        self.finish(pad=0.1)
        self.play(FadeOut(VGroup(lab_plate, arr1, arr1b, lab_diel, arr2, block, cross, sym_box, chap)),
                  run_time=0.5)

        # ----------------------------------------------------------- CHARGE
        self.segment("charge")
        chap = self.chapter(self.tr("chap2"))
        # fils
        A = np.array([-PLATE_X - 0.09, CY, 0])
        B = np.array([-LOOP_X, CY, 0])
        C = np.array([-LOOP_X, LOOP_Y, 0])
        D = np.array([LOOP_X, LOOP_Y, 0])
        E = np.array([LOOP_X, CY, 0])
        F = np.array([PLATE_X + 0.09, CY, 0])
        sw_a = np.array([-LOOP_X, -0.4, 0])   # interrupteur sur le fil de gauche
        sw_b = np.array([-LOOP_X, -1.3, 0])
        bat_c = np.array([0, LOOP_Y, 0])
        wires = VGroup(
            Line(A, B), Line(B, sw_a), Line(sw_b, C), Line(C, bat_c + LEFT * 0.15),
            Line(bat_c + RIGHT * 0.15, D), Line(D, E), Line(E, F),
        ).set_stroke(COPPER, 4)
        bat = battery(bat_c)
        lever = Line(sw_b, sw_b + np.array([-0.55, 0.75, 0]), color=FG, stroke_width=5)
        pivots = VGroup(Dot(sw_a, radius=0.07, color=FG), Dot(sw_b, radius=0.07, color=FG))
        lab_sw = T(self.tr("switch"), 22, MUTED).next_to(sw_a + DOWN * 0.45, LEFT, buff=0.45)

        self.play(FadeIn(chap), Create(wires), FadeIn(bat), FadeIn(pivots), Create(lever), FadeIn(lab_sw),
                  run_time=1.0)
        self.play(Rotate(lever, angle=-math.atan2(0.55, 0.75), about_point=sw_b), run_time=0.4)
        closed = Line(sw_b, sw_a, color=FG, stroke_width=5)
        self.remove(lever)
        self.add(closed)

        # électrons : départ de la plaque gauche, tour du circuit, arrivée sur la plaque droite.
        # Les départs s'espacent de plus en plus -> la charge ralentit (exponentielle).
        n = 9
        ys = np.linspace(CY + 1.2, CY - 1.2, n)
        order = [4, 3, 5, 2, 6, 1, 7, 0, 8]  # on remplit depuis le centre
        starts = [-0.85 * math.log(1 - (i + 0.6) / (n + 0.4)) for i in range(n)]
        travel = 1.1
        total = max(starts) + travel
        clock = ValueTracker(0)
        paths = []
        for k in range(n):
            y = ys[order[k]]
            p = VMobject().set_points_as_corners([
                [-PLATE_X - 0.09, y, 0], [-PLATE_X - 0.6, CY, 0], B, C, D, E,
                [PLATE_X + 0.6, CY, 0], [PLATE_X + 0.09, y, 0]])
            paths.append(p)

        def electron(k):
            d = Dot(radius=0.09, color=CYAN)

            def upd(m):
                a = (clock.get_value() - starts[k]) / travel
                a = min(max(a, 0), 1)
                m.move_to(paths[k].point_from_proportion(a))
                m.set_opacity(0 if a <= 0 else 1)
            d.add_updater(upd)
            return d

        def charge_sign(k, side):
            y = ys[order[k]]
            if side == "+":
                s = T("+", 30, RED, weight="BOLD").move_to([-PLATE_X - 0.38, y, 0])
            else:
                s = T("−", 30, CYAN, weight="BOLD").move_to([PLATE_X + 0.38, y, 0])

            def upd(m):
                m.set_opacity(1 if clock.get_value() >= starts[k] + (0 if side == "+" else travel) else 0)
            s.add_updater(upd)
            return s

        es = VGroup(*[electron(k) for k in range(n)])
        plus = VGroup(*[charge_sign(k, "+") for k in range(n)])
        minus = VGroup(*[charge_sign(k, "-") for k in range(n)])
        self.add(es, plus, minus)
        seg_left = self.timings["charge"]["duration"] - 1.4 + 0.3
        self.play(clock.animate.set_value(total), run_time=max(seg_left, total * 0.6), rate_func=linear)
        for m in [*es, *plus, *minus]:
            m.clear_updaters()
        self.remove(es)
        self.finish(pad=0.1)

        # ------------------------------------------------------------ COURBE
        self.segment("courbe")
        circuit = VGroup(left, right, diel, wires, bat, pivots, closed, plus, minus)
        new_chap = self.chapter(self.tr("chap3"))
        self.play(circuit.animate.scale(0.5).move_to([-4.3, -0.2, 0]), FadeOut(lab_sw),
                  ReplacementTransform(chap, new_chap), run_time=0.9)
        chap = new_chap
        ax = Axes(x_range=[0, 6.3, 1], y_range=[0, 1.1, 0.25], x_length=7.2, y_length=4.4,
                  axis_config={"color": MUTED, "stroke_width": 2, "include_ticks": False},
                  tips=True).move_to([2.3, -0.3, 0])
        xl = T(self.tr("time"), 22, MUTED).next_to(ax.x_axis.get_end(), DOWN, buff=0.25).shift(LEFT * 0.1)
        yl = T(self.tr("vc"), 22, AMBER).next_to(ax.y_axis.get_end(), RIGHT, buff=0.2)
        vmax = DashedLine(ax.c2p(0, 1), ax.c2p(6.2, 1), color=MUTED, stroke_width=2, dash_length=0.1)
        vlab = T(self.tr("vsrc"), 20, MUTED).next_to(ax.c2p(6.2, 1), UP, buff=0.12).align_to(ax.c2p(6.2, 1), RIGHT)
        curve = ax.plot(lambda t: 1 - math.exp(-t), x_range=[0, 6.1], color=AMBER, stroke_width=6)
        self.play(Create(ax), FadeIn(xl), FadeIn(yl), run_time=0.7)
        self.play(Create(vmax), FadeIn(vlab), run_time=0.4)
        remaining = self.timings["courbe"]["duration"] - 2.0
        self.play(Create(curve), run_time=max(remaining, 1.5), rate_func=linear)
        self.finish(pad=0.2)

        # --------------------------------------------------------------- TAU
        self.segment("tau")
        new_chap = self.chapter(self.tr("chap4"))
        formula = T("τ = R × C", 44, AMBER, weight="BOLD").move_to([2.3, 2.6, 0])
        self.play(ReplacementTransform(chap, new_chap), Write(formula), run_time=1.0)
        chap = new_chap
        self.wait(1.4)
        p1 = ax.c2p(1, 1 - math.exp(-1))
        v1 = DashedLine(ax.c2p(1, 0), p1, color=CYAN, stroke_width=3)
        h1 = DashedLine(ax.c2p(0, 1 - math.exp(-1)), p1, color=CYAN, stroke_width=3)
        d1 = Dot(p1, color=CYAN, radius=0.08)
        t1 = T("1τ", 24, CYAN).next_to(ax.c2p(1, 0), DOWN, buff=0.15)
        l1 = T(self.tr("pct63"), 26, CYAN, weight="BOLD").next_to(p1, RIGHT, buff=0.2).shift(DOWN * 0.15)
        self.play(Create(v1), Create(h1), FadeIn(d1, scale=2), FadeIn(t1), run_time=0.8)
        self.play(FadeIn(l1, shift=LEFT * 0.2), Indicate(l1, color=CYAN), run_time=0.9)
        self.wait(0.6)
        p5 = ax.c2p(5, 1 - math.exp(-5))
        v5 = DashedLine(ax.c2p(5, 0), p5, color=CYAN, stroke_width=3)
        d5 = Dot(p5, color=CYAN, radius=0.08)
        t5 = T("5τ", 24, CYAN).next_to(ax.c2p(5, 0), DOWN, buff=0.15)
        l5 = T(self.tr("full"), 24, CYAN, weight="BOLD").next_to(p5, DOWN, buff=0.35).shift(LEFT * 0.6)
        self.play(Create(v5), FadeIn(d5, scale=2), FadeIn(t5), run_time=0.7)
        self.play(FadeIn(l5, shift=UP * 0.2), run_time=0.6)
        self.finish(pad=0.2)

        # ----------------------------------------------------------- EXEMPLE
        self.segment("exemple")
        new_chap = self.chapter(self.tr("chap5"))
        graph = VGroup(ax, xl, yl, vmax, vlab, curve, v1, h1, d1, t1, l1, v5, d5, t5, l5)
        self.play(FadeOut(graph), FadeOut(circuit), ReplacementTransform(chap, new_chap),
                  formula.animate.move_to([0, 2.0, 0]).scale(1.2), run_time=0.8)
        chap = new_chap
        r = T("R = 10 kΩ", 40, FG).move_to([-2.4, 0.6, 0])
        c = T("C = 100 µF", 40, FG).move_to([2.4, 0.6, 0])
        calc = T(self.tr("calc"), 38, FG).move_to([0, -0.6, 0])
        calc[-2:].set_color(AMBER)
        self.play(FadeIn(r, shift=RIGHT * 0.3), run_time=0.5)
        self.play(FadeIn(c, shift=LEFT * 0.3), run_time=0.5)
        self.play(Write(calc), run_time=1.3)
        box = SurroundingRectangle(calc[-2:], color=AMBER, buff=0.12, corner_radius=0.08)
        self.play(Create(box), run_time=0.4)
        self.wait(0.6)
        nxt = T(self.tr("next"), 34, CYAN).move_to([0, -2.1, 0])
        sub = T(self.tr("subscribe"), 28, FG, weight="BOLD").next_to(nxt, DOWN, buff=0.3)
        self.play(FadeIn(nxt, shift=UP * 0.2), run_time=0.6)
        self.play(FadeIn(sub, scale=1.2), run_time=0.5)
        self.finish(pad=0.6)
        self.play(FadeOut(Group(*self.mobjects)), run_time=0.6)
