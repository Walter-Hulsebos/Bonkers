using Bonkers.Shared;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private CharacterDatabase characterDatabase;
    
    #if ODIN_INSPECTOR
    [InlineEditor]
    #endif
    [SerializeField] private Team              team;
    [SerializeField] private GameObject        visuals;
    [SerializeField] private Image             characterIconImage;
    [SerializeField] private TMP_Text          playerNameText;
    [SerializeField] private TMP_Text          characterNameText;

    public void UpdateDisplay(CharacterSelectState state)
    {
        if (state.CharacterId != -1)
        {
            Character character = characterDatabase.GetCharacterById(state.CharacterId);
            characterIconImage.sprite  = character.Icon;
            characterIconImage.enabled = true;
            characterNameText.text     = character.DisplayName;
        }
        else { characterIconImage.enabled = false; }

        playerNameText.text = state.IsLockedIn ? $"Player {state.ClientId}" : $"Player {state.ClientId} (Picking...)";

        visuals.SetActive(true);
    }

    public void DisableDisplay() { visuals.SetActive(false); }
}