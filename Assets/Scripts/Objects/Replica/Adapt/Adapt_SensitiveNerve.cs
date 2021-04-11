using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adapt_SensitiveNerve : Adapt
{
    public override void Init()
    {
        base.Init();
        mValue[0] = 0;
        mValue[1] = 0.25f;
        mValue[2] = 0.50f;
        mValue[3] = 1f;
        MaxTier = 3;
        mEnumAdapt = EnumAdapts.SensitiveNerve;
        mEnumPart = EnumParts.Core;
        mName = "수분 감지";
        mInformation = "홍수의 범위를 예측";
    }

    public bool CheckPrediction()
    {
        float rand = Random.Range(0f, 1f);
        return mValue[mTier] > rand;
    }

    protected override void MaxTierFunc()
    {
        
    }
}
