using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class PlayerKnockback
    {
        //Knockback functionality for player objects that have been hit

        [SerializeField] private PlayerStateMachine playerStateMachine;

        private Vector3 attackOriginPos;    //Attacking player position
        private Vector3 attackTargetPos;    //Receiving player position
        private Vector3 knockDirection;     //Direction of knockback
        private float knockBackAmount = 1;  //Rutgers specified knockback calculation

        [SerializeField] private AnimationCurve axisKnockback_XY;   //Knockback behaviour for X- and Y-Axis
        [SerializeField] private AnimationCurve axisKnockback_Z;    //Knockback behaviour for Z-Axis

        private bool allowKnockback;    //Whether knockback will be applied
        private float timeElapsed;      //Time elapsed since start of knockback

        private void StartKnockback()
        {
            attackTargetPos = playerStateMachine.transform.position;    //Getting receiving players position
            knockDirection = attackTargetPos - attackOriginPos;         //Calculating knockback direction
            
            //Rutger Knockback Calculation, I dont know where all required variables are stored
            //knockBackAmount = ((((currentDamage + attackDamage) * characterWeight) * attackKnockBackScaling) + attackBaseKnockBackValue);

            allowKnockback = true;  //Allowing knockback behaviour
        }

        private void EndKnockback()
        {
            allowKnockback = false; //Deniying knockback behaviour
        }

        private void FixedUpdate()
        {
            //Checking which anim curve is longer, increasing timeElapsed based on longest knockback behaviour
            if (timeElapsed < Mathf.Max(axisKnockback_XY.length, axisKnockback_Z.length))
            {
                timeElapsed += Time.fixedDeltaTime;
            }
            else
            {
                EndKnockback();
            }

            //Adding velocity based on AnimationCurve evaluation
            if (allowKnockback)
            {
                playerStateMachine.AddVelocity(new Vector3
                                (knockDirection.x * axisKnockback_XY.Evaluate(timeElapsed) * Time.fixedDeltaTime * knockBackAmount,
                                knockDirection.y * axisKnockback_Z.Evaluate(timeElapsed) * Time.fixedDeltaTime * knockBackAmount,
                                knockDirection.z * axisKnockback_XY.Evaluate(timeElapsed) * Time.fixedDeltaTime * knockBackAmount
                                ));
            }
        }
    }
}
