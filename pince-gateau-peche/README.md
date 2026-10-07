# Pince-moule gâteaux pêches (7abbet el khoukh)

Inspirée du reel @zakiemballage. Modèle paramétrique (`generer_pince.py`).

| Fichier | Rôle | Orientation |
|---|---|---|
| `bras_coupelle.stl` | coupelle Ø35 mm (forme du gâteau) + manche | telle quelle, ouverture vers le haut |
| `bras_presse.stl` | disque qui ferme la coupelle + manche | telle quelle, face plate au plateau |
| `axe_optionnel.stl` | goupille si pas de vis | debout, tête en bas |
| `apercu_assemblage.stl` | pince fermée, pour visualiser (ne pas imprimer) | — |

Aucun support nécessaire. Assemblage : vis **M3 × 25** + écrou frein (ou la goupille imprimée).

Réglages conseillés : PETG (alimentaire de préférence), 0,2 mm, 4 périmètres, 30 % de remplissage.

Changer la taille : modifier `DIAMETRE_GATEAU` puis `pip install manifold3d trimesh numpy && python3 generer_pince.py`.
