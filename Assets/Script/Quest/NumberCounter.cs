using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class NumberCounter : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public int CountFPS = 30;
    public float Duration = 1f;
    public string NumberFormat = "N0";
    
    private int _value;
    public int Value
    {
        get { return _value; }
        set
        {
            UpdateText(value);
            _value = value;
        }
    }
    
    private Coroutine CountingCoroutine;

    private void Awake()
    {
        Text = GetComponent<TextMeshProUGUI>();
        
        // Pastikan teks tidak blank saat game baru mulai
        if (Text != null) Text.text = "0"; 
    }

    private void UpdateText(int newValue)
    {
        if (CountingCoroutine != null)
        {
            StopCoroutine(CountingCoroutine);
        }

        CountingCoroutine = StartCoroutine(CountText(newValue));
    }

    private IEnumerator CountText(int newValue)
    {
        // --- PERBAIKAN BUG ---
        // Jika angka awal dan target sama (misal 0 ke 0), langsung tulis teksnya dan hentikan!
        if (_value == newValue)
        {
            if (Text != null) Text.SetText(newValue.ToString(NumberFormat));
            yield break; 
        }

        WaitForSeconds Wait = new WaitForSeconds(1f / CountFPS);
        int previousValue = _value;
        int stepAmount;

        if (newValue - previousValue < 0)
        {
            stepAmount = Mathf.FloorToInt((newValue - previousValue) / (CountFPS * Duration)); 
        }
        else
        {
            stepAmount = Mathf.CeilToInt((newValue - previousValue) / (CountFPS * Duration)); 
        }

        if (previousValue < newValue)
        {
            while(previousValue < newValue)
            {
                previousValue += stepAmount;
                if (previousValue > newValue) previousValue = newValue;

                if (Text != null) Text.SetText(previousValue.ToString(NumberFormat));
                yield return Wait;
            }
        }
        else
        {
            while (previousValue > newValue)
            {
                previousValue += stepAmount; 
                if (previousValue < newValue) previousValue = newValue;

                if (Text != null) Text.SetText(previousValue.ToString(NumberFormat));
                yield return Wait;
            }
        }
    }
}