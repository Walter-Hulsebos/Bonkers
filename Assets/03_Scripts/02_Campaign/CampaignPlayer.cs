using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class CampaignPlayer : MonoBehaviour
    {
        [SerializeField] private int currentHealth;
        [SerializeField] private int maxHealth;
        // Start is called before the first frame update

        
        void Start()
        {
            CampaignManager.instance.player = gameObject.transform.parent.gameObject;
            currentHealth = maxHealth;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        public void SetCurrentHealth(int amount, Operator _operator)
        {
            print("Player got hit");
            switch (_operator)
            {
                case Operator.ADD:
                    currentHealth += amount;
                    break;
                case Operator.SUBSTRACT:
                    currentHealth -= amount;
                    break;
                default:
                    break;
            }
        }
    }
}
public enum Operator
{
    ADD,
    SUBSTRACT
}