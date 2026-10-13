# Niveaux de référence Bloom Garden

`BG-001.json` à `BG-020.json` sont des échantillons **mathématiquement solvables** selon les règles déterministes version 0.1.0. Les trois premiers sont volontairement simples et didactiques ; les suivants ont été générés puis vérifiés par un solveur BFS exhaustif. Ils ne sont **pas** encore équilibrés/testés humainement et leurs décors/graphismes ne sont pas finalisés.

### Schéma

- `rulesVersion`: "0.1.0"
- `slotCount`: toujours 3 dans le MVP
- `grid.width`,`grid.height`: dimensions du plateau
- `grid.cells`: tableau de `width*height` cases, ordre de lecture haut→bas puis gauche→droite, chaque case est une liste de `colorId` de la **couche visible vers les couches cachées**
- `lanes`: trois tableaux de cans, première entrée = tête de file sélectionnable
- `can.id`: unique ; `can.color`: coral/blue/yellow/purple ; `capacity`: unité par bouton arrosé
- `referenceSolution`: indices des files (0,1,2) dans l'ordre d'une solution validée.

### Vérifier (Python standard)

Depuis la racine du kit :

```bash
python tools/solver_reference.py levels/BG-001.json levels/BG-020.json
```

Pour tous les fichiers, sous PowerShell :

```powershell
python tools/solver_reference.py (Get-ChildItem levels/BG-*.json | ForEach-Object FullName)
```

La commande `--generate 20 --output <dossier>` est un exemple de génération, **attention elle écrase les niveaux au même nom dans le dossier de sortie**. Utiliser un dossier temporaire de test, pas `levels/`, si tu souhaites conserver les trois tutoriels édités.

### Protocole de portage C#

1. Reproduire `validate`, `stabilize`, `move`, `solve` en logique C# pure.
2. Lire les 20 JSON et rejouer exactement `referenceSolution` sur chaque niveau.
3. Exiger que chaque replay finisse en victoire et que les invariants restent vrais.
4. Ajouter tests de propriétés et niveaux fabriqués à la main pour cas limites.
