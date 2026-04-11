using UnityEngine;

public class GameHandler : MonoBehaviour
{
    private void Awake()
    {
        SaveSystem.Init();
    }

    // ================= SAVE =================

    public void SavePlayerName(string namaPemain)
    {
        SaveObject saveObject = LoadSaveObject();
        saveObject.nama = namaPemain;

        string json = JsonUtility.ToJson(saveObject);
        SaveSystem.Save(json);
    }

    public void SaveTutorialFinished()
    {
        SaveObject saveObject = LoadSaveObject();
        saveObject.tutorialFinished = true;

        string json = JsonUtility.ToJson(saveObject);
        SaveSystem.Save(json);
    }

    public void ResetGame()
    {
        SaveSystem.DeleteSave();
    }


    // ================= LOAD =================

    public string LoadPlayerName()
    {
        SaveObject saveObject = LoadSaveObject();
        return saveObject.nama;
    }

    public bool IsTutorialFinished()
    {
        SaveObject saveObject = LoadSaveObject();
        return saveObject.tutorialFinished;
    }
    

    // ================= CORE =================

    private SaveObject LoadSaveObject()
    {
        string json = SaveSystem.Load();
        if (string.IsNullOrEmpty(json))
        {
            return new SaveObject();
        }

        return JsonUtility.FromJson<SaveObject>(json);
    }

    [System.Serializable]
    private class SaveObject
    {
        public string nama;
        public bool tutorialFinished;
    }
}
