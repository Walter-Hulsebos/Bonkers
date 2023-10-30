using System;

//using Bonkers.Characters;

using CGTK.Utils.Extensions.Math.Math;

using Sirenix.OdinInspector;

using UnityEngine;
using static UnityEngine.Mathf;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;

//using static Bonkers.Characters.OrientationMethod;

using F32 = System.Single;
using F32x3 = Unity.Mathematics.float3;

[Serializable]
    public sealed class PlayerAttackState : PlayerBaseState
    {

    #region Variables

    [SerializeField] private F32 Damage = 10;

    #endregion

    #region Constructor
    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    #endregion

        public override void EnterState()
        {
        Debug.Log("Penis is active");
        Ctx.Anims.SetTrigger(Ctx.Special1Hash);
    }

    public override void ExitState()
    {
    }

    #region Updates

    protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
        }

    protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
        //if(Ctx.Anims.GetCurrentAnimatorClipInfo(layerIndex: 0))
        AnimatorStateInfo __animStateInfo = Ctx.Anims.GetCurrentAnimatorStateInfo(layerIndex: 0);

        Debug.Log($"Expected Hash = {Ctx.Special1Hash}, tag hash {__animStateInfo.tagHash}, short hash {__animStateInfo.shortNameHash}, long hash {__animStateInfo.fullPathHash}");
        // if (__animStateInfo.tagHash == Ctx.Special1Hash)
        //{
            F32 animPercentage = __animStateInfo.normalizedTime;

            if (animPercentage > 1.0f)
            {
                
            }
        //}
    }
    #endregion
}

