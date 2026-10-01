// Pop Archer, région 2 : Trap Castle (niveaux 157 à 216).
// Fichier fabriqué par tools/atelier.mjs à partir de tools/regions/region-02.mjs : ne pas le modifier à la main.
// Vérifié : 59/60 niveaux finis par le robot dans la marge de temps voulue.
(window.POP_REGIONS = window.POP_REGIONS || []).push({
  name: "Trap Castle",
  art: {"castleHall":["img/castle-hall.webp",0.856],"castleDungeon":["img/castle-dungeon.webp",0.873],"castleArmory":["img/castle-armory.webp",0.885],"castleThrone":["img/castle-throne.webp",0.855]},
  i18n: {
    "Trap Castle": ["Château des pièges","Castillo de las Trampas","Castelo das Armadilhas","Fallenburg"],
    "Spike Hall": ["Salle des pointes","Sala de los Pinchos","Salão dos Espinhos","Stachelhalle"],
    "Crushing Corridor": ["Couloir de la presse","Pasillo Aplastante","Corredor Esmagador","Quetschgang"],
    "Moving Walls": ["Murs mouvants","Muros Móviles","Paredes Móveis","Wandernde Wände"],
    "Gargoyle Gallery": ["Galerie des gargouilles","Galería de Gárgolas","Galeria das Gárgulas","Wasserspeier-Galerie"],
    "Stone Keep": ["Donjon de pierre","Torreón de Piedra","Torre de Pedra","Steinfried"],
    "Spike Hall: bubbles that touch the spiked ceiling burst at once!": ["Salle des pointes : les bulles qui touchent le plafond à pointes éclatent d’un coup !","Sala de los Pinchos: ¡las burbujas que tocan el techo de pinchos revientan de golpe!","Salão dos Espinhos: as bolhas que tocam o teto de espinhos estouram de uma vez!","Stachelhalle: Blasen, die die Stacheldecke berühren, platzen auf einmal!"],
    "Crushing Corridor: the ceiling comes down as time runs out!": ["Couloir de la presse : le plafond descend à mesure que le temps passe !","Pasillo Aplastante: ¡el techo baja a medida que se acaba el tiempo!","Corredor Esmagador: o teto desce conforme o tempo acaba!","Quetschgang: Die Decke senkt sich, während die Zeit abläuft!"],
    "Moving Walls: some walls slide, some doors open wide!": ["Murs mouvants : certains murs avancent, certaines portes s’ouvrent en grand !","Muros Móviles: ¡algunos muros se deslizan y algunas puertas se abren del todo!","Paredes Móveis: algumas paredes deslizam, algumas portas se abrem por inteiro!","Wandernde Wände: Manche Wände rücken vor, manche Türen öffnen sich ganz!"],
    "Gargoyle Gallery: stone heads spit bubbles from the walls!": ["Galerie des gargouilles : des têtes de pierre crachent des bulles depuis les murs !","Galería de Gárgolas: ¡cabezas de piedra escupen burbujas desde los muros!","Galeria das Gárgulas: cabeças de pedra cospem bolhas das paredes!","Wasserspeier-Galerie: Steinköpfe spucken Blasen aus den Wänden!"],
    "Stone Keep: stone bubbles roll on the floor. You get the sticky arrow!": ["Donjon de pierre : les bulles de pierre roulent au sol. Tu reçois la flèche collante !","Torreón de Piedra: las burbujas de piedra ruedan por el suelo. ¡Recibes la flecha pegajosa!","Torre de Pedra: as bolhas de pedra rolam no chão. Você ganha a flecha grudenta!","Steinfried: Steinblasen rollen über den Boden. Du bekommst den Klebepfeil!"],
  },
  worlds: [
    {"name":"Spike Hall","tip":"Spike Hall: bubbles that touch the spiked ceiling burst at once!","scenes":[{"theme":"castle","sky":["#2e3a56","#5a6a88"],"far":"#4a5a78","ground":"#8a94a4","banner":"#c8323a","ball":"#ff4f7a","art":"castleHall"},{"theme":"castle","sky":["#5a3a1e","#c8a070"],"far":"#8a6a4a","ground":"#a8743a","banner":"#2a5aa8","ball":"#3bd1ff","art":"castleArmory"}]},
    {"name":"Crushing Corridor","tip":"Crushing Corridor: the ceiling comes down as time runs out!","scenes":[{"theme":"castle","sky":["#14241e","#3a5a48"],"far":"#2f4a3e","ground":"#5a6a4a","banner":"#3a6a8a","ball":"#ffd23b","art":"castleDungeon"},{"theme":"castle","sky":["#2e3a56","#5a6a88"],"far":"#4a5a78","ground":"#8a94a4","banner":"#c8323a","ball":"#9dff5a","art":"castleHall","filter":"hue-rotate(150deg) saturate(.8) brightness(.85)"}]},
    {"name":"Moving Walls","tip":"Moving Walls: some walls slide, some doors open wide!","scenes":[{"theme":"castle","sky":["#5a3a1e","#c8a070"],"far":"#8a6a4a","ground":"#a8743a","banner":"#2a5aa8","ball":"#b45cff","art":"castleArmory","filter":"hue-rotate(-12deg) saturate(1.15)"},{"theme":"castle","sky":["#14241e","#3a5a48"],"far":"#2f4a3e","ground":"#5a6a4a","banner":"#3a6a8a","ball":"#ff8a2e","art":"castleDungeon","filter":"hue-rotate(40deg) saturate(.9)"}]},
    {"name":"Gargoyle Gallery","tip":"Gargoyle Gallery: stone heads spit bubbles from the walls!","scenes":[{"theme":"castle","sky":["#0e1630","#2a3a5e"],"far":"#4a5a78","ground":"#8a94a4","banner":"#c8323a","ball":"#ffd23b","art":"castleHall","filter":"brightness(.62) saturate(.8) contrast(1.1)"},{"theme":"castle","sky":["#5a3a1e","#c8a070"],"far":"#8a6a4a","ground":"#a8743a","banner":"#2a5aa8","ball":"#ff4fa3","art":"castleArmory","filter":"sepia(.35) brightness(.9)"}]},
    {"name":"Stone Keep","tip":"Stone Keep: stone bubbles roll on the floor. You get the sticky arrow!","scenes":[{"theme":"castle","sky":["#8a7a9a","#e8dcc8"],"far":"#c8bca8","ground":"#b8303a","banner":"#6a3a9a","ball":"#3bd1ff","art":"castleThrone"},{"theme":"castle","sky":["#8a7a9a","#e8dcc8"],"far":"#c8bca8","ground":"#b8303a","banner":"#6a3a9a","ball":"#ffd23b","art":"castleThrone","filter":"hue-rotate(200deg) saturate(.75) brightness(.8)"}]},
  ],
  levels: [
    {"b":[[3,0.2,1],[2,0.8,-1]],"spikes":1}, // 157 · intro d=0.42 · robot 21 s / 55 s
    {"b":[[4,0.8,1]],"spikes":1}, // 158 · giant d=0.44 · robot 24 s / 71 s
    {"b":[[1,0.08,1,120],[1,0.25,-1,120],[1,0.42,1,120],[1,0.58,-1,120],[1,0.75,1,120],[1,0.92,-1,120]],"k":[["s",220,200,200,14]],"spikes":1}, // 159 · row d=0.46 · robot 13 s / 49 s
    {"b":[[3,0.1,1,null,"bouncy"],[3,0.9,-1,null,"bouncy"],[2,0.24,1],[2,0.76,-1]],"k":[["s",260,190,120,14]],"spikes":1}, // 160 · mirror d=0.41 · robot 17 s / 103 s
    {"b":[[2,0.08,1,110,"sleep"],[2,0.92,-1,110,"sleep"],[2,0.18,1,140,"sleep"],[2,0.82,-1,140,"sleep"],[2,0.28,1,170,"sleep"],[2,0.72,-1,170,"sleep"],[4,0.5,-1]],"spikes":1,"hard":1}, // 161 · difficile · sleepers d=0.61 · robot 40 s / 142 s
    {"b":[[4,0.08,1,null,"steel"],[1,0.28,-1],[1,0.49,1,null,"gold"],[2,0.72,-1,null,"ghost"],[2,0.9,1]],"k":[["g",213],["g",427]],"spikes":1}, // 162 · gates2 d=0.53 · robot 24 s / 121 s
    {"b":[[1,0.08,1,120],[1,0.22,-1,120],[1,0.36,1,120],[1,0.5,-1,120],[1,0.64,1,120],[1,0.78,-1,120],[1,0.92,1,120]],"k":[["b",160,190,8]],"spikes":1}, // 163 · row d=0.55 · robot 18 s / 59 s
    {"b":[[3,0.1,1,null,"bouncy"],[3,0.9,-1,null,"bouncy"],[2,0.24,1],[2,0.76,-1]],"k":[["s",260,190,120,14]],"spikes":1}, // 164 · mirror d=0.49 · robot 32 s / 103 s
    {"b":[[3,0.1,1,null,"bomb"],[2,0.38,-1],[2,0.64,1],[2,0.9,-1]],"k":[["g",320]],"spikes":1}, // 165 · gate d=0.6 · robot 26 s / 85 s
    {"b":[[4,0.12,1,null,"bomb"],[2,0.27,-1],[2,0.4,1],[2,0.69,-1,null,"bouncy"],[2,0.91,1]],"k":[["b",160,190,8]],"spikes":1,"hard":1}, // 166 · difficile · spikes d=0.72 · robot 18 s / 125 s
    {"b":[[3,0.1,1,null,"steel"],[3,0.9,-1,null,"steel"],[2,0.24,1,null,"gold"],[2,0.76,-1,null,"gold"],[1,0.38,1],[1,0.62,-1]],"k":[["s",260,190,120,14]],"spikes":1}, // 167 · mirror d=0.64 · robot 30 s / 106 s
    {"boss":"yeti","mega":1}, // 168 · boss ·  · robot 49 s / 162 s
    {"b":[[4,0.12,1],[2,0.35,-1],[2,0.61,1],[2,0.86,-1,null,"ghost"]],"k":[["g",320]],"press":1}, // 169 · press d=0.49 · robot 32 s / 115 s
    {"b":[[0,0.08,1,70],[0,0.16,-1,123],[0,0.24,1,176],[0,0.32,-1,79],[0,0.4,1,132],[0,0.68,-1,185],[0,0.76,1,88],[0,0.84,-1,141],[0,0.92,1,194]],"press":1}, // 170 · rain d=0.51 · robot 7 s / 33 s
    {"b":[[4,0.12,1,null,"gold"],[2,0.39,-1],[2,0.62,1],[2,0.9,-1,null,"ghost"]],"spikes":1}, // 171 · spikes d=0.53 · robot 61 s / 109 s
    {"b":[[3,0.15,1],[1,0.35,-1],[2,0.63,1],[1,0.9,-1,null,"green"]],"k":[["g",320],["s",70,205,120,14],["s",450,205,120,14]]}, // 172 · pads d=0.48 · robot 29 s / 73 s
    {"b":[[4,0.1,1],[3,0.35,-1],[2,0.6,1],[2,0.89,-1,null,"bouncy"]],"k":[["s",0,170,260,14],["s",380,170,260,14]],"hard":1}, // 173 · difficile · shelves d=0.68 · robot 41 s / 122 s
    {"b":[[1,0.08,1,70],[0,0.15,-1,123],[0,0.22,1,176],[1,0.29,-1,79],[0,0.36,1,132],[0,0.64,-1,185],[1,0.71,1,88],[0,0.78,-1,141],[0,0.85,1,194],[1,0.92,-1,97]],"press":1}, // 174 · rain d=0.6 · robot 14 s / 49 s
    {"b":[[3,0.13,1,262,"gold"],[1,0.35,-1,262],[2,0.6,1,262,"sleep"],[2,0.86,-1,262]],"k":[["s",0,200,640,14]]}, // 175 · corridor d=0.62 · robot 28 s / 72 s
    {"b":[[1,0.08,1,110,"sleep"],[1,0.92,-1,110,"sleep"],[1,0.18,1,140,"sleep"],[1,0.82,-1,140,"sleep"],[1,0.28,1,170,"sleep"],[1,0.72,-1,170,"sleep"],[4,0.5,1]],"spikes":1}, // 176 · sleepers d=0.56 · robot 28 s / 101 s
    {"b":[[4,0.13,1],[2,0.38,-1,null,"bouncy"],[2,0.65,1,null,"gold"],[3,0.9,-1]],"k":[["g",320]],"press":1}, // 177 · press d=0.67 · robot 28 s / 128 s
    {"b":[[1,0.07,1,70],[0,0.14,-1,123],[0,0.2,1,176],[1,0.27,-1,79],[0,0.33,1,132],[0,0.4,-1,185],[1,0.67,1,88],[0,0.73,-1,141],[0,0.8,1,194],[1,0.86,-1,97],[0,0.93,1,150]],"press":1,"hard":1}, // 178 · difficile · rain d=0.79 · robot 12 s / 50 s
    {"b":[[5,0.6,1]],"k":[["b",120,230,10]],"press":1}, // 179 · press d=0.71 · robot 2 s / 131 s
    {"boss":"dragon","mega":1}, // 180 · boss ·  · robot 94 s / 176 s
    {"b":[[4,0.12,1],[1,0.44,-1,null,"green"],[2,0.6,1],[2,0.87,-1]],"k":[["g",434,281,8.17]]}, // 181 · slide d=0.56 · robot 31 s / 121 s
    {"b":[[4,0.07,1,null,"bomb"],[1,0.28,-1,null,"ghost"],[2,0.4,1,null,"sleep"],[2,0.71,-1],[2,0.89,1]],"press":1}, // 182 · press d=0.58 · robot 39 s / 113 s
    {"b":[[5,0.72,-1]],"spikes":1}, // 183 · giant d=0.6 · robot 1 s / 125 s
    {"b":[[4,0.13,1,60],[1,0.38,-1,60,"steel"],[1,0.65,1,60],[2,0.89,-1,60,"green"]],"k":[["s",0,210,40,16],["b",40,210,1],["b",80,210,1],["b",120,210,1],["b",160,210,1],["s",200,210,40,16],["b",240,210,1],["b",280,210,1],["b",320,210,1],["b",360,210,1],["s",400,210,40,16],["b",440,210,1],["b",480,210,1],["b",520,210,1],["b",560,210,1],["s",600,210,40,16]]}, // 184 · brickFloor d=0.51 · robot 34 s / 112 s
    {"b":[[4,0.11,1,262],[2,0.4,-1,262],[3,0.62,1,262,"ghost"],[2,0.9,-1,262,"steel"]],"k":[["s",0,200,640,14]],"hard":1}, // 185 · difficile · ceiling d=0.75 · robot 27 s / 127 s
    {"b":[[1,0.08,1],[1,0.25,-1,null,"bouncy"],[1,0.42,1,null,"bouncy"],[2,0.58,-1],[2,0.75,1],[2,0.92,-1]],"k":[["g",107],["g",213],["g",320],["g",427],["g",533]]}, // 186 · chambers d=0.67 · robot 32 s / 101 s
    {"b":[[4,0.11,1,null,"bomb"],[2,0.45,-1,null,"bomb"],[2,0.64,1,null,"steel"],[2,0.86,-1,null,"bomb"]],"k":[["g",448,281,7.7]]}, // 187 · slide d=0.69 · robot 27 s / 128 s
    {"b":[[2,0.16,1],[2,0.5,-1],[4,0.8,-1]],"k":[["g",213],["g",427,0,0,1]]}, // 188 · doors d=0.63 · robot 37 s / 107 s
    {"b":[[1,0.08,1],[1,0.25,-1],[1,0.42,1,null,"bouncy"],[2,0.58,-1,null,"bouncy"],[2,0.75,1],[2,0.92,-1,null,"bouncy"]],"k":[["g",107],["g",213],["g",320],["g",427],["g",533]]}, // 189 · chambers d=0.74 · robot 15 s / 105 s
    {"b":[[4,0.1,1,null,"gold"],[3,0.35,-1],[2,0.6,1,null,"bouncy"],[3,0.87,-1]],"hard":1}, // 190 · difficile · none d=0.78 · robot 38 s / 136 s
    {"b":[[2,0.16,1],[2,0.5,-1],[4,0.8,-1]],"k":[["g",213],["g",427,0,0,1]]}, // 191 · doors d=0.78 · robot 44 s / 107 s
    {"boss":"pirate","mega":1}, // 192 · boss ·  · robot 67 s / 189 s
    {"b":[],"k":[["s",0,170,200,14],["s",440,170,200,14]],"spawn":[[0],[0],[0],[1],[1],[2]],"every":2.92}, // 193 · gargoyles d=0.63 · robot 33 s / 56 s
    {"b":[[3,0.13,1],[1,0.28,-1],[1,0.4,1,null,"steel"],[1,0.72,-1,null,"bouncy"],[1,0.93,1]],"k":[["b",160,190,8]],"spikes":1}, // 194 · spikes d=0.65 · robot 27 s / 71 s
    {"b":[[1,0.08,1,70],[0,0.15,-1,123],[0,0.22,1,176],[1,0.29,-1,79],[0,0.36,1,132],[0,0.64,-1,185],[1,0.71,1,88],[0,0.78,-1,141],[0,0.85,1,194],[1,0.92,-1,97]],"press":1}, // 195 · rain d=0.63 · robot 13 s / 49 s
    {"b":[[3,0.1,1,null,"sleep"],[1,0.25,-1,null,"bomb"],[1,0.5,1],[1,0.71,-1],[2,0.88,1]],"k":[["g",442,252,8.03]]}, // 196 · slide d=0.62 · robot 55 s / 97 s
    {"b":[[1,0.07,1,70],[0,0.14,-1,123],[0,0.2,1,176],[1,0.27,-1,79],[0,0.33,1,132],[0,0.4,-1,185],[1,0.67,1,88],[0,0.73,-1,141],[0,0.8,1,194],[1,0.86,-1,97],[0,0.93,1,150]],"press":1,"hard":1}, // 197 · difficile · rain d=0.82 · robot 12 s / 50 s
    {"b":[[2,0.5,1]],"k":[["s",0,170,200,14],["s",440,170,200,14]],"spawn":[[0],[0],[0],[1],[1],[1],[2],[2]],"every":2.51}, // 198 · gargoyles d=0.74 · robot 17 s / 86 s
    {"b":[[5,0.23,1,null,"bouncy"],[1,0.78,-1]],"k":[["g",454,294,9.17]]}, // 199 · slide d=0.76 · robot 46 s / 164 s
    {"b":[[4,0.12,1,null,"bomb"],[2,0.35,-1],[2,0.47,1,null,"green"],[2,0.68,-1],[2,0.88,1,null,"gold"]],"k":[["g",320],["s",70,205,120,14],["s",450,205,120,14]]}, // 200 · pads d=0.7 · robot 48 s / 135 s
    {"b":[[2,0.5,1]],"spawn":[[0],[0],[1],[1],[1],[1],[2],[2],[3]],"every":2.43}, // 201 · gargoyles d=0.81 · robot 35 s / 116 s
    {"b":[[4,0.1,1],[4,0.9,-1],[2,0.24,1,null,"bouncy"],[2,0.76,-1,null,"bouncy"],[1,0.38,1],[1,0.62,-1]],"k":[["s",260,190,120,14]],"hard":1}, // 202 · difficile · mirror d=0.89 · robot 57 s / 162 s
    {"b":[[2,0.5,1]],"spawn":[[0],[0],[0],[1],[1],[1],[2],[2],[3]],"every":2.38}, // 203 · gargoyles d=0.85 · robot 39 s / 112 s
    {"boss":"phantom","mega":1}, // 204 · boss ·  · robot 64 s / 203 s
    {"b":[[2,0.32,1,null,"stone"]],"k":[["s",220,200,200,14]],"weapon":"sticky"}, // 205 · stone d=0.7 · robot 11 s / 32 s
    {"b":[[1,0.08,1,null,"bouncy"],[1,0.25,-1],[1,0.42,1,null,"bouncy"],[2,0.58,-1],[2,0.75,1,null,"bouncy"],[2,0.92,-1]],"k":[["g",107],["g",213],["g",320],["g",427],["g",533]]}, // 206 · chambers d=0.72 · robot 22 s / 103 s
    {"b":[[4,0.09,1],[2,0.29,-1],[2,0.4,1,null,"ghost"],[3,0.71,-1],[2,0.93,1,null,"gold"]],"k":[["s",0,170,260,14],["s",380,170,260,14]]}, // 207 · shelves d=0.74 · robot 58 s / 137 s
    {"b":[[4,0.11,1,60,"bomb"],[2,0.27,-1,60,"black"],[2,0.6,1,60,"bouncy"],[2,0.73,-1,60],[2,0.93,1,60,"sleep"]],"k":[["s",0,210,40,16],["b",40,210,1],["b",80,210,1],["b",120,210,1],["b",160,210,1],["s",200,210,40,16],["b",240,210,1],["b",280,210,1],["b",320,210,1],["b",360,210,1],["s",400,210,40,16],["b",440,210,1],["b",480,210,1],["b",520,210,1],["b",560,210,1],["s",600,210,40,16]]}, // 208 · brickFloor d=0.69 · robot 39 s / 130 s
    {"b":[[3,0.32,1,null,"stone"],[2,0.78,-1,null,"stone"],[1,0.55,1,120]],"weapon":"sticky","hard":1}, // 209 · difficile · stone d=0.89 · robot 19 s / 68 s
    {"b":[[2,0.16,1],[2,0.5,-1],[4,0.8,-1]],"k":[["g",213],["g",427,0,0,1]]}, // 210 · doors d=0.81 · robot 48 s / 107 s
    {"b":[[5,0.28,-1]],"spikes":1}, // 211 · giant d=0.83 · robot 1 s / 125 s
    {"b":[[1,0.08,1],[1,0.25,-1],[1,0.42,1],[2,0.58,-1],[2,0.75,1],[2,0.92,-1,null,"bouncy"]],"k":[["g",107],["g",213],["g",320],["g",427],["g",533]]}, // 212 · chambers d=0.77 · robot 33 s / 101 s
    {"b":[[3,0.31,1,null,"stone"],[2,0.78,-1],[1,0.55,1,120]],"k":[["s",220,200,200,14]],"weapon":"sticky"}, // 213 · stone d=0.88 · robot 27 s / 66 s
    {"b":[[5,0.07,1,null,"steel"],[1,0.29,-1,null,"sleep"],[1,0.48,1,null,"green"],[1,0.7,-1,null,"gold"],[1,0.93,1,null,"bomb"]],"k":[["g",213],["g",427]],"hard":1}, // 214 · difficile · gates2 d=0.95 · robot 22 s / 161 s
    {"b":[[2,0.16,1],[2,0.5,-1],[4,0.8,-1]],"k":[["g",213],["g",427,0,0,1]]}, // 215 · doors d=0.92 · robot 40 s / 107 s
    {"boss":"guardian"}, // 216 · boss ·  · robot 160 s / 160 s
  ],
});
