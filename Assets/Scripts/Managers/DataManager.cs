using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System.Linq;

public static class DataManagerUtilities
{
	public static T StrToEnum<T>(this string _String)
	{
#if UNITY_EDITOR
		if (System.Enum.IsDefined(typeof(T), _String) == false)
		{
			Debug.Log((_String) + "을" + typeof(T) + "으로 변환할 수 없습니다.");
		}
#endif

		return (T)System.Enum.Parse(typeof(T), _String);
	}
}

public class DataManager : Singleton<DataManager>
{
	private void Awake()
	{
		GameObject Temp = GameObject.Find(gameObject.name);
		if (Temp != gameObject)
		{
			Destroy(gameObject);
		}
		else
		{
			DontDestroyOnLoad(gameObject);
		}

		Sprite[] ItemSprites = Resources.LoadAll<Sprite>("ItemSprites");
		for(int i = 0; i < ItemSprites.Length; i++)
		{
			mItemSprites.Add(ItemSprites[i].name, ItemSprites[i]);
		}

		ClearAdaptCount();
		if (SendQueue.IsInitialize == false)
		{
			SendQueue.StartSendQueue();
		}
	}

	private void OnApplicationQuit()
	{
		while (SendQueue.UnprocessedFuncCount > 0)
		{
			SendQueue.Poll();
		}
		SendQueue.StopSendQueue();
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause == true)
		{
			while (SendQueue.UnprocessedFuncCount > 0)
			{
				SendQueue.Poll();
			}
			SendQueue.PauseSendQueue();
		}
		else
		{
			SendQueue.ResumeSendQueue();
		}
	}

	private void FixedUpdate()
	{
		SendQueue.Poll();
	}

	/// <summary>
	/// Server
	/// </summary>
	private string mInDate = "";
	public string InDate { get => mInDate; set => mInDate = value; }
	public bool IsMyData(string _InDate)
	{
		return mInDate.Equals(_InDate);
	}

	public void SendDataToServerSchema(string _Table, Param _Param)
	{
		Where where = new Where();
		where.Equal("gamerInDate", mInDate);
		SendQueue.Enqueue(Backend.GameSchemaInfo.Get, _Table, where, 1, (BackendReturnObject Getbro) =>
		{
			
			if (Getbro.IsSuccess())
			{
				SendQueue.Enqueue(Backend.GameSchemaInfo.Update, _Table, Getbro.GetReturnValuetoJSON()["rows"][0]["inDate"]["S"].ToString(), _Param, (BackendReturnObject Updatebro) =>
				{

				});
			}
			else
			{
				SendQueue.Enqueue(Backend.GameSchemaInfo.Insert, _Table, _Param, (BackendReturnObject Insertbro) =>
				{
					
				});
			}
		});
	}

	public void GetDataFromServerSchema(string _Table, System.Action<BackendReturnObject> _Callback)
	{
		Where where = new Where();
		where.Equal("gamerInDate", InDate);
		SendQueue.Enqueue(Backend.GameSchemaInfo.Get, _Table, where, 1, (BackendReturnObject Getbro) =>
		{
			_Callback(Getbro);
		});
	}

	public void DeleteDataOfServer(string _Key)
	{
		SendQueue.Enqueue(Backend.GameSchemaInfo.Delete, _Key, mInDate, (BackendReturnObject) =>
		{

		});
	}

	public void Clear()
	{
		mUnlockedSkin = "";
		mUserData.Clear();
		mHighScore = 0;
	}

	/// <summary>
	/// Gold
	/// </summary>
	private int mGold = 0;
	public int Gold
	{
		get { return mGold; }
		set { mGold = value; }
	}
	public void AddGold(int _Value)
	{
		mGold += _Value;
		Param param = new Param();
		param.Add("gold", mGold);
		SendDataToServerSchema("gold", param);
	}

	/// <summary>
	/// UserData
	/// </summary>
	private Dictionary<string, string> mUserData = new Dictionary<string, string>();
	private string mUnlockedSkin = "";

	public string GetData(string _Key, string _StartValue = "NULL")
	{
		string OutValue;
		if(mUserData.TryGetValue(_Key, out OutValue))
		{
			return OutValue;
		}
		else
		{
			mUserData.Add(_Key, _StartValue);
			return _StartValue;
		}
	}

	public string SetData(string _Key, string _Value)
	{
		mUserData[_Key] = _Value;
		return _Value;
	}

	public void AddUnlockedSkin(string _Skin)
	{
		if (mUserData[_Skin] == "Unlocked")
			return;
		mUnlockedSkin += _Skin + " ";
		SetData(_Skin, "Unlocked");
		Param param = new Param();
		param.Add("unlocked", mUnlockedSkin);
		SendDataToServerSchema("skin", param);
	}

	public void SetUnlockedSkin(string _UnlockedSkin)
	{
		mUnlockedSkin = _UnlockedSkin;
	}

	/// <summary>
	/// ItemSprites
	/// </summary>
	private Dictionary<string, Sprite> mItemSprites = new Dictionary<string, Sprite>();

	public Sprite GetItemSprite(string _Key)
	{
		return _Key.Equals("NULL") ? null : mItemSprites[_Key];
	}

	/// <summary>
	/// AdaptCount
	/// </summary>
	[System.Serializable]
	public class AdaptCount
	{
		[SerializeField] private EnumAdapts mType;
		public EnumAdapts Type { get => mType; set => mType = value; }
		[SerializeField] private int mCount;
		public int Count { get => mCount; set => mCount = value; }
		public AdaptCount(EnumAdapts _Type)
		{
			Type = _Type;
			Count = 1;
		}
	}
	private List<AdaptCount> mAdaptCount = new List<AdaptCount>();

	public void AddAdpatCount(EnumAdapts _Adapt)
	{
		for(int i = 0; i < mAdaptCount.Count; i++)
		{
			if(mAdaptCount[i].Type == _Adapt)
			{
				mAdaptCount[i].Count++;
				return;
			}
		}
		mAdaptCount.Add(new AdaptCount(_Adapt));
		mAdaptCount.Sort(delegate(AdaptCount _Prev, AdaptCount _Next)
		{
			if (_Prev.Count < _Next.Count) return 1;
			else if (_Prev.Count > _Next.Count) return -1;
			return 0;
		});
	}

	public AdaptCount GetAdaptCountSorted(int _Index)
	{
		return _Index >= mAdaptCount.Count ? null : mAdaptCount[_Index];
	}

	public void ClearAdaptCount()
	{
		mAdaptCount.Clear();
	}

	private int mHighScore = 0;
	public int HighScore { get => mHighScore; set => mHighScore = value; }

	public float Mileage { get; set; }
}
