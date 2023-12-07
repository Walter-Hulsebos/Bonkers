using System;
using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine;

using Random = UnityEngine.Random;

public class CharacterSpawner : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterDatabase characterDatabase;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        foreach (KeyValuePair<String, UserData> client in MatchplayNetworkServer.Instance.ClientData)
        {
            Character character = characterDatabase.GetCharacterById(client.Value.characterId);

            if (character != null)
            {
                
                NetworkObject characterInstance = Instantiate(character.GameplayPrefab, Vector3.zero, Quaternion.identity);
                characterInstance.SpawnAsPlayerObject(client.Value.clientId);
            }
        }
    }
}