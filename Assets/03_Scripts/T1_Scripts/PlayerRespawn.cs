using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerRespawn : MonoBehaviour
{
    public GameObject player;
    public GameObject playerSpawnPoint;
    public int maxRespawns;
    private int remainingRespawns;

    private void Start()
    {
        remainingRespawns = maxRespawns;
    }

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
            LoadSpectateScene();
            Debug.Log("No more respawns available for the player.");
        }
    }

    private void LoadSpectateScene()
    {
        SceneManager.LoadScene("SpectateScene");
    }


    // public void Spawn(PlayerStateMachine stateMachine)
    // {
    //    stateMachine.Motor.SetPosition(playerSpawnPoint.transform.position);
    //    Debug.Log("Teleport my g");
    // }


}

