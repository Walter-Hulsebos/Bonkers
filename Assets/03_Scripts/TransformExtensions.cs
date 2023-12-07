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
                
                Transform __child = child.FindChildWithTag(tag);
                if (__child != null)
                {
                    return __child;
                }

            }

            return null;
        }
    }
}
