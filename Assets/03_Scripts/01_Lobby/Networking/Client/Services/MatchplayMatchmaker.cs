using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;

using UnityEngine;

public enum MatchmakerPollingResult
{
    Success,
    TicketCreationError,
    TicketCancellationError,
    TicketRetrievalError,
    MatchAssignmentError,
}

public class MatchmakingResult
{
    public String                  ip;
    public Int32                   port;
    public MatchmakerPollingResult result;
    public String                  resultMessage;
}

public class MatchplayMatchmaker : IDisposable
{
    private String                  lastUsedTicket;
    private CancellationTokenSource cancelToken;

    private const Int32 TicketCooldown = 1000;

    public Boolean IsMatchmaking { get; private set; }

    public async Task<MatchmakingResult> Matchmake(UserData data)
    {
        cancelToken = new CancellationTokenSource();

        String              queueName           = data.userGamePreferences.ToMultiplayQueue();
        CreateTicketOptions createTicketOptions = new (queueName);
        Debug.Log(createTicketOptions.QueueName);

        List<Player> players = new()  { new Player (data.userAuthId, data.userGamePreferences), };

        try
        {
            IsMatchmaking = true;
            CreateTicketResponse createResult = await MatchmakerService.Instance.CreateTicketAsync(players, createTicketOptions);

            lastUsedTicket = createResult.Id;

            try
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    TicketStatusResponse checkTicket = await MatchmakerService.Instance.GetTicketAsync(lastUsedTicket);

                    if (checkTicket.Type == typeof(MultiplayAssignment))
                    {
                        MultiplayAssignment matchAssignment = (MultiplayAssignment)checkTicket.Value;

                        if (matchAssignment.Status == MultiplayAssignment.StatusOptions.Found)
                        {
                            return ReturnMatchResult(MatchmakerPollingResult.Success, "", matchAssignment);
                        }

                        if (matchAssignment.Status == MultiplayAssignment.StatusOptions.Timeout ||
                            matchAssignment.Status == MultiplayAssignment.StatusOptions.Failed)
                        {
                            return ReturnMatchResult
                            (
                                MatchmakerPollingResult.MatchAssignmentError,
                                $"Ticket: {lastUsedTicket} - {matchAssignment.Status} - {matchAssignment.Message}",
                                null
                            );
                        }

                        Debug.Log($"Polled Ticket: {lastUsedTicket} Status: {matchAssignment.Status} ");
                    }

                    await Task.Delay(TicketCooldown);
                }
            }
            catch (MatchmakerServiceException e) { return ReturnMatchResult(MatchmakerPollingResult.TicketRetrievalError, e.ToString(), null); }
        }
        catch (MatchmakerServiceException e) { return ReturnMatchResult(MatchmakerPollingResult.TicketCreationError, e.ToString(), null); }

        return ReturnMatchResult(MatchmakerPollingResult.TicketRetrievalError, "Cancelled Matchmaking", null);
    }

    public async Task CancelMatchmaking()
    {
        if (!IsMatchmaking) { return; }

        IsMatchmaking = false;

        if (cancelToken.Token.CanBeCanceled) { cancelToken.Cancel(); }

        if (String.IsNullOrEmpty(lastUsedTicket)) { return; }

        Debug.Log($"Cancelling {lastUsedTicket}");

        await MatchmakerService.Instance.DeleteTicketAsync(lastUsedTicket);
    }

    private MatchmakingResult ReturnMatchResult(MatchmakerPollingResult resultErrorType, String message, MultiplayAssignment assignment)
    {
        IsMatchmaking = false;

        if (assignment != null)
        {
            String parsedIp   = assignment.Ip;
            Int32? parsedPort = assignment.Port;

            if (parsedPort == null)
            {
                return new MatchmakingResult
                {
                    result        = MatchmakerPollingResult.MatchAssignmentError,
                    resultMessage = $"Port missing? - {assignment.Port}\n-{assignment.Message}",
                };
            }

            return new MatchmakingResult
            {
                result = MatchmakerPollingResult.Success, ip = parsedIp, port = (Int32)parsedPort, resultMessage = assignment.Message,
            };
        }

        return new MatchmakingResult { result = resultErrorType, resultMessage = message, };
    }

    public void Dispose()
    {
        _ = CancelMatchmaking();

        cancelToken?.Dispose();
    }
}