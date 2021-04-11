using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class COMBOTEXT : MonoBehaviour
{
    [SerializeField]
    Text mText;
    Outline mOutline;
    RectTransform mRectTransform;
    Color mTextOrgColor;
    Color mOutlineOrgColor;
    private void Awake()
    {
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
    public void Effect(float _COMBO)
    {
        mText.text = _COMBO.ToString() + "COMBO!";
        mText.color = mTextOrgColor;
        mOutline.effectColor = mOutlineOrgColor;
        //mRectTransform.position = _Pos;
        Vector3 originPos = mRectTransform.position;
        mRectTransform.DOMoveY(mRectTransform.position.y + 2f, 5f);
        mOutline.DOFade(0, 5f);
        mText.DOFade(0, 5f).OnComplete(()=>
        {
            mRectTransform.position = originPos;
            gameObject.SetActive(false);
        });
    }
}
