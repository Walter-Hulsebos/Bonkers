using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerRespawn : MonoBehaviour
{
    public GameObject player;
    public GameObject playerSpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered box");

            if (other.TryGetComponent(out PlayerStateMachine __stateMachine))
            {
                Spawn(__stateMachine);
            }
        }

    }

    public void Spawn(PlayerStateMachine stateMachine)
    {
        stateMachine.Motor.SetPosition(playerSpawnPoint.transform.position);
        Debug.Log("Teleport my g");
    }
    

}

