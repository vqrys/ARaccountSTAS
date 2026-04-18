using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AnswerOptionButton : MonoBehaviour
{
    [Header("UI")]
    public Button button;
    public Image background;
    public TMP_Text label;

    [Header("Visuals")]
    public Sprite normalSprite;
    public Sprite selectedSprite;
    public Sprite wrongSprite;
    public Sprite correctSprite;

    public Color normalTextColor = Color.white;
    public Color selectedTextColor = Color.black;
    public Color wrongTextColor = Color.white;
    public Color correctTextColor = Color.white;

    int index;
    System.Action<int> onSelected;

    Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Init(int optionIndex, string text, System.Action<int> callback)
    {
        index = optionIndex;
        onSelected = callback;

        label.text = text;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);

        SetNormal();
    }

    void OnClick()
    {
        StopAllCoroutines();
        StartCoroutine(PopScale());

        onSelected?.Invoke(index);
    }

    public void SetNormal()
    {
        background.sprite = normalSprite;
        label.color = normalTextColor;
    }

    public void SetSelected()
    {
        Debug.Log("SetSelected dipanggil di " + gameObject.name);

        background.sprite = selectedSprite;
        label.color = selectedTextColor;

        // Tambahkan animasi pop saat dipilih
        StopAllCoroutines();
        StartCoroutine(PopScale());
    }

    
    public void SetWrong()
    {
        background.sprite = wrongSprite;
        label.color = wrongTextColor;
        StopAllCoroutines();
        StartCoroutine(PopScale());
    }

    public void SetCorrect()
    {
        background.sprite = correctSprite;
        label.color = correctTextColor;
        StopAllCoroutines();
        StartCoroutine(PopScale());
    }

    public void SetInteractable(bool value)
    {
        button.interactable = value;
    }

    IEnumerator PopScale()
    {
        float duration = 0.1f;
        float elapsed = 0f;

        Vector3 start = originalScale;
        Vector3 target = originalScale * 1.1f;

        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = target;

        elapsed = 0f;

        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(target, start, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = start;
    }
}