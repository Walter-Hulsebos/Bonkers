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

    [SerializeField]
    float radius;

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = new Color(r: 1, g: 0, b: 0, a: 0.2f);
        UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, radius);

        UnityEditor.Handles.color = Color.black;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius, 5);
#endif
    } 

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        foreach (KeyValuePair<String, UserData> client in MatchplayNetworkServer.Instance.ClientData)
        {
            Character character = characterDatabase.GetCharacterById(client.Value.characterId);


            if (character != null)
            {
                Vector2 random = Random.insideUnitCircle * radius;
                Vector3 spawnPos = transform.position + new Vector3 (random.x,0, random.y);
                NetworkObject characterInstance = Instantiate(character.GameplayPrefab, spawnPos, Quaternion.identity);
                characterInstance.SpawnAsPlayerObject(client.Value.clientId);

            }
        }
    }
}