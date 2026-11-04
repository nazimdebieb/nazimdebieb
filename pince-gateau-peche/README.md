# Pinces-moules gâteaux pêches (7abbet el khoukh)

Reproduction des pinces du reel @zakiemballage (modèle paramétrique : `generer_pince.py`).

| Modèle | Gâteau obtenu | Fichiers à imprimer |
|---|---|---|
| **Goutte** (pince turquoise) | pêche entière 46 × 36 × 46 mm, 2 lobes + pointe + sillon courbe (nervure dans le bras A) | `goutte_bras_A.stl`, `goutte_bras_B.stl` |
| **Ronde** (pince bleue) | nectarine Ø40 × 32 mm, creux en étoile pyramidale (petit plateau plat au centre) | `ronde_bras_A.stl`, `ronde_bras_B.stl` |

+ `axe_optionnel.stl` si vous n'avez pas de vis M3 × 25 + écrou frein.
Les fichiers `*_apercu_ferme.stl` et `*_gateau_obtenu.stl` servent seulement à visualiser.

## Impression sur Bambu Lab A1 (Bambu Studio)

- Importer les 2 bras d'un modèle : ils tiennent ensemble sur le plateau 256 × 256.
- Ne pas tourner les pièces : coupelle vers le haut, fond plat sur le plateau. **Supports : désactivés.**
- Filament : PETG (Bambu PETG HF ou PETG alimentaire), profil « 0.20mm Standard @BBL A1 ».
- Parois : 4 · Remplissage : 25 % gyroïde · Couches supérieures/inférieures : 5.
- Pour une coupelle plus lisse (meilleur démoulage) : hauteur de couche 0,12 mm.
- Durée ≈ 2 h 30 par pince.

Nettoyer à l'eau tiède savonneuse (pas de lave-vaisselle) et fariner/huiler légèrement avant usage.

Changer la taille : modifier `G_R_LOBE` / `R_DIAM`, puis `pip install manifold3d trimesh numpy && python3 generer_pince.py`.
