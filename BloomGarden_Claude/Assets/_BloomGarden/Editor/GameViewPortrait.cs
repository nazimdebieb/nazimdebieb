using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BloomGarden.EditorTools
{
    /// <summary>
    /// Sets the Game view to a fixed 1080x1920 portrait resolution (reference phone screen of the GDD) so play
    /// sessions and captures match the target format. Unity exposes no public API for this: the internal
    /// GameViewSizes types are reached by reflection, and a failure only logs a warning.
    /// </summary>
    public static class GameViewPortrait
    {
        private const int Width = 1080, Height = 1920;
        private const string Label = "Bloom Garden portrait";

        [MenuItem("Bloom Garden/Vue Game portrait 1080x1920")]
        public static bool Apply()
        {
            try
            {
                Assembly editor = typeof(Editor).Assembly;
                Type sizesType = editor.GetType("UnityEditor.GameViewSizes");
                Type sizeType = editor.GetType("UnityEditor.GameViewSize");
                Type sizeKind = editor.GetType("UnityEditor.GameViewSizeType");
                Type gameViewType = editor.GetType("UnityEditor.GameView");

                Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
                object sizes = singleton.GetProperty("instance").GetValue(null);
                object groupType = sizesType.GetProperty("currentGroupType").GetValue(sizes);
                object group = sizesType.GetMethod("GetGroup").Invoke(sizes, new[] { groupType });
                Type groupClass = group.GetType();

                int index = FindSize(group, groupClass);
                if (index < 0)
                {
                    object fixedKind = Enum.Parse(sizeKind, "FixedResolution");
                    ConstructorInfo ctor = sizeType.GetConstructor(new[] { sizeKind, typeof(int), typeof(int), typeof(string) });
                    object size = ctor.Invoke(new[] { fixedKind, Width, Height, Label });
                    groupClass.GetMethod("AddCustomSize").Invoke(group, new[] { size });
                    index = FindSize(group, groupClass);
                }

                EditorWindow window = EditorWindow.GetWindow(gameViewType);
                PropertyInfo selected = gameViewType.GetProperty("selectedSizeIndex",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                selected.SetValue(window, index);
                window.Repaint();
                Debug.Log($"[BloomGarden] Game view set to {Width}x{Height} (index {index})");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning("[BloomGarden] Could not set the Game view size: " + e.Message);
                return false;
            }
        }

        private static int FindSize(object group, Type groupClass)
        {
            int count = (int)groupClass.GetMethod("GetTotalCount").Invoke(group, null);
            MethodInfo get = groupClass.GetMethod("GetGameViewSize");
            for (int i = 0; i < count; i++)
            {
                object size = get.Invoke(group, new object[] { i });
                Type t = size.GetType();
                if ((int)t.GetProperty("width").GetValue(size) == Width && (int)t.GetProperty("height").GetValue(size) == Height)
                    return i;
            }
            return -1;
        }
    }
}
