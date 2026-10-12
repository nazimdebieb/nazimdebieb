# Environnement — diagnostic réel (2026-10-12)

## Où travaille cet agent

Cette session Claude Code tourne dans un **conteneur Linux dans le cloud** (claude.ai/code), pas sur le PC Windows de l'utilisateur. Elle n'a **aucun accès** à Unity Hub, à l'éditeur Unity, au plugin Unity MCP ni au téléphone de l'utilisateur.

| Élément | Résultat | Preuve |
|---|---|---|
| Unity Hub / Editor 6+ | **Non vérifiable d'ici** (installé sur le PC selon l'utilisateur, version exacte inconnue) | — |
| Modules Android (SDK/NDK/OpenJDK) | **Non vérifiable d'ici** | — |
| Plugin `unity@unity-agent-plugin` / Unity CLI / MCP | **Non connecté** : le MCP Unity tourne en local et ne peut pas être joint depuis le cloud | — |
| Git | OK, dépôt `nazimdebieb/nazimdebieb`, branche de travail `claude/awesome-einstein-rsj774` | `git log` |
| Python | 3, stdlib : solveur de référence exécuté, 20/20 niveaux solvables | sortie dans `Docs/QA/P0.md` |
| .NET SDK | 8.0.425 installé dans le conteneur pour compiler et tester `Core` (C# 9) | `Docs/QA/P1_dotnet_test.log` |
| iOS | Build natif impossible sans macOS/Xcode (ou service de build) | GDD ch. 17 |

## Signalé par l'utilisateur sur son PC

Sécurité Windows (Contrôle intelligent des applications) a bloqué `UxmlCloneTreeGenerator.dll` chargé par « .NET Host ». Ce fichier est fourni avec Unity 6 et sert à la compilation des scripts UI Toolkit. Si la console Unity ne montre aucune erreur de compilation, le blocage n'a pas d'effet sur le projet. Sinon : voir la section « Interventions » de `Docs/PLAN.md`.

## Pour que Claude pilote Unity (MCP)

Il faut une session Claude Code **sur le PC** (application Claude Desktop, ou `claude` dans un terminal ouvert dans le dossier `BloomGarden_Claude`), puis :

```text
/plugin marketplace add Unity-Technologies/unity-agent-plugin
/plugin install unity@unity-agent-plugin
```

Unity doit être ouvert sur le projet. La session locale peut ensuite reprendre ce dépôt (même branche) et faire les vérifications Unity marquées « à faire » dans `Docs/PLAN.md`.
