using System;

using Unity.Collections;
using Unity.Netcode;

public class SynchedServerData : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> serverId  = new ();
    public NetworkVariable<Map>                map       = new ();
    public NetworkVariable<GameMode>           gameMode  = new ();
    public NetworkVariable<GameQueue>          gameQueue = new ();

    public Action OnNetworkSpawned;

    public override void OnNetworkSpawn() { OnNetworkSpawned?.Invoke(); }
}