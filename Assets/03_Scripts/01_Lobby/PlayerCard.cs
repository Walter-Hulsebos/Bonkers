using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace Bonkers.Lobby
{
    public class PlayerCard : MonoBehaviour
    {
        [SerializeField] private CharacterDatabase characterDatabase;

        [SerializeField] private GameObject visuals;

        [SerializeField] private Image characterIconImage;

        [SerializeField] private TMP_Text playerNameText;

        [SerializeField] private TMP_Text characterNameText;

        public void UpdateDisplay(CharacterSelectState state)
        {
            if(state.CharacterId != -1)
            {
                var character = characterDatabase.GetCharacterById(state.CharacterId);
                characterIconImage.sprite = character.Icon;
                characterIconImage.enabled = true;
                characterNameText.text = character.DisplayName;
            }
            else
            {
                characterIconImage.enabled = false;
            }

            playerNameText.text = $"Player{state.ClientId}";

            visuals.SetActive(true);
        }

        public void DisableDisplay()
        {
            visuals.SetActive(false);
        }
    }
}
