using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class RespawnUIEffect : MonoBehaviour
{
    public GameObject player;
    public GameObject playerSpawnPoint;
    public GameObject[] objectsToTurnOnOne;
    public GameObject[] objectsToTurnOffOne;
    public GameObject[] objectsToTurnOnTwo;
    public GameObject[] objectsToTurnOffTwo;
    public GameObject inGameCanvas;
    public GameObject gameOverCanvas;

    private int spawnTimes = 3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && spawnTimes > 0)
        {
            Debug.Log("Player entered box");
            spawnTimes--;

            if (spawnTimes == 2)
            {
                foreach (GameObject obj in objectsToTurnOnOne)
                {
                    obj.SetActive(true);
                }

                foreach (GameObject obj in objectsToTurnOffOne)
                {
                    obj.SetActive(false);
                }
            }
            else if (spawnTimes == 1)
            {
                foreach (GameObject obj in objectsToTurnOnTwo)
                {
                    obj.SetActive(true);
                }

                foreach (GameObject obj in objectsToTurnOffTwo)
                {
                    obj.SetActive(false);
                }
            }
            else if (spawnTimes == 0)
            {
                inGameCanvas.SetActive(false);
                gameOverCanvas.SetActive(true);
            }

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