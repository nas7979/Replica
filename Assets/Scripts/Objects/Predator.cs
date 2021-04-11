using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Predator : Object
{
	[InspectorName("레플리카보다 낮은 레벨일 때")]
	public Color GreenLevelColor;
	[InspectorName("레플리카와 같은 레벨")]
	public Color BlackLevelColor;
	[InspectorName("레플리카보다 높은 레벨")]
	public Color RedLevelColor;

	private int mLevel;
    [SerializeField]
    private Text mLevelText;
	[SerializeField]
	private Outline mOutLineLevelText;

	[SerializeField]
	private Sprite[] mSprites = new Sprite[3];

	protected override void OnEnable()
	{
		base.OnEnable();
		int ReplicaLevel = GameManager.Instance.GetReplica().GetComponent<Replica>().GetLevel();
		mLevel = Random.Range(ReplicaLevel, ReplicaLevel + GameManager.Instance.GetPredatorLevel());
        mLevelText.text = mLevel.ToString();
		LevelColoring();
		GetComponent<SpriteRenderer>().sprite = mSprites[Random.Range(0, 3)];
    }

    public override void SetDestroy(bool _Destroyed = true)
    {
        base.SetDestroy(_Destroyed);
        GameManager.Instance.DecreaseObjectCount(EnumObjects.Predator);
    }

    public override void OnCollision(GameObject _Other, ref Tile _TargetTile, out bool _CheckOneMoreTime)
	{
		_CheckOneMoreTime = false;
		switch (_Other.tag)
		{
			case "Rock":
				_TargetTile = GameManager.Instance.GetTile(_TargetTile.GetIndexInArray() - InputManager.Instance.GetSlideDirectionVector());
				break;

			case "Replica":
				if (Random.Range(0f, 1f) <= GameManager.Instance.GetReplica().GetComponent<Adapt_Threat>().GetValue())
				{
					AddLevel(-1);
                    GameObject Effect = ObjectManager.Instance.CreateOutTile(EnumObjects.Threat_Effect);
                    Effect.transform.position = mTargetTile.transform.position;
                    SoundManager.Instance.PlaySound("Sfx_Threat");
				}
				if (_Other.GetComponent<Replica>().GetLevel() > mLevel)
				{
					SetDestroy();
					_Other.GetComponent<Replica>().AddExp(mLevel * 10);
					//GameManager.Instance.AddTotalScore(mLevel * 5);
					_Other.GetComponent<Replica>().GetComboSystem().Extend(mLevel * (10 + DataManager.Instance.Mileage));
				}
				else if(_Other.GetComponent<Replica>().GetLevel() < mLevel)
				{
					_Other.GetComponent<Replica>().SetDestroy();
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

			case "Flood":
				mFlood = _Other.GetComponent<Flood>();
                break;

			default:
				_TargetTile = GameManager.Instance.GetTile(_TargetTile.GetIndexInArray() - InputManager.Instance.GetSlideDirectionVector());
				break;
		}
	}

	public override bool OnSlide(InputManager.SlideDirection _Direction, int _Distance = 5)
	{
		bool hasTileSlided = false;
		if (mFlood != null)
		{
			Tile CurTile = mTile;
			Flood CurFlood = mFlood;
			mFlood = null;
			hasTileSlided = base.OnSlide(_Direction, 1);
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
		return hasTileSlided;
	}

    public override void Deactivate()
    {
        base.Deactivate();
        GameManager.Instance.GetReplica().GetComponent<Adapt_Cadaver>().PlusExp(mLevel);
        if(GameManager.Instance.GetReplica().GetComponent<Adapt_Cadaver>().GetValue() != 0)
		{
			SoundManager.Instance.PlaySound("Sfx_Cadaver");
		}
    }

    private void OnTriggerEnter2D(Collider2D collision)
	{
		if (GetDestroy())
		{
			GetComponent<SpriteRenderer>().enabled = false;
			TutorialManager.Instance.AddProgression(3);
		}
	}

	public void SetLevel(int _Level)
	{
		mLevel = _Level;
		mLevelText.text = mLevel.ToString();
		LevelColoring();
	}

	public void LevelColoring()
	{
		int replicaLevel = GameManager.Instance.GetReplica().GetComponent<Replica>().GetLevel();
		if (replicaLevel > mLevel)
			mOutLineLevelText.effectColor = GreenLevelColor;
		else if (replicaLevel == mLevel)
			mOutLineLevelText.effectColor = BlackLevelColor;
		else if (replicaLevel < mLevel)
			mOutLineLevelText.effectColor = RedLevelColor;
	}

	public void AddLevel(int _Level)
	{
		mLevel += _Level;
		mLevelText.text = mLevel.ToString();
	}

	public int GetLevel()
	{
		return mLevel;
	}

    public override void CreateAnimation(Vector3 _TargetScale)
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 1) * _TargetScale.x;
        transform.DOScale(_TargetScale, 0.3f).OnComplete(() =>
        {
            IdleAnimation(_TargetScale);
        });
    }
}
