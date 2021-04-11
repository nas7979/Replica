using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using BackEnd;

public enum EnumButton
{
	Button_SFX,
	Button_BGM
}

public class SettingWindow : MonoBehaviour
{
	private bool mIsTouchAble;
	[SerializeField]
	private GameObject[] mButtons = new GameObject[2];
	[SerializeField]
	private Sprite[] mButtonSprites = new Sprite[2];
	[SerializeField]
	FadeInFadeOut mFade;
	
	private void Awake()
	{
		mButtons[0].GetComponent<SettingButton>().SetType(EnumButton.Button_BGM);
		mButtons[1].GetComponent<SettingButton>().SetType(EnumButton.Button_SFX);
    }

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

	public void OnClickLogOut()
	{
		SceneManager.LoadScene("Title");
		DataManager.Instance.Clear();
		RankingManager.Instance.Clear();
		SoundManager.Instance.StopBGM();
		Backend.BMember.Logout();
		DestroyImmediate(ObjectManager.Instance.gameObject);
		DestroyImmediate(SoundManager.Instance.gameObject);
	}

	public Sprite GetButtonSprite(int _Index)
	{
		return mButtonSprites[_Index];
	}

	public void Quit()
	{
		if (GameManager.Instance.GetAdaptWindow().gameObject.activeInHierarchy == false && mIsTouchAble)
		{
			mFade.GetComponent<SpriteRenderer>().color = Color.black;
			mFade.FadeOut("Menu", 2);
			SoundManager.Instance.StopBGM();
			GameManager.Clear();
		}
	}

	public void Restart()
	{
		if (GameManager.Instance.GetAdaptWindow().gameObject.activeInHierarchy == false && mIsTouchAble)
		{
			mFade.GetComponent<SpriteRenderer>().color = Color.black;
			mFade.FadeOut("Ingame", 2);
			SoundManager.Instance.StopBGM();
			GameManager.Clear();
		}
	}

	public void CloseWindow() {
		StartCoroutine(CR_CloseWindow());
		mIsTouchAble = false;
	}
	IEnumerator CR_OpenWindow()
	{
		SoundManager.Instance.PlaySound("Sfx_Button");
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
		if (SceneManager.GetActiveScene().name == "Ingame")
		{
			if (GameManager.Instance.GetInOnAdaptSelect() == false)
			{
				GameManager.Instance.SetSlideAble(true);
			}
			GameManager.Instance.GetSettingButton().GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 0), 0.5f);
		}
        yield break;
	}
}
