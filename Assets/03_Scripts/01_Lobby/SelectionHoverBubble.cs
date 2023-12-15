namespace Bonkers._01_Lobby
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using UnityEngine;
    
    using Bonkers.Shared;

    using UnityEngine.UI;

    using I32 = System.Int32;

    public sealed class SelectionHoverBubble : MonoBehaviour
    {
        [SerializeField, HideInInspector] private CharacterSelectDisplay characterSelectDisplay;
        
        [SerializeField] private CharacterSelectButton characterSelectButton;
        //[SerializeField] private Button button;
        [SerializeField] private Team   team;
        [SerializeField] private Image  p1,    p2,    p3;
        [SerializeField] private Image  size1, size2, size3;

        private void Reset()
        {
            SetParentStuff();
            
            //button = transform.parent.GetComponentInChildren<Button>();
            
            p1 = transform.Find(n: "Images/P1").GetComponent<Image>();
            p2 = transform.Find(n: "Images/P2").GetComponent<Image>();
            p3 = transform.Find(n: "Images/P3").GetComponent<Image>();
            
            size1 = transform.Find(n: "Size1").GetComponent<Image>();
            size2 = transform.Find(n: "Size2").GetComponent<Image>();
            size3 = transform.Find(n: "Size3").GetComponent<Image>();
        }

        [ContextMenu(itemName: "Set Parent Stuff")]        
        private void SetParentStuff()
        {
            characterSelectDisplay = transform.GetComponentInParent<CharacterSelectDisplay>();
            characterSelectButton  = transform.GetComponentInParent<CharacterSelectButton>();
        }

        private void Start()
        {
            //size1.color = size2.color = size3.color = team.Color;
            p1.color    = p2.color    = p3.color    = team.Color;
        }

        private void Update()
        {
            List<I32> __playersInBubble = PlayersInBubble;

            if (__playersInBubble == null || characterSelectDisplay == null)
            {
                Debug.LogError(message: "Required components are not set or available", context: this);
                return;
            }

            // Enable/disable size images based on player count
            size1.gameObject.SetActive(__playersInBubble.Count >= 1);
            size2.gameObject.SetActive(__playersInBubble.Count >= 2);
            size3.gameObject.SetActive(__playersInBubble.Count >= 3);

            // Enable/disable player images
            p1.gameObject.SetActive(__playersInBubble.Contains(item: 0));
            p2.gameObject.SetActive(__playersInBubble.Contains(item: 1));
            p3.gameObject.SetActive(__playersInBubble.Contains(item: 2));

            if (__playersInBubble.Count == 0)
            {
                Debug.Log(message: $"{__playersInBubble.Count} player(s) in bubble", context: this);
                return;
            }

            StringBuilder __playersInBubbleString = new ();
            foreach (I32 __playerInBubble in __playersInBubble)
            {
                __playersInBubbleString.Append(value: __playerInBubble).Append(value: ", ");
            }
            Debug.Log(message: $"{__playersInBubble.Count} player(s) in bubble: ({__playersInBubbleString})", context: this);

        }

        private List<I32> PlayersInBubble
        {
            get
            {
                //I32[] __playersIndices = { -1, -1, -1, };
                List<I32> __playersIndicesInBubble = new ();
                
                //I32 __playersInBubble = 0;
                for (I32 __index = 0; __index < characterSelectDisplay.Players.Count; __index += 1)
                {
                    CharacterSelectState __player = characterSelectDisplay.Players[index: __index];
                    
                    //Skip if player is locked in, not the character we're looking for, or not on the team we're looking for
                    if (__player.IsLockedIn)                                        continue;
                    if (__player.CharacterId != characterSelectButton.Character.Id) continue;
                    if (__player.Team        != team)                               continue;

                    I32 __playerIndexInTeam = __index / 2;
                    __playersIndicesInBubble.Add(item: __playerIndexInTeam);
                    
                    //__playersIndices[__index] = __playerIndexInTeam;
                    //__playersInBubble += 1;
                }

                //return __playersIndices;
                return __playersIndicesInBubble;
            }
        }
        
    }
}