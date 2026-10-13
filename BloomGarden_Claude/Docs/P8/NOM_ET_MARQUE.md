# Nom et marque (P8-01)

Date : 2026-10-13 · Recherche web simple, **pas un avis juridique**.

## Constat : « Bloom Garden » est déjà utilisé

- Un jeu de puzzle **« Bloom Garden »** (tri de fleurs) de **PIXON PTE. LTD.** est publié sur iOS et Android, noté 4,74/5 (882 avis), gratuit avec achats intégrés : [App Store](https://apps.apple.com/app/id6748257880). Même catégorie (puzzle de fleurs relaxant) : **risque élevé de confusion, de refus par le store ou de litige** si nous publions sous ce nom.
- Autres jeux proches : « Blossom Garden Match 3 », « Bloom Garden: Merge & Grow! », « Blooming Garden ».
- Aucune marque « Bloom Garden » trouvée par recherche web aux registres USPTO / EUIPO, mais plusieurs marques « BLOOM » existent dans d'autres classes. Une recherche officielle reste nécessaire.

## Recommandation

Garder « Bloom Garden » comme **nom de travail** (comparaison avec Codex), mais **choisir un autre nom avant toute publication** (D-005 était provisoire). Pistes sans résultat évident lors d'une recherche web rapide : **« Petal Pour »**, **« Drip & Bloom »**. À vérifier par toi :

1. Recherche de marque sur [EUIPO eSearch](https://euipo.europa.eu/eSearch/), [INPI](https://data.inpi.fr/) et [USPTO](https://tmsearch.uspto.gov/), classes 9 (logiciels de jeu) et 41 (divertissement).
2. Recherche dans Google Play et l'App Store.
3. Disponibilité d'un nom de domaine si besoin.

Changer le nom se fait en deux endroits : la clé `app_title` dans `Scripts/Presentation/Loc.cs` et `PlayerSettings.productName` dans `Editor/ProjectSetup.cs`, puis relancer `StoreArt.RenderAll()` pour l'icône et la bannière.

## Licences

Voir `Docs/LICENSES.md` : aucun asset tiers ; modèles, matériaux et sons créés par code ; police intégrée à Unity.

Sources : [Bloom Garden (PIXON) sur l'App Store](https://apps.apple.com/app/id6748257880), [Blossom Garden Match 3](https://apps.apple.com/us/app/id1240749176), [Blooming Garden](https://www.appgoblin.info/apps/6763208641).
