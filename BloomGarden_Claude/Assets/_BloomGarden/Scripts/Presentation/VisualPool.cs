using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloomGarden.Presentation
{
    /// <summary>
    /// Reuses garden visuals (buds, blooms, drops, sparkles) within a level, so blooming cascades do not
    /// create or destroy GameObjects every step (P4-08). Objects are keyed by kind; a released object is
    /// deactivated and parked under the pool root.
    /// </summary>
    public sealed class VisualPool
    {
        private readonly Transform _root;
        private readonly Dictionary<string, Stack<GameObject>> _free = new Dictionary<string, Stack<GameObject>>();
        private readonly Dictionary<GameObject, string> _kindOf = new Dictionary<GameObject, string>();

        public int Created { get; private set; }

        public VisualPool(Transform root)
        {
            _root = root;
        }

        public GameObject Get(string kind, Func<Transform, GameObject> factory, Transform parent)
        {
            GameObject go;
            if (_free.TryGetValue(kind, out var stack) && stack.Count > 0)
            {
                go = stack.Pop();
                go.transform.SetParent(parent, false);
            }
            else
            {
                go = factory(parent);
                _kindOf[go] = kind;
                Created++;
            }
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            go.SetActive(true);
            return go;
        }

        /// <summary>Returns a pooled object; objects not created by this pool are destroyed.</summary>
        public void Release(GameObject go)
        {
            if (go == null) return;
            if (!_kindOf.TryGetValue(go, out string kind))
            {
                UnityEngine.Object.Destroy(go);
                return;
            }
            go.SetActive(false);
            go.transform.SetParent(_root, false);
            if (!_free.TryGetValue(kind, out var stack)) _free[kind] = stack = new Stack<GameObject>();
            stack.Push(go);
        }

        public void ReleaseChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--) Release(parent.GetChild(i).gameObject);
        }
    }
}
