using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class Title_Login : MonoBehaviour
{
	[SerializeField] private GameObject mTouchCheck = null;
	[SerializeField] private Button mButton_GuestLogin = null;
	[SerializeField] private Button mButton_GoogleLogin = null;
	[SerializeField] private Button mButton_CustomLogin = null;
	[SerializeField] private Button mButton_Login = null;
	[SerializeField] private Button mButton_SignUp = null;
	[SerializeField] private Button mButton_Confirm = null;
	[SerializeField] private Button mButton_Cancel = null;
	[SerializeField] private InputField mInput_Name = null;
	[SerializeField] private InputField mInput_ID = null;
	[SerializeField] private InputField mInput_PW = null;
	[SerializeField] private Title_OnConnectFailed mOnConnectFailed = null;
	[SerializeField] private GameObject mErrorMessage = null;
	private CurState mCurState = CurState.None;
	private enum CurState
	{
		None,
		Login,
		SignUp,
		Name
	}

	public void Awake()
	{
		if(SystemInfo.deviceType == DeviceType.Desktop)
			mButton_CustomLogin.gameObject.SetActive(true);

		if(Backend.IsInitialized == false)
		{
			Backend.Initialize(OnBackendInitialized);
		}
		else
		{
			OnBackendInitialized();
		}

		PlayGamesClientConfiguration Config = new PlayGamesClientConfiguration.Builder().RequestServerAuthCode(false).RequestIdToken().Build();
		PlayGamesPlatform.InitializeInstance(Config);
		PlayGamesPlatform.DebugLogEnabled = true;
		PlayGamesPlatform.Activate();
	}

	private string GetGoogleToken()
	{
		if (PlayGamesPlatform.Instance.localUser.authenticated)
		{
			return PlayGamesPlatform.Instance.GetIdToken();
		}
		else
		{
			DisplayErrorMessage("구글 로그인 빵빵 터졌어요");
			return null;
		}
	}

	private void OnBackendInitialized()
	{
		if (Backend.IsInitialized)
		{
			//이미 로그인한 적이 있어서 기기에 계정 정보가 남아있을 경우
			if (Backend.BMember.LoginWithTheBackendToken().IsSuccess())
			{
				OnLogined();
			}
			//본 기기에서 처음 접속했거나, 로그아웃을 했을 경우, 혹은 같은 기기에서 1년 이상이 지난 후에 접속했을 경우
			else
			{
				mButton_GuestLogin.gameObject.SetActive(true);
				mButton_GoogleLogin.gameObject.SetActive(true);
			}
		}
		else
		{
			mOnConnectFailed.gameObject.SetActive(true);
			mOnConnectFailed.OnRetry = () => { Backend.Initialize(OnBackendInitialized); };
		}
	}

	public void OnClickGuestLogin()
	{
		mButton_GuestLogin.gameObject.SetActive(false);
		mButton_GoogleLogin.gameObject.SetActive(false);
		mButton_CustomLogin.gameObject.SetActive(false);

		Backend.BMember.GuestLogin((BackendReturnObject bro) => {
			if(bro.IsSuccess())
			{
				OnLogined();
			}
			else
			{
				DisplayErrorMessage("알 수 없는 오류가 발생했습니다.");
				mButton_GuestLogin.gameObject.SetActive(true);
				mButton_GoogleLogin.gameObject.SetActive(true);
				if (SystemInfo.deviceType == DeviceType.Desktop)
					mButton_CustomLogin.gameObject.SetActive(true);
			}
		});
	}

	public void OnClickCustomLogin()
	{
		mButton_Login.gameObject.SetActive(true);
		mButton_SignUp.gameObject.SetActive(true);
		mButton_GuestLogin.gameObject.SetActive(false);
		mButton_GoogleLogin.gameObject.SetActive(false);
		mButton_CustomLogin.gameObject.SetActive(false);
	}

	public void OnClickGoogleLogin()
	{
		mButton_GuestLogin.gameObject.SetActive(false);
		mButton_GoogleLogin.gameObject.SetActive(false);
		mButton_CustomLogin.gameObject.SetActive(false);
		if (Social.localUser.authenticated == true)
		{
			BackendReturnObject bro = Backend.BMember.AuthorizeFederation(GetGoogleToken(), FederationType.Google);
			OnLogined();
		}
		else
		{
			Social.localUser.Authenticate((bool _IsSuccess) =>
			{
				if (_IsSuccess)
				{
					BackendReturnObject bro = Backend.BMember.AuthorizeFederation(GetGoogleToken(), FederationType.Google);
					OnLogined();
				}
				else
				{
					DisplayErrorMessage("구글 로그인에 실패했습니다.");
					mButton_GuestLogin.gameObject.SetActive(true);
					mButton_GoogleLogin.gameObject.SetActive(true);
				}
			});
		}
	}

	public void OnClickLogin()
	{
		DisableButtons();
		mCurState = CurState.Login;
	}

	public void OnClickSignUp()
	{
		DisableButtons();
		mCurState = CurState.SignUp;
	}

	public void OnClickConfirm()
	{
		switch(mCurState)
		{
			case CurState.Login:
				{
					BackendReturnObject bro = Backend.BMember.CustomLogin(mInput_ID.text, mInput_PW.text);
					if (bro.IsSuccess())
					{
						OnLogined();
					}
					else
					{
						switch (bro.GetMessage())
						{
							case "bad customId, 잘못된 customId 입니다": DisplayErrorMessage("존재하지 않는 아이디입니다."); break;
							case "bad customPassword, 잘못된 customPassword 입니다": DisplayErrorMessage("비밀번호가 일치하지 않습니다."); break;
						}
					}
					break;
				}

			case CurState.SignUp:
				{
					if (mInput_ID.text == "")
					{
						DisplayErrorMessage("아이디를 입력해 주세요.");
						return;
					}
					if (mInput_PW.text == "")
					{
						DisplayErrorMessage("비밀번호를 입력해 주세요.");
						return;
					}

					BackendReturnObject bro = Backend.BMember.CustomSignUp(mInput_ID.text, mInput_PW.text);
					if (bro.IsSuccess())
					{
						OnLogined();
					}
					else
					{
						if (bro.GetStatusCode().Equals("409"))
						{
							DisplayErrorMessage("해당 아이디는\n사용할 수 없습니다.");
						}
					}
					break;
				}

			case CurState.Name:
				{
					if (mInput_Name.text == "")
					{
						DisplayErrorMessage("닉네임을 입력해 주세요.");
						return;
					}
					BackendReturnObject bro = Backend.BMember.CheckNicknameDuplication(mInput_Name.text);
					if (bro.IsSuccess())
					{
						if (Backend.BMember.CreateNickname(mInput_Name.text).IsSuccess())
						{
							OnLogined();
						}
						else
						{
							DisplayErrorMessage("알 수 없는 오류가 발생했습니다:" + bro.ToString());
						}
					}
					else
					{
						switch (bro.GetStatusCode())
						{
							case "409": DisplayErrorMessage("이미 존재하는 닉네임입니다."); break;
							case "400": DisplayErrorMessage("닉네임의 처음이나 끝에\n공백이 있거나,\n20자 이상입니다."); break;
						}
					}

					break;
				}
		}
	}

	public void OnClickCancel()
	{
		mButton_Login.gameObject.SetActive(true);
		mButton_SignUp.gameObject.SetActive(true);
		mButton_Confirm.gameObject.SetActive(false);
		mButton_Cancel.gameObject.SetActive(false);
		mInput_ID.gameObject.SetActive(false);
		mInput_ID.text = "";
		mInput_PW.gameObject.SetActive(false);
		mInput_PW.text = "";
		mInput_Name.gameObject.SetActive(false);
		mInput_Name.text = "";
	}

	public void OnLogined()
	{
		mButton_Cancel.gameObject.SetActive(false);
		mInput_ID.gameObject.SetActive(false);
		mInput_PW.gameObject.SetActive(false);
		mInput_Name.gameObject.SetActive(false);
		mButton_GoogleLogin.gameObject.SetActive(false);
		mButton_GuestLogin.gameObject.SetActive(false);
		mButton_CustomLogin.gameObject.SetActive(false);
		BackendReturnObject bro = Backend.BMember.GetUserInfo();
		DataManager.Instance.InDate = bro.GetReturnValuetoJSON()["row"]["inDate"].ToString();
		if (bro.GetReturnValuetoJSON()["row"]["nickname"] == null)
		{
			mInput_Name.gameObject.SetActive(true);
			mButton_Confirm.gameObject.SetActive(true);
			mCurState = CurState.Name;
			return;
		}
		else
		{
			mButton_Confirm.gameObject.SetActive(false);
			mTouchCheck.SetActive(true);
		}

		Where where = new Where();
		where.Equal("gamerInDate", DataManager.Instance.InDate);
		bro = Backend.GameSchemaInfo.Get("gold", where, 1);
		if (bro.IsSuccess())
		{
			DataManager.Instance.Gold = int.Parse(bro.Rows()[0]["gold"]["N"].ToString());
			DataManager.Instance.SetData("IsFirstPlay", "False");
		}
		else
		{
			Param param = new Param();
			param.Add("gold", 0);
			param.Add("gamerInDate", DataManager.Instance.InDate);
			DataManager.Instance.SendDataToServerSchema("gold", param);
			DataManager.Instance.SetData("IsFirstPlay", "True");
			RankingManager.Instance.AddRanking(true);
		}

		DataManager.Instance.GetDataFromServerSchema("highscore", (BackendReturnObject Getbro) =>
		{
			if(Getbro.IsSuccess())
			{
				DataManager.Instance.HighScore = int.Parse(Getbro.Rows()[0]["score"]["N"].ToString());
			}
		});

		DataManager.Instance.GetDataFromServerSchema("adsremove", (BackendReturnObject Getbro) =>
		{
			if(Getbro.IsSuccess())
			{
				DataManager.Instance.SetData("AdsRemove", Getbro.Rows()[0]["isAdsRemoved"]["BOOL"].ToString());
				AdsManager.Instance.AdsCount = int.Parse(Getbro.Rows()[0]["adsCount"]["N"].ToString());
				AdsManager.Instance.RewardAdsLimit = int.Parse(Getbro.Rows()[0]["rewardAdsLimit"]["N"].ToString());
			}
			else
			{
				Param param = new Param();
				param.Add("isAdsRemoved", false);
				param.Add("adsCount", 0);
				param.Add("rewardAdsLimit", 0);
				param.Add("gamerInDate", DataManager.Instance.InDate);
				DataManager.Instance.SendDataToServerSchema("adsremove", param);
				DataManager.Instance.SetData("AdsRemove", "False");
			}
		});

		DataManager.Instance.GetDataFromServerSchema("lastlogined", (BackendReturnObject Getbro) =>
		{
			if (Getbro.IsSuccess())
			{
				DataManager.Instance.SetData("LastLogined", Getbro.Rows()[0]["date"]["S"].ToString());
			}
			else
			{
				DataManager.Instance.SetData("LastLogined", System.DateTime.Now.ToString());
			}
			Param param = new Param();
			param.Add("date", System.DateTime.Now.ToString());
			param.Add("gamerInDate", DataManager.Instance.InDate);
			DataManager.Instance.SendDataToServerSchema("lastlogined", param);
		});

		DataManager.Instance.GetDataFromServerSchema("mileage", (BackendReturnObject Getbro) =>
		{
			if (Getbro.IsSuccess())
			{
				DataManager.Instance.Mileage = float.Parse(Getbro.Rows()[0]["mileage"]["N"].ToString());
			}
			else
			{
				DataManager.Instance.Mileage = 0;
				Param param = new Param();
				param.Add("mileage", 0f);
				param.Add("gamerInDate", DataManager.Instance.InDate);
				DataManager.Instance.SendDataToServerSchema("mileage", param);
			}
		});

		//언락한 스킨들을 담고 있는 스트링은 맨 끝에 공백이 한칸 있어야함
		DataManager.Instance.GetDataFromServerSchema("skin", (BackendReturnObject Getbro) =>
		{
			if (Getbro.IsSuccess())
			{
				string List = Getbro.Rows()[0]["unlocked"]["S"].ToString();
				DataManager.Instance.SetUnlockedSkin(List);
				if (List.Length == 0) return;

				int Start = 0;
				for (int i = 0; i < List.Length; i++)
				{
					Start = i;
					while (List[i] != ' ')
					{
						i++;
					}
					DataManager.Instance.SetData(new string(List.ToCharArray(), Start, i - Start), "Unlocked");
				}

				DataManager.Instance.SetData("Replica_Skin", Getbro.Rows()[0]["currentSkin"]["S"].ToString());
			}
			else
			{
				Param param = new Param();
				param.Add("unlocked", "");
				param.Add("gamerInDate", DataManager.Instance.InDate);
				DataManager.Instance.SendDataToServerSchema("skin", param);
			}
		});
	}

	public void DisableButtons()
	{
		mButton_Login.gameObject.SetActive(false);
		mButton_SignUp.gameObject.SetActive(false);
		mButton_Confirm.gameObject.SetActive(true);
		mButton_Cancel.gameObject.SetActive(true);
		mInput_ID.gameObject.SetActive(true);
		mInput_PW.gameObject.SetActive(true);
	}

	public void DisplayErrorMessage(string _Desc)
	{
		mErrorMessage.SetActive(true);
		mErrorMessage.transform.GetChild(1).GetComponent<Text>().text = _Desc;
	}
}
