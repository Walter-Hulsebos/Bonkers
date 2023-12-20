using Bonkers.Characters;
using Bonkers.Lobby;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Bonkers
{
    public class PlayerSpecialMoves : MonoBehaviour
    {

        private enum InSpecial
        {
            None,
            Special
        }
        private InSpecial inSpecial;

        Controls.Controls input;

        PlayerCharacter playerInput;

        public GameObject mine;
        public GameObject pigeons;
        public GameObject rats;

        public AnimatorController animatorController;

        public bool doable = true;

        bool recharging = true;

        bool inputSpecial1;
        bool inputSpecial2;
        bool inputSpecial3;
        bool inputRightshoulder;
        bool inputToggleKB;
        bool inputSpecialKeyboard;

        public GameObject player;

        // Start is called before the first frame update
        void Awake()
        {
           playerInput = GetComponent<PlayerCharacter>();

            inSpecial = InSpecial.None;

            input = new Controls.Controls();

            input.Gameplay.Special1.performed += ctx => inputSpecial1 = ctx.ReadValueAsButton();

            input.Gameplay.Special2.performed += ctx => inputSpecial2 = ctx.ReadValueAsButton();

            input.Gameplay.Special3.performed += ctx => inputSpecial3 = ctx.ReadValueAsButton();

            input.Gameplay.SpecialsGamepad.performed += ctx => inputRightshoulder = ctx.ReadValueAsButton();

            input.Gameplay.SpecialKeyboard.performed += ctx => inputSpecialKeyboard = ctx.ReadValueAsButton();

            input.Gameplay.ToggleSpecialKB.performed += ctx =>
            {
                if (inputToggleKB == true)
                    inputToggleKB = false;
                else
                    inputToggleKB = true;
            };

            input.Gameplay.Special1.canceled += ctx => inputSpecial1 = false;

            input.Gameplay.Special2.canceled += ctx => inputSpecial2 = false;

            input.Gameplay.Special3.canceled += ctx => inputSpecial3 = false;

            input.Gameplay.SpecialsGamepad.canceled += ctx => inputRightshoulder = false;

            input.Gameplay.SpecialKeyboard.canceled += ctx => inputSpecialKeyboard = false;
        }

        // Update is called once per frame
        void Update()
        {
            handleSpecial1();
            handleSpecial2();
            handleSpecial3();

            if (inputToggleKB)
            {
                Debug.Log("Toggled Special moves");
            }

            if (inputRightshoulder || inputSpecialKeyboard)
            {
                Debug.Log("input2 - RMB or R1 or right shoulder button pressed");
            }

            switch (inSpecial)
            {
            case InSpecial.None:               
                    break;
            case InSpecial.Special:
                    Debug.Log("DOABLE SHOULD BE FALSE");
                    if (recharging)
                    {
                        StartCoroutine(ChangetoNone());
                        recharging = false;
                    }
                    break;
            }

            if(inSpecial == InSpecial.Special)
            {
                // BUG if the player does a special while moving it wont trigger the animation and they will continue to move 
                animatorController.enabled = false;
                playerInput.enabled = false;
                doable = false;
            }
            else
                if(inSpecial == InSpecial.None)
            {
                animatorController.enabled = true;
                playerInput.enabled = true;
                doable = true;
            }
        }

        void handleSpecial1()
        {
            if (inputSpecial1)
            {
                Debug.Log("input1 - LMB (for special 1) or square or X button pressed");
            }

            if (inputSpecial1 && inputRightshoulder && doable == true || inputSpecial1 && inputToggleKB && doable == true)
            {
                inSpecial = InSpecial.Special;
                Debug.Log("Special1 - both input1 and input2 pressed");
                StartCoroutine(SpawnRats());
                animatorController.animSpecial1();
            }
        }

        void handleSpecial2()
        {
            if (inputSpecial2)
            {
                Debug.Log("input1 - RMB (for special2) or triangle or Y button pressed");
            }

            if(inputSpecial2 && inputRightshoulder && doable == true || inputSpecial2 && inputToggleKB && doable == true)
            {
                inSpecial = InSpecial.Special;
                Debug.Log("Special2 - both input1 and input2 pressed");
                Instantiate(pigeons, transform.position+(transform.up * 2), Quaternion.identity);
                animatorController.animSpecial2();
            }
        }

        void handleSpecial3()
        {
            if (inputSpecial3)
            {
                Debug.Log("input1 - LMB (for spceial3)  or cirlce or B button pressed");
            }                      

            if (inputSpecial3 && inputRightshoulder && doable == true || inputSpecialKeyboard && inputToggleKB && doable == true)
            {
                inSpecial = InSpecial.Special;
                Debug.Log("Special3 - both input1 and input2 pressed");
                Instantiate(mine, transform.position+(transform.forward*4), Quaternion.identity);
                animatorController.animSpecial3();
            }
        }

        IEnumerator SpawnRats()
        {
            Instantiate(rats, transform.position + (transform.forward * 2), Quaternion.identity);
            yield return new WaitForEndOfFrame();
            Instantiate(rats, transform.position + (transform.forward * 2) + (transform.right * 1), Quaternion.identity);
            yield return new WaitForEndOfFrame();
            Instantiate(rats, transform.position + (transform.forward * 2) + (transform.right * -2), Quaternion.identity);
            yield return new WaitForEndOfFrame();
            Instantiate(rats, transform.position + (transform.forward * 1) + (transform.right * 0.5f), Quaternion.identity);
            yield return new WaitForEndOfFrame();
            Instantiate(rats, transform.position + (transform.forward * 1) + (transform.right * -0.5f), Quaternion.identity);
            
        }

        IEnumerator ChangetoNone()
        {
            yield return new WaitForSeconds(2);
            recharging = true;
            inSpecial = InSpecial.None;
        }


        void OnEnable()
        {
            input.Gameplay.Enable();
        }
        void OnDisable()
        {
            input.Gameplay.Disable();
        }
    }
}
