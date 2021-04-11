using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 자연스러운 윈도우 Open과 Close 이벤트를 위해 쓰이는 스크립트
/// </summary>
public class WindowOpenClose : MonoBehaviour
{
	private bool mIsTouchAble;
	[SerializeField]
	FadeInFadeOut mFade;
	private void OnEnable()
	{
		mIsTouchAble = false;
		StartCoroutine(CR_OpenWindow());
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (!GetComponent<BoxCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)) && mIsTouchAble)
			{
				StartCoroutine(CR_CloseWindow());
				mIsTouchAble = false;
			}
		}
	}
	public void CloseWindow()
	{
		StartCoroutine(CR_CloseWindow());
		mIsTouchAble = false;
		SoundManager.Instance.PlaySound("Sfx_Button");
	}
	IEnumerator CR_OpenWindow()
	{
		transform.localScale = new Vector3(0.1f, 0.1f, 1);
		while (transform.localScale.x < 1)
		{
			transform.localScale += new Vector3(5f, 5f, 0) * Time.deltaTime;
			yield return null;
		}
		transform.localScale = new Vector3(1, 1, 1);
		mIsTouchAble = true;
	}

	IEnumerator CR_CloseWindow()
	{
		while (transform.localScale.x > 0.1)
		{
			transform.localScale -= new Vector3(5f, 5f, 0) * Time.deltaTime;
			yield return null;
		}
		gameObject.SetActive(false);
		yield break;
	}

}
