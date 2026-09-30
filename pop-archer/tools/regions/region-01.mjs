// Recette de la région 1 : Jungle Temple (mondes 9 à 13, niveaux 97 à 156).
// Tout ce qui est écrit à la main (mondes, décors, textes, boss) ; les niveaux sont fabriqués par tools/atelier.mjs.
const sky = { sky: ['#63c4ff', '#d8f4e0'], sun: '#fffbe0', far: '#8fcf8a', trees2: '#5fb86a', trees: '#2f8a4c', trunk: '#5b3a22', ground: '#6b5a3a', grass: '#5cc254' };
const dusk = { sky: ['#ff9a6b', '#ffe0b0'], sun: '#fff1c9', far: '#c98a6a', trees2: '#7a8a4a', trees: '#3f6a34', trunk: '#4b2e1a', ground: '#5b4430', grass: '#8cb84b' };
const inside = { sky: ['#5a4a2a', '#b8943e'], sun: '#ffe7a8', far: '#6a5a3a', trees2: '#5a6a3a', trees: '#3a4a2a', trunk: '#3a2a1a', ground: '#6a5a3a', grass: '#8a9a4a' };

export default {
  id: 1,
  name: 'Jungle Temple',
  first: 97,
  mechanic: 'moving',           // plateformes mobiles : va-et-vient, ascenseurs, briques qui glissent
  art: {
    jungleDay: ['img/jungle-day.webp', .876],
    jungleDusk: ['img/jungle-dusk.webp', .9],
    jungleTemple: ['img/jungle-temple.webp', .853],
    jungleFalls: ['img/jungle-falls.webp', .918],
  },
  // Deux ambiances par monde ; « filter » recolore un décor pour varier sans nouvelle image.
  worlds: [
    { name: 'Jungle Gate', tip: 'Jungle Gate: some platforms move!',
      scenes: [{ theme: 'jungle', ...sky, ball: '#ff4f7a', art: 'jungleDay' }, { theme: 'jungle', ...dusk, ball: '#4f7aff', art: 'jungleDusk' }] },
    { name: 'Monkey Canopy',
      scenes: [{ theme: 'jungle', ...sky, ball: '#ff8a2e', art: 'jungleFalls' }, { theme: 'jungle', ...sky, ball: '#b45cff', art: 'jungleDay', filter: 'saturate(1.25) hue-rotate(-14deg) brightness(.97)' }] },
    { name: 'Temple Steps',
      scenes: [{ theme: 'jungle', ...inside, ball: '#3bd1ff', art: 'jungleTemple' }, { theme: 'jungle', ...dusk, ball: '#ff4f7a', art: 'jungleDusk', filter: 'hue-rotate(-18deg) saturate(1.2)' }] },
    { name: 'Hidden Falls',
      scenes: [{ theme: 'jungle', ...sky, ball: '#ffd23b', art: 'jungleFalls', filter: 'hue-rotate(12deg) saturate(1.1)' },
               { theme: 'jungle', ...sky, sky: ['#0e1f3d', '#2a4f5e'], ball: '#ffd23b', art: 'jungleDay', filter: 'brightness(.58) saturate(.75) contrast(1.1) hue-rotate(10deg)' }] },
    { name: 'Golden Sanctum',
      scenes: [{ theme: 'jungle', ...inside, ball: '#ff4fa3', art: 'jungleTemple', filter: 'sepia(.4) saturate(1.45) brightness(1.05)' }, { theme: 'jungle', ...inside, ball: '#5ff0c8', art: 'jungleTemple' }] },
  ],
  // Boss du 12e niveau de chaque monde : 4 méga-boss, puis le nouveau boss de la région.
  bosses: [{ boss: 'king', mega: 1 }, { boss: 'storm', mega: 1 }, { boss: 'squid', mega: 1 }, { boss: 'eye', mega: 1 }, { boss: 'golem' }],
  i18n: {
    'Jungle Gate': ['Porte de la jungle', 'Puerta de la Selva', 'Portão da Selva', 'Dschungeltor'],
    'Monkey Canopy': ['Canopée des singes', 'Copas de los Monos', 'Copa dos Macacos', 'Affenwipfel'],
    'Temple Steps': ['Marches du temple', 'Escalinata del Templo', 'Degraus do Templo', 'Tempelstufen'],
    'Hidden Falls': ['Chutes cachées', 'Cascada Escondida', 'Cachoeira Escondida', 'Verborgene Fälle'],
    'Golden Sanctum': ['Sanctuaire doré', 'Santuario Dorado', 'Santuário Dourado', 'Goldenes Heiligtum'],
    'Jungle Gate: some platforms move!': ['Porte de la jungle : certaines plateformes bougent !', 'Puerta de la Selva: ¡algunas plataformas se mueven!', 'Portão da Selva: algumas plataformas se movem!', 'Dschungeltor: Manche Plattformen bewegen sich!'],
  },
  // Difficulté de base de la région (0 à 1) et sa progression d'un monde à l'autre.
  base: .34, perWorld: .08,
};
