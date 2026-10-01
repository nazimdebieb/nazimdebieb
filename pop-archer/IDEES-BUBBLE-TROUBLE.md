# Idées tirées de Bubble Trouble

Notes prises sur les captures envoyées (niveaux 1 à 22), puis les propositions adaptées à Pop Archer. Le but n'est pas de copier, mais de reprendre la logique et la difficulté.


## Lot 1 (niv. 1-5) — simples, pas de commentaire de l'utilisateur
- 1 petite, 1 moyenne, 1 grosse, 2 moyennes en miroir ; 5 = 2 salles (petite à gauche, plus grosse à droite), mur avec passage en bas.
- Une couleur par taille ; fonds en dégradé simple ; barre de temps rouge pleine largeur ; vies = flammes.

## Lot 2 (niv. 6-9)
- 6 : 8 mini-bulles qui rebondissent EN DÉCALÉ (pas en phase) ; le PLAFOND À SCIES DESCEND avec le temps : il faut finir avant d'être écrasé.
- 7 : couloir bas (plafond bas dès le départ), 12 mini-bulles par 3 — simple.
- 8 (selon l'utilisateur) : un grand ballon FIXE et 2 qui bougent ; capture : 3 salles petite / moyenne / grosse.
- 9 : une grosse au centre, deux moyennes sur les côtés, symétrique.

## Lot 3 (niv. 10-14)
- 10 : une bulle TRÈS grande qui se divise plusieurs fois (niveau d'une seule bulle géante).
- 11 : 8 petites bulles en ligne en haut qui rebondissent TRÈS HAUT (sol « trampoline »).
- 12 : 3 bulles à gauche + 3 à droite IMMOBILES (suspendues) jusqu'à ce qu'on les touche ; une seule plus grosse bouge.
- 13 : 6 chambres étroites (murs avec passages bas), une bulle par chambre ; toutes les bulles rebondissent un peu plus haut que la normale.
- 14 : 3 salles ; la 1re porte ne s'ouvre qu'en petit passage (bas), la 2e porte s'ouvre COMPLÈTEMENT, avec une bulle plus grosse derrière.

## Mécanique transversale
- Le plafond a toujours des lames de scie : une bulle qui touche le plafond éclate directement.
  Astuce de pro : éclater vite une grosse bulle, ses moitiés remontent ; éclater l'autre moitié avant qu'elle retombe la fait remonter jusqu'au plafond -> elle éclate seule.

## Lot 4 (niv. 15-18)
- 15 : 3 salles, grosse rouge en haut à gauche, jaune au centre (salle large, archer au centre), orange à droite ; on part du milieu.
- 16 : 10 bulles bleues moyennes en 2 rangées de 5 ; sol surélevé (plateau). QUAND ON LES ÉCLATE, LES PETITS MORCEAUX NE REBONDISSENT PAS : ILS ROULENT PAR TERRE. Impossible de passer dessous -> il faut poser une flèche collante (corde qui reste) pour qu'ils roulent dedans.
  Idée utilisateur : une NOUVELLE COULEUR DE BULLE « roulante » (ses morceaux roulent au sol au lieu de rebondir).
- 17 : arène ÉTROITE au centre (murs du château avec fenêtres sur les côtés) : 2 grosses oranges + 1 moyenne jaune serrées.
- 18 : 3 grosses bulles alignées en hauteur (verte, jaune, orange) ; décor volcanique.

## Lot 5 (niv. 19-22)
- 19 : 11 bulles moyennes rouges alignées en haut (rangée complète).
- 20 : 2 salles (grosse rouge à gauche, grosse verte à droite) ; LE MUR SE DÉCALE DOUCEMENT VERS LA GAUCHE : il réduit la 1re salle, il faut finir vite la première bulle.
- 21 : aucune bulle au départ ; ELLES TOMBENT DES MURS SUR LES CÔTÉS, une par une, rythme qui accélère, d'abord petites puis grandes.
- 22 : bulle qui se divise en 3 (on a déjà : verte).

## Bonus et armes vus (utilisateur)
- pièces qui tombent ; protection : 2 types (une qui saute au 1er contact, une qui dure ~10 s) ; horloge = temps en plus.
- armes : collante ; ONDE (tir très rapide, inefficace sur les petites, très efficace sur les grandes : les envoie vite contre le plafond à scies) ; LASER qu'on pose et qu'on commande à distance.
- armes déblocables avec l'argent du jeu ou réel.
- Consigne : s'inspirer de la logique et de la difficulté, adapter à l'archer, être créatif ; ne pas copier.

## Propositions pour Pop Archer (validées, faites)

### Nouvelles mécaniques de niveau
1. **Plafond à épines** : une bulle qui touche le plafond éclate d'un coup, sans se diviser. Récompense l'astuce « frapper tôt pour que les morceaux montent ». Propriété de niveau `spikes`.
2. **La presse** : le plafond à épines descend lentement ; s'il touche l'archer, c'est perdu. Remplace le chrono sur ces niveaux (on voit le temps qui reste).
3. **Mur qui avance** : une cloison glisse lentement et réduit une salle ; il faut finir la bulle de cette salle avant d'être coincé.
4. **Bulle endormie** (nouvelle bulle, « Zzz ») : flotte immobile jusqu'au premier tir qui la touche (ou une flèche qui la frôle), puis se réveille.
5. **Bulle de pierre** (idée du niveau 16) : ses morceaux ne rebondissent pas, ils roulent au sol ; on ne peut pas passer dessous, il faut une flèche collante posée sur leur chemin (donnée au départ de ces niveaux).
6. **Gargouilles** : la salle commence vide, des gargouilles dans les murs crachent les bulles une à une, de plus en plus vite (petites puis grosses), avec un compteur « bulles restantes ».
7. **Portes à deux étapes** : la 1re porte n'ouvre qu'un passage bas, la 2e s'efface entièrement (salle finale plus grande, bulle plus grosse).
8. Formats de niveaux à reprendre : bulle géante unique, pluie de mini-bulles décalées, couloir bas, 6 chambres étroites, rangée complète de bulles, composition symétrique.

### Bonus
- **Bouclier** (existe, saute au 1er contact) + **Aura 10 s** (nouveau : invulnérable 10 s, sauf bulle noire).
- **Sablier +10 s** (nouveau bonus qui tombe ; le boost +15 s existe déjà).

### Armes (adaptées à l'archer)
- **Flèche de vent** (l'« onde ») : part très vite ; les petites bulles la traversent sans éclater, mais elle souffle les grosses vers le haut : avec le plafond à épines, elles y éclatent d'un coup.
- **Baliste** (le « laser posé ») : on la pose au sol, elle tire seule vers le haut pendant 8 s pendant que l'archer se déplace.
- **Armurerie** dans la boutique : les nouvelles armes se débloquent avec des pièces (ou un pack), puis se choisissent comme boost avant la partie.

### Où les mettre
Une **région 2, « Château des pièges »** (mondes 14 à 18, niveaux 157 à 216), un monde par mécanique, présentée une à la fois comme les bulles spéciales :
14 Salle des épines (plafond à épines, bulle endormie), 15 La presse (plafond qui descend), 16 Remparts mouvants (murs qui avancent, portes à deux étapes), 17 Galerie des gargouilles (bulles crachées), 18 Fosse aux boulets (bulles de pierre, sol rebondissant), avec un nouveau boss : le Gardien du château.
Décors à faire sur Canva (intérieurs de château). Les armes et bonus nouveaux servent aussi dans les régions suivantes et en Survie.

## Ce qui a été fait

Tout a été validé (« pas la peine qu'on garde leur chemin ») et adapté librement :
- **Monde 14, Salle des pointes** : plafond à pointes (les morceaux ne remontent qu'un peu : il faut retoucher vite celui qui monte pour l'y crever), bulles endormies, bulle géante seule, rangée complète, niveaux en miroir. Bonus Aura et Sablier.
- **Monde 15, Couloir de la presse** : le plafond descend avec le chrono (« Écrasé ! » à zéro), pluie de mini-bulles décalées, couloir bas. Flèche du vent.
- **Monde 16, Murs mouvants** : mur qui avance, portes en deux temps, six chambres étroites. Baliste.
- **Monde 17, Galerie des gargouilles** : les bulles arrivent des murs, une à une, de plus en plus vite.
- **Monde 18, Donjon de pierre** : les bulles de pierre (déplacées ici, plus tard qu'au niveau 16 d'origine), avec la flèche collante au départ ; tous les pièges mélangés ; boss **Gardien du château**.
- **Armurerie** (onglet *Weapons* de la boutique) : flèche collante 600, flèche du vent 1 500, baliste 2 000, ou le pack armurerie à 2,99 € ; l'arme choisie part gratuitement à chaque partie.
- Le « sol rebondissant » n'a pas été gardé : les bulles rebondissantes existent déjà.
