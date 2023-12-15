namespace Bonkers._01_Lobby
{
    using System;

    using UnityEngine;
    
    using Bonkers.Shared;
    
    using I32 = System.Int32;

    public sealed class SelectionHoverBubble : MonoBehaviour
    {
        [SerializeField, HideInInspector] private CharacterSelectDisplay characterSelectDisplay;
        
        [SerializeField] private I32    characterId;
        [SerializeField] private Team   team;
        [SerializeField] private Sprite p1, p2, p3;

        private void Reset()
        {
            characterSelectDisplay = transform.GetComponentInParent<CharacterSelectDisplay>();
        }

        private void Update()
        {
            Int32[] __playersInBubble = PlayersInBubble;
            
            if (__playersInBubble == null)
            {
                foreach (I32 __playerInBubble in __playersInBubble)
                {
                    if(__playerInBubble == -1) continue;
                }
            }
            
            
        }

        private I32[] PlayersInBubble
        {
            get
            {
                I32[] __playersIndices = { -1, -1, -1, };

                for (I32 __index = 0; __index < characterSelectDisplay.Players.Count; __index += 1)
                {
                    CharacterSelectState __player = characterSelectDisplay.Players[__index];
                    if (__player.CharacterId != characterId) continue;
                    if (__player.Team        != team)        continue;

                    I32 __playerIndex = __index / 2;
                    __playersIndices[__playerIndex] = __index;
                }

                return __playersIndices;
            }
        }
        
    }
}