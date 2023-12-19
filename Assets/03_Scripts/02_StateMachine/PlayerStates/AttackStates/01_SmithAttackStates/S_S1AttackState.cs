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
using Cysharp.Threading.Tasks;

[Serializable]
public sealed class S_S1AttackState : PlayerBaseState
 {
    #region Enums
    private InSpecial1 inSpecial1;
    #endregion

    #region Constructor
    public S_S1AttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    #endregion
    private enum InSpecial1
    {
        None,
        Special1
    }
    public override void EnterState()
    {
        Ctx.Anims.SetTrigger(Ctx.Special1Hash);
        HandleSmithSpecial1();
    }

    public override void ExitState(){}

    void HandleSmithSpecial1()
    {
        inSpecial1 = InSpecial1.Special1;
        // do your special here 
    }

    #region Updates

    protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime){}

    protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        currentVelocity = Vector3.zero;

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
