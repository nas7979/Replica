using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SCORETEXT : MonoBehaviour
{
	Text mText;
	Outline mOutline;
	Color mTextOrgColor;
	Color mOutlineOrgColor;
	private void Awake()
	{
		mText = GetComponent<Text>();
		mOutline = GetComponent<Outline>();
		mTextOrgColor = mText.color;
		mOutlineOrgColor = mOutline.effectColor;
	}

	private void OnDisable()
	{
		mText.DOKill();
		mOutline.DOKill();
	}
	public void Effect(Vector3 _Pos, float _Score)
	{
		mText.text = "+" + _Score.ToString();
		mText.color = mTextOrgColor;
		mOutline.effectColor = mOutlineOrgColor;
		mOutline.DOFade(0, 1.5f);
		transform.position = _Pos;
		transform.DOMoveY(transform.position.y + 0.6f, 1f);
		mText.DOFade(0, 1.5f).OnComplete(() =>
		{
			gameObject.SetActive(false);
		});
	}
}
