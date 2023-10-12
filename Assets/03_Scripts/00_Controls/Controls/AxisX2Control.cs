namespace Bonkers.Controls
{
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif

    using UnityEngine;

    using F32x2 = Unity.Mathematics.float2;
    
    [AddComponentMenu("Bonkers/Controls/Axis(×2) Control")]
    public sealed class AxisX2Control : MonoBehaviour,
                                        IControl<F32x2>
    {
        #if ODIN_INSPECTOR
        [BoxGroup("Value", showLabel: false)]
        [ReadOnly]
        [ShowInInspector]
        #endif
        public F32x2 Value
        {
            get
            {
                if (axisX == null || axisY == null)
                {
                    return F32x2.zero;
                }
                
                return new F32x2(x: axisX.Value, y: axisY.Value);
            }
        }
        
        #if ODIN_INSPECTOR
        [LabelText("Axis (x)")]
        [InlineEditor]
        #endif
        [SerializeField] private AxisControl axisX;
        
        #if ODIN_INSPECTOR
        [LabelText("Axis (y)")]
        [InlineEditor]
        #endif
        [SerializeField] private AxisControl axisY;
    }
}
