# Kit d'enregistrement — Épisode 01 : le capacitor (darija)

Une phrase = un fichier. Tu peux refaire **une seule phrase** quand tu veux : l'animation se recale toute seule.

## Conseils (5 minutes de préparation)

- Pièce calme, porte fermée, pas de ventilateur. Une pièce avec des rideaux / un lit résonne moins qu'une cuisine.
- Téléphone à 15–20 cm de la bouche, un peu sur le côté (pour éviter les « p » qui claquent).
- Application dictaphone de base, format par défaut (m4a, mp3, wav… tout est accepté).
- Attends 1 seconde avant de parler et après la fin : les silences sont coupés automatiquement.
- Parle comme si tu expliquais à un ami. Pas besoin d'être parfait : le volume, le souffle et les longues pauses sont corrigés automatiquement.
- Les mots en anglais (capacitor, plates…) : prononce-les naturellement, comme tu les dirais en cours.

## Les phrases

Nomme chaque fichier exactement comme indiqué (le numéro est optionnel).

| Fichier | Ce que tu dis | À l'écran pendant ce temps |
|---|---|---|
| `1-intro.m4a` | <span dir="rtl">السلام عليكم، واش راكم؟ معاكم Artronic! اليوم نفهمو الـ capacitor، بالساهل.</span> | Logo + titre « The Capacitor » |
| `2-plaques.m4a` | <span dir="rtl">الـ capacitor، هو زوج plates تاع المعدن، وبيناتهم insulator. الـ current ما يفوتش… بصح الـ charges، يقدرو يتلمو.</span> | Les 2 plaques, l'isolant, le courant bloqué, le symbole |
| `3-charge.m4a` | <span dir="rtl">كي نسكرو الـ switch، الـ electrons يخرجو من plate ويتلمو في لوخرا. الـ capacitor راهو يتشارجا.</span> | Le circuit, l'interrupteur se ferme, les électrons circulent |
| `4-courbe.m4a` | <span dir="rtl">في اللول، تروح بالخف بزاف، ومن بعد بالشوية بالشوية. الـ voltage تتبع هاد الـ curve.</span> | La courbe de tension se dessine |
| `5-tau.m4a` | <span dir="rtl">الوقت المهم هو tau، يساوي R fois C. بعد tau واحد، نوصلو لـ 63 percent. وبعد 5 tau، راهو تقريبا fully charged.</span> | τ = R × C, repères 63 % et 5τ |
| `6-exemple.m4a` | <span dir="rtl">مثال: 10 kilo-ohms و 100 microfarads، يعطيونا tau يساوي 1 second. المرة الجاية: الـ inductor. ما تنساوش دير subscribe!</span> | Le calcul 10 kΩ × 100 µF = 1 s, puis « Next episode » |

Tu peux reformuler à ta façon : c'est ta voix et ton style. Si tu changes beaucoup une phrase, envoie-moi aussi le texte pour les sous-titres.

## M'envoyer les fichiers

Mets-les dans un dossier Google Drive (par ex. « Artronic / ep01-darija ») et dis-le-moi : je les récupère, je les nettoie et je monte la vidéo.

(Version technique : `python tools/import_voice.py episodes/01-condensateur dz <dossier>` puis `./build.sh episodes/01-condensateur Condensateur dz`.)
