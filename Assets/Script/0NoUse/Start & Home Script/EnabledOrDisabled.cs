using UnityEngine;

public class ToggleGameObject : MonoBehaviour
{
    [Header("Target GameObject")]
    public GameObject target;

    public void Toggle()
    {
        if (target == null) return;

        target.SetActive(!target.activeSelf);
    }
}
