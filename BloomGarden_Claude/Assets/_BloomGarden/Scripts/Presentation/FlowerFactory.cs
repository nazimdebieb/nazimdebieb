using BloomGarden.Core.Model;
using UnityEngine;

namespace BloomGarden.Presentation
{
    /// <summary>
    /// Soft-primitive flowers. A bud (closed, upright oval) and a bloom (flat ring of petals) differ by shape,
    /// not only by hue (GDD chapter 03). Each color also carries its own symbol for color-blind players.
    /// </summary>
    public static class FlowerFactory
    {
        public static GameObject Bud(ColorId color, GardenPalette palette, Transform parent)
        {
            var root = new GameObject("Bud " + color.ToId());
            root.transform.SetParent(parent, false);
            GardenView.Prim(PrimitiveType.Cylinder, root.transform, palette.leaf, new Vector3(0f, 0.2f, 0f), new Vector3(0.08f, 0.2f, 0.08f));
            GardenView.Prim(PrimitiveType.Sphere, root.transform, palette.leaf, new Vector3(-0.16f, 0.12f, 0f), new Vector3(0.28f, 0.06f, 0.14f))
                .transform.localRotation = Quaternion.Euler(0f, 0f, 20f);
            GardenView.Prim(PrimitiveType.Sphere, root.transform, palette.Flower(color), new Vector3(0f, 0.62f, 0f), new Vector3(0.46f, 0.6f, 0.46f));
            AddSymbol(color, palette, root.transform, new Vector3(0f, 0.62f, -0.24f), 0.16f);
            return root;
        }

        public static GameObject Bloom(ColorId color, GardenPalette palette, Transform parent)
        {
            var root = new GameObject("Bloom " + color.ToId());
            root.transform.SetParent(parent, false);
            GardenView.Prim(PrimitiveType.Cylinder, root.transform, palette.leaf, new Vector3(0f, 0.12f, 0f), new Vector3(0.08f, 0.12f, 0.08f));
            const int petals = 6;
            for (int i = 0; i < petals; i++)
            {
                float angle = i * 360f / petals;
                var petal = GardenView.Prim(PrimitiveType.Sphere, root.transform, palette.Flower(color),
                    Quaternion.Euler(0f, angle, 0f) * new Vector3(0f, 0.3f, 0.26f), new Vector3(0.26f, 0.08f, 0.4f));
                petal.transform.localRotation = Quaternion.Euler(-12f, angle, 0f);
            }
            GardenView.Prim(PrimitiveType.Sphere, root.transform, palette.flowerHeart, new Vector3(0f, 0.34f, 0f), new Vector3(0.24f, 0.14f, 0.24f));
            return root;
        }

        public static GameObject Grass(GardenPalette palette, Transform parent)
        {
            var root = new GameObject("Grass");
            root.transform.SetParent(parent, false);
            for (int i = 0; i < 3; i++)
            {
                var blade = GardenView.Prim(PrimitiveType.Capsule, root.transform, palette.grass,
                    new Vector3(-0.15f + i * 0.15f, 0.12f, (i % 2) * 0.1f), new Vector3(0.07f, 0.13f, 0.07f));
                blade.transform.localRotation = Quaternion.Euler(0f, 0f, -15f + i * 15f);
            }
            return root;
        }

        /// <summary>Color symbol (GDD accessibility): coral = dot, blue = diamond, yellow = bar, purple = double dot.</summary>
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
