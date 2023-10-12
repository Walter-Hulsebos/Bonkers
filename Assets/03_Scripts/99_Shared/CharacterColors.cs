namespace Bonkers.Shared
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public static class CharacterColors
    {
        private static readonly Color[] player_colors = 
        {
            Color.red,
            Color.blue,
            Color.green,
            Color.yellow,
            Color.magenta,
            Color.cyan,
            Color.white,
            Color.black,
        };
        
        public static Color GetPlayerColor(PlayerInput playerInput) => player_colors[playerInput.playerIndex];
    }
}
