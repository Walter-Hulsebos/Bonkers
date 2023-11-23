using System;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;

using UnityEngine;

public class MultiplayAllocationService : IDisposable
{
    private IMultiplayService       multiplayService;
    private MultiplayEventCallbacks serverCallbacks;
    private IServerQueryHandler     serverCheckManager;
    private IServerEvents           serverEvents;
    private CancellationTokenSource serverCheckCancel;
    private String                  allocationId;

    public MultiplayAllocationService()
    {
        try
        {
            multiplayService  = MultiplayService.Instance;
            serverCheckCancel = new CancellationTokenSource();
        }
        catch (Exception ex) { Debug.LogWarning(message: $"Error creating Multiplay allocation service.\n{ex}"); }
    }

    public async Task<MatchmakingResults> SubscribeAndAwaitMatchmakerAllocation()
    {
        if (multiplayService == null) { return null; }

        allocationId             =  null;
        serverCallbacks          =  new MultiplayEventCallbacks();
        serverCallbacks.Allocate += OnMultiplayAllocation;
        serverEvents             =  await multiplayService.SubscribeToServerEventsAsync(callbacks: serverCallbacks);

        String             allocationID       = await AwaitAllocationID();
        MatchmakingResults matchmakingPayload = await GetMatchmakerAllocationPayloadAsync();

        return matchmakingPayload;
    }

    private async Task<String> AwaitAllocationID()
    {
        ServerConfig config = multiplayService.ServerConfig;

        Debug.Log
        (
            message: $"Awaiting Allocation. Server Config is:\n" + $"-ServerID: {config.ServerId}\n" + $"-AllocationID: {config.AllocationId}\n" +
                     $"-Port: {config.Port}\n"                   + $"-QPort: {config.QueryPort}\n"   + $"-logs: {config.ServerLogDirectory}"
        );

        while (String.IsNullOrEmpty(value: allocationId))
        {
            String configID = config.AllocationId;

            if (!String.IsNullOrEmpty(value: configID) && String.IsNullOrEmpty(value: allocationId))
            {
                Debug.Log(message: $"Config had AllocationID: {configID}");
                allocationId = configID;
            }

            await Task.Delay(millisecondsDelay: 100);
        }

        return allocationId;
    }

    private async Task<MatchmakingResults> GetMatchmakerAllocationPayloadAsync()
    {
        MatchmakingResults payloadAllocation = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();
        String             modelAsJson       = JsonConvert.SerializeObject(value: payloadAllocation, formatting: Formatting.Indented);
        Debug.Log(message: nameof(GetMatchmakerAllocationPayloadAsync) + ":" + Environment.NewLine + modelAsJson);
        return payloadAllocation;
    }

    private void OnMultiplayAllocation(MultiplayAllocation allocation)
    {
        Debug.Log(message: $"OnAllocation: {allocation.AllocationId}");

        if (String.IsNullOrEmpty(value: allocation.AllocationId)) { return; }

        allocationId = allocation.AllocationId;
    }

    public async Task BeginServerCheck()
    {
        if (multiplayService == null) { return; }

        serverCheckManager = await multiplayService.StartServerQueryHandlerAsync(maxPlayers: (UInt16)20, serverName: "", gameType: "", buildId: "0", map: "");

        #pragma warning disable 4014
        ServerCheckLoop(cancellationToken: serverCheckCancel.Token);
        #pragma warning restore 4014
    }

    public void SetServerName(String name) { serverCheckManager.ServerName = name; }
    public void SetBuildID(String    id)   { serverCheckManager.BuildId    = id; }

    public void SetMaxPlayers(UInt16 players) { serverCheckManager.MaxPlayers = players; }

    public void AddPlayer() { serverCheckManager.CurrentPlayers++; }

    public void RemovePlayer() { serverCheckManager.CurrentPlayers--; }

    public void SetMap(String newMap) { serverCheckManager.Map = newMap; }

    public void SetMode(String mode) { serverCheckManager.GameType = mode; }

    private async Task ServerCheckLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            serverCheckManager.UpdateServerCheck();
            await Task.Delay(millisecondsDelay: 100);
        }
    }

    private void OnMultiplayDeAllocation(MultiplayDeallocation deallocation)
    {
        Debug.Log(message: $"Multiplay Deallocated : ID: {deallocation.AllocationId}\nEvent: {deallocation.EventId}\nServer{deallocation.ServerId}");
    }

    private void OnMultiplayError(MultiplayError error) { Debug.Log(message: $"MultiplayError : {error.Reason}\n{error.Detail}"); }

    public void Dispose()
    {
        if (serverCallbacks != null)
        {
            serverCallbacks.Allocate   -= OnMultiplayAllocation;
            serverCallbacks.Deallocate -= OnMultiplayDeAllocation;
            serverCallbacks.Error      -= OnMultiplayError;
        }

        if (serverCheckCancel != null) { serverCheckCancel.Cancel(); }

        serverEvents?.UnsubscribeAsync();
    }
}