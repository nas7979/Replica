using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop_TableItem : MonoBehaviour
{
	[InspectorName("스킨 등급 할당 N : 0, R : 1, L : 2")]
	public int SkinRank = 0;


	[SerializeField] private string mDesc = "효과 없음";
	public string Desc
	{
		get { return mDesc; }
	}

	[SerializeField] private string mName = "너의 이름은.";
	public string Name
	{
		get { return mName; }
	}

	[SerializeField] private int mPrice = 0;
	public int Price
	{
		get { return mPrice; }
	}
	[SerializeField] private string mDataKey = "";
	[SerializeField] private Replica_Menu mReplica = null;
	private bool mIsUnlocked = false;

	private void OnEnable()
	{
		transform.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.GetItemSprite(gameObject.name);
		mIsUnlocked = DataManager.Instance.GetData(gameObject.name).Equals("Unlocked") ? true : false;
		if(mIsUnlocked)
		{
			transform.GetChild(1).gameObject.SetActive(false);
			transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);

			if(DataManager.Instance.GetData(mDataKey) == gameObject.name)
			{
				transform.GetChild(2).gameObject.SetActive(true);
			}
		}
	}

	public void Unlock()
	{
		mIsUnlocked = true;
		transform.GetChild(1).gameObject.SetActive(false);
		transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
		DataManager.Instance.AddUnlockedSkin(gameObject.name);
		SoundManager.Instance.PlaySound("Sfx_Buy");
	}

	public bool IsUnlocked()
	{
		return mIsUnlocked;
	}

	public void Equip()
	{
		DataManager.Instance.SetData(mDataKey, gameObject.name);
		transform.GetChild(2).gameObject.SetActive(true); // CheckIcon 활성화
		mReplica.SetImage();
		mReplica.GetComponent<Image>().color = Color.white;
	}

	public void Unequip()
	{
		DataManager.Instance.SetData(mDataKey, "Core-idle");
		transform.GetChild(2).gameObject.SetActive(false); // CheckIcon 비활성화
		mReplica.SetImage();
	}

	public bool IsEquiped()
	{
		return DataManager.Instance.GetData(mDataKey) == gameObject.name;
	}
}
