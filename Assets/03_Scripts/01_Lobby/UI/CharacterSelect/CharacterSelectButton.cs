using Bonkers.Shared;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Sprite     borderA;
    [SerializeField] private Sprite     borderB;
    [SerializeField] private Sprite     borderAB;
    [SerializeField] private Sprite     borderEmpty;
    [SerializeField] private Image      borderImage;
    [SerializeField] private Image      iconImage;
    //[SerializeField] private GameObject disabledOverlay;
    [SerializeField] private Button     button;
    [SerializeField] private Team       teamA;
    [SerializeField] private Team       teamB;
    [SerializeField] private HashSet<Team>     selectorTeams = new();

    [SerializeField] private Character character;
    [SerializeField] private CharacterSelectDisplay characterSelect;
    
    public Character Character  { get; private set; }
    public Boolean   IsDisabled { get; private set; }
    public void Reset()
    {
        button = transform.GetComponentInChildren<Button>();
        iconImage = button.GetComponent<Image>();
    }

    private void Awake()
    {
        SetCharacter();
    }
    public void SetCharacter()
    {
        iconImage.sprite = character.Icon;
        Character = character;
    }

    [Button]
    public void SelectCharacter(Team team) 
    {
        if (selectorTeams.Contains(team))
        {
            return;
        }
        selectorTeams.Add(team);
        characterSelect.Select(Character);
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
        if (selectorTeams.Contains(teamA) && !selectorTeams.Contains(teamB))
        {
            borderImage.sprite = borderA;
        }
        else if (selectorTeams.Contains(teamA) && selectorTeams.Contains(teamB))
        {
            borderImage.sprite = borderAB;
        }
        else if (!selectorTeams.Contains(teamA) && selectorTeams.Contains(teamB))
        {
            borderImage.sprite = borderB;
        }
        else if (selectorTeams.Count == 0) 
        {
            borderImage.sprite = borderEmpty;
        }
    }

    public void SetDisabled()
    {
        IsDisabled = true;
        //disabledOverlay.SetActive(true);
        button.interactable = false;
    }
}