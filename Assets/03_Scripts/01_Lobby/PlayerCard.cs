using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bonkers.Lobby
{
    public class PlayerCard : MonoBehaviour
    {
        [SerializeField] private CharacterDatabase characterDatabase;

        [SerializeField] private GameObject visuals;

        [SerializeField] private Image characterIconImage;

        [SerializeField] private TextMesh playerNameText;

        [SerializeField] private TextMesh characterNameText;

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
