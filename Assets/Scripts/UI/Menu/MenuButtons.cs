using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using GoogleMobileAds.Api;
using System;
using BackEnd;

public class MenuButtons : MonoBehaviour
{
	[SerializeField] private GameObject mRankWindow = null;
	[SerializeField] private GameObject mSettingWindow = null;
	[SerializeField] private Text mGold = null;
	[SerializeField] private Text mMileage = null;
	[SerializeField] private FadeInFadeOut mFade = null;
	[SerializeField] private GameObject mAdsRemoveButton = null;
	[SerializeField] private GameObject mErrorMessage = null;


	[SerializeField] private Animator mGateAnim = null;
	[SerializeField] private Animator mGoingAnim = null;

	[SerializeField] private bool mHasGateAnimSkip = false;
	[SerializeField] private bool mIsGateAnimPlaying = false;
	[SerializeField] private float mRocketAppearDelay = 2f;
	[SerializeField] private Canvas mReplicaCanvas = null;
	[SerializeField] private Transform mReplicaTransform = null;

	private void OnEnable()
	{
		mGold.text = DataManager.Instance.Gold.ToString();
		mMileage.text = "+" + DataManager.Instance.Mileage.ToString();

		if(!SoundManager.Instance.IsBGMPlaying())
		{
			ObjectManager.Instance.Clear();
			SoundManager.Instance.PlaySound("BGM_Menu", true, true);
		}
	}

	public void Play()
	{
		//게이트 애니메이션 실행 중이지 않을 때에만 애니메이션 실행
		if (mIsGateAnimPlaying == false)
		{
			SoundManager.Instance.PlaySound("Sfx_Button");
			StartCoroutine(GateAnimation());
			mIsGateAnimPlaying = true;
		}
		else
			mHasGateAnimSkip = true;
	}
	IEnumerator GateAnimation() {
        #region 구멍 열기
        if (mHasGateAnimSkip == false)
		{
			mGateAnim.SetTrigger("Open");
			float delay = 1.5f;
			mReplicaTransform.DOJump(mReplicaTransform.position, 4, 1, delay).SetEase(Ease.OutSine);
			//mReplicaTransform.DOMoveY(mReplicaTransform.position.y - 10, delay).SetEase(Ease.InQuart);
			mReplicaTransform.DOScale(0.01f, delay).SetEase(Ease.InQuart);
			yield return null;
			//Open Animation과 delay가 끝날때까지 기다리기
			while (!mHasGateAnimSkip && (mGateAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f || delay > 0))
			{
				delay -= Time.deltaTime;
				yield return null;
			}
		}
        #endregion
        #region 구멍 닫기
        if (mHasGateAnimSkip == false)
		{
			mGateAnim.SetTrigger("Close");
			mReplicaCanvas.sortingOrder = -1;
			yield return null;
			//Close Animation 끝날때까지 기다리기
			while (!mHasGateAnimSkip && mGateAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
			{
				yield return null;
			}
		}
		#endregion
		#region 로켓 등장
		if (mHasGateAnimSkip == false)
		{
			float delay = mRocketAppearDelay;
			while (!mHasGateAnimSkip && delay > 0)
			{
			    delay -= Time.deltaTime;
			    yield return null;
			}
			mGoingAnim.SetTrigger("Going");
			yield return null;
			//우주선 Going Animation 끝날때까지 기다리기
			while (!mHasGateAnimSkip && mGoingAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
			{
				yield return null;
			}
		}
        #endregion

		mFade.FadeOut("Ingame", 0.7f);
		mIsGateAnimPlaying = false;
	}
	public void Shop()
	{
		SoundManager.Instance.PlaySound("Sfx_Button");
		mFade.FadeOut("Shop", 0.4f);
	}

	public void Gacha()
	{
		SoundManager.Instance.PlaySound("Sfx_Button");
		mFade.FadeOut("Gacha", 0.4f);
	}

	public void Ranking()
	{
		mRankWindow.SetActive(true);
	}

	public void Setting()
	{
		mSettingWindow.SetActive(true);
	}

	public void DisplayErrorMessage(string _Desc)
	{
		mErrorMessage.SetActive(true);
		mErrorMessage.transform.GetChild(1).GetComponent<Text>().text = _Desc;
	}
}
