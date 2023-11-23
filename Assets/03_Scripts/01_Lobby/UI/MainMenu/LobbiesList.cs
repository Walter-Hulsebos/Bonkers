using System;
using System.Collections;
using System.Collections.Generic;

using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private Transform lobbyItemParent;
    [SerializeField] private LobbyItem lobbyItemPrefab;

    private Boolean isRefreshing;
    private Boolean isJoining;

    private void OnEnable() { RefreshList(); }

    public async void RefreshList()
    {
        if (isRefreshing) { return; }

        isRefreshing = true;

        try
        {
            QueryLobbiesOptions options = new ();
            options.Count = 25;

            options.Filters = new List<QueryFilter>()
            {
                new(QueryFilter.FieldOptions.AvailableSlots, op: QueryFilter.OpOptions.GT, value: "0"),
                new(QueryFilter.FieldOptions.IsLocked, op: QueryFilter.OpOptions.EQ, value: "0"),
            };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            foreach (Transform child in lobbyItemParent) { Destroy(child.gameObject); }

            foreach (Lobby lobby in lobbies.Results)
            {
                LobbyItem lobbyInstance = Instantiate(lobbyItemPrefab, lobbyItemParent);
                lobbyInstance.Initialise(this, lobby);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            isRefreshing = false;
            throw;
        }

        isRefreshing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isJoining) { return; }

        isJoining = true;

        try
        {
            Lobby  joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            String joinCode     = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.Manager.BeginConnection(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            isJoining = false;
            throw;
        }

        isJoining = false;
    }
}