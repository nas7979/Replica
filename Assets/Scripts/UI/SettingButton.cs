using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour
{
	private bool mIsOn;
	private EnumButton mType;
	//[SerializeField] private GameObject[] mOnOffTexts = new GameObject[2];
	[SerializeField]
	private Color buttonOffColor;

	private void Start()
	{
		mIsOn = true;
		switch(mType)
		{
			case EnumButton.Button_BGM:
				if (SoundManager.Instance.GetBGMVolume() < 0.5f)
					mIsOn = false;
				break;
			case EnumButton.Button_SFX:
				if (SoundManager.Instance.GetSFXVolume() < 0.5f)
					mIsOn = false;
				break;
		}

		if (!mIsOn)
		{
			GetComponent<Image>().color = buttonOffColor;
			transform.position += new Vector3(-160 * transform.lossyScale.x, 0, 0);
		}
	}

	void Update()
    {
		if (Input.GetMouseButtonDown(0))
		{
			if (GetComponent<BoxCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
			{
				SoundManager.Instance.PlaySound("Sfx_Option");
				mIsOn = !mIsOn;
				//mOnOffTexts[System.Convert.ToInt32(mIsOn)].SetActive(false);
				//mOnOffTexts[System.Convert.ToInt32(!mIsOn)].SetActive(true);
				//GetComponent<Image>().sprite = GetComponentInParent<SettingWindow>().GetButtonSprite(mIsOn ? 0 : 1);
				//StartCoroutine(CR_Move(transform.position + new Vector3(mIsOn ? -1.7f : 1.7f, 0, 0)));
				//StartCoroutine(CR_Move(transform.position + new Vector3(mIsOn ? 1f : -1f, 0, 0)));
				StartCoroutine(CR_Move(transform.position + new Vector3(mIsOn ? 1.6f : -1.6f, 0, 0)));
				GetComponent<Image>().color = (mIsOn ? Color.white : buttonOffColor);
				switch (mType)
				{
					case EnumButton.Button_BGM:
						SoundManager.Instance.SetBGMVolume(mIsOn ? 1 : 0);
						break;

					case EnumButton.Button_SFX:
						SoundManager.Instance.SetSFXVolume(mIsOn ? 1 : 0);
						break;
				}
			}
		}
	}

	public void SetIsOn(bool _IsOn)
	{
		mIsOn = _IsOn;
	}

	public bool GetIsOn()
	{
		return mIsOn;
	}

	public void SetType(EnumButton _Type)
	{
		mType = _Type;
	}

	IEnumerator CR_Move(Vector3 _MoveTo)
	{
		while (transform.position != _MoveTo)
		{
			transform.position = Vector3.MoveTowards(transform.position, _MoveTo, 20 * Time.deltaTime);
			yield return null;
		}
	}
}
