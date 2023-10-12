using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

using JetBrains.Annotations;

#if HAS_UNITY_NETCODE && HAS_UNITY_TRANSPORT && HAS_UNITY_RELAY
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay.Models;
#endif

using U16 = System.UInt16;

namespace CGTK.Utils.Extensions.NetCode
{
    [PublicAPI]
    public static class UnityTransportExtensions
    {

        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static void SetHostRelayDataFromAlloc(this UnityTransport transport, in Allocation alloc)
        {
            transport.SetHostRelayData
            (
                ipAddress:      alloc.RelayServer.IpV4,
                port:      (U16)alloc.RelayServer.Port,
                allocationId:   alloc.AllocationIdBytes,
                key:            alloc.Key,
                connectionData: alloc.ConnectionData
            );
        }

        [MethodImpl(methodImplOptions: AggressiveInlining)]
        public static void SetClientRelayDataFromAlloc(this UnityTransport transport, in JoinAllocation alloc)
        {
            transport.SetClientRelayData
            (
                ipAddress:          alloc.RelayServer.IpV4,
                port:          (U16)alloc.RelayServer.Port,
                allocationId:       alloc.AllocationIdBytes,
                key:                alloc.Key,
                connectionData:     alloc.ConnectionData,
                hostConnectionData: alloc.HostConnectionData
            );
        }
        
    }
}