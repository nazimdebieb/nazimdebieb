// Fichier fabriqué par tools/chrono.mjs : ne pas modifier à la main (relancer l'outil).
// Pour chaque niveau : [temps en secondes sur un écran de 640 de large, difficulté (1 facile, 2 moyen,
// 3 difficile, 4 très difficile, 0 boss), temps mis par le robot]. Le jeu ajoute du temps sur les écrans larges.
window.POP_TIMING = {
  1: [25, 1, 7], // facile, robot 7 s
  2: [39, 2, 19], // moyen, robot 19 s
  3: [46, 1, 18], // facile, robot 18 s
  4: [44, 1, 17], // facile, robot 17 s
  5: [43, 2, 21], // moyen, robot 21 s
  6: [37, 1, 14], // facile, robot 14 s
  7: [42, 1, 16], // facile, robot 16 s
  8: [50, 2, 25], // moyen, robot 25 s
  9: [63, 3, 40], // difficile, robot 40 s
  10: [67, 2, 34], // moyen, robot 34 s
  11: [63, 3, 40], // difficile, robot 40 s
  12: [50, 0, 24], // boss, robot 24 s
  13: [53, 1, 21], // facile, robot 21 s
  14: [56, 2, 28], // moyen, robot 28 s
  15: [47, 2, 23], // moyen, robot 23 s
  16: [53, 4, 41], // très difficile, robot 41 s
  17: [42, 1, 16], // facile, robot 16 s
  18: [56, 3, 35], // difficile, robot 35 s
  19: [49, 1, 19], // facile, robot 19 s
  20: [67, 2, 34], // moyen, robot 34 s
  21: [35, 1, 13], // facile, robot 13 s
  22: [58, 2, 29], // moyen, robot 29 s
  23: [60, 3, 38], // difficile, robot 38 s
  24: [66, 0, 34], // boss, robot 34 s
  25: [58, 2, 29], // moyen, robot 29 s
  26: [62, 1, 25], // facile, robot 25 s
  27: [63, 2, 32], // moyen, robot 32 s
  28: [69, 1, 28], // facile, robot 28 s
  29: [63, 3, 40], // difficile, robot 40 s
  30: [63, 2, 32], // moyen, robot 32 s
  31: [57, 4, 44], // très difficile, robot 44 s
  32: [63, 2, 32], // moyen, robot 32 s
  33: [62, 3, 39], // difficile, robot 39 s
  34: [69, 1, 28], // facile, robot 28 s
  35: [53, 3, 33], // difficile, robot 33 s
  36: [44, 0, 20], // boss, robot 20 s
  37: [61, 2, 31], // moyen, robot 31 s
  38: [60, 3, 38], // difficile, robot 38 s
  39: [54, 2, 27], // moyen, robot 27 s
  40: [66, 3, 42], // difficile, robot 42 s
  41: [65, 2, 33], // moyen, robot 33 s
  42: [51, 1, 20], // facile, robot 20 s
  43: [65, 1, 26], // facile, robot 26 s
  44: [55, 4, 42], // très difficile, robot 42 s
  45: [72, 2, 37], // moyen, robot 37 s
  46: [60, 3, 38], // difficile, robot 38 s
  47: [65, 1, 26], // facile, robot 26 s
  48: [38, 0, 16], // boss, robot 16 s
  49: [71, 2, 36], // moyen, robot 36 s
  50: [42, 1, 16], // facile, robot 16 s
  51: [65, 3, 41], // difficile, robot 41 s
  52: [52, 2, 26], // moyen, robot 26 s
  53: [68, 3, 43], // difficile, robot 43 s
  54: [63, 2, 32], // moyen, robot 32 s
  55: [57, 4, 44], // très difficile, robot 44 s
  56: [72, 2, 37], // moyen, robot 37 s
  57: [61, 4, 47], // très difficile, robot 47 s
  58: [40, 1, 15], // facile, robot 15 s
  59: [68, 3, 43], // difficile, robot 43 s
  60: [177, 0, 103], // boss, robot 103 s
  61: [62, 1, 25], // facile, robot 25 s
  62: [53, 3, 33], // difficile, robot 33 s
  63: [54, 2, 27], // moyen, robot 27 s
  64: [63, 2, 32], // moyen, robot 32 s
  65: [57, 3, 36], // difficile, robot 36 s
  66: [65, 1, 26], // facile, robot 26 s
  67: [60, 4, 46], // très difficile, robot 46 s
  68: [51, 4, 39], // très difficile, robot 39 s
  69: [54, 2, 27], // moyen, robot 27 s
  70: [63, 2, 32], // moyen, robot 32 s
  71: [53, 3, 33], // difficile, robot 33 s
  72: [130, 0, 74], // boss, robot 74 s
  73: [60, 2, 30], // moyen, robot 30 s
  74: [65, 2, 33], // moyen, robot 33 s
  75: [53, 1, 21], // facile, robot 21 s
  76: [69, 2, 35], // moyen, robot 35 s
  77: [80, 2, 41], // moyen, robot 41 s
  78: [72, 1, 29], // facile, robot 29 s
  79: [69, 3, 44], // difficile, robot 44 s
  80: [72, 3, 46], // difficile, robot 46 s
  81: [76, 4, 59], // très difficile, robot 59 s
  82: [74, 3, 47], // difficile, robot 47 s
  83: [65, 4, 50], // très difficile, robot 50 s
  84: [191, 0, 112], // boss, robot 112 s
  85: [54, 2, 27], // moyen, robot 27 s
  86: [71, 2, 36], // moyen, robot 36 s
  87: [46, 1, 18], // facile, robot 18 s
  88: [75, 3, 48], // difficile, robot 48 s
  89: [68, 3, 43], // difficile, robot 43 s
  90: [60, 2, 30], // moyen, robot 30 s
  91: [82, 2, 42], // moyen, robot 42 s
  92: [58, 1, 23], // facile, robot 23 s
  93: [116, 4, 91], // très difficile, robot 91 s
  94: [76, 4, 59], // très difficile, robot 59 s
  95: [89, 3, 57], // difficile, robot 57 s
  96: [132, 0, 75], // boss, robot 75 s
  97: [46, 1, 18], // facile, robot 18 s
  98: [47, 2, 23], // moyen, robot 23 s
  99: [41, 4, 31], // très difficile, robot 31 s
  100: [45, 2, 22], // moyen, robot 22 s
  101: [45, 3, 28], // difficile, robot 28 s
  102: [48, 4, 37], // très difficile, robot 37 s
  103: [46, 1, 18], // facile, robot 18 s
  104: [48, 3, 30], // difficile, robot 30 s
  105: [39, 2, 19], // moyen, robot 19 s
  106: [41, 3, 25], // difficile, robot 25 s
  107: [39, 2, 19], // moyen, robot 19 s
  108: [70, 0, 36], // boss, robot 36 s
  109: [56, 2, 28], // moyen, robot 28 s
  110: [68, 4, 53], // très difficile, robot 53 s
  111: [42, 1, 16], // facile, robot 16 s
  112: [40, 1, 15], // facile, robot 15 s
  113: [54, 3, 34], // difficile, robot 34 s
  114: [34, 2, 16], // moyen, robot 16 s
  115: [56, 3, 35], // difficile, robot 35 s
  116: [47, 2, 23], // moyen, robot 23 s
  117: [56, 3, 35], // difficile, robot 35 s
  118: [53, 4, 41], // très difficile, robot 41 s
  119: [56, 2, 28], // moyen, robot 28 s
  120: [119, 0, 67], // boss, robot 67 s
  121: [52, 2, 26], // moyen, robot 26 s
  122: [52, 4, 40], // très difficile, robot 40 s
  123: [59, 3, 37], // difficile, robot 37 s
  124: [58, 1, 23], // facile, robot 23 s
  125: [53, 4, 41], // très difficile, robot 41 s
  126: [65, 2, 33], // moyen, robot 33 s
  127: [53, 1, 21], // facile, robot 21 s
  128: [67, 2, 34], // moyen, robot 34 s
  129: [62, 3, 39], // difficile, robot 39 s
  130: [57, 3, 36], // difficile, robot 36 s
  131: [60, 2, 30], // moyen, robot 30 s
  132: [46, 0, 21], // boss, robot 21 s
  133: [62, 3, 39], // difficile, robot 39 s
  134: [54, 2, 27], // moyen, robot 27 s
  135: [44, 1, 17], // facile, robot 17 s
  136: [53, 1, 21], // facile, robot 21 s
  137: [45, 2, 22], // moyen, robot 22 s
  138: [48, 2, 24], // moyen, robot 24 s
  139: [53, 3, 33], // difficile, robot 33 s
  140: [58, 2, 29], // moyen, robot 29 s
  141: [56, 3, 35], // difficile, robot 35 s
  142: [58, 4, 45], // très difficile, robot 45 s
  143: [66, 4, 51], // très difficile, robot 51 s
  144: [50, 0, 24], // boss, robot 24 s
  145: [71, 2, 36], // moyen, robot 36 s
  146: [67, 1, 27], // facile, robot 27 s
  147: [72, 3, 46], // difficile, robot 46 s
  148: [74, 2, 38], // moyen, robot 38 s
  149: [63, 3, 40], // difficile, robot 40 s
  150: [72, 1, 29], // facile, robot 29 s
  151: [86, 4, 67], // très difficile, robot 67 s
  152: [72, 2, 37], // moyen, robot 37 s
  153: [72, 2, 37], // moyen, robot 37 s
  154: [74, 3, 47], // difficile, robot 47 s
  155: [66, 4, 51], // très difficile, robot 51 s
  156: [113, 0, 63], // boss, robot 63 s
};
