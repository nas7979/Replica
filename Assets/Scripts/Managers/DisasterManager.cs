using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisasterManager : MonoBehaviour
{
    private enum EnumDisaster
    {
        Flood,
        Earthquake,
        Meteor,
        End
    }
    private float mProbability; //확률
    private List<GameObject> mFlood = new List<GameObject>();
    public List<GameObject> mEffect;
    private bool mIsReadyDisater; //현재 재난발생 준비중인가
    private int mStartCount;
    private int mFloodActiveCount;
    private List<GameObject> mDisasterPredictionView;
    private bool IsViewPrediction;
    [SerializeField]
    private EnumDisaster mNextDisaster;
	private bool mIsDisasterHappened;

    private Vector2[] mFloodSpawnPos = new Vector2[3];
    private bool mIsFloodWidth;
    private Vector2[] mMeteorSpawnPos = new Vector2[3];


    private void Start()
    {
        for(int i = 0;i<mEffect.Count;i++)
        {
            mEffect[i].SetActive(false);
        }
    }
    private void OnEnable()
    {
        mProbability = 0;
        mStartCount = 0;
        mIsReadyDisater = false;
        mDisasterPredictionView = new List<GameObject>();
        IsViewPrediction = false;
		mIsDisasterHappened = false;
		mNextDisaster = EnumDisaster.End;
    }

    public void OnSlide()
    {
        //현재 자연재해가 대기중이 아닐 때//
        if (!mIsReadyDisater)
        {
            if (Random.Range(0f, 1f) < mProbability && mFloodActiveCount <= 0)
            {
                bool LoopRand = true;
                int ReplicaLevel = GameManager.Instance.GetReplica().GetComponent<Replica>().GetLevel();
                EnumDisaster rand;
                do
                {
                    //자연재해 랜덤
                    rand = (EnumDisaster)Random.Range(0, (int)EnumDisaster.End);
                    switch (rand)
                    {
                        case EnumDisaster.Flood:
                            //레플리카 레벨 체크//
                            if (ReplicaLevel >= 3) //3
                            {
                                ReadyFlood();
                                LoopRand = false;
                            }
                            break;
                        case EnumDisaster.Earthquake:
                            LoopRand = false;
                            break;
                        case EnumDisaster.Meteor:
                            if (ReplicaLevel >= 5) //5
                            {
                                ReadyMeteor();
                                LoopRand = false;
                            }
                            break;
                    }
                } while (LoopRand);
				mStartCount = 2;
                mIsReadyDisater = true;
                mProbability = 0;
                mNextDisaster = rand;

			}
            else
            {
                mProbability += 0.1f;
            }
			mIsDisasterHappened = false;
		}
        //현재 자연재해가 대기중일 때//
        else
        {
            mStartCount--;
            if (mStartCount == 0)
            {
                switch (mNextDisaster)
                {
                    case EnumDisaster.Flood:
						StartCoroutine(CR_SpawnFlood());
                        break;
                    case EnumDisaster.Earthquake:
						StartCoroutine(CR_SpawnEarthquake());
                        break;
                    case EnumDisaster.Meteor:
                        SpawnMeteor();
                        break;
                }
                //StartCoroutine(CR_StartProduction(1f));
                mNextDisaster = EnumDisaster.End;
                mIsReadyDisater = false;
				mIsDisasterHappened = true;

			}

        }

        if (mFlood.Count > 0)
        {
            if (mFloodActiveCount <= 0)
            {
                for (int i = 0; i < mFlood.Count; i++)
                {
                    Flood curFlood = mFlood[i].GetComponent<Flood>();
                    if (curFlood.mTile.GetObjectOnTile() != null && curFlood.mTile.GetObjectOnTile() != gameObject)
                    {
                        curFlood.mTile.GetObjectOnTile().GetComponent<Object>().SetFlood(null);
                    }
                    if (mFlood[i].activeSelf)
                    {
                        curFlood.Deactivate();
                    }
                }
            }
            mFloodActiveCount--;
        }

    }

    private void ReadyFlood()
    {
        //홍수 준비(위치 설정)
        //mDisaster.Clear();
        for (int i = 0; i < mFlood.Count; i++)
        {
            Flood curFlood = mFlood[i].GetComponent<Flood>();
            if (curFlood.mTile.GetObjectOnTile() != null && curFlood.mTile.GetObjectOnTile() != gameObject)
            {
                curFlood.mTile.GetObjectOnTile().GetComponent<Object>().SetFlood(null);
            }
            curFlood.GetComponent<Flood>().Deactivate();
        }
        mFlood.Clear();
        mFloodSpawnPos[0].x = Random.Range(1, 4);
        mFloodSpawnPos[0].y = Random.Range(0, 5);
        if (Random.Range(0, 2) == 0)
        {
            mIsFloodWidth = true;
            mFloodSpawnPos[1] = new Vector2(mFloodSpawnPos[0].x + 1, mFloodSpawnPos[0].y);
            mFloodSpawnPos[2] = new Vector2(mFloodSpawnPos[0].x - 1, mFloodSpawnPos[0].y);
        }
        else
        {
            mIsFloodWidth = false;
            mFloodSpawnPos[0] = new Vector2(mFloodSpawnPos[0].y, mFloodSpawnPos[0].x);
            mFloodSpawnPos[1] = new Vector2(mFloodSpawnPos[0].x, mFloodSpawnPos[0].y + 1);
            mFloodSpawnPos[2] = new Vector2(mFloodSpawnPos[0].x, mFloodSpawnPos[0].y - 1);
        }
        if (GameManager.Instance.GetReplica().GetComponent<Adapt_SensitiveNerve>().CheckPrediction())
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject ObjTemp = ObjectManager.Instance.CreateOutTile(EnumObjects.FloodView);
                ObjTemp.transform.position = GameManager.Instance.GetTile(mFloodSpawnPos[i]).transform.position;
                mDisasterPredictionView.Add(ObjTemp);
                ObjTemp.SetActive(true);
				SoundManager.Instance.PlaySound("Sfx_SensitiveNerve");
			}
        }
    }

    IEnumerator CR_SpawnFlood()
    {
        for (int i = 0; i < mDisasterPredictionView.Count; i++)
        {
            mDisasterPredictionView[i].SetActive(false);
        }
        mDisasterPredictionView.Clear();

        mFloodActiveCount = 3;
        //(홍수 생성)
        mEffect[(int)EnumDisaster.Flood].SetActive(true);
		SoundManager.Instance.PlaySound("Sfx_Flood");

		yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 3; i++)
        {
            CreateFlood(GameManager.Instance.GetTile(new Vector2(mFloodSpawnPos[i].x, mFloodSpawnPos[i].y)));
        }
		GameObject Temp;
		for(int i = 0; i < 5; i++)
		{
			for(int j = 0; j < 5; j++)
			{
				Temp = GameManager.Instance.GetTile(new Vector2(i, j)).GetObjectOnTile();
				if (Temp != null)
				{
					if (Temp.CompareTag("Predator") && Random.Range(0f, 1f) <= GameManager.Instance.GetReplica().GetComponent<Adapt_River>().GetValue())
					{
						Temp.GetComponent<Object>().Deactivate();
						GameManager.Instance.DecreaseObjectCount(EnumObjects.Predator);
					}
				}
			}
		}
    }

    private Flood CreateFlood(Tile _Tile)
    {
        GameObject ObjTemp = _Tile.GetObjectOnTile();
        Flood FloodTemp = ObjectManager.Instance.Create(EnumObjects.Flood, _Tile).GetComponent<Flood>();
        if (ObjTemp != null)
        {
            _Tile.SetObjectOnTile(ObjTemp);
            ObjTemp.GetComponent<Object>().SetFlood(FloodTemp);
        }
        mFlood.Add(FloodTemp.gameObject);
        return FloodTemp;
    }

	IEnumerator CR_SpawnEarthquake()
    {
        //새로운 바위 생성//
        mEffect[(int)EnumDisaster.Earthquake].SetActive(true);
		SoundManager.Instance.PlaySound("Sfx_EarthQuake");

		yield return new WaitForSeconds(0.5f);

		int RockCountMax = GameManager.Instance.GetObjectCount(EnumObjects.Rock);
        Vector2 RandPos;
        Vector2[] SpawnPos = new Vector2[RockCountMax];
        //새로운 바위 위치 설정//
        for (int i = 0; i < RockCountMax; i++)
        {
            while (true)
            {
                RandPos.x = Random.Range(0, 5);
                RandPos.y = Random.Range(0, 5);
                int Count = 0;
                for (int j = 0; j < RockCountMax; j++)
                {
                    //랜덤 포스가 이미 정해진 스폰포스와 같거나 타일에 오브젝트가 없거나 홍수 오브젝트가 아닐경우 스폰포스로 설정
                    if (RandPos == SpawnPos[j] || (GameManager.Instance.GetTile(RandPos).GetObjectOnTile() != null && !(GameManager.Instance.GetTile(RandPos).GetObjectOnTile().CompareTag("Flood"))))
                    {
                        Count++;
                    }
                }
                if (Count == 0)
                {
                    SpawnPos[i] = RandPos;
                    break;
                }
            }
        }
        //새로운 바위 위치 설정//
        //원래 있던 바위 삭제//
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Object ObjTemp;
                if (GameManager.Instance.GetTile(new Vector2(i, j)).GetObjectOnTile() != null)
                {
                    ObjTemp = GameManager.Instance.GetTile(new Vector2(i, j)).GetObjectOnTile().GetComponent<Object>();
                    if (ObjTemp.GetEnumtTag() == EnumObjects.Rock)
                    {
                        ObjTemp.Deactivate();
                    }
                }
            }
        }
        //원래 있던 바위 삭제//
        //새로운 바위 생성//
        for (int i = 0; i < RockCountMax; i++)
        {
            ObjectManager.Instance.Create(EnumObjects.Rock, GameManager.Instance.GetTile(SpawnPos[i]));
        }

		Replica Rep = GameManager.Instance.GetReplica().GetComponent<Replica>();
		float SkinEffectScore = Rep.SkinEffect("OnEarthquake", 0);
		if(SkinEffectScore != 0)
		{
			GameObject Temp = ObjectManager.Instance.CreateInCanvas(EnumObjects.ScoreText);
			Temp.GetComponent<SCORETEXT>().Effect(new Vector3(Rep.GetTargetTile().transform.position.x, Rep.GetTargetTile().transform.position.y + 2), SkinEffectScore);
		}
    }


    private void ReadyMeteor()
    {
        Vector2 RandPos;
        bool IsPrediction = GameManager.Instance.GetReplica().GetComponent<Adapt_SensitiveEar>().CheckPrediction();
        for (int i = 0; i < 3; i++)
        {
            while (true)
            {
                RandPos.x = Random.Range(0, 4);
                RandPos.y = Random.Range(0, 4);
                int Count = 0;
                for (int j = 0; j < 3; j++)
                {
                    if (RandPos == mMeteorSpawnPos[j])
                    {
                        Count++;
                    }
                    if (GameManager.Instance.GetTile(RandPos).GetObjectOnTile() != null)
                    {
                        if (GameManager.Instance.GetTile(RandPos).GetObjectOnTile().CompareTag("Rock"))
                            Count++;
                    }
                }
                if (Count == 0)
                {
                    mMeteorSpawnPos[i] = RandPos;
                    break;
                }
            }
            if (IsPrediction)
            {
                GameObject ObjTemp = ObjectManager.Instance.CreateOutTile(EnumObjects.MeteorView);
                ObjTemp.transform.position = GameManager.Instance.GetTile(mMeteorSpawnPos[i]).transform.position;
                mDisasterPredictionView.Add(ObjTemp);
                ObjTemp.SetActive(true);
				SoundManager.Instance.PlaySound("Sfx_SensitiveEar");
			}
        }
    }
	private void SpawnMeteor()
    {
        for(int i = 0;i<mDisasterPredictionView.Count;i++)
        {
            mDisasterPredictionView[i].SetActive(false);
        }
        for(int i = 0;i<3;i++)
        {
            CreateMeteor(GameManager.Instance.GetTile(mMeteorSpawnPos[i]));
        }
            mDisasterPredictionView.Clear();
    }

    private void CreateMeteor(Tile _Tile)
    {
        GameObject ObjTemp = _Tile.GetObjectOnTile();
        Meteor MeteorTemp = ObjectManager.Instance.CreateOutTile(EnumObjects.Meteor).GetComponent<Meteor>();
        MeteorTemp.transform.position = _Tile.transform.position;
        MeteorTemp.MeteorEffect.transform.localPosition = new Vector3(0, 10);
        if (ObjTemp != null)
        {
            MeteorTemp.SetDestroyObject(ObjTemp.GetComponent<Object>());
        }
    }

    IEnumerator CR_StartProduction(float _Time)
    {
        float CurTime = _Time;
        while(CurTime > 0)
        {
            GameManager.Instance.SetSlideAble(false);
            CurTime -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        GameManager.Instance.SetSlideAble(true);
        yield break;
    }

	public bool GetIsDisasterHappened()
	{
		return mIsDisasterHappened;
	}
}
