namespace Bonkers.Controls
{
    using UnityEngine.InputSystem;

    public interface ISettableControl<T> : IControl<T>
    {
        internal void SetValue(T value);

        public void OnAnyInputCallback(InputAction.CallbackContext callbackContext);
        
        public InputActionReference Action { get; }
    }
}
