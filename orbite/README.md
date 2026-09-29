# Orbite — prototype jouable

Casse-briques circulaire sans raquette. La bille tourne autour du noyau ; une touche sur l'écran la lâche tout droit vers l'extérieur. Elle rebondit sur les briques et le bord de l'arène, puis la gravité du noyau la ramène en orbite. Les anneaux de briques avancent vers le noyau : s'ils atteignent le cercle rouge, la partie est finie.

## Jouer

```sh
cd orbite
python3 -m http.server 8000   # puis http://localhost:8000
```

Touche l'écran (ou Espace) pour lancer. Échap ou P : pause.

## Briques

| Brique | Effet |
|---|---|
| Verte / jaune « 2 » / rose « 3 » | 1, 2 ou 3 coups pour la casser |
| Orange (étoile) | Explose et casse ses voisines, réactions en chaîne possibles |
| Bleue « +1 » | Une bille de plus en orbite (4 max) ; une touche lance toutes les billes en orbite |
| Bleu clair (flocon) | Ralentit l'avance des briques pendant 7 s |
| Grise rayée | Indestructible, arrive à partir de la vague 7 |

Points : 10 × le rang du coup dans le tir (combo). Anneaux entièrement nettoyés : +250. Après une défaite, « Repousser les briques et continuer » (une fois par partie) simule l'offre d'échec.

## Réglage

Les constantes sont en tête de `game.js` (vitesse d'orbite, vitesse de lancer, gravité). L'avance des briques est dans `speed()`. Avec `#debug` dans l'URL, `window.OrbiteDebug` permet de simuler des parties : sans jouer, on perd en 38 s ; un robot qui vise tient de 95 à 130 s (vagues 12 à 16).
