using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Object
{
	private float mExp;
	private float mScore;
	[SerializeField]
	private Sprite mSprite_Normal;
	[SerializeField]
	private Sprite mSprite_Special;

	protected override void OnEnable()
	{
		base.OnEnable();
		mExp = 10;
		mScore = 10 + GameManager.Instance.GetReplica().GetComponent<Replica>().SkinEffect("OnEatFood", 0) + DataManager.Instance.Mileage;
		GetComponent<SpriteRenderer>().sprite = mSprite_Normal;
	}

	public override void OnCollision(GameObject _Other, ref Tile _TargetTile, out bool _CheckOneMoreTime)
	{
		_CheckOneMoreTime = false;
		switch (_Other.tag)
		{
			case "Replica":
				if (_Other.GetComponent<Replica>().GetIsAteSomething() != 0)
				{
					SetDestroy();
					_Other.GetComponent<Replica>().SetIsAteSomething(_Other.GetComponent<Replica>().GetIsAteSomething() - 1);
                    _Other.GetComponent<Replica>().AddExp(GameManager.Instance.GetReplica().GetComponent<Adapt_Growing>().PlusExp(mExp));
					//GameManager.Instance.AddTotalScore(mScore);
					_Other.GetComponent<Replica>().GetComboSystem().Extend(mScore);
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

	public override void SetDestroy(bool _Destroyed = true)
	{
		base.SetDestroy(_Destroyed);
		GameManager.Instance.DecreaseObjectCount(EnumObjects.Food);
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

	public void SetSpecial()
	{
		mExp = 40;
		mScore = (10 + GameManager.Instance.GetReplica().GetComponent<Replica>().SkinEffect("OnEatFood", 0) + DataManager.Instance.Mileage) * 3;
		GetComponent<SpriteRenderer>().sprite = mSprite_Special;
	}

	public float GetExp()
	{
		return mExp;
	}
	
	public float GetScore()
	{
		return mScore;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (GetDestroy())
		{
			GetComponent<SpriteRenderer>().enabled = false;
			TutorialManager.Instance.AddProgression(1);
		}
	}
}
