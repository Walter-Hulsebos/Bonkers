using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

namespace Bonkers
{
    public class PlayerSpecialMoves : MonoBehaviour
    {
        Controls.Controls input;

        public GameObject mine;
        public GameObject pigeons;

        public AnimatorController animatorController;

        bool done = true;

        bool inputSpecial1;
        bool inputSpecial2;
        bool inputSpecial3;
        bool inputRightshoulder;
        bool inputToggleKB;
        bool inputSpecialMouse;

        public GameObject player;

        // Start is called before the first frame update
        void Awake()
        {
            input = new Controls.Controls();

            input.Gameplay.Special1.performed += ctx => inputSpecial1 = ctx.ReadValueAsButton();

            input.Gameplay.Special2.performed += ctx => inputSpecial2 = ctx.ReadValueAsButton();

            input.Gameplay.Special3.performed += ctx => inputSpecial3 = ctx.ReadValueAsButton();

            input.Gameplay.SpecialsGamepad.performed += ctx => inputRightshoulder = ctx.ReadValueAsButton();

            input.Gameplay.SpecialMouse.performed += ctx => inputSpecialMouse = ctx.ReadValueAsButton();

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

            input.Gameplay.SpecialMouse.canceled += ctx => inputSpecialMouse = false;
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

            if (inputRightshoulder || inputSpecialMouse)
            {
                Debug.Log("input2 - RMB or R1 or right shoulder button pressed");
            }
        }

        void handleSpecial1()
        {
            if (inputSpecial1)
            {
                Debug.Log("input1 - LMB (for special 1) or square or X button pressed");
            }

            if (inputSpecial1 && inputRightshoulder && done == true || inputSpecial1 && inputToggleKB && done == true)
            {
                Debug.Log("Special1 - both input1 and input2 pressed");

            }
        }

        void handleSpecial2()
        {
            if (inputSpecial2)
            {
                Debug.Log("input1 - RMB (for special2) or triangle or Y button pressed");
            }

            if(inputSpecial2 && inputRightshoulder && done == true || inputSpecial2 && inputToggleKB && done == true)
            {
                Debug.Log("Special2 - both input1 and input2 pressed");
                Instantiate(pigeons, transform.position+(transform.up * 2), Quaternion.identity);
                done = false;
            }
        }

        void handleSpecial3()
        {
            if (inputSpecial3)
            {
                Debug.Log("input1 - LMB (for spceial3)  or cirlce or B button pressed");
            }                      

            if (inputSpecial3 && inputRightshoulder && done == true || inputSpecial3 && inputSpecialMouse && inputToggleKB && done == true)
            {
                Debug.Log("Special3 - both input1 and input2 pressed");
                Instantiate(mine, transform.position+(transform.forward*4), Quaternion.identity);
                animatorController.handleSpecial3();
                done = false;
            }
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
