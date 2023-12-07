using System;
using UnityEngine;

namespace Bonkers
{
    public static class TransformExtensions
    {
        public static Transform FindChildWithTag(this Transform transform, String tag)
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag(tag))
                {
                    return child;
                }

                if (child.FindChildWithTag(tag) != null)
                {
                    return child.FindChildWithTag(tag);
                }
            }

            return null;
        }
    }
}
