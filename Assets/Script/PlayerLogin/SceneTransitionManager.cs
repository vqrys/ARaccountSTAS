using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Loading UI")]
    [SerializeField] private RectTransform loadingPanel;
    [SerializeField] private CanvasGroup loadingCanvasGroup;
    [SerializeField] private Image progressBarFill;

    [Header("Settings")]
    [Tooltip("Waktu minimal loading agar Cloud Data sempat didownload di belakang layar")]
    [SerializeField] private float minLoadingTime = 2.5f; 
    [SerializeField] private float slideDuration = 0.8f;
    [SerializeField] private float fadeDuration = 0.6f;
    [SerializeField] private float slideDistance = 1600f;

    private bool _isTransitioning = false;
    private float _displayProgress = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (loadingPanel != null)
            {
                DontDestroyOnLoad(loadingPanel.gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ShowLoadingInstant();
    }

    // FUNGSI BARU: Untuk menyembunyikan loading screen saat tetap berada di LoginScene
    public void HideLoadingScreen()
    {
        if (!_isTransitioning)
        {
            StartCoroutine(PlayExitAnimation());
        }
    }

    public void LoadScene(string sceneName)
    {
        if (_isTransitioning) return;

        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        _isTransitioning = true;
        ShowLoadingInstant();

        // Mulai memuat scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        
        // PENTING: Langsung aktifkan scene!
        // Ini membuat fungsi Start() di scene baru langsung berjalan dan mulai men-download data dari Cloud
        // meskipun tertutup oleh Loading Screen.
        operation.allowSceneActivation = true; 

        float timer = 0f;
        _displayProgress = 0f;
        UpdateProgressUI(0f);

        // Loading fiktif berdasarkan waktu agar UI terlihat mulus dan memberi waktu bagi Cloud Data
        while (timer < minLoadingTime)
        {
            timer += Time.deltaTime;
            _displayProgress = Mathf.Clamp01(timer / minLoadingTime);
            UpdateProgressUI(_displayProgress);
            
            yield return null;
        }

        // Pastikan scene Unity benar-benar sudah termuat (biasanya sudah selesai sejak awal)
        while (!operation.isDone)
        {
            yield return null;
        }

        _displayProgress = 1f;
        UpdateProgressUI(1f);

        // Beri sedikit jeda ekstra (0.5 detik) untuk memastikan data Cloud selesai diproses dan UI telah ter-refresh
        yield return new WaitForSeconds(0.5f);

        yield return PlayExitAnimation();

        _isTransitioning = false;
    }

    private void ShowLoadingInstant()
    {
        if (loadingPanel == null) return;

        loadingPanel.gameObject.SetActive(true);
        loadingPanel.anchoredPosition = Vector2.zero;

        if (loadingCanvasGroup != null)
        {
            loadingCanvasGroup.alpha = 1f;
            loadingCanvasGroup.blocksRaycasts = true;
            loadingCanvasGroup.interactable = false;
        }

        UpdateProgressUI(0f);
    }

    private void UpdateProgressUI(float value)
    {
        if (progressBarFill != null)
        {
            progressBarFill.fillAmount = value;
        }
    }

    private IEnumerator PlayExitAnimation()
    {
        if (loadingPanel == null)
            yield break;

        Sequence sequence = DOTween.Sequence();

        if (loadingCanvasGroup != null)
        {
            sequence.Join(
                loadingCanvasGroup.DOFade(0f, fadeDuration)
            );
        }

        sequence.Join(
            loadingPanel.DOAnchorPosY(slideDistance, slideDuration)
                .SetEase(Ease.InOutCubic)
        );

        yield return sequence.WaitForCompletion();

        loadingPanel.gameObject.SetActive(false);

        if (loadingCanvasGroup != null)
        {
            loadingCanvasGroup.alpha = 1f;
            loadingCanvasGroup.blocksRaycasts = false;
        }
    }
}