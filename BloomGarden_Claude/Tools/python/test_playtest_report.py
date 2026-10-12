#!/usr/bin/env python3
"""Unit test of playtest_report.py with a tiny SYNTHETIC fixture (parser check only, not player data).

    python Tools/python/test_playtest_report.py
"""
import tempfile, unittest
from pathlib import Path
import playtest_report as report

# Synthetic lines in the game's exact format, as a messaging app might deliver them (with extra text around).
TESTER_A = '''Voici mes données :
{"e":"session_started","ts":1,"sid":"aaaa0001","app":"0.6.0","platform":"Android","lang":"fr"}
{"e":"level_started","ts":2,"sid":"aaaa0001","level_id":"BG-001","attempt":1,"resumed":false}
{"e":"tutorial_hint_shown","ts":3,"sid":"aaaa0001","level_id":"BG-001","hint":"tut_tap","turn_index":0}
{"e":"can_selected","ts":4,"sid":"aaaa0001","level_id":"BG-001","lane":0,"color":"coral","cap":3,"turn_index":1,"ms":21000}
{"e":"can_selected","ts":5,"sid":"aaaa0001","level_id":"BG-001","lane":1,"color":"blue","cap":3,"turn_index":2,"ms":25000}
{"e":"level_completed","ts":6,"sid":"aaaa0001","level_id":"BG-001","steps":2,"duration_ms":30000,"undos":0,"restarts":0,"deadlocks":0}
{"e":"level_started","ts":7,"sid":"aaaa0001","level_id":"BG-002","attempt":2,"resumed":false}
{"e":"can_selected","ts":8,"sid":"aaaa0001","level_id":"BG-002","lane":2,"color":"blue","cap":1,"turn_index":1,"ms":3000}
{"e":"level_abandoned","ts":9,"sid":"aaaa0001","level_id":"BG-002","steps":1,"duration_ms":9000,"last_state_hash":"00ff","reason":"menu"}
merci !'''
TESTER_B = '''{"e":"level_started","ts":1,"sid":"bbbb0002","level_id":"BG-001","attempt":1,"resumed":false}
{"e":"can_selected","ts":2,"sid":"bbbb0002","level_id":"BG-001","lane":0,"color":"coral","cap":3,"turn_index":1,"ms":4000}
{"e":"undo_used","ts":3,"sid":"bbbb0002","level_id":"BG-001","turn_index":0}
{"e":"can_selected","ts":4,"sid":"bbbb0002","level_id":"BG-001","lane":0,"color":"coral","cap":3,"turn_index":1,"ms":6000}
{"e":"can_selected","ts":5,"sid":"bbbb0002","level_id":"BG-001","lane":1,"color":"blue","cap":3,"turn_index":2,"ms":8000}
{"e":"level_completed","ts":6,"sid":"bbbb0002","level_id":"BG-001","steps":2,"duration_ms":10000,"undos":1,"restarts":0,"deadlocks":0}
{"e":"settings_changed","ts":7,"sid":"bbbb0002","setting_name":"speed","value":"fast"}'''


class PlaytestReportTest(unittest.TestCase):
    def test_empty_folder_reports_nothing(self):
        with tempfile.TemporaryDirectory() as d:
            text = report.build_report(d)
            self.assertIn('Rien à rapporter', text)
            self.assertNotIn('| BG-', text)

    def test_counts_and_signals(self):
        with tempfile.TemporaryDirectory() as d:
            Path(d, 'testeur_A.txt').write_text(TESTER_A, encoding='utf-8')
            Path(d, 'testeur_B.txt').write_text(TESTER_B, encoding='utf-8')
            text = report.build_report(d)
            self.assertIn('**2 testeur(s)**', text)
            self.assertIn('| BG-001 | 2 | 2 | 2 | 2 | 0 | 0 | 1 | 0 | 20 s | 2 | 0 |', text)
            self.assertIn('| BG-002 | 1 | 0 | 1 | 0 | 1 | 0 | 0 | 0 | - | - | - |', text)
            self.assertIn('| testeur_A | BG-001 | 1 | 1 | 21.0 s |', text)
            self.assertIn('BG-002 : 1 testeur(s) l\'ont commencé sans le gagner (testeur_A)', text)
            self.assertIn('plus de 15 s pour : testeur_A', text)
            self.assertIn('speed=fast (1)', text)


if __name__ == '__main__':
    unittest.main()
