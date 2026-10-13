# Proposition de monétisation (P8-03) — rien n'est intégré

Date : 2026-10-13. Le GDD interdit toute intégration de publicité, d'achats ou d'analytics tiers avant d'avoir prouvé la boucle de jeu, et sans ton accord explicite. **Ce document est une proposition ; aucun SDK n'a été ajouté.**

## Principes (GDD)

- Jamais de blocage artificiel pour vendre un « booster » ; « Annuler » reste gratuit et illimité.
- Pas de minuteur, pas de vies.
- Toute publicité est facultative et récompensée, jamais imposée au milieu d'une partie.

## Options, de la plus douce à la plus engageante

| Option | Description | Avantages | Risques | Travail |
|---|---|---|---|---|
| A. Achat unique « Soutenir le jardin » | 2 à 4 € : retire d'éventuelles pubs futures et débloque un thème décoratif | Respecte le joueur, simple | Revenus faibles sans audience | Google Play Billing ou Unity IAP 5, écran d'achat, restauration |
| B. Thèmes cosmétiques | Prairie / Étang / Zen déjà prêts ; vendre des thèmes supplémentaires (Crépuscule, Japon…) | Aucun impact sur la difficulté | Contenu à produire | Idem A + boutique de thèmes |
| C. Publicité récompensée facultative | Une vidéo pour un indice (montrer le prochain bon coup grâce au solveur) | Monétise sans faire payer | Consentement RGPD, déclaration des données, SDK lourd | LevelPlay ou AdMob, écran de consentement, mise à jour de la politique |
| D. Publicité interstitielle | Entre deux niveaux | Revenu par joueur plus élevé | Contraire à l'esprit « relaxant », nuit à la rétention | À éviter pour le MVP |

## Recommandation

Commencer par **A + B** après le test humain (P6), et seulement si la rétention est encourageante ; envisager **C** plus tard, avec écran de consentement. Chaque option demande : ton accord, un compte marchand Google, la mise à jour de la politique de confidentialité et du formulaire « Sécurité des données ».
