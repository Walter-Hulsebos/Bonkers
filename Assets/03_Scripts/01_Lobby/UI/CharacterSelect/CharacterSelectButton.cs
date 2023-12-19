using Bonkers.Shared;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;

using UnityEngine;
using UnityEngine.UI;

using Bonkers;

using UnityEngine.EventSystems;

using U64 = System.UInt64;

public class CharacterSelectButton : NetworkBehaviour, ISelectHandler
{
    [SerializeField] private Sprite borderA;
    [SerializeField] private Sprite borderB;
    [SerializeField] private Sprite borderAB;
    [SerializeField] private Sprite borderEmpty;
    [SerializeField] private Image  borderImage;

    [SerializeField] private Image iconImage;

    //[SerializeField] private GameObject disabledOverlay;
    [SerializeField] private ButtonPlus        button;
    [SerializeField] private Team          teamA;
    [SerializeField] private Team          teamB;
    [SerializeField] private HashSet<Team> selectorTeams = new();

    [SerializeField] private Character              character;
    [SerializeField] private CharacterSelectDisplay characterSelect;

    public Character Character  { get; private set; }
    public Boolean   IsDisabled { get; private set; }

    public void Reset()
    {
        button    = transform.GetComponentInChildren<ButtonPlus>();
        iconImage = button.GetComponent<Image>();
    }

    private void Awake() { SetCharacter(); }

    public void SetCharacter()
    {
        iconImage.sprite = character.Icon;
        Character        = character;
    }
    
    [Button]
    public void OnCharacterSelect()
    {
        //U64 __localClientId = NetworkManager.Singleton.LocalClientId;

        if (!characterSelect.Players.TryGetLocal(out CharacterSelectState __local))
        {
            Debug.LogWarning("Could not find local player in players list", context: this);
            return;
        }
        
        Team __team = __local.Team;
        
        if (selectorTeams.Contains(__team)) { return; }

        selectorTeams.Add(__team);
        characterSelect.LockIn();
        ChangeBorder();
    }

    public void OnCharacterDeselect(Team team)
    {
        selectorTeams.Remove(team);
        //TODO: add character deselect
        ChangeBorder();
    }
    
    private void ChangeBorder()
    {
        Boolean __hasTeamA = selectorTeams.Contains(teamA);
        Boolean __hasTeamB = selectorTeams.Contains(teamB);

        if (__hasTeamA && __hasTeamB) { borderImage.sprite      = borderAB; }
        else if (__hasTeamA) { borderImage.sprite               = borderA; }
        else if (__hasTeamB) { borderImage.sprite               = borderB; }
        else if (selectorTeams.Count == 0) { borderImage.sprite = borderEmpty; }
    }

    public void SetDisabled()
    {
        IsDisabled = true;
        //disabledOverlay.SetActive(true);
        button.interactable = false;
    }

    #region ISelectHandler implementation

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("OnSelect called.", context: this);
        
        OnCharacterHover();
    }
    
    // public void OnDeselect(BaseEventData eventData)
    // {
    //     //HoverOffCharacter();
    // }

    #endregion
    
    #if ODIN_INSPECTOR
    [Button]
    #endif
    public void OnCharacterHover()
    {
        Debug.Log("HoverOnCharacter called.", context: this);
        characterSelect.HoverOn(character: character);
    }
    
    public void OnCharacterHoverOff()
    {
        Debug.Log("HoverOffCharacter called.", context: this);
        
    }

}