using UnityEngine;

public class AnimatorTriggerCaller : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void SetTriggerByName(string triggerName)
    {
        if (animator == null)
        {
            Debug.LogError($"Animator tidak ditemukan di {gameObject.name}");
            return;
        }

        if (string.IsNullOrWhiteSpace(triggerName))
        {
            Debug.LogWarning($"Trigger kosong pada {gameObject.name}");
            return;
        }

        animator.SetTrigger(triggerName);
    }

    public void ResetTriggerByName(string triggerName)
    {
        if (animator == null)
        {
            Debug.LogError($"Animator tidak ditemukan di {gameObject.name}");
            return;
        }

        if (string.IsNullOrWhiteSpace(triggerName))
        {
            Debug.LogWarning($"Trigger kosong pada {gameObject.name}");
            return;
        }

        animator.ResetTrigger(triggerName);
    }

    public void SetBool(string paramName, bool value)
    {
        if (animator == null)
        {
            Debug.LogError($"Animator tidak ditemukan di {gameObject.name}");
            return;
        }

        animator.SetBool(paramName, value);
    }

    public void SetInt(string paramName, int value)
    {
        if (animator == null)
        {
            Debug.LogError($"Animator tidak ditemukan di {gameObject.name}");
            return;
        }

        animator.SetInteger(paramName, value);
    }
}