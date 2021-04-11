using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingWindow : MonoBehaviour
{
	// Start is called before the first frame update
	[SerializeField]
	Text mScore;
	[SerializeField]
	Text mHighScore;
	[SerializeField]
	Text mGold;
	[SerializeField]
	Image mRankBorder;
	[SerializeField]
	Text mRankText;
	[SerializeField]
	FadeInFadeOut mFade;
	[SerializeField]
	Sprite[] mRankBorderSprite = new Sprite[4];
	private bool mIsTouchAble;
	//[SerializeField]
	//Image mReplicaSkin;
	void Start()
	{
		Replica Rep = GameManager.Instance.GetReplica().GetComponent<Replica>();
		SoundManager.Instance.StopBGM();
		SoundManager.Instance.PlaySound("Sfx_Ending");
		mIsTouchAble = false;
		StartCoroutine(CR_OpenWindow());
		GameManager.Instance.AddTotalScore(Rep.SkinEffect("OnGameOverScore", GameManager.Instance.GetTotalScore()));
		int Gold = (int)(GameManager.Instance.GetTotalScore() * 0.01f);
		Gold += (int)Rep.SkinEffect("OnGameOverGold", Gold);

		mScore.text = ((int)GameManager.Instance.GetTotalScore()).ToString();
		mHighScore.text = DataManager.Instance.HighScore.ToString();
		mGold.text = Gold.ToString();
		mRankBorder.sprite = mRankBorderSprite[GameManager.GetRound() - 1];
		DataManager.Instance.AddGold(Gold);
		RankingManager.Instance.Clear();
		//mReplicaSkin.sprite = DataManager.Instance.GetItemSprite(DataManager.Instance.GetData("Replica_Skin", "Core-idle"));
		switch (GameManager.GetRound())
		{
			case 1:
				mRankText.text = "행성 탐사원";
				break;
			case 2:
				mRankText.text = "훌륭한 탐사원";
				break;
			case 3:
				mRankText.text = "영웅적 탐사원";
				break;
			case 4:
				mRankText.text = "전설적 탐사원";
				break;
		}

        if (GameManager.Instance.GetTotalScore() > DataManager.Instance.HighScore || DataManager.Instance.HighScore == 0)
        {
			DataManager.Instance.HighScore = (int)GameManager.Instance.GetTotalScore();
            RankingManager.Instance.AddRanking();
			//최고점수 텍스트 업데이트 (작성자 강승현 | 11월 26일 5:06 )
            mHighScore.text = ((int)GameManager.Instance.GetTotalScore()).ToString();

        }
	}

	public void Quit()
	{
		if (GameManager.Instance.GetAdaptWindow().gameObject.activeInHierarchy == false && mIsTouchAble)
		{
			mFade.GetComponent<SpriteRenderer>().color = Color.black;
			mFade.FadeOut("Menu", 2);
			AdsManager.Instance.ShowInterstitialAd();
			GameManager.Clear();
		}
	}

	public void Adapt()
	{
		if (mIsTouchAble)
		{
			GameManager.Instance.GetAdaptWindow().gameObject.SetActive(true);
			SoundManager.Instance.PlaySound("Sfx_Button");
		}
	}

	public void Restart()
	{
		if (GameManager.Instance.GetAdaptWindow().gameObject.activeInHierarchy == false && mIsTouchAble)
		{
			mFade.GetComponent<SpriteRenderer>().color = Color.black;
			mFade.FadeOut("Ingame", 2);
			AdsManager.Instance.ShowInterstitialAd();
			GameManager.Clear();
		}
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
}
