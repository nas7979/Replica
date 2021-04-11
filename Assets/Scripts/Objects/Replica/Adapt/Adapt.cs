using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumAdapts
{
    Cadaver, //시체 처리
    Fish, //물만난 물고기
    Appetite, //식탐
    Growing, //성장기
    Threat, //위협
    SensitiveNerve, //예민한 신경
    SensitiveEar, //예민한 귀
    Lucky, // 5 3 2 번째 먹이마다 좋은 먹이 생성
    River, //강의 축복
    End
}

public enum EnumParts
{
    Slime,
    Core,
    Parts,
    End
}

public class Adapt : MonoBehaviour
{
    [SerializeField]
    protected Sprite mIconSprite;
    protected string mName;
    protected float[] mValue;
    protected int mTier; //Tier는 0, 1, 2, 3 까지
    [Tooltip("최대 레벨 설정")]
    protected int MaxTier = 1;
    protected EnumParts mEnumPart;
    protected EnumAdapts mEnumAdapt;
    [SerializeField]
    protected Sprite[] mPartsSprite;
    [SerializeField]
    protected string mInformation;

    public virtual int Tier
    {
        get
        {
            return mTier;
        }
        set
        {
            if (Tier <= MaxTier)
                mTier = value;
            if (mTier == MaxTier)
                MaxTierFunc();
        }
    }

    protected virtual void MaxTierFunc()
    {

    }

    public int GetMaxTier()
    {
        return MaxTier;
    }

    public EnumAdapts GetEnumAdapts()
    {
        return mEnumAdapt;
    }

    public EnumParts GetEnumParts()
    {
        return mEnumPart;
    }
    public virtual void Init()
    {
        MaxTier = 3;
        mValue = new float[MaxTier + 1];
        Tier = 0;
    }

    public Sprite GetIconSprite()
    {
        return mIconSprite;
    }

	public float GetValue()
	{
		return mValue[mTier];
	}

    public string GetName()
    {
        return mName;
    }

    public string GetInformation()
    {
        return mInformation;
    }
}
