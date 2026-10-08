"""Pinces-moules pour gâteaux pêches (7abbet el khoukh), d'après le reel @zakiemballage.

Deux modèles, chacun en deux bras identiques reliés par une charnière (vis M3) :
  - goutte : pince turquoise. Les deux coupelles forment une pêche entière,
             corps à deux lobes et pointe ; une nervure courbe dans une
             coupelle imprime le sillon de la pêche.
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
NERVURE_H = 2.2           # hauteur de la nervure (profil triangulaire)
NERVURE_BASE = 4.4        # largeur de la nervure à sa base
NERVURE_POINTE = 0.3      # rayon d'arrondi de l'arête (pointue)
NERVURE_COURBE = 8.0      # écart max de la nervure par rapport à l'axe

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
    pointe = sphere((G_POINTE_X, 0, 0), max(G_R_POINTE + e, 0.2))
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


def nervure():
    """Nervure à profil triangulaire dans la coupelle goutte : part de la pointe et file en
    arc vers le dos arrondi, posée sur la surface de la cavité. Elle imprime
    le sillon courbe de la pêche."""
    cav = to_trimesh(demi_goutte(0.0), poser=False)
    x0, x1 = G_POINTE_X - 3.0, -(G_R_LOBE - 1.0)
    t = np.linspace(0, 1, 40)
    xy = np.c_[x0 + (x1 - x0) * t, NERVURE_COURBE * np.sin(np.pi * t)]
    origines = np.c_[xy, np.full(len(t), -100.0)]
    loc, i_ray, i_tri = cav.ray.intersects_location(
        origines, np.tile([0, 0, 1.0], (len(t), 1)), multiple_hits=False)
    ordre = np.argsort(i_ray)
    pts = loc[ordre]
    normales = -cav.face_normals[i_tri[ordre]]   # vers l'intérieur de la cavité
    # Section triangulaire : une large base noyée dans la paroi (largeur
    # NERVURE_BASE au niveau de la surface) et une arête fine à NERVURE_H.
    rb = NERVURE_BASE / 2 / 0.8
    sections = [(sphere(p - n * 0.6 * rb, rb), sphere(p + n * (NERVURE_H - NERVURE_POINTE), NERVURE_POINTE))
                for p, n in zip(pts, normales)]
    chaine = Manifold()
    for s0, s1 in zip(sections, sections[1:]):
        chaine += Manifold.batch_hull([*s0, *s1])
    return chaine ^ SOUS_JOINT


def bras(demi, x_avant, x_arriere, charnons_ext, relief=None):
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
    if relief is not None:
        cavite = cavite - relief
    trou = cyl_y(hx, 0, D_TROU_AXE / 2, -50, 50)
    piece = corps + manche + charnons - cavite - trou
    # Les booléens peuvent laisser des éclats de volume nul : on les retire.
    return max(piece.decompose(), key=lambda c: c.volume())


def modele(nom, demi, relief_a=None, relief_b=None):
    env = demi(PAROI).bounding_box()
    x_arriere, x_avant = env[0], env[3]
    a = bras(demi, x_avant, x_arriere, charnons_ext=True, relief=relief_a)
    b = bras(demi, x_avant, x_arriere, charnons_ext=False, relief=relief_b)
    return {f"{nom}_bras_A.stl": a, f"{nom}_bras_B.stl": b,
            f"{nom}_apercu_ferme.stl": a + b.rotate([180, 0, 0])}


def axe_imprime():
    long = 2 * (W_MID / 2 + JEU + W_OUT) + 1
    return Manifold.cylinder(1.5, 3.2) + Manifold.cylinder(long + 1.5, 1.5)


def to_trimesh(m, poser=True):
    mesh = m.to_mesh()
    t = trimesh.Trimesh(np.asarray(mesh.vert_properties)[:, :3],
                        np.asarray(mesh.tri_verts), process=False)
    if poser:
        t.apply_translation([0, 0, -t.bounds[0][2]])
    return t


def gateau(demi):
    """Volume du gâteau obtenu (pour contrôle)."""
    return demi(0.0) + demi(0.0).rotate([180, 0, 0])


if __name__ == "__main__":
    pieces = {}
    pieces.update(modele("goutte", demi_goutte, relief_a=nervure()))
    pieces.update(modele("ronde", demi_ronde, relief_b=etoile()))
    pieces["axe_optionnel.stl"] = axe_imprime()
    pieces["goutte_gateau_obtenu.stl"] = gateau(demi_goutte) - nervure()
    pieces["ronde_gateau_obtenu.stl"] = gateau(demi_ronde) - etoile().rotate([180, 0, 0])
    for nom, m in pieces.items():
        t = to_trimesh(m)
        assert t.is_watertight, nom
        t.export(nom)
        e = t.extents
        print(f"{nom}: {e[0]:.0f} x {e[1]:.0f} x {e[2]:.0f} mm, vol {t.volume/1000:.1f} cm3")
