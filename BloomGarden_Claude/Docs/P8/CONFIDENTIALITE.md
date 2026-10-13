# Confidentialité et conformité des données (P8-02)

Date : 2026-10-13 · Version du jeu : 0.7.0

## Inventaire des SDK et des envois réseau

| Élément | Présent ? | Données | Envoi réseau |
|---|---|---|---|
| SDK publicitaire (AdMob, LevelPlay…) | Non | — | Non |
| SDK d'analytics (Firebase, Unity Analytics, GameAnalytics…) | Non | — | Non |
| Achats intégrés (Unity IAP, Google Play Billing) | Non (retiré en P0) | — | Non |
| Services Unity (Analytics, Crash Reporting, Performance Reporting) | Désactivés (`UnityConnectSettings.asset`) | — | Non |
| Statistiques matérielles Unity (`submitAnalytics`) | **Désactivé** le 2026-10-13 | — | Non |
| Package `com.unity.pipeline` (outil de développement) | Exclu des builds non-développement | — | Non |
| Journal de test local (`PlaytestLog`) | Oui | Niveaux joués, choix, temps, réglages, fluidité, mémoire, RAM arrondie au Go, identifiant de session aléatoire | **Non** : copie volontaire par le joueur |
| Sauvegarde locale | Oui | Progression, records, options, partie en cours | Non |

Permissions Android : uniquement celles ajoutées par défaut par Unity ; aucune permission sensible (localisation, contacts, caméra, micro). À revérifier dans le manifeste fusionné de l'AAB final.

## Formulaire « Sécurité des données » de Google Play (réponses proposées)

- L'application collecte-t-elle ou partage-t-elle des données utilisateur ? **Non.** Le journal de test ne quitte l'appareil que si l'utilisateur le copie lui-même ; des données qui restent sur l'appareil ne sont pas considérées comme « collectées ».
- Chiffrement en transit : sans objet (aucune transmission).
- Suppression des données : désinstaller l'application supprime tout ; le bouton « Effacer » supprime le journal de test.

À mettre à jour **avant** toute intégration future de publicité, d'analytics ou d'achats (voir `MONETISATION.md`).

## Politique de confidentialité (projet, FR)

> **Bloom Garden — Politique de confidentialité**
> Bloom Garden ne collecte aucune donnée personnelle et n'envoie aucune donnée sur Internet. Votre progression, vos options et, si vous l'utilisez, un journal de test anonyme (niveaux joués, coups, temps, fluidité) sont enregistrés uniquement sur votre téléphone. Vous pouvez effacer le journal de test dans Options ▸ Données de test, et tout supprimer en désinstallant le jeu. Le jeu ne contient ni publicité, ni achat intégré, ni service tiers. Contact : [adresse e-mail à compléter].

## Privacy policy (draft, EN)

> **Bloom Garden — Privacy Policy**
> Bloom Garden does not collect personal data and sends nothing over the Internet. Your progress, settings and, if you use it, an anonymous playtest log (levels played, moves, time, smoothness) are stored only on your phone. You can clear the playtest log in Options ▸ Playtest data, and remove everything by uninstalling the game. The game contains no ads, no in-app purchases and no third-party services. Contact: [email address to fill in].

Google Play exige une URL publique pour la politique : à héberger par toi (page web, Google Sites…). **Publication : accord requis.**
