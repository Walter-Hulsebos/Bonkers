using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;



namespace Bonkers.Lobby
{
    public class CharacterSelectDisplay : NetworkBehaviour
    {
        [SerializeField] private CharacterDatabase characterDatabase;

        [SerializeField] private Transform charactersHolder;

        [SerializeField] private CharacterSelectButton selectButtonPrefab;

        [SerializeField] private PlayerCard[] playerCards;

        [SerializeField] private GameObject characterInfoPanel;

        [SerializeField] private TextMesh characterNameText;


        private NetworkList<CharacterSelectState> players;

        private void Awake()
        {
            players = new NetworkList<CharacterSelectState>();
        }
        public override void OnNetworkSpawn()
        {
            if(IsClient)
            {
                Character[] allCharacters = characterDatabase.GetAllCharacters();

                foreach(var character in allCharacters)
                {
                    var selectbuttonInstance = Instantiate(selectButtonPrefab, charactersHolder);
                    selectbuttonInstance.SetCharacter(this, character);
                }

                players.OnListChanged += HandlePlayersStateChanged;
            }

            if (IsServer)
            {
                //Make sure everyone who is connected after that poit to be added
                NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

                //Make sure everyone who is already connected is added(Fixing Host problems)
                foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    HandleClientConnected(client.ClientId);
                }
            }
            
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient)
            {
                players.OnListChanged -= HandlePlayersStateChanged;
            }

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
            }
        }

        private void HandleClientConnected( ulong cliendtId)
        {
            players.Add(new CharacterSelectState(cliendtId));
        }

        private void HandleClientDisconnected(ulong cliendtId)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].ClientId == cliendtId)
                {
                    players.RemoveAt(i);
                    break;
                }
            }
        }

        public void Select(Character character)
        {
            characterNameText.text = character.DisplayName;
            characterInfoPanel.SetActive(true);

            SelectServerRpc(character.Id);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SelectServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
        {
            for(int i = 0;i < players.Count;i++)
            {
                if (players[i].ClientId == serverRpcParams.Receive.SenderClientId)
                {
                    players[i] = new CharacterSelectState(
                        players[i].ClientId,
                         characterId
                        );
                }
            }
        }

        private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
        {
            for (int i = 0; i < playerCards.Length; i++)
            {
                if(players.Count > i)
                {
                    playerCards[i].UpdateDisplay(players[i]);
                }
                else
                {
                    playerCards[i].DisableDisplay();
                }
            }
        }
    }

    


    
}
