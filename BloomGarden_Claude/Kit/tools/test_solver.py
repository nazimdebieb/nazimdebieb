"""Unit tests for the deterministic Python reference solver (stdlib)."""
import json
import unittest
from pathlib import Path
import solver_reference as s

ROOT=Path(__file__).resolve().parent.parent

class TestReferenceSolver(unittest.TestCase):
    def test_all_20_sample_levels_are_solvable(self):
        files=sorted((ROOT/'levels').glob('BG-*.json'))
        self.assertEqual(len(files),20)
        for f in files:
            lv=json.loads(f.read_text(encoding='utf-8'))
            result=s.solve(lv)
            self.assertEqual(result['status'],'solvable',f.name)
            self.assertEqual(result['solutionLanes'],lv['referenceSolution'])
            state=s.initial(lv); by_id=s.can_mapping(lv)
            for lane in result['solutionLanes']:
                state=s.move(state,lane,lv,by_id)
                self.assertIsNotNone(state)
            self.assertTrue(s.finished(state,lv))

    def test_invalid_color_rejected(self):
        lv=json.loads((ROOT/'levels/BG-001.json').read_text(encoding='utf-8'))
        lv['grid']['cells'][0]=['neon']
        with self.assertRaises(s.LevelError):s.validate(lv)

    def test_invalid_resources_rejected(self):
        lv=json.loads((ROOT/'levels/BG-001.json').read_text(encoding='utf-8'))
        lv['lanes'][0][0]['capacity']+=1
        with self.assertRaises(s.LevelError):s.validate(lv)

    def test_retry_is_deterministic(self):
        lv=json.loads((ROOT/'levels/BG-015.json').read_text(encoding='utf-8'))
        self.assertEqual(s.solve(lv),s.solve(lv))

if __name__=='__main__':unittest.main()
