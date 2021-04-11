using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 오브젝트 매니저 없이 간단한 알림을 사용하고 싶을 때 쓰는 클래스
/// </summary>
public class NOTICETEXT : MonoBehaviour
{
    [InspectorName("페이드, 텍스트 움직임 연출 시간")]
    public float mDuration = 3f;
	[InspectorName("이동 거리")]
	public float mDistance = 2f;

    [SerializeField]
    Text mText;
    Outline mOutline;
    RectTransform mRectTransform;
    Color mTextOrgColor;
    Color mOutlineOrgColor;

    bool mIsEffected = false;

    Vector3 originPos;

    private void Awake()
    {
        mOutline = GetComponent<Outline>();
        mRectTransform = GetComponent<RectTransform>();
        mTextOrgColor = mText.color;
        mOutlineOrgColor = mOutline.effectColor;

        originPos = mRectTransform.position;
    }

    private void OnDisable()
    {
        mText.DOKill();
        mOutline.DOKill();
        mRectTransform.DOKill();
        mIsEffected = false;
    }

    public void SetText(string newText)
    {
        mText.text = newText;
    }

    public void Effect()
    {
        if (mIsEffected == false)
        {
            mIsEffected = true;
            mText.color = mTextOrgColor;
            mOutline.effectColor = mOutlineOrgColor;

            //텍스트를 부드럽게 위로 이동시킨다
            //Vector3 originPos = mRectTransform.position;
            mRectTransform.DOMoveY(mRectTransform.position.y + mDistance, mDuration);
            mOutline.DOFade(0, mDuration);
            mText.DOFade(0, mDuration).OnComplete(() =>
            {
                mRectTransform.position = originPos;
                gameObject.SetActive(false);
            });
        }
        else
        {
            mRectTransform.position = originPos;

            mText.DOKill();
            mOutline.DOKill();
            mRectTransform.DOKill();
            mIsEffected = false;

            Effect();
        }
    }

}
