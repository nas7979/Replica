using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Replica_Menu : MonoBehaviour
{

	private void Awake()
	{
		SetImage();
	}

	public void SetImage(RankingExtraData _Data)
	{
		GetComponent<Image>().sprite = DataManager.Instance.GetItemSprite(_Data.Skin);
		GetComponent<Image>().SetNativeSize();
	}
	
	public void SetImage(string _SkinName)
	{
		GetComponent<Image>().sprite = DataManager.Instance.GetItemSprite(_SkinName);
		GetComponent<Image>().SetNativeSize();
	}

	public void SetImage()
	{
		GetComponent<Image>().sprite = DataManager.Instance.GetItemSprite(DataManager.Instance.GetData("Replica_Skin", "Core-idle"));
		GetComponent<Image>().SetNativeSize();
	}
}
