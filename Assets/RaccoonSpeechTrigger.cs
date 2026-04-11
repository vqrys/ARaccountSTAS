using UnityEngine;

public class RaccoonSpeechTrigger : MonoBehaviour
{
    private Animator mAnimator;

    void Awake()
    {
        mAnimator = GetComponent<Animator>();
        if (mAnimator == null)
            Debug.LogError("Animator tidak ditemukan di object ini.");
    }

    public void TrRaccoonSpeech()
    {
        if (mAnimator != null) mAnimator.SetTrigger("TrRaccoon");
    }

    public void TrRaccoonNormal()
    {
        if (mAnimator != null) mAnimator.SetTrigger("TrRNormal");
    }
}