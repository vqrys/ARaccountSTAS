using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

public class UGSBootstrap : MonoBehaviour
{
    public static bool IsInitialized { get; private set; }

    private async void Awake()
    {
        if (IsInitialized) return;

        await InitializeServicesAsync();
    }

    public static async Task InitializeServicesAsync()
    {
        if (IsInitialized) return;

        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            await UnityServices.InitializeAsync();
        }

        IsInitialized = true;
        Debug.Log("[UGSBootstrap] Unity Services initialized.");
    }
}