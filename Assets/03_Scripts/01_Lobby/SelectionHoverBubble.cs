namespace Bonkers._01_Lobby
{
    using System;

    using UnityEngine;
    
    using Bonkers.Shared;

    using UnityEngine.UI;

    using I32 = System.Int32;

    public sealed class SelectionHoverBubble : MonoBehaviour
    {
        [SerializeField, HideInInspector] private CharacterSelectDisplay characterSelectDisplay;
        
        [SerializeField] private CharacterSelectButton characterSelectButton;
        [SerializeField] private Button button;
        //[SerializeField] private I32    characterId;
        [SerializeField] private Team   team;
        //[SerializeField] private Sprite p1,    p2,    p3;
        [SerializeField] private Image  p1,    p2,    p3;
        [SerializeField] private Image  size1, size2, size3;

        private void Reset()
        {
            characterSelectDisplay = transform.GetComponentInParent<CharacterSelectDisplay>();
            characterSelectButton  = transform.GetComponentInParent<CharacterSelectButton>();
            
            p1   = transform.Find("Images/P1").GetComponent<Image>();
            p2   = transform.Find("Images/P2").GetComponent<Image>();
            p3   = transform.Find("Images/P3").GetComponent<Image>();
            
            size1 = transform.Find("Size1").GetComponent<Image>();
            size2 = transform.Find("Size2").GetComponent<Image>();
            size3 = transform.Find("Size3").GetComponent<Image>();
        }

        private void Start()
        {
            size1.color = size2.color = size3.color = team.Color;
        }

        private void Update()
        {
            Int32[] __playersInBubble = PlayersInBubble;
            
            if (__playersInBubble != null)
            {
                I32 __playersInBubbleCount = 0;
                foreach (I32 __playerInBubble in __playersInBubble)
                {
                    if(__playerInBubble == -1) continue;
                    __playersInBubbleCount += 1;
                    switch (__playerInBubble)
                    {
                        case 1: p1.enabled = true;
                            break;
                        case 2: p2.enabled = true;
                            break;
                        case 3: p3.enabled = true;
                            break;
                    }
                }
                
                //Enable/disable size 1, 2, 3 based on how many players are in the bubble
                switch (__playersInBubbleCount)
                {
                    case 0:
                        size1.enabled = size2.enabled = size3.enabled = false;
                        break;
                    case 1:
                        size1.enabled = true;
                        size2.enabled = size3.enabled = false;
                        break;
                    case 2:
                        size1.enabled = size2.enabled = true;
                        size3.enabled = false;
                        break;
                    case 3:
                        size1.enabled = size2.enabled = size3.enabled = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            
        }

        private I32[] PlayersInBubble
        {
            get
            {
                I32[] __playersIndices = { -1, -1, -1, };

                //I32 __playersInBubble = 0;
                for (I32 __index = 0; __index < characterSelectDisplay.Players.Count; __index += 1)
                {
                    CharacterSelectState __player = characterSelectDisplay.Players[__index];
                    if (__player.CharacterId != characterSelectButton.Character.Id) continue;
                    if (__player.Team        != team)        continue;

                    I32 __playerIndex = __index / 2;
                    __playersIndices[__playerIndex] = __index;
                    //__playersInBubble += 1;
                }

                return __playersIndices;
            }
        }
        
    }
}