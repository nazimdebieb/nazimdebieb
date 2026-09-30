# F2P casual mobile economy benchmarks (for calibrating Pop Archer's coin economy)

Scope: public rules of thumb and benchmark numbers for soft-currency earn/spend, daily rewards, rewarded ads, IAP pricing and lives/energy in casual mobile games. Research date: 2026-09-30. About 20 tool calls. Several key pages could not be fetched (Medium/Liquid and Grit article on Royal Match's economy returned a Cloudflare block; Candy Crush fandom wiki returned HTTP 402; Appodeal's Q4 2024 eCPM PDF is image-only). Where a figure comes only from a search-engine summary and not a fetched page, it is marked **[snippet]**. Where it is a practitioner's recommendation rather than measured data, it is marked **[opinion]**. Sources from before 2023 are marked **[OLD: year]**.

## 1. Earn-to-spend ratios: how many levels of play should a booster, a continue, a lives refill or a cosmetic cost?

### Takeaway
I found no published, sourced rule such as "a booster ≈ N levels of income" or "a cosmetic ≈ N weeks of play". Public writing on economy design stays qualitative ("balance sources and sinks", "A/B test"). The closest concrete anchor is Royal Match, where one extra life or continue costs 900 coins. That price is widely reported, but I could not verify Royal Match's coins-per-level from a fetchable source.

### Cited Findings
- Every currency needs a source (faucet) and a sink. If players earn more than they can spend, the currency loses value. If it is too scarce, progression feels slow. **[opinion / definitional]** — [Unity, Building a game economy guide pt 2](https://activation.unity3d.com/how-to/building-game-economy-guide-part-2) **[snippet]**; [MWM glossary: game economy](https://mwm.ai/glossary/game-economy) **[snippet]**
- In a typical casual loop, players earn coins for finishing levels and spend them on lives, boosters and other resources. Enough enticing sinks plus a balanced source/sink ratio "ultimately drive monetization". **[opinion]** — [Unity economy guide pt 2](https://activation.unity3d.com/how-to/building-game-economy-guide-part-2) **[snippet]**
- Economies with more faucets than sinks devalue the currency over time and break late-game pacing and monetization. **[opinion]** — [MWM glossary](https://mwm.ai/glossary/game-economy) **[snippet]**; see also [Bruin "soft currency oversupply projection" use case](https://getbruin.com/use-cases/mobile-gaming/soft-currency-oversupply-projection/) **[snippet]**
- Royal Match: an extra life or life purchase costs **900 coins**. Players can hold up to **8 lives**, and one life refills every **30 min** (Feb 2024). — [esports.net, Royal Match free lives](https://www.esports.net/news/mobile-games/royal-match-free-lives/). *Caveat:* 900 coins is more commonly described as the price of the first "+5 moves" continue. This fan site may be mixing the two up. Verify in-game.
- Royal Match's coin-earning channels are level completion, area-completion chests, the battle pass, bonus levels / King's Nightmare, and events (Team Battle, Book of Treasure, etc.). The article gives **no amounts** (Feb 2024). — [esports.net, Royal Match free coins](https://www.esports.net/news/mobile-games/royal-match-free-coins/)
- Kolibri (Idle Miner Tycoon) observed that players tend to **hoard**: "Most players are saving their Super Cash for a rainy day" and "they save a few more boosts than they receive over an average 30 day period". Over 50% of boosts come from expedition rewards. **[data, idle genre] [OLD: 2020]** — [Kolibri Games, Economy in F2P mobile games pt 2](https://kolibrigames.com/blog/economy-in-free-to-play-mobile-games-part-2/)
- A Liquid and Grit analysis titled "Royal Match's economy is more generous and less costly than Candy Crush Saga's" exists and probably has per-level coin and continue figures, but it could not be fetched. — [Liquid and Grit (Medium)](https://blog.liquidandgrit.com/royal-matchs-economy-is-more-generous-and-less-costly-than-candy-crush-saga-s-744f43389104)

### Inferences
- The Kolibri hoarding finding suggests that in a casual game, the default failure is players stockpiling coins and boosters, not running short. Sinks need to be attractive and time-sensitive (continue at the moment of failure, limited-time outfits).
- For Pop Archer, a working heuristic (my inference, not sourced) is to price a continue at roughly 10–20 level wins of coins. If Royal Match's continue is 900 coins and a typical early-level win is worth a few dozen coins, that puts a Royal Match continue at the equivalent of many level wins, which is what makes coin packs worth buying. Cosmetics are pure sinks with no power effect, so they can be priced at the equivalent of several days of free income without harming progression.
- Keep one soft currency and model it in a spreadsheet: coins/day for a median player (≈4 sessions/day × a few levels, see §5) against planned daily spend. Target a slight deficit for engaged players, so that the free faucets (daily reward, ads) close the gap and coin packs feel useful.

### Gaps
- No sourced "N levels of income per booster / continue / cosmetic" rule was found, whether from Deconstructor of Fun, Mobile Free To Play, GameRefinery or Machinations. Those sites returned no fetchable page with such numbers in this session.
- Verified coins-per-level for Royal Match or Candy Crush was not obtained. The Liquid and Grit article was blocked.

## 2. Source/sink balancing: share of soft currency from gameplay vs daily rewards vs ads vs events; inflation risks

### Takeaway
No public benchmark splits casual-game soft-currency income by source (gameplay vs daily vs ads vs events). Practitioners stress two things: make sure sinks keep pace with faucets, and use diminishing rewards on repeatable free sources, such as successive ad watches paying less.

### Cited Findings
- Inflation risk: more faucets than sinks devalues currency and breaks late-game pacing. **[opinion]** — [MWM glossary](https://mwm.ai/glossary/game-economy) **[snippet]**
- Kolibri injects premium currency through advancement rewards, daily rewards, expedition rewards and event rewards, besides IAP. "A game economy… is healthy when money is constantly moving through it." **[OLD: 2020]** — [Kolibri Games](https://kolibrigames.com/blog/economy-in-free-to-play-mobile-games-part-2/)
- Diminishing returns on ad rewards, e.g. 1st ad = 100 gems, 2nd = 50, 3rd = 25. This protects the economy and pushes players to return the next day. **[opinion]** — [Applixir blog](https://www.applixir.com/?p=2944) **[snippet]**
- LiveOps (events) matter commercially: "84% of all mobile in-app purchase revenue came from games using live operations" in 2024 (Adjust 2024, relayed by a secondary blog). — [Juego Studio, ARPDAU benchmarks](https://www.juegostudio.com/blog/arpdau-benchmarks-by-game-genre)

### Inferences
- Suggested starting mix for Pop Archer (my inference, to be tuned with analytics): most daily coins from level play (roughly 60–70%), with the daily reward, rewarded ads and events each supplying a minority. Cap and taper the ad faucet so it cannot replace coin packs.
- Adding events later is the main inflation risk. Pair every new faucet (event rewards) with a new sink (limited outfits, event boosters).

### Gaps
- No sourced data on the actual share of soft-currency inflow by source in casual games.

## 3. Daily login rewards: 7-day streak curves and what is given

### Takeaway
Sourced material is thin and mostly non-authoritative. The common pattern is an escalating 7-day ladder with a large day-7 payout. One example curve is 5, 5, 10, 10, 15, 15, 50, so day 7 = 10× day 1. Some games also spread a purchase over up to 30 days as a "daily benefits" pack.

### Cited Findings
- Example escalating streak: **5, 5, 10, 10, 15, 15, 50** tokens (day 7 = 10× day 1; total 110, of which day 7 is ≈45%). **[opinion / example; low-authority site]** — [thefestivals.uk, Why daily rewards keep players coming back](https://thefestivals.uk/?p=42821) **[snippet]**
- Escalating rewards that "grow bigger the longer your streak runs" are the most effective mechanic. The retention effect is claimed to be strongest after about one week of streak. **[opinion; unsourced claim]** — same source **[snippet]**
- Daily-benefit ("annuity") purchases spread rewards over time, e.g. 3,000 premium currency paid as 100/day over 30 days if the player logs in. This drives both retention and first purchase. **[opinion] [OLD: 2016]** — [PocketGamer.biz, Matt Suckley, Best practices for starter bundles](https://www.pocketgamer.biz/comment-and-opinion/64530/best-practices-starter-bundles/entry/1/)
- A "Free Daily Chest" at the top of the shop is a high-converting rewarded-ad placement. **[opinion]** — [Applixir blog](https://www.applixir.com/?p=1792) **[snippet]**

### Inferences
- For Pop Archer: a 7-day ladder with day 7 at about 5–10× day 1. Days 1–6 in coins, with one booster on about day 3 or 5, and day 7 as a chest (coins + boosters + maybe 30–60 min of unlimited lives). The whole week should equal roughly 1–2 days of gameplay income so it does not inflate the economy (inference).
- Decide explicitly whether a missed day resets the streak or only pauses it. No benchmark was found on which retains better.

### Gaps
- No authoritative (Deconstructor of Fun / GameRefinery / GameAnalytics) data on 7-day curves, reset rules or the retention lift of daily rewards in casual games.

## 4. Rewarded ads: placements, daily caps, eCPM ranges and share of revenue vs IAP

### Takeaway
Standard placements are revive/continue at game over, double the level reward, a free daily chest in the shop, and an extra life when out of lives. Practitioners cap rewarded views at roughly 3–8 per day. In mature Western markets IAP is 77–90% of game revenue and ads 10–23%. Puzzle games take 53% of all game ad revenue. Rewarded-video eCPMs run roughly $12–14 in the US and $3–6 in Western Europe; iOS is at least double Android in Europe.

### Cited Findings
**Placements and caps**
- High-converting placements: game-over "watch to revive" / "double your coins"; "double your reward" after a level; a "Free Daily Chest" in the shop. **[opinion]** — [Applixir blog](https://www.applixir.com/?p=1792) **[snippet]**
- Caps: **3–5 rewarded videos/day or 1 per session**. For a shopfront slot, cap at **5/day/user** so that when free rewards run out, players are nudged toward IAP. **[opinion]** — [Applixir support: Best practices](https://support.applixir.com/hc/en-us/articles/360056835154-Best-Practices-); [InMobi, Three commandments of rewarded ads](https://www.inmobi.com/blog/succeeding-with-rewarded-ads-the-three-commandments) **[snippet]**
- For casual games: a **5–8 frequency cap per 20–50 minutes** of play. **[opinion]** — [Pangle, Rewarded video guideline](https://www.pangleglobal.com/knowledge/rewarded-video-ads-guideline) **[snippet]**
- Cooldown of **2–5 min** between ad offers. Do not show ad prompts in a new user's first few minutes. **[opinion]** — [Applixir blog](https://www.applixir.com/?p=2944) **[snippet]**
- Unity (Feb 2025): "over 60% [of players] are interested in engaging with rewarded ad placements" versus about 3% converting to IAP. Best moment: "when a player has run out of in-game currency or retries". Unity gives no numeric caps and recommends A/B testing. **[data + opinion]** — [Unity blog, Rewarded ad systems (N. TenBoer, 2025-02-13)](https://unity.com/blog/rewarded-ad-systems)

**eCPM (rewarded video)**
- Q4 2024 (Appodeal): US rewarded video **$13.75 iOS / $12.01 Android**; Western Europe **$5.77 iOS / $2.95 Android**. **[data, via search summary; the PDF is image-only]** — [Appodeal Q4 2024 eCPM report](https://appodeal.com/the-mobile-ecpm-report-updated-q4-2024-view); [PDF](https://appodeal.com/wp-content/uploads/2025/03/Quarterly_Mobile_eCPM_Report_-_Q4_2024.pdf) **[snippet]**
- H2 2021–H1 2022 (Appodeal, 100k+ apps). Rewarded video: US $12.91 Android / $13.81 iOS; W. Europe $3.62 Android / $6.51 iOS; E. Europe $1.49 / $2.55. Interstitial: US $11.06 / $9.58; W. Europe $3.31 / $3.68. Banner: US $0.60 / $0.27; W. Europe $0.22 / $0.15. **[data] [OLD: 2021–22]** — [Udonis, eCPMs](https://blog.udonis.co/mobile-marketing/mobile-apps/ecpms)
- A secondary source claims US iOS rewarded eCPMs "commonly range $10–25 in 2026" and W. Europe iOS $5–12. **[low-confidence aggregator]** — [MWM glossary: eCPM](https://mwm.ai/glossary/ecpm) **[snippet]**

**Ads vs IAP share**
- Sensor Tower (ad data Jan 2025–May 2026). In mature markets (US, Canada, S. Korea, Japan), **IAP = 77–90% of revenue, ads = 10–23%**. In growth markets (India, Indonesia, Brazil) ads = 55–70%. **Puzzle = 53% of all game ad revenue, arcade = 13%.** 84.1% of games are ad-supported (May 2026). Outside the top 1,000 games, ad revenue is less concentrated: those games earn 29% of ad revenue but only 9% of IAP. — [GameDev Reports, Sensor Tower: Mobile game ad monetization in 2026](https://gamedevreports.substack.com/p/sensor-tower-mobile-game-ad-monetization)
- The share of games using hybrid monetization grew from 36% (Q2 2023) to 43% (Q1 2024) (AppsFlyer, relayed). — [Juego Studio](https://www.juegostudio.com/blog/arpdau-benchmarks-by-game-genre)
- IAP-focused hybrid-casual games outperform ad-first hybrids "by a factor of four". The top 10 hybrid-casual games made $87M net IAP in Q1 2025 (+67% YoY), with puzzle 48% and arcade 45% of that. **[data, AppMagic]** — [GameDev Reports, AppMagic Top 10 hybridcasual Q1'25](https://gamedevreports.substack.com/p/appmagic-top-10-hybridcasual-games) **[snippet]**

### Inferences
- Using Q4 2024 figures, 3–5 rewarded views/day in France or other Western European countries is worth about $0.009–0.03/DAU/day from rewarded video alone (3–5 × $2.95–5.77 / 1000). That is small next to casual ARPDAU (§5). Size ad rewards generously enough to engage players, but cap them so they do not undercut coin packs.
- For a Western-focused casual game like Pop Archer, plan for IAP as the main revenue line and ads as a retention and first-value tool. Expect ads to be about 10–25% of revenue, plus remove-ads purchases.

### Gaps
- No verified 2025–2026 Appodeal/AppLovin/Unity rewarded eCPM tables for individual EU countries (FR/DE/UK) were fetched.
- No source gave typical rewarded views/DAU actually observed in casual games (only recommended caps).

## 5. Real-money pricing: price points, pack bonuses, starter pack, remove-ads, conversion, ARPDAU

### Takeaway
Use 6–8 tiers from $0.99 to $99.99, with $4.99 and $9.99 as the workhorses. Starter packs sit around $1.99–4.99 and offer 5–10× normal value, or a "70–90% discount" framing. Payer conversion in casual/puzzle is usually 0.5–2%; broader "industry" claims of 2–5% are optimistic. Casual/puzzle ARPDAU is roughly $0.03–0.10 (up to ~$0.30 for strong hybrids). The long tail spends much less.

### Cited Findings
**Price points**
- Top-grossing games keep **6–8 price tiers**: $0.99, $4.99, $9.99, $19.99, $49.99, $99.99. **[opinion/observation]** — [Unity, In-app purchases guide (2026-06-22)](https://unity.com/resources/in-app-purchases-guide)
- Games' IAP price distribution: $4.99 = 18%, $9.99 = 14%, $0.99 = 13%, $19.99 = 11% of listed IAP prices. **[data] [OLD: likely 2016–17; date not confirmed; page now redirects]** — [Appbot, In-app purchases of the top grossing apps](https://blog.appbot.co/in-app-purchases-of-the-top-grossing-apps/) **[snippet]**
- In one game's data, the $0.99 tier was 50% of transactions but 9% of revenue. $4.99 was 42% of both transactions and revenue. $29.99 was 8% of transactions but 49% of revenue. **[single-case data; original source not confirmed] [OLD]** — reported alongside [Appbot](https://blog.appbot.co/in-app-purchases-of-the-top-grossing-apps/) **[snippet]**
- Price anchoring: show a $99.99 mega-pack next to mid tiers so that mid tiers look reasonable. **[opinion]** — [Unity IAP guide](https://unity.com/resources/in-app-purchases-guide)

**Bonus % for bigger packs**
- No sourced benchmark was found for the "+x% bonus" ladder across coin packs (see Gaps).

**Starter pack**
- Starter pack **$1.99–$4.99**, **5–10× the value** of an equivalent standard purchase, time-gated **24–72 h** after install. **[opinion]** — [Unity IAP guide (2026)](https://unity.com/resources/in-app-purchases-guide)
- Offer it in the first 24 h (Amazon Appstore data: 18% of revenue happens in that window). Advertise **70–90% discounts** with strikethrough prices. Keep it available **3–7 days** for urgency. Include hard currency plus exclusive items. **[opinion + data] [OLD: 2016]** — [PocketGamer.biz, Matt Suckley](https://www.pocketgamer.biz/comment-and-opinion/64530/best-practices-starter-bundles/entry/1/)

**Remove ads**
- Examples from App Store listings: a Solitaire game sells remove-ads at $2.99/month, $6.99/3 months and $24.99/year. A match game sells weekly $0.99, monthly $1.99, permanent $3.99 and annual $5.99. A number-match game sells a one-time remove-ads for $7.99. **[data, individual listings; game names not captured]** — [App Pricing Lab listing 1632384400](https://apppricinglab.com/iap/apple/1632384400); [listing 6447614068](https://apppricinglab.com/iap/apple/6447614068); [Number Games – Match Numbers](https://apppricinglab.com/iap/apple/6739208242) **[snippet]**

**Conversion (% payers)**
- Casual/puzzle baseline payer conversion **0.5–1%**. "Less than 5%" of mobile gamers ever spend (AppsFlyer 2024, relayed). — [Juego Studio](https://www.juegostudio.com/blog/arpdau-benchmarks-by-game-genre)
- Unity claims an "industry standard" 2–5% payer conversion, 6–8% for high performers, and below 1% as a red flag. **[opinion; conflicts with the figures above]** — [Unity IAP guide](https://unity.com/resources/in-app-purchases-guide)
- GameAnalytics (relayed): only the most successful games reach over 1% conversion. The average is about 0.4%, and the bottom 15% show zero daily conversions. **[data, exact period unclear]** — [GameDev Reports, GameAnalytics hypercasual snapshot](https://gamedevreports.substack.com/p/gameanalytics-hypercasual-games-metrics) **[snippet]**
- AppsFlyer, US casual/puzzle: conversion 1.96% organic / 2.38% non-organic; ARPPU $36.06. **[data] [OLD: Q1 2017]** — [WN Hub](https://wnhub.io/news/marketing/item-12861)
- Top 5% of players generate 48% of IAP revenue, and the top 25% generate 77% (Moloco 2025, relayed). — [Juego Studio](https://www.juegostudio.com/blog/arpdau-benchmarks-by-game-genre)
- *Conflict:* Unity's 2–5% contradicts AppsFlyer/GameAnalytics' 0.4–1% for casual games. For planning a small casual title, the lower range is more defensible.

**ARPDAU**
- Casual/puzzle **$0.03–0.10**. Hyper-casual $0.01–0.05. Mid-core $0.08–0.20 (attributed to AppsFlyer 2024 / Appodeal 2025 / Moloco 2025 by a secondary blog). — [Juego Studio](https://www.juegostudio.com/blog/arpdau-benchmarks-by-game-genre)
- Casual/hyper-casual blended (IAP + ads) **$0.10–0.30** (2026). **[low-confidence aggregator]** — [MWM glossary: ARPDAU](https://mwm.ai/glossary/arpdau) **[snippet]**
- Hyper-casual lifetime ARPU ≈ $0.86 (Appodeal Mobile Casual Benchmarks 2025, relayed). — [Juego Studio](https://www.juegostudio.com/blog/arpdau-benchmarks-by-game-genre) **[snippet]**

**Engagement context (for converting "levels per day" into coins per day)**
- GameAnalytics 2025 benchmarks (11,600 games): median daily playtime **22 min**. Median session **5–6 min** (top 25%: 8–9 min). About **4 sessions/day**. D1 retention top 25% ≈ 26.5–27.7%. D7 median ≈ 3.4–3.9% (top 25%: 7–8%). Puzzle has the strongest D7–D28 retention. Arcade has high D1 but weak long-term retention. — [GameDev Reports, GameAnalytics benchmarks 2025](https://gamedevreports.substack.com/p/gameanalytics-mobile-gaming-benchmarks); [PDF](https://investgame.net/wp-content/uploads/2025/02/2025-GameAnalytics-Mobile-Gaming-Benchmarks.pdf)

### Inferences
- Suggested Pop Archer coin-pack ladder (inference): €0.99 / €4.99 / €9.99 / €19.99 / €49.99 (/ €99.99 as an anchor), with coins-per-euro rising at each step. A common convention is roughly +10–20% bonus per tier, up to about +50–100% at the top (unsourced here; verify against competitors).
- Starter pack: €1.99–€2.99, containing coins + boosters + an exclusive outfit, with a displayed "value" of 5–10×, shown after about 10–20 levels or on day 1–2 and limited to 48–72 h.
- Remove-ads: a one-time €3.99–€6.99 is consistent with the examples found. Keep rewarded ads available after purchase, since they are opt-in.
- Plan business cases at 1% payer conversion and $0.03–0.08 ARPDAU. Treat 2%+ as upside.

### Gaps
- No sourced benchmark was found for the bonus-% ladder across coin-pack tiers.
- No 2024–2026 primary data on the IAP price-point mix for casual games. The price-point shares above are old.
- No arcade-specific ARPDAU.

## 6. Lives/energy design: max lives, regen times, refill prices

### Takeaway
The genre standard is 5 lives regenerating 1 per 30 min (Candy Crush; Royal Match also uses 30 min, with a cap of up to 8 lives in some states). A full refill costs 9 gold bars in Candy Crush. Pop Archer's 20-min regen is more generous than the market leaders.

### Cited Findings
- Candy Crush: one life every **30 min** while below max. A full refill costs **9 gold bars**. **[data, fan wiki via search]** — [Candy Crush Saga Wiki: Lives](https://candycrush.fandom.com/wiki/Lives) **[snippet; page returned 402 on fetch]**
- Royal Match: one life every **30 min**. "Up to 8 lives", so a full refill takes at least 2.5 h. Extra life for **900 coins**. Joining a team (after level 20) lets players request free lives (Feb 2024). — [esports.net](https://www.esports.net/news/mobile-games/royal-match-free-lives/)

### Inferences
- Pop Archer (5 lives, 20 min, so 100 min for a full refill) is about 1.5× more generous than Candy Crush's 30 min (150 min). That is fine for a smaller game where retention matters more than friction. Price the lives refill in coins at about the same level as or slightly below a continue. Offer a rewarded ad for +1 life, capped per day.
- Timed "unlimited lives" (30 min – 2 h) make good day-7, event or starter-pack rewards, because they do not add to the coin supply.

### Gaps
- Candy Crush's max-lives count and the euro price of gold bars were not verified in this session (the wiki fetch failed).
- Royal Match's price for a full lives refill vs a single life was not verified.
