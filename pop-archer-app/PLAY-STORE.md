# Mettre Pop Archer sur le Play Store, pas à pas

Ce guide suit l'ordre de la Play Console. Les textes à copier sont dans `store/listing.md` (anglais) et `store/listing-translations.md` (français, espagnol, portugais du Brésil, allemand) ; les images dans `store/`.

## Ce qui est déjà prêt

- **L'appli** : `com.nazimdebieb.poparcher`, version 1.0 (`versionCode 2`), Android 7 à 16.
- **La clé d'envoi** (`pop-archer-upload.jks`, alias `poparcher`) et **l'AAB signé** (`pop-archer-1.0-2.aab`), envoyés à part. Garde la clé et son mot de passe **en lieu sûr, avec une copie** : il les faudra pour chaque mise à jour.
- **Les pubs** (AdMob) et **les achats** (Google Play Billing) sont branchés. Pour l'instant, les pubs utilisent les **identifiants de test de Google** : elles affichent « Test Ad » et ne rapportent rien. On les remplace par les tiens avant la sortie publique (étape 8).
- **Le consentement RGPD** : au premier lancement en Europe, le formulaire de consentement de Google s'affiche ; « Réglages → Choix de confidentialité » permet de changer d'avis.

## 1. Créer l'appli

Play Console → **Créer une application** :

- Nom : **Pop Archer** ;
- Langue par défaut : **anglais (États-Unis) – en-US** (les autres langues s'ajoutent comme traductions) ;
- Application ou jeu : **Jeu** ; Gratuit ou payant : **Gratuit** ;
- coche les déclarations, puis **Créer**.

## 2. Envoyer l'AAB en test interne

**Tester → Test interne → Créer une release** :

1. Accepte **Play App Signing** (Google garde la clé de l'appli ; ta clé d'envoi sert à prouver que les fichiers viennent de toi, et elle peut être remplacée si tu la perds).
2. Envoie `pop-archer-1.0-2.aab`. Nom de la release : `1.0` ; notes : « First test version ».
3. **Testeurs** : crée une liste avec ton adresse Gmail (celle de ton téléphone), enregistre, puis **Lancer le déploiement**.
4. Ouvre le lien d'invitation sur ton téléphone, accepte, et installe depuis le Play Store.

Le test interne ne passe pas par l'examen de Google : c'est le moyen le plus rapide d'essayer l'appli et les achats.

## 3. Créer les produits intégrés

Ils ne peuvent être créés qu'**après** l'envoi d'un AAB qui contient Billing (étape 2). **Monétiser → Produits → Produits intégrés → Créer un produit**. L'**ID produit** doit être exactement celui du tableau (le jeu les cherche par ces noms) :

| ID produit | Nom | Description | Prix |
|---|---|---|---|
| `c200` | Pouch | 200 coins | 0,99 € |
| `c750` | Chest | 750 coins | 2,99 € |
| `c2000` | Treasure | 2,000 coins | 6,99 € |
| `c5000` | Dragon hoard | 5,000 coins, best value | 14,99 € |
| `starter` | Starter pack | 1,000 coins, 3 boosts and the Royal outfit | 1,99 € |
| `pass` | Season pass | Bigger rewards, +3 max lives, gold outfit and arrow | 4,99 € |
| `piggy` | Piggy bank | Break the piggy bank and keep its coins | 1,99 € |
| `noads` | No ads | No more ads between levels | 3,99 € |
| `armory` | Armory pack | All 3 weapons + 500 coins | 2,99 € |

Pour chacun : prix en euros, puis **Mettre à jour les prix de conversion** (Google calcule les autres pays), **Enregistrer** et **Activer**. Le jeu affiche ensuite le prix de Google dans la devise du joueur.

Achats qu'on peut refaire : les pièces, la tirelire et le pass (une fois par saison). Achats définitifs, rendus après une réinstallation : sans pub, l'armurerie, la tenue du pack de départ.

**Tester sans payer** : **Paramètres (de la console) → Test de licence**, ajoute ton adresse Gmail. Tes achats dans l'appli de test seront marqués « Commande test » et ne te seront pas facturés.

## 4. La fiche du Play Store

**Croissance → Présence sur le Play Store → Fiche principale** :

- Nom, description courte, description complète : `store/listing.md` ; puis **Gérer les traductions** pour le français, l'espagnol, le portugais (Brésil) et l'allemand, avec `store/listing-translations.md` ;
- icône : `store/icon-512.png` ; bannière : `store/feature-1024x500.png` ;
- captures d'écran du téléphone : `store/screenshots/1.png` à `8.png`, dans l'ordre ;
- catégorie : **Arcade** ; tags : arcade, casual, shooter ;
- email de contact : ton **adresse dédiée** au jeu.

## 5. Contenu de l'appli (Politique → Contenu de l'appli)

- **Règles de confidentialité** : `https://nazimdebieb.github.io/nazimdebieb/privacy.html`. Pour la mettre en ligne : sur GitHub, **Settings → Pages**, Source **Deploy from a branch**, branche `claude/inspiring-babbage-byvelx`, dossier **/docs**, **Save** (ou la branche par défaut une fois la PR fusionnée). Avant, j'ajoute ton adresse de contact dans la page.
- **Annonces** : **Oui, l'appli contient des annonces**.
- **Accès à l'appli** : tout est accessible sans compte.
- **Public cible** : **13 ans et plus** (13-15, 16-17, 18+). Pas les tranches de moins de 13 ans : elles imposent le programme Familles, et la politique de confidentialité dit 13 ans et plus.
- **Classification du contenu** (questionnaire IARC) : catégorie Jeu ; violence **fantastique légère** (des flèches sur des bulles et des monstres de dessin animé, sans sang) ; pas de contenu sexuel, de langage grossier, de drogue ni de jeu d'argent ; **achats intégrés : oui** ; les joueurs ne communiquent pas entre eux ; pas de partage de position.
- **Sécurité des données** : le développeur ne collecte rien lui-même, mais le SDK Google Mobile Ads collecte des données pour les pubs. Réponses, d'après la page de Google sur le SDK Mobile Ads :
  - collecte des données : **Oui** ; chiffrées en transit : **Oui** ;
  - **Position approximative** (déduite de l'adresse IP) : collectée et partagée, pour la publicité, les statistiques et la prévention des fraudes ;
  - **Activité dans les applis → Interactions avec l'appli** : collectée et partagée, mêmes finalités ;
  - **Infos et performances de l'appli → Journaux de plantage, Diagnostic** : collectés et partagés, mêmes finalités ;
  - **Appareil ou autres identifiants** (identifiant publicitaire) : collecté et partagé, mêmes finalités ;
  - rien d'autre (pas de nom, email, contacts, photos, etc.). Les paiements sont faits par Google Play : le jeu ne voit pas les données de paiement.
- **Identifiant publicitaire** : **Oui, l'appli l'utilise**, pour la **publicité**.
- **Appli gouvernementale, actualités, santé, etc.** : Non.

## 6. Test fermé (obligatoire pour un compte personnel récent)

Si ton compte développeur est un compte **personnel créé après novembre 2023**, Google demande un **test fermé avec au moins 12 testeurs inscrits pendant 14 jours d'affilée** avant d'autoriser la production. Le tableau de bord de la console dit si c'est ton cas.

**Tester → Test fermé → Créer un canal** : envoie le même AAB, ajoute tes testeurs (une liste d'adresses Gmail ou un groupe Google), puis **Envoyer pour examen**. Le premier examen prend en général de quelques heures à quelques jours. Partage le lien d'inscription avec tes testeurs, et demande-leur de garder l'appli installée et d'y jouer pendant les 14 jours.

## 7. Compte AdMob

1. Crée un compte sur [admob.google.com](https://admob.google.com) avec le même compte Google, et remplis les infos de paiement et de taxe.
2. **Applis → Ajouter une appli** : Android, « publiée sur une plateforme d'applis » : oui, cherche Pop Archer (une fois l'appli en test, sinon ajoute-la à la main et associe-la plus tard).
3. Crée deux **blocs d'annonces** : un **Avec récompense** (nommé « Rewarded ») et un **Interstitiel** (« Interstitial »).
4. **Confidentialité et messages → RGPD** : crée et publie un message de consentement (c'est lui que le jeu affiche en Europe).
5. Envoie-moi l'**ID de l'appli** (`ca-app-pub-…~…`) et les **deux ID de blocs** (`ca-app-pub-…/…`).

## 8. Avant la sortie publique

Avec tes identifiants AdMob, je remplace ceux de test (`AD_UNITS` et `AD_TEST` dans `pop-archer/index.html`, l'ID d'appli dans `android/app/src/main/AndroidManifest.xml`), j'augmente `versionCode` à 3, et je refais l'AAB. Ne clique jamais sur tes propres vraies pubs : AdMob peut fermer le compte. Sur ton téléphone, ajoute-le comme **appareil de test** dans AdMob.

Ensuite : **Production → Créer une release** avec cet AAB, pays (tous, ou commence par quelques-uns), et **Envoyer pour examen**.
