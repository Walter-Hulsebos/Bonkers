//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/characters/Input/CharacterInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @CharacterInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @CharacterInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""CharacterInput"",
    ""maps"": [
        {
            ""name"": ""Character map"",
            ""id"": ""ea0b4c61-2355-4e17-9590-893e387e3978"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""7adfbb95-b5d2-4653-9634-bd38d6f1001b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Ability1"",
                    ""type"": ""Button"",
                    ""id"": ""7d984220-de64-468b-8d0a-bbf938ec6478"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Ability2"",
                    ""type"": ""Button"",
                    ""id"": ""f4c67c86-4753-46e9-a010-7ed1ea2413e1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""4b28f354-5b33-42f0-aea5-a2eb3ed816d2"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c445289e-0f0e-4b13-b743-6a056f5e92cd"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ability1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ff76de08-e777-41a4-9c67-f4a83c96f553"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ability2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Character map
        m_Charactermap = asset.FindActionMap("Character map", throwIfNotFound: true);
        m_Charactermap_Movement = m_Charactermap.FindAction("Movement", throwIfNotFound: true);
        m_Charactermap_Ability1 = m_Charactermap.FindAction("Ability1", throwIfNotFound: true);
        m_Charactermap_Ability2 = m_Charactermap.FindAction("Ability2", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Character map
    private readonly InputActionMap m_Charactermap;
    private List<ICharactermapActions> m_CharactermapActionsCallbackInterfaces = new List<ICharactermapActions>();
    private readonly InputAction m_Charactermap_Movement;
    private readonly InputAction m_Charactermap_Ability1;
    private readonly InputAction m_Charactermap_Ability2;
    public struct CharactermapActions
    {
        private @CharacterInput m_Wrapper;
        public CharactermapActions(@CharacterInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Charactermap_Movement;
        public InputAction @Ability1 => m_Wrapper.m_Charactermap_Ability1;
        public InputAction @Ability2 => m_Wrapper.m_Charactermap_Ability2;
        public InputActionMap Get() { return m_Wrapper.m_Charactermap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CharactermapActions set) { return set.Get(); }
        public void AddCallbacks(ICharactermapActions instance)
        {
            if (instance == null || m_Wrapper.m_CharactermapActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_CharactermapActionsCallbackInterfaces.Add(instance);
            @Movement.started += instance.OnMovement;
            @Movement.performed += instance.OnMovement;
            @Movement.canceled += instance.OnMovement;
            @Ability1.started += instance.OnAbility1;
            @Ability1.performed += instance.OnAbility1;
            @Ability1.canceled += instance.OnAbility1;
            @Ability2.started += instance.OnAbility2;
            @Ability2.performed += instance.OnAbility2;
            @Ability2.canceled += instance.OnAbility2;
        }

        private void UnregisterCallbacks(ICharactermapActions instance)
        {
            @Movement.started -= instance.OnMovement;
            @Movement.performed -= instance.OnMovement;
            @Movement.canceled -= instance.OnMovement;
            @Ability1.started -= instance.OnAbility1;
            @Ability1.performed -= instance.OnAbility1;
            @Ability1.canceled -= instance.OnAbility1;
            @Ability2.started -= instance.OnAbility2;
            @Ability2.performed -= instance.OnAbility2;
            @Ability2.canceled -= instance.OnAbility2;
        }

        public void RemoveCallbacks(ICharactermapActions instance)
        {
            if (m_Wrapper.m_CharactermapActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ICharactermapActions instance)
        {
            foreach (var item in m_Wrapper.m_CharactermapActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_CharactermapActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public CharactermapActions @Charactermap => new CharactermapActions(this);
    public interface ICharactermapActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnAbility1(InputAction.CallbackContext context);
        void OnAbility2(InputAction.CallbackContext context);
    }
}
