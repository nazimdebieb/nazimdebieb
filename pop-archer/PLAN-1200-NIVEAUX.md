# Plan : 1 200 niveaux de plus

Aujourd'hui : 96 niveaux, 8 mondes de 12. Objectif : **1 200 niveaux de plus** (niveaux 97 à 1 296), soit **100 nouveaux mondes**, publiés au fil des mises à jour. Grâce à la carte « saga », le joueur ne voit jamais la fin : seuls son monde et le suivant sont dessinés.

## 1. Découpage : 20 régions de 5 mondes

Faire 100 mondes entièrement différents coûterait trop cher en images et en temps. On regroupe donc les mondes par **région** :

- **1 région = 5 mondes = 60 niveaux**, avec son univers visuel, sa musique et **une nouvelle mécanique** ;
- la mécanique est présentée dans le 1er monde de la région, puis combinée avec les précédentes ;
- chaque monde de la région a sa propre ambiance (jour, couchant, nuit, orage, variante spéciale), obtenue avec les décors de la région et un étalonnage des couleurs dans le code, plutôt qu'avec 5 fois plus d'images ;
- **boss** : le 5e monde de chaque région a un **nouveau boss** (20 nouveaux boss au total). Les 4 autres mondes ont des **Méga-boss**, des versions renforcées des boss déjà connus (autre couleur, plus de vie, plus rapides, une attaque en plus).

| # | Région | Mondes | Niveaux | Nouvelle mécanique | Nouveau boss |
|---|---|---|---|---|---|
| 1 | Jungle Temple (temple de la jungle) | 9–13 | 97–156 | plateformes mobiles (va-et-vient, ascenseurs) | Stone Golem |
| 2 | Candy Kingdom (royaume des bonbons) | 14–18 | 157–216 | ressorts et champignons rebondissants qui renvoient les bulles | Gummy Giant |
| 3 | Crystal Caves (grottes de cristal) | 19–23 | 217–276 | portails : une bulle entre d'un côté, ressort de l'autre | Crystal Spider |
| 4 | Clockwork Factory (usine mécanique) | 24–28 | 277–336 | tapis roulants au sol et interrupteurs à toucher d'une flèche | Robo-Tank |
| 5 | Mushroom Marsh (marais aux champignons) | 29–33 | 337–396 | l'eau monte : dans l'eau, bulles et archer ralentissent | Frog King |
| 6 | Moon Base (base lunaire) | 34–38 | 397–456 | gravité faible : bulles lentes qui rebondissent très haut | UFO Commander |
| 7 | Pirate Cove (crique des pirates) | 39–43 | 457–516 | canons à bulles sur les côtés (générateurs à détruire) | Captain Skull |
| 8 | Samurai Garden (jardin samouraï) | 44–48 | 517–576 | bulles qui grossissent si on les laisse tranquilles | Oni Mask |
| 9 | Toy Box (coffre à jouets) | 49–53 | 577–636 | nouvelle bulle « essaim » : se coupe en 4 mini-bulles | Jack-in-the-Box |
| 10 | Pharaoh's Pyramid (pyramide) | 54–58 | 637–696 | sables mouvants qui ralentissent l'archer, momies-bulles qui reviennent une fois | Mummy Pharaoh |
| 11 | Coral Reef (récif de corail) | 59–63 | 697–756 | courants marins qui poussent les bulles sur le côté | Giant Jellyfish |
| 12 | Storm Peaks (pics de l'orage) | 64–68 | 757–816 | éclairs annoncés qui frappent le sol | Thunderbird |
| 13 | Neon City (ville néon) | 69–73 | 817–876 | bulles caméléon : on ne peut les toucher que dans leur couleur « ouverte » | DJ Robot |
| 14 | Dino Valley (vallée des dinos) | 74–78 | 877–936 | tremblements de terre et chutes de rochers | T-Rex |
| 15 | Winter Village (village d'hiver) | 79–83 | 937–996 | boules de neige qui grossissent en roulant au sol | Evil Snowman |
| 16 | Wizard Tower (tour du sorcier) | 84–88 | 997–1 056 | runes à activer qui ouvrent des portes ou font apparaître des plateformes | Dark Wizard |
| 17 | Cloud Circus (cirque des nuages) | 89–93 | 1 057–1 116 | trampolines et cibles mobiles | Circus Clown |
| 18 | Bee Meadow (prairie des abeilles) | 94–98 | 1 117–1 176 | ruches qui relâchent des bulles tant qu'on ne les a pas cassées | Queen Bee |
| 19 | Shadow Realm (royaume des ombres) | 99–103 | 1 177–1 236 | brouillard en bandes et bulles miroirs | Shadow Archer (le double de l'archer) |
| 20 | Bubble Galaxy (galaxie des bulles) | 104–108 | 1 237–1 296 | toutes les mécaniques mélangées | Bubble Emperor (boss final… pour l'instant) |

## 2. Plus de variété : les objectifs de niveau

1 200 fois « éclate toutes les bulles », c'est trop répétitif. On ajoute des **objectifs** (comme les objectifs de Candy Crush), affichés sur l'écran avant la partie :

- **Nettoyage** (le classique) : tout éclater avant la fin du temps ;
- **Survie** : tenir X secondes pendant que des bulles tombent ;
- **Cible** : éclater N bulles d'un type (dorées, acier…) ; les autres comptent pour le score ;
- **Sauvetage** : des oiseaux sont prisonniers de bulles, il faut les libérer avant qu'elles ne touchent le sol ;
- **Flèches comptées** : finir le niveau avec un nombre limité de flèches ;
- **Course** : un temps très court, mais un bonus au départ.

Répartition par monde (12 niveaux) : environ 7 Nettoyages, 1 à 2 Cibles, 1 Survie, 1 Sauvetage ou Flèches comptées, puis le boss. Les objectifs arrivent petit à petit : Cible à partir de la région 1, Survie à partir de la région 2, Sauvetage à partir de la région 3, Flèches comptées à partir de la région 4.

## 3. Courbe de difficulté

- **En dents de scie** dans chaque monde : ça monte, puis ça redescend un peu, pour souffler avant de remonter ;
- **niveaux « difficiles »** (le 5e et le 10e de chaque monde) : un rond rouge avec une tête de mort sur la carte, un peu plus de pièces à la clé ;
- d'une région à l'autre, la difficulté de base monte doucement : plus de bulles, plus grosses, plus de bulles spéciales, des mécaniques combinées ;
- le premier monde de chaque région est plus facile, le temps de découvrir la nouvelle mécanique.

## 4. Fabrication des niveaux : l'atelier

Écrire 1 200 niveaux à la main n'est pas réaliste. On fabrique un **atelier** (un script, hors du jeu) qui produit les niveaux, les **teste**, et les **fige** dans des fichiers de données :

1. **Recette de région** : mécaniques, bulles et objectifs permis, dispositions d'obstacles, courbe de difficulté ;
2. **Génération** : pour chaque niveau, l'atelier propose plusieurs candidats (bulles, taille, position, obstacles, objectif) ;
3. **Test automatique** : un robot joue chaque candidat dans le vrai jeu (sans navigateur visible), mesure le temps qu'il lui faut, vérifie qu'aucun piège n'est injuste (une bulle qui tombe sur le joueur au départ, un niveau trop long…) ;
4. **Choix** : on garde le candidat qui colle le mieux à la difficulté voulue, sans répéter la même disposition deux fois de suite ;
5. **Figé** : le niveau est écrit dans `levels/region-01.js`, etc. Un niveau publié ne change plus, pour que les étoiles des joueurs restent valables ;
6. **Niveaux « signature »** : 1 à 2 niveaux par monde dessinés à la main (la présentation de chaque mécanique, les niveaux marquants).

Temps de fabrication estimé : une région de 60 niveaux en une séance de travail, avec ses images Canva et ses tests.

## 5. Images, musique et textes par région

| Élément | Nombre par région | Outil |
|---|---|---|
| Décors 2:1 (2000×1000) | 2 à 4 | Canva |
| Plateforme et brique | 1 + 1 | Canva |
| Nouveau boss (fond transparent) | 1 | Canva |
| Sprites de la mécanique (ressort, portail, tapis…) | 0 à 3 | Canva ou dessin dans le code |
| Méga-boss | 4 | recoloration dans le code |
| Musique | 1 air + variations | synthèse dans le code (comme aujourd'hui) |
| Noms des 5 mondes, du boss, conseils | ~15 textes | en 5 langues |

Au total : environ 150 images pour les 100 mondes. Chaque région pèse environ 1 Mo, donc 20 Mo de plus dans l'application.

## 6. Ce qu'il faut adapter dans le jeu

- **Niveaux en fichiers de données** (`levels/region-XX.js`) au lieu d'être écrits dans `index.html` ;
- **Décors chargés à la demande** : seules les images de la région en cours (et de la suivante) sont chargées ;
- **Carte** : déjà prête (on ne dessine que 2 mondes), elle affiche aussi la marque des niveaux difficiles ;
- **Objectifs** : l'écran avant la partie, le bandeau en jeu et l'écran de résultat les affichent ;
- **Trophées** : de nouveaux paliers (500, 1 000 étoiles ; 20, 50 boss…) ;
- **Défi du jour** : il puise déjà dans tous les niveaux débloqués ;
- **Statistiques** (Firebase, à brancher) : taux de réussite par niveau, pour corriger les niveaux trop durs après leur sortie.

## 7. Calendrier proposé

- **Sortie du jeu** : les 8 mondes actuels et la **région 1** (156 niveaux) ;
- ensuite **une région toutes les 2 semaines** (60 niveaux). C'est un rythme courant pour ce genre de jeu, qui donne aux joueurs une raison de revenir ;
- les 20 régions seraient toutes sorties environ **10 mois** après la sortie du jeu ;
- on peut accélérer au début (2 régions pour la sortie) et ralentir ensuite, selon les statistiques.

## 8. Avancement

- [x] Carte « saga » qui cache la fin
- [x] Nombre de niveaux calculé à partir de la liste des mondes
- [ ] Atelier de niveaux (génération, test automatique, niveaux figés)
- [ ] Région 1 : Jungle Temple (mondes 9 à 13, niveaux 97 à 156)
- [ ] Objectifs de niveau
- [ ] Régions 2 à 20
