using System.Collections.Generic;

[System.Serializable] // Wajib agar bisa diubah menjadi JSON
public class PlayerProgressData
{
    // Menyimpan ID misi yang sedang berjalan (Contoh: "Mission_01_Tutorial")
    public string currentMissionId = ""; 

    // Menyimpan daftar ID objektif yang sudah diselesaikan
    public List<string> completedObjectives = new List<string>();

    // Menyimpan daftar ID misi yang sudah tamat
    public List<string> completedMissions = new List<string>();
}