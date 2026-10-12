// Pop Archer, région 1 : Jungle Temple (niveaux 97 à 156).
// Fichier fabriqué par tools/atelier.mjs à partir de tools/regions/region-01.mjs : ne pas le modifier à la main.
// Vérifié : 60/60 niveaux finis par le robot dans la marge de temps voulue.
(window.POP_REGIONS = window.POP_REGIONS || []).push({
  name: "Jungle Temple",
  art: {"jungleDay":["img/jungle-day.webp",0.876],"jungleDusk":["img/jungle-dusk.webp",0.9],"jungleTemple":["img/jungle-temple.webp",0.853],"jungleFalls":["img/jungle-falls.webp",0.918]},
  i18n: {
    "Jungle Gate": ["Porte de la jungle","Puerta de la Selva","Portão da Selva","Dschungeltor"],
    "Monkey Canopy": ["Canopée des singes","Copas de los Monos","Copa dos Macacos","Affenwipfel"],
    "Temple Steps": ["Marches du temple","Escalinata del Templo","Degraus do Templo","Tempelstufen"],
    "Hidden Falls": ["Chutes cachées","Cascada Escondida","Cachoeira Escondida","Verborgene Fälle"],
    "Golden Sanctum": ["Sanctuaire doré","Santuario Dorado","Santuário Dourado","Goldenes Heiligtum"],
    "Jungle Gate: some platforms move!": ["Porte de la jungle : certaines plateformes bougent !","Puerta de la Selva: ¡algunas plataformas se mueven!","Portão da Selva: algumas plataformas se movem!","Dschungeltor: Manche Plattformen bewegen sich!"],
  },
  worlds: [
    {"name":"Jungle Gate","tip":"Jungle Gate: some platforms move!","scenes":[{"theme":"jungle","sky":["#63c4ff","#d8f4e0"],"sun":"#fffbe0","far":"#8fcf8a","trees2":"#5fb86a","trees":"#2f8a4c","trunk":"#5b3a22","ground":"#6b5a3a","grass":"#5cc254","ball":"#ff4f7a","art":"jungleDay"},{"theme":"jungle","sky":["#ff9a6b","#ffe0b0"],"sun":"#fff1c9","far":"#c98a6a","trees2":"#7a8a4a","trees":"#3f6a34","trunk":"#4b2e1a","ground":"#5b4430","grass":"#8cb84b","ball":"#4f7aff","art":"jungleDusk"}]},
    {"name":"Monkey Canopy","scenes":[{"theme":"jungle","sky":["#63c4ff","#d8f4e0"],"sun":"#fffbe0","far":"#8fcf8a","trees2":"#5fb86a","trees":"#2f8a4c","trunk":"#5b3a22","ground":"#6b5a3a","grass":"#5cc254","ball":"#ff8a2e","art":"jungleFalls"},{"theme":"jungle","sky":["#63c4ff","#d8f4e0"],"sun":"#fffbe0","far":"#8fcf8a","trees2":"#5fb86a","trees":"#2f8a4c","trunk":"#5b3a22","ground":"#6b5a3a","grass":"#5cc254","ball":"#b45cff","art":"jungleDay","filter":"saturate(1.25) hue-rotate(-14deg) brightness(.97)"}]},
    {"name":"Temple Steps","scenes":[{"theme":"jungle","sky":["#5a4a2a","#b8943e"],"sun":"#ffe7a8","far":"#6a5a3a","trees2":"#5a6a3a","trees":"#3a4a2a","trunk":"#3a2a1a","ground":"#6a5a3a","grass":"#8a9a4a","ball":"#3bd1ff","art":"jungleTemple"},{"theme":"jungle","sky":["#ff9a6b","#ffe0b0"],"sun":"#fff1c9","far":"#c98a6a","trees2":"#7a8a4a","trees":"#3f6a34","trunk":"#4b2e1a","ground":"#5b4430","grass":"#8cb84b","ball":"#ff4f7a","art":"jungleDusk","filter":"hue-rotate(-18deg) saturate(1.2)"}]},
    {"name":"Hidden Falls","scenes":[{"theme":"jungle","sky":["#63c4ff","#d8f4e0"],"sun":"#fffbe0","far":"#8fcf8a","trees2":"#5fb86a","trees":"#2f8a4c","trunk":"#5b3a22","ground":"#6b5a3a","grass":"#5cc254","ball":"#ffd23b","art":"jungleFalls","filter":"hue-rotate(12deg) saturate(1.1)"},{"theme":"jungle","sky":["#0e1f3d","#2a4f5e"],"sun":"#fffbe0","far":"#8fcf8a","trees2":"#5fb86a","trees":"#2f8a4c","trunk":"#5b3a22","ground":"#6b5a3a","grass":"#5cc254","ball":"#ffd23b","art":"jungleDay","filter":"brightness(.58) saturate(.75) contrast(1.1) hue-rotate(10deg)"}]},
    {"name":"Golden Sanctum","scenes":[{"theme":"jungle","sky":["#5a4a2a","#b8943e"],"sun":"#ffe7a8","far":"#6a5a3a","trees2":"#5a6a3a","trees":"#3a4a2a","trunk":"#3a2a1a","ground":"#6a5a3a","grass":"#8a9a4a","ball":"#ff4fa3","art":"jungleTemple","filter":"sepia(.4) saturate(1.45) brightness(1.05)"},{"theme":"jungle","sky":["#5a4a2a","#b8943e"],"sun":"#ffe7a8","far":"#6a5a3a","trees2":"#5a6a3a","trees":"#3a4a2a","trunk":"#3a2a1a","ground":"#6a5a3a","grass":"#8a9a4a","ball":"#5ff0c8","art":"jungleTemple"}]},
  ],
  levels: [
    {"b":[[3,0.2,1],[2,0.82,-1]],"k":[["s",250,190,140,14,150,0,5,0]]}, // 97 · slider d=0.34 · robot 25 s / 55 s
    {"b":[[3,0.15,1,null,"green"],[1,0.74,-1]],"k":[["g",320],["s",420,185,100,14,70,0,3.97,0],["s",100,205,100,14,60,0,4.8,3.14]]}, // 98 · gateSlider d=0.36 · robot 22 s / 97 s
    {"b":[[3,0.15,1],[1,0.39,-1],[1,0.62,1],[2,0.85,-1]],"k":[["b",160,190,8]]}, // 99 · bricks d=0.38 · robot 29 s / 70 s
    {"b":[[3,0.15,1,null,"green"],[1,0.75,-1]],"k":[["g",320],["s",70,205,120,14],["s",450,205,120,14]]}, // 100 · pads d=0.33 · robot 23 s / 97 s
    {"b":[[3,0.14,1,null,"gold"],[2,0.39,-1],[2,0.61,1],[2,0.9,-1,null,"steel"]],"k":[["g",320]],"hard":1}, // 101 · difficile · gate d=0.53 · robot 28 s / 87 s
    {"b":[[4,0.12,1,null,"gold"],[1,0.4,-1],[2,0.65,1],[2,0.88,-1]],"k":[["s",0,170,260,14],["s",380,170,260,14]]}, // 102 · shelves d=0.45 · robot 35 s / 100 s
    {"b":[[3,0.14,1],[1,0.39,-1,null,"black"],[1,0.61,1],[2,0.86,-1,null,"steel"]],"k":[["b",160,190,8]]}, // 103 · bricks d=0.47 · robot 20 s / 73 s
    {"b":[[3,0.12,1,60],[2,0.35,-1,60,"ghost"],[1,0.64,1,60,"bomb"],[2,0.86,-1,60,"bouncy"]],"k":[["s",0,210,40,16],["b",40,210,1],["b",80,210,1],["b",120,210,1],["b",160,210,1],["s",200,210,40,16],["b",240,210,1],["b",280,210,1],["b",320,210,1],["b",360,210,1],["s",400,210,40,16],["b",440,210,1],["b",480,210,1],["b",520,210,1],["b",560,210,1],["s",600,210,40,16]]}, // 104 · brickFloor d=0.41 · robot 30 s / 84 s
    {"b":[[3,0.14,1,null,"steel"],[1,0.37,-1,null,"ghost"],[2,0.64,1],[1,0.88,-1]],"k":[["s",180,208,158,14,146,0,5.19,0.27]]}, // 105 · slider d=0.52 · robot 25 s / 68 s
    {"b":[[3,0.07,1],[2,0.31,-1],[1,0.6,1],[1,0.67,-1,null,"ghost"],[1,0.87,1,null,"black"]],"k":[["s",190,125,260,16,150,0,6.82,0]],"hard":1}, // 106 · difficile · lid d=0.64 · robot 24 s / 73 s
    {"b":[[3,0.19,1],[2,0.4,-1],[1,0.81,1]],"k":[["s",0,170,180,14],["s",460,170,180,14],["s",265,230,110,14,150,0,4.5,0]]}, // 107 · shelfSlider d=0.56 · robot 22 s / 61 s
    {"boss":"king","mega":1}, // 108 · boss ·  · robot 34 s / 135 s
    {"b":[[3,0.17,1],[1,0.6,-1],[1,0.82,1,null,"bomb"]],"k":[["s",0,170,180,14],["s",460,170,180,14],["s",265,230,110,14,150,0,4.5,0]]}, // 109 · shelfSlider d=0.42 · robot 18 s / 54 s
    {"b":[[4,0.11,1],[1,0.36,-1,null,"black"],[2,0.62,1],[2,0.89,-1,null,"ghost"]],"k":[["s",0,170,260,14],["s",380,170,260,14]]}, // 110 · shelves d=0.44 · robot 36 s / 103 s
    {"b":[[3,0.14,1,null,"bomb"],[2,0.37,-1],[2,0.61,1],[1,0.88,-1]],"k":[["s",275,130,90,14,180,0,6,0],["s",275,185,90,14,180,0,6,2.09],["s",275,240,90,14,180,0,6,4.19]]}, // 111 · ferris d=0.46 · robot 22 s / 72 s
    {"b":[[3,0.15,1,null,"bomb"],[2,0.6,-1],[1,0.82,1]],"k":[["s",250,150,130,14,170,0,5.5,1.58],["s",250,225,130,14,170,0,5.5,4.72]]}, // 112 · twin d=0.41 · robot 26 s / 61 s
    {"b":[[3,0.1,1,null,"green"],[2,0.35,-1],[2,0.65,1],[1,0.86,-1]],"k":[["s",60,195,120,14,0,45,4.33,0],["s",460,195,120,14,0,45,4.33,3.14]],"hard":1}, // 113 · difficile · elevators d=0.61 · robot 28 s / 115 s
    {"b":[[3,0.15,1,null,"ghost"],[1,0.53,-1],[2,0.85,1,null,"bomb"]],"k":[["g",320],["s",420,185,100,14,70,0,3.62,0],["s",100,205,100,14,60,0,4.94,3.14]]}, // 114 · gateSlider d=0.53 · robot 19 s / 72 s
    {"b":[[4,0.14,1,null,"gold"],[2,0.4,-1,null,"bouncy"],[2,0.62,1],[2,0.88,-1]],"k":[["s",40,250,100,14],["s",180,200,100,14],["s",320,150,100,14],["s",460,100,100,14]]}, // 115 · stairs d=0.55 · robot 33 s / 109 s
    {"b":[[3,0.15,1],[2,0.4,-1],[1,0.65,1],[1,0.88,-1,null,"green"]],"k":[["s",250,150,130,14,170,0,5.5,1.99],["s",250,225,130,14,170,0,5.5,5.13]]}, // 116 · twin d=0.49 · robot 33 s / 67 s
    {"b":[[4,0.14,1,null,"gold"],[2,0.4,-1,null,"bomb"],[2,0.6,1,null,"gold"],[2,0.85,-1]],"k":[["b",160,190,8]]}, // 117 · bricks d=0.6 · robot 31 s / 111 s
    {"b":[[4,0.13,1],[2,0.32,-1],[2,0.6,1],[2,0.67,-1,null,"steel"],[2,0.88,1,null,"steel"]],"k":[["s",275,130,90,14,180,0,6,0],["s",275,185,90,14,180,0,6,2.09],["s",275,240,90,14,180,0,6,4.19]],"hard":1}, // 118 · difficile · ferris d=0.72 · robot 46 s / 122 s
    {"b":[[3,0.15,1,null,"green"],[2,0.4,-1],[2,0.61,1,null,"gold"],[2,0.88,-1]],"k":[["s",190,125,260,16,150,0,6.69,0]]}, // 119 · lid d=0.64 · robot 28 s / 122 s
    {"boss":"storm","mega":1}, // 120 · boss ·  · robot 55 s / 135 s
    {"b":[[3,0.11,1],[2,0.38,-1,null,"steel"],[2,0.65,1],[1,0.9,-1]],"k":[["s",40,250,100,14],["s",180,200,100,14],["s",320,150,100,14],["s",460,100,100,14]]}, // 121 · stairs d=0.5 · robot 23 s / 74 s
    {"b":[[3,0.14,1,null,"bouncy"],[2,0.37,-1,null,"green"],[2,0.63,1,null,"black"],[1,0.86,-1,null,"ghost"]],"k":[["s",190,125,260,16,150,0,7.12,0]]}, // 122 · lid d=0.52 · robot 22 s / 91 s
    {"b":[[4,0.11,1,null,"gold"],[2,0.38,-1,null,"bouncy"],[2,0.63,1,null,"ghost"],[2,0.86,-1]],"k":[["b",220,178,5,150,0,6.74,6.05]]}, // 123 · brickSlide d=0.54 · robot 33 s / 114 s
    {"b":[[4,0.2,1,262,"bomb"],[2,0.6,-1,262],[2,0.81,1,262,"bouncy"]],"k":[["s",0,200,640,14]]}, // 124 · ceiling d=0.49 · robot 32 s / 97 s
    {"b":[[5,0.4,1]],"k":[["s",180,215,157,14,125,0,4.72,5.96]],"hard":1}, // 125 · difficile · slider d=0.69 · robot 41 s / 125 s
    {"b":[[4,0.13,1],[2,0.35,-1,null,"gold"],[2,0.65,1],[2,0.89,-1]],"k":[["b",220,171,5,150,0,7.81,1.01]]}, // 126 · brickSlide d=0.61 · robot 24 s / 109 s
    {"b":[[3,0.12,1,262],[2,0.4,-1,262,"bomb"],[2,0.65,1,262,"steel"],[1,0.85,-1,262,"gold"]],"k":[["s",0,200,640,14]]}, // 127 · ceiling d=0.63 · robot 12 s / 74 s
    {"b":[[4,0.15,1,null,"bomb"],[3,0.6,-1],[2,0.86,1]],"k":[["s",0,170,180,14],["s",460,170,180,14],["s",265,230,110,14,150,0,4.5,0]]}, // 128 · shelfSlider d=0.57 · robot 30 s / 108 s
    {"b":[[5,0.6,1]],"k":[["s",325,195,123,14,178,0,6.39,2.07]]}, // 129 · slider d=0.68 · robot 34 s / 125 s
    {"b":[[4,0.08,1,null,"bomb"],[2,0.32,-1],[2,0.6,1,null,"gold"],[2,0.7,-1,null,"ghost"],[2,0.88,1,null,"green"]],"k":[["s",275,130,90,14,180,0,6,0],["s",275,185,90,14,180,0,6,2.09],["s",275,240,90,14,180,0,6,4.19]],"hard":1}, // 130 · difficile · ferris d=0.76 · robot 45 s / 131 s
    {"b":[[4,0.12,1],[2,0.33,-1,null,"bomb"],[2,0.6,1],[2,0.71,-1,null,"bomb"],[2,0.9,1,null,"bomb"]],"k":[["b",220,169,5,150,0,6.99,0.22]]}, // 131 · brickSlide d=0.72 · robot 25 s / 121 s
    {"boss":"squid","mega":1}, // 132 · boss ·  · robot 20 s / 149 s
    {"b":[[4,0.11,1,null,"ghost"],[2,0.35,-1,null,"black"],[2,0.61,1],[2,0.86,-1,null,"ghost"]],"k":[["s",192,177,126,14,168,0,4.38,0.06]]}, // 133 · slider d=0.58 · robot 49 s / 122 s
    {"b":[[4,0.1,1],[2,0.37,-1],[2,0.61,1,null,"green"],[2,0.9,-1]],"k":[["s",60,195,120,14,0,45,3.6,0],["s",460,195,120,14,0,45,3.6,3.14]]}, // 134 · elevators d=0.6 · robot 48 s / 117 s
    {"b":[[3,0.12,1],[1,0.39,-1,null,"bomb"],[2,0.64,1],[1,0.87,-1]],"k":[["b",220,194,5,150,0,6.31,4.19]]}, // 135 · brickSlide d=0.62 · robot 18 s / 69 s
    {"b":[[3,0.15,1],[1,0.36,-1],[1,0.62,1],[1,0.88,-1]],"k":[["g",320],["s",70,205,120,14],["s",450,205,120,14]]}, // 136 · pads d=0.57 · robot 21 s / 65 s
    {"b":[[4,0.09,1,262],[2,0.31,-1,262],[2,0.4,1,262,"bomb"],[3,0.71,-1,262,"bouncy"],[2,0.89,1,262,"ghost"]],"k":[["s",0,200,640,14]],"hard":1}, // 137 · difficile · ceiling d=0.77 · robot 28 s / 139 s
    {"b":[[3,0.12,1],[1,0.35,-1],[2,0.64,1,null,"bouncy"],[1,0.88,-1]],"k":[["s",275,130,90,14,180,0,6,0],["s",275,185,90,14,180,0,6,2.09],["s",275,240,90,14,180,0,6,4.19]]}, // 138 · ferris d=0.69 · robot 22 s / 68 s
    {"b":[[4,0.08,1,null,"ghost"],[2,0.35,-1,null,"steel"],[2,0.53,1,null,"gold"],[2,0.73,-1],[2,0.9,1]],"k":[["g",320]]}, // 139 · gate d=0.71 · robot 62 s / 137 s
    {"b":[[4,0.11,1,null,"bomb"],[2,0.4,-1],[3,0.63,1,null,"bouncy"],[2,0.88,-1,null,"ghost"]],"k":[["b",220,172,5,150,0,6.13,1.83]]}, // 140 · brickSlide d=0.65 · robot 22 s / 130 s
    {"b":[[4,0.09,1],[1,0.29,-1,null,"green"],[2,0.4,1,null,"gold"],[2,0.7,-1,null,"green"],[2,0.87,1]],"k":[["s",190,125,260,16,150,0,7.94,0]]}, // 141 · lid d=0.76 · robot 35 s / 123 s
    {"b":[[4,0.1,1],[2,0.31,-1,null,"bouncy"],[3,0.4,1,null,"gold"],[3,0.67,-1,null,"bouncy"],[2,0.9,1]],"k":[["b",220,165,5,150,0,7.01,0.63]],"hard":1}, // 142 · difficile · brickSlide d=0.88 · robot 42 s / 156 s
    {"b":[[5,0.14,1],[1,0.6,-1],[1,0.81,1]],"k":[["s",275,130,90,14,180,0,6,0],["s",275,185,90,14,180,0,6,2.09],["s",275,240,90,14,180,0,6,4.19]]}, // 143 · ferris d=0.8 · robot 47 s / 135 s
    {"boss":"eye","mega":1}, // 144 · boss ·  · robot 39 s / 162 s
    {"b":[[4,0.14,1,null,"gold"],[2,0.37,-1],[2,0.64,1,null,"bouncy"],[2,0.88,-1]],"k":[["s",305,216,127,14,115,0,4.14,0.12]]}, // 145 · slider d=0.66 · robot 27 s / 109 s
    {"b":[[3,0.11,1],[1,0.38,-1,null,"bouncy"],[2,0.65,1,null,"bouncy"],[2,0.85,-1,null,"gold"]],"k":[["s",250,150,130,14,170,0,5.5,0.31],["s",250,225,130,14,170,0,5.5,3.45]]}, // 146 · twin d=0.68 · robot 38 s / 76 s
    {"b":[[5,0.4,1]],"k":[["s",0,170,180,14],["s",460,170,180,14],["s",265,230,110,14,150,0,4.5,0]]}, // 147 · shelfSlider d=0.7 · robot 58 s / 125 s
    {"b":[[4,0.13,1],[2,0.36,-1],[2,0.62,1,null,"black"],[3,0.88,-1]],"k":[["s",309,173,150,14,151,0,6.43,4.98]]}, // 148 · slider d=0.65 · robot 38 s / 122 s
    {"b":[[5,0.1,1,null,"bouncy"],[1,0.35,-1],[1,0.6,1,null,"steel"],[1,0.88,-1,null,"bomb"]],"k":[["b",220,176,5,150,0,6,6.04]],"hard":1}, // 149 · difficile · brickSlide d=0.85 · robot 55 s / 167 s
    {"b":[[4,0.09,1,null,"gold"],[2,0.32,-1],[2,0.4,1,null,"green"],[2,0.69,-1],[2,0.92,1,null,"ghost"]],"k":[["s",275,130,90,14,180,0,6,0],["s",275,185,90,14,180,0,6,2.09],["s",275,240,90,14,180,0,6,4.19]]}, // 150 · ferris d=0.77 · robot 33 s / 131 s
    {"b":[[4,0.12,1],[2,0.3,-1],[2,0.6,1],[2,0.7,-1,null,"bomb"],[3,0.91,1,null,"bomb"]],"k":[["s",40,250,100,14],["s",180,200,100,14],["s",320,150,100,14],["s",460,100,100,14]]}, // 151 · stairs d=0.79 · robot 39 s / 132 s
    {"b":[[4,0.08,1,null,"bomb"],[2,0.28,-1,null,"steel"],[2,0.6,1],[2,0.71,-1],[2,0.87,1,null,"bomb"]],"k":[["s",178,214,144,14,126,0,6.24,4.35]]}, // 152 · slider d=0.73 · robot 31 s / 120 s
    {"b":[[4,0.06,1],[1,0.24,-1,null,"steel"],[2,0.4,1],[2,0.6,-1,null,"gold"],[2,0.77,1,null,"green"],[2,0.94,-1,null,"ghost"]],"k":[["s",250,150,130,14,170,0,5.5,2.48],["s",250,225,130,14,170,0,5.5,5.62]]}, // 153 · twin d=0.84 · robot 43 s / 138 s
    {"b":[[5,0.11,1],[1,0.31,-1,null,"steel"],[1,0.4,1,null,"bomb"],[1,0.7,-1],[1,0.92,1,null,"steel"]],"k":[["s",275,130,90,14,180,0,6,0],["s",275,185,90,14,180,0,6,2.09],["s",275,240,90,14,180,0,6,4.19]],"hard":1}, // 154 · difficile · ferris d=0.95 · robot 40 s / 149 s
    {"b":[[4,0.09,1,null,"steel"],[2,0.31,-1],[2,0.4,1,null,"gold"],[3,0.68,-1],[2,0.92,1,null,"black"]],"k":[["s",250,150,130,14,170,0,5.5,1.83],["s",250,225,130,14,170,0,5.5,4.97]]}, // 155 · twin d=0.88 · robot 24 s / 136 s
    {"boss":"golem"}, // 156 · boss ·  · robot 66 s / 150 s
  ],
});
