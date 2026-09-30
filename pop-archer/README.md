# Pop Archer

Jeu d'arcade dans l'esprit de Pang / Bubble Trouble : on tire un harpon vers le haut, chaque bulle touchée se coupe en deux bulles plus petites, jusqu'à ce que les plus petites éclatent. Un fichier `index.html` et un dossier `img/`, sans dépendance.

**Pop Archer** (anciennement « Éclate-Bulles ») est en **anglais et en français** : la langue suit celle du téléphone au premier lancement (français si le téléphone est en français, anglais sinon), et se change dans les Réglages (ligne « Language / Langue »). On joue seul un petit **archer** à capuche rouge qui tire des flèches-grappins vers le haut. Chaque monde a son décor : forêt, désert en ruines, fonds marins, cosmos, pics gelés et volcan, avec des obstacles dans la matière du monde. Les bulles et les portes sont dessinées dans le code. Sont des images générées avec Canva, rangées dans `img/` (le dossier doit rester à côté de `index.html`) :

- le logo « Pop Archer » ;
- les décors des 8 mondes, 2 ambiances chacun : forêt de jour et au couchant, désert de jour et au crépuscule, fonds marins clairs et grand fond, cosmos violet et bleu nuit, pics gelés de jour et sous les aurores boréales, volcan en éruption et grotte de lave, royaume des nuages de jour et au couchant, cour et grande salle du château hanté ;
- les 8 boss : Bubble King, Great Storm, Ink Octopus, Void Eye, Frost Yeti, Lava Dragon, Sky Pirate et Lantern Phantom. Leur portrait apparaît aussi au-dessus de leur niveau sur la carte.
- l'archer (`archer-*.webp`) en 3 poses : debout, en marche et en train de tirer vers le haut. Il est dessiné tourné vers la droite et retourné en miroir quand il va à gauche ;
- les plateformes et briques de chaque monde (`plat-*.webp`, `brick-*.webp`) : rondin herbeux et caisse en bois (forêt), dalle de grès gravée et brique d'argile (désert), roche à coquillages et bloc de corail (fonds marins), plateforme métal néon et bloc de cristal (cosmos), corniche enneigée et bloc de glace (pics gelés), roche volcanique et bloc de lave (volcan), nuage doré et bloc de nuage (royaume des nuages), planche aux bougies et pierre violette (château hanté ; un halo pâle les détache des murs sombres). Les plateformes gardent leurs bouts arrondis quelle que soit leur longueur ; le milieu est répété ;
- les 12 icônes de bonus (`item-*.webp`), dans le même style : pastille arrondie brillante avec un symbole blanc. Elles servent pour les bonus qui tombent et pour les pastilles d'effet en haut à gauche.

Si une image manque, le dessin fait par le code prend le relais.

## Traduction

Le jeu existe en **5 langues** : anglais, français, espagnol, portugais du Brésil et allemand. Au premier lancement il suit la langue du téléphone (l'anglais si elle n'est pas dans la liste) ; on change dans Réglages › Langue, qui passe d'une langue à l'autre sans recharger, et le choix est gardé.

Le jeu est écrit en anglais ; dans une autre langue, chaque texte affiché passe par `tr()`. Les textes des écrans sont traduits dès qu'ils apparaissent dans la page (un `MutationObserver` garde l'anglais d'origine pour changer de langue sans recharger), ceux dessinés dans le canvas au moment du dessin.

- `tr()` cherche d'abord le texte exact dans la table `T`, qui donne pour chaque texte anglais sa traduction dans les 4 langues (français, espagnol, portugais, allemand, dans cet ordre).
- Sinon, il essaie les modèles `TX`, pour les textes avec des nombres.
- Sinon, il traduit morceau par morceau les textes de la forme « A · B » ou « A: B ».
- Les phrases avec des pluriels (objectifs des trophées) sont écrites dans chaque langue avec `L(anglais, français, espagnol, portugais, allemand)`.
- Les nombres et les prix suivent la langue (« 1 000 » en français, « 1.000 » en allemand ; « 2,99 € » partout sauf en anglais).

La typographie française (espace insécable avant « ! ? : ; ») est ajoutée automatiquement. Le logo « sans pub » de la boutique rétrécit si le mot traduit est long (« WERBUNG »).

