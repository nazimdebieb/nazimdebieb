# Color-mixing mobile games and whether "Mix Rush" already exists (state at 2026-09-28)

## Q1. Which mobile games (esp. 2024-2026) use color mixing as a core mechanic, and what traction do they have?

### Takeaway
Color mixing has produced one genuine mass-market hit (Supersonic/Garawell's "Color Match"/"Coloring Match", 2021-22, #1 overall iOS US, 50M+ Google Play installs) plus a cluster of CrazyLabs/Geisha Tokyo "mix the shade" ASMR simulators with 10M+ installs each; all are 2019-2023 hypercasual "match-the-target-shade" games, not logic puzzles. Every color-mixing *puzzle* released 2024-2026 that I could find (roughly 30 titles, many launched in the last 9 months) has negligible traction (from 0 to about 2,700 US ratings, and mostly under 10K installs).

### Cited Findings

#### How store data was collected (applies to all store figures below)
- Google Play figures come from the US listing on 2026-09-28, pulled with the open-source `google-play-scraper` library. They cover 51 search queries (the top ~20-30 results each), plus a detail fetch for 550 apps. "Installs" is the public range; the "~N" figure next to it is the exact count exposed in the listing data (`realInstalls`). Apple figures come from the official iTunes Search/Lookup API (US storefront, plus FR where noted) on 2026-09-28, with up to 200 results per query, and "US ratings" is `userRatingCount` for the US storefront. Store URLs are given per app so the figures can be re-checked.

#### Tier 1: mass-market "mix the shade" hits (all pre-2024, still live and updated in 2026)
- **Color Match / Coloring Match**: developed by Garawell Games (Turkey) and published by Supersonic Studios. Google Play: released Aug 6, 2021, 50,000,000+ installs (~79.5M), 562,301 ratings, 4.39, updated Sep 25, 2026, IAP + ads. Loop: "Mix colors, paint 3D objects, and match the perfect shade!" — [Google Play](https://play.google.com/store/apps/details?id=com.JacobVanHaag.ColorMatch). The iOS version is "Coloring Match": released 2021-10-05, v3.90 updated 2026-09-17, 505,814 US ratings and 34,207 FR ratings — [App Store](https://apps.apple.com/us/app/coloring-match/id1586980403). The Play package is named "JacobVanHaag", but Supersonic credits Garawell Games and its CEO Nebih Başaran — [Supersonic case study](https://supersonic.com/learn/case-studies/color-match/).
- **Makeup Kit - Color Mixing** (CrazyLabs): Google Play released Jun 20, 2022, 10,000,000+ (~47.0M), 372,272 ratings, updated May 20, 2026 — [Google Play](https://play.google.com/store/apps/details?id=com.apetrus.makeup). iOS: 2022-06-21, 256,547 US ratings and 12,761 FR ratings — [App Store](https://apps.apple.com/us/app/makeup-kit-color-mixing/id1629429171).
- **France/EU:** the FR App Store search "color mixing" is led by Coloring Match (34,207 FR ratings), Makeup Kit - Color Mixing (12,761) and Eye Color Mix (8,218). "mélange couleurs" surfaces only tiny 2026 titles (Dollop, Huefall). No France-specific color-mixing hit exists (iTunes Search API, FR storefront, 2026-09-28).
- **Eye Color Mix** (CrazyLabs): Google Play released Sep 23, 2023, 10,000,000+ (~35.4M), 238,303 ratings, updated Aug 27, 2026. Pitch: "Start painting, match paint, merge colors" — [Google Play](https://play.google.com/store/apps/details?id=com.alexp.eyemix). iOS: 2023-09-26, 132,685 US ratings and 8,218 FR ratings — [App Store](https://apps.apple.com/us/app/eye-color-mix/id6467606448).
- **Mix & Paint** (Geisha Tokyo): Google Play released Feb 13, 2020, 10,000,000+ (~30.8M), 40,004 ratings, ads only (no IAP), updated Jun 5, 2026 — [Google Play](https://play.google.com/store/apps/details?id=com.mix.color.paint.game). iOS: 2019-12-18, 87,256 US ratings. Loop: "Use your fingers and the palette to mix the colors, then paint a picture based on the sample" — [App Store](https://apps.apple.com/us/app/mix-paint/id1492035864).
- **I Can Paint - Art your way** (Coco Play by TabTale / CrazyLabs): Aug 14, 2020, 10,000,000+ (~23.4M), "Mix and paint" — [Google Play](https://play.google.com/store/apps/details?id=com.crazylabs.i.can.paint).
- **Panda Game: Mix & Match Colors** (BabyBus, educational for kids): Jul 15, 2020, 10,000,000+ (~18.4M) — [Google Play](https://play.google.com/store/apps/details?id=com.sinyee.babybus.art).

#### Tier 2: publisher-backed color-mixing puzzles with modest traction
- **Play Colors** (Supersonic Studios): Google Play released Feb 3, 2023, 1,000,000+ (~3.59M), 9,868 ratings, **3.44 score**, updated Apr 14, 2026, "Create the colors!" — [Google Play](https://play.google.com/store/apps/details?id=com.blueberrygames.gamel). iOS: 2023-02-22, 7,933 US ratings, "Color mixing puzzle!" — [App Store](https://apps.apple.com/us/app/play-colors/id1672319479).
- **Color Merge Puzzle** (Supersonic Studios): Google Play released May 31, 2023, 1,000,000+ (~1.17M), 11,096 ratings, **last updated Oct 22, 2025** — [Google Play](https://play.google.com/store/apps/details?id=com.Rastegar.ColorMergePuzzle). iOS: 2023-06-24, 3,823 US ratings — [App Store](https://apps.apple.com/us/app/color-merge-puzzle/id6449178197).
- **Mix Colors!** (Tokyo Smart Games): 2019, 500,000+ (~897K), 3,013 ratings, **2.85 score** — [Google Play](https://play.google.com/store/apps/details?id=jp.aiayatsuji.mixcolors). iOS: 23,533 US ratings, last iOS update 2021-05-13 — [App Store](https://apps.apple.com/us/app/mix-colors/id1476681691).
- **tint.** (Lykkegaard, premium): 2019-09-19, 8,749 US ratings. "Mix watercolors to match the color of the origami" — [App Store](https://apps.apple.com/us/app/tint/id1451786388).
- **Silicone Color Match** (Lion Studios): 2022-05-20, stuck at **v0.83** with last update 2022-09-01, 129 US ratings, 3.26 average. Slime color mixing to match — [App Store](https://apps.apple.com/us/app/silicone-color-match/id1624494991).
- **Water Pour Puzzle: Color Jam** (dev "澳军 魏"): iOS 2026-01-26, 2,746 US ratings, 4.69. Water sort in which "Pour specific colors together to create new hues" — [App Store](https://apps.apple.com/us/app/water-pour-puzzle-color-jam/id6757102070). This is the best-performing 2026 color-mixing puzzle I found.

#### Tier 3: 2024-2026 indie color-mixing puzzles (all small)
- **Mazeiro: Color Mixing Puzzle** (ERI MUNAKATA, iOS only): released 2026-07-31, v1.0.1 on 2026-08-05, **0 US ratings**. "Red, yellow, and blue. Mix the three primary colors to create exactly the color each customer ordered… Tap a cup to pour paint into another cup… tap the order card to deliver it", 6 chapters and 120 levels, ad-supported — [App Store](https://apps.apple.com/us/app/mazeiro-color-mixing-puzzle/id6791533011).
- **Hex Mixer: Color Mixing Puzzle** (Potato Battery Games): Google Play released Dec 17, 2025, 1,000+ (~4.9K). iOS: 2025-12-18 (2 US ratings). Match target colors by adding drops from a ten-color palette, with a daily puzzle — [Google Play](https://play.google.com/store/apps/details?id=com.potatobatterygames.hexmixer), [App Store](https://apps.apple.com/us/app/hex-mixer-color-mixing-puzzle/id6756682061).
- **Paint Match** (Ombosoft): an open-source React.js game on itch.io, Kongregate, Android and iOS. "Recreate the target color… using the provided droplets of paint", with primaries plus white/black and no ads — [GitHub](https://github.com/Ombosoft/paint-match), [itch.io](https://ombosoft.itch.io/paint-match). Google Play "Paint Match Offline": Jul 20, 2023, 5,000+ (~9.9K), 49 ratings, updated Sep 6, 2026 — [Google Play](https://play.google.com/store/apps/details?id=com.ombosoft.paintmatch).
- **Color Spark — Mix, Match & Sort** (MEGARAMA): Google Play Mar 14, 2024, 10,000+ (~16.8K). "Tap a test tube to pour its color into the mixing area. Add a second watercolor… to create the targeted shade", with limited moves — [Google Play](https://play.google.com/store/apps/details?id=com.water.color.mixing). iOS: 25 US ratings — [App Store](https://apps.apple.com/us/app/color-spark-mix-match-sort/id6478590498).
- **PureFill: Color Mixing Puzzle** (Natvas Studio): May 28, 2025, 1,000+. "Guide falling colorful drops into jars to complete customer paint orders… Pop unwanted drops and mix colors" — [Google Play](https://play.google.com/store/apps/details?id=com.natvasstudio.purefill).
- **Kalabux: Paint Mixing Puzzle** (JollyGoodApps): Aug 30, 2026, 100+. You lay pipes carrying red, yellow and blue from vats to mix orders in a steampunk factory; black and white tanks are researched later for shades and tints — [Google Play](https://play.google.com/store/apps/details?id=com.jollygoodapps.kalabux).
- **Primary Color** (dpdmb-creation): Apr 16, 2026, 100+. You place primaries around target squares so their neighbors compose the target, under a time limit — [Google Play](https://play.google.com/store/apps/details?id=com.dpdmb.creation.primarycolor).
- **Color Mix Match** (Yeti Game Studio): Mar 29, 2025, 1,000+. You stack transparent red, blue and yellow lens blocks to create target colors — [Google Play](https://play.google.com/store/apps/details?id=net.yetigames.colormixmatch).
- **Color Mix: Paint Match Game** (Electric Bird Games): Nov 30, 2024, 10,000+ (~26.7K), "no tube sorting" — [Google Play](https://play.google.com/store/apps/details?id=com.electricbirdgames.watercolormatch).
- **Color Mixing Matching** (AppStorehouse): Sep 12, 2023, 100,000+ (~120K) — [Google Play](https://play.google.com/store/apps/details?id=com.appstorehouse.cbq.app).
- **Hueventure - Color Mix Puzzle** (Ideavezy LLC, iOS): 2026-07-23, 0 US ratings. "Slide the colored blocks, combine two primaries into the color you need, and guide every block to its matching door" — [App Store](https://apps.apple.com/us/app/hueventure-color-mix-puzzle/id6791423824).
- **Color Merge: Mix & Match** (LVII Ltd., iOS): 2026-04-28, 0 US ratings. Drag primary tiles together to make secondary and tertiary colors, then match 3 to clear, with goals, moves and time — [App Store](https://apps.apple.com/us/app/color-merge-mix-match/id6763693330).
- **Chromix: Color Mixing Puzzle** (YUKI HIGASHI, iOS 2026-04-16): pour colors into cups and adjust ratios to a target, scored out of 100 — [App Store](https://apps.apple.com/us/app/chromix-color-mixing-puzzle/id6762201673).
- Further 2026 micro-releases, all with 0-35 US ratings:
  - **Hue Mix Puzzle: Color Match**: Jun 2026.
  - **Paint Bucket - Color Game** (Turkish studio ONURION, Kubelka-Munk pigment model): Jul 2026 — [App Store](https://apps.apple.com/us/app/paint-bucket-color-game/id6785648869).
  - **Pocket Dioramas: Color Mixing**: May 2026 — [App Store](https://apps.apple.com/us/app/pocket-dioramas-color-mixing/id6765855204).
  - **hued**: Aug 2026 — [App Store](https://apps.apple.com/us/app/hued-color-mixing-puzzle/id6796434562).
  - Daily color games: **SHADE** (Jan 2026) — [App Store](https://apps.apple.com/us/app/shade-a-daily-color-game/id6757939671); **Hue Today** (Mar 2026) — [App Store](https://apps.apple.com/us/app/hue-today-color-mix-to-match/id6760416647); **iro** (DOMU, Apr 2026, 35 US ratings) — [App Store](https://apps.apple.com/us/app/iro-daily-color-game/id6762220262).
  - **Dollop : mélange de couleurs**: FR store, 2026-09-12 — [App Store FR](https://apps.apple.com/fr/app/dollop-m%C3%A9lange-de-couleurs/id6807468926).
  - **Huefall: Mélange de couleurs**: FR store, 2026-09-02. Tube pouring in which two primaries make a new color and three make "Boue" (mud), an irreversible error — [App Store FR](https://apps.apple.com/fr/app/huefall-m%C3%A9lange-de-couleurs/id6805483656).
  - **IROZAN - Color Mixing Puzzle** (naoya saito, iOS 2026-04-07) — [App Store](https://apps.apple.com/us/app/id6761189163).
  - **Color Mix RUNNER**: runner, Jun 23, 2026, 1,000+ — [Google Play](https://play.google.com/store/apps/details?id=com.jlqhsb.color_mix_runner).
  - **Paint Mix: Color Puzzle** (DicodeMy): May 2026, 100+ — [Google Play](https://play.google.com/store/apps/details?id=com.dicodemy.colormatchmy).
  - **ColorChef** (for kids): Aug 2026 — [Google Play](https://play.google.com/store/apps/details?id=com.jigsaw.pallet).

#### Tier 4: "sort + mix" twist, a micro-trend visible in Sept 2026
- **Color Mix Sort** (StevenNguyen2603 on Google Play, Huy Nguyen on iOS): iOS 2026-09-17, Google Play Sep 21, 2026 (10+). "Pour two different colors together and they MIX into a third color… No undo on a mix" — [Google Play](https://play.google.com/store/apps/details?id=com.colormix.vn).
- **Paint Mix: Color Sort** (Kirill Prikota, iOS 2026-09-23): "a calm water sort game with a twist: some levels ask you to BLEND colors. Pour red and yellow into the cauldron… Fill the order" — [App Store](https://apps.apple.com/us/app/paint-mix-color-sort/id6813350894).
- **Hue Lab – Pour & Mix Puzzle** (Robert Tadevosyan, Google Play Sep 24, 2026). It has a "Mix" chapter rule: "pour red onto blue and the run becomes purple, so some colours have to be made" — [Google Play](https://play.google.com/store/apps/details?id=am.game.colorsortx).

#### Background classics (pre-2024)
- **I Love Hue** (Zut!) is a hue-ordering game with *no* mixing: 10,000,000+ (~11.7M) and 277,264 Google Play ratings — [Google Play](https://play.google.com/store/apps/details?id=com.zutgames.ilovehue). iOS: 2017-01-25, 49,597 US ratings — [App Store](https://apps.apple.com/us/app/i-love-hue/id1081075274). The sequel I Love Hue Too has 1,000,000+ (~4.9M) — [Google Play](https://play.google.com/store/apps/details?id=com.zutgames.ilovehue2).
- **Color Puzzle: Offline Hue Game** (CO2 Games; package name "i.love.hue.blendoku"): 10,000,000+ (~19.1M) — [Google Play](https://play.google.com/store/apps/details?id=com.color.puzzle.i.love.hue.blendoku.game). Blendoku (Lonely Few) returns 404 today on Google Play under com.lonelyfew.blendoku and com.lonelyfew.blendoku2.
- Older iOS mixing puzzles with small rating counts:
  - Color Sheep (2013, 125 US ratings) — [App Store](https://apps.apple.com/us/app/color-sheep/id592148079)
  - Kotoro (2014) — [App Store](https://apps.apple.com/us/app/kotoro/id779855682)
  - Watercolors, where you "Mix red, yellow and blue to paint your way through hundreds of challenging levels" (2014) — [App Store](https://apps.apple.com/us/app/watercolors/id875838755)
  - RYO (2014) — [App Store](https://apps.apple.com/us/app/ryo-color-puzzle/id871355777)
  - Colorcube (2016, 851 US ratings, Apple-featured) — [App Store](https://apps.apple.com/us/app/colorcube/id1076402133)
  - Splotches (2018) — [App Store](https://apps.apple.com/us/app/splotches-color-mixing-game/id1440429452)
  - Mélange (2020) — [App Store](https://apps.apple.com/us/app/m%C3%A9lange/id1513430421)
- ChromaGun is a PC/console first-person puzzle with RYB mixing; it is not mobile — [Wikipedia](https://en.wikipedia.org/wiki/ChromaGun).

#### Web and itch.io
- **Color Match (AMG)** on CrazyGames: blend named paints to match an object's color, with a 90% threshold — [CrazyGames](https://www.crazygames.com/game/color-match-amg).
- **Color Artist** on Poki: pixel-art coloring from a fixed palette, "doesn't appear to be… mixing" — [Poki](https://poki.com/en/g/color-artist).
- Mix to Match — [mixtomatch.org](https://mixtomatch.org/); Colorfle (daily) — [colorfle.com](https://colorfle.com/).
- itch.io jam games:
  - Fill It! (Global Game Jam 2026, RGB overlay mixing) — [itch.io](https://joanaw.itch.io/fill-it)
  - HEXAMIX — [itch.io](https://davidbuzatto.itch.io/hexamix)
  - Paint Splash (Brackeys Jam 2025.1) — [itch.io](https://emil1891.itch.io/paint-splash)
  - Paint Shop Rush ("learn how to mix colors in a fun and fast game") — [itch.io](https://reneu-morais.itch.io/paint-shop-rush)
  - COLOR RUSH! ("mix and match colors to gain points") — [itch.io](https://oafihaemfiuh.itch.io/color-rush)
  - MixUp! — [itch.io](https://sheryb.itch.io/mixup)
  - Mixtris — [itch.io](https://rainbowshell.itch.io/mixtris)

#### False positives (these say "mix" but have no mixing mechanic)
- **Magic Sort!** (Grand Games, 658,690 US ratings): a water sort in which "Mix colors" is only marketing copy — [App Store](https://apps.apple.com/us/app/magic-sort/id6499209744).
- Get Color Pack (Tripledot, separating "mixed liquids"); Blend It 3D (SayGames) and Blendy! (Lion Studios), which are smoothie blenders; Rollic's Colors Runners! (a color-zone hider). All were verified from their iTunes API descriptions.

### Inferences
- Color mixing has *proven mass appeal only as a satisfying hypercasual/ASMR simulator*: pick paints, see a % match, paint a 3D object or a makeup look. In that format the skill required is perceptual rather than logical. The logic-puzzle framings (orders, pipes, cups, tube-sorting) have not scaled in any case found.
- The 2026 wave (≈25-30 new color-mixing puzzles in 9 months, several in September 2026 alone) points to a crowded long tail of solo/AI-assisted projects. It is not publisher activity. The keywords "color mixing puzzle" are therefore saturated with low-quality competitors, even though no strong title exists.
- Among the 2024-26 puzzles, Water Pour Puzzle: Color Jam (2,746 US ratings) suggests that grafting mixing onto an already mass-market sort loop retains better than standalone mixing puzzles. This is a single data point.

### Gaps
- No revenue or DAU estimates (AppMagic/Sensor Tower) for any color-mixing title; those pages are paywalled or rendered in JavaScript.
- Install counts for iOS-only games (Mazeiro, Hueventure, Pintuki, Paint Mix: Color Sort) are not public; only rating counts are available.
- The Google Play search surface is ~30 results per query, so very obscure titles may have been missed.

## Q2. Does ANY game already combine color mixing with (a) a jammed pile, (b) a limited buffer/slots, and (c) a conveyor or pixel-art painting target? Does "Mix Rush" (or something very close) exist?

### Takeaway
Nothing identical to Mix Rush exists under that or any other name. The Mix Rush combination is: primary-only pots from a jammed pile, dropped into 4-5 mixing bowls that *are* the buffer, auto-brushes painting pixel art, and clogging bowls as the fail state.

However, **two near-identical precursors were published in December 2024**, both with essentially zero traction:
- **"Color Merge Jam"** (iOS, Garawell Games' CEO account, the team behind the Color Match hit) has a layered pot tray, 5 buffer slots, and a mixing bowl that combines primaries (e.g. blue + yellow → green) to paint objects.
- **"Color Jam"** (Android, Taninty Game Studio) has a jammed grid of paint bottles, 5 buffer slots, and a mixing can that paints a jigsaw-style picture.

Several other games combine mixing with pixel art (Merge Colors, Pintuki, Color Pixel Destroy) or with block-jam doors (Hueventure). The core idea is therefore not novel, but it is unexploited: nobody has made it work commercially.

### Cited Findings

#### Closest precursors (ranked by overlap with Mix Rush)
1. **Color Merge Jam** — iOS id6738347865. Seller "Nebih Basaran", the CEO of Garawell Games, credited for Color Match in [Supersonic's case study](https://supersonic.com/learn/case-studies/color-match/).
   - Released 2024-12-18; only version is v1.0 (2024-12-19); 0 ratings in the US/FR/TR/GB/DE storefronts (iTunes Lookup API with `country=`us/fr/tr/gb/de, 2026-09-28) — [App Store](https://apps.apple.com/us/app/color-merge-jam/id6738347865), [Lookup API](https://itunes.apple.com/lookup?id=6738347865&country=fr).
   - Description: "Mix colors to create stunning new shades and use them to paint incoming objects… Mix & Match: Experiment with primary colors to unlock endless combinations. Challenging Puzzles: Strategically color objects before they reach the end." — [App Store](https://apps.apple.com/us/app/color-merge-jam/id6738347865)
   - Screenshots (viewed 2026-09-28; captions "Mix Colors!", "Tap For Paint!", "Choose Right Colors!") show:
     - a "Goal" color and a white 3D object to paint (a mushroom, a rocket, a pear);
     - a row of **5 empty holding slots**;
     - a **single mixing bowl** with a recipe indicator (e.g. yellow + blue ticked → green);
     - a **two-layer tray of paint pots** (red, black, yellow, blue, cyan, pink…) in which only the front row is active and the back row is dimmed.
   - A Google Play listing for the Android twin `com.Garawell.ColorMergeJam` is still indexed by web search, but the package now returns 404 (checked in the US, TR, VN, GB, NP and IN). "Jelly Color Merge" (`com.Garawell.JellyColorMerge`) behaves the same way — [Google Play (404)](https://play.google.com/store/apps/details?id=com.Garawell.ColorMergeJam).
2. **Color Jam** (Taninty Game Studio, India; Android only).
   - Released Dec 11, 2024; 1,000+ (~2.8K) installs; no ratings; last update Oct 30, 2025. The listing flags no ads and no IAP.
   - Store text: "A Color mixing jam where paint bottles are waiting, all you need to sort the needed mixing color and form a Color Palette. Finish all piece to make Palette Art" — [Google Play](https://play.google.com/store/apps/details?id=com.taninty.colorjam).
   - Screenshots ("TAP SMART", "SOLVE PUZZLE", "FILL THE CAN", "CLEAR BOARD") show:
     - a line-art picture split into jigsaw pieces at the top;
     - a paint can that shows the remaining ingredients (e.g. "1 purple + 1 red") and pours its color onto the picture;
     - **5 holding slots plus 3 ad-unlockable slots**;
     - below them, a dense **jammed grid of paint bottles** with hidden "?" bottles and locked cells carrying counters (3, 5, 8).
   - Difference from Mix Rush: bottles come in already-mixed colors (green, orange, purple, cyan, pink…). The "mix" is an ingredient recipe for the can, not free primary mixing across several bowls.
   - Taninty was founded in 2013 by M.S. Sathish Kumar — [LinkedIn](https://www.linkedin.com/in/taninty/). It has no iOS version of Color Jam (iTunes Lookup of developer id983585874).
3. **Color Pixel Destroy** (Firat ZEREN, iOS 2023-12-04, v1.0, 0 ratings): "Mix colors and destroy pixels." — [App Store](https://apps.apple.com/us/app/color-pixel-destroy/id6473683646). The screenshot shows paint pots (black, white, blue, green, red) around a central mixing blob that sprays the mixed color at a **pixel-art image**, which breaks into cubes. This is a mixing + pixel-art-target prototype that predates Pixel Flow.
4. **Merge Colors: Puzzle Coloring / Merge Colors Puzzle: Mix&Draw** (F-WAY GAMES).
   - Google Play: Apr 7, 2023, 50,000+ (~86.5K), 3.56 score, last updated Oct 21, 2025 — [Google Play](https://play.google.com/store/apps/details?id=com.fwaygames.mergecolors).
   - iOS: 2023-05-15, 26 US ratings, last updated 2024-05-17 — [App Store](https://apps.apple.com/us/app/merge-colors-puzzle-mix-draw/id6448279168).
   - "Immerse yourself in a happy world of pixel pictures… explore the perfect color mix to bring each image to life… Combine colors by blending them together to create new shades." Screenshots show a pixel-art picture, a target palette, a merge grid of paint drops, and spawner tubes in red, blue, yellow, black and white.
   - It shares mixing primaries + white/black to paint pixel art with Mix Rush, but has no jam and no clog/fail pressure.
5. **Pintuki: Mix Colors Pixel Art** (Ferran Espuna, iOS 2026-09-15, 0 ratings) — [App Store](https://apps.apple.com/us/app/pintuki-mix-colors-pixel-art/id6797773932).
   - A color-by-number pixel game in which every numbered color must be mixed on a "mixing bench" from 5 starting pots: crimson, sunflower, cobalt, titanium white, ink black. Tangerine, emerald and violet unlock later.
   - It has an 8-drop well, a match meter, and a "Load the brush" step.
   - There is no jam, buffer or fail state; the game is about precision.
6. **Hueventure** (Ideavezy, iOS 2026-07-23): combines color mixing with a *block-jam* "slide to matching door" loop. "combine two primaries into the color you need, and guide every block to its matching door" — [App Store](https://apps.apple.com/us/app/hueventure-color-mix-puzzle/id6791423824).
7. **Color Drop: Puzzle Mania ASMR** (WingsMob, iOS 2025-05-08, 175 US ratings) has *the same brush/slot structure without mixing*: "Pick a colorless brush from below - Place it into a top slot to fill it with matching drop ink - Once full, it flies up to paint the artwork… boosters like undo, extra slots" — [App Store](https://apps.apple.com/us/app/color-drop-puzzle-mania-asmr/id6745587999).

#### "Paint/pixel + jam + buffer" games with no mixing (a crowded adjacent space, 2024-2026)
- Paint Jam 3D: Sorting Game (Aug 2025, 100+): "fill the paint buckets and complete the paintings… without running out of space on the board" — [Google Play](https://play.google.com/store/apps/details?id=com.TeesAndCees.PaintJam).
- Liquid Paint Jam (Feb 2026): "send matching color jellies from the bottom to fill art pieces" — [Google Play](https://play.google.com/store/apps/details?id=com.kolpoverse.lpj).
- Pixel Paint Jam (Feb 2026), a Pixel Flow-style painter-box conveyor — [Google Play](https://play.google.com/store/apps/details?id=com.AetherCoreStudios.PixelPaintJam).
- Paint Block Jam / "Color Drop: Paint Jam" (Lumos Games, Turkey, Jul 2026): "Place colorful ball blocks into the correct conveyor slots… watch cute 2D objects transform" — [Google Play](https://play.google.com/store/apps/details?id=com.rocketgames.paintblockjam).
- Gem Jam Painting (A Thinking Ape, 2025, 5,000+): block jam → diamond-painting pixel art — [Google Play](https://play.google.com/store/apps/details?id=ata.strike.puzzle.diamond.paint.jam).
- Jumbo Paint! (2026) — [Google Play](https://play.google.com/store/apps/details?id=com.oreon.cubepainters).
- Graffiti Jam (2025): spray cans stuck on a grid, freed into an inventory — [Google Play](https://play.google.com/store/apps/details?id=com.gamesngames.graffitijam).
- Pixel Jam (MAGIC GAME STUDIO, Jul 2026): "Tap the blocks in the picture and watch them jump into the three trays above" — [Google Play](https://play.google.com/store/apps/details?id=com.fc.pixel.jam).
- Color Jam: Art Puzzle (XGame HK, 100,000+). Here mixing is a *failure*: "Choose the right bucket to avoid mixing color" — [Google Play](https://play.google.com/store/apps/details?id=com.color.flow.puzzle).
- Sort Paint: Water Sort Puzzle (Freeplay, 2022, 1,000,000+): sort the bottles, then paint a picture with them — [Google Play](https://play.google.com/store/apps/details?id=com.game.paint.sort).
- **Fruit Jam** (iKame Games, Jul 24, 2026, 5,000+) is the closest *structural* analogue from a hybrid-casual publisher, with a flow/conveyor, a blender and orders: "Watch fruits travel through the flow… Send fruits into the blender… Complete every drink order" — [Google Play](https://play.google.com/store/apps/details?id=com.ig.fruit.jam). Fruit juice replaces color theory.

#### Name search
- No app titled "Mix Rush", "Color Mix Rush" or "Paint Mix Rush" appeared in the iOS US storefront (up to 200 results per query) or in Google Play US (top ~30 results per query). Full detail is in Q5.

#### Searches actually run
- **Google Play (US, 51 queries):**
  - color mixing / colour mixing / color mix / mix colors / color mixing puzzle / color mixing game
  - paint mixing / paint mix / mix paint / color blend / color mixer / mix to match / paint match
  - mix rush / color mix rush / paint mix rush
  - color mix jam / paint jam / mix jam / mix sort / paint sort / color mix sort / painting sort
  - liquid mix color / color pour / paint pour
  - hue puzzle / i love hue / blendoku / chromatron
  - paint puzzle / pixel paint jam / pixel flow / primary colors puzzle / color mix pixel / mix and paint / blend jam / color mixer sort / hue mix
  - color merge puzzle / merge colors / paint mixer / mix match color / paint bucket puzzle / pixel jam / paint rush / mix master color
  - colour match paint / color match mix / paint shooter pixel / mélange couleurs
- **App Store (iTunes Search API, 36 queries, all completed after rate-limit retries; 2,478 unique apps collected):**
  - US: color mixing, colour mixing, color mix, mix colors, color mixing puzzle, paint mixing, paint mix, mix paint, color blend, color mixer, mix rush, color mix rush, paint mix rush, color mix jam, paint jam, mix jam, mix sort, color mix sort, painting sort, paint sort, color pour, paint pour, primary colors puzzle, color mix pixel, mix and paint, blend jam, hue mix, pixel flow, paint rush, mix master, blendoku, i love hue, chromatron
  - FR: mélange couleurs, mix rush, color mixing
  - A developer-catalogue lookup of Nebih Basaran (id1586973680) and Taninty (id983585874).
- **Web:** "Mix Rush" game/app/trademark; color mixing jam puzzle 2026; Pixel Flow color mixing; Pixel Flow clone color mixing 2026; Voodoo/SayGames/Homa color mixing; the Poki/CrazyGames/itch.io color-mixing listings; mobilegamer.biz / pocketgamer.biz color mixing.

### Inferences
- Mix Rush's defensible novelty sits in the *combination*: (i) only primaries (plus white/black) arrive, so every secondary must be *built*; (ii) the mixing bowls *are* the buffer, so a wrong mix consumes buffer space and clogs it (a "space pressure" fail state, as in Bus Jam and Pixel Flow); (iii) an auto-brush paints exposed pixels (a Pixel Flow-like payoff).
  - Color Merge Jam has a separate buffer and a single bowl. Taninty's Color Jam uses pre-mixed bottles and one can. Merge Colors and Pintuki lack pressure. Huefall's "Boue/mud" and Color Mix Sort's "no undo on a mix" show that indie devs already use *irreversible bad mixes* as a tension device.
- Because a top hypercasual studio (Garawell) shipped a near-identical "mix-jam" prototype in Dec 2024 and never updated it past v1.0 (Android build now removed), the concept **probably already failed at least one marketability/CPI test**. This is an inference from version history and delisting; no CPI data was published.
- There is clear prior art, so there is no originality claim to rely on for marketing or for platform-feature pitches. The upside is that there is no incumbent to beat either: no color-mixing jam/pixel title has traction.

### Gaps
- Actual gameplay videos of Color Merge Jam and Taninty's Color Jam were not found (no YouTube or ad hits). The mechanics above are inferred from store screenshots and descriptions.
- I could not access ad-intelligence libraries (Meta Ad Library, TikTok Creative Center, AppMagic/Sensor Tower ad-intel). Unpublished CPI tests by Voodoo, Homa, Rollic, SayGames and others that never reached public store search (or ran on test markets or TestFlight) cannot be ruled out.
- Store search APIs return a bounded list (up to 200 results on iOS, ~30 on Google Play). A title that ranks poorly for all of these queries could be missed.

## Q3. Have big hybrid-casual publishers or Pixel Flow clones added a color-mixing feature or level type recently (2025-2026)?

### Takeaway
I found no evidence that any top hybrid-casual publisher, Pixel Flow itself, or any Pixel Flow clone shipped a color-mixing mechanic or level type in 2025-2026. Publisher-side color-mixing activity is concentrated in 2021-2024 (Supersonic, CrazyLabs, Lion Studios, Garawell tests). The only 2026 publisher release that is structurally similar (iKame's Fruit Jam) blends fruit, not colors.

### Cited Findings
- **Pixel Flow! itself (Loom Games)**
  - Store data:
    - Google Play: released Aug 18, 2025; 10,000,000+ (~13.2M); 224,139 ratings; updated Sep 25, 2026 — [Google Play](https://play.google.com/store/apps/details?id=com.loomgames.pixelflow).
    - iOS: 2025-08-17; v0.35.0 on 2026-09-28; 161,429 US ratings — [App Store](https://apps.apple.com/us/app/pixel-flow/id6751056652).
  - The store description is purely color-*matching*: "Pigs only hit their own color", with the "5 waiting slots" and conveyor capacity. Mixing is not mentioned — [Google Play](https://play.google.com/store/apps/details?id=com.loomgames.pixelflow).
  - Deconstructor of Fun calls it fully deterministic "space pressure" with a conveyor, a bench/slots and front-row-only interaction. By December 2025 it cites "$500K+ a day", "~$180M yearly run rate" and "Around 200K downloads per day" — [Deconstructor of Fun](https://www.deconstructoroffun.com/blog/2026/2/13/pixel-flow-the-publishers-dream).
  - $105M IAP by May 2026 (AppMagic), making it the third hybrid-casual puzzle past $100M after Color Block Jam ($148M) and Screwdom ($120M). The top-10 hybrid-casual puzzles made >$270M in Jan-May 2026 vs ~$340M in all of 2025 — [mobilegamer.biz](https://mobilegamer.biz/data-digest-pixel-flow-hits-100m-mays-top-games-neverness-to-everness-pokemon-go-more/).
  - Scopely took majority control of Loom Games in Feb 2026 at a $1B+ valuation — [AppBird](https://appbird.ai/blog/pixel-flow-copycat-20-million-gone/).
- **Clones**
  - Color Blaze Shooter (Amobear/AVN Globalis) was a "1-to-1 copy" that made an estimated $20M ($10M IAP + $10M ads) in ~4 weeks before being removed on March 1, 2026. It introduced no new mechanics, and "hundreds of Pixel Flow-inspired games" remain — [AppBird](https://appbird.ai/blog/pixel-flow-copycat-20-million-gone/).
  - Color Pixel Shooter (a Vietnamese studio) reached ~$140K/day, and Voodoo's This Is Blast peaked at ~$40K/day — [Deconstructor of Fun](https://www.deconstructoroffun.com/blog/2026/2/13/pixel-flow-the-publishers-dream).
- The 2025-26 pixel/flow clones on Google Play have **no mixing in their descriptions**:
  - Ants Flow (Estoty, Mar 2026, 1,000,000+) — [Google Play](https://play.google.com/store/apps/details?id=com.ants.box)
  - Voxel Blast Jam (Unico, Jan 2026) — [Google Play](https://play.google.com/store/apps/details?id=com.unicostudio.voxelblastjam)
  - Color Train (Dec 2025) — [Google Play](https://play.google.com/store/apps/details?id=com.fc.slf.flow.pixel.color.puzzle)
  - Color Cube (Jan 2026) — [Google Play](https://play.google.com/store/apps/details?id=com.horus.cube.flow.color.sort.puzzle)
  - Bug Flow (Jun 2026) — [Google Play](https://play.google.com/store/apps/details?id=com.bug.flow.pixel.puzzle)
  - Color Flow: Pixel Jam (Apr 2026, squid ink shooters) — [Google Play](https://play.google.com/store/apps/details?id=com.game.ig14.octopusjam)
  - Pixel Drop: Bus Puzzle (Amobear, Jun 30 2026, pixel art + bus jam) — [Google Play](https://play.google.com/store/apps/details?id=com.pixel.bus.color.challenge.puzzle)
  - Loop Master: Color Jam Sort (Jul 2026, 1,000,000+) — [Google Play](https://play.google.com/store/apps/details?id=com.loop.clear.cube.free.card.puzzle.sort.match)
  - I scanned all 550 fetched Google Play descriptions with a regex for pixel/flow/shoot/blast/loop titles that mention mixing colors. The only hits were false positives, such as "new coloring pages".
- **Big-publisher pixel-art + sort launches in 2026, without mixing**
  - Easybrain's **Sort Art - Pixel Coloring Game** (Jul 10, 2026, 50,000+): "blends pixel coloring and tile sorting" — [Google Play](https://play.google.com/store/apps/details?id=com.easybrain.color.pixel.sort).
  - Take-Two/Popcore's **Marvellous Marble**, soft-launched on iOS in Turkey (May 2026), is "colour-matching meets classic pixel art". Mixing is not mentioned — [mobilegamer.biz](https://mobilegamer.biz/new-game-digest-warhammer-farming-simulator-the-walking-dead-godforge-centurys-next-game-more/).
  - Gem Coloring Match (Pine Stars, Mar 2026, 1,000,000+): sort gems to create pixel art — [Google Play](https://play.google.com/store/apps/details?id=gem.sort.color.match.puzzle.paint).
- **Garawell's test pipeline** (the Nebih Basaran iOS account) shows repeated color-mixing spin-offs after Color Match, none of which scaled. The account holds 110 apps; 106 of them never went past version 0.x/1.x, and the best-rated has 22 US ratings — [developer catalogue via Lookup API](https://itunes.apple.com/lookup?id=1586973680&entity=software&limit=200&country=us):
  - Restore Paintings (2022-01, "painting the cracked area with the matching color from the palette mixture")
  - Color Match Run (2022-01, "mix your character with the appropriate colors")
  - Color Leak (2022-11, "You will have to mix sometimes to get the correct color")
  - Blend Artist (2023-02)
  - Jelly Color Merge (2024-01, v1.0)
  - Color Merge Jam (2024-12, v1.0)
  - The account's most recent tests are Loop Match!, Block Splat and Stick Blast (Aug 25-26, 2026), Pixel Shoot 3D (2026-09-01) and Word Loop! (2026-09-12). The studio has moved on to Pixel Flow-like and loop formats.
- **Other abandoned color-mixing tests by known studios** (inferred from version numbers and last-update dates):
  - Lion Studios, Silicone Color Match: v0.83, last updated 2022-09-01 — [App Store](https://apps.apple.com/us/app/silicone-color-match/id1624494991)
  - BOOM Games (Turkey), Jelly Mix 3D: v0.0.1, 2022 — [App Store](https://apps.apple.com/us/app/jelly-mix-3d/id1619762729)
  - DODO Bilişim, Color Pools!: v0.2, 2022 — [App Store](https://apps.apple.com/us/app/color-pools/id1639554624)
  - Mooncake Games, Color Master 3D!: v1.0.0 only, 2022 — [App Store](https://apps.apple.com/us/app/color-master-3d/id1609909856)
  - Tape and Paint: v0.1, 2022 — [App Store](https://apps.apple.com/us/app/tape-and-paint/id1617766535)
- Supersonic kept publishing color-mixing *puzzles* in 2023 (Play Colors, Color Merge Puzzle) but only reached 1-3.6M Google Play installs. Color Merge Puzzle has not been updated since Oct 2025 (see Q1 citations).

### Inferences
- The absence of mixing among hundreds of Pixel Flow-likes is telling. Mixing breaks the "one glance = one decision" readability that makes color-*matching* conveyors work at mass-market CPI. Publishers appear to prefer adding blockers, hidden colors and multi-layer targets instead. This is my reasoning, not a sourced statement.
- The window is open, but it is open *because* the big players have tested and passed on color mixing, not because they overlooked it.

### Gaps
- No access to ad-intel (Meta/TikTok ad libraries, AppMagic creatives) to confirm whether publishers ran mixing *ads* or fake-door CPI tests in 2025-26.
- No statement from any publisher explaining why color-mixing tests were dropped.

## Q4. How commercially successful are color-mixing games, and why (niche/educational/relaxing vs mass-market)? Any CPI evidence?

### Takeaway
Color mixing tested exceptionally well once, in 2021-22, as an ASMR "match the shade" hypercasual. Color Match hit #1 overall on iOS US, was the most-downloaded iOS game of January 2022, and had an iOS CPI of $0.10 with D1 above 38%. Its spin-offs (makeup and eye mixing by CrazyLabs) also reached 10M+ installs. Puzzle-style mixing (orders, pipes, sorting, recipes) has stayed niche. Supersonic's own mixing puzzles plateaued at 1-3.6M installs with weak ratings, and every 2024-26 indie mixing puzzle is small. A studio with the #1 mixing hit tested at least six mixing follow-ups (including a mix-jam) and scaled none.

### Cited Findings
- **Color Match: Supersonic case study**
  - Metrics: "Achieved #1 overall on iOS in the US"; "top 10 overall on Android in the US"; "top 10 games for over two months"; Initial CPI $0.53 → optimized **$0.29 (Android)**; **iOS CPI $0.10** (stable for 2+ months); **D1 retention >38%**; **D0 playtime 1100 seconds**; the 3D UI improved APPU by 75% — [Supersonic](https://supersonic.com/learn/case-studies/color-match/).
  - Origin: the concept evolved from Garawell's self-published "Colour Merge 3D" after the studio observed color-matching trends on Reddit and TikTok — [Supersonic](https://supersonic.com/learn/case-studies/color-match/).
- **Color Match in the charts**
  - Most downloaded iOS game of January 2022 with 6.2M downloads; the loop is "mix paints on the palette until the color matches… paint the model… sell it at auction" (AppMagic via WN Hub) — [WN Hub](https://wnhub.io/news/other/item-19843).
  - 29M downloads in Q1 2022 and #7 among hypercasual games, with the US 28% of downloads (AppMagic via PreMortem Games) — [PreMortem Games](https://premortem.games/2022/04/13/market-for-hypercasual-games-down-8-still-4-2b-downloads-in-q1-2022/).
  - Today: 50M+ Google Play (~79.5M) and 505,814 US iOS ratings, still updated in Sept 2026 (Q1 citations).
- **Other mass-market mixing sims** (all IAP + ads, with makeup and art framing): Makeup Kit - Color Mixing ~47.0M Google Play installs; Eye Color Mix ~35.4M; Mix & Paint ~30.8M, ads only (Q1 citations).
- **Puzzle-style mixing results:**
  - Play Colors (Supersonic): ~3.59M installs, 3.44 score.
  - Color Merge Puzzle (Supersonic): ~1.17M installs, no update since Oct 2025.
  - Mix Colors!: ~897K installs, 2.85 score.
  - Merge Colors (pixel art + mixing): ~86.5K installs.
  - Color Spark: ~16.8K. Paint Match: ~9.9K. Hex Mixer: ~4.9K. Taninty Color Jam: ~2.8K. Mazeiro: 0 US ratings. (Q1 and Q2 citations)
- **Adjacent color-perception puzzles scale as "relaxing" titles:** I Love Hue ~11.7M installs and 277K Google Play ratings; Color Puzzle: Offline Hue Game ~19.1M (Q1 citations). A search-result snippet of the official site describes I Love Hue as a meditative, chilled experience with no timers, no move limits and no punishments for failure. I could not verify this on the page itself, which sits behind a bot check — [i-love-hue.com](https://i-love-hue.com/).
- **Market context for the target genre:** Color Block Jam $148M IAP, Screwdom $120M, Pixel Flow $105M lifetime IAP (as of May 2026) — [mobilegamer.biz](https://mobilegamer.biz/data-digest-pixel-flow-hits-100m-mays-top-games-neverness-to-everness-pokemon-go-more/). Rollic's Color Block Jam is described as blending Block Puzzle and Parking Jam, with $33M revenue and 18.6M downloads in the Q3 2025 AppMagic ranking (per search-result summary; page not fetched) — [AppMagic blog](https://appmagic.rocks/blog/q3hybrid2025).

### Inferences
- **Why the Color Match format worked:** instant, perceptual feedback (a % match), an ASMR paint payoff, and zero color-theory knowledge needed. The player can always "get closer" by feel. Puzzle framings instead demand explicit knowledge (RYB rules, tertiary colors, tints and shades) and punish mistakes. That friction shows up as low ratings (2.85-3.44 for Mix Colors! and Play Colors) and small audiences.
- **Accessibility and readability risks:** mixed hues are harder to distinguish at small pixel sizes and for color-blind players. Several indie titles felt the need to add symbol overlays: Mazeiro advertises a "Color Assist" setting that assigns symbols to paint colors ([App Store](https://apps.apple.com/us/app/mazeiro-color-mixing-puzzle/id6791533011)), and Paint Spill offers "Optional colour patterns… for colour-blind and low-vision players" ([Google Play](https://play.google.com/store/apps/details?id=com.ariwellstudios.paintspill)). This is a known design cost, inferred from features the developers advertise.
- **CPI implication for Mix Rush:** the only public CPI evidence for mixing is very favorable, but it is dated (2021-22 hypercasual, ad-monetized, iOS pre-ATT maturity). No public evidence shows a *hybrid-casual puzzle* built on mixing passing CPI in 2024-26. The repeated abandonment of mixing prototypes by professional testers (Garawell, Lion Studios, BOOM, DODO) weakly suggests poor CPI or retention.

### Gaps
- No revenue figures for Color Match / Coloring Match, Makeup Kit or Eye Color Mix. The Sensor Tower page did not render and AppMagic is paywalled.
- No published CPI/IPM numbers for any 2024-26 color-mixing prototype, including Garawell's Color Merge Jam.
- The claim that Color Match was the "2nd most downloaded game in the App Store worldwide in Q1 2022" appeared only in a search summary without a verifiable source, so it is excluded.

## Q5. Name conflicts: does a game called "Mix Rush" (or "Color Mix Rush" / "Paint Mix Rush") exist, and are there trademark issues?

### Takeaway
No live mobile game titled "Mix Rush", "Color Mix Rush" or "Paint Mix Rush" was found on the US App Store or Google Play as of 2026-09-28. The main historical collision is Cartoon Network's **"Mixels Rush"** (2015, delisted 2018). Nearby names already in use include "Color Mix RUNNER", many "Color Rush" titles, "Paint Rush" and "Makeup Rush 3D". A formal trademark search (EUIPO TMview, INPI, USPTO) was not possible from this environment and remains to be done.

### Cited Findings
- **iTunes Search API, US and FR** (up to 200 results each):
  - For "mix rush", "color mix rush" and "paint mix rush" (US) and "mix rush" (FR), no title contains both "Mix" and "Rush" except an unrelated 2016 kids' coloring app, "Animal Color Mix Page Paintbrush…".
  - The FR "mix rush" results are generic apps (YouTube, CapCut, Super Mario Run…) and small "Rush" games.
  - The top results for "mix rush" are DJ/music apps and Ketchapp's "Rush" (51,709 US ratings) — [App Store: Rush](https://apps.apple.com/us/app/rush/id1257043104).
- **Google Play US** (top ~30 results each): "mix rush" returns Rhythm Rush, Streamer Rush, Mix Blox and similar, with no "Mix Rush". "color mix rush" returns Color Mixer, Color Rush (Rayole Games, 100,000+), Mix & Paint, Giant Color Rush and others.
- **Existing near-names:**
  - **Color Mix RUNNER** (Rizky-PAR, Jun 2026): "blend primary colors to match obstacles and gates" — [Google Play](https://play.google.com/store/apps/details?id=com.jlqhsb.color_mix_runner).
  - Rainbow Rush: Color Defense (Kite Games, Apr 2026) — [Google Play](https://play.google.com/store/apps/details?id=com.kite.rainbowsiege).
  - Makeup Rush 3D (Taninty, 2023; from the iOS catalogue lookup of developer id983585874).
  - itch.io: "Paint Shop Rush" (color mixing) and "COLOR RUSH!" (mix and match colors) — [itch.io](https://reneu-morais.itch.io/paint-shop-rush), [itch.io](https://oafihaemfiuh.itch.io/color-rush).
- **Mixels Rush** (Heavy Boat / Cartoon Network; package com.turner.mixelrush): released May 21, 2015; made free June 22, 2016; no longer downloadable as of May 17, 2018 — [Mixels Wiki](https://mixels.fandom.com/wiki/Mixels_Rush), [AppBrain](https://www.appbrain.com/app/mixels-rush/com.turner.mixelrush).
- **Trademarks:** web searches returned no "MIX RUSH" or "MIXRUSH" mark, only unrelated RUSH marks (e.g. Capcom's "RUSH", USPTO serial 74576855) and a "Rushmix" company — [LegalHoop](https://www.legalhoop.com/trademark/detail/74576855/RUSH), [Crunchbase](https://www.crunchbase.com/organization/rushmix). A direct query to the TMview API from this environment failed with an empty server reply.

### Inferences
- "Mix Rush" looks free as a store title, but "Rush" is heavily used in games (Ketchapp's Rush, Rush Royale, Minion Rush, Sugar Rush…). The combination may be weakly distinctive for trademark purposes and hard to rank for in ASO against "color rush" and "rhythm rush" results.
- The name also slightly mis-signals the genre. "Rush" implies speed or timers, whereas the concept is a deterministic space-management puzzle, the reason Pixel Flow is praised as deterministic. This is worth weighing against more descriptive options (e.g. "Mix & Paint Jam") if ASO is a priority.

### Gaps
- No authoritative trademark clearance. The name must be checked directly in EUIPO TMview (EU and national offices, including INPI France) and in USPTO Trademark Search, in Nice classes 9 and 41.
- Store searches cover the US storefronts plus three FR App Store queries. Region-locked or unlisted test apps named "Mix Rush" cannot be excluded.
