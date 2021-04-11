using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialManager : Singleton<TutorialManager>
{
	private bool mIsFirstPlay;
	private int mCurrentStep;
	private int mProgression;
	private Text mText;
	private GameObject mStar;
	[SerializeField]
	private Sprite[] mStarSprite = new Sprite[2];
	[SerializeField]
	private FadeInFadeOut mFade = null;

	// Start is called before the first frame update
	void Start()
    {
		mIsFirstPlay = DataManager.Instance.GetData("IsFirstPlay") == "True";
		if (mIsFirstPlay)
		{
			mIsFirstPlay = false;
			DataManager.Instance.SetData("IsFirstPlay", "False");
			mCurrentStep = 1;
			mProgression = 0;
			transform.localScale.Set(0, 0, 1);
			StartCoroutine(CR_ShowTutorial());
			mText = transform.GetChild(0).GetComponent<Text>();
			mStar = mText.transform.GetChild(0).gameObject;
		}
		else
		{
			mCurrentStep = 0;
			gameObject.SetActive(false);
		}
	}

	private void Update()
	{

	}

	public void AddProgression(int _Step)
	{
		if(mCurrentStep == _Step)
		{
			mProgression++;
			switch (mCurrentStep)
			{
				case 1:
					mText.text = "화면을 슬라이드해서 열매를 획득하세요(" + mProgression + "/4)";
					break;
				case 3:
					mText.text = "레벨이 더 낮은 포식자를 처치하세요(" + mProgression + "/ 1)";
					break;
			}
		}
	}


	public int GetCurrentStep()
	{
		return mCurrentStep;
	}

    public IEnumerator CR_ShowTutorial()
	{
		gameObject.transform.localPosition = new Vector2(10000, 10000);
		yield return new WaitForSeconds(mCurrentStep == 1 ? 5f : 0.5f);
		gameObject.transform.localPosition = new Vector2(0, 468);

		switch (mCurrentStep)
		{
			case 1:
				mText.text = "화면을 슬라이드해서 열매를 획득하세요(0/4)";
				break;
			case 2:
				GameManager.Instance.SetPredatorLevel(1);
				mText.text = "당신의 적응을 선택하세요";
				break;
			case 3:
				mText.text = "레벨이 더 낮은 포식자를 처치하세요(0/1)";
				break;
			case 4:
				mText.text = "이제, 당신의 레플리카를 성장시키세요!";
				GameManager.Instance.SetPredatorLevel(3);
				mStar.SetActive(false);
				mText.transform.position = new Vector3(0, -9, 0);
				break;
		}

        mStar.GetComponent<Animator>().Play("Clear", -1, 0f);
		mStar.GetComponent<Animator>().speed = 0;
		while (transform.localScale.x <= 1.2f)
		{
			transform.localScale += new Vector3(5, 5, 0) * Time.deltaTime;
			yield return null;
		}
		while (transform.localScale.x >= 1f)
		{
			transform.localScale = Vector3.LerpUnclamped(transform.localScale, new Vector3(0.9f, 0.9f, 1f), 10 * Time.deltaTime);
			yield return null;
		}
		transform.localScale = new Vector3(1, 1, 1);

		if (mCurrentStep != 4)
		{
			int Goal = 0;
			switch (mCurrentStep)
			{
				case 2:
				case 3:
					Goal = 1;
					break;
				case 1:
					Goal = 4;
					break;
			}
			while (mProgression != Goal)
			{
				yield return null;
			}
		}
		else
		{
			yield return new WaitForSeconds(3f);
			gameObject.GetComponent<Image>().DOFade(0, 0.5f);
			mText.DOFade(0, 0.5f);
			mStar.GetComponent<Image>().DOFade(0, 0.5f).OnComplete(()=>
			{
				gameObject.SetActive(false);
			});
			yield break;
		}

		mStar.GetComponent<Animator>().speed = 1;
		mCurrentStep++;
		mProgression = 0;
		SoundManager.Instance.PlaySound("Sfx_Tutorial");
		while (transform.localScale.x <= 1.27f)
		{
			transform.localScale = Vector3.LerpUnclamped(transform.localScale, new Vector3(1.3f, 1.3f, 1f), 4 * Time.deltaTime);
			yield return null;
		}
		while (transform.localScale.x >= 0f)
		{
			transform.localScale -= new Vector3(4, 4, 0) * Time.deltaTime;
			yield return null;
		}
		transform.localScale = new Vector3(0, 0, 1);

		if(mCurrentStep != 5)
		{
			yield return new WaitForSeconds(0.5f);
			StartCoroutine(CR_ShowTutorial());
		}
	}

	public bool GetIsFirstPlay()
	{
		return mIsFirstPlay;
	}
}
