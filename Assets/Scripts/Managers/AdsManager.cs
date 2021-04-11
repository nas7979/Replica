using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using BackEnd;

public class AdsManager : Singleton<AdsManager>
{
	private InterstitialAd mAd_Interstitial;
	private RewardedAd mAd_Reward;
	private string mAd_Interstitial_ID = "ca-app-pub-3940256099942544/1033173712";
	private string mAd_Reward_ID = "ca-app-pub-3940256099942544/5224354917";
	private int mAdsCount = 0;
	public int AdsCount { get => mAdsCount; set => mAdsCount = value; }
	private int mRewardAdsLimit = 0;
	public int RewardAdsLimit { get => mRewardAdsLimit; set => mRewardAdsLimit = value; }
	private int mInterstitialAdsCount = 0;
	public bool IsUserEarnedReward { get; set; }

	private void Awake()
	{
		mAd_Interstitial = new InterstitialAd(mAd_Interstitial_ID);
		mAd_Reward = new RewardedAd(mAd_Reward_ID);
		MobileAds.Initialize((InitStatus) => { });

		DontDestroyOnLoad(gameObject);

		IsUserEarnedReward = false;

		OnAdClosed(null, null);
	}

	private void Update()
	{

	}

	public void OnAdOpening(object _Sender, EventArgs _Args)
	{
		Time.timeScale = 0;
		SoundManager.Instance.PauseAll();
	}

	public void OnAdClosed(object _Sender, EventArgs _Args)
	{
		Time.timeScale = 1;
		mAd_Reward = new RewardedAd(mAd_Reward_ID);
		mAd_Reward.OnAdOpening += OnAdOpening;
		mAd_Reward.OnAdClosed += OnAdClosed;
		mAd_Reward.OnUserEarnedReward += OnUserEarnedReward;
		AdRequest Req = new AdRequest.Builder().Build();
		mAd_Reward.LoadAd(Req);
		mAd_Interstitial.Destroy();
		mAd_Interstitial = new InterstitialAd(mAd_Interstitial_ID);
		mAd_Interstitial.OnAdOpening += OnAdOpening;
		mAd_Interstitial.OnAdClosed += OnAdClosed;
		Req = new AdRequest.Builder().Build();
		mAd_Interstitial.LoadAd(Req);
	}

	public void OnUserEarnedReward(object _Sender, Reward _Args)
	{
		IsUserEarnedReward = true;
		mAdsCount++;
		Param param = new Param();
		param.Add("adsCount", mAdsCount);
		DataManager.Instance.SendDataToServerSchema("adsremove", param);
		if (mAdsCount == 20)
		{
			DataManager.Instance.AddUnlockedSkin("Item_Rare_Mario");
		}
	}

	public void ShowInterstitialAd()
	{
		Debug.Log(mAd_Interstitial.IsLoaded());
		Debug.Log(mInterstitialAdsCount);
		Debug.Log(DataManager.Instance.GetData("AdsRemove"));
		if (DataManager.Instance.GetData("AdsRemove") == "True")
			return;
		mInterstitialAdsCount++;
		if (mInterstitialAdsCount != 3)
			return;
		if (mAd_Interstitial.IsLoaded())
		{
			mInterstitialAdsCount = 2;
			mAd_Interstitial.Show();
			mAd_Interstitial.Destroy();
		}
	}

	public void ShowRewardAd()
	{
		Debug.Log(mAd_Reward.IsLoaded());
		if (DataManager.Instance.GetData("AdsRemove") == "True")
			return;
		if (mAd_Reward.IsLoaded())
		{
			mAd_Reward.Show();
			mRewardAdsLimit++;
			Param param = new Param();
			param.Add("rewardAdsLimit", mRewardAdsLimit);
			DataManager.Instance.SendDataToServerSchema("adsremove", param);
		}
	}
}
