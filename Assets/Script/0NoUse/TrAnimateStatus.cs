using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Import UnityEngine.UI untuk penggunaan Button

public class TrAnimateStatus : MonoBehaviour
{
    private Animator mAnimator;
    // public Button trDownButton;  // Tombol untuk TrDown
    // public Button trUpButton;    // Tombol untuk TrUp

    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    // Fungsi untuk memicu TrDown
    public void TriggerTrDown()
    {
        mAnimator.SetTrigger("TrDown");
    }

    // Fungsi untuk memicu TrUp
    public void TriggerTrUp()
    {
        mAnimator.SetTrigger("TrUp");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
