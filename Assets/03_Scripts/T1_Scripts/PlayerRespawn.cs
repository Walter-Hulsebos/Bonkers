using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerRespawn : MonoBehaviour
{
    public GameObject player;
    public GameObject playerSpawnPoint;
    public int maxRespawns;
    private int remainingRespawns;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered box");

            if (other.TryGetComponent(out PlayerStateMachine __stateMachine))
            {
                AttemptRespawn(__stateMachine);
            }
        }

    }
    public void AttemptRespawn(PlayerStateMachine stateMachine)
    {
        if (remainingRespawns > 0)
        {
            remainingRespawns--;
            stateMachine.Motor.SetPosition(playerSpawnPoint.transform.position);
            Debug.Log("Teleport my g. Remaining Respawns: " + remainingRespawns);
        }
        else
        {
            Debug.Log("No more respawns available for the player.");
        }
    }

    // public void Spawn(PlayerStateMachine stateMachine)
    // {
    //    stateMachine.Motor.SetPosition(playerSpawnPoint.transform.position);
    //    Debug.Log("Teleport my g");
    // }


}

