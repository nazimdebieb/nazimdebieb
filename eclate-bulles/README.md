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
- **Boss** : le 12e niveau de chaque monde est un combat de boss, avec une barre de vie. Chaque coup de harpon lui retire un point ; une fois vaincu, toutes les bulles restantes éclatent.
  - Monde 1, **Roi Bulle** : une bulle géante couronnée qui rebondit de plus en plus vite et lâche des petites bulles tous les 3 coups.
  - Monde 2, **Grand Orage** : un nuage qui fait pleuvoir des gouttes et des bulles, puis lance des éclairs (une ligne en pointillés prévient avant la frappe).
  - Monde 3, **Pieuvre Encre** : elle crache de l'encre en éventail et plonge sur toi après avoir tremblé.
  - Monde 4, **Œil du Néant** : protégé par 3 bulles en orbite qui arrêtent le harpon, il tire des lasers visés sur toi.
- **Survie** : un seul écran, pas de niveaux, 3 vies. Des bulles tombent par vagues (une marque jaune prévient où), de plus en plus vite et de plus en plus grosses. Éclater des bulles à moins de 2 s d'intervalle fait monter un combo (x2 dès 3 bulles, puis x3, x4, x5). Toutes les 45 s le terrain et le décor changent, et toutes les 2 minutes un boss arrive (plus résistant à chaque fois) pendant que les vagues continuent. Les 5 meilleurs scores sont gardés, avec le temps tenu (un tableau pour 1 joueur, un pour 2).

Les deux modes se jouent à 1 ou 2 joueurs (choix dans le menu).

## Règles

- Les petites bulles rapportent plus (30 → 100 points) ; le temps restant donne un bonus en fin de niveau.
- Bonus qui tombent parfois : double harpon, harpon collant (reste accroché au plafond), bouclier, bulles gelées, +500, +1 vie.
- Obstacles, dans l'esprit de Bubble Trouble 2 :
  - **plateformes en métal** : les bulles rebondissent dessus et le harpon s'y arrête (le harpon collant reste accroché dessous) ;
  - **briques** : les bulles rebondissent dessus, un coup de harpon les casse (+20) ;
  - **barrières** : elles coupent l'écran en zones et se lèvent quand toutes les bulles à leur gauche ont éclaté.
- 24 niveaux dessinés à la main (escaliers, étagères, damier de briques, tunnel, quatre barrières…), puis des niveaux générés de plus en plus chargés à partir de 11 dispositions d'obstacles. Le record est gardé dans le navigateur.
