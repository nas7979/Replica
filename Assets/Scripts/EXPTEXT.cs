using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EXPTEXT : MonoBehaviour
{
    Text mText;
    Outline mOutline;
    RectTransform mRectTransform;
    Color mTextOrgColor;
    Color mOutlineOrgColor;
    private void Awake()
    {
        mText = GetComponent<Text>();
        mOutline = GetComponent<Outline>();
        mRectTransform = GetComponent<RectTransform>();
        mTextOrgColor = mText.color;
        mOutlineOrgColor = mOutline.effectColor;
    }

    private void OnDisable()
    {
        mText.DOKill();
        mOutline.DOKill();
        mRectTransform.DOKill();
    }
    public void Effect(Vector3 _Pos, float _EXP)
    {
        mText.text = "+" + _EXP.ToString();
        mText.color = mTextOrgColor;
        mOutline.effectColor = mOutlineOrgColor;
        mRectTransform.position = _Pos;
        mRectTransform.DOMoveY(mRectTransform.position.y + 0.6f, 1f);
        mOutline.DOFade(0, 1.5f);
        mText.DOFade(0, 1.5f).OnComplete(()=>
        {
            gameObject.SetActive(false);
        });
    }
}
