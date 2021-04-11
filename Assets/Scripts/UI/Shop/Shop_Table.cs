using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class Shop_Table : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	[SerializeField] private Shop_Buttons shopButtons;
	[SerializeField] private Scrollbar mScrollbar = null;
	[SerializeField] private Text mItemDesc = null;
	[SerializeField] private Text mItemName = null;
	[SerializeField] private Text mUnlockButtonText = null;
	[SerializeField] private GameObject mUnlockButtonGoldIcon = null;
	[SerializeField] private Replica_Menu mReplica = null;
	[SerializeField] private GameObject mMarioAdCount = null;
	private Shop_TableItem[] mItems;
	private int mSize = 1;
	private int mCurItem = 0;
	private float[] mBoundary;
	private float mDistance;
	private float mTargetPos;
	private bool mIsDraging;

	private void Awake()
	{
		mIsDraging = false;
		mTargetPos = 0;
		Transform Content = transform.GetChild(0).GetChild(0);
		mSize = Content.childCount;
		mScrollbar.value = 0;

		mItems = new Shop_TableItem[mSize];
		mDistance = 1f / (mSize - 1);
		mBoundary = new float[mSize];
		for(int i = 0; i < mSize; i++)
		{
			mBoundary[i] = mDistance * i;
			mItems[i] = Content.GetChild(i).gameObject.GetComponent<Shop_TableItem>();
		}
		SetText();
	}

	private void OnEnable()
	{
		mReplica.SetImage(mItems[mCurItem].name);
		if (!mItems[mCurItem].IsUnlocked())
			mReplica.GetComponent<Image>().color = new Color(1, 1, 1, 0.58f);
		else
			mReplica.GetComponent<Image>().color = Color.white;
	}

	private void Update()
	{
		if (mIsDraging == false)
		{
			mScrollbar.value = Mathf.Lerp(mScrollbar.value, mTargetPos, 0.1f * 60 * Time.deltaTime);
		}
		for(int i = 0; i < mSize; i++)
		{
			mItems[i].transform.localScale = new Vector2(2f, 2f) * (1 - Mathf.Pow(Mathf.Abs(Mathf.Clamp(mBoundary[i] - mScrollbar.value, -0.3f, 0.3f)), 0.5f));
			mItems[i].transform.GetChild(0).localPosition = new Vector2(Mathf.Clamp(mBoundary[i] - mScrollbar.value, -0.3f, 0.3f) * mItems[i].transform.localScale.x * 250f, 0);
		}
	}

	public void OnBeginDrag(PointerEventData _Event)
	{
		mIsDraging = true;
	}

	public void OnDrag(PointerEventData _Event)
	{
		int CurItem = mCurItem;
		for (int i = 0; i < mSize; i++)
		{
			if (mScrollbar.value < mBoundary[i] + mDistance * 0.5f && mScrollbar.value > mBoundary[i] - mDistance * 0.5f)
			{
				mTargetPos = mBoundary[i];
				mCurItem = i;
				SetText();
				shopButtons.ChangeUnlockButtonSprite(GetCurItem().IsUnlocked());
				mReplica.SetImage(mItems[mCurItem].name);
				if (!mItems[mCurItem].IsUnlocked())
					mReplica.GetComponent<Image>().color = new Color(1, 1, 1, 0.58f);
				else
					mReplica.GetComponent<Image>().color = Color.white;

				if (mCurItem != CurItem)
				{
					CurItem = mCurItem;
					SoundManager.Instance.PlaySound("Sfx_Scroll");
				}
			}
		}
	}

	public void OnEndDrag(PointerEventData _Event)
	{
		mIsDraging = false;
	}

	public Shop_TableItem GetCurItem()
	{
		return mItems[mCurItem];
	}

	public void SetText()
	{
		if(mItemDesc != null) mItemDesc.text = GetCurItem().Desc;
		mItemName.text = GetCurItem().Name;
		//mUnlockButtonText.text = (GetCurItem().IsUnlocked() ? (GetCurItem().IsEquiped() ? "착용 해제" : "착용") : GetCurItem().Price.ToString() + "GOLD");
		if (GetCurItem().IsUnlocked() == true)
		{
			mUnlockButtonText.text = (GetCurItem().IsEquiped() ? "착용 해제" : "착용");
			mUnlockButtonGoldIcon.SetActive(false);

			if (!mItems[mCurItem].name.Equals("Item_Rare_Mario"))
			{
				if (mMarioAdCount)
					mMarioAdCount.SetActive(false);
				mUnlockButtonText.transform.parent.gameObject.SetActive(true);
			}
			else
			{
				if (mMarioAdCount && DataManager.Instance.GetData("Item_Rare_Mario") != "Unlocked")
				{
					mMarioAdCount.SetActive(true);
					mMarioAdCount.transform.GetChild(0).GetComponent<Text>().text = "광고" + (20 - AdsManager.Instance.AdsCount).ToString() + "회 시청";
					mUnlockButtonText.transform.parent.gameObject.SetActive(false);
				}
			}
		}
		else
		{
			if(gameObject.name.Equals("ItemTable_Normal"))
			{
				mUnlockButtonText.text = GetCurItem().Price.ToString() + "  ";
				mUnlockButtonGoldIcon.SetActive(true);
				mUnlockButtonText.transform.parent.gameObject.SetActive(true);
			}
			else if(!mItems[mCurItem].name.Equals("Item_Rare_Mario"))
			{
				mUnlockButtonText.text = "뽑기로 획득";
				mUnlockButtonGoldIcon.SetActive(false);
				if (mMarioAdCount)
					mMarioAdCount.SetActive(false);
				mUnlockButtonText.transform.parent.gameObject.SetActive(true);
			}
			else
			{
				mUnlockButtonText.text = "즉시 획득";
				mUnlockButtonGoldIcon.SetActive(false);
				if (mMarioAdCount && DataManager.Instance.GetData("Item_Rare_Mario") != "Unlocked")
				{
					mMarioAdCount.SetActive(true);
					mMarioAdCount.transform.GetChild(0).GetComponent<Text>().text = "광고" + (20 - AdsManager.Instance.AdsCount).ToString() + "회 시청";
					mUnlockButtonText.transform.parent.gameObject.SetActive(false);
				}
			}
		}
	}
}
