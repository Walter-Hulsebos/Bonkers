namespace Bonkers
{
    using UnityEngine;

    using UnityEngine.Events;
    using UnityEngine.InputSystem;

    public sealed class ActionsOnPlayerInput : MonoBehaviour
    {
        [SerializeField] private UnityEvent[] actionsOnPlayerInput;

        [SerializeField] private UnityEvent[] actionsOnPlayerGameObject;

        public void OnSpawned(PlayerInput playerInput)
        {
            foreach (UnityEvent __action in actionsOnPlayerInput)
            {
                __action.Invoke();
            }
        }
    }
}