# INSTALLATION UNITY + MCP — WINDOWS (OCTOBRE 2026)

## 1) Installation moteur

1. Installer **Unity Hub** depuis https://unity.com/download.
2. Installer une version **Unity 6** stable compatible, idéalement LTS si proposée. Conserver **strictement la même version** dans les deux essais.
3. Dans les modules de l'éditeur, cocher **Android Build Support**, **Android SDK & NDK Tools** et **OpenJDK**. Le support iOS n'est nécessaire que pour la chaîne iOS et ne remplace pas macOS/Xcode pour les builds natifs.
4. Créer un projet URP 3D léger `BloomGarden_Codex` et une copie isolée `BloomGarden_Claude` depuis le même starter, ou laisser les agents le faire selon les droits accordés.
5. Initialiser Git et ajouter le `.gitignore` Unity ; ne pas versionner `Library/`, `Temp/`, `Logs/`, `Obj/`, les builds lourds ni les secrets.

## 2) Connecter Claude Code

Dans une session Claude Code (commandes slash ; **pas du PowerShell tel quel**) :

```text
/plugin marketplace add Unity-Technologies/unity-agent-plugin
/plugin install unity@unity-agent-plugin
```

Vérifier `/unity:` et `/plugin`. Si nécessaire relancer la session. Les commandes équivalentes depuis un terminal sont :

```powershell
claude plugin marketplace add Unity-Technologies/unity-agent-plugin
claude plugin install unity@unity-agent-plugin
```

## 3) Connecter Codex

Depuis un terminal où la CLI Codex est installée :

```powershell
codex plugin marketplace add Unity-Technologies/unity-agent-plugin
codex plugin add unity@unity-agent-plugin
codex plugin list
```

Démarrer une nouvelle session pour charger les compétences, vérifier `/unity:` dans la session. L'accès réel à l'éditeur repose sur Unity CLI / MCP, selon les outils disponibles ; le plugin peut vérifier et aider à installer/configurer ces composants. Ne pas supposer que les compétences seules donnent accès à une instance Unity non ouverte.

## 4) Quel type de connexion ?

**MCP + Unity CLI** est la voie prioritaire : l'agent peut interroger le projet ouvert, modifier scènes/objets, exploiter les erreurs, lancer des tests/builds selon les capacités activées. Une « API Unity distante » ou une clé API générique n'est pas nécessaire pour développer localement dans l'éditeur. Les scripts C# restent la base du jeu.

**Limite importante :** ChatGPT dans ce chat n'est pas connecté à ton ordinateur et ne peut pas cliquer dans ton Unity local. Codex et Claude Code locaux peuvent le faire une fois autorisés et configurés. Réserve les confirmations manuelles aux opérations sensibles, aux publications et aux achats.

## 5) Tests Android / iOS

- Android : depuis Unity, installer modules, paramétrer signature et `versionCode` uniquement dans un environnement sécurisé, générer un APK de test puis un AAB pour Play Store selon ses exigences courantes. Tester réellement sur smartphone.
- iOS : il faut macOS/Xcode pour la compilation/signature native iOS, ou un service compatible pour la chaîne de build ; un ordinateur Windows seul ne produit pas le binaire final App Store par magie.
- Placer les secrets de signature hors Git. Faire des sauvegardes avant toute action de MCP qui modifie beaucoup d'assets.

## 6) Diagnostic demandé aux agents

Exiger une réponse factuelle : version Unity, Hub, CLI/MCP, plugin, capacité d'inspection scène, plateforme de build disponible, erreurs, accès au téléphone, méthodes de test réelles et liste des bloqueurs.

## Sources officielles

- Unity plugin : https://docs.unity.com/en-us/ai/unity-plugin/about-unity-plugin
- Claude Code : https://docs.unity.com/en-us/ai/unity-plugin/claude-code
- Codex : https://docs.unity.com/en-us/ai/unity-plugin/codex
- GitHub Unity : https://github.com/Unity-Technologies/unity-agent-plugin
- Unity CLI : https://docs.unity.com/en-us/ai/unity-cli
- Téléchargement : https://unity.com/download
