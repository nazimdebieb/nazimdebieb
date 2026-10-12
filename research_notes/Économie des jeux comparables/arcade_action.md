# Coin economies of casual arcade/action games (Angry Birds 2, Subway Surfers, Archero), for comparison with Pop Archer

Research date: 2026-09-30. Fandom wiki pages were read through the MediaWiki API (`/api.php?action=parse`) on that date, because the normal page returns 402/403 to fetchers. The links below point to the normal wiki pages. The App Store (France) in-app purchase lists were read from apps.apple.com/fr on 2026-09-30, so those prices are **current and in €**. Values in **$** come from US sources and are marked as such. Anything dated before 2024 is flagged with its year.

## Q1 - Soft currency earned per run/level/stage, and chapter/world rewards

### Takeaway
None of the three games pays a fixed coin amount per level. Subway Surfers pays whatever coins you pick up during the run, plus boxes. Archero pays gold that scales with the chapter (a coin wheel at the start of each run worth about 383 × (chapter − 1) on average), plus gems every few stages. Angry Birds 2 mostly pays gems, feathers and pearls from quests, chests and events, not coins per level. The practical takeaway for Pop Archer is to scale rewards with progress and push extra payouts through boxes, wheels and streaks.

### Cited Findings
**Subway Surfers (SYBO)**
- Coins are collected during runs, from Mystery Boxes, from Word Hunt days 3 and 4, from friends (after 25 runs), from Season Hunt tiers, from connecting Facebook (5K coins) and from the Welcome Pack (one-time, up to 45K coins) — [Subway Surfers Wiki: Coin](https://subwaysurf.fandom.com/wiki/Coin)
- The official help center confirms the same sources: runs, IAP, Mystery Boxes, Word Hunt day 3/4, friends completing 25 runs, social-media follows, and ads "for some free Coins". It gives no per-run figure — [SYBO Help Center: Coins](https://sybo.helpshift.com/hc/en/5-subway-surfers/faq/147-coins/)
- Word Hunt (daily word challenge) streak rewards: day 1–2 = Mini Mystery Box, day 3 = 1,050 coins, day 4 = 1,500 coins, day 5+ = Super Mystery Box — [Wiki: Word Hunt](https://subwaysurf.fandom.com/wiki/Word_Hunt)
- The early mission sets ask for "Pick up 100 Coins in one run" (set 2), "Pick up 200 Coins in one run" (set 4) and "Pick up 2500 Coins" cumulative (set 5). Each set of 3 missions raises the score multiplier by +1, and from set 30 onward it gives a Super Mystery Box. Missions can be skipped with coins or an ad — [Wiki: Missions](https://subwaysurf.fandom.com/wiki/Missions)
- Mystery Box (500 coins, or found in runs): coins 300–1,000 (34.89%), coins 1,500–5,000 (4.1%), jackpot 100,000 (0.01%), tokens 35%, hoverboards 1–3 (17%), key 1 (4%). Prizes can be doubled by watching an ad — [Wiki: Mystery Box](https://subwaysurf.fandom.com/wiki/Mystery_Box)
- Super Mystery Box (not buyable): coins 1,000–8,000 (51%), 15,000–50,000 (0.9%), 300,000 jackpot (0.08%), keys 1–3 (4.85%) — [Wiki: Super Mystery Box](https://subwaysurf.fandom.com/wiki/Super_Mystery_Box)
- Mini Mystery Box: coins 200–2,000 (35%), event coins 100–1,000 (26%), key 1 (1.8%) — [Wiki: Mini Mystery Box](https://subwaysurf.fandom.com/wiki/Mini_Mystery_Box)

**Archero (Habby)**
- Starting Lucky Wheel, once per run from chapter 2: 1/2 chance of 300×(chapter−1) coins, 1/3 chance of 400×(chapter−1), 1/6 chance of 600×(chapter−1) — [Archero Wiki: Lucky Wheel](https://archero.fandom.com/wiki/Lucky_Wheel)
- After a boss, the random Lucky Wheel gives a 1/6 chance of coins worth about 45% of a level-up and a 1/3 chance of about 25% of a level-up; the other outcomes are skills or healing — [Wiki: Lucky Wheel](https://archero.fandom.com/wiki/Lucky_Wheel)
- Gold also drops from enemies in chapters ("higher gives more"), from the Time Reward and from the shop — [Wiki: Gold](https://archero.fandom.com/wiki/Gold)
- Gems: +20 per player level-up, from Stage Rewards, from the ad Lucky Wheel, from Expedition mode (every 5 stages) and from challenges — [Wiki: Gems](https://archero.fandom.com/wiki/Gems)
- Gems also come from stage rewards "every 5-10 new stages", and completing 5 new stages gives +5 energy (2019) — [Game Developer, "Finding the fun: Archero part 3 – monetization", 16 Jul 2019](https://gamedeveloper.com/design/finding-the-fun-archero-part-3---monetization)
- Hero/boss chapters (7, 14, 21, 28…) are described as quick runs with a high amount of gold. This is a search snippet only; the page was not verified — [Android Authority / search results](https://androidauthority.com/archero-cheats-tips-hacks-glitches-1087839)

**Angry Birds 2 (Rovio)**
- Gems come from daily quests (Treasure Pass), free chests, clan and Arena rewards, Tower of Fortune cards, the Wonder Whirl, and the shop — [Angry Birds Wiki: Gems](https://angrybirds.fandom.com/wiki/Gems)
- Daily Challenge: 3 stages (stage 1, hard stage, boss). Clearing it pays gems, feathers and hats. A 7-day streak with no failures gives a **Legendary Chest** — [Wiki: AB2 Daily Challenge](https://angrybirds.fandom.com/wiki/Angry_Birds_2/Daily_Challenge)
- Feats (long-term achievements) pay feathers, gems, black pearls and chests — [Wiki: Angry Birds 2](https://angrybirds.fandom.com/wiki/Angry_Birds_2)
- Mighty Eagle's Bootcamp pays Mighty Eagle Coins based on performance. Those coins are spent in a separate cosmetic/hat shop — [Wiki: Angry Birds 2](https://angrybirds.fandom.com/wiki/Angry_Birds_2)

### Inferences
- Subway Surfers: the early missions (100–200 coins in one run) suggest that a new player's normal run yields on the order of **100–300 coins** (estimate). Boxes add 300–8,000 coins at random. The daily Word Hunt gives about 2,550 coins over a 4-day streak plus boxes.
- Archero: the average starting-wheel payout is 383 × (chapter − 1). That is about 383 gold in chapter 2 and about 3,450 gold in chapter 10 (calculated), before enemy drops.
- For Pop Archer: all three games separate a **per-level trickle** from **lumpy random rewards** (boxes and wheels), and the lumpy rewards are usually doubled by an ad.

### Gaps
- There is no reliable source for the average coins per Subway Surfers run or Archero gold per full chapter clear. Community spreadsheets exist (Archero wiki links a Google Sheet), but they were not accessible.
- The exact gem amounts per AB2 daily quest and per level are not documented anywhere I could reach.

## Q2 - Daily logins, daily missions/challenges and free chests

### Takeaway
All three games use a daily habit loop: a daily challenge with a streak reward, free timed boxes, and a daily ad ladder. Subway Surfers has the most explicit version: a 5-step ad ladder plus a free Mini Mystery Box every 4 hours.

### Cited Findings
- **Subway Surfers daily rewards (ad ladder):** 1 = Mini Mystery Box (no ad), 2 = 3 keys, 3 = 1,500 coins, 4 = Super Mystery Box, 5 = 3 keys — [Wiki: Daily Video](https://subwaysurf.fandom.com/wiki/Daily_Video)
- The Keys page adds that the Daily Video gives 3 keys on the 1st and 4th watch, and up to 1 random key from the 5th watch onward — [Wiki: Keys](https://subwaysurf.fandom.com/wiki/Keys)
- **Subway Surfers free box:** a Mini Mystery Box can be claimed free every 4 hours in the shop — [Wiki: Mini Mystery Box](https://subwaysurf.fandom.com/wiki/Mini_Mystery_Box)
- **Subway Surfers Daily Calendar (since 2021):** lasts about 10–20 days. It gives coins, event coins, headstarts, score boosters, boxes, boards and keys, with an exclusive board or character at the end (or 5 keys if already owned) — [Wiki: Daily Calendar](https://subwaysurf.fandom.com/wiki/Daily_Calendar)
- **Subway Surfers daily/season quests:** daily quest sneakers give up to 3–6 keys. Season quests give 5 keys at 3,500 sneakers and 10 keys at 4,000 sneakers. Collection milestones give 5 keys each — [Wiki: Keys](https://subwaysurf.fandom.com/wiki/Keys)
- **Subway Surfers Season Hunt:** a collect-tokens-daily track that ends with an exclusive board or character each season. Tiers give 2 keys (tiers 0/5/10) and 5 keys (tiers 15–31) — [Wiki: Season Hunt](https://subwaysurf.fandom.com/wiki/Season_Hunt); [Wiki: Keys](https://subwaysurf.fandom.com/wiki/Keys)
- **Archero:** a daily reward pack is available by watching an ad — [Wiki: Gems](https://archero.fandom.com/wiki/Gems). The Time Reward (idle gold) is tied to talents — [Wiki: Gold](https://archero.fandom.com/wiki/Gold)
- **Angry Birds 2:** there are 6 daily quests, each giving "a small amount of gems", and completing all 6 gives a feather reward — [Wiki: Angry Birds 2](https://angrybirds.fandom.com/wiki/Angry_Birds_2)
- In 2015, AB2 had daily rewards that could be doubled by watching a video ad — [PocketGamer.biz IAP Inspector, 26 Aug 2015](https://www.pocketgamer.biz/iap-inspector-angry-birds-2/)
- AB2 Daily Deals (since late 2021) refresh every 15–24 h and sell apples, feathers, spells and tickets. Some items can be claimed with a ≤30 s ad, and exotic hats sometimes appear at half their 10,000 Black Pearl price — [Wiki: Angry Birds 2](https://angrybirds.fandom.com/wiki/Angry_Birds_2)

### Inferences
- A good benchmark for Pop Archer is a **daily ad ladder of about 5 steps** that mixes soft currency, premium currency and a box. On top of that, a **7-day streak** can pay a big chest, as in the AB2 Daily Challenge.

### Gaps
- I found no classic 7-day login calendar with per-day amounts for any of the three games. Subway Surfers uses the Daily Calendar and AB2 uses streaks instead, but per-day coin amounts were not documented.

## Q3 - Rewarded video ads: what they grant and daily caps

### Takeaway
Daily ad caps are small and explicit: Archero allows 4 energy ads and 5 wheel ads per day, and Subway Surfers has a 5-step daily ad ladder. Every game also offers "double this prize" and "continue" ads.

### Cited Findings
- **Archero:** an ad gives +5 energy, **up to 4 times per day** — [Wiki: Energy](https://archero.fandom.com/wiki/Energy); [Game Developer 2019](https://gamedeveloper.com/design/finding-the-fun-archero-part-3---monetization)
- **Archero ad Lucky Wheel:** **5 per day** after a boss wheel. Odds are 1/6 for 20 gems, 1/3 for 10 gems and 1/2 for gold (scales with chapter). The wiki calls it "the only true income of gems aside from leveling up" — [Wiki: Lucky Wheel](https://archero.fandom.com/wiki/Lucky_Wheel)
- **Subway Surfers:** in-run Coin Doubler ads give double coins for 45 s with one ad, or for the rest of the run with two ads — [Wiki: Coin](https://subwaysurf.fandom.com/wiki/Coin)
- Subway Surfers box prizes can be doubled by an ad — [Wiki: Mystery Box](https://subwaysurf.fandom.com/wiki/Mystery_Box)
- Missions can be skipped with an ad — [Wiki: Missions](https://subwaysurf.fandom.com/wiki/Missions)
- Every 10 ads watched give "Mystery Box Mania" (1 Mini + 1 Mystery + 1 Super Mystery Box) — [Wiki: Mystery Box](https://subwaysurf.fandom.com/wiki/Mystery_Box)
- Occasional Subway Surfers events give 20 headstarts for 10 ads — [Wiki: Headstart](https://subwaysurf.fandom.com/wiki/Headstart)
- **Angry Birds 2:** an ad restores 1 life. This was confirmed in 2015 — [PocketGamer.biz 2015](https://www.pocketgamer.biz/iap-inspector-angry-birds-2/) — and is still listed on [Wiki: Lives](https://angrybirds.fandom.com/wiki/Lives)
- During the energy era (2024–2026), AB2 energy could also be refilled by an ad — [Wiki: Energy](https://angrybirds.fandom.com/wiki/Energy)
- AB2 Wonder Whirl: 5 ad spins, then it closes until it reopens — [Wiki: Black Pearls](https://angrybirds.fandom.com/wiki/Black_Pearls)

### Inferences
- Expected gems from Archero's 5 daily wheel ads: (1/6×20 + 1/3×10) × 5 ≈ **33 gems/day**, plus gold on about half the spins (calculated). At the 2019 price of about 100 gems = $1.25, that is roughly **$0.40 of premium value per day** from ads.
- For Pop Archer, caps of **4–5 ads per reward type per day** match the genre.

### Gaps
- I found no official statement of a daily cap on Subway Surfers Coin Doubler or revive ads, or on AB2 life ads.

## Q4 - Soft/hard currency prices (boosters, continue/revive, energy refill, cosmetics)

### Takeaway
Across the three games, **continues are cheap and escalate**: Subway Surfers revives cost 1, 2, 4… keys, Archero's first revive costs 30 gems, and an AB2 continue cost 60 gems (2015). **Pre-run boosters cost about 0.3–4× a typical early run's coins.** **Cosmetics are the big sinks**: 50,000–95,000 coins, or 30–150 keys, in Subway Surfers.

### Cited Findings
**Subway Surfers (coins/keys)**
- Hoverboard (consumable, one-crash shield for 30 s): **300 coins** — [Wiki: Power-Ups](https://subwaysurf.fandom.com/wiki/Power-Ups); [Wiki: Hoverboard](https://subwaysurf.fandom.com/wiki/Hoverboard)
- Headstart: **2,000 coins** — [Wiki: Headstart](https://subwaysurf.fandom.com/wiki/Headstart)
- Score Booster: **3,000 coins** — [Wiki: Score Booster](https://subwaysurf.fandom.com/wiki/Score_Booster)
- Mystery Box: **500 coins** — [Wiki: Mystery Box](https://subwaysurf.fandom.com/wiki/Mystery_Box)
- Power-up duration upgrades (Magnet, Jetpack, Sneakers, 2X): **500 → 1,000 → 3,000 → 10,000 → 60,000 coins**, +5 s each — [Wiki: Power-Ups](https://subwaysurf.fandom.com/wiki/Power-Ups)
- **Revive ("Save Me"): 1 key the first time, then the cost doubles** on each later revive in the same run — [Wiki: Keys](https://subwaysurf.fandom.com/wiki/Keys)
- Coin-priced limited characters cost **95,000 coins**. Limited hoverboards cost **50,000 coins** (Bamboo 20,000). No coin-priced characters were released in 2022–2025; newer ones use keys or event coins — [Wiki: Coin](https://subwaysurf.fandom.com/wiki/Coin)
- Character outfits in keys: the most common price on the outfits page is **75 keys** (121 occurrences), then 150 keys (48), 30 keys (16) and 100 keys (12). I counted these myself from the page's wikitext, so treat them as an approximate distribution — [Wiki: Outfits](https://subwaysurf.fandom.com/wiki/Outfits)
- Token Box (character tokens): **6 keys** — [Wiki: Token Box](https://subwaysurf.fandom.com/wiki/Token_Box)

**Archero (gems)**
- Energy refill: **100 gems = 20 energy** — [Wiki: Energy](https://archero.fandom.com/wiki/Energy)
- Revive: **30 gems** at the first death, with a 5-second decision window (2019) — [Game Developer 2019](https://gamedeveloper.com/design/finding-the-fun-archero-part-3---monetization)
- Golden Chest: **60 gems** (common/great gear). Obsidian Chest: **300 gems** (great/rare/epic) (2019) — [Game Developer 2019](https://gamedeveloper.com/design/finding-the-fun-archero-part-3---monetization)
- Gold is spent on talents, equipment, heroes and scroll upgrades — [Wiki: Gold](https://archero.fandom.com/wiki/Gold)

**Angry Birds 2 (gems)**
- Continue a failed level (refill birds): **60 gems**. Full lives refill: **60 gems**. Spells: **20–30 gems** each (2015) — [PocketGamer.biz 2015](https://www.pocketgamer.biz/iap-inspector-angry-birds-2/)
- Energy refill during the 2024–2026 energy era: **50 gems** — [Wiki: Energy](https://angrybirds.fandom.com/wiki/Energy)
- Card shuffles and room restarts in the Arena, Clan Battles and Bootcamp cost gems, and **the cost doubles each time** — [Wiki: Gems](https://angrybirds.fandom.com/wiki/Gems)
- Rowdy Rumble retry: **200 gems**, or free after a 3 h wait — [Wiki: Angry Birds 2](https://angrybirds.fandom.com/wiki/Angry_Birds_2)
- Hats (cosmetics that also add a power bonus) are bought with **Black Pearls**. Exotic hats cost **10,000 Black Pearls** and are sometimes half price in Daily Deals — [Wiki: Angry Birds 2](https://angrybirds.fandom.com/wiki/Angry_Birds_2)
- Mighty-rarity hats are bought with Mighty Eagle Coins — [Wiki: Black Pearls](https://angrybirds.fandom.com/wiki/Black_Pearls)
- Black Pearl Whirl: after 5 gem spins it closes until it reopens — [Wiki: Black Pearls](https://angrybirds.fandom.com/wiki/Black_Pearls)

### Inferences
- Subway Surfers price ratios (estimate): a consumable shield (300) costs about **1–3 early runs**. A booster (2,000–3,000) costs about **10–20 early runs**. A premium cosmetic board (50,000) costs **hundreds of runs** unless boxes and events help, or about **4.60–6.70 € of coins** at store rates (see Q6).
- The escalating continue price (doubling) is a standard pattern and fits Pop Archer's revive.

### Gaps
- I found no current (2024–2026) gem prices for the Archero revive or chests, or for the AB2 continue. Those figures are from 2015/2019 and may have changed.
- AB2 hat prices in Black Pearls by rarity (below Exotic) are not documented in what I read.

## Q5 - Energy/lives systems

### Takeaway
Archero uses **20 energy, 5 per run, 1 energy every 12 min** (4 h to refill), with 100 gems for a refill and 4 ad refills a day. AB2 has changed systems several times: 5 lives, then **60 energy / 10 per level (Feb 2024)**, then lives again (the wiki says since June 2026). Subway Surfers has **no energy at all**.

### Cited Findings
- **Archero:** max 20 energy, **1 energy every 12 minutes**, a full bar every 4 hours. An ad gives +5 energy (4/day). 100 gems buys +20 energy. Stage rewards give +5 energy — [Wiki: Energy](https://archero.fandom.com/wiki/Energy)
- Archero runs cost **5 energy** each, and the Battle Pass raises the energy cap from 20 to 30 for the season (2019) — [Game Developer 2019](https://gamedeveloper.com/design/finding-the-fun-archero-part-3---monetization)
- **Angry Birds 2 lives:** 5 lives, **30 min per life**, 2.5 h to full — [Wiki: Lives](https://angrybirds.fandom.com/wiki/Lives). This conflicts with PocketGamer.biz (2015), which says **10 min per life** — [PocketGamer.biz 2015](https://www.pocketgamer.biz/iap-inspector-angry-birds-2/). The regen time has likely changed between versions.
- **AB2 energy era (Feb 2024 →):** max 60 energy, **10 energy per level played (win or lose)**, recharge "every 3 minutes", refill with ads or **50 gems**, and 250 energy granted at launch — [Wiki: Energy](https://angrybirds.fandom.com/wiki/Energy)
- Rovio's announcement confirms that energy is "consumed at the start of each level", recharges up to a limit, and can be exceeded with gems or an ad. It gives no numbers — [angrybirds.com: New energy system](https://www.angrybirds.com/stories/new-energy-system-on-world-map/)
- **AB2 June 2026:** the wiki states that "as of June 2026, the energy system is removed throughout all devices and replaced with lives again" — [Wiki: Energy](https://angrybirds.fandom.com/wiki/Energy). **This is unverified with an official source.**
- **Subway Surfers:** no energy or lives; runs are unlimited. The pressure points are keys for revives and time-limited seasons — [Wiki: Keys](https://subwaysurf.fandom.com/wiki/Keys); [Wiki: Season Hunt](https://subwaysurf.fandom.com/wiki/Season_Hunt)

### Inferences
- The AB2 story, going from lives to energy and back to lives, and the reviewer and player complaints ("energy is worse than lives because it charges on wins too") suggest that **lives lost only on failure** are better received in a level-based arcade game. That is a caution for Pop Archer against charging energy on wins.
- Archero's arithmetic: about 4 runs per full bar, plus 4 runs from the 4 ads (5 energy each) ≈ **8 free runs a day** without regen during play (calculated).

### Gaps
- The exact AB2 life regen time in the post-June-2026 version is unknown, as is the current gem cost to refill lives.

## Q6 - Real-money offers (currency packs, starter pack, ad removal, pass)

### Takeaway
Entry offers sit at 0.99–1.99 €. The standard soft-currency pack ladder runs 0.99 € → 5.99 € → 9.99 €/10.99 € → 20.99 €/22.99 €. Battle passes cost 5.99 € (Archero; 19.99 € for the premium version), and there is a 2.99 €/month subscription (Archero). Subway Surfers sells a **permanent Double Coins upgrade** (5.99 € / $4.99). **None of the three sells a plain "remove ads" item** in its store list.

### Cited Findings
**Subway Surfers — App Store France IAP list (2026-09-30):**
- 7,500 Coins 0.99 €; Coin Pack 1 0.99 €; 12 500 Coins 0.99 €; Starter Pack 0.99 €
- 45,000 Coins 5.99 €; 65 000 Coins 5.99 €; 25 Keys 5.99 €; 40 Keys 5.99 €
- Double Coins 5.99 €; 180,000 Coins 22.99 €
- Source: [App Store FR, Subway Surfers](https://apps.apple.com/fr/app/id512939461)
- Full coin pack ladder in USD: 7,500 = $0.99; 40,000 = $4.99; 90,000 = $9.99; 200,000 = $19.99; 550,000 = $49.99; 1,250,000 = $99.99; Double Coins = $4.99 — [Wiki: Coin](https://subwaysurf.fandom.com/wiki/Coin)
- Key packs in USD: 25 = $4.99; 55 = $9.99; 125 = $19.99; 350 = $49.99; 800 = $99.99 — [Wiki: Keys](https://subwaysurf.fandom.com/wiki/Keys)
- Unlimited Double Coins is permanent, $4.99 — [SYBO Help Center](https://sybo.helpshift.com/hc/en/5-subway-surfers/faq/147-coins/)
- The wiki mentions a Mumbai-era starter pack (7,000 coins plus keys and hoverboards for $0.99) — [Wiki: Coin](https://subwaysurf.fandom.com/wiki/Coin)
- Welcome Pack: one-time, up to 45K coins — [Wiki: Coin](https://subwaysurf.fandom.com/wiki/Coin)

**Archero — App Store France IAP list (2026-09-30):**
- Battle Pass 5.99 €; Battle Pass 2 19.99 €
- Value Pack for First Purchase 1.99 €; Value Pack for C1 1.99 €; Value Pack for Chapter 2 7.99 €; Value Pack for Chapter 3 9.99 €
- Pile of Gems 0.99 €; Heap of Gems 5.99 €; Carte mensuelle (monthly card) 2.99 €; Coin Push 0.99 €
- Source: [App Store FR, Archero](https://apps.apple.com/fr/app/id1453651052)
- US prices (Mar 2026): Battle Pass $4.99, Battle Pass 2 $16.99, Chapter 2 pack $6.99, Growth Pack $0.99, Monthly Card $2.99 — [AppPricingLab, Archero IAP](https://apppricinglab.com/iap/apple/1453651052)
- Subscription at $2.99 per calendar month: +3 daily Quick Raids, +50 Battle Pass XP per day, **+50 bonus gems per day** — [App Store FR description](https://apps.apple.com/fr/app/id1453651052)
- Gem pack quantities: Pile 80 gems $0.99; Heap 500 $4.99; Bucket 1,200 $9.99; Barrel 2,500 $19.99; Chest 6,500 $49.99; Cart 14,000 $99.99. This comes from a search-engine summary attributed to [ProGameGuides](https://progameguides.com/archero/archero-gems-coins-energy-guide/); the page itself returned 403 and **could not be verified**, so treat it as unconfirmed.
- 2019 figures: 100 gems ≈ $1.25. The beginner chapter bundle contained 300 gems + 10,000 gold + 5 free revives, and chapter bundles unlock in sequence — [Game Developer 2019](https://gamedeveloper.com/design/finding-the-fun-archero-part-3---monetization)
- The Battle Pass has 16 levels per season, adds 16 extra rewards over the free track, and unlocks at chapter 2 stage 26 — [Wiki: Battle Pass](https://archero.fandom.com/wiki/Battle_Pass)

**Angry Birds 2 — App Store France IAP list (2026-09-30):**
- Tas de gemmes (pile of gems) 0.99 €; Bourse de gemmes précieuse (precious pouch of gems) 4.99 €; Coffre de gemmes (chest of gems) 9.99 €
- Sacs de gemmes précieux (precious bags of gems) 20.99 €; Lettre de gemmes (gem letter) 9.99 €
- Themed bundles: Blues' Bling 5.99 €; Chuck's Cheddar 10.99 €; Matilda's Marvels 20.99 €
- Offer Pack 699 6.99 €; Offer Pack 999 9.99 €
- Source: [App Store FR, Angry Birds 2](https://apps.apple.com/fr/app/id880047117)
- The US list adds Food Pack S $0.99 and Terence's Treasure $99.99 (Mar 2026) — [AppPricingLab AB2](https://apppricinglab.com/app/apple/880047117)
- Gem quantities (**2015**): 80 gems $0.99; 560 gems $7.49; 5,700 gems $49.99. The launch "Early Bird" offer was 160 gems for $0.99 — [PocketGamer.biz 2015](https://www.pocketgamer.biz/iap-inspector-angry-birds-2/)
- Treasure Pass (since Nov 2023): daily and epic quests raise its rank. The free track gives feathers and Black Pearls; a paid track adds cosmetic sparkles and more — [Wiki: Angry Birds 2](https://angrybirds.fandom.com/wiki/Angry_Birds_2)

### Inferences
- **Subway Surfers coin value** (calculated): 7,500 coins/0.99 € ≈ **7,600 coins per €** on the base pack, and the promo packs (12,500 or 65,000 coins) reach ≈ **10,900–12,600 coins per €**. That makes a 50,000-coin board worth about **4–7 €**, a 95,000-coin character about **8–12.50 €**, and a 300-coin hoverboard about **0.03–0.04 €**. Keys cost about **0.15–0.24 € each** (25–40 keys for 5.99 €), so a 75-key outfit is worth about **11–18 €**.
- **Archero gem value** (calculated, using the unverified quantities): about 81–100 gems per €. The 30-gem revive is then worth about **0.30–0.37 €** and a 100-gem energy refill about **1.00–1.25 €**.
- **Pop Archer benchmark:** a 0.99 € starter pack, a 1.99 € first-purchase pack, a 5.99 € mid pack, 9.99–10.99 € and about 20.99 € upper packs, a 5.99 € pass (with an optional premium version around 19.99 €), and a permanent "double coins" at about 5.99 €. That last one is a good alternative to "remove ads" in a coin-shop game.

### Gaps
- Current (2026) AB2 gem quantities per pack and the Treasure Pass price were not found.
- Current Archero gem pack quantities and battle pass contents are only partly verified.
- A 4th comparable game (bubble shooter or stickman casual) **was not researched**, for lack of time or tool budget.
- Archero 2 (Habby, 2024–2025), which may be a more current comparison, was not covered.
