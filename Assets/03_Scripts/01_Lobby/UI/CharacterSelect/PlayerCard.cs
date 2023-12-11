using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private GameObject        visuals;
    [SerializeField] private Image             characterIconImage;
    [SerializeField] private TMP_Text          playerNameText;
    [SerializeField] private TMP_Text          characterNameText;
    [SerializeField] private Transform         characterBody;
    [SerializeField] private GameObject        emptyPlayerIcon;
    private GameObject characterInstance;

    public void UpdateDisplay(CharacterSelectState state)
    {
        if (state.CharacterId != -1)
        {
            Character character = characterDatabase.GetCharacterById(state.CharacterId);
            characterIconImage.sprite   = character.Icon;
            characterIconImage.enabled  = true;
            characterNameText.text      = character.DisplayName;

            emptyPlayerIcon.SetActive(false);

            if (characterInstance != null) { Destroy(characterInstance); }
            characterInstance = Instantiate(character.WholeBody, characterBody);
            
        }
        else { characterIconImage.enabled = false; }

        playerNameText.text = state.IsLockedIn ? $"Player {state.ClientId}" : $"Player {state.ClientId} (Picking...)";

        visuals.SetActive(true);
    }

    public void DisableDisplay() { visuals.SetActive(false); }
}