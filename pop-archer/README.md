# Pop Archer

Jeu d'arcade dans l'esprit de Pang / Bubble Trouble : on tire un harpon vers le haut, chaque bulle touchée se coupe en deux bulles plus petites, jusqu'à ce que les plus petites éclatent. Un fichier `index.html` et un dossier `img/`, sans dépendance.

**Pop Archer** (anciennement « Éclate-Bulles ») est entièrement en anglais. On joue seul un petit **archer** à capuche rouge qui tire des flèches-grappins vers le haut. Chaque monde a son décor : forêt, désert en ruines, fonds marins et cosmos, avec des obstacles dans la matière du monde. Les personnages, bulles, boss et obstacles sont dessinés dans le code ; le logo « Pop Archer » et les décors des 4 mondes (2 ambiances chacun : forêt de jour et au couchant, désert de jour et au crépuscule, fonds marins clairs et grand fond, cosmos violet et bleu nuit) sont des images générées avec Canva, rangées dans `img/` (le dossier doit rester à côté de `index.html`). Si une image manque, le décor dessiné par le code prend le relais.

## Jouer

Ouvrir `index.html` dans un navigateur (double-clic suffit), sur ordinateur ou téléphone.

## Commandes

| | Bouger | Tirer |
|---|---|---|
| Clavier | `←` `→` ou `A` `D` (`Q` `D` en AZERTY) | `↑`, `W` (`Z` en AZERTY) ou `Espace` |

`P` ou `Échap` : pause. Sur téléphone, un pavé ◀ ▶ et un bouton **Tir** s'affichent.

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
- **Survie** : un seul écran, pas de niveaux, 3 vies. Des bulles tombent par vagues (une marque jaune prévient où), de plus en plus vite et de plus en plus grosses. Éclater des bulles à moins de 2 s d'intervalle fait monter un combo (x2 dès 3 bulles, puis x3, x4, x5). Toutes les 45 s le terrain et le décor changent, et toutes les 2 minutes un boss arrive (plus résistant à chaque fois) pendant que les vagues continuent. Les 5 meilleurs scores sont gardés, avec le temps tenu.

Le jeu se joue à 1 joueur (le mode à 2 joueurs a été retiré : trop compliqué sur un seul téléphone).

## Règles

- Les petites bulles rapportent plus (30 → 100 points) ; le temps restant donne un bonus en fin de niveau.
- Bonus qui tombent parfois : double harpon, harpon collant (reste accroché au plafond), bouclier, bulles gelées, +500, +1 vie.
- En Aventure, les bonus sont dosés pour que les étoiles restent à gagner :
  - chaque niveau donne **1 à 3 bonus au maximum** (selon le nombre de bulles, 3 pour un boss), avec au moins 5 s entre deux, et une seule vie en plus par niveau ;
  - trois bonus de la Survie arrivent monde par monde, en version plus courte : **Triple harpon** (monde 2, 6 s), **Ralenti** (monde 3, 5 s), **Mitraille** (monde 4, 5 s). Un message l'annonce au premier niveau du monde ;
  - Bombe, Étoile et Points x2 restent réservés à la Survie.
- Bonus en plus en Survie (une pastille en haut de l'écran montre ceux qui sont actifs et leur temps restant) :
  - **Mitraille** (8 s) : des balles courtes et rapides, jusqu'à 6 à l'écran ;
  - **Triple harpon** (10 s) : trois harpons en éventail à chaque tir ;
  - **Bombe** : toutes les bulles à l'écran éclatent une fois (les grosses se coupent en deux) ;
  - **Ralenti** (8 s) : bulles, boss et projectiles au ralenti ;
  - **Étoile** (6 s) : invincible, et les bulles touchées éclatent ;
  - **Points x2** (10 s) : se cumule avec le combo.
- Obstacles, dans l'esprit de Bubble Trouble 2 :
  - **plateformes en métal** : les bulles rebondissent dessus et le harpon s'y arrête (le harpon collant reste accroché dessous) ;
  - **briques** : les bulles rebondissent dessus, un coup de harpon les casse (+20) ;
  - **barrières** : elles coupent l'écran en zones et se lèvent quand toutes les bulles à leur gauche ont éclaté.
- 24 niveaux dessinés à la main (escaliers, étagères, damier de briques, tunnel, quatre barrières…), puis des niveaux générés de plus en plus chargés à partir de 11 dispositions d'obstacles. Le record est gardé dans le navigateur.
