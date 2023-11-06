using System;
using System.Linq;

using Bonkers.Characters;

using CGTK.Utils.Extensions.Math.Math;
using CGTK.Utils.UnityFunc;

using UnityEngine;
using static UnityEngine.Mathf;

using Unity.Netcode;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;

using KinematicCharacterController;

using Sirenix.OdinInspector;

using UnityEngine.Serialization;

using static Bonkers.Characters.OrientationMethod;

using F32 = System.Single;
using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;

using I32 = System.Int32;
using I32x2 = Unity.Mathematics.int2;
using I32x3 = Unity.Mathematics.int3;

using Bool = System.Boolean;
using Rotor = Unity.Mathematics.quaternion;

namespace Bonkers
{
    public class CampaignStateMachine : PlayerStateMachine
    {

        [SerializeField] UnityFunc<bool> getTestButton;
        [SerializeField] UnityFunc<bool> getAttackButton;
        [SerializeField] UnityFunc<bool> getInteractionButton;
        [SerializeField] UnityFunc<bool> getTimedEventButton; 
        protected override void Update()
        {
            base.Update();

            if (getTestButton.Invoke())
            {
                print("Invoked Test Buttons");
                CampaignManager.instance.ToNextScene();
            }
            if (getAttackButton.Invoke())
            {
                CampaignManager.instance.player.GetComponentInChildren<PlayerHitBox>().OnReceiveInput();
            }
            if (getInteractionButton.Invoke())
            {
                CampaignManager.instance.ForwardNextSentence();
            }
            if (getTimedEventButton.Invoke())
            {
                CampaignManager.instance.ForwardTimedEvent();
            }

        }
    }
}
