# Pop Archer

Jeu d'arcade dans l'esprit de Pang / Bubble Trouble : on tire un harpon vers le haut, chaque bulle touchée se coupe en deux bulles plus petites, jusqu'à ce que les plus petites éclatent. Un fichier `index.html` et un dossier `img/`, sans dépendance.

**Pop Archer** (anciennement « Éclate-Bulles ») est entièrement en anglais. On joue seul un petit **archer** à capuche rouge qui tire des flèches-grappins vers le haut. Chaque monde a son décor : forêt, désert en ruines, fonds marins, cosmos, pics gelés et volcan, avec des obstacles dans la matière du monde. Les bulles et les portes sont dessinées dans le code. Sont des images générées avec Canva, rangées dans `img/` (le dossier doit rester à côté de `index.html`) :

- le logo « Pop Archer » ;
- les décors des 6 mondes, 2 ambiances chacun : forêt de jour et au couchant, désert de jour et au crépuscule, fonds marins clairs et grand fond, cosmos violet et bleu nuit, pics gelés de jour et sous les aurores boréales, volcan en éruption et grotte de lave ;
- les 6 boss : Bubble King, Great Storm, Ink Octopus, Void Eye, Frost Yeti et Lava Dragon. Leur portrait apparaît aussi sur la case « Boss » de la carte.
- l'archer (`archer-*.webp`) en 3 poses : debout, en marche et en train de tirer vers le haut. Il est dessiné tourné vers la droite et retourné en miroir quand il va à gauche ;
- les plateformes et briques de chaque monde (`plat-*.webp`, `brick-*.webp`) : rondin herbeux et caisse en bois (forêt), dalle de grès gravée et brique d'argile (désert), roche à coquillages et bloc de corail (fonds marins), plateforme métal néon et bloc de cristal (cosmos), corniche enneigée et bloc de glace (pics gelés), roche volcanique et bloc de lave (volcan). Les plateformes gardent leurs bouts arrondis quelle que soit leur longueur ; le milieu est répété ;
- les 12 icônes de bonus (`item-*.webp`), dans le même style : pastille arrondie brillante avec un symbole blanc. Elles servent pour les bonus qui tombent et pour les pastilles d'effet en haut à gauche.

Si une image manque, le dessin fait par le code prend le relais.

## Jouer

Ouvrir `index.html` dans un navigateur (double-clic suffit), sur ordinateur ou téléphone.

## Commandes

| | Bouger | Tirer |
|---|---|---|
| Clavier | `←` `→` ou `A` `D` (`Q` `D` en AZERTY) | `↑`, `W` (`Z` en AZERTY) ou `Espace` |

`P` ou `Échap` : pause. Sur téléphone, un pavé ◀ ▶ et un bouton **Tir** s'affichent.

Sur téléphone, le jeu est toujours en paysage : si l'écran est tenu droit, le jeu s'affiche couché et il suffit de tourner le téléphone vers la gauche. Le bouton ⟳ retourne l'affichage pour ceux qui le tournent vers la droite.

## Modes

- **Aventure** : 72 niveaux répartis en 6 mondes de 12, à jouer un par un depuis une carte. Chaque niveau réussi débloque le suivant et la progression est sauvegardée. On a 3 cœurs par niveau (une bulle touchée = un cœur en moins) ; sans cœur ou sans temps, on recommence **juste ce niveau**. Étoiles :
  - ★ niveau fini ;
  - ★★ niveau fini sans être touché ;
  - ★★★ sans être touché et avec plus de la moitié du temps restant.