- **Ajouter un texte** : l'écrire en anglais dans le jeu, puis ajouter une ligne dans `T` avec ses 4 traductions.
- **Ajouter une langue** : l'ajouter à `LANGS` et `LOCALE`, puis ajouter une colonne à chaque ligne de `T` et une entrée à chaque modèle de `TX` et à chaque `L()`.

La fiche Play Store existe aussi dans ces langues : `pop-archer-app/store/listing-translations.md`.

## Jouer

Ouvrir `index.html` dans un navigateur (double-clic suffit), sur ordinateur ou téléphone.

## Commandes

| | Bouger | Tirer |
|---|---|---|
| Clavier | `←` `→` ou `A` `D` (`Q` `D` en AZERTY) | `↑`, `W` (`Z` en AZERTY) ou `Espace` |

`P` ou `Échap` : pause.

Sur téléphone et tablette, le jeu prend **tout l'écran** et il n'y a plus de boutons :

- **moitié gauche** : on pose le pouce n'importe où, un joystick apparaît sous le doigt. Il ne sert qu'à aller à gauche ou à droite ; si le pouce dépasse le bord du joystick, celui-ci le suit, pour pouvoir repartir dans l'autre sens tout de suite ;
- **moitié droite** : un appui n'importe où tire une flèche (on peut tirer en marchant, avec les deux pouces).

**Bande de commandes** (réglage « Control strip », activé par défaut) : le monde se place au-dessus d'une bande de sol en bas de l'écran (environ 17 % de la hauteur). La bande prolonge la terre du décor, un peu assombrie. On y pose les pouces : le joystick à gauche, l'arc à droite, toujours affichés en transparence pendant la partie. Le pouce ne cache donc plus le jeu, même quand une bulle arrive tout contre le bord. Les deux moitiés de l'écran restent actives si on préfère toucher plus haut. Sans la bande, le jeu reprend tout l'écran, et le joystick et l'arc « fantômes » n'apparaissent qu'au début de chaque niveau et pendant le tutoriel.

Sur les écrans peu hauts, les réglages se rangent sur deux colonnes pour que les boutons restent dans l'écran.

**Plein écran, jusqu'aux bords.** Le monde fait toujours 360 unités de haut, mais sa largeur suit le format de l'écran : 640 en 16:9 (et sur ordinateur), jusqu'à 960 au maximum (avec la bande de commandes, l'aire de jeu d'un téléphone 19,5:9 fait environ 938 de large). Les murs sont donc les bords de l'écran. Les niveaux sont dessinés sur 640 de large et étirés au chargement : bulles, plateformes, briques et barrières sont placées en proportion, et le temps du niveau augmente un peu avec la largeur (+60 % de l'élargissement). La largeur ne change qu'au début d'un niveau ou d'une partie ; si l'écran change en cours de route (rotation), les côtés sont comblés en miroir jusqu'au niveau suivant. Le bandeau des scores flotte sur le haut du ciel ; la barre de vie des boss, le combo et les bonus actifs se placent juste en dessous. Les boss se déplacent sur 800 de large au plus, au centre : sur un écran plus large, le combat reste le même.

Les décors sont en 2:1 (2000×1000) : chacun est calé sur le sol du jeu, un peu recadré sur les côtés en 16:9 ou un peu agrandi (le haut du ciel déborde) sur les écrans plus larges que 2:1.

Sur téléphone, le jeu est toujours en paysage : si l'écran est tenu droit, le jeu s'affiche couché et il suffit de tourner le téléphone vers la gauche. Le bouton ⟳ retourne l'affichage pour ceux qui le tournent vers la droite.

## Modes

- **Aventure** : 96 niveaux répartis en 8 mondes de 12, à jouer un par un depuis une carte (voir plus bas). Chaque niveau réussi débloque le suivant et la progression est sauvegardée. On a 3 cœurs par niveau (une bulle touchée = un cœur en moins) ; sans cœur ou sans temps, on recommence **juste ce niveau**. Étoiles :
  - ★ niveau fini ;
  - ★★ niveau fini sans être touché ;
  - ★★★ sans être touché et avec plus de la moitié du temps restant.
- **Boss** : le 12e niveau de chaque monde est un combat de boss, avec une barre de vie. Chaque coup de harpon lui retire un point ; une fois vaincu, toutes les bulles restantes éclatent.
- **Monde 5, Frozen Peaks** (niveaux 49 à 60) : le sol est en glace, l'archer glisse et met un peu de temps à s'arrêter ou à repartir. Boss : le **Frost Yeti**, qui lance des boules de neige puis fait de grands sauts d'un côté à l'autre ; à chaque atterrissage, des stalactites tombent du plafond (signalées en rouge avant de tomber). Au sol sa fourrure arrête les flèches : on ne peut le blesser que pendant ses sauts.
- **Monde 6, Volcano** (niveaux 61 à 72) : des geysers de lave jaillissent du sol, annoncés par des bulles et des pointillés 1,3 s avant. Boss : le **Lava Dragon**, qui vole en haut de l'écran, crache des boules de feu en cloche (3 d'un coup quand il enrage) et un souffle de flammes visé sur l'archer.
- **Monde 7, Sky Kingdom** (niveaux 73 à 84) : des rafales de vent, annoncées par des traînées et des flèches « » » sur le bord de l'écran 1,2 s avant, poussent l'archer pendant 3 à 4 s (il peut marcher contre le vent, plus lentement). Boss : le **Sky Pirate**, un bateau volant qui tire des boulets en cloche (2 quand il enrage) et lâche des ancres annoncées au plafond.
- **Monde 8, Haunted Castle** (niveaux 85 à 96) : de temps en temps les lumières s'éteignent (elles clignotent d'abord) : seul un halo autour de l'archer reste éclairé, et on devine les bulles à leur contour. Boss : le **Lantern Phantom**, qui disparaît (les flèches le traversent alors), réapparaît ailleurs, glisse d'un côté à l'autre et lance des volées de feux follets ; quand il enrage, il éteint lui-même les lumières.
  - Monde 1, **Roi Bulle** : une bulle géante couronnée qui rebondit de plus en plus vite et lâche des petites bulles tous les 3 coups.
  - Monde 2, **Grand Orage** : un nuage qui fait pleuvoir des gouttes et des bulles, puis lance des éclairs (une ligne en pointillés prévient avant la frappe).
  - Monde 3, **Pieuvre Encre** : elle crache de l'encre en éventail et plonge sur toi après avoir tremblé.
  - Monde 4, **Œil du Néant** : protégé par 3 bulles en orbite qui arrêtent le harpon, il tire des lasers visés sur toi.
- **Défi du jour** (bouton violet « Daily » du menu) : chaque jour, un niveau déjà débloqué (jusqu'à 2 niveaux après le plus loin atteint, jamais un boss), tiré au sort pour la journée, avec une règle spéciale : **Gold Rush** (toutes les bulles en or), **Giants** (la plus grosse bulle grandit d'une taille), **Ghost Night** (toutes fantômes), **Iron Rain** (toutes en acier), **Split Party** (toutes vertes, une taille plus petites), **Bounce House** (toutes rebondissantes) ou **Glass Heart** (un seul cœur). La première victoire du jour rapporte 20 pièces, +5 par jour d'affilée (jusqu'au 7e jour) ; on peut le rejouer pour le plaisir. Le bouton affiche ★ tant que le défi du jour n'est pas gagné, ✓ ensuite.
- **Trophées** (bouton « Trophies » du menu, avec le nombre de récompenses à réclamer) : 12 trophées à 2 ou 3 paliers (30 en tout), chacun rapportant des pièces à réclamer : bulles éclatées, étoiles, boss vaincus, niveaux sans être touché, défis du jour gagnés et série, temps tenu en Survie, combo, bulles dorées, acier et bombes éclatées, tenues possédées. Un bandeau « Trophy unlocked » s'affiche en haut de l'écran dès qu'un palier est atteint. Au premier lancement, la progression déjà faite est comptée sans bandeau, et les récompenses correspondantes attendent dans l'écran des trophées. Les icônes sont faites sur Canva dans le style des bonus (`ach-*.webp`).
- **Survie** : un seul écran, pas de niveaux, 3 vies. Des bulles tombent par vagues (une marque jaune prévient où), de plus en plus vite et de plus en plus grosses. Éclater des bulles à moins de 2 s d'intervalle fait monter un combo (x2 dès 3 bulles, puis x3, x4, x5). Toutes les 45 s le terrain et le décor changent, et toutes les 2 minutes un boss arrive (plus résistant à chaque fois) pendant que les vagues continuent. Les 5 meilleurs scores sont gardés, avec le temps tenu.
- **Entraînement** (caché, pour le créateur du jeu) : **toucher 5 fois le logo** du menu fait apparaître un bouton vert « Training » (5 nouvelles touches le cachent). Rien n'y compte : ni étoiles, ni pièces, ni records, ni trophées, ni pubs.
  - Le panneau permet de choisir **n'importe quel niveau** (curseur, ±1 niveau, les doubles flèches pour passer d'un monde à l'autre, avec le nom du monde et du boss), **n'importe quelle règle du défi du jour**, et la façon de jouer :
    - **Partie normale** : le chrono et 3 cœurs, comme en Aventure ; on peut perdre.
    - **Infini** : on ne perd jamais, pour mesurer un niveau. Le chrono continue sous zéro et les cœurs se vident sans tuer. Le bandeau affiche le temps joué sur le temps du niveau (« 0:42 / 1:10 »).
    - Dans les deux cas, l'écran de fin donne le temps joué, sa part du temps du niveau et le nombre de touches (« Temps : 0:42 / 1:10 (60 %) · Touches : 2 »), à comparer avec la difficulté du niveau. En Infini, il dit aussi ce qui se serait passé en vrai : « Partie normale : réussi », ou perdu (temps écoulé, plus de cœurs, bulle noire), et les étoiles sont celles qu'on aurait eues.
  - « Play level » lance le niveau choisi ; « Sandbox » ouvre une arène vide dans le décor de ce niveau, sans chrono, qui ne se termine jamais.
  - En partie, une **clé** à côté de la pause ouvre la boîte à outils (le jeu est en pause pendant ce temps) : **Bulles** (lâcher des bulles de la mini au titan, normales ou spéciales : noire, dorée, acier, verte, fantôme, bombe, rebondissante), **Bonus** (prendre tout de suite ou faire tomber n'importe quel bonus ou arme, ou revenir à la flèche normale), **Boss** (faire venir n'importe quel boss, normal ou méga) et **Options** (partie normale ou infinie, n'importe quel décor avec ses pièges : vent, noir, geysers…, recommencer, tout vider).
  - Les écrans de fin proposent « Next level », « Play again » et « Training » (retour au panneau). Les réglages du panneau sont gardés, même après « Reset progress ».

Le jeu se joue à 1 joueur (le mode à 2 joueurs a été retiré : trop compliqué sur un seul téléphone).

### Régions : les mondes 9 et suivants

À partir du monde 9, les niveaux arrivent par **régions** de 5 mondes (60 niveaux). Chaque région est un fichier de données `levels/region-XX.js`, chargé avant le jeu, qui apporte ses mondes (nom, deux ambiances, conseil), ses décors, ses traductions et ses niveaux figés. Le jeu n'a rien d'autre à connaître : la carte, le nombre de niveaux et le défi du jour suivent tout seuls. Le plan complet (20 régions, 1 200 niveaux) est dans [`PLAN-1200-NIVEAUX.md`](PLAN-1200-NIVEAUX.md).

- **Région 1, Jungle Temple** (mondes 9 à 13, niveaux 97 à 156) :
  - les mondes sont Jungle Gate, Monkey Canopy, Temple Steps, Hidden Falls et Golden Sanctum ;
  - 4 nouveaux décors Canva (jungle de jour, au couchant, intérieur du temple, cascade cachée), déclinés en 10 ambiances par un étalonnage des couleurs (`filter`) : cascade de nuit, sanctuaire doré… ;
  - une pierre moussue en plateforme et une brique du temple, et une musique à elle.
- **Plateformes mobiles** : une plateforme ou une rangée de briques peut aller et venir (`ax`) ou monter et descendre (`ay`), en douceur (sinus, période `per`). Leur trajet s'étire avec la largeur de l'écran comme le reste du niveau.
- **Méga-boss** : les boss des 4 premiers mondes d'une région sont des versions renforcées des boss connus. Ils ont d'autres couleurs, 40 % de vie en plus, vont 15 % plus vite et ont 35 % de temps en plus.
- **Stone Golem**, le boss du monde 13 :
  - il flotte en haut ; ouvert, son cœur vert brille et il lance des pierres en cloche ;
  - de temps en temps il se referme dans sa carapace : il devient gris, les flèches ricochent, et il fait tomber des rochers annoncés au plafond (le premier sur l'archer) ;
  - quand il enrage, il lance deux pierres à la fois et appelle des bulles.
- **Niveaux difficiles** : l'atelier rend le 5e et le 10e niveau de chaque monde plus chargés que leurs voisins ; le chronomètre les classe en général « très difficile ».
- **Décors chargés à la demande** : un décor, une plateforme ou une brique n'est chargé qu'au moment où il sert (dans un niveau ou sur la carte). Les régions ajoutées ne ralentissent donc pas le démarrage.

**L'atelier** (`tools/atelier.mjs`) fabrique une région à partir de sa **recette** (`tools/regions/region-XX.mjs` : mondes, décors, textes, boss, difficulté de départ) :

1. chaque niveau est tiré au hasard mais toujours pareil pour une même graine, selon une difficulté en dents de scie dans chaque monde (plus dure au 5e et au 10e niveau) qui monte de monde en monde ; la mécanique de la région est dans plus de la moitié des niveaux, et jamais deux fois de suite la même disposition ;
2. un robot invincible joue chaque niveau dans le vrai jeu (Chromium sans fenêtre, en accéléré) ;
3. un niveau est gardé si le robot le finit en utilisant au plus 60 % du temps de la formule (75 % pour un boss), pour laisser de la marge à un joueur qui doit esquiver ; sinon l'atelier en tire un autre, un peu plus facile ;
4. le résultat est figé dans `levels/region-XX.js`, avec pour chaque niveau le temps mis par le robot ; un niveau publié ne change plus.

5. enfin, le chronomètre (`tools/chrono.mjs`, ci-dessous) donne à chaque niveau de la région sa difficulté et son temps.

Commandes : `node tools/atelier.mjs 1` (fabrique la région 1) et `node tools/atelier.mjs 1 --check` (rejoue les niveaux figés). Il faut Playwright (`npm i -g playwright`).

### Le chronomètre des niveaux

`tools/chrono.mjs` donne à chaque niveau sa difficulté et son temps, et les écrit dans `levels/timing.js` (chargé par le jeu) :

1. un robot (invincible, qui ramasse aussi les pièces proches, au rythme de la boucle du jeu) joue chaque niveau **sans limite de temps** : 3 fois pour un niveau normal (on garde la médiane), 5 fois pour un boss (on garde le 4e plus court, car un boss varie beaucoup d'une partie à l'autre) ;
2. dans chaque monde, les 11 niveaux normaux sont classés du plus rapide au plus long à finir pour le robot. Les plus rapides (donc ceux avec le moins de bulles) sont **faciles**, les plus longs **très difficiles**, avec une répartition qui change d'un monde à l'autre (`DIST`) :

   | Mondes | Facile | Moyen | Difficile | Très difficile |
   |---|---|---|---|---|
   | 1 | 5 | 4 | 2 | 0 |
   | 2 | 4 | 4 | 2 | 1 |
   | 3 et 4 | 3 | 4 | 3 | 1 |
   | 5 et suivants | 2 | 4 | 3 | 2 |

3. le temps du niveau est le temps du robot multiplié par une marge pour un humain, qui doit esquiver et vise moins vite (`MARGIN`) : facile ×2,8 + 8 s, moyen ×1,6 + 4 s, difficile ×1,2 + 3 s, très difficile ×1 + 2 s (25 s au moins) ; un boss ×2,2 + 15 s, entre 60 et 240 s (il faut en plus esquiver ses tirs, et le robot invincible en bat certains très vite). Le jeu ajoute ensuite un peu de temps sur les écrans larges, et une règle du défi du jour change le temps dans la même proportion que la formule. Ces marges ont été réglées avec `--verify` (ci-dessous) ;
4. `--verify` rejoue tout avec un « joueur moyen » et compte ses réussites par difficulté. C'est un robot plus lent (il réagit 1,6 fois moins souvent, vise moins finement et fait un pas de côté quand une bulle va lui tomber dessus) : il met en moyenne 1,4 fois le temps du robot de mesure, de 0,9 à 2,6 fois selon les parties.

Commandes : `node tools/chrono.mjs --levels 1-156` (mesure et écrit), `node tools/chrono.mjs --retime` (recalcule sans rejouer, après avoir changé `DIST` ou `MARGIN`), `node tools/chrono.mjs --verify`.

**Ajouter une région** :

1. écrire `tools/regions/region-02.mjs` en s'inspirant de la région 1 ;
2. faire ses images avec Canva ;
3. si la région a un nouveau thème : ajouter ses matériaux, sa musique et sa nouvelle mécanique dans `index.html` ;
4. lancer l'atelier, puis ajouter `<script src="levels/region-02.js"></script>` à `index.html`.

### La carte de l'Aventure

La carte est un long chemin qui monte, façon jeu « saga » : on la fait défiler du doigt (ou à la molette, ou en la faisant glisser à la souris) et les niveaux se suivent en zigzag à travers les décors de chaque monde, qui se fondent l'un dans l'autre. Un bandeau marque l'entrée de chaque monde, avec son numéro et son nom.

- Les niveaux finis sont des ronds orange avec leur numéro et leurs étoiles, le niveau à jouer est rose et pulse, les niveaux verrouillés sont des ronds vides, **sans numéro**. Le chemin est rayé rose jusqu'au niveau du joueur.
- Le boss de chaque monde est un rond plus gros, bleu, avec son portrait au-dessus (gris tant qu'il n'est pas débloqué).
- Le repère du joueur (son archer, dans la tenue portée) est posé à côté de son niveau. Quand on revient sur la carte après avoir réussi un niveau, le niveau suivant apparaît et le repère y saute, la carte suit.
- Un bouton rose en bas à droite ramène à son niveau quand on s'en est éloigné.
- **On ne voit jamais la fin** : seuls le monde du joueur et le suivant sont dessinés ; au-dessus, des nuages et un « ? » (« Continue d'éclater des bulles pour découvrir la suite ! »). Une fois tous les niveaux finis, les nuages annoncent « De nouveaux mondes arrivent bientôt ! ». Le menu affiche le total d'étoiles, sans maximum.
- **Ajouter un monde** : un nom dans `WORLD_NAMES`, deux ambiances dans `SCENES` (avec leurs images `loadArt`), 12 niveaux (le 12e est un boss) et un boss. Le nombre de niveaux, la carte et ses décors suivent tout seuls.

## Règles

- **Temps et difficulté de chaque niveau** : chaque niveau a un temps mesuré en le faisant jouer, et une difficulté affichée : **facile**, **moyen**, **difficile** ou **très difficile** (voir « Le chronomètre des niveaux » plus bas).
  - Un niveau facile a peu de bulles et beaucoup de temps ; un niveau très difficile a juste assez de temps pour un joueur rapide : la plupart du temps on échoue, et un boost (double flèche, bouclier…) aide à passer.
  - Au début, surtout du facile et du moyen ; de monde en monde, de plus en plus de difficile et de très difficile, mélangés dans le monde (les niveaux montent en dents de scie).
  - Sur la carte : une pastille de couleur en haut à droite du rond, verte pour facile, jaune pour moyen, orange pour difficile ; un très difficile est un rond rouge avec une tête de mort. L'écran avant la partie montre une pastille de couleur avec le nom et 1 à 4 barres (et, en difficile ou très difficile, conseille un boost) ; le bandeau de départ dit « Très difficile · Prêt ? » ; la barre du haut « Niveau 5 · Difficile ».
  - Un niveau sans mesure (une région toute neuve) prend le temps de la formule : 18 s, plus 1,7 s par coup nécessaire (20 % de plus pour les bulles noires, fantômes et rebondissantes, 6 s par barrière) ; les boss ont chacun leur temps (35 % de plus en méga).
- **Tailles de bulles** : de la mini à la grosse, plus deux nouvelles : l'**énorme** (dès le monde 1) et le **titan** (à partir du monde 4, et en Survie). Une grosse bulle se coupe en deux à chaque coup, jusqu'à la mini qui éclate. Les morceaux partent toujours vers le haut (d'autant plus haut qu'ils sont gros), même quand la bulle retombait au moment du tir.
- **Portes** : la première fois qu'une porte s'ouvre, un message explique qu'il faut passer sous le mur.
- **Statistiques par niveau** : pour chaque niveau d'Aventure, l'appareil garde le nombre d'essais, de victoires et le meilleur temps utilisé (`lvstats`). L'Entraînement les affiche ; elles serviront à recaler les temps avec de vrais joueurs.
- **Pastilles de difficulté** : elles portent aussi un chiffre (1 facile, 2 moyen, 3 difficile) pour ceux qui distinguent mal les couleurs.
- **Vitesse** : tout le jeu (bulles, archer, flèches, boss, chrono) tourne à 88 % du temps réel (`GAME_SPEED`), pour un rythme un peu plus calme. Les temps des niveaux sont en secondes de jeu et restent justes.
- **Bulles spéciales**, chacune avec sa couleur et son signe. Un message les présente la première fois qu'elles apparaissent :

  | Bulle | Signe | Effet |
  |---|---|---|
  | **Noire** | yeux rouges, halo rouge | Si elle te touche, tu perds **tous tes cœurs** d'un coup (toutes tes vies en Survie). Le bouclier ne protège pas, seule l'étoile d'invincibilité la fait éclater. |
  | **Dorée** | étoile | Points x3, fait toujours tomber une pièce s'il en reste pour la partie, un bonus tombe une fois sur deux |
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
  - **barrières** : elles coupent l'écran en chambres. Quand toutes les bulles à gauche d'une barrière ont éclaté, une **porte** s'ouvre en bas (le mur remonte de 48) : l'archer passe dessous, mais les grosses bulles de la chambre suivante restent de leur côté ;
  - **les chambres vont de la plus facile à la plus dure** : deux chambres voisines de même largeur échangent leurs bulles si celle de gauche demande plus de coups, et si la plus grosse bulle de la dernière chambre n'est pas plus grosse que celle de la première, elle prend une taille de plus (sans devenir un titan, et pas pour une verte). Comme dans les jeux du genre : une bulle moyenne d'abord, une plus grosse derrière la porte. C'est fait au chargement (`orderRooms`) ; 33 niveaux ont changé, et ils ont été remesurés.
- 24 niveaux dessinés à la main (escaliers, étagères, damier de briques, tunnel, quatre barrières…), puis des niveaux générés de plus en plus chargés à partir de 11 dispositions d'obstacles. Le record est gardé dans le navigateur.

## Son, vibrations et tutoriel

- **Musique** jouée par le jeu lui-même (aucun fichier audio) : une boucle pour le menu, une par monde (forêt, désert, fonds marins, cosmos) et une pour les boss. Elle baisse pendant la pause et s'arrête sur l'écran de fin et quand l'appli passe en arrière-plan.
- **Vibrations** sur téléphone : petite secousse à chaque bulle éclatée, plus forte quand on est touché, une série pour la bulle noire, et une petite quand on réussit un niveau.
- **Réglages** (roue dentée en haut à droite) : bruitages, musique et vibrations séparés, et « Reset progress » pour tout effacer (avec confirmation). Ouvrir les réglages en pleine partie met le jeu en pause.
- **Tutoriel** au tout premier niveau : « Move », puis « Shoot », puis « Pop them all! ». Les bulles et le temps attendent que le joueur ait marché et tiré une fois, et la commande à utiliser clignote. Il ne revient plus ensuite.

## Monétisation (version de test)

Règle d'or : **aucune pub pendant qu'on joue**. Les pubs et les achats sont **simulés** dans cette version : une fausse pub (écran « AD · TEST » de 3 à 5 s) et un faux paiement (une fenêtre de confirmation, sans argent réel). Dans l'appli Android, seules les deux fonctions `showAd` (AdMob) et `buyProduct` (Google Play Billing) seront remplacées.

- **Pièces** 🪙, gardées sur l'appareil, visibles en haut à droite hors partie :
  - **en jouant, seulement en ramassant les pièces qui tombent des bulles éclatées** (Aventure, défi du jour, Survie) :
    - une pièce vaut 5, deux pièces collées 10, trois 15 ; elles scintillent au sol et, comme les bonus, disparaissent au bout de **5 s** (elles clignotent la dernière seconde et demie) ;
    - **environ 10 pièces par partie** : chaque niveau reçoit une réserve de 5 à 15 selon son nombre de coups (en moyenne 10 sur les 156 niveaux ; 20 pour un boss et pour une partie de Survie), découpée au hasard en pièces de 5, 10 et 15 ; en fin de niveau, une pub les double ;
    - en niveau, elles tombent au fil des bulles éclatées, étalées jusqu'à la fin (la chance suit les pièces qui restent sur les coups qui restent) ; une bulle dorée en fait toujours tomber une s'il en reste ; face à un boss, un coup au boss compte comme une bulle ; en Survie, une de temps en temps (au plus une toutes les 7 s) ;
    - les pièces encore au sol quand le niveau est réussi sont ramassées d'office ;
    - plus rien d'autre en partie : ni pièces par niveau réussi, par étoile, par boss ou pour le score de Survie ;
  - **le week-end** (samedi et dimanche), deux fois plus de pièces tombent (au plus 40 par partie) ; le menu l'annonce ;
  - **2 pièces par nouvelle étoile** en Aventure (seulement la 1re fois qu'on la gagne) et **25 pour un monde fini** (son boss battu la 1re fois) ;
  - la victoire du jour au défi du jour : 20, plus 5 par jour d'affilée (jusqu'à 50) ;
  - cadeau du jour : 5, plus 5 par jour d'affilée (jusqu'à 35 au 7e jour), doublable avec une pub ;
  - coffre gratuit : 5 à 20 pièces ou un boost ; trophées : 5 à 125 par palier.
- **Vies de l'Aventure**, façon Candy Crush : **5 au plus**. Rater un niveau ou le quitter en cours en coûte une (l'écran d'échec dit combien il en reste) ; elles reviennent seules, **une toutes les 20 min**, même jeu fermé. Sur la carte, un compteur ♥ montre les vies et le temps avant la prochaine. Sans vie, on ne peut pas lancer de niveau : on attend, on regarde une pub (+1 vie) ou on paie 30 pièces pour les 5. Continuer après un échec rend la vie perdue. Le défi du jour, la Survie et l'Entraînement ne coûtent pas de vie.
- **Pubs récompensées** (le joueur choisit) :
  - **Continuer** après un échec ou un game over : +1 cœur, +15 s si le temps était écoulé, ou +1 vie en Survie. Une seule fois par partie. Un score de Survie « continué » est marqué ↻ dans le classement ;
  - **Doubler les pièces** en fin de niveau ;
  - **Bouclier gratuit** avant une partie ;
  - **Coffre gratuit** au menu, 3 par jour : des pièces ou un boost au hasard.
  - Chaque récompense existe aussi sans pub : continuer coûte 30 pièces, un boost 25 pièces, les 5 vies 30 pièces.
- **Pubs imposées**, seulement en quittant l'écran de fin :
  - un niveau réussi sur trois en Aventure, une partie sur deux en Survie ;
  - jamais après un échec, jamais juste avant un boss, jamais pendant les 10 premières minutes de jeu ni avant le niveau 6 ;
  - au plus une toutes les 3 minutes, pub récompensée comprise ;
  - pas de bannière.
- **Écran avant la partie** : un seul boost au choix, pris dans le stock ou acheté 25 pièces (le bouclier aussi contre une pub) : **Bouclier**, **Double flèche**, **+15 s** au chrono (pas en Survie) ou **+1 cœur**. La difficulté du niveau est affichée juste au-dessus ; en difficile et très difficile, le jeu conseille un boost.
- **Boutique** (bouton *Shop* du menu) :
  - **costumes** faits sur Canva, chacun en 3 poses : Red Hood (de base), Robin 250, Nomad 400, Sailor 500, Astronaut 650, Royal (pack de départ uniquement) ;
  - **flèches** : Fire et Ice 100, Rainbow 200 (couleur de la corde et étincelles) ;
  - **boosts** : 25 pièces l'unité (les 4 boosts) ;
  - **achats** : Sans pub 2,99 €, Pack de départ 3,99 € (sans pub + 300 pièces + costume Royal), 150 pièces 0,99 €, 600 pièces 2,99 €, 1 800 pièces 6,99 €. L'achat « sans pub » retire les pubs imposées ; les pubs récompensées restent au choix.

Les réglages (gains, prix, rythme des pubs) sont regroupés en haut du bloc « Monétisation » dans `index.html` (`EARN`, `SKINS`, `TRAILS`, `PRODUCTS`, `interstitialDue`).
