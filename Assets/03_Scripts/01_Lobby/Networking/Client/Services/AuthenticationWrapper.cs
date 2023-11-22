using System;
using System.Threading.Tasks;

using Unity.Services.Authentication;
using Unity.Services.Core;

using UnityEngine;

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimedOut,
}

public static class AuthenticationWrapper
{
    public static AuthState AuthorizationState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(Int32 tries = 5)
    {
        //If we are already authenticated, just return Auth
        if (AuthorizationState == AuthState.Authenticated) { return AuthorizationState; }

        if (AuthorizationState == AuthState.Authenticating)
        {
            Debug.LogWarning(message: "Cant Authenticate if we are authenticating or authenticated");
            await Authenticating();
            return AuthorizationState;
        }

        await SignInAnonymouslyAsync(maxRetries: tries);
        Debug.Log(message: $"Auth attempts Finished : {AuthorizationState.ToString()}");

        return AuthorizationState;
    }

    //Awaitable task that will pass the clientID once authentication is done.
    public static String PlayerID() => AuthenticationService.Instance.PlayerId;

    //Awaitable task that will pass once authentication is done.
    public static async Task<AuthState> Authenticating()
    {
        while (AuthorizationState == AuthState.Authenticating || AuthorizationState == AuthState.NotAuthenticated) { await Task.Delay(millisecondsDelay: 200); }

        return AuthorizationState;
    }

    private static async Task SignInAnonymouslyAsync(Int32 maxRetries)
    {
        AuthorizationState = AuthState.Authenticating;
        Int32 tries = 0;

        while (AuthorizationState == AuthState.Authenticating && tries < maxRetries)
        {
            try
            {
                //To ensure staging login vs non staging
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthorizationState = AuthState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogError(message: ex);
                AuthorizationState = AuthState.Error;
            }
            catch (RequestFailedException exception)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogError(message: exception);
                AuthorizationState = AuthState.Error;
            }

            tries++;
            await Task.Delay(millisecondsDelay: 1000);
        }

        if (AuthorizationState != AuthState.Authenticated)
        {
            Debug.LogWarning(message: $"Player was not signed in successfully after {tries} attempts");
            AuthorizationState = AuthState.TimedOut;
        }
    }

    public static void SignOut()
    {
        AuthenticationService.Instance.SignOut(clearCredentials: false);
        AuthorizationState = AuthState.NotAuthenticated;
    }
}