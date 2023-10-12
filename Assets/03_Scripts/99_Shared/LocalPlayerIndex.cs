namespace Bonkers.Shared
{
    using UnityEngine;
    
    using I32 = System.Int32;
    
    public sealed class LocalPlayerIndex : MonoBehaviour
    {
        [field:SerializeField] public I32 Index { get; set; } = -1;

        private void Awake()
        {
            if (Index < 0)
            {
                Debug.LogError(message: $"{nameof(LocalPlayerIndex)} on {gameObject.name} is less than 0!", context: this);
            }
        }
    }
}
