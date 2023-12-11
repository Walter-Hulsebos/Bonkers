using System;
using System.Collections.Generic;

using TMPro;

using Unity.Netcode;

using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterDatabase characterDatabase;

    [SerializeField] private Transform             charactersHolder;
    [SerializeField] private CharacterSelectButton selectButtonPrefab;
    [SerializeField] private PlayerCard[]          playerCards;
    [SerializeField] private GameObject            characterInfoPanel;
    [SerializeField] private TMP_Text              characterNameText;
    [SerializeField] private TMP_Text              joinCodeText;
    [SerializeField] private Button                lockInButton;

    // THIS code is obsolete but im keeping it just in case (BOBI)
    // [SerializeField] private Transform             introSpawnPoint;
    // private GameObject                        introInstance;

    private List<CharacterSelectButton>       characterButtons = new ();
    private NetworkList<CharacterSelectState> players;

    private void Awake() { players = new NetworkList<CharacterSelectState>(); }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            Character[] allCharacters = characterDatabase.GetAllCharacters();

            foreach (Character character in allCharacters)
            {
                CharacterSelectButton selectbuttonInstance = Instantiate(selectButtonPrefab, charactersHolder);
                selectbuttonInstance.SetCharacter(this, character);
                characterButtons.Add(selectbuttonInstance);
            }

            players.OnListChanged += HandlePlayersStateChanged;
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
        if (IsClient) { players.OnListChanged -= HandlePlayersStateChanged; }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback  -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }

    private void HandleClientConnected(UInt64 clientId) { players.Add(new CharacterSelectState(clientId)); }

    private void HandleClientDisconnected(UInt64 clientId)
    {
        for (Int32 i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != clientId) { continue; }

            players.RemoveAt(i);
            break;
        }
    }

    public void Select(Character character)
    {
        for (Int32 i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != NetworkManager.Singleton.LocalClientId) { continue; }

            if (players[i].IsLockedIn) { return; }

            if (players[i].CharacterId == character.Id) { return; }

            if (IsCharacterTaken(character.Id, false)) { return; }
        }

        characterNameText.text = character.DisplayName;

        characterInfoPanel.SetActive(true);

        // THIS code is obsolete but im keeping it just in case (BOBI)

        //if (introInstance != null) { Destroy(introInstance); }

        //introInstance = Instantiate(character.IntroPrefab, introSpawnPoint);

        SelectServerRpc(character.Id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectServerRpc(Int32 characterId, ServerRpcParams serverRpcParams = default)
    {
        for (Int32 i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!characterDatabase.IsValidCharacterId(characterId)) { return; }

            if (IsCharacterTaken(characterId, true)) { return; }

            players[i] = new CharacterSelectState(players[i].ClientId, characterId, players[i].IsLockedIn);
        }
    }

    public void LockIn() { LockInServerRpc(); }

    [ServerRpc(RequireOwnership = false)]
    private void LockInServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (Int32 i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!characterDatabase.IsValidCharacterId(players[i].CharacterId)) { return; }

            if (IsCharacterTaken(players[i].CharacterId, true)) { return; }

            players[i] = new CharacterSelectState(players[i].ClientId, players[i].CharacterId, true);
        }

        foreach (CharacterSelectState player in players)
        {
            if (!player.IsLockedIn) { return; }
        }

        foreach (CharacterSelectState player in players) { MatchplayNetworkServer.Instance.SetCharacter(player.ClientId, player.CharacterId); }

        MatchplayNetworkServer.Instance.StartGame();
    }

    private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        for (Int32 i = 0; i < playerCards.Length; i++)
        {
            if (players.Count > i) { playerCards[i].UpdateDisplay(players[i]); }
            else { playerCards[i].DisableDisplay(); }
        }

        foreach (CharacterSelectButton button in characterButtons)
        {
            if (button.IsDisabled) { continue; }

            if (IsCharacterTaken(button.Character.Id, false)) { button.SetDisabled(); }
        }

        foreach (CharacterSelectState player in players)
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

    private Boolean IsCharacterTaken(Int32 characterId, Boolean checkAll)
    {
        for (Int32 i = 0; i < players.Count; i++)
        {
            if (!checkAll)
            {
                if (players[i].ClientId == NetworkManager.Singleton.LocalClientId) { continue; }
            }

            if (players[i].IsLockedIn && players[i].CharacterId == characterId) { return true; }
        }

        return false;
    }
}