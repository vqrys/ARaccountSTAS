using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Dotween : MonoBehaviour
{
    [SerializeField] GameObject Sprite;
    [SerializeField] RectTransform spriteRect;
    [SerializeField] float posY, posX, middlePosY, middlePosX;
    [SerializeField] float tweenDuration;

    public void Intro()
    {
        spriteRect.DOAnchorPosY(middlePosY, tweenDuration);
        spriteRect.DOAnchorPosX(middlePosX, tweenDuration);
    }

    // Update is called once per frame
    public void Outro()
    {
        spriteRect.DOAnchorPosY(posY, tweenDuration);
        spriteRect.DOAnchorPosX(posX, tweenDuration);
    }
}
