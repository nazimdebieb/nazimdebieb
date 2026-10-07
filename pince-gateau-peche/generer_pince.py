"""Pinces-moules pour gâteaux pêches (7abbet el khoukh), d'après le reel @zakiemballage.

Deux modèles, chacun en deux bras identiques reliés par une charnière (vis M3) :
  - goutte : pince turquoise. Les deux coupelles forment une pêche entière,
             corps à deux lobes (sillon au plan de joint) et pointe.
  - ronde  : pince bleue. Boule légèrement aplatie ; une coupelle porte une
             étoile à 4 branches qui marque le creux en X sur le dessus.

Les bras s'impriment coupelle vers le haut, fond plat, sans support.
Usage : pip install manifold3d trimesh numpy && python3 generer_pince.py
"""
import numpy as np
import trimesh
from manifold3d import Manifold, set_circular_segments

set_circular_segments(96)

# ---- Paramètres communs (mm) ----
PAROI = 2.4
LONG_MANCHE = 105.0
LARG_MANCHE = 15.0
EP_MANCHE = 8.0
R_CHARNIERE = 5.0
D_TROU_AXE = 3.3          # vis M3
JEU = 0.4
W_MID = 6.0
W_OUT = 5.5

# ---- Pêche « goutte » ----
G_R_LOBE = 18.0           # rayon de chaque lobe
G_DECALAGE = 5.0          # écart lobe / plan de joint -> profondeur du sillon
G_POINTE_X = 26.0         # distance centre -> pointe (longueur totale ~ 44 mm)
G_R_POINTE = 1.5

# ---- Pêche « ronde » ----
R_DIAM = 40.0
R_HAUT = 32.0             # hauteur totale (boule aplatie)
ETOILE_BRANCHE = 11.0      # rayon de l'étoile
ETOILE_RELIEF = 6.0       # hauteur au centre


def box(x0, x1, y0, y1, z0, z1):
    return Manifold.cube([x1 - x0, y1 - y0, z1 - z0]).translate([x0, y0, z0])


def sphere(c, r):
    return Manifold.sphere(r).translate(list(c))


def cyl_y(x, z, r, y0, y1):
    return Manifold.cylinder(y1 - y0, r).rotate([-90, 0, 0]).translate([x, y0, z])


SOUS_JOINT = box(-200, 200, -200, 200, -200, 0)


# Chaque forme renvoie (cavité, enveloppe extérieure) pour la demi-coupelle
# sous le plan de joint z=0. L'enveloppe est la cavité décalée de PAROI.

def demi_goutte(e=0.0):
    lobe = sphere((0, 0, -G_DECALAGE), G_R_LOBE + e)
    epaule = sphere((G_POINTE_X * 0.55, 0, -G_DECALAGE * 0.6), G_R_LOBE * 0.62 + e)
    pointe = sphere((G_POINTE_X, 0, 0), G_R_POINTE + e)
    return Manifold.batch_hull([lobe, epaule, pointe]) ^ SOUS_JOINT


def demi_ronde(e=0.0):
    rx = R_DIAM / 2 + e
    rz = R_HAUT / 2 + e
    return Manifold.sphere(1.0).scale([rx, rx, rz]) ^ SOUS_JOINT


def etoile():
    """Relief en étoile au fond de la coupelle (pôle en z = -R_HAUT/2)."""
    rx, rz = R_DIAM / 2, R_HAUT / 2
    centre = sphere((0, 0, -rz + ETOILE_RELIEF), 2.5)
    base = sphere((0, 0, -rz - 1), 6.0)
    z_bout = -rz * np.sqrt(1 - (ETOILE_BRANCHE / rx) ** 2)
    branches = Manifold()
    for a in np.radians([0, 90, 180, 270]):
        bout = sphere((ETOILE_BRANCHE * np.cos(a), ETOILE_BRANCHE * np.sin(a), z_bout), 1.0)
        branches += Manifold.batch_hull([centre, base, bout])
    return branches


def bras(demi, x_avant, x_arriere, charnons_ext, etoile_fond=False):
    """Un bras : coupelle à fond plat + manche + charnons, rim au plan z=0."""
    cavite = demi(0.0)
    env = demi(PAROI)
    fond = env.bounding_box()[2]
    # Fond plat : enveloppe convexe de la coque et de son empreinte au plateau.
    empreinte = (env ^ box(-200, 200, -200, 200, -0.5, 0)).translate([0, 0, fond + 0.5])
    corps = Manifold.batch_hull([env, empreinte])
    manche = box(x_avant - 6, x_avant + LONG_MANCHE, -LARG_MANCHE / 2, LARG_MANCHE / 2,
                 fond, fond + EP_MANCHE)
    hx = x_arriere - R_CHARNIERE - 1.0
    y_out0 = W_MID / 2 + JEU
    tranches = ([(-y_out0 - W_OUT, -y_out0), (y_out0, y_out0 + W_OUT)]
                if charnons_ext else [(-W_MID / 2, W_MID / 2)])
    charnons = Manifold()
    for y0, y1 in tranches:
        charnons += cyl_y(hx, 0, R_CHARNIERE, y0, y1)
        charnons += box(hx - R_CHARNIERE, x_arriere + 3, y0, y1, fond, 0)
    if etoile_fond:
        cavite = cavite - etoile()
    trou = cyl_y(hx, 0, D_TROU_AXE / 2, -50, 50)
    return corps + manche + charnons - cavite - trou


def modele(nom, demi, etoile_bras_b=False):
    env = demi(PAROI).bounding_box()
    x_arriere, x_avant = env[0], env[3]
    a = bras(demi, x_avant, x_arriere, charnons_ext=True)
    b = bras(demi, x_avant, x_arriere, charnons_ext=False, etoile_fond=etoile_bras_b)
    return {f"{nom}_bras_A.stl": a, f"{nom}_bras_B.stl": b,
            f"{nom}_apercu_ferme.stl": a + b.rotate([180, 0, 0])}


def axe_imprime():
    long = 2 * (W_MID / 2 + JEU + W_OUT) + 1
    return Manifold.cylinder(1.5, 3.2) + Manifold.cylinder(long + 1.5, 1.5)


def to_trimesh(m):
    mesh = m.to_mesh()
    t = trimesh.Trimesh(np.asarray(mesh.vert_properties)[:, :3],
                        np.asarray(mesh.tri_verts), process=True)
    t.apply_translation([0, 0, -t.bounds[0][2]])
    return t


def gateau(demi):
    """Volume du gâteau obtenu (pour contrôle)."""
    return demi(0.0) + demi(0.0).rotate([180, 0, 0])


if __name__ == "__main__":
    pieces = {}
    pieces.update(modele("goutte", demi_goutte))
    pieces.update(modele("ronde", demi_ronde, etoile_bras_b=True))
    pieces["axe_optionnel.stl"] = axe_imprime()
    pieces["goutte_gateau_obtenu.stl"] = gateau(demi_goutte)
    pieces["ronde_gateau_obtenu.stl"] = gateau(demi_ronde) - etoile().rotate([180, 0, 0])
    for nom, m in pieces.items():
        t = to_trimesh(m)
        assert t.is_watertight, nom
        t.export(nom)
        e = t.extents
        print(f"{nom}: {e[0]:.0f} x {e[1]:.0f} x {e[2]:.0f} mm, vol {t.volume/1000:.1f} cm3")
