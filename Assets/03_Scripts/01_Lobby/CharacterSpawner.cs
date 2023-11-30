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
                Vector3       spawnPos          = new (Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
                NetworkObject characterInstance = Instantiate(character.GameplayPrefab, spawnPos, Quaternion.identity);
                characterInstance.SpawnAsPlayerObject(client.Value.clientId);
            }
        }
    }
}