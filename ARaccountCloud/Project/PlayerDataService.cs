using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using Unity.Services.CloudSave.Model;

namespace ARaccountCloud;

public class PlayerDataService
{
    public const string PlayerNameKey = "PLAYER_NAME";
    public const string PlayerProgressKey = "PLAYER_PROGRESS"; // Tambahkan Key untuk JSON

    private readonly ILogger<PlayerDataService> _logger;

    public PlayerDataService(ILogger<PlayerDataService> logger)
    {
        _logger = logger;
    }

    [CloudCodeFunction("SavePlayerData")]
    public async Task SavePlayerData(IExecutionContext context, IGameApiClient gameApiClient, string key, string value)
    {
        try
        {
            await gameApiClient.CloudSaveData.SetItemAsync(
                context, context.AccessToken!, context.ProjectId!, context.PlayerId!,
                new SetItemBody(key, value)
            );
            _logger.LogInformation("Saved player data. Key: {Key}, PlayerId: {PlayerId}", key, context.PlayerId);
        }
        catch (ApiException ex)
        {
            _logger.LogError("Failed to save player data. Key: {Key}, Error: {Error}", key, ex.Message);
            throw new Exception($"Unable to save player data for key '{key}': {ex.Message}");
        }
    }

    [CloudCodeFunction("GetPlayerData")]
    public async Task<string> GetPlayerData(IExecutionContext context, IGameApiClient gameApiClient, string key)
    {
        try
        {
            var result = await gameApiClient.CloudSaveData.GetItemsAsync(
                context, context.AccessToken!, context.ProjectId!, context.PlayerId!,
                new List<string> { key }
            );

            var item = result.Data.Results.FirstOrDefault(x => x.Key == key);

            if (item?.Value == null) return string.Empty;

            return item.Value.ToString() ?? string.Empty;
        }
        catch (ApiException ex)
        {
            _logger.LogError("Failed to get player data. Key: {Key}, Error: {Error}", key, ex.Message);
            throw new Exception($"Unable to retrieve player data for key '{key}': {ex.Message}");
        }
    }

    [CloudCodeFunction("HandleNewPlayerNameEntry")]
    public async Task<string> HandleNewPlayerNameEntry(IExecutionContext context, IGameApiClient gameApiClient, string newName)
    {
        if (!IsPlayerNameValid(newName)) throw new ArgumentException("Name invalid.");
        var trimmedName = newName.Trim();
        await SavePlayerData(context, gameApiClient, PlayerNameKey, trimmedName);
        return trimmedName;
    }

    // --- FUNGSI BARU UNTUK PROGRES GAME ---
    [CloudCodeFunction("SavePlayerProgress")]
    public async Task<string> SavePlayerProgress(IExecutionContext context, IGameApiClient gameApiClient, string jsonProgress)
    {
        if (string.IsNullOrWhiteSpace(jsonProgress))
        {
            throw new ArgumentException("Progress data is empty.");
        }

        // Menyimpan string JSON ke Cloud Save melalui Server
        await SavePlayerData(context, gameApiClient, PlayerProgressKey, jsonProgress);

        _logger.LogInformation("Player progress JSON saved successfully. PlayerId: {PlayerId}", context.PlayerId);
        return "Progress Saved";
    }

    private bool IsPlayerNameValid(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        name = name.Trim();
        if (name.Length < 4 || name.Length > 16) return false;
        return name.All(char.IsLetterOrDigit);
    }
}