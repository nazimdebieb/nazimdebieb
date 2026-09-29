# Pop Archer · appli Android

Ce dossier emballe le jeu de `../pop-archer/` dans une vraie application Android, avec [Capacitor](https://capacitorjs.com). Le jeu lui-même ne change pas : il est copié tel quel dans l'appli, avec ses images et ses polices (Baloo 2 et Bungee, licence libre), pour marcher **sans connexion**.

- **Identifiant de l'appli** : `com.nazimdebieb.poparcher`. Il devient **définitif** dès le premier envoi sur la Play Console : à changer dans `capacitor.config.json` et `android/app/build.gradle` avant, si tu veux un autre nom.
- **Android** : fonctionne à partir d'Android 7 (API 24) et vise Android 16 (API 36).
- **Écran** : toujours en paysage, en plein écran. Les barres du système se cachent et reviennent en glissant depuis le bord.
- **Icône et écran de démarrage** : l'icône vient de Canva (`art/icon-1024.png`), l'écran de démarrage montre le logo sur le fond violet du jeu.

## Tester sur ton téléphone

À chaque changement du jeu poussé sur GitHub, l'action **Pop Archer Android** construit un APK de test :

1. Sur GitHub, ouvre l'onglet **Actions** du dépôt, puis la dernière exécution de **Pop Archer Android**.
2. En bas, dans **Artifacts**, télécharge **pop-archer-debug-apk** (un .zip qui contient `app-debug.apk`).
3. Envoie `app-debug.apk` sur ton téléphone et ouvre-le. Android demande d'autoriser l'installation depuis cette source (réglage « Installer des applis inconnues ») : accepte pour ce fichier.

Cet APK de test n'est pas celui du Play Store : il est signé avec une clé de test.

## Préparer la version Play Store

Le Play Store demande un fichier **AAB signé** avec **ta** clé. Cette clé prouve que les mises à jour viennent de toi.

1. **Crée ta clé une seule fois**, sur ton ordinateur (Java doit être installé) :

   ```sh
   keytool -genkeypair -keystore pop-archer-release.jks -alias poparcher \
     -keyalg RSA -keysize 2048 -validity 10000
   ```

   Garde le fichier `.jks` et ses mots de passe **en lieu sûr, avec une copie de secours**. Ne les mets jamais dans le dépôt.
2. **Donne la clé à GitHub**, dans **Settings → Secrets and variables → Actions → New repository secret** :
   - `POP_ARCHER_KEYSTORE_BASE64` : le fichier `.jks` encodé en base64 (`base64 -w0 pop-archer-release.jks` sous Linux, `base64 -i pop-archer-release.jks` sous macOS) ;
   - `POP_ARCHER_KEYSTORE_PASSWORD` : le mot de passe du fichier ;
   - `POP_ARCHER_KEY_ALIAS` : `poparcher` ;
   - `POP_ARCHER_KEY_PASSWORD` : le mot de passe de la clé.
3. Dès que ces secrets existent, l'action produit aussi **pop-archer-release-aab** : c'est le fichier à envoyer sur la Play Console. Active **Play App Signing** quand la console le propose.

## Construire sur un ordinateur (facultatif)

Il faut Node.js 22, Java 21 et le SDK Android (Android Studio l'installe).

```sh
npm ci
npm run build:debug      # APK de test : android/app/build/outputs/apk/debug/app-debug.apk
npm run build:release    # AAB signé, avec un fichier keystore.properties (voir plus bas)
```

Pour signer sur ton ordinateur, crée `keystore.properties` à côté de ce README (il est ignoré par git) :

```properties
storeFile=/chemin/vers/pop-archer-release.jks
storePassword=…
keyAlias=poparcher
keyPassword=…
```

À chaque nouvelle version envoyée au Play Store, augmente `versionCode` (1, 2, 3…) et `versionName` dans `android/app/build.gradle`.

## Fichiers

- `scripts/copy-web.mjs` : copie le jeu dans `www/` et remplace les polices Google par les copies locales de `fonts/`.
- `scripts/make-icons.mjs` : refait les icônes Android, l'écran de démarrage et les visuels de la fiche Play Store à partir de `art/icon-1024.png` (Playwright nécessaire).
- `store/` : visuels et textes de la fiche Play Store (icône, bannière, 7 captures légendées dans `store/screenshots/`) et la politique de confidentialité.
- `android/` : le projet Android généré par Capacitor. Modifications : paysage forcé (`AndroidManifest.xml`), plein écran (`MainActivity.java`), signature (`app/build.gradle`), couleurs de démarrage (`res/values/styles.xml`).