- **Boss** : le 12e niveau de chaque monde est un combat de boss, avec une barre de vie. Chaque coup de harpon lui retire un point ; une fois vaincu, toutes les bulles restantes éclatent.
- **Monde 5, Frozen Peaks** (niveaux 49 à 60) : le sol est en glace, l'archer glisse et met un peu de temps à s'arrêter ou à repartir. Boss : le **Frost Yeti**, qui lance des boules de neige puis saute d'un bord à l'autre ; à chaque atterrissage, des stalactites tombent du plafond (signalées en rouge avant de tomber). Au sol sa fourrure arrête les flèches : on ne peut le blesser que pendant ses sauts.
- **Monde 6, Volcano** (niveaux 61 à 72) : des geysers de lave jaillissent du sol, annoncés par des bulles et des pointillés 1,3 s avant. Boss : le **Lava Dragon**, qui vole en haut de l'écran, crache des boules de feu en cloche (3 d'un coup quand il enrage) et un souffle de flammes visé sur l'archer.
  - Monde 1, **Roi Bulle** : une bulle géante couronnée qui rebondit de plus en plus vite et lâche des petites bulles tous les 3 coups.
  - Monde 2, **Grand Orage** : un nuage qui fait pleuvoir des gouttes et des bulles, puis lance des éclairs (une ligne en pointillés prévient avant la frappe).
  - Monde 3, **Pieuvre Encre** : elle crache de l'encre en éventail et plonge sur toi après avoir tremblé.
  - Monde 4, **Œil du Néant** : protégé par 3 bulles en orbite qui arrêtent le harpon, il tire des lasers visés sur toi.
- **Survie** : un seul écran, pas de niveaux, 3 vies. Des bulles tombent par vagues (une marque jaune prévient où), de plus en plus vite et de plus en plus grosses. Éclater des bulles à moins de 2 s d'intervalle fait monter un combo (x2 dès 3 bulles, puis x3, x4, x5). Toutes les 45 s le terrain et le décor changent, et toutes les 2 minutes un boss arrive (plus résistant à chaque fois) pendant que les vagues continuent. Les 5 meilleurs scores sont gardés, avec le temps tenu.

Le jeu se joue à 1 joueur (le mode à 2 joueurs a été retiré : trop compliqué sur un seul téléphone).

## Règles

- **Tailles de bulles** : de la mini à la grosse, plus deux nouvelles : l'**énorme** (dès le monde 1) et le **titan** (à partir du monde 4, et en Survie). Une grosse bulle se coupe en deux à chaque coup, jusqu'à la mini qui éclate.
- **Bulles spéciales**, chacune avec sa couleur et son signe. Un message les présente la première fois qu'elles apparaissent :

  | Bulle | Signe | Effet |
  |---|---|---|
  | **Noire** | yeux rouges, halo rouge | Si elle te touche, tu perds **tous tes cœurs** d'un coup (toutes tes vies en Survie). Le bouclier ne protège pas, seule l'étoile d'invincibilité la fait éclater. |
  | **Dorée** | étoile | Points x3, 3 pièces à chaque éclatement, un bonus tombe une fois sur deux |
  | **Acier** | rivets | Il faut 2 coups : le premier la fissure, le second la coupe en deux bulles normales |
  | **Verte** | trois points | Se coupe en **trois** au lieu de deux |
  | **Fantôme** | violette transparente, deux yeux | Traverse les plateformes et les briques (pas les barrières) |
  | **Bombe** | mèche allumée | En éclatant, elle explose et touche toutes les bulles autour, puis se coupe en deux bulles normales |
  | **Rebondissante** | chevrons | Plus rapide et rebondit plus haut |

  Elles arrivent peu à peu : la dorée au niveau 5, la rebondissante au 7, l'acier au 9, la verte au 13, la noire au 16, la fantôme au 19, la bombe au 21. Ensuite, environ une bulle sur trois est spéciale. En Survie, elles se débloquent avec le temps (la dorée à 20 s, puis toutes les 20 s environ, et la noire à 2 min 30), avec jamais plus d'une noire à la fois. Les niveaux avec des bulles noires, fantômes ou rebondissantes donnent un peu plus de temps.

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

## Son, vibrations et tutoriel

