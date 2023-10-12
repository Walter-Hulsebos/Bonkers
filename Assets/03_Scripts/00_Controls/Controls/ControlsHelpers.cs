namespace Bonkers.Controls
{
    using System;

    using JetBrains.Annotations;

    using UnityEngine;
    using UnityEngine.InputSystem;

    [PublicAPI]
    public static class ControlsHelpers
    {
        public static Boolean GetPlayerInput<T>(this T context, out PlayerInput playerInput) where T : Component
        {
            if (context.TryGetComponent(out playerInput)) return true;
            
            playerInput = context.GetComponentInParent<PlayerInput>();

            if (playerInput != null) return true;

            Debug.Log(message: $"{nameof(PlayerInput)} component not found for {context.name}.", context: context);
            return false;
        }
    }
}