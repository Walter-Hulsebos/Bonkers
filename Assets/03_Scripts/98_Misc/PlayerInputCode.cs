namespace Bonkers
{
    using System;

    using UnityEngine;
    using UnityEngine.InputSystem;
    
    using JetBrains.Annotations;

    [PublicAPI]
    public static class PlayerInputExtensions
    {
        public static void MakeChildOf(this PlayerInput playerInput, Transform parent, Boolean resetTransform = true)
        {
            playerInput.transform.SetParent(parent);
            
            if (resetTransform)
            {
                Transform __transform = playerInput.transform;
                 
                __transform.localPosition = Vector3.zero;
                __transform.localScale    = Vector3.one;
                __transform.localRotation = Quaternion.identity;
            }
        }
    }
}
