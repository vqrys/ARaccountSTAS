using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //Screen object variables
    public GameObject StartingUI;
    public GameObject homePage;
    public GameObject profilePage;
    public GameObject raccoon;
    public GameObject Icons;
    public GameObject Mission;
    public GameObject DialogVN;

    // private bool isAnimatingit = false;
    public Animator homeButtonAnimator;
    
    private void Start()
    {
        if (homeButtonAnimator != null)
        {
            // isAnimatingit = true;
            homeButtonAnimator.SetTrigger("Selected");
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    //Functions to change the login screen UI
    public void StartingScreen()
    {
        StartingUI.SetActive(true);
        homePage.SetActive(false);
        profilePage.SetActive(false);
        raccoon.SetActive(false);
        Icons.SetActive(false);
    }
    public void HomePage()
    {
        StartingUI.SetActive(false);
        homePage.SetActive(true);
        profilePage.SetActive(false);
        raccoon.SetActive(true);
        Icons.SetActive(true);

        // isAnimatingit = true;
        homeButtonAnimator.SetTrigger("Selected");
    }
    public void ProfilePage()
    {   
        StartingUI.SetActive(false);
        homePage.SetActive(false);
        profilePage.SetActive(true);
        raccoon.SetActive(false);
        Icons.SetActive(true);
    }
}