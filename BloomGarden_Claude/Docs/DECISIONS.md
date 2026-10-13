# Journal des décisions — Bloom Garden (copie Claude)

Décisions héritées du kit (non modifiées) :

| ID | Statut | Décision | Date | Justification |
|---|---|---|---|---|
| D-001 | VERROUILLÉE | Jeu puzzle 2,5D portrait sans limite de temps | 2026-10-12 | Expérience relaxante |
| D-002 | VERROUILLÉE | 3 files, 3 places, 4 couleurs, arrosage automatique | 2026-10-12 | Prototype comparable |
| D-003 | VERROUILLÉE | Undo gratuit, victoire par fleurs toutes écloses | 2026-10-12 | Éviter frustration |
| D-004 | VERROUILLÉE | Unity 6+, C# logique pure, Android puis iOS | 2026-10-12 | Déploiement et test |
| D-005 | PROVISOIRE | Nom « Bloom Garden » | 2026-10-12 | Vérification marque nécessaire |
| D-006 | PROVISOIRE | UI Toolkit ou UI classique selon preuve de stabilité | 2026-10-12 | Arbitrage documenté par agent |
| D-007 | REPORTÉE | Boutique, publicité, achats | 2026-10-12 | Après test du gameplay |

Décisions secondaires prises par Claude (aucune ne change une règle verrouillée) :

| ID | Statut | Décision | Date | Options envisagées | Impact | Validation utilisateur |
|---|---|---|---|---|---|---|
| D-101 | PRISE | Places à position fixe (gauche → droite) avec un numéro d'ordre de sélection ; l'ancienneté décide de la priorité d'arrosage | 2026-10-12 | Liste compactée comme le Python | Même résultat que le Python (vérifié sur 1242 coups) ; la vue sait dans quelle place dessiner chaque arrosoir | Non requise |
| D-102 | PRISE | Blocage déclaré aussi quand des places sont libres mais toutes les files sont vides (aucun coup possible) | 2026-10-12 | Ne déclarer que « 3 places pleines » | Impossible avec des niveaux à eau = fleurs, mais évite qu'une partie reste sans issue ni message | Non requise |
| D-103 | PRISE | Une sélection est refusée avec une raison (`InvalidLane`, `GameOver`, `LaneEmpty`, `SlotsFull`) et l'état reste identique | 2026-10-12 | Exceptions | La vue peut donner un retour discret sans planter | Non requise |
| D-104 | PRISE | Lecteur JSON maison (`MiniJson`) dans `Core`, sans dépendance | 2026-10-12 | `JsonUtility` (ne lit pas les tableaux imbriqués `cells`), Newtonsoft (package en plus) | `Core` reste du C# pur, testable hors Unity | Non requise |
| D-105 | PRISE | Toute la logique pure (modèle, niveaux, règles) dans une seule assembly `BloomGarden.Core` (`noEngineReferences`) sous `Scripts/Core/{Model,Levels,Rules,Json}` | 2026-10-12 | Une assembly par dossier du GDD | `Scripts/Levels` est réservé au chargement côté Unity (TextAsset) | Non requise |
| D-106 | PRISE | Tests EditMode aussi exécutables hors Unity avec `dotnet test` (`Tools/DotnetTests`, C# 9 comme Unity 6) | 2026-10-12 | Seulement Unity Test Runner | Preuve de tests disponible sans éditeur ; le passage dans Unity Test Runner reste à faire | Non requise |
| D-107 | **TRANCHÉE** | 19 niveaux de référence sur 20 n'ont **aucun** état de blocage atteignable (n'importe quelle suite de taps gagne) | 2026-10-12 | **Garder les 20 niveaux tels quels pour le MVP1** (décision de l'utilisateur) : Claude et Codex travaillent sur le même contenu pour une comparaison équitable. Les pièges (niveaux avec choix perdants) sont reportés au **MVP2**. | P2-06 se limite à documenter et vérifier la progression pédagogique, sans modifier les niveaux ; le rapport P2-07 signale les niveaux « triviaux » | Validée par l'utilisateur |
| D-108 | PRISE | P3 en primitives Unity (sphères, cylindres) générées par code, palette du GDD en matériaux URP Lit | 2026-10-12 | Modèles 3D importés / sprites 2D | Rendu remplaçable en P4 sans toucher la logique | Non requise (GDD : « commencer avec des primitives ») |
| D-109 | PRISE | HUD en uGUI avec la police intégrée (`LegacyRuntime.ttf`), construit par code | 2026-10-12 | UI Toolkit (D-006), TextMeshPro | Évite l'import de TMP Essentials et le risque `UxmlCloneTreeGenerator.dll` ; à revoir en P5 (localisation) | Non requise |
| D-110 | PRISE | Entrées via l'ancien Input Manager (`Input.GetMouseButtonDown`, compatible tactile) | 2026-10-12 | Package Input System | Unity signale l'Input Manager comme « marqué pour dépréciation » : migration à planifier avant la sortie | Non requise |
| D-111 | PRISE | Une forme blanche par couleur sur boutons et arrosoirs : corail = point, bleu = losange, jaune = barre, violet = double point | 2026-10-12 | Motifs de texture | Accessibilité daltonisme (GDD ch. 03 et 12) | Non requise |
| D-112 | PRISE | Package `com.unity.pipeline` (0.8.0-exp.1) ajouté pour piloter l'éditeur ; APK de test construits **sans** Development Build | 2026-10-12 | Pas de pilotage en direct | Le package est exclu des builds non-développement : aucun serveur de débogage dans l'APK | Non requise |
| D-113 | PRISE | Sons synthétisés par code au démarrage (`GardenAudio`), aucun fichier audio | 2026-10-12 | Banque de sons gratuite / payante | Aucune licence à suivre ; notes d'éclosion sur une gamme pentatonique | Non requise |
| D-114 | PRISE | Réglages son / vitesse ×2 / animations réduites stockés dans PlayerPrefs | 2026-10-12 | Attendre la sauvegarde P5 | Provisoire : la sauvegarde versionnée de P5 les reprendra | Non requise |
| D-115 | PRISE | Ombres douces de la lumière principale (1 cascade, 1024 px) et MSAA 2× | 2026-10-12 | Sans ombres (P3) | Effet diorama ; coût GPU à mesurer sur téléphone (P7-03), réductible en mode bas de gamme (P7-04) | Non requise |
| D-116 | PRISE | Sauvegarde JSON versionnée (`SaveService`, version 1) dans `persistentDataPath` : écriture temporaire + sauvegarde de secours ; la partie en cours est stockée comme la liste des files jouées et rejouée à la reprise | 2026-10-12 | Sérialiser l'état complet | Reprise exacte avec l'historique d'annulation, fichier minuscule ; dépend du déterminisme des règles (déjà testé) | Non requise |
| D-117 | PRISE | Niveaux débloqués un par un (gagner le niveau N ouvre N+1) ; meilleur nombre de coups conservé | 2026-10-12 | Tout ouvert | Progression du GDD ch. 08 ; aucune étoile ni monnaie (MVP) | Non requise |
| D-118 | PRISE | Tutoriel en bulles sur les niveaux 1 à 4 (taper, arrosage automatique, 3 places, annuler gratuit, fleurs cachées), affiché jusqu'à la première réussite du niveau | 2026-10-12 | Écrans d'explication bloquants | Ne bloque jamais un coup ; la bulle ne masque ni fleur ni tête de file | Non requise |
| D-119 | PRISE | Table de traduction FR/EN en code (`Loc`), langue par défaut = langue de l'appareil | 2026-10-12 | Package Unity Localization | Pas de dépendance ; à migrer si plus de langues sont ajoutées | Non requise |
| D-120 | PRISE | Instrumentation de test locale (`PlaytestLog`) : fichier JSONL dans le dossier de l'app, schéma fermé, identifiant de session aléatoire ; partage volontaire par le bouton « Copier » | 2026-10-13 | SDK d'analytics (Firebase, Unity Analytics) | Aucun réseau, aucune donnée personnelle (GDD ch. 15) ; à remplacer par un vrai service seulement en P8, avec consentement | Non requise |
| D-121 | PRISE | Cible 60 i/s ; graphismes auto / élevés / économie (économie : sans ombres ni MSAA, moins de particules) ; auto bascule en économie sous 40 i/s pendant 10 s, jamais dans l'éditeur | 2026-10-13 | Garder 30 i/s par défaut d'Android | Fluidité ; aucun effet sur les règles (test) | Non requise |
| D-122 | PRISE | Thèmes de décor Prairie / Étang / Zen selon le champ `theme` des niveaux ; parcelles de terre identiques | 2026-10-13 | 4 thèmes du GDD (alpha) | Même gameplay, variété visuelle ; Crépuscule et Japon restent pour plus tard | Non requise |
| D-123 | PRISE | 30 niveaux candidats MVP2 avec vrais pièges dans `LevelsCandidates/`, **hors du jeu livré**, `reviewed: false` | 2026-10-13 | Les mettre dans le catalogue | Respecte D-107 (MVP1 = niveaux du kit) ; revue humaine obligatoire avant publication | Non requise |
| D-124 | **À DÉCIDER** | Nom « Bloom Garden » déjà utilisé par un jeu de puzzle de fleurs publié (PIXON) | 2026-10-13 | Garder / « Petal Pour » / « Drip & Bloom » / autre | Risque de refus ou de litige à la publication ; aucun impact sur la comparaison Codex | **Oui : avant toute publication** |
| D-125 | PRISE | Statistiques matérielles Unity (`submitAnalytics`) désactivées ; icône rendue depuis les pièces du jeu (icône adaptative Android) | 2026-10-13 | Garder les valeurs par défaut | Aucun envoi de données ; identité visuelle cohérente | Non requise |
