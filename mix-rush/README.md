# Mix Rush — prototype jouable

Puzzle de peinture : on ne reçoit que des pots **rouges, jaunes et bleus**, on les mélange dans quelques bols pour peindre un tableau en pixel art.

## Jouer

Ouvrir `index.html` via un petit serveur local (les scripts sont chargés à côté de la page) :

```sh
cd mix-rush
python3 -m http.server 8000
# puis http://localhost:8000 sur l'ordinateur ou le téléphone (même Wi-Fi)
```

## Règles

- Seul le pot **devant** chaque colonne peut être pris. Touche un pot, puis un bol (ou glisse le pot sur le bol).
- Un bol mélange tout ce qu'il reçoit : rouge + jaune = orange, jaune + bleu = vert, rouge + bleu = violet, les trois = **boue** (bol bouché).
- Dès qu'un bol contient une couleur du tableau, il peint tout seul les pixels de cette couleur. **L'ordre des pots compte.**
- Quand un pot est levé, chaque bol affiche ce qui se passera : `+6` (peint 6 pixels), `Réserve`, `Inutile`, `Boue !`, `Plein`.
- Perdu quand plus aucun pot ne peut entrer dans un bol. Gagné quand le tableau est complet.
- Boosters : Annuler (×3), Rincer un bol (×1). Après un échec, « Rincer et continuer » simule l'offre d'échec (pub récompensée dans le jeu final).
- Symboles pour daltoniens, activés par défaut : rouge = barre verticale, jaune = barre horizontale, bleu = anneau ; un mélange porte les symboles de ses deux couleurs.

Les obstacles arrivent au niveau 8 (pots mystère « ? ») et au niveau 9 (pots gelés).

## Fichiers

| Fichier | Rôle |
|---|---|
| `engine.js` | Règles pures (mélange, peinture, défaite, solveur), utilisées par le jeu et les outils |
| `levels.js` | Les 10 niveaux, générés — ne pas éditer à la main |
| `game.js` | Rendu canvas, animations, sons, boosters, musée, progression |
| `tools/gen-levels.js` | Générateur de niveaux |

## Niveaux

`node tools/gen-levels.js` régénère `levels.js`. Pour chaque niveau, le générateur construit la pile à partir d'une solution connue, donc chaque niveau est gagnable sans booster. Il simule ensuite des milliers de parties pour choisir la pile dont la difficulté est la plus proche de la cible : un joueur « glouton » simulé gagne 100 % des parties au niveau 1 et environ 25 % au niveau 10.

Pour modifier un tableau ou la difficulté, éditer `SPECS` dans `tools/gen-levels.js` : image, nombre de bols et de colonnes, pots en trop, pots mystère, pots gelés, cible de réussite.
