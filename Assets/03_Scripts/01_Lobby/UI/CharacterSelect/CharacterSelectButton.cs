using Bonkers.Shared;

using Sirenix.OdinInspector;

using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;

using UnityEngine;
using UnityEngine.UI;

using Bonkers;

using U64 = System.UInt64;

public class CharacterSelectButton : NetworkBehaviour
{
    [SerializeField] private Sprite borderA;
    [SerializeField] private Sprite borderB;
    [SerializeField] private Sprite borderAB;
    [SerializeField] private Sprite borderEmpty;
    [SerializeField] private Image  borderImage;

    [SerializeField] private Image iconImage;

    //[SerializeField] private GameObject disabledOverlay;
    [SerializeField] private Button        button;
    [SerializeField] private Team          teamA;
    [SerializeField] private Team          teamB;
    [SerializeField] private HashSet<Team> selectorTeams = new();

    [SerializeField] private Character              character;
    [SerializeField] private CharacterSelectDisplay characterSelect;

    public Character Character  { get; private set; }
    public Boolean   IsDisabled { get; private set; }

    public void Reset()
    {
        button    = transform.GetComponentInChildren<Button>();
        iconImage = button.GetComponent<Image>();
    }

    private void Awake() { SetCharacter(); }

    public void SetCharacter()
    {
        iconImage.sprite = character.Icon;
        Character        = character;
    }
    
    [Button]
    public void SelectCharacter()
    {
        U64 __localClientId = NetworkManager.Singleton.LocalClientId;

        if (!characterSelect.Players.TryGetLocal(out CharacterSelectState __local)) return;
        
        Team __team = __local.Team;
        
        if (selectorTeams.Contains(__team)) { return; }

        selectorTeams.Add(__team);
        characterSelect.LockIn();
        ChangeBorder();
    }

    public void DeselectCharacter(Team team)
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
}