namespace Bonkers.Controls
{
    using UnityEngine.InputSystem;

    public interface ISettableControl<out T> : IControl<T>
    {
        public void OnAnyInputCallback(InputAction.CallbackContext callbackContext);

        public InputActionReference Action { get; }
    }
}