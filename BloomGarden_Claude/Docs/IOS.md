# iOS — état et plan (P7-05)

Date : 2026-10-13

## Ce qui est prêt

- Code sans aucune dépendance à Android : pas de plugin natif, pas de `#if UNITY_ANDROID`. Sauvegarde (`Application.persistentDataPath`), presse-papiers (bouton « Copier » des données de test), sons synthétisés et zone sûre (encoche) fonctionnent aussi sur iOS.
- Réglages iOS enregistrés dans le projet (menu **Bloom Garden ▸ P7 - Reglages iOS**) : identifiant `com.nazimdebieb.bloomgarden.claude`, iOS 15.0 minimum, portrait uniquement, IL2CPP, numéro de build aligné sur la version Android.

## Ce qui bloque (vérifié le 2026-10-13)

| Bloqueur | Constat | Ce qu'il faut |
|---|---|---|
| Module Unity « iOS Build Support » | **Absent** : `PlaybackEngines` contient Android, WebGL et Windows ; `IsBuildTargetSupported(iOS)` = false | L'installer depuis Unity Hub (Installs ▸ 6000.6.5f1 ▸ Add modules ▸ iOS Build Support), environ 1 Go |
| Compilation et signature | Unity exporte un projet Xcode ; seul **Xcode sur macOS** produit l'application signée | Un Mac, ou un service de build dans le cloud (Unity Build Automation, Codemagic, GitHub Actions macOS) |
| Compte Apple | Distribution TestFlight / App Store | Apple Developer Program (99 $/an) : **dépense, accord explicite requis** |

## Étapes quand un Mac (ou un service) est disponible

1. Installer le module iOS, ouvrir le projet, **Build Profiles ▸ iOS ▸ Switch Platform**.
2. Menu **Bloom Garden ▸ P7 - Reglages iOS**, puis **Build** : Unity produit un dossier Xcode.
3. Sur le Mac : ouvrir `Unity-iPhone.xcodeproj`, choisir l'équipe Apple, lancer sur un iPhone branché, puis Archive ▸ TestFlight.
4. Rejouer la suite de tests PlayMode dans l'éditeur sur la cible iOS (aucun code spécifique attendu).

Je ne peux ni compiler ni tester iOS depuis ce PC Windows : aucune affirmation de fonctionnement sur iPhone n'est faite.
