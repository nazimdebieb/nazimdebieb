// Recette de la région 2 : Château des pièges (mondes 14 à 18, niveaux 157 à 216).
// Inspirée des niveaux à pièges des jeux de bulles classiques, sans les copier : un piège par monde,
// qui revient ensuite mêlé aux autres. Les niveaux sont fabriqués par tools/atelier.mjs.
const hall = { sky: ['#2e3a56', '#5a6a88'], far: '#4a5a78', ground: '#8a94a4', banner: '#c8323a' };
const dungeon = { sky: ['#14241e', '#3a5a48'], far: '#2f4a3e', ground: '#5a6a4a', banner: '#3a6a8a' };
const armory = { sky: ['#5a3a1e', '#c8a070'], far: '#8a6a4a', ground: '#a8743a', banner: '#2a5aa8' };
const throne = { sky: ['#8a7a9a', '#e8dcc8'], far: '#c8bca8', ground: '#b8303a', banner: '#6a3a9a' };

export default {
  id: 2,
  name: 'Trap Castle',
  first: 157,
  mechanic: 'traps',            // plafond à pointes, presse, murs qui avancent, gargouilles, bulles de pierre
  // premier niveau de la région : le plafond à pointes, deux bulles faciles
  intro: { b: [[3, .2, 1], [2, .8, -1]], spikes: 1 },
  // bulles spéciales en plus du lot commun (la bulle de pierre n'arrive qu'avec son piège, au dernier monde)
  pool: ['sleep'],
  art: {
    castleHall: ['img/castle-hall.webp', .856],
    castleDungeon: ['img/castle-dungeon.webp', .873],
    castleArmory: ['img/castle-armory.webp', .885],
    castleThrone: ['img/castle-throne.webp', .855],
  },
  // mech : pièges du monde (le premier est montré au 1er niveau) ; fresh : combien des premiers reviennent plus souvent ;
  // all : champs ajoutés à tous les niveaux du monde
  worlds: [
    { name: 'Spike Hall', tip: 'Spike Hall: bubbles that touch the spiked ceiling burst at once!',
      mech: ['spikes', 'giant', 'sleepers', 'row', 'mirror'], fresh: 2, all: { spikes: 1 },
      scenes: [{ theme: 'castle', ...hall, ball: '#ff4f7a', art: 'castleHall' }, { theme: 'castle', ...armory, ball: '#3bd1ff', art: 'castleArmory' }] },
    { name: 'Crushing Corridor', tip: 'Crushing Corridor: the ceiling comes down as time runs out!',
      mech: ['press', 'rain', 'corridor', 'spikes', 'sleepers'], fresh: 2,
      scenes: [{ theme: 'castle', ...dungeon, ball: '#ffd23b', art: 'castleDungeon' }, { theme: 'castle', ...hall, ball: '#9dff5a', art: 'castleHall', filter: 'hue-rotate(150deg) saturate(.8) brightness(.85)' }] },
    { name: 'Moving Walls', tip: 'Moving Walls: some walls slide, some doors open wide!',
      mech: ['slide', 'doors', 'chambers', 'press', 'giant'], fresh: 3,
      scenes: [{ theme: 'castle', ...armory, ball: '#b45cff', art: 'castleArmory', filter: 'hue-rotate(-12deg) saturate(1.15)' }, { theme: 'castle', ...dungeon, ball: '#ff8a2e', art: 'castleDungeon', filter: 'hue-rotate(40deg) saturate(.9)' }] },
    { name: 'Gargoyle Gallery', tip: 'Gargoyle Gallery: stone heads spit bubbles from the walls!',
      mech: ['gargoyles', 'spikes', 'slide', 'rain', 'mirror'], fresh: 1,
      scenes: [{ theme: 'castle', ...hall, sky: ['#0e1630', '#2a3a5e'], ball: '#ffd23b', art: 'castleHall', filter: 'brightness(.62) saturate(.8) contrast(1.1)' }, { theme: 'castle', ...armory, ball: '#ff4fa3', art: 'castleArmory', filter: 'sepia(.35) brightness(.9)' }] },
    { name: 'Stone Keep', tip: 'Stone Keep: stone bubbles roll on the floor. You get the sticky arrow!',
      mech: ['stone', 'gargoyles', 'press', 'doors', 'giant', 'chambers'], fresh: 1,
      scenes: [{ theme: 'castle', ...throne, ball: '#3bd1ff', art: 'castleThrone' }, { theme: 'castle', ...throne, ball: '#ffd23b', art: 'castleThrone', filter: 'hue-rotate(200deg) saturate(.75) brightness(.8)' }] },
  ],
  // Boss du 12e niveau de chaque monde : 4 méga-boss, puis le nouveau boss de la région.
  bosses: [{ boss: 'yeti', mega: 1 }, { boss: 'dragon', mega: 1 }, { boss: 'pirate', mega: 1 }, { boss: 'phantom', mega: 1 }, { boss: 'guardian' }],
  i18n: {
    'Trap Castle': ['Château des pièges', 'Castillo de las Trampas', 'Castelo das Armadilhas', 'Fallenburg'],
    'Spike Hall': ['Salle des pointes', 'Sala de los Pinchos', 'Salão dos Espinhos', 'Stachelhalle'],
    'Crushing Corridor': ['Couloir de la presse', 'Pasillo Aplastante', 'Corredor Esmagador', 'Quetschgang'],
    'Moving Walls': ['Murs mouvants', 'Muros Móviles', 'Paredes Móveis', 'Wandernde Wände'],
    'Gargoyle Gallery': ['Galerie des gargouilles', 'Galería de Gárgolas', 'Galeria das Gárgulas', 'Wasserspeier-Galerie'],
    'Stone Keep': ['Donjon de pierre', 'Torreón de Piedra', 'Torre de Pedra', 'Steinfried'],
    'Spike Hall: bubbles that touch the spiked ceiling burst at once!': ['Salle des pointes : les bulles qui touchent le plafond à pointes éclatent d’un coup !', 'Sala de los Pinchos: ¡las burbujas que tocan el techo de pinchos revientan de golpe!', 'Salão dos Espinhos: as bolhas que tocam o teto de espinhos estouram de uma vez!', 'Stachelhalle: Blasen, die die Stacheldecke berühren, platzen auf einmal!'],
    'Crushing Corridor: the ceiling comes down as time runs out!': ['Couloir de la presse : le plafond descend à mesure que le temps passe !', 'Pasillo Aplastante: ¡el techo baja a medida que se acaba el tiempo!', 'Corredor Esmagador: o teto desce conforme o tempo acaba!', 'Quetschgang: Die Decke senkt sich, während die Zeit abläuft!'],
    'Moving Walls: some walls slide, some doors open wide!': ['Murs mouvants : certains murs avancent, certaines portes s’ouvrent en grand !', 'Muros Móviles: ¡algunos muros se deslizan y algunas puertas se abren del todo!', 'Paredes Móveis: algumas paredes deslizam, algumas portas se abrem por inteiro!', 'Wandernde Wände: Manche Wände rücken vor, manche Türen öffnen sich ganz!'],
    'Gargoyle Gallery: stone heads spit bubbles from the walls!': ['Galerie des gargouilles : des têtes de pierre crachent des bulles depuis les murs !', 'Galería de Gárgolas: ¡cabezas de piedra escupen burbujas desde los muros!', 'Galeria das Gárgulas: cabeças de pedra cospem bolhas das paredes!', 'Wasserspeier-Galerie: Steinköpfe spucken Blasen aus den Wänden!'],
    'Stone Keep: stone bubbles roll on the floor. You get the sticky arrow!': ['Donjon de pierre : les bulles de pierre roulent au sol. Tu reçois la flèche collante !', 'Torreón de Piedra: las burbujas de piedra ruedan por el suelo. ¡Recibes la flecha pegajosa!', 'Torre de Pedra: as bolhas de pedra rolam no chão. Você ganha a flecha grudenta!', 'Steinfried: Steinblasen rollen über den Boden. Du bekommst den Klebepfeil!'],
  },
  // Difficulté de base de la région (0 à 1) et sa progression d'un monde à l'autre.
  base: .42, perWorld: .07,
};
