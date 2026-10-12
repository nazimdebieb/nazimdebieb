# Bloom Garden — kit de lancement Unity + IA

Kit de production du jeu mobile de réflexion **Bloom Garden**, version du 12 octobre 2026.

## À quoi servent les fichiers ?

| Fichier | Usage |
|---|---|
| `Bloom_Garden_Dossier_Production_FR.pdf` | Cahier des charges illustré à lire / consulter |
| `docs/00_BLOOM_GARDEN_GDD_FR.md` | Version texte faisant foi pour Codex et Claude Code |
| `docs/PROMPT_MASTER_COMMUN.md` | Prompt principal à donner IDENTIQUEMENT aux deux IA |
| `docs/PROMPTS_PAR_PHASE.md` | Prompts de P0 à P9, à lancer selon les validations |
| `docs/INSTALLATION_UNITY_ET_MCP.md` | Installation Unity et plugins agents |
| `docs/DECISIONS.md` | Journal de décisions verrouillées / ouvertes |
| `assets/` | Concepts visuels originaux, **pas** des assets finaux du jeu |
| `levels/` | Données JSON de référence + contrat de format |
| `tools/solver_reference.py` | Solveur de référence indépendant en Python |
| `benchmark/GRILLE_EVALUATION.csv` | Comparaison objective Codex / Claude |

## Mode d'emploi recommandé

1. Lire le PDF puis le document d'installation.
2. Installer Unity et les modules Android, puis les plugins Unity officiels pour Codex et Claude Code.
3. Dupliquer le kit : `BloomGarden_Codex/` et `BloomGarden_Claude/`. Garder une archive non modifiée en référence.
4. Vérifier qu'une même version Unity, les mêmes niveaux et les mêmes consignes sont disponibles dans les deux copies.
5. Dans chacun, envoyer **tel quel** `docs/PROMPT_MASTER_COMMUN.md`. Commencer à P0 ; poursuivre par `docs/PROMPTS_PAR_PHASE.md`.
6. À chaque phase comparer logs tests, scène jouable, captures, build, temps, interventions et score de la grille. Ne pas faire travailler les deux agents sur le même dossier Unity.
7. Seul l'utilisateur valide les changements des règles du jeu, les dépenses, l'accès aux comptes et la publication.

## Règles de départ

3 files d'arrosoirs ; 3 places d'arrosage ; arrosoirs de couleurs et capacités ; fleurs en piles visibles une à une ; ciblage automatique déterministe ; annulation gratuite ; **aucun chronomètre**. 20 niveaux MVP validés via solveur. Ce kit contient les spécifications et visuels, **pas un projet Unity déjà construit**.

## Risque juridique

Concevoir des noms, graphismes, niveaux, animations, sons et UI originaux. Utiliser les jeux connus comme source de recherche de mécaniques, non comme bibliothèque d'assets à copier. Vérifier marques et noms avant publication.
