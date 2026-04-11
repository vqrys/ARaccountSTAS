using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class ARViewSwitcher : MonoBehaviour
{
    [Header("Vuforia")]
    public VuforiaBehaviour vuforia;
    public ObserverBehaviour[] observers;

    [Header("UI Background")]
    public GameObject backgroundRoot;
    public Button toggleButton;

    [Header("Optional")]
    public GameObject arContentRoot;

    [Header("Raycaster")]
    public ARLookRaycaster raycaster; // drag dari inspector

    bool arEnabled = true;

    void Awake()
    {
        if (vuforia == null) vuforia = FindObjectOfType<VuforiaBehaviour>();

        if (observers == null || observers.Length == 0)
            observers = FindObjectsOfType<ObserverBehaviour>(true);

        if (toggleButton != null)
        {
            toggleButton.onClick.RemoveAllListeners();
            toggleButton.onClick.AddListener(Toggle);
        }

        if (backgroundRoot != null) backgroundRoot.SetActive(false);
    }

    public void Toggle()
    {
        if (arEnabled) TurnOffARShowBackground();
        else TurnOnARHideBackground();
    }

    public void TurnOffARShowBackground()
    {
        arEnabled = false;

        if (raycaster != null) raycaster.enabled = false;

        if (arContentRoot != null) arContentRoot.SetActive(false);

        for (int i = 0; i < observers.Length; i++)
        {
            if (observers[i] != null) observers[i].enabled = false;
        }

        if (backgroundRoot != null) backgroundRoot.SetActive(true);
    }

    public void TurnOnARHideBackground()
    {
        arEnabled = true;

        for (int i = 0; i < observers.Length; i++)
        {
            if (observers[i] != null) observers[i].enabled = true;
        }

        if (arContentRoot != null) arContentRoot.SetActive(true);

        if (backgroundRoot != null) backgroundRoot.SetActive(false);

        if (raycaster != null) raycaster.enabled = true;
    }
}