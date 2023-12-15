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
    [Header("References")]
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

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList) { HandleClientConnected(client.ClientId); }
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

        Players.Add(new CharacterSelectState(clientId, __teamId));
    }

    private void HandleClientDisconnected(UInt64 clientId)
    {
        for (Int32 i = 0; i < Players.Count; i++)
        {
            if (Players[i].ClientId != clientId) { continue; }

            Players.RemoveAt(i);
            break;
        }
    }

    public void Select(Character character)
    {
        for (Int32 i = 0; i < Players.Count; i++)
        {
            if (Players[i].ClientId != NetworkManager.Singleton.LocalClientId) { continue; }

            if (Players[i].IsLockedIn) { return; }

            if (Players[i].CharacterId == character.Id) { return; }

            if (IsCharacterTaken(character.Id, false)) { return; }
        }

        characterNameText.text = character.DisplayName;

        characterInfoPanel.SetActive(true);

        SelectServerRpc(character.Id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectServerRpc(Int32 characterId, ServerRpcParams serverRpcParams = default)
    {
        for (Int32 i = 0; i < Players.Count; i++)
        {
            if (Players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!characterDatabase.IsValidCharacterId(characterId)) { return; }

            if (IsCharacterTaken(characterId, true)) { return; }

            Players[i] = new CharacterSelectState(Players[i].ClientId, Players[i].TeamId, characterId, Players[i].IsLockedIn);
        }
    }

    public void LockIn() { LockInServerRpc(); }

    [ServerRpc(RequireOwnership = false)]
    private void LockInServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (Int32 i = 0; i < Players.Count; i++)
        {
            if (Players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!characterDatabase.IsValidCharacterId(Players[i].CharacterId)) { return; }

            if (IsCharacterTaken(Players[i].CharacterId, true)) { return; }

            Players[i] = new CharacterSelectState(Players[i].ClientId, Players[i].TeamId, Players[i].CharacterId, true);
        }

        foreach (CharacterSelectState player in Players)
        {
            if (!player.IsLockedIn) { return; }
        }

        foreach (CharacterSelectState player in Players) { MatchplayNetworkServer.Instance.SetCharacter(player.ClientId, player.CharacterId); }

        MatchplayNetworkServer.Instance.StartGame();
    }

    private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        for (Int32 i = 0; i < playerCards.Length; i++)
        {
            if (Players.Count > i) { playerCards[i].UpdateDisplay(Players[i]); }
            else { playerCards[i].DisableDisplay(); }
        }

        foreach (CharacterSelectButton button in characterButtons)
        {
            if (button.IsDisabled) { continue; }

            if (IsCharacterTaken(button.Character.Id, false)) { button.SetDisabled(); }
        }

        foreach (CharacterSelectState player in Players)
        {
            if (player.ClientId != NetworkManager.Singleton.LocalClientId) { continue; }

            if (player.IsLockedIn)
            {
                lockInButton.interactable = false;
                break;
            }

            if (IsCharacterTaken(player.CharacterId, false))
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
                if (Players[i].ClientId == NetworkManager.Singleton.LocalClientId) { continue; }
            }

            if (Players[i].IsLockedIn && Players[i].CharacterId == characterId) { return true; }
        }

        return false;
    }
}