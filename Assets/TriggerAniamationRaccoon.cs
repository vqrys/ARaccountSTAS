using UnityEngine;

public class TriggerAniamationRaccoon : MonoBehaviour
{
    private Animator mAnimator;

    void Awake()
    {
        mAnimator = GetComponent<Animator>();
        if (mAnimator == null)
            Debug.LogError("Animator tidak ditemukan di object ini.");
    }

    public void TrQuestoneRaccoon()
    {
        if (mAnimator != null) mAnimator.SetTrigger("TrQ1RAnimate");
    }

    public void TrQuestoneNormal()
    {
        if (mAnimator != null) mAnimator.SetTrigger("TrQ1RNormal");
    }
}