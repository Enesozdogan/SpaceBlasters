using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthHelper
{
    public static AuthState AuthState { get; private set; } = AuthState.Fail;

    /// <summary>
    /// Auth islemini max deneme sayisi ile gerceklestiren ve kontrol eden fonksion.
    /// </summary>
    /// <param name="maxAttempt"></param>
    /// <returns></returns>
    public static async Task<AuthState> DoAuth(int maxAttempt=5)
    {

        if (AuthState == AuthState.Success) return AuthState.Success;
        
        if(AuthState == AuthState.Waiting)
        {
            Debug.LogWarning("Already in Authentication Process");
            await WaitAuthentication();
        }
        await SignInAnonimousAsync(maxAttempt);
       
        return AuthState;

    }

    private static async Task<AuthState> WaitAuthentication()
    {
        while(AuthState == AuthState.Waiting || AuthState == AuthState.Fail)
        {
            await Task.Delay(200);
        }
        return AuthState;
    }

    public static async Task SignInAnonimousAsync(int maxAttempt)
    {
        int attempt = 0;
        while (AuthState != AuthState.Success && attempt < maxAttempt)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Success;
                    break;
                }
            }
            catch (AuthenticationException ex)
            {
                Debug.LogError(ex);
                AuthState = AuthState.Error;
            }
            catch (RequestFailedException ex1)
            {
                Debug.LogError(ex1);
                AuthState = AuthState.Error;
            }




            attempt++;
            await Task.Delay(1000);
        }

        if(AuthState != AuthState.Success)
        {
            Debug.LogError("Authentication Timed out");
            AuthState=AuthState.Timeout;
        }
    }

}

public enum  AuthState
{
    Fail,
    Success,
    Waiting,
    Error,
    Timeout

}
