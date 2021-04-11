using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : Singleton<GameManager>
{

	private static int mRound = 1;
	private Tile[,] mTiles = new Tile[5, 5];
    private DisasterManager mDisasterManager;
    [SerializeField]
    private ButtonManager mButtonManager;
	private List<GameObject> mObjects = new List<GameObject>();
	private bool mIsSlideAble = false;
	private bool mIsOnIngame = false;
	private int[] mObjectCount = new int[(int)EnumObjects.End];
    private int[,,] mObjectCountMax = new int[4, 11, (int)EnumObjects.End];
	private List<Tile> mEmptyTiles = new List<Tile>();
	private bool mIsOnAdaptSelect;
	private int mPredatorLevel;
    [SerializeField]
    private GameObject mReplica;
	[SerializeField]
	private GameObject mSettingWindow;
	[SerializeField]
	private GameObject mAdaptWindow;
    [SerializeField]
    private GameObject mAdaptInformation;
    [SerializeField]
    private CameraControl mMainCamera;
    [SerializeField]
    private GameObject mRocket;
    private static float mTotalScore = 0;
    [SerializeField]
    private Text mTotalScoreText;
    [SerializeField]
    private GameObject mAdaptListButtonEffect;
    [SerializeField]
    private GameObject mAdaptButton;
    [SerializeField]
    private GameObject mSettingButton;
	[SerializeField]
	private GameObject mRank;
	[SerializeField]
	private GameObject mEndingWindow;
	[SerializeField]
	private Sprite[] mRankSprite = new Sprite[4];



	void Start()
	{
		ObjectManager.Instance.SetCanvas(GameObject.Find("Canvas"));
		SoundManager.Instance.StopBGM();
		SoundManager.Instance.PlaySound("BGM_Ingame", true, true);
        mDisasterManager = GetComponent<DisasterManager>();
		mAdaptWindow.GetComponent<AdaptWindow>().Init();
		mIsOnAdaptSelect = false;
		mPredatorLevel = 3;

		int Round;
		//오브젝트 수 제한

		#region Round1
		Round = 0;

		mObjectCountMax[Round, 1, (int)EnumObjects.Food] = 10;
		mObjectCountMax[Round, 1, (int)EnumObjects.Rock] = 2;
		mObjectCountMax[Round, 1, (int)EnumObjects.Predator] = 0;

		mObjectCountMax[Round, 2, (int)EnumObjects.Food] = 10;
		mObjectCountMax[Round, 2, (int)EnumObjects.Rock] = 2;
		mObjectCountMax[Round, 2, (int)EnumObjects.Predator] = 1;

		mObjectCountMax[Round, 3, (int)EnumObjects.Food] = mObjectCountMax[Round, 4, (int)EnumObjects.Food] = 9;
		mObjectCountMax[Round, 3, (int)EnumObjects.Rock] = mObjectCountMax[Round, 4, (int)EnumObjects.Rock] = 2;
		mObjectCountMax[Round, 3, (int)EnumObjects.Predator] = mObjectCountMax[Round, 4, (int)EnumObjects.Predator] = 2;

		mObjectCountMax[Round, 5, (int)EnumObjects.Food] = mObjectCountMax[Round, 6, (int)EnumObjects.Food] = 8;
		mObjectCountMax[Round, 5, (int)EnumObjects.Rock] = mObjectCountMax[Round, 6, (int)EnumObjects.Rock] = 2;
		mObjectCountMax[Round, 5, (int)EnumObjects.Predator] = mObjectCountMax[Round, 6, (int)EnumObjects.Predator] = 3;

		mObjectCountMax[Round, 7, (int)EnumObjects.Food] = mObjectCountMax[Round, 8, (int)EnumObjects.Food] = 7;
		mObjectCountMax[Round, 7, (int)EnumObjects.Rock] = mObjectCountMax[Round, 8, (int)EnumObjects.Rock] = 3;
		mObjectCountMax[Round, 7, (int)EnumObjects.Predator] = mObjectCountMax[Round, 8, (int)EnumObjects.Predator] = 4;

		mObjectCountMax[Round, 9, (int)EnumObjects.Food] = mObjectCountMax[Round, 10, (int)EnumObjects.Food] = 6;
		mObjectCountMax[Round, 9, (int)EnumObjects.Rock] = mObjectCountMax[Round, 10, (int)EnumObjects.Rock] = 3;
		mObjectCountMax[Round, 9, (int)EnumObjects.Predator] = mObjectCountMax[Round, 10, (int)EnumObjects.Predator] = 5;
		#endregion

		#region Round2
		Round = 1;

		mObjectCountMax[Round, 1, (int)EnumObjects.Food] = 9;
		mObjectCountMax[Round, 1, (int)EnumObjects.Rock] = 2;
		mObjectCountMax[Round, 1, (int)EnumObjects.Predator] = 1;

		mObjectCountMax[Round, 2, (int)EnumObjects.Food] = 9;
		mObjectCountMax[Round, 2, (int)EnumObjects.Rock] = 2;
		mObjectCountMax[Round, 2, (int)EnumObjects.Predator] = 2;

		mObjectCountMax[Round, 3, (int)EnumObjects.Food] = mObjectCountMax[Round, 4, (int)EnumObjects.Food] = 8;
		mObjectCountMax[Round, 3, (int)EnumObjects.Rock] = mObjectCountMax[Round, 4, (int)EnumObjects.Rock] = 2;
		mObjectCountMax[Round, 3, (int)EnumObjects.Predator] = mObjectCountMax[Round, 4, (int)EnumObjects.Predator] = 3;

		mObjectCountMax[Round, 5, (int)EnumObjects.Food] = mObjectCountMax[Round, 6, (int)EnumObjects.Food] = 7;
		mObjectCountMax[Round, 5, (int)EnumObjects.Rock] = mObjectCountMax[Round, 6, (int)EnumObjects.Rock] = 2;
		mObjectCountMax[Round, 5, (int)EnumObjects.Predator] = mObjectCountMax[Round, 6, (int)EnumObjects.Predator] = 4;

		mObjectCountMax[Round, 7, (int)EnumObjects.Food] = mObjectCountMax[Round, 8, (int)EnumObjects.Food] = 6;
		mObjectCountMax[Round, 7, (int)EnumObjects.Rock] = mObjectCountMax[Round, 8, (int)EnumObjects.Rock] = 3;
		mObjectCountMax[Round, 7, (int)EnumObjects.Predator] = mObjectCountMax[Round, 8, (int)EnumObjects.Predator] = 5;

		mObjectCountMax[Round, 9, (int)EnumObjects.Food] = mObjectCountMax[Round, 10, (int)EnumObjects.Food] = 5;
		mObjectCountMax[Round, 9, (int)EnumObjects.Rock] = mObjectCountMax[Round, 10, (int)EnumObjects.Rock] = 3;
		mObjectCountMax[Round, 9, (int)EnumObjects.Predator] = mObjectCountMax[Round, 10, (int)EnumObjects.Predator] = 5;
		#endregion

		#region Round3
		Round = 2;

		mObjectCountMax[Round, 1, (int)EnumObjects.Food] = 8;
		mObjectCountMax[Round, 1, (int)EnumObjects.Rock] = 3;
		mObjectCountMax[Round, 1, (int)EnumObjects.Predator] = 2;

		mObjectCountMax[Round, 2, (int)EnumObjects.Food] = 7;
		mObjectCountMax[Round, 2, (int)EnumObjects.Rock] = 3;
		mObjectCountMax[Round, 2, (int)EnumObjects.Predator] = 3;

		mObjectCountMax[Round, 3, (int)EnumObjects.Food] = mObjectCountMax[Round, 4, (int)EnumObjects.Food] = 6;
		mObjectCountMax[Round, 3, (int)EnumObjects.Rock] = mObjectCountMax[Round, 4, (int)EnumObjects.Rock] = 3;
		mObjectCountMax[Round, 3, (int)EnumObjects.Predator] = mObjectCountMax[Round, 4, (int)EnumObjects.Predator] = 4;

		mObjectCountMax[Round, 5, (int)EnumObjects.Food] = mObjectCountMax[Round, 6, (int)EnumObjects.Food] = 5;
		mObjectCountMax[Round, 5, (int)EnumObjects.Rock] = mObjectCountMax[Round, 6, (int)EnumObjects.Rock] = 3;
		mObjectCountMax[Round, 5, (int)EnumObjects.Predator] = mObjectCountMax[Round, 6, (int)EnumObjects.Predator] = 5;

		mObjectCountMax[Round, 7, (int)EnumObjects.Food] = mObjectCountMax[Round, 8, (int)EnumObjects.Food] = 4;
		mObjectCountMax[Round, 7, (int)EnumObjects.Rock] = mObjectCountMax[Round, 8, (int)EnumObjects.Rock] = 4;
		mObjectCountMax[Round, 7, (int)EnumObjects.Predator] = mObjectCountMax[Round, 8, (int)EnumObjects.Predator] = 5;

		mObjectCountMax[Round, 9, (int)EnumObjects.Food] = mObjectCountMax[Round, 10, (int)EnumObjects.Food] = 3;
		mObjectCountMax[Round, 9, (int)EnumObjects.Rock] = mObjectCountMax[Round, 10, (int)EnumObjects.Rock] = 4;
		mObjectCountMax[Round, 9, (int)EnumObjects.Predator] = mObjectCountMax[Round, 10, (int)EnumObjects.Predator] = 5;
		#endregion

		#region Round4
		Round = 3;

		mObjectCountMax[Round, 1, (int)EnumObjects.Food] = 7;
		mObjectCountMax[Round, 1, (int)EnumObjects.Rock] = 4;
		mObjectCountMax[Round, 1, (int)EnumObjects.Predator] = 3;

		mObjectCountMax[Round, 2, (int)EnumObjects.Food] = 6;
		mObjectCountMax[Round, 2, (int)EnumObjects.Rock] = 4;
		mObjectCountMax[Round, 2, (int)EnumObjects.Predator] = 4;

		mObjectCountMax[Round, 3, (int)EnumObjects.Food] = mObjectCountMax[Round, 4, (int)EnumObjects.Food] = 5;
		mObjectCountMax[Round, 3, (int)EnumObjects.Rock] = mObjectCountMax[Round, 4, (int)EnumObjects.Rock] = 4;
		mObjectCountMax[Round, 3, (int)EnumObjects.Predator] = mObjectCountMax[Round, 4, (int)EnumObjects.Predator] = 5;

		mObjectCountMax[Round, 5, (int)EnumObjects.Food] = mObjectCountMax[Round, 6, (int)EnumObjects.Food] = 4;
		mObjectCountMax[Round, 5, (int)EnumObjects.Rock] = mObjectCountMax[Round, 6, (int)EnumObjects.Rock] = 4;
		mObjectCountMax[Round, 5, (int)EnumObjects.Predator] = mObjectCountMax[Round, 6, (int)EnumObjects.Predator] = 5;

		mObjectCountMax[Round, 7, (int)EnumObjects.Food] = mObjectCountMax[Round, 8, (int)EnumObjects.Food] = 3;
		mObjectCountMax[Round, 7, (int)EnumObjects.Rock] = mObjectCountMax[Round, 8, (int)EnumObjects.Rock] = 5;
		mObjectCountMax[Round, 7, (int)EnumObjects.Predator] = mObjectCountMax[Round, 8, (int)EnumObjects.Predator] = 6;

		mObjectCountMax[Round, 9, (int)EnumObjects.Food] = mObjectCountMax[Round, 10, (int)EnumObjects.Food] = 2;
		mObjectCountMax[Round, 9, (int)EnumObjects.Rock] = mObjectCountMax[Round, 10, (int)EnumObjects.Rock] = 5;
		mObjectCountMax[Round, 9, (int)EnumObjects.Predator] = mObjectCountMax[Round, 10, (int)EnumObjects.Predator] = 7;
		#endregion

		GameStart();
	}

	// Update is called once per frame
	void Update()
	{
		if (mIsSlideAble == true && mIsOnAdaptSelect == false)
		{
			InputManager.SlideDirection SlideDirectionTemp = InputManager.Instance.GetSlideDirection();
			GameObject Temp = null;

			if (SlideDirectionTemp != InputManager.SlideDirection.None)
			{
				bool hasAnyTileSlided = false;

				switch (SlideDirectionTemp)
				{
					case InputManager.SlideDirection.Right:
						for (int i = 0; i < 5; i++)
						{
							for (int j = 4; j >= 0; j--)
							{
								Temp = mTiles[j, i].GetComponent<Tile>().GetObjectOnTile();
								if (Temp != null && Temp.GetComponent<Object>().GetDestroy() == false)
								{
									//타일 이동에 대해 true 반환을 받아 하나라도 움직였다면 NEXT Slide 코루틴이 실행되게 한다.
									if (Temp.GetComponent<Object>().OnSlide(SlideDirectionTemp) == true)
										hasAnyTileSlided = true;
								}
							}
						}
						break;

					case InputManager.SlideDirection.Left:
					case InputManager.SlideDirection.Up:
						for (int i = 4; i >= 0; i--)
						{
							for (int j = 0; j < 5; j++)
							{
								Temp = mTiles[j, i].GetComponent<Tile>().GetObjectOnTile();
								if (Temp != null && Temp.GetComponent<Object>().GetDestroy() == false)
								{
									if (Temp.GetComponent<Object>().OnSlide(SlideDirectionTemp) == true)
										hasAnyTileSlided = true;
								}
							}
						}
						break;

					case InputManager.SlideDirection.Down:
						for (int i = 0; i < 5; i++)
						{
							for (int j = 0; j < 5; j++)
							{
								Temp = mTiles[j, i].GetComponent<Tile>().GetObjectOnTile();
								if (Temp != null && Temp.GetComponent<Object>().GetDestroy() == false)
								{
									if (Temp.GetComponent<Object>().OnSlide(SlideDirectionTemp) == true)
										hasAnyTileSlided = true;
								}
							}
						}
						break;
				}
				if (hasAnyTileSlided == true)
				{
					SoundManager.Instance.PlaySound("Sfx_Slide");
					mIsSlideAble = false;

					//매 턴마다 콤보 업데이트(연장과 초기화 유무 검사)
					mReplica.GetComponent<Replica>().GetComboSystem().ComboSystemUpdate();

					StartCoroutine(CR_waitForNextSlide(SlideDirectionTemp));
				}
			}
		}
	}

	public Tile GetTile(Vector2 _Pos)
	{
		return mTiles[(int)_Pos.x, (int)_Pos.y];
	}

	public Tile CheckLine(Vector2 _StartTile, InputManager.SlideDirection _Direction, int _Distance = 5)
	{
		Vector2 Direction = InputManager.Instance.GetSlideDirectionVector();

		Tile TileTemp;
		bool IsObjectOnTile;
		Vector2 CurrentTilePos = _StartTile;
		for (int i = 0; i < _Distance; i++)
		{
			CurrentTilePos += Direction;

			if (CurrentTilePos.x == -1 || CurrentTilePos.x == 5 || CurrentTilePos.y == -1 || CurrentTilePos.y == 5)
			{
				return mTiles[(int)(CurrentTilePos - Direction).x, (int)(CurrentTilePos - Direction).y];
			}


			TileTemp = mTiles[(int)CurrentTilePos.x, (int)CurrentTilePos.y];
			IsObjectOnTile = TileTemp.GetObjectOnTile() != null;
			if (IsObjectOnTile == true)
			{
				return mTiles[(int)CurrentTilePos.x, (int)CurrentTilePos.y];
			}
		}

		return mTiles[(int)CurrentTilePos.x, (int)CurrentTilePos.y];
	}

	public void GameStart()
	{
		mIsSlideAble = true;
		mIsOnIngame = true;
		CreateTiles(new Vector2(0, -0.2f));
		mAdaptWindow.GetComponent<AdaptWindow>().Init();
		mTotalScoreText.text = ((int)mTotalScore).ToString();
		mRank.GetComponent<Image>().sprite = mRankSprite[mRound - 1];
		switch (mRound)
		{
			case 1:
				mRank.transform.GetChild(0).GetComponent<Text>().text = "행성 탐사원";
				break;
			case 2:
				mRank.transform.GetChild(0).GetComponent<Text>().text = "훌륭한 탐사원";
				break;
			case 3:
				mRank.transform.GetChild(0).GetComponent<Text>().text = "영웅적 탐사원";
				break;
			case 4:
				mRank.transform.GetChild(0).GetComponent<Text>().text = "전설적 탐사원";
				break;
		}

		GameObject Temp = ObjectManager.Instance.Create(EnumObjects.Replica, mTiles[2, 2]);
        mReplica = Temp;
        for (int i = 0; i < mObjectCountMax[mRound - 1, 1, (int)EnumObjects.Rock]; i++)
		{
			Temp = ObjectManager.Instance.Create(EnumObjects.Rock, GetRandomTile());
			mObjectCount[(int)EnumObjects.Rock]++;
		}
		for (int i = 0; i < mObjectCountMax[mRound - 1, 1, (int)EnumObjects.Food]; i++)
		{
			Temp = ObjectManager.Instance.Create(EnumObjects.Food, GetRandomTile());
			mObjectCount[(int)EnumObjects.Food]++;
		}
		for (int i = 0; i < mObjectCountMax[mRound - 1, 1, (int)EnumObjects.Predator]; i++)
		{
			Temp = ObjectManager.Instance.Create(EnumObjects.Predator, GetRandomTile());
			mObjectCount[(int)EnumObjects.Predator]++;
		}

		mRocket.SetActive(true);
		SoundManager.Instance.PlaySound("Sfx_Spaceship");
	}

	public void CreateTiles(Vector2 offset = new Vector2())
	{
		for (int i = -2; i <= 2; i++)
		{
			for (int j = -2; j <= 2; j++)
			{
				mTiles[j + 2, i + 2] = Instantiate(ObjectManager.Instance.GetObject(EnumObjects.Tile)).GetComponent<Tile>();
				mTiles[j + 2, i + 2].transform.position = new Vector2(j, i - (1.1f / 1.95f)) * 1.95f + offset;
				mTiles[j + 2, i + 2].GetComponent<Tile>().SetIndexInArray(new Vector2(j + 2, i + 2));
			}
		}
	}

	public IEnumerator CR_waitForNextSlide(InputManager.SlideDirection _Direction)
	{
		yield return new WaitForSeconds(0.2f);

		int Level = mReplica.GetComponent<Replica>().GetLevel();
		if (!(mObjectCount[(int)EnumObjects.Food] >= mObjectCountMax[mRound - 1, Level, (int)EnumObjects.Food] &&
			mObjectCount[(int)EnumObjects.Predator] >= mObjectCountMax[mRound - 1, Level, (int)EnumObjects.Predator] &&
			mObjectCount[(int)EnumObjects.Rock] >= mObjectCountMax[mRound - 1, Level, (int)EnumObjects.Rock]))
		{
			int Rand = Random.Range((int)EnumObjects.Predator, (int)EnumObjects.Rock + 1);
			while (mObjectCount[Rand] >= mObjectCountMax[mRound - 1, Level, Rand])
			{
				Rand = Random.Range((int)EnumObjects.Predator, (int)EnumObjects.Rock + 1);
			}
			mObjectCount[Rand]++;
			//Debug.Log(mObjectCount[Rand]);
			if ((EnumObjects)Rand == EnumObjects.Food && mReplica.GetComponent<Adapt_Lucky>().GetCount() <= 0)
			{
				Food FoodTemp = ObjectManager.Instance.Create((EnumObjects)Rand, GetRandomTileFromBorder(_Direction)).GetComponent<Food>();
				FoodTemp.SetSpecial();
				mReplica.GetComponent<Adapt_Lucky>().ResetCount();
			}
			else
			{
				ObjectManager.Instance.Create((EnumObjects)Rand, GetRandomTileFromBorder(_Direction));
			}
		}

		yield return new WaitForSeconds(0.3f);

		mDisasterManager.OnSlide();
		if(mDisasterManager.GetIsDisasterHappened() == true)
		{
			yield return new WaitForSeconds(1.5f);
		}

		mReplica.GetComponent<Replica>().AddLevel();

		yield return new WaitForSeconds(0.1f);

		if (mReplica.activeInHierarchy == true)
		{
			if (mIsOnAdaptSelect == false && mSettingWindow.activeInHierarchy == false && mAdaptWindow.activeInHierarchy == false)
			{
				mIsSlideAble = true;
			}
		}
		else
		{
			InputManager.Instance.SetIsIgnoreInput(true);
			CreateEndingWindow();
		}
	}

	public Tile GetRandomTile()
	{
		Vector2 Pos;
		do
		{
			Pos = new Vector2(Random.Range(0, 5), Random.Range(0, 5));
		} while (mTiles[(int)Pos.x, (int)Pos.y].GetComponent<Tile>().GetObjectOnTile() != null);
		return mTiles[(int)Pos.x, (int)Pos.y];
	}

	public Tile GetRandomTileFromBorder(InputManager.SlideDirection _Direction)
	{
		Tile Temp;
		mEmptyTiles.Clear();

		switch (_Direction)
		{
			case InputManager.SlideDirection.Up:
				for (int i = 0; i < 5; i++)
				{
					for (int j = 0; j < 5; j++)
					{
						Temp = mTiles[j, i];
						if (Temp.GetObjectOnTile() == null)
						{
							mEmptyTiles.Add(Temp);
						}
					}
					if (mEmptyTiles.Count != 0)
						break;
				}
				break;

			case InputManager.SlideDirection.Down:
				for (int i = 4; i >= 0; i--)
				{
					for (int j = 4; j >= 0; j--)
					{
						Temp = mTiles[j, i];
						if (Temp.GetObjectOnTile() == null)
						{
							mEmptyTiles.Add(Temp);
						}
					}
					if (mEmptyTiles.Count != 0)
						break;
				}
				break;

			case InputManager.SlideDirection.Right:
				for (int i = 0; i < 5; i++)
				{
					for (int j = 0; j < 5; j++)
					{
						Temp = mTiles[i, j];
						if (Temp.GetObjectOnTile() == null)
						{
							mEmptyTiles.Add(Temp);
						}
					}
					if (mEmptyTiles.Count != 0)
						break;
				}
				break;

			case InputManager.SlideDirection.Left:
				for (int i = 4; i >= 0; i--)
				{
					for (int j = 0; j < 5; j++)
					{
						Temp = mTiles[i, j];
						if (Temp.GetObjectOnTile() == null)
						{
							mEmptyTiles.Add(Temp);
						}
					}
					if (mEmptyTiles.Count != 0)
						break;
				}
				break;
		}

		return mEmptyTiles[Random.Range(0, mEmptyTiles.Count)];
	}

	public void DecreaseObjectCount(EnumObjects _Object)
	{
		mObjectCount[(int)_Object]--;
	}

    public int GetObjectCountMax(EnumObjects _EnumObject)
    {
        return mObjectCountMax[mRound - 1, mReplica.GetComponent<Replica>().GetLevel(), (int)_EnumObject];
    }

    public int GetObjectCount(EnumObjects _EnumObject)
    {
        return mObjectCount[(int)_EnumObject];
    }

	
    public GameObject GetReplica()
    {
        return mReplica;
    }

	public void SetSlideAble(bool _Able)
	{
		mIsSlideAble = _Able;
	}

	public void CreateSettingWindow()
	{
        if(mIsSlideAble == true)
        {
            mSettingButton.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 180), 0.5f);
            mSettingWindow.SetActive(true);
		mIsSlideAble = false;
		SoundManager.Instance.PlaySound("Sfx_Button");
        }
	}

	public void CreateAdaptWindow()
	{
            mAdaptButton.GetComponent<RectTransform>().DORotate(new Vector3(0,0,180), 0.5f);
            mAdaptListButtonEffect.SetActive(false);
            mAdaptWindow.SetActive(true);
            mIsSlideAble = false;
            SoundManager.Instance.PlaySound("Sfx_Button");
	}

	public ButtonManager GetButtonManager()
    {
        return mButtonManager;
    }

	public AdaptWindow GetAdaptWindow()
	{
		return mAdaptWindow.GetComponent<AdaptWindow>();
	}

	public void SetInOnAdaptSelect(bool _IsOnAdaptSelect)
	{
		mIsOnAdaptSelect = _IsOnAdaptSelect;
	}

	public bool GetInOnAdaptSelect()
	{
		return mIsOnAdaptSelect;
	}

    public CameraControl GetCameraControl()
    {
        return mMainCamera;
    }

    public IEnumerator SetAdaptInformation(string _Information, bool _Deactive)
    {
        mAdaptInformation.SetActive(false);
        mAdaptInformation.SetActive(true);
        mAdaptInformation.transform.GetChild(0).GetComponent<Text>().text = _Information;
        if (_Deactive)
        {
        yield return new WaitForSeconds(1f);
        mAdaptInformation.SetActive(false);
        }
        yield break;
    }

    public void AddTotalScore(float _Score)
    {
        mTotalScore += _Score;
        mTotalScoreText.text = ((int)mTotalScore).ToString();
    }
	
	

	public void SetTotalScore(float _Score)
	{
		mTotalScore = _Score;
	}

	public void SetObjectCountMax(EnumObjects _Object, int _Level, int _Count)
	{
		mObjectCountMax[mRound - 1, _Level, (int)_Object] = _Count;
	}

	public void SetPredatorLevel(int _Level)
	{
		mPredatorLevel = _Level;
	}

	public int GetPredatorLevel()
	{
		return mPredatorLevel;
	}
    public GameObject GetAdaptListButtonEffect()
    {
        return mAdaptListButtonEffect;
    }

    public GameObject GetAdaptButton()
    {
        return mAdaptButton;
    }

    public GameObject GetSettingButton()
    {
        return mSettingButton;
    }

	public static int GetRound()
	{
		return mRound;
	}

	public void SetRound(int _Round)
	{
		mRound = _Round;
	}

	public GameObject GetRank()
	{
		return mRank;
	}

	public static void Clear()
	{
		mRound = 1;
		mTotalScore = 0;
		DataManager.Instance.ClearAdaptCount();
	}

	public float GetTotalScore()
	{
		return mTotalScore;
	}
	

	public void CreateEndingWindow()
	{
		mEndingWindow.SetActive(true);
	}
}