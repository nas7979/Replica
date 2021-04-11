using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingElement : MonoBehaviour
{
	[SerializeField] private Text mPlace = null;
	[SerializeField] private Text mName = null;
	[SerializeField] private Text mScore = null;
	[SerializeField] private Image mTopPlayer = null;
	[SerializeField] private Image mTier = null;
	[SerializeField] private Image[] mAdapt = new Image[3];
	[SerializeField] private Replica_Menu mReplica = null;
	[SerializeField] private Sprite mMyRankImage = null;
	private RankingData mData = null;
	private Sprite[] mAdaptIcons = null;
	private Sprite[] mTierIcons = null;
	private Sprite[] mTopPlayerIcons = null;
	private bool mIsExtraDataSet = false;
	private string Score;
	private string Place;
	private string Name;

	private void Update()
	{
		if (mIsExtraDataSet == false && mData.ExtraData != null)
		{
			SetExtraData();
		}
	}

	public void Setting(RankingData _Data, Sprite[] _AdaptIcons, Sprite[] _TierIcons, Sprite[] _TopPlayerIcons)
	{
		mData = _Data;
		mData.Element = this;
		mAdaptIcons = _AdaptIcons;
		mTierIcons = _TierIcons;
		mTopPlayerIcons = _TopPlayerIcons;
	}

	public void SetExtraData()
	{
		mIsExtraDataSet = true;
		mAdapt[0].sprite = mData.ExtraData.MostSelAdapt[0].Count == 0 ? mAdapt[0].sprite : mAdaptIcons[(int)mData.ExtraData.MostSelAdapt[0].Type];
		mAdapt[1].sprite = mData.ExtraData.MostSelAdapt[1].Count == 0 ? mAdapt[1].sprite : mAdaptIcons[(int)mData.ExtraData.MostSelAdapt[1].Type];
		mAdapt[2].sprite = mData.ExtraData.MostSelAdapt[2].Count == 0 ? mAdapt[2].sprite : mAdaptIcons[(int)mData.ExtraData.MostSelAdapt[2].Type];
		mTier.sprite = mTierIcons[mData.ExtraData.Tier - 1];
		mReplica.SetImage(mData.ExtraData);
		if (DataManager.Instance.IsMyData(mData.InDate))
			GetComponent<Image>().sprite = mMyRankImage;

		mPlace.text = mData.Rank.ToString();
		mName.text = mData.Name;
		mScore.text = mData.Score.ToString();
		if (mData.Rank <= 3)
		{
			mTopPlayer.sprite = mTopPlayerIcons[mData.Rank - 1];
			mTopPlayer.SetNativeSize();
		}
	}
}
