using BloomGarden.Core.Model;
using UnityEngine;

namespace BloomGarden.Presentation
{
    /// <summary>
    /// Soft-primitive garden pieces. A bud (closed tulip) and a bloom (open cup of petals) differ by shape,
    /// not only by hue (GDD chapter 03). Each gameplay color also carries its own white symbol (D-111).
    /// Petals hang under "Petal pivot" objects so the bloom can unfold (see <see cref="SetBloomOpen"/>).
    /// </summary>
    public static class FlowerFactory
    {
        public const float ClosedPetalAngle = -78f;
        public const float OpenPetalAngle = -24f;
        private const int Petals = 6;

        public static GameObject Bud(ColorId color, GardenPalette palette, Transform parent)
        {
            var root = new GameObject("Bud " + color.ToId());
            root.transform.SetParent(parent, false);
            Stem(palette, root.transform, 0.42f);
            var head = new GameObject("Head").transform;
            head.SetParent(root.transform, false);
            head.localPosition = new Vector3(0f, 0.62f, 0f);
            GardenView.Prim(PrimitiveType.Sphere, head, palette.Flower(color), Vector3.zero, new Vector3(0.44f, 0.62f, 0.44f), true);
            for (int i = 0; i < 3; i++)
            {
                var sepal = GardenView.Prim(PrimitiveType.Sphere, head, palette.leaf,
                    Quaternion.Euler(0f, i * 120f, 0f) * new Vector3(0f, -0.2f, 0.13f), new Vector3(0.14f, 0.26f, 0.08f), true);
                sepal.transform.localRotation = Quaternion.Euler(-25f, i * 120f, 0f);
            }
            AddSymbol(color, palette, head, new Vector3(0f, 0.02f, -0.23f), 0.15f);
            return root;
        }

        public static GameObject Bloom(ColorId color, GardenPalette palette, Transform parent)
        {
            var root = new GameObject("Bloom " + color.ToId());
            root.transform.SetParent(parent, false);
            Stem(palette, root.transform, 0.3f);
            var cup = new GameObject("Cup").transform;
            cup.SetParent(root.transform, false);
            cup.localPosition = new Vector3(0f, 0.42f, 0f);
            for (int i = 0; i < Petals; i++)
            {
                var pivot = new GameObject("Petal pivot").transform;
                pivot.SetParent(cup, false);
                pivot.localRotation = Quaternion.Euler(OpenPetalAngle, i * 360f / Petals, 0f);
                GardenView.Prim(PrimitiveType.Sphere, pivot, palette.Flower(color), new Vector3(0f, 0f, 0.24f), new Vector3(0.28f, 0.07f, 0.44f), true);
            }
            GardenView.Prim(PrimitiveType.Sphere, cup, palette.flowerHeart, new Vector3(0f, 0.04f, 0f), new Vector3(0.22f, 0.14f, 0.22f), true);
            for (int i = 0; i < 5; i++)
            {
                GardenView.Prim(PrimitiveType.Sphere, cup, palette.flowerHeart,
                    Quaternion.Euler(0f, i * 72f, 0f) * new Vector3(0f, 0.13f, 0.07f), Vector3.one * 0.06f);
            }
            return root;
        }

        /// <summary>0 = closed tulip, 1 = fully open cup.</summary>
        public static void SetBloomOpen(GameObject bloom, float open)
        {
            Transform cup = bloom.transform.Find("Cup");
            if (cup == null) return;
            float angle = Mathf.LerpUnclamped(ClosedPetalAngle, OpenPetalAngle, open);
            int i = 0;
            foreach (Transform pivot in cup)
            {
                if (pivot.name != "Petal pivot") continue;
                pivot.localRotation = Quaternion.Euler(angle, i * 360f / Petals, 0f);
                i++;
            }
        }

        public static GameObject Grass(GardenPalette palette, Transform parent)
        {
            var root = new GameObject("Grass");
            root.transform.SetParent(parent, false);
            for (int i = 0; i < 4; i++)
            {
                var blade = GardenView.Prim(PrimitiveType.Capsule, root.transform, palette.grass,
                    new Vector3(-0.2f + i * 0.13f, 0.12f, (i % 2) * 0.12f - 0.05f), new Vector3(0.06f, 0.13f, 0.06f));
                blade.transform.localRotation = Quaternion.Euler(0f, 0f, -20f + i * 13f);
            }
            GardenView.Prim(PrimitiveType.Sphere, root.transform, palette.marker, new Vector3(0.22f, 0.06f, -0.18f), Vector3.one * 0.09f);
            return root;
        }

        public static GameObject Drop(GardenPalette palette, Transform parent)
        {
            var drop = GardenView.Prim(PrimitiveType.Sphere, parent, palette.water, Vector3.zero, new Vector3(0.1f, 0.13f, 0.1f));
            drop.name = "Drop";
            return drop;
        }

        public static GameObject Sparkle(Material material, Transform parent)
        {
            var s = GardenView.Prim(PrimitiveType.Sphere, parent, material, Vector3.zero, Vector3.one * 0.08f);
            s.name = "Sparkle";
            return s;
        }

        public static GameObject Tree(GardenPalette palette, Material crown, Transform parent, float size)
        {
            var root = new GameObject("Tree");
            root.transform.SetParent(parent, false);
            GardenView.Prim(PrimitiveType.Cylinder, root.transform, palette.wood, new Vector3(0f, 0.45f * size, 0f), new Vector3(0.22f, 0.45f, 0.22f) * size, true);
            GardenView.Prim(PrimitiveType.Sphere, root.transform, crown, new Vector3(0f, 1.25f * size, 0f), new Vector3(1.2f, 1f, 1.2f) * size, true);
            GardenView.Prim(PrimitiveType.Sphere, root.transform, crown, new Vector3(0.38f, 1.0f * size, 0.1f), new Vector3(0.7f, 0.6f, 0.7f) * size, true);
            GardenView.Prim(PrimitiveType.Sphere, root.transform, crown, new Vector3(-0.35f, 1.05f * size, -0.15f), new Vector3(0.65f, 0.55f, 0.65f) * size, true);
            return root;
        }

        public static GameObject Bush(GardenPalette palette, Transform parent)
        {
            var root = new GameObject("Bush");
            root.transform.SetParent(parent, false);
            GardenView.Prim(PrimitiveType.Sphere, root.transform, palette.leaf, new Vector3(0f, 0.18f, 0f), new Vector3(0.55f, 0.4f, 0.5f), true);
            GardenView.Prim(PrimitiveType.Sphere, root.transform, palette.grass, new Vector3(0.22f, 0.14f, -0.1f), new Vector3(0.35f, 0.3f, 0.35f), true);
            return root;
        }

        public static GameObject Stone(GardenPalette palette, Transform parent, float size)
        {
            var stone = GardenView.Prim(PrimitiveType.Sphere, parent, palette.stone, Vector3.zero, new Vector3(1f, 0.55f, 0.85f) * size, true);
            stone.name = "Stone";
            return stone;
        }

        private static void Stem(GardenPalette palette, Transform parent, float height)
        {
            GardenView.Prim(PrimitiveType.Cylinder, parent, palette.leaf, new Vector3(0f, height / 2f, 0f), new Vector3(0.07f, height / 2f, 0.07f), true);
            var leaf = GardenView.Prim(PrimitiveType.Sphere, parent, palette.leaf, new Vector3(-0.15f, 0.12f, 0.02f), new Vector3(0.28f, 0.05f, 0.13f), true);
            leaf.transform.localRotation = Quaternion.Euler(0f, 15f, 22f);
            var leaf2 = GardenView.Prim(PrimitiveType.Sphere, parent, palette.leaf, new Vector3(0.14f, 0.09f, 0.04f), new Vector3(0.22f, 0.05f, 0.11f), true);
            leaf2.transform.localRotation = Quaternion.Euler(0f, -20f, -18f);
        }

        /// <summary>Color symbol (accessibility, D-111): coral = dot, blue = diamond, yellow = bar, purple = double dot.</summary>
        public static void AddSymbol(ColorId color, GardenPalette palette, Transform parent, Vector3 position, float size)
        {
            switch (color)
            {
                case ColorId.Coral:
                    GardenView.Prim(PrimitiveType.Sphere, parent, palette.marker, position, Vector3.one * size);
                    break;
                case ColorId.Blue:
                    GardenView.Prim(PrimitiveType.Cube, parent, palette.marker, position, new Vector3(size, size, size * 0.5f))
                        .transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
                    break;
                case ColorId.Yellow:
                    GardenView.Prim(PrimitiveType.Cube, parent, palette.marker, position, new Vector3(size * 1.8f, size * 0.5f, size * 0.5f));
                    break;
                case ColorId.Purple:
                    GardenView.Prim(PrimitiveType.Sphere, parent, palette.marker, position + Vector3.up * size * 0.6f, Vector3.one * size * 0.8f);
                    GardenView.Prim(PrimitiveType.Sphere, parent, palette.marker, position - Vector3.up * size * 0.6f, Vector3.one * size * 0.8f);
                    break;
            }
        }
    }
}
