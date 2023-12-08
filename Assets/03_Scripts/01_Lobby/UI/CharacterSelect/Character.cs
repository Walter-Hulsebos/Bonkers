using System;

using Unity.Netcode;

using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if ODIN_INSPECTOR
[InlineEditor]
#endif
[CreateAssetMenu(fileName = "New Character", menuName = "Characters/Character")]
public class Character : ScriptableObject
{
    [SerializeField] private Int32         id          = -1;
    [SerializeField] private String        displayName = "New Display Name";
    [SerializeField] private Sprite        icon;
    [SerializeField] private GameObject    introPrefab;
    // #if ODIN_INSPECTOR
    // [InlineEditor]
    // #endif
    [SerializeField] private NetworkObject gameplayPrefab;

    public Int32         Id             => id;
    public String        DisplayName    => displayName;
    public Sprite        Icon           => icon;
    public GameObject    IntroPrefab    => introPrefab;
    public NetworkObject GameplayPrefab => gameplayPrefab;
}