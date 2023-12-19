using System;
using System.Collections.Generic;

using Bonkers.Shared;

using JetBrains.Annotations;

using TMPro;

using Unity.Netcode;

using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectDisplay : NetworkBehaviour
{
    [Header(header: "References")]
    [SerializeField] private CharacterDatabase characterDatabase;

    //[SerializeField] private Transform             charactersHolder;
    //[SerializeField] private CharacterSelectButton selectButtonPrefab;
    [SerializeField] private Team[]       allTeams;
    [SerializeField] private PlayerCard[] playerCards;
    [SerializeField] private GameObject   characterInfoPanel;
    [SerializeField] private TMP_Text     characterNameText;
    [SerializeField] private TMP_Text     joinCodeText;
    [SerializeField] private Button       lockInButton;

    // THIS code is obsolete but im keeping it just in case (BOBI)
    // [SerializeField] private Transform             introSpawnPoint;
    // private GameObject                        introInstance;

    private List<CharacterSelectButton>       characterButtons = new ();
    public  NetworkList<CharacterSelectState> Players { get; private set; }

    private void Awake() { Players = new NetworkList<CharacterSelectState>(); }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            Character[] allCharacters = characterDatabase.GetAllCharacters();

            //foreach (Character character in allCharacters)
            //{
            //    CharacterSelectButton selectbuttonInstance = Instantiate(selectButtonPrefab, charactersHolder);
            //    selectbuttonInstance.SetCharacter(this, character);
            //    characterButtons.Add(selectbuttonInstance);
            //}

            Players.OnListChanged += HandlePlayersStateChanged;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback  += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList) { HandleClientConnected(clientId: client.ClientId); }
        }

        if (IsHost) { joinCodeText.text = HostSingleton.Instance.HostRelayData.JoinCode; }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient) { Players.OnListChanged -= HandlePlayersStateChanged; }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback  -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }

    private void HandleClientConnected(UInt64 clientId)
    {
        Int32 __teamIndex = (Players.Count + 1) / 2;
        Int32 __teamId    = allTeams[__teamIndex].GetInstanceID();

        Players.Add(item: new CharacterSelectState(clientId: clientId, teamId: __teamId));
    }

    private void HandleClientDisconnected(UInt64 clientId)
    {
        for (Int32 i = 0; i < Players.Count; i++)
        {
            if (Players[index: i].ClientId != clientId) { continue; }

            Players.RemoveAt(index: i);
            break;
        }
    }

    [PublicAPI]
    public void HoverOn(Character character)
    {
        for (Int32 i = 0; i < Players.Count; i++)
        {
            //Only the case for the local player
            if (Players[index: i].ClientId != NetworkManager.Singleton.LocalClientId) { continue; }

            if (Players[index: i].IsLockedIn) { return; }

            if (Players[index: i].CharacterId == character.Id) { return; }

            if (IsCharacterTaken(characterId: character.Id, checkAll: false)) { return; }
        }

        characterNameText.text = character.DisplayName;

        characterInfoPanel.SetActive(value: true);

        HoverOnServerRpc(characterId: character.Id);
    }
    
    // [PublicAPI]
    // public void HoverOff()
    // {
    //     characterInfoPanel.SetActive(value: false);
    // }

    [ServerRpc(RequireOwnership = false)]
    private void HoverOnServerRpc(Int32 characterId, ServerRpcParams serverRpcParams = default)
    {
        for (Int32 i = 0; i < Players.Count; i++)
        {
            if (Players[index: i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!characterDatabase.IsValidCharacterId(id: characterId)) { return; }

            if (IsCharacterTaken(characterId: characterId, checkAll: true)) { return; }

            Players[index: i] = new CharacterSelectState(clientId: Players[index: i].ClientId, teamId: Players[index: i].TeamId, characterId: characterId, isLockedIn: Players[index: i].IsLockedIn);
        }
    }

    public void LockIn() { LockInServerRpc(); }

    [ServerRpc(RequireOwnership = false)]
    private void LockInServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (Int32 i = 0; i < Players.Count; i++)
        {
            if (Players[index: i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!characterDatabase.IsValidCharacterId(id: Players[index: i].CharacterId)) { return; }

            if (IsCharacterTaken(characterId: Players[index: i].CharacterId, checkAll: true)) { return; }

            Players[index: i] = new CharacterSelectState(clientId: Players[index: i].ClientId, teamId: Players[index: i].TeamId, characterId: Players[index: i].CharacterId, isLockedIn: true);
        }

        foreach (CharacterSelectState player in Players)
        {
            if (!player.IsLockedIn) { return; }
        }

        foreach (CharacterSelectState player in Players) { MatchplayNetworkServer.Instance.SetCharacter(clientId: player.ClientId, characterId: player.CharacterId); }

        MatchplayNetworkServer.Instance.StartGame();
    }

    private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        for (Int32 i = 0; i < playerCards.Length; i++)
        {
            if (Players.Count > i) { playerCards[i].UpdateDisplay(state: Players[index: i]); }
            else { playerCards[i].DisableDisplay(); }
        }

        foreach (CharacterSelectButton button in characterButtons)
        {
            if (button.IsDisabled) { continue; }

            if (IsCharacterTaken(characterId: button.Character.Id, checkAll: false)) { button.SetDisabled(); }
        }

        foreach (CharacterSelectState player in Players)
        {
            if (player.ClientId != NetworkManager.Singleton.LocalClientId) { continue; }

            if (player.IsLockedIn)
            {
                lockInButton.interactable = false;
                break;
            }

            if (IsCharacterTaken(characterId: player.CharacterId, checkAll: false))
            {
                lockInButton.interactable = false;
                break;
            }

            lockInButton.interactable = true;

            break;
        }
    }

    [PublicAPI]
    public Boolean IsCharacterTaken(Int32 characterId, Boolean checkAll)
    {
        for (Int32 i = 0; i < Players.Count; i++)
        {
            if (!checkAll)
            {
                if (Players[index: i].ClientId == NetworkManager.Singleton.LocalClientId) { continue; }
            }

            if (Players[index: i].IsLockedIn && Players[index: i].CharacterId == characterId) { return true; }
        }

        return false;
    }
}