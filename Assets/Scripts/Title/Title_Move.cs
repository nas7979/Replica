using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Title_Move : MonoBehaviour
{
    void Start()
    {
        transform.position = new Vector3(0, 12);
        transform.DOMove(new Vector3(0, 5.5f), 1f).SetEase(Ease.OutBounce);
    }

    private void OnEnable()
    {
        transform.position = new Vector3(0, 12);
        transform.DOMove(new Vector3(0, 5.5f), 1f).SetEase(Ease.OutBounce);
    }
}