- **Musique** jouée par le jeu lui-même (aucun fichier audio) : une boucle pour le menu, une par monde (forêt, désert, fonds marins, cosmos) et une pour les boss. Elle baisse pendant la pause et s'arrête sur l'écran de fin et quand l'appli passe en arrière-plan.
- **Vibrations** sur téléphone : petite secousse à chaque bulle éclatée, plus forte quand on est touché, une série pour la bulle noire, et une petite quand on réussit un niveau.
- **Réglages** (roue dentée en haut à droite) : bruitages, musique et vibrations séparés, et « Reset progress » pour tout effacer (avec confirmation). Ouvrir les réglages en pleine partie met le jeu en pause.
- **Tutoriel** au tout premier niveau : « Move », puis « Shoot », puis « Pop them all! ». Les bulles et le temps attendent que le joueur ait marché et tiré une fois, et la commande à utiliser clignote. Il ne revient plus ensuite.

## Monétisation (version de test)

Règle d'or : **aucune pub pendant qu'on joue**. Les pubs et les achats sont **simulés** dans cette version : une fausse pub (écran « AD · TEST » de 3 à 5 s) et un faux paiement (une fenêtre de confirmation, sans argent réel). Dans l'appli Android, seules les deux fonctions `showAd` (AdMob) et `buyProduct` (Google Play Billing) seront remplacées.

- **Pièces** 🪙, gardées sur l'appareil, visibles en haut à droite hors partie :
  - 1 par petite bulle éclatée en Aventure ;
  - 20 par niveau réussi, plus 10 par étoile ;
  - 100 par boss vaincu ;
  - 1 pour 100 points en Survie ;
  - cadeau du jour : 50, plus 25 par jour d'affilée (jusqu'à 200 au 7e jour), doublable avec une pub.
- **Pubs récompensées** (le joueur choisit) :
  - **Continuer** après un échec ou un game over : +1 cœur, +15 s si le temps était écoulé, ou +1 vie en Survie. Une seule fois par partie. Un score de Survie « continué » est marqué ↻ dans le classement ;
  - **Doubler les pièces** en fin de niveau ;
  - **Bouclier gratuit** avant une partie ;
  - **Coffre gratuit** au menu, 3 par jour : des pièces ou un boost au hasard.
  - Chaque récompense existe aussi sans pub : continuer coûte 150 pièces, un boost 100 pièces.
- **Pubs imposées**, seulement en quittant l'écran de fin :
  - un niveau réussi sur trois en Aventure, une partie sur deux en Survie ;
  - jamais après un échec, jamais juste avant un boss, jamais pendant les 10 premières minutes de jeu ni avant le niveau 6 ;
  - au plus une toutes les 3 minutes, pub récompensée comprise ;
  - pas de bannière.
- **Écran avant la partie** : un seul boost au choix (Bouclier ou Double flèche), pris dans le stock, acheté 100 pièces, ou bouclier contre une pub.
- **Boutique** (bouton *Shop* du menu) :
  - **costumes** faits sur Canva, chacun en 3 poses : Red Hood (de base), Robin 800, Nomad 1 200, Sailor 1 500, Astronaut 2 000, Royal (pack de départ uniquement) ;
  - **flèches** : Fire et Ice 300, Rainbow 600 (couleur de la corde et étincelles) ;
  - **boosts** : 100 pièces l'unité ;
  - **achats** : Sans pub 2,99 €, Pack de départ 3,99 € (sans pub + 1 000 pièces + costume Royal), 500 pièces 0,99 €, 2 000 pièces 2,99 €, 6 000 pièces 6,99 €. L'achat « sans pub » retire les pubs imposées ; les pubs récompensées restent au choix.

Les réglages (gains, prix, rythme des pubs) sont regroupés en haut du bloc « Monétisation » dans `index.html` (`EARN`, `SKINS`, `TRAILS`, `PRODUCTS`, `interstitialDue`).
