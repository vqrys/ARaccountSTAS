using UnityEngine;

public class ARLookRaycaster : MonoBehaviour
{
    public float rayDistance = 5f;
    public LayerMask targetLayer;

    private ARLookable currentLooked;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, targetLayer))
        {
            ARLookable lookable = hit.collider.GetComponent<ARLookable>();

            if (lookable != null && lookable != currentLooked)
            {
                if (currentLooked != null)
                    currentLooked.OnLookExit();

                currentLooked = lookable;
                currentLooked.OnLookEnter();
            }
        }
        else
        {
            if (currentLooked != null)
            {
                currentLooked.OnLookExit();
                currentLooked = null;
            }
        }
    }

    void OnDisable()
    {
        // biar tidak ada state "nyangkut" ketika dimatikan
        if (currentLooked != null)
        {
            currentLooked.OnLookExit();
            currentLooked = null;
        }
    }
}