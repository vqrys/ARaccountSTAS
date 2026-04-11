using UnityEngine;
using DG.Tweening;

public class ScrollingSystemDOTween : MonoBehaviour
{
    [Header("Slides Layout")]
    [Tooltip("Jarak antar Quest")]
    [SerializeField] private float slideSpacing = 2.5f;
    [SerializeField] private float startOffset = 0f;

    [Header("Scale")]
    [SerializeField] private float focusedScale = 1f;
    [SerializeField] private float unfocusedScale = 0.65f;

    [Header("Snap")]
    [SerializeField] private float snapDuration = 0.35f;
    [SerializeField] private Ease snapEase = Ease.OutCubic;

    [Header("Drag")]
    [SerializeField] private float dragSensitivity = 1f;
    [SerializeField] private bool allowMouse = true;
    [SerializeField] private bool allowTouch = true;

    [Header("Visual")]
    [SerializeField] private float maxVisibleDistance = 6f;
    [SerializeField] private float alphaPower = 1f;

    [Header("Optional")]
    [SerializeField] private bool useWorldSpace = true;

    private Transform[] slides;
    private Vector3[] originalScales;

    private Camera mainCamera;
    private Tween snapTween;

    private bool isDragging;
    private Vector3 lastPointerWorldPos;
    private float dragVelocity;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("ScrollingSystemDOTween: Main Camera not found.");
            enabled = false;
            return;
        }

        // Tidak lagi menduplikasi (Instantiate) Prefab!
        // Kita langsung mengambil objek Quest yang sudah dimasukkan sebagai Child di Scene.
        FetchExistingSlides();
    }

    private void Update()
    {
        HandleInput();
        UpdateSlideVisuals();
    }

    private void FetchExistingSlides()
    {
        int count = transform.childCount;
        if (count == 0) return;

        slides = new Transform[count];
        originalScales = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            Transform t = transform.GetChild(i);
            
            // Posisikan secara otomatis
            Vector3 pos = new Vector3(startOffset + (i * slideSpacing), 0f, 0f);

            if (useWorldSpace)
                t.position = transform.position + pos;
            else
                t.localPosition = pos;

            slides[i] = t;
            originalScales[i] = t.localScale;
        }
    }

    private void HandleInput()
    {
        if (TryGetPointerDown(out Vector3 pointerDown))
        {
            isDragging = true;
            dragVelocity = 0f;
            lastPointerWorldPos = pointerDown;

            if (snapTween != null && snapTween.IsActive())
                snapTween.Kill();
        }

        if (isDragging && TryGetPointerHeld(out Vector3 pointerHeld))
        {
            float deltaX = (pointerHeld.x - lastPointerWorldPos.x) * dragSensitivity;
            MoveSlides(deltaX);

            dragVelocity = deltaX / Mathf.Max(Time.deltaTime, 0.0001f);
            lastPointerWorldPos = pointerHeld;
        }

        if (isDragging && TryGetPointerUp())
        {
            isDragging = false;
            SnapToClosestSlide();
        }
    }

    private void MoveSlides(float deltaX)
    {
        for (int i = 0; i < slides.Length; i++)
        {
            if (useWorldSpace)
                slides[i].position += new Vector3(deltaX, 0f, 0f);
            else
                slides[i].localPosition += new Vector3(deltaX, 0f, 0f);
        }
    }

    private void SnapToClosestSlide()
    {
        if (slides == null || slides.Length == 0) return;

        int closestIndex = GetClosestSlideIndex();
        float closestX = GetSlideX(slides[closestIndex]);
        float shiftAmount = -closestX;

        if (Mathf.Abs(shiftAmount) < 0.001f)
            return;

        if (snapTween != null && snapTween.IsActive())
            snapTween.Kill();

        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < slides.Length; i++)
        {
            Transform slide = slides[i];

            if (useWorldSpace)
            {
                Vector3 targetPos = slide.position + new Vector3(shiftAmount, 0f, 0f);
                seq.Join(slide.DOMoveX(targetPos.x, snapDuration).SetEase(snapEase));
            }
            else
            {
                Vector3 targetLocalPos = slide.localPosition + new Vector3(shiftAmount, 0f, 0f);
                seq.Join(slide.DOLocalMoveX(targetLocalPos.x, snapDuration).SetEase(snapEase));
            }
        }

        snapTween = seq;
    }

    private int GetClosestSlideIndex()
    {
        int closestIndex = 0;
        float closestDistance = Mathf.Abs(GetSlideX(slides[0]));

        for (int i = 1; i < slides.Length; i++)
        {
            float distance = Mathf.Abs(GetSlideX(slides[i]));
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    private float GetSlideX(Transform slide)
    {
        return useWorldSpace ? slide.position.x : slide.localPosition.x;
    }

    private void UpdateSlideVisuals()
    {
        if (slides == null) return;

        for (int i = 0; i < slides.Length; i++)
        {
            Transform slide = slides[i];
            float distance = Mathf.Abs(GetSlideX(slide));

            float t = Mathf.Clamp01(distance / maxVisibleDistance);

            float scaleMultiplier = Mathf.Lerp(focusedScale, unfocusedScale, t);
            slide.localScale = originalScales[i] * scaleMultiplier;

            SpriteRenderer sr = slide.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = Mathf.Pow(1f - t, alphaPower);
                sr.color = c;
            }
        }
    }

    private bool TryGetPointerDown(out Vector3 worldPos)
    {
        worldPos = Vector3.zero;

        if (allowTouch && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            worldPos = ScreenToWorld(Input.GetTouch(0).position);
            return true;
        }

        if (allowMouse && Input.GetMouseButtonDown(0))
        {
            worldPos = ScreenToWorld(Input.mousePosition);
            return true;
        }

        return false;
    }

    private bool TryGetPointerHeld(out Vector3 worldPos)
    {
        worldPos = Vector3.zero;

        if (allowTouch && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                worldPos = ScreenToWorld(touch.position);
                return true;
            }
        }

        if (allowMouse && Input.GetMouseButton(0))
        {
            worldPos = ScreenToWorld(Input.mousePosition);
            return true;
        }

        return false;
    }

    private bool TryGetPointerUp()
    {
        if (allowTouch && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                return true;
        }

        if (allowMouse && Input.GetMouseButtonUp(0))
            return true;

        return false;
    }

    private Vector3 ScreenToWorld(Vector3 screenPosition)
    {
        screenPosition.z = Mathf.Abs(mainCamera.transform.position.z);
        Vector3 world = mainCamera.ScreenToWorldPoint(screenPosition);
        world.z = 0f;
        return world;
    }

    private void OnDestroy()
    {
        if (snapTween != null && snapTween.IsActive())
            snapTween.Kill();
    }
}