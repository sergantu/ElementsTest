using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeBase.Extension
{
    public static class TransformExtension
    {
        public static List<GameObject> GetChildren(this Transform objectTransform)
        {
            return (from Transform child in objectTransform select child.gameObject).ToList();
        }

        public static void RemoveAllChildren(this Transform objectTransform)
        {
            List<GameObject> children = (from Transform child in objectTransform select child.gameObject).ToList();

            foreach (GameObject child in children) {
                child.transform.SetParent(null);
                Object.Destroy(child);
            }
        }

        public static Transform FindChildRecursive(this Transform objectTransform, string name, bool includeNotActive = false)
        {
            Transform[] childComponents = objectTransform.gameObject.GetComponentsInChildren<Transform>(includeNotActive);
            return childComponents.FirstOrDefault(childComponent => childComponent.gameObject.name == name);
        }

        public static bool IsObjectInChildRecursive(this Transform objectTransform, Transform childToFind)
        {
            Transform[] childComponents = objectTransform.gameObject.GetComponentsInChildren<Transform>();
            return childComponents.Any(childComponent => childComponent.gameObject.transform == childToFind);
        }

        public static void SetMaxSibling(this Transform objectTransform)
        {
            if (objectTransform.parent == null) {
                return;
            }

            int maxIndex = int.MinValue;
            Transform parent = objectTransform.parent;

            for (int i = 0; i < parent.childCount; i++) {
                int index = parent.GetChild(i).GetSiblingIndex();

                if (index > maxIndex) {
                    maxIndex = index;
                }
            }

            objectTransform.SetSiblingIndex(maxIndex + 1);
        }
    }
}