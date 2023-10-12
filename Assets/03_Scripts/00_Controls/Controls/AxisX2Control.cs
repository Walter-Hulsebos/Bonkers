namespace Bonkers.Controls
{
    using Sirenix.OdinInspector;

    using UnityEngine;

    using F32x2 = Unity.Mathematics.float2;
    
    [AddComponentMenu("Bonkers/Controls/Axis(×2) Control")]
    public sealed class AxisX2Control : MonoBehaviour,
                                        IControl<F32x2>
    {
        [BoxGroup("Value", showLabel: false)]
        [ReadOnly]
        [ShowInInspector]
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
        
        [LabelText("Axis (x)")]
        [InlineEditor]
        [SerializeField] private AxisControl axisX;
        
        [LabelText("Axis (y)")]
        [InlineEditor]
        [SerializeField] private AxisControl axisY;
    }
}
