using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootLockerBootstrap : MonoBehaviour
{
    public static bool SessionStarted { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        StartGuest();
    }

    void StartGuest()
    {
        LootLockerSDKManager.StartGuestSession("", response =>
        {
            if (!response.success)
            {
                Debug.LogError("Fallo");
                return;
            }
            SessionStarted = true;
            Debug.Log("Conectado");
        });
    }
}