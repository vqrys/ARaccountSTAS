using UnityEngine;
using DG.Tweening;

public class SimpleUIRotate : MonoBehaviour
{
    [SerializeField] private RectTransform target;
    [SerializeField] private float rotate = 180f;
    [SerializeField] private float unRotate = 0f;
    [SerializeField] private float duration = 0.25f;

    public void RotateZ()
    {
        target.DORotate(new Vector3(0, 0, rotate), duration);
    }

    public void ResetZ()
    {
        target.DORotate(new Vector3(0, 0, unRotate), duration); 
    }
}