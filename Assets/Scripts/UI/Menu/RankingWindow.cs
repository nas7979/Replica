using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using BackEnd;

public class RankingWindow : MonoBehaviour
{
	private bool mIsTouchAble;
	[SerializeField] private Sprite[] mAdaptIcons = new Sprite[(int)EnumAdapts.End];
	[SerializeField] private Sprite[] mTierIcons = new Sprite[4];
	[SerializeField] private Sprite[] mTopPlayerIcons = new Sprite[3];
	[SerializeField] private GameObject mWindow = null;
	[SerializeField] private RankingElement mMyRankingElement = null;
	[SerializeField] private GameObject mAllRankingElements = null;
	[SerializeField] private GameObject mMyRankingElements = null;
	[SerializeField] private GameObject mRankingElementPrefab = null;
	[SerializeField] private GameObject mTopPlayerPrefab = null;
	[SerializeField] private GameObject mTopPlayers = null;
	[SerializeField] private Text mMyPlaceText = null;
	[SerializeField] private Text mMyNameText = null;
	private Scrollbar mScrollbar = null;
	private LinkedList<RankingData> mCurRanking = null;
	private GameObject mCurRankingElements = null;
	private bool mIsLoading = false;
	private float mMySiblingIndex = 0;
	private int mOpenMyRankingFirstTime = 0;

	private const int mDataPerScroll = 8;

	private void Start()
	{
		mCurRanking = RankingManager.Instance.AllRanking;
		mCurRankingElements = mAllRankingElements;
		mScrollbar = mCurRankingElements.transform.parent.parent.GetChild(1).GetComponent<Scrollbar>();
		if (mCurRanking.Count == 0)
		{
			RankingManager.Instance.UpdateRanking(RankingManager.Instance.AllRanking, 0, mDataPerScroll, () =>
			{
				CreateElements(mAllRankingElements, RankingManager.Instance.AllRanking);

				LinkedListNode<RankingData> iter = RankingManager.Instance.AllRanking.First;
				for (int i = 0; i < Mathf.Min(3, RankingManager.Instance.AllRanking.Count); i++)
				{
					Instantiate(mTopPlayerPrefab, mTopPlayers.transform).GetComponent<Replica_TopPlayer>().Setting(iter.Value);
					iter = iter.Next;
				}
			});
		}
		else if(mCurRankingElements.transform.childCount == 0)
		{
			CreateElements(mCurRankingElements, mCurRanking);
			LinkedListNode<RankingData> iter = RankingManager.Instance.AllRanking.First;
			if (mTopPlayers)
			{
				for (int i = 0; i < Mathf.Min(3, RankingManager.Instance.AllRanking.Count); i++)
				{
					Instantiate(mTopPlayerPrefab, mTopPlayers.transform).GetComponent<Replica_TopPlayer>().Setting(iter.Value);
					iter = iter.Next;
				}
			}
		}

		if (mMyNameText)
		{
			mMyNameText.text = RankingManager.Instance.MyRankingData.Name;
			mMyPlaceText.text = RankingManager.Instance.MyRankingData.Rank.ToString() + "위";
		}
		RankingManager.Instance.GetMyRankingData(() =>
		{
			mMyRankingElement.Setting(RankingManager.Instance.MyRankingData, mAdaptIcons, mTierIcons, mTopPlayerIcons);
			if (mMyNameText)
			{
				mMyNameText.text = RankingManager.Instance.MyRankingData.Name;
				mMyPlaceText.text = RankingManager.Instance.MyRankingData.Rank.ToString() + "위";
			}
		});
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

		if(mOpenMyRankingFirstTime != 0)
		{
			mOpenMyRankingFirstTime++;
			if (mOpenMyRankingFirstTime == 4)
			{
				mOpenMyRankingFirstTime = 0;
				CenterElement(mMySiblingIndex);
			}
		}

		if (mWindow.activeInHierarchy && !mIsLoading && mOpenMyRankingFirstTime == 0)//Up
		{
			if (mScrollbar.value > 1 - (1f / mCurRankingElements.transform.childCount * mDataPerScroll * 0.2f))
			{
				int Highest = mCurRanking.First.Value.Rank;
				if (Highest == 1)
					return;
				mIsLoading = true;
				int From = Mathf.Clamp(Highest - 1 - mDataPerScroll, 0, 10000000);
				int Count = Mathf.Clamp(Highest - 1, 0, mDataPerScroll);
				GameObject Temp;
				LinkedListNode<RankingData> iter = mCurRanking.First;
				GameObject Element = mCurRankingElements;
				RankingManager.Instance.UpdateRanking(mCurRanking, From, Count, () =>
				{
					if (mScrollbar.value > 1)
						mScrollbar.value = 1;
					for (int i = 0; i < Count; i++)
					{
						iter = iter.Previous;
						Temp = Instantiate(mRankingElementPrefab, Element.transform);
						Temp.GetComponent<RankingElement>().Setting(iter.Value, mAdaptIcons, mTierIcons, mTopPlayerIcons);
						Temp.transform.SetSiblingIndex(0);
					}
					mIsLoading = false;
					if (Element == mMyRankingElements)
						mMySiblingIndex += Count;

					Debug.Log(mScrollbar.value);
					float Size = Element.transform.childCount;
					StartCoroutine(CR_SetScrollPosition(Count));
				});
			}
			else if (mScrollbar.value < (1f / mCurRankingElements.transform.childCount * mDataPerScroll * 0.2f))//Down
			{
				int Lowest = mCurRanking.First.Value.Rank + mCurRankingElements.transform.childCount - 1;
				if (Lowest == RankingManager.Instance.mRankingTotalCount)
					return;
				mIsLoading = true;
				int From = Mathf.Clamp(Lowest, 0, 10000000);
				int Count = Mathf.Clamp(mDataPerScroll, 0, RankingManager.Instance.mRankingTotalCount - Lowest);
				GameObject Temp;
				LinkedListNode<RankingData> iter = mCurRanking.Last;
				GameObject Element = mCurRankingElements;
				RankingManager.Instance.UpdateRanking(mCurRanking, From, Count, () =>
				{
					if (mScrollbar.value < 0)
						mScrollbar.value = 0;
					for (int i = 0; i < Count; i++)
					{
						iter = iter.Next;
						Temp = Instantiate(mRankingElementPrefab, Element.transform);
						Temp.GetComponent<RankingElement>().Setting(iter.Value, mAdaptIcons, mTierIcons, mTopPlayerIcons);
					}
					mIsLoading = false;
				});
			}
		}
	}

	public void OnScroll()
	{

	}

	public Sprite GetAdaptIcon(EnumAdapts _Type)
	{
		return mAdaptIcons[(int)_Type];
	}

	public void OnClickAllRanking()
	{
		SoundManager.Instance.PlaySound("Sfx_Button");
		mCurRanking = RankingManager.Instance.AllRanking;
		mCurRankingElements.transform.parent.parent.gameObject.SetActive(false);
		mCurRankingElements = mAllRankingElements;
		mCurRankingElements.transform.parent.parent.gameObject.SetActive(true);
		mScrollbar = mCurRankingElements.transform.parent.parent.GetChild(1).GetComponent<Scrollbar>();
		mScrollbar.value = 1;
	}

	public void OnClickMyRanking()
	{
		SoundManager.Instance.PlaySound("Sfx_Button");
		mCurRanking = RankingManager.Instance.MyRanking;
		mMyRankingElements.transform.parent.parent.gameObject.SetActive(false);
		mCurRankingElements = mMyRankingElements;
		mMyRankingElements.transform.parent.parent.gameObject.SetActive(true);
		mScrollbar = mMyRankingElements.transform.parent.parent.GetChild(1).GetComponent<Scrollbar>();
		int MyRank = RankingManager.Instance.MyRankingData.Rank;
		if (RankingManager.Instance.MyRanking.Count == 0)
		{
			mIsLoading = true;
			RankingManager.Instance.UpdateRanking(RankingManager.Instance.MyRanking, Mathf.Clamp(MyRank - 1 - mDataPerScroll, 0, 1000000), mDataPerScroll * 2 - Mathf.Clamp(mDataPerScroll - (MyRank - 1),0,1000000) + 1, () =>
			{
				GameObject Temp;
				int i = 0;
				foreach (var iter in RankingManager.Instance.MyRanking)
				{
					Temp = Instantiate(mRankingElementPrefab, mMyRankingElements.transform);
					Temp.GetComponent<RankingElement>().Setting(iter, mAdaptIcons, mTierIcons, mTopPlayerIcons);
					if(DataManager.Instance.IsMyData(iter.InDate))
					{
						mMySiblingIndex = i;
					}
					i++;
				}
				mIsLoading = false;
				mOpenMyRankingFirstTime = 1;
			});
		}
		else
		{
			if(mMyRankingElements.transform.childCount == 0)
			{
				GameObject Temp;
				int i = 0;
				foreach (var iter in RankingManager.Instance.MyRanking)
				{
					Temp = Instantiate(mRankingElementPrefab, mMyRankingElements.transform);

					Temp.GetComponent<RankingElement>().Setting(iter, mAdaptIcons, mTierIcons, mTopPlayerIcons);
					if (DataManager.Instance.IsMyData(iter.InDate))
					{
						mMySiblingIndex = i;
					}
					i++;
				}
			}
			CenterElement(mMySiblingIndex);
		}
	}

	void CreateElements(GameObject _Elements, LinkedList<RankingData> _Ranking)
	{
		GameObject Temp;
		foreach (var iter in _Ranking)
		{
			Temp = Instantiate(mRankingElementPrefab, _Elements.transform);
			Temp.GetComponent<RankingElement>().Setting(iter, mAdaptIcons, mTierIcons, mTopPlayerIcons);
		}
	}

	void CenterElement(float _Index)
	{
		_Index = Mathf.Clamp(_Index - 2, 0, mCurRanking.Count - 5);
		float Dist = 1f / (mCurRankingElements.transform.childCount - 5);
		mScrollbar.value = 1 - (Dist * _Index);
	}

	IEnumerator CR_SetScrollPosition(float _Position)
	{
		Debug.Log(mScrollbar.value);
		yield return null;
		yield return null;
		float a = (_Position + 2.5f) / mCurRankingElements.transform.childCount * 0.5f;
		mScrollbar.value -= a;
		Debug.Log(mScrollbar.value);
	}

	public void OpenWindow()
	{
		SoundManager.Instance.PlaySound("Sfx_Button");
		mIsTouchAble = false;
		StartCoroutine(CR_OpenWindow());
		mScrollbar.value = 1;
	}
	IEnumerator CR_OpenWindow()
	{
		mWindow.gameObject.SetActive(true);
		mWindow.transform.localScale = new Vector3(0.1f, 0.1f, 1);
		while (mWindow.transform.localScale.x < 1)
		{
			mWindow.transform.localScale += new Vector3(5f, 5f, 0) * Time.deltaTime;
			yield return null;
		}
		mWindow.transform.localScale = new Vector3(1, 1, 1);
		mIsTouchAble = true;
	}

	IEnumerator CR_CloseWindow()
	{
		while (mWindow.transform.localScale.x > 0.1)
		{
			mWindow.transform.localScale -= new Vector3(5f, 5f, 0) * Time.deltaTime;
			yield return null;
		}
		mWindow.gameObject.SetActive(false);
		yield break;
	}
}
