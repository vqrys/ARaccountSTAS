using UnityEngine;
using UnityEngine.Events; // Wajib ditambahkan untuk menggunakan UnityEvent
using System.Collections.Generic;

public class EventSequenceManager : MonoBehaviour
{
    [Header("Pengaturan Urutan Event")]
    [Tooltip("Daftar event yang akan dijalankan secara berurutan.")]
    public List<UnityEvent> eventList;

    [Tooltip("Jika dicentang, menekan Next di akhir akan kembali ke event pertama.")]
    public bool loopSequence = false; 

    [Tooltip("Jika dicentang, event pertama akan otomatis dijalankan saat game mulai.")]
    public bool playPertamaSaatStart = true;

    // Menyimpan posisi urutan saat ini (dimulai dari 0)
    private int currentIndex = 0;

    void Start()
    {
        // Jalankan event pertama (List 0) saat game dimulai
        if (playPertamaSaatStart && eventList.Count > 0)
        {
            InvokeCurrentEvent();
        }
    }

    /// <summary>
    /// Panggil fungsi ini menggunakan tombol "Next"
    /// </summary>
    public void NextEvent()
    {
        // Cegah error jika daftar event kosong
        if (eventList.Count == 0) return;

        // Tambah urutan
        currentIndex++;

        // Cek apakah sudah melewati batas akhir daftar
        if (currentIndex >= eventList.Count)
        {
            if (loopSequence)
            {
                currentIndex = 0; // Kembali ke awal
            }
            else
            {
                currentIndex = eventList.Count - 1; // Tetap di event terakhir
            }
        }

        // Jalankan event
        InvokeCurrentEvent();
    }

    /// <summary>
    /// Panggil fungsi ini menggunakan tombol "Previous"
    /// </summary>
    public void PreviousEvent()
    {
        // Cegah error jika daftar event kosong
        if (eventList.Count == 0) return;

        // Kurangi urutan
        currentIndex--;

        // Cek apakah mundur melewati batas awal (kurang dari 0)
        if (currentIndex < 0)
        {
            if (loopSequence)
            {
                currentIndex = eventList.Count - 1; // Langsung ke event paling akhir
            }
            else
            {
                currentIndex = 0; // Tetap di event pertama
            }
        }

        // Jalankan event
        InvokeCurrentEvent();
    }

    // Fungsi bantuan untuk menjalankan event pada urutan saat ini
    private void InvokeCurrentEvent()
    {
        Debug.Log("Menjalankan Event urutan ke-: " + currentIndex);
        eventList[currentIndex].Invoke();
    }
}