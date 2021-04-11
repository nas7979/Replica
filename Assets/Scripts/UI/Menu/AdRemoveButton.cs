using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using GooglePlayGames;

public class AdRemoveButton : MonoBehaviour
{
	[SerializeField] private GameObject mErrorMessage = null;
	[SerializeField] private GameObject mGuestToGoogleWindow = null;
	[SerializeField] private Button m_YesButton = null;
	[SerializeField] private Button m_NoButton = null;
	// Start is called before the first frame update
	void Awake()
    {
		if (DataManager.Instance.GetData("AdsRemove") == "True")
		{
			gameObject.SetActive(false);
		}

		GetComponent<IAPButton>().enabled = false;
		SendQueue.Enqueue(Backend.BMember.GetUserInfo, (BackendReturnObject bro) =>
		{
			DataManager.Instance.InDate = bro.GetReturnValuetoJSON()["row"]["inDate"].ToString();
			if (bro.GetReturnValuetoJSON()["row"]["federationId"] != null)
			{
				GetComponent<IAPButton>().enabled = true;
			}
		});
	}

	public void OnGuestClick()
	{
		if(GetComponent<IAPButton>().enabled == false)
		{
			mGuestToGoogleWindow.SetActive(true);
		}
	}

	public void GuestToGoogle()
	{
		m_YesButton.interactable = false;
		m_NoButton.interactable = false;
		if (Social.localUser.authenticated)
		{
			string Code = Backend.BMember.ChangeCustomToFederation(PlayGamesPlatform.Instance.GetIdToken(), FederationType.Google).GetStatusCode();
			switch (Code)
			{
				case "204":
					DisplayErrorMessage("구글 계정 연동에 성공하였습니다.");
					break;
				case "409":
					DisplayErrorMessage("이미 가입되어 있는 구글 계정입니다.");
					break;
			}
			m_YesButton.interactable = true;
			m_NoButton.interactable = true;
		}
		else
		{
			Social.localUser.Authenticate((bool _IsSuccess) =>
			{
				if (_IsSuccess)
				{
					string Code = Backend.BMember.ChangeCustomToFederation(PlayGamesPlatform.Instance.GetIdToken(), FederationType.Google).GetStatusCode();
					switch (Code)
					{
						case "204":
							DisplayErrorMessage("구글 계정 연동에 성공하였습니다.");
							break;
						case "409":
							DisplayErrorMessage("이미 가입되어 있는 구글 계정입니다.");
							break;
					}
				}
				else
				{
					DisplayErrorMessage("구글 로그인에 실패했습니다.");
				}
				m_YesButton.interactable = true;
				m_NoButton.interactable = true;
			});
		}
	}

	public void OnPurchase()
	{
		DataManager.Instance.SetData("AdsRemove", "True");
		Param param = new Param();
		param.Add("isAdsRemoved", true);
		param.Add("gamerInDate", DataManager.Instance.InDate);
		DataManager.Instance.SendDataToServerSchema("adsremove", param);
		gameObject.SetActive(false);
		DataManager.Instance.AddUnlockedSkin("Item_Rare_Mario");
	}

	public void DisplayErrorMessage(string _Desc)
	{
		mErrorMessage.SetActive(true);
		mErrorMessage.transform.GetChild(1).GetComponent<Text>().text = _Desc;
	}

	public void OnPurchaseFailed(Product _Product, PurchaseFailureReason _Reason)
	{
		if(_Reason != PurchaseFailureReason.UserCancelled)
		{
			DisplayErrorMessage("결제에 실패하였습니다: " + _Reason.ToString());
		}
	}
}
