using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

//이 스크립트를 사용하려면 Replica_Combo스크립트가 부착되어있어야한다.
[RequireComponent(typeof(Replica_Combo))]
public class Replica : Object
{
	private float mExp;
	private float mMaxExp;
	private int mLevel;
	private int mIsAteSomething;
    [SerializeField]
    public List<Adapt>[] mAdapts;
	private Image mLevelIndicatorGauge;
	private Text mLevelIndicatorNumber;
    [SerializeField]
    private GameObject[] mParts;
    private SpriteRenderer mSpriteRenderer;
	[SerializeField]
	private GameObject mGameClearText;
    [SerializeField]
    private GameObject mGameOverText;

	private Replica_Combo m_Combo;
	private string mSkin;
	private int mSkinLevel = 4;

	private void Awake()
    {
		mSpriteRenderer = GetComponent<SpriteRenderer>();
		mSkin = DataManager.Instance.GetData("Replica_Skin", "Core-idle");
		mSpriteRenderer.sprite = DataManager.Instance.GetItemSprite(mSkin);

		mLevelIndicatorGauge = GameObject.Find("LevelIndicator").GetComponent<Image>();
		mLevelIndicatorNumber = GameObject.Find("Number").GetComponent<Text>();

        mAdapts = new List<Adapt>[(int)EnumParts.End];
        for (int i = 0; i < (int)EnumParts.End; i++)
        {
            mAdapts[i] = new List<Adapt>();
        }
        Adapt newAdapt = gameObject.GetComponent<Adapt_Growing>();
        newAdapt.Init();
        mAdapts[(int)newAdapt.GetEnumParts()].Add(newAdapt);

        newAdapt = gameObject.GetComponent<Adapt_Cadaver>();
        newAdapt.Init();
        mAdapts[(int)newAdapt.GetEnumParts()].Add(newAdapt);

        newAdapt = gameObject.GetComponent<Adapt_Fish>();
        newAdapt.Init();
        mAdapts[(int)newAdapt.GetEnumParts()].Add(newAdapt);

        newAdapt = gameObject.GetComponent<Adapt_Appetite>();
        newAdapt.Init();
        mAdapts[(int)newAdapt.GetEnumParts()].Add(newAdapt);

		newAdapt = gameObject.GetComponent<Adapt_Lucky>();
        newAdapt.Init();
        mAdapts[(int)newAdapt.GetEnumParts()].Add(newAdapt);

		newAdapt = gameObject.GetComponent<Adapt_Threat>();
        newAdapt.Init();
        mAdapts[(int)newAdapt.GetEnumParts()].Add(newAdapt);

        newAdapt = gameObject.GetComponent<Adapt_River>();
        newAdapt.Init();
        mAdapts[(int)newAdapt.GetEnumParts()].Add(newAdapt);

        newAdapt = gameObject.GetComponent<Adapt_SensitiveNerve>();
        newAdapt.Init();
        mAdapts[(int)newAdapt.GetEnumParts()].Add(newAdapt);

        newAdapt = gameObject.GetComponent<Adapt_SensitiveEar>();
        newAdapt.Init();
        mAdapts[(int)newAdapt.GetEnumParts()].Add(newAdapt);
    }

    private void Start()
    {
		m_Combo = GetComponent<Replica_Combo>();
	}
    protected override void OnEnable()
	{
		base.OnEnable();
		mExp = 0;
		mMaxExp = 40 + (GameManager.GetRound() - 1) * 30;
		mLevel = 1;
        mLevelIndicatorNumber.text = mLevel.ToString();
    }

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
            mExp = mMaxExp;
            AddLevel();
		}
	}

	public override void OnCollision(GameObject _Other, ref Tile _TargetTile, out bool _CheckOneMoreTime)
	{
		_CheckOneMoreTime = false;
		if (gameObject == _Other)
			return;

		switch (_Other.tag)
		{
			case "Rock":
				_TargetTile = GameManager.Instance.GetTile(_TargetTile.GetIndexInArray() - InputManager.Instance.GetSlideDirectionVector());

				break;

			case "Food":
                AddExp(GetComponent<Adapt_Growing>().PlusExp(_Other.GetComponent<Food>().GetExp()));
				_Other.GetComponent<Object>().SetDestroy();
				//GameManager.Instance.AddTotalScore(_Other.GetComponent<Food>().GetScore());
				m_Combo.Extend(_Other.GetComponent<Food>().GetScore());
				mIsAteSomething--;
				if(mIsAteSomething != 0)
				{
					_CheckOneMoreTime = true;
				}
				if(_Other.GetComponent<Object>().GetFlood() != null)
				{
					mFlood = _Other.GetComponent<Object>().GetFlood();
				}
				break;

			case "Flood":
				mFlood = _Other.GetComponent<Flood>();
				if(GetComponent<Adapt_Fish>().CheckTier())
				{
					_CheckOneMoreTime = true;
				}
                break;

			case "Predator":
				if(Random.Range(0f, 1f) <= GetComponent<Adapt_Threat>().GetValue())
				{
					_Other.GetComponent<Predator>().AddLevel(-1);
                    GameObject Effect = ObjectManager.Instance.CreateOutTile(EnumObjects.Threat_Effect);
                    Effect.transform.position = mTargetTile.transform.position;
					SoundManager.Instance.PlaySound("Sfx_Threat");
				}
				if (_Other.GetComponent<Predator>().GetLevel() > mLevel)
				{
					SetDestroy();
				}
				else if (_Other.GetComponent<Predator>().GetLevel() < mLevel)
				{
					AddExp(_Other.GetComponent<Predator>().GetLevel() * 10);
					//GameManager.Instance.AddTotalScore(_Other.GetComponent<Predator>().GetLevel() * 5);
					m_Combo.Extend(_Other.GetComponent<Predator>().GetLevel() * (10 + DataManager.Instance.Mileage));
					_Other.GetComponent<Predator>().SetDestroy();
					if (_Other.GetComponent<Object>().GetFlood() != null)
					{
						mFlood = _Other.GetComponent<Object>().GetFlood();
					}
				}
				else
				{
					_TargetTile = GameManager.Instance.GetTile(_TargetTile.GetIndexInArray() - InputManager.Instance.GetSlideDirectionVector());
				}
				break;
		}
	}

	public override bool OnSlide(InputManager.SlideDirection _Direction, int _Distance = 5)
	{
		mIsAteSomething = (int)GetComponent<Adapt_Appetite>().GetValue();
        GetComponent<Adapt_Lucky>().OnSlide();

		bool hasTileSlided = false;
		if (mFlood != null)
		{
			mIsAteSomething = 1;
			Tile CurTile = mTile;
			Flood CurFlood = mFlood;
			mFlood = null;
			hasTileSlided = base.OnSlide(_Direction, GetComponent<Adapt_Fish>().CheckTier() ? 5 : 1);
			if (mTile != CurTile)
			{
				CurFlood.SetTile(CurTile);
			}
			else
			{
				mFlood = CurFlood;
			}
		}
		else
		{
			hasTileSlided = base.OnSlide(_Direction, _Distance);
		}
		if (hasTileSlided == false)
		{
			//플레이어 오브젝트에게만 이동 불가 연출
			//StartCoroutine(SlideFailAnimation(_Direction, 0.4f));

			
		}
		return hasTileSlided;
	}
	IEnumerator SlideFailAnimation(InputManager.SlideDirection dir, float duration)
	{
		InputManager.Instance.SetIsIgnoreInput(true);

		Vector3 originPos = transform.position;
		Vector3 endPos = transform.position;

		Vector3 dirVec = Vector2.zero;
		switch (dir) {
			case InputManager.SlideDirection.Right:
				dirVec.x = 1;
				break;
			case InputManager.SlideDirection.Left:
				dirVec.x = -1;
				break;
			case InputManager.SlideDirection.Up:
				dirVec.y = 1;
				break;
			case InputManager.SlideDirection.Down:
				dirVec.y = -1;
				break;

		}
		endPos += dirVec * 10;

        float t = 0.01f;
        while (t < duration)
        {
            t += Time.deltaTime;
			transform.position += Mathf.Sin(180 / t) * dirVec * Time.deltaTime;
            yield return null;
        }

        InputManager.Instance.SetIsIgnoreInput(false);
		yield break;
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (GetDestroy())
		{
			GetComponent<SpriteRenderer>().enabled = false;
		}
	}

    public void AddExp(float _Exp)
    {
        SoundManager.Instance.PlaySound("Sfx_Eat");
        mExp += _Exp;
        mLevelIndicatorGauge.fillAmount = mExp / mMaxExp;
        GameObject Temp = ObjectManager.Instance.CreateInCanvas(EnumObjects.PlusEXP);
        Temp.GetComponent<EXPTEXT>().Effect(new Vector3(mTargetTile.transform.position.x,mTargetTile.transform.position.y + 1),_Exp);
		//GameManager.Instance.AddTotalScore(_Exp);
    }

	public float GetExp()
	{
		return mExp;
	}

	public void SetLevel(int _Level)
	{
		mLevel = _Level;
        mLevelIndicatorNumber.text = mLevel.ToString();
    }

    public void AddLevel()
    {
		int[] RequiredExp = new int[11] { 40, 60, 100, 160, 220, 280, 400, 520, 640, 800, 960 };
		if (mExp >= (RequiredExp[mLevel - 1] + (GameManager.GetRound() - 1) * mLevel * 10))
		{
			if (mLevel == 10)
			{
				StartCoroutine(CR_NextRound());
			}
			else if(GetDestroy() == false)
			{
				SoundManager.Instance.PlaySound("Sfx_LevelUp");
				mExp -= (RequiredExp[mLevel - 1] + (GameManager.GetRound() - 1) * 30);
				mLevel++;
				mLevelIndicatorNumber.text = mLevel.ToString();
				mMaxExp = (RequiredExp[mLevel - 1] + (GameManager.GetRound() - 1) * 30);
				mLevelIndicatorGauge.fillAmount = mExp / mMaxExp;
                mLevelIndicatorGauge.rectTransform.localScale = new Vector3(1, 1, 1);
                mLevelIndicatorGauge.rectTransform.DOScale(new Vector3(2, 2), 0.5f);
				GameManager.Instance.GetButtonManager().CreateButton();
				GameObject LevelUpEffect = ObjectManager.Instance.CreateOutTile(EnumObjects.LevelUpEffect);
				LevelUpEffect.transform.position = mLevelIndicatorGauge.transform.position;

				//프레데터 레벨 색깔 업데이트
				foreach (GameObject predator in ObjectManager.Instance.GetObjects(EnumObjects.Predator))
				{
					predator.GetComponent<Predator>().LevelColoring();
				}
			}
		}
	}

    public int GetLevel()
	{
		return mLevel;
	}

	public void SetIsAteSomething(int _IsAteSomething)
	{
		mIsAteSomething = _IsAteSomething;
	}

	public int GetIsAteSomething()
	{
		return mIsAteSomething;
	}

    public List<Adapt> GetAdapts(EnumParts _Parts)
    {
        return mAdapts[(int)_Parts];
    }

    public override void Deactivate()
    {
        base.Deactivate();
        GameManager.Instance.SetSlideAble(false);
    }

    public SpriteRenderer GetSlimeSprite()
    {
        return mSpriteRenderer;
    }

	public Replica_Combo GetComboSystem() {
		return m_Combo;
	}
    public override void CreateAnimation(Vector3 _TargetScale)
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 1) * _TargetScale.x;
        transform.DOScale(_TargetScale, 0.3f).OnComplete(() =>
        {
            IdleAnimation(_TargetScale);
        });
    }

    public void StartParts(string _AnimTriggerKey)
    {
        for(int i = 0;i<mParts.Length;i++)
        {
            mParts[i].SetActive(true);
            mParts[i].GetComponent<Animator>().SetTrigger(_AnimTriggerKey);
        }
    }

	IEnumerator CR_NextRound()
	{
		GameManager.Instance.SetInOnAdaptSelect(true);
		GameObject Temp = ObjectManager.Instance.CreateOutTile(EnumObjects.Resurrection);
		Temp.transform.position = transform.position + new Vector3(0, -0.75f, 0);

		yield return new WaitForSeconds(2.5f);

		while(mLevel != 1)
		{
			mLevel--;
			mLevelIndicatorNumber.text = mLevel.ToString();
			yield return new WaitForSeconds(0.2f);
		}
		Temp = ObjectManager.Instance.CreateOutTile(EnumObjects.LevelUpEffect);
		Temp.transform.position = mLevelIndicatorGauge.transform.position;

		yield return new WaitForSeconds(1);

		Temp = ObjectManager.Instance.CreateInCanvas(EnumObjects.PlusEXP);
		Temp.GetComponent<EXPTEXT>().Effect(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1), 1000 * GameManager.GetRound());
		GameManager.Instance.AddTotalScore(1000 * GameManager.GetRound());

		Temp = ObjectManager.Instance.CreateOutTile(EnumObjects.HitPow);
		Temp.transform.position = GameManager.Instance.GetRank().transform.position;

		yield return new WaitForSeconds(1);

		if (GameManager.GetRound() <= 3)
		{
			GameManager.Instance.SetRound(GameManager.GetRound() + 1);
		}

		GameObject Fade = GameObject.Find("Black Mask");
		Fade.GetComponent<SpriteRenderer>().color = Color.white;
		Fade.GetComponent<FadeInFadeOut>().FadeOut("Ingame", 2);
	}

	public void SetSkinOfRankingData(RankingExtraData _Data)
	{
		_Data.Skin = mSpriteRenderer.sprite.name;
	}

	public float SkinEffect(string _Key, in float _Value)
	{
		switch(_Key)
		{
			case "OnSlide": 
				if (mSkin.Equals("Item_Rare_Indian")) //n번째 슬라이드마다 콤보 +1
				{
					if ((int)_Value % new int[] { 6, 5, 4, 3 }[mSkinLevel - 1] == 0)
						return 1;
					return 0;
				}
				return 0;

			case "OnGameOverGold":
				if (mSkin.Equals("Item_Rare_Mario")) //게임 종료시 골드 n%(n = 5의 배수)추가 획득
				{
					return (int)(_Value * new float[] { 0.05f, 0.1f, 0.15f, 0.2f }[mSkinLevel - 1]);
				}
				return 0;

			case "OnGameOverScore":
				if (mSkin.Equals("Item_Rare_Mouse")) //게임 종료시 점수 n% 추가 획득
				{
					return (int)(_Value * new float[] { 0.05f, 0.065f, 0.08f, 0.1f }[mSkinLevel - 1]);
				}
				return 0;

			case "OnEarthquake":
				if (mSkin.Equals("Item_Legend_Catbot")) //지진 발생시 n점 획득
				{
					return new float[] { 100, 150, 200, 250 }[mSkinLevel - 1];
				}
				return 0;

			case "OnEatFood":
				if (mSkin.Equals("Item_Legend_Kaito")) //먹이 먹으면 n점(특별한 먹이는 n*3점) 획득
				{
					return new float[] { 5, 10, 15, 20 }[mSkinLevel - 1];
				}
				return 0;

			case "OnComboLost":
				if (mSkin.Equals("Item_Legend_Fishbowl")) //콤보 끊기면 n% 확률로 콤보 유지
				{
					return new float[] { 0.1f, 0.2f, 0.3f, 0.4f }[mSkinLevel - 1];
				}
				return 0;
		}

		return 0;
	}
}