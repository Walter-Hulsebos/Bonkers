using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class MapSelectManager : MonoBehaviour
    {
        [SerializeField] private int[] map;
        [SerializeField] private GameObject mapSelectCanvas;

        public void ActiveMapSelect()
        {
            mapSelectCanvas.SetActive(true);
        }

        public void IncreaseVotes(int mapN)
        {
            map[mapN]++; 
        }

        private int SelectedMap()
        {
            return Math.Max(map[0], Math.Max(map[1], Math.Max(map[2], map[3])));
        }
        public void StartMapSelected()
        {
            switch (SelectedMap())
            {
                case 0:
                    Debug.Log("random map");
                    break;
                case 1:
                    Debug.Log("mansion");
                    break;
                case 2:
                    Debug.Log("steelworks");
                    break;
                case 3:
                    Debug.Log("waterpark");
                    break;
                case 4:
                    Debug.Log("station");
                    break;
                default:
                    Debug.Log("random map");
                    break;
            }          
        }

        public void StartGameMap() 
        { 
            MatchplayNetworkServer.Instance.StartGame();
        }
    }
}
