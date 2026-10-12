# Protocole d'observation — Bloom Garden (P6-02)

Basé sur `Kit/benchmark/PROTOCOLE_TEST_HUMAIN.md`. Objectif : **trouver des problèmes concrets**, pas mesurer des pourcentages. Avec 5 personnes, on ne tire pas de conclusion statistique.

## Avant la séance

1. Installer l'APK du jeu sur le téléphone du testeur (ou prêter le tien). Version à noter dans la fiche.
2. Si c'est une **comparaison Claude / Codex** : vérifier que les deux versions sont à la même phase, renommer les APK **A** et **B** sans dire d'où ils viennent, et inverser l'ordre une fois sur deux. 10 minutes par version.
3. Pour un vrai premier contact : dans le jeu, **Options → Données de test → Effacer** (n'efface pas la progression, seulement les statistiques), et utiliser un téléphone où le jeu n'a jamais été lancé, ou désinstaller / réinstaller l'APK.
4. Ouvrir `fiche_observation.csv` (une ligne par testeur).

## Pendant la séance (10 minutes)

- Dire seulement : « C'est un petit jeu de jardin, joue comme tu veux. Pense à voix haute si tu peux. »
- **Ne pas expliquer les règles.** Si la personne est bloquée plus d'une minute, aider le minimum et **noter l'intervention** (c'est un défaut du tutoriel, pas du testeur).
- Noter : temps avant le premier tap, gestes qui ne font rien (taper une fleur, un arrosoir qui n'est pas en tête de file…), utilisation d'Annuler, premier « ah, j'ai compris ! », lisibilité des couleurs et des gouttes.

## Après la séance

Poser les 5 questions du kit et noter les réponses telles quelles :

1. Quelle version recommanderais-tu ? (comparaison A/B seulement)
2. Quelle est la règle du jeu, en une phrase ?
3. Quelles animations as-tu préférées ?
4. Aurais-tu envie de refaire un niveau ?
5. À quel moment étais-tu perdu(e) ?

Puis récupérer les données de jeu : **Options → Données de test → Copier les données**, coller dans un message (WhatsApp, e-mail…) et l'envoyer. Enregistrer chaque message reçu dans un fichier texte `Docs/P6/donnees/<prénom-ou-numéro>.txt` (un fichier par testeur ; un pseudonyme suffit).

## Rapport

```bash
python Tools/python/playtest_report.py Docs/P6/donnees > Docs/P6/rapport.md
```

Le rapport donne, par niveau : qui a commencé et gagné, abandons, blocages, annulations, temps et coups médians ; puis où chaque testeur s'est arrêté et le temps avant le premier tap. Les corrections (P6-04) se décident **avec les fiches d'observation**, pas avec les chiffres seuls.

## Ce que le jeu enregistre (et ce qu'il n'enregistre pas)

Enregistré sur le téléphone uniquement, fichier `bloomgarden_playtest.jsonl` : début et fin de niveau, chaque arrosoir choisi (file, couleur, capacité, temps écoulé), annulations, recommencements, blocages, abandons (menu, autre niveau, fermeture), bulles du tutoriel affichées, réglages modifiés, et un identifiant de session tiré au hasard à chaque lancement.

**Pas enregistré :** nom, contact, modèle ou identifiant du téléphone, identifiant publicitaire, position. **Rien n'est envoyé** par le jeu : c'est le testeur qui copie et envoie ses données s'il le souhaite.
