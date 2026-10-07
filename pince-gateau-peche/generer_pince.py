"""Pince-moule pour gâteaux pêches (7abbet el khoukh) — modèle paramétrique.

Deux pièces reliées par une charnière (vis M3) :
  - bras_coupelle : coupelle hémisphérique (la forme du gâteau) + manche
  - bras_presse   : disque plat qui ferme la coupelle + manche
On remplit la coupelle de pâte, on serre : on obtient une demi-sphère
à fond plat, toujours identique.

Usage : python3 generer_pince.py   (pip install manifold3d trimesh numpy)
"""
import numpy as np
import trimesh
from manifold3d import Manifold, set_circular_segments

set_circular_segments(128)

# ---- Paramètres (mm) ----
DIAMETRE_GATEAU = 35.0   # diamètre de la demi-sphère obtenue
PAROI = 2.4              # épaisseur des parois
LONG_MANCHE = 110.0
LARG_MANCHE = 14.0
EP_MANCHE = 5.0
R_CHARNIERE = 5.0        # rayon extérieur des charnons
D_TROU_AXE = 3.3         # passage d'une vis M3
JEU = 0.4                # jeu entre charnons

R = DIAMETRE_GATEAU / 2
RE = R + PAROI                    # rayon extérieur coupelle / disque
HX = RE + R_CHARNIERE + 1.0       # position X de l'axe de charnière
W_MID = 6.0                       # largeur du charnon central (bras presse)
W_OUT = 5.5                       # largeur de chaque charnon extérieur
Y_OUT0 = W_MID / 2 + JEU
Y_OUT1 = Y_OUT0 + W_OUT


def box(x0, x1, y0, y1, z0, z1):
    return Manifold.cube([x1 - x0, y1 - y0, z1 - z0]).translate([x0, y0, z0])


def cyl_y(x, z, r, y0, y1):
    """Cylindre d'axe Y."""
    return (Manifold.cylinder(y1 - y0, r)
            .rotate([-90, 0, 0]).translate([x, y0, z]))


def cyl_z(r, z0, z1):
    return Manifold.cylinder(z1 - z0, r).translate([0, 0, z0])


def trou_axe():
    return cyl_y(HX, 0, D_TROU_AXE / 2, -50, 50)


# Plan de joint (pince fermée) : z = 0. Axe de charnière : (HX, z=0), selon Y.

def bras_coupelle():
    fond = -RE
    corps = cyl_z(RE, fond, 0)                       # extérieur à fond plat
    cavite = Manifold.sphere(R) - box(-50, 50, -50, 50, 0, 50)
    manche = box(-RE - LONG_MANCHE, -RE + 4, -LARG_MANCHE / 2, LARG_MANCHE / 2,
                 fond, fond + EP_MANCHE)
    charnons = Manifold()
    for y0, y1 in [(-Y_OUT1, -Y_OUT0), (Y_OUT0, Y_OUT1)]:
        charnons += cyl_y(HX, 0, R_CHARNIERE, y0, y1)
        charnons += box(RE - 3, HX, y0, y1, fond, 0)  # bras rejoignant la coupelle
        charnons += box(HX - R_CHARNIERE, HX + R_CHARNIERE, y0, y1, fond, 0)
    return corps + manche + charnons - cavite - trou_axe()


def bras_presse():
    disque = cyl_z(RE, 0, EP_MANCHE)
    manche = box(-RE - LONG_MANCHE, -RE + 4, -LARG_MANCHE / 2, LARG_MANCHE / 2,
                 0, EP_MANCHE)
    y0, y1 = -W_MID / 2, W_MID / 2
    charnon = (cyl_y(HX, 0, R_CHARNIERE, y0, y1)
               + box(RE - 3, HX, y0, y1, 0, EP_MANCHE))
    return disque + manche + charnon - trou_axe()


def axe_imprime():
    """Goupille alternative à la vis (imprimée debout, tête au plateau)."""
    long = 2 * Y_OUT1 + 1
    return cyl_z(3.2, 0, 1.5) + cyl_z(1.5, 0, long + 1.5)


def to_trimesh(m):
    mesh = m.to_mesh()
    return trimesh.Trimesh(np.asarray(mesh.vert_properties)[:, :3],
                           np.asarray(mesh.tri_verts), process=True)


def poser_au_plateau(t):
    t.apply_translation([0, 0, -t.bounds[0][2]])
    return t


if __name__ == "__main__":
    a = bras_coupelle()
    b = bras_presse()
    # Orientation d'impression : coupelle ouverture vers le haut (déjà le cas),
    # presse retournée face plate contre le plateau.
    b_print = b.rotate([180, 0, 0])
    pieces = {
        "bras_coupelle.stl": a,
        "bras_presse.stl": b_print,
        "axe_optionnel.stl": axe_imprime(),
    }
    for nom, m in pieces.items():
        t = poser_au_plateau(to_trimesh(m))
        assert t.is_watertight, nom
        t.export(nom)
        e = t.extents
        print(f"{nom}: {e[0]:.0f} x {e[1]:.0f} x {e[2]:.0f} mm, vol {t.volume/1000:.1f} cm3")
    # Assemblage fermé pour visualisation
    asm = to_trimesh(a + b)
    asm.export("apercu_assemblage.stl")
