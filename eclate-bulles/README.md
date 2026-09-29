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

## Règles

- 5 vies par joueur. Toucher une bulle ou laisser filer le temps coûte une vie et relance le niveau.
- Les petites bulles rapportent plus (30 → 100 points) ; le temps restant donne un bonus en fin de niveau.
- Bonus qui tombent parfois : double harpon, harpon collant (reste accroché au plafond), bouclier, bulles gelées, +500, +1 vie.
- Obstacles, dans l'esprit de Bubble Trouble 2 :
  - **plateformes en métal** : les bulles rebondissent dessus et le harpon s'y arrête (le harpon collant reste accroché dessous) ;
  - **briques** : les bulles rebondissent dessus, un coup de harpon les casse (+20) ;
  - **barrières** : elles coupent l'écran en zones et se lèvent quand toutes les bulles à leur gauche ont éclaté.
- 12 niveaux dessinés à la main, puis des niveaux générés de plus en plus chargés, avec obstacles. Le record est gardé dans le navigateur.
