using System;

using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Image      iconImage;
    [SerializeField] private GameObject disabledOverlay;
    [SerializeField] private Button     button;

    [SerializeField] private Character character;
    [SerializeField] private CharacterSelectDisplay characterSelect;
    
    public Character Character  { get; private set; }
    public Boolean   IsDisabled { get; private set; }

    private void Awake()
    {
        //characterSelect = GetComponent<CharacterSelectDisplay>();
        SetCharacter();
    }
    public void SetCharacter()
    {
        iconImage.sprite = character.Icon;
        //changes here//
        Character = character;
    }

    public void SelectCharacter() { characterSelect.Select(Character); }

    public void SetDisabled()
    {
        IsDisabled = true;
        disabledOverlay.SetActive(true);
        button.interactable = false;
    }
}