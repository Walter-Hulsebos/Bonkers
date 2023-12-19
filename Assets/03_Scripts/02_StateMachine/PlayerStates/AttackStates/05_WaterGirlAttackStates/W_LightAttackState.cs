using System;

using CGTK.Utils.Extensions.Math.Math;

using Sirenix.OdinInspector;

using UnityEngine;
using static UnityEngine.Mathf;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;

//using static Bonkers.Characters.OrientationMethod;

using F32 = System.Single;
using F32x3 = Unity.Mathematics.float3;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

    public sealed class W_LightAttackState : PlayerBaseState
    {
    #region Enums
    private InLightAttack inLight;

    #endregion

    #region Constructor
    public W_LightAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    #endregion

    private enum InLightAttack
    {
        None,
        Light
    }
    public override void EnterState()
    {
        Ctx.KnockBackPlane.SetActive(true);
        Ctx.Anims.SetTrigger(Ctx.LightAttackHash);
        HandleWaterGirlLightAttack();
    }

    public override void ExitState() {
        Ctx.KnockBackPlane.SetActive(false);
     }

    void HandleWaterGirlLightAttack()
    {
        inLight = InLightAttack.Light;
    }


    #region Updates

    protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime) { }
    protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        currentVelocity = Vector3.zero;

        //if(Ctx.Anims.GetCurrentAnimatorClipInfo(layerIndex: 0))
        AnimatorStateInfo __animStateInfo = Ctx.Anims.GetCurrentAnimatorStateInfo(layerIndex: 0);

        Debug.Log($"Expected Hash = {Ctx.LightAttackHash}, tag hash {__animStateInfo.tagHash}, short hash {__animStateInfo.shortNameHash}, long hash {__animStateInfo.fullPathHash}");
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

