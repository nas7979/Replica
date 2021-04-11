using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class Shop_Buttons : MonoBehaviour
{
	[SerializeField] private Text mGold = null;
	[SerializeField] private Text mMileage = null;
	[SerializeField] private Shop_TableButton mTableButton;
	[SerializeField] private FadeInFadeOut mFade;

	[SerializeField] private Button mUnlockButton = null;
	[SerializeField] private Sprite mSprite_ButtonGreen = null;
	[SerializeField] private Sprite mSprite_ButtonOrange = null;
	[SerializeField] private Sprite mSprite_ButtonGray = null;

	[SerializeField] private NOTICETEXT mNeedGoldNoticeText;

	private Shop_TableItem curEquipItem = null;

	[InspectorName("씬 UI 업데이트를 위해 광고 버튼 스크립트 할당")]
	public AdReward.AdRewardButton mAdRewardButton = null;
	private void OnEnable()
	{
		mAdRewardButton.shopButtons = this;
		mGold.text = DataManager.Instance.Gold.ToString();
		mMileage.text = "+" + DataManager.Instance.Mileage.ToString();
	}

    public void ResetSceneUI()
    {
		mTableButton.OnEnable();
		OnEnable();
		ChangeUnlockButtonSprite(true);
    }
    private void Update()
    {
    }
    public void Unlock()
	{
		Shop_TableItem Item = mTableButton.GetCurTable().GetCurItem();
		if(Item.IsUnlocked())
		{
			SoundManager.Instance.PlaySound("Sfx_EquipSkin");
			if (Item.IsEquiped())
			{
				Item.Unequip();
			}
			else
			{
				if (curEquipItem)
				{
					curEquipItem.Unequip();//이미 입고 있는 스킨을 비활성화한다
				}
				curEquipItem = Item;
				Item.Equip();
			}
			mTableButton.GetCurTable().SetText();
		}
		else
		{
			//노말 스킨
			if(mTableButton.GetCurTable().name.Equals("ItemTable_Normal"))
			{
				//돈이 충분하다면 잠금 해제
				if (DataManager.Instance.Gold >= Item.Price)
				{
					DataManager.Instance.AddGold(-Item.Price);
					mGold.text = DataManager.Instance.Gold.ToString();
					Item.Unlock();
					if (curEquipItem)
					{
						curEquipItem.Unequip();//이미 입고 있는 스킨을 비활성화한다
					}
					curEquipItem = Item;
					Item.Equip();

					mTableButton.GetCurTable().SetText();
				}
				else
				{
					SoundManager.Instance.PlaySound("Sfx_Error");

					mNeedGoldNoticeText.gameObject.SetActive(true);
					mNeedGoldNoticeText.SetText("코인이 부족합니다.");
					mNeedGoldNoticeText.Effect();
				}
			}
			//레어, 레전드 스킨 
			else if(Item.SkinRank >= 1)
			{
				if(!Item.name.Equals("Item_Rare_Mario"))
				{
					SoundManager.Instance.PlaySound("Sfx_Button");
					mFade.FadeOut("Gacha", 0.4f);
				}
				else
				{
					SoundManager.Instance.PlaySound("Sfx_Button");
					mFade.FadeOut("Gacha", 0.4f);
				}
				
			}
		}
		ChangeUnlockButtonSprite(Item.IsUnlocked());
	}

	public void BackToMenu(float mFadeTime)
	{
		SoundManager.Instance.PlaySound("Sfx_Button");
		mFade.FadeOut("Menu", mFadeTime);
		Param param = new Param();
		param.Add("currentSkin", DataManager.Instance.GetData("Replica_Skin"));
		DataManager.Instance.SendDataToServerSchema("skin", param);
	}

	public void ChangeUnlockButtonSprite(bool tmp = true) {
		ColorBlock colors = mUnlockButton.colors;
		colors.colorMultiplier = 1;
		//mUnlockButton.interactable = true;
		mUnlockButton.transition = Selectable.Transition.Animation;

		Shop_TableItem Item = mTableButton.GetCurTable().GetCurItem();

		//장착중인 아이템일 경우
		if (Item.IsEquiped())
		{
			mUnlockButton.image.sprite = mSprite_ButtonGreen;
		}
		//이미 잠금 해제되어있거나, 뽑기 씬에서 획득 가능한 경우, 구매 가능한 경우
		else if (Item.IsUnlocked() == true || Item.SkinRank > 0 || Item.SkinRank == 0 && DataManager.Instance.Gold >= mTableButton.GetCurTable().GetCurItem().Price)
		//else if (Item.IsUnlocked() == true || Item.SkinRank > 0 && !Item.name.Equals("Item_Rare_Mario") || Item.SkinRank == 0 && DataManager.Instance.Gold >= mTableButton.GetCurTable().GetCurItem().Price)
			mUnlockButton.image.sprite = mSprite_ButtonOrange;
		else
		{
			mUnlockButton.image.sprite = mSprite_ButtonGray;
			mUnlockButton.transition = Selectable.Transition.ColorTint;
			colors.colorMultiplier = 100;
			//mUnlockButton.interactable = false;
		}
		mUnlockButton.colors = colors;
	}
}
