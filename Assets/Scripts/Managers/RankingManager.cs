using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class RankingData
{
	private string mInDate;
	public string InDate { get => mInDate; set => mInDate = value; }
	private int mRank;
	public int Rank { get => mRank; set => mRank = value; }
	private string mName;
	public string Name { get => mName; set => mName = value; }
	private int mScore;
	public int Score { get => mScore; set => mScore = value; }
	private RankingExtraData mExtraData = null;
	public RankingExtraData ExtraData { get => mExtraData; set => mExtraData = value; }
	public RankingElement Element { get; set; }
}

public class RankingExtraData
{
	[SerializeField] private int mTier;
	public int Tier { get => mTier; set => mTier = value; }
	[SerializeField] private DataManager.AdaptCount[] mMostSelAdapt = new DataManager.AdaptCount[3];
	public DataManager.AdaptCount[] MostSelAdapt { get => mMostSelAdapt; set => mMostSelAdapt = value; }
	[SerializeField] private string mSkin;
	public string Skin { get => mSkin; set => mSkin = value; }
}

public class RankingManager : Singleton<RankingManager>
{
	private const string mRankinguuid = "b4c3bcd0-5a12-11eb-8f27-af991fb502b0";
	public int mRankingTotalCount { get; private set; }
	private LinkedList<RankingData> mAllRanking = new LinkedList<RankingData>();
	public LinkedList<RankingData> AllRanking { get => mAllRanking; }
	private LinkedList<RankingData> mMyRanking = new LinkedList<RankingData>();
	public LinkedList<RankingData> MyRanking { get => mMyRanking; }
	private RankingData mMyRankingData = new RankingData();
	public RankingData MyRankingData { get => mMyRankingData; }

	public void Awake()
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
	}

	public void UpdateRanking(LinkedList<RankingData> _Ranking, int _From, int _Count, System.Action _Callback)
	{
		List<RankingData> ListTemp = new List<RankingData>();
		SendQueue.Enqueue(Backend.RTRank.GetRTRankByUuid, mRankinguuid, _Count, _From, (BackendReturnObject bro) =>
		{
			if (bro.IsSuccess())
			{
				LitJson.JsonData Json = bro.Rows();
				mRankingTotalCount = int.Parse(bro.GetReturnValuetoJSON()["totalCount"].ToString());
				for (int i = 0; i < Json.Count; i++)
				{
					RankingData Temp;
					Temp = new RankingData();
					Temp.InDate = Json[i]["gamerInDate"].ToString();
					if(DataManager.Instance.IsMyData(Temp.InDate))
					{
						Temp = mMyRankingData;
					}
					ListTemp.Add(Temp);
					Temp.Name = Json[i]["nickname"].ToString();
					Temp.Score = int.Parse(Json[i]["score"]["N"].ToString());
					Temp.Rank = int.Parse(Json[i]["rank"]["N"].ToString());

					Where where = new Where();
					where.Equal("gamerInDate", Temp.InDate);
					SendQueue.Enqueue(Backend.GameSchemaInfo.Get, "highscore_extradata", where, 1, (BackendReturnObject ExtraDatabro) =>
					{
						Temp.ExtraData = new RankingExtraData();
						Temp.ExtraData = JsonUtility.FromJson<RankingExtraData>(ExtraDatabro.Rows()[0]["extradata"]["S"].ToString());
					});
				}

				if(_Ranking.Count == 0 || _Ranking.First.Value.Rank < _From)
				{
					for (int i = 0; i < ListTemp.Count; i++)
					{
						_Ranking.AddLast(ListTemp[i]);
					}
				}
				else
				{
					for (int i = ListTemp.Count - 1; i >= 0; i--)
					{
						_Ranking.AddFirst(ListTemp[i]);
					}
				}
			}

			_Callback();
		});
	}

	public void GetMyRankingData(System.Action _Callback)
	{
		SendQueue.Enqueue(Backend.RTRank.GetMyRTRank, mRankinguuid, 0, (BackendReturnObject bro) =>
		{
			if (bro.IsSuccess())
			{
				LitJson.JsonData Json = bro.Rows();
				mRankingTotalCount = int.Parse(bro.GetReturnValuetoJSON()["totalCount"].ToString());
				for (int i = 0; i < Json.Count; i++)
				{
					mMyRankingData.Name = Json[i]["nickname"].ToString();
					mMyRankingData.Score = int.Parse(Json[i]["score"]["N"].ToString());
					mMyRankingData.Rank = int.Parse(Json[i]["rank"]["N"].ToString());
					mMyRankingData.InDate = Json[i]["gamerInDate"].ToString();

					Where where = new Where();
					where.Equal("gamerInDate", mMyRankingData.InDate);
					SendQueue.Enqueue(Backend.GameSchemaInfo.Get, "highscore_extradata", where, 1, (BackendReturnObject ExtraDatabro) =>
					{
						mMyRankingData.ExtraData = JsonUtility.FromJson<RankingExtraData>(ExtraDatabro.Rows()[0]["extradata"]["S"].ToString());
					});
				}
				_Callback();
			}
		});
	}

	public void AddRanking(bool _isForFirstLogin = false)
	{
		int Score = (int)GameManager.Instance.GetTotalScore();
		RankingExtraData ExtraData = new RankingExtraData();
		ExtraData.MostSelAdapt[0] = DataManager.Instance.GetAdaptCountSorted(0);
		ExtraData.MostSelAdapt[1] = DataManager.Instance.GetAdaptCountSorted(1);
		ExtraData.MostSelAdapt[2] = DataManager.Instance.GetAdaptCountSorted(2);
		ExtraData.Tier = GameManager.GetRound();
		if (_isForFirstLogin)
			ExtraData.Skin = "Core-idle";
		else
			GameManager.Instance.GetReplica().GetComponent<Replica>().SetSkinOfRankingData(ExtraData);

		LitJson.JsonData Json = JsonUtility.ToJson(ExtraData);
		Param param = new Param();
		param.Add("extradata", Json.ToString());
		param.Add("gamerInDate", DataManager.Instance.InDate);

		DataManager.Instance.SendDataToServerSchema("highscore_extradata", param);

		SendQueue.Enqueue(Backend.GameSchemaInfo.Get, "highscore", DataManager.Instance.InDate, (BackendReturnObject GetBro) =>
		{
			Param ScoreParam = new Param();
			ScoreParam.Add("score", !_isForFirstLogin ? Score : 0);
			ScoreParam.Add("gamerInDate", DataManager.Instance.InDate);
			if(GetBro.IsSuccess())
			{
				DataManager.Instance.SendDataToServerSchema("highscore", ScoreParam);
				SendQueue.Enqueue(Backend.GameInfo.UpdateRTRankTable, "highscore", "score", !_isForFirstLogin ? Score : 0, GetBro.GetInDate(), (BackendReturnObject UpdateBro) =>
				{

				});
			}
			else
			{
				SendQueue.Enqueue(Backend.GameSchemaInfo.Insert, "highscore", ScoreParam, (BackendReturnObject InsertBro) =>
				{
					SendQueue.Enqueue(Backend.GameInfo.UpdateRTRankTable, "highscore", "score", !_isForFirstLogin ? Score : 0, InsertBro.GetInDate(), (BackendReturnObject UpdateBro) =>
					{

					});
				});
			}
		});
	}

	public void Clear()
	{
		mAllRanking.Clear();
		mMyRanking.Clear();
		mMyRankingData = new RankingData();
	}
}
