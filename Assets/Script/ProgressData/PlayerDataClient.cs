using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;

// Class kecil untuk membungkus data profil
public class PlayerProfile
{
    public string Name;
    public string Avatar;
}

public class PlayerDataClient : MonoBehaviour
{
    public static PlayerDataClient Instance { get; private set; }

    private const string PlayerNameKey = "PLAYER_NAME";
    private const string PlayerAvatarKey = "PLAYER_AVATAR";
    private const string PlayerProgressKey = "PLAYER_PROGRESS"; // Kunci untuk data JSON Progres Misi

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- BAGIAN PLAYER PROFILE (NAME & AVATAR) --- //

    // Mengambil Nama dan Avatar sekaligus
    public async Task<PlayerProfile> GetProfileDataAsync()
    {
        var profile = new PlayerProfile { Name = string.Empty, Avatar = "Boy" }; // Default

        var data = await CloudSaveService.Instance.Data.Player.LoadAsync(
            new HashSet<string> { PlayerNameKey, PlayerAvatarKey });

        if (data.TryGetValue(PlayerNameKey, out var nameItem))
        {
            profile.Name = nameItem.Value.GetAs<string>();
        }

        if (data.TryGetValue(PlayerAvatarKey, out var avatarItem))
        {
            profile.Avatar = avatarItem.Value.GetAs<string>();
        }

        return profile;
    }

    // Menyimpan Nama dan Avatar
    public async Task SaveProfileDataAsync(string playerName, string avatarId)
    {
        string trimmedName = playerName.Trim();

        var data = new Dictionary<string, object>
        {
            { PlayerNameKey, trimmedName },
            { PlayerAvatarKey, avatarId }
        };

        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
    }

    public bool IsValidPlayerName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        string name = value.Trim();

        if (name.Length < 4 || name.Length > 16)
            return false;

        foreach (char c in name)
        {
            if (!char.IsLetterOrDigit(c) && c != ' ') // Boleh pakai spasi
                return false;
        }

        return true;
    }


    // --- BAGIAN PROGRESS DATA (JSON HYBRID) --- //
    
    // Fungsi untuk mengambil data JSON dari Cloud dan mengubahnya kembali jadi Object
    public async Task<PlayerProgressData> LoadProgressAsync()
    {
        try
        {
            var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { PlayerProgressKey });
            if (data.TryGetValue(PlayerProgressKey, out var item))
            {
                string jsonString = item.Value.GetAs<string>();
                Debug.Log($"[PlayerDataClient] Progress Loaded: {jsonString}");
                
                // Ubah JSON string kembali menjadi C# Object
                return JsonUtility.FromJson<PlayerProgressData>(jsonString);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[PlayerDataClient] Gagal memuat progres: {e.Message}");
        }

        // Jika pemain baru / belum ada data, kembalikan data kosong
        return new PlayerProgressData();
    }

    // Fungsi untuk mengubah Object C# menjadi JSON lalu menyimpannya ke Cloud
    public async Task SaveProgressAsync(PlayerProgressData progressData)
    {
        try
        {
            // Ubah Object menjadi JSON string
            string jsonString = JsonUtility.ToJson(progressData);
            Debug.Log($"[PlayerDataClient] Saving Progress: {jsonString}");

            var data = new Dictionary<string, object> { { PlayerProgressKey, jsonString } };
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[PlayerDataClient] Gagal menyimpan progres: {e.Message}");
        }
    }
}