# Éclate-Bulles

Jeu d'arcade dans l'esprit de Pang / Bubble Trouble : on tire un harpon vers le haut, chaque bulle touchée se coupe en deux bulles plus petites, jusqu'à ce que les plus petites éclatent. Un seul fichier `index.html`, sans dépendance.

## Jouer

Ouvrir `index.html` dans un navigateur (double-clic suffit), sur ordinateur ou téléphone.

## Commandes

| | Bouger | Tirer |
|---|---|---|
| Joueur 1 | `←` `→` | `↑` ou `Espace` |
| Joueur 2 | `Q` `D` (AZERTY) / `A` `D` (QWERTY) | `Z` (AZERTY) / `W` (QWERTY) |

`P` ou `Échap` : pause. Sur téléphone, un pavé ◀ ▶ et un bouton **Tir** s'affichent ; à deux, chaque joueur a son côté de l'écran.

Sur téléphone, le jeu est toujours en paysage : si l'écran est tenu droit, le jeu s'affiche couché et il suffit de tourner le téléphone vers la gauche. Le bouton ⟳ retourne l'affichage pour ceux qui le tournent vers la droite.

## Modes

- **Aventure** : 48 niveaux répartis en 4 mondes de 12, à jouer un par un depuis une carte. Chaque niveau réussi débloque le suivant et la progression est sauvegardée. On a 3 cœurs par niveau (une bulle touchée = un cœur en moins) ; sans cœur ou sans temps, on recommence **juste ce niveau**. Étoiles :
  - ★ niveau fini ;
  - ★★ niveau fini sans être touché ;
  - ★★★ sans être touché et avec plus de la moitié du temps restant.
- **Arcade** : les niveaux s'enchaînent avec 5 vies. Toucher une bulle ou laisser filer le temps coûte une vie et relance le niveau ; sans vie, on repart du niveau 1. C'est le mode du record.

Les deux modes se jouent à 1 ou 2 joueurs (choix dans le menu).

## Règles

- Les petites bulles rapportent plus (30 → 100 points) ; le temps restant donne un bonus en fin de niveau.
- Bonus qui tombent parfois : double harpon, harpon collant (reste accroché au plafond), bouclier, bulles gelées, +500, +1 vie.
- Obstacles, dans l'esprit de Bubble Trouble 2 :
  - **plateformes en métal** : les bulles rebondissent dessus et le harpon s'y arrête (le harpon collant reste accroché dessous) ;
  - **briques** : les bulles rebondissent dessus, un coup de harpon les casse (+20) ;
  - **barrières** : elles coupent l'écran en zones et se lèvent quand toutes les bulles à leur gauche ont éclaté.
- 24 niveaux dessinés à la main (escaliers, étagères, damier de briques, tunnel, quatre barrières…), puis des niveaux générés de plus en plus chargés à partir de 11 dispositions d'obstacles. Le record est gardé dans le navigateur.
