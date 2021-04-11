using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TextCreateAnimation : MonoBehaviour
{
    [SerializeField]
    private Ease mEase;
    private RectTransform mRectTransform;
    private void Awake()
    {
        mRectTransform = GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        CreateAnimation(mRectTransform.localScale);
    }

    public virtual void CreateAnimation(Vector3 _TargetScale)
    {
        mRectTransform.localScale = new Vector3(0.1f, 0.1f, 1) * _TargetScale.x;
        mRectTransform.DOScale(_TargetScale, 0.3f).SetEase(mEase);
    }
}
