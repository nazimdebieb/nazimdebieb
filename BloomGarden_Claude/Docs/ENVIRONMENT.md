# Environnement — diagnostic réel (2026-10-12, PC Windows)

## Où travaille cet agent

Depuis le 2026-10-12, Claude Code tourne **en local sur le PC Windows 11 de l'utilisateur** (application Claude Desktop, onglet Code). Dépôt cloné dans `C:\Users\nazim\Projets\BloomGarden`, branche `claude/awesome-einstein-rsj774`. La première partie (P0 partiel, P1) a été faite dans une session cloud ; voir l'historique Git.

| Élément | Résultat | Preuve |
|---|---|---|
| Unity Hub | Installé (`C:\Program Files\Unity Hub`) | processus `Unity Hub.exe` |
| Unity Editor | **6000.6.5f1** (révision 3ff58d469c8a) | `ProjectSettings/ProjectVersion.txt` |
| Modules Android | AndroidPlayer avec **SDK, NDK, OpenJDK** présents ; aussi WebGL et Windows Standalone | `Editor/Data/PlaybackEngines/AndroidPlayer/{SDK,NDK,OpenJDK}` |
| Packages | URP 17.6.0, Test Framework 1.8.0, uGUI 2.6.0 (versions fournies avec l'éditeur) ; IAP et Analytics retirés (hors MVP) | `Packages/manifest.json` |
| Unity CLI (batchmode) | **Opérationnel** : configuration P0 et tests EditMode exécutés sans ouvrir de fenêtre | `Logs/p0_setup.log` (non versionné), `Docs/QA/P1_unity_editmode_results.xml` |
| Plugin Claude Code | `unity@unity-agent-plugin` **0.1.8-beta**, scope utilisateur, activé | `claude plugin list` |
| MCP Unity | Pas encore démontré : le plugin se charge au démarrage d'une nouvelle session, et l'éditeur ouvert était celui du projet Codex | voir H6 dans `Docs/PLAN.md` |
| Git / GitHub CLI | Git 2.54.0, gh 2.101.0 (connecté au compte `nazimdebieb`) | `git --version`, `gh auth status` |
| Python | 3.11.15 (solveur de référence) | `python --version` |
| .NET SDK | Absent sur le PC ; inutile : les tests passent maintenant dans Unity Test Runner | — |
| Téléphone Android | Non vérifié (pas encore nécessaire avant P3-10) | — |
| iOS | Build natif impossible sans macOS/Xcode (ou service de build) | GDD ch. 17 |

## Commandes de référence (PowerShell)

```powershell
$u = "C:\Program Files\Unity\Hub\Editor\6000.6.5f1\Editor\Unity.exe"
$proj = "C:\Users\nazim\Projets\BloomGarden\BloomGarden_Claude"
# Configuration P0 (idempotente)
& $u -batchmode -quit -projectPath $proj -executeMethod BloomGarden.EditorTools.ProjectSetup.ConfigureAll -logFile "$proj\Logs\p0_setup.log"
# Tests EditMode
& $u -batchmode -projectPath $proj -runTests -testPlatform EditMode -testResults "$proj\Docs\QA\P1_unity_editmode_results.xml" -logFile "$proj\Logs\tests.log"
```

Le batchmode échoue si le même projet est déjà ouvert dans l'éditeur : dans ce cas, passer par le MCP.

## Point d'attention

`Packages/manifest.json` doit être en UTF-8 **sans BOM** : Windows PowerShell 5.1 en ajoute un avec `Set-Content -Encoding utf8`, et Unity refuse alors de résoudre les packages.

Sécurité Windows avait bloqué `UxmlCloneTreeGenerator.dll` (fourni avec Unity). Aucune erreur de compilation n'en a résulté lors des ouvertures du 2026-10-12.
