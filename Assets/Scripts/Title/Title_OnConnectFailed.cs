using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Title_OnConnectFailed : MonoBehaviour
{
	private System.Action mOnRetry = null;
	public System.Action OnRetry { set => mOnRetry = value; }
	private Vector3 mSize = Vector3.zero;

	private void Awake()
	{
		mSize = transform.localScale;
	}

	private void OnEnable()
	{
		transform.localScale = Vector3.zero;
		transform.DOScale(mSize, 0.5f);
	}

	public void Close()
	{
		transform.DOScale(0, 0.5f).OnComplete(() => { gameObject.SetActive(false); });
	}

	public void OnClickExit()
	{
		Application.Quit();
	}

	public void OnClickRetry()
	{
		mOnRetry();
		Close();
	}
}
