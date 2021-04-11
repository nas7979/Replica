using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Object : MonoBehaviour
{
	public Tile mTile;
    protected Tile mTargetTile;
    protected Flood mFlood;
    protected EnumObjects mEnumTag;
	private bool mIsDestroyed;
    [SerializeField]
    protected Vector3 mScaleOffset = new Vector3(0.75f, 0.75f, 1);

    protected virtual void OnEnable()
	{
		mFlood = null;
        transform.localScale = mScaleOffset;
        CreateAnimation(transform.localScale);
	}

    private void OnDisable()
    {
        transform.DOKill();
    }

    public virtual void SetDestroy(bool _Destroyed = true)
	{
		mIsDestroyed = _Destroyed;
	}

    public void SetEnumTag(EnumObjects _EnumTag)
    {
        mEnumTag = _EnumTag;
    }

    public EnumObjects GetEnumtTag()
    {
        return mEnumTag;
    }

	public bool GetDestroy()
	{
		return mIsDestroyed;
	}

	public virtual void Deactivate()
	{
		gameObject.SetActive(false);
		if (mIsDestroyed == false && mTile.GetObjectOnTile() == gameObject)
		{
			mTile.GetComponent<Tile>().SetObjectOnTile(null);
		}
	}

	public abstract void OnCollision(GameObject _Other, ref Tile _TargetTile, out bool _CheckOneMoreTime);

	public virtual bool OnSlide(InputManager.SlideDirection _Direction, int _Distance = 5)
	{
		mTargetTile = null;
		GameObject ObjectOnTargetTile = null;
		bool CheckOneMoreTime = false;
        Vector2 prevIndex = mTile.GetIndexInArray();
        bool ReplicaComboLost = true;

        if (mFlood != null)
		{
			mFlood.OnSlide(_Direction, _Distance);
		}
        do
        {
            mTargetTile = null;
            ObjectOnTargetTile = null;
            CheckOneMoreTime = false;
            //자신이 있는 라인에서 인잣값으로 받은 방향으로 타겟 타일 캐치
            mTargetTile = GameManager.Instance.CheckLine(mTile.GetIndexInArray(), _Direction, _Distance);
            ObjectOnTargetTile = mTargetTile.GetObjectOnTile();
            if (ObjectOnTargetTile != null)
            {
                if (gameObject != ObjectOnTargetTile)
                {
                    OnCollision(ObjectOnTargetTile, ref mTargetTile, out CheckOneMoreTime);
                }
            }
            //자신의 타일을 빈 공간으로 한다
            mTile.GetComponent<Tile>().SetObjectOnTile(null);
            //삭제 안당했다면 타겟 타일을 자신으로 채운다
            if (mIsDestroyed == false)
            {
                SetTile(mTargetTile);
            }

            
        } while (CheckOneMoreTime == true);
		MoveWhenSlided(mTargetTile.transform.position);

        Vector2 curIndex = mTargetTile.GetIndexInArray();
        if (prevIndex == curIndex)
        {

            //이 오브젝트는 움직이지 않았다고 게임매니저에 보고한다
            return false;
        }

        //변경점이 있었다고 게임매니저에 보고한다
        return true;
    }

	public void SetTile(Tile _Tile)
	{
		mTile = _Tile;
		_Tile.SetObjectOnTile(gameObject);
	}

	public Tile GetTile()
	{
		return mTile;
	}

	public void SetFlood(Flood _Flood)
	{
		mFlood = _Flood;
	}

	public Flood GetFlood()
	{
		return mFlood;
	}

	protected void MoveWhenSlided(Vector2 _TargetPos)
	{
        //Vector2 mMoveStartPos = transform.position;
        //Vector2 mMoveTargetPos = _TargetPos;
        //float mMoveLerpT = 0;
        //float mMoveVelocity = 9f;

        //while (mMoveLerpT <= 1.05)
        //{
        //	mMoveVelocity -= 8f * Time.deltaTime;
        //	mMoveLerpT += mMoveVelocity * Time.deltaTime;
        //	transform.position = Vector2.LerpUnclamped(mMoveStartPos, mMoveTargetPos, mMoveLerpT);
        //	yield return null;
        //}

        //mMoveVelocity = 0f;
        //while(mMoveLerpT >= 1)
        //{
        //	mMoveVelocity -= 8f * Time.deltaTime;
        //	mMoveLerpT += mMoveVelocity * Time.deltaTime;
        //	transform.position = Vector2.LerpUnclamped(mMoveStartPos, mMoveTargetPos, mMoveLerpT);
        //	yield return null;
        //}
        //transform.position = mMoveTargetPos;

        transform.DOMove(_TargetPos, 0.3f).SetEase(Ease.OutBack).OnComplete(()=>
        {
        if (mIsDestroyed == true)
        {
            mIsDestroyed = false;
            GetComponent<SpriteRenderer>().enabled = true;
            Deactivate();
        }
        });
	}

	public virtual void CreateAnimation(Vector3 _TargetScale)
	{
        transform.localScale = new Vector3(0.1f, 0.1f, 1) * _TargetScale.x;
        transform.DOScale(_TargetScale, 0.3f);
        //while(transform.localScale.x < _TargetScale.x + 0.2)
        //{
        //	transform.localScale += new Vector3(10f, 10f, 0) * _TargetScale.x * Time.deltaTime;
        //	yield return null;
        //}

        //while(transform.localScale.x > _TargetScale.x)
        //{
        //	transform.localScale -= new Vector3(4f, 4f, 0) * _TargetScale.x * Time.deltaTime;
        //	yield return null;
        //}

        //transform.localScale = _TargetScale;
	}

    public void IdleAnimation(Vector3 _TargetScale)
    {
        transform.DOScale(_TargetScale.y + 0.05f, 1).From(_TargetScale.y,true).SetLoops(-1,LoopType.Yoyo);
    }

	public Tile GetTargetTile()
	{
		return mTargetTile;
	}
}
