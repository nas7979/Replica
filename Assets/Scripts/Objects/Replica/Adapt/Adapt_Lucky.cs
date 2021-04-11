using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adapt_Lucky : Adapt
{
	private int mCount;

	public override void Init()
	{
		base.Init();
		mCount = 1000000;
		mValue[0] = 1000000f;
		mValue[1] = 5f;
		mValue[2] = 3f;
		mValue[3] = 2f;
        MaxTier = 3;
        mEnumPart = EnumParts.Core;
		mEnumAdapt = EnumAdapts.Lucky;
        mName = "운수 좋은 날";
        mInformation = "가끔 좋은\n열매가 등장";
    }

	public override int Tier
	{
		get
		{
			return mTier;
		}
		set
		{
			if (mTier <= MaxTier)
				mTier = value;
            if (mTier == MaxTier)
                MaxTierFunc();
            ResetCount();
		}
	}

	public void OnSlide()
	{
		mCount--;
	}

	public void ResetCount()
	{
		mCount = (int)mValue[mTier];
	}

	public int GetCount()
	{
		return mCount;
	}

    protected override void MaxTierFunc()
    {
        
    }
}
