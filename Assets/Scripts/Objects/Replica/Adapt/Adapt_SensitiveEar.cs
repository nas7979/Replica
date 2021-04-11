using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adapt_SensitiveEar : Adapt
{
    public override void Init()
    {
        base.Init();
        mValue[0] = 0;
        mValue[1] = 0.25f;
        mValue[2] = 0.50f;
        mValue[3] = 1f;
        MaxTier = 3;
        mEnumAdapt = EnumAdapts.SensitiveEar;
        mEnumPart = EnumParts.Parts;
        mName = "재난 예지";
        mInformation = "운석의 위치를 예측";
    }

    public bool CheckPrediction()
    {
        float rand = Random.Range(0f, 1f);
        if(mValue[mTier] > rand)
        {
            GameObject Effect = ObjectManager.Instance.CreateOutTile(EnumObjects.Prognosis_Effect);
            Effect.transform.position = gameObject.transform.position;
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void MaxTierFunc()
    {
        
    }
}
