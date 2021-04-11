using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adapt_Cadaver : Adapt
{
    public override void Init()
    {
        base.Init();
        mValue[0] = 0;
        mValue[1] = 0.25f;
        mValue[2] = 0.5f;
        mValue[3] = 1f;
        MaxTier = 3;
        mEnumAdapt = EnumAdapts.Cadaver;
        mEnumPart = EnumParts.Slime;
        mName = "시체처리부";
        mInformation = "포식자 제거 시, 경험치 추가 획득";
    }

    public void PlusExp(int _Level)
    {
        if(mTier != 0)
        {
        GetComponent<Replica>().AddExp(_Level * 10 * mValue[mTier]);
            GameObject Effect = ObjectManager.Instance.CreateOutTile(EnumObjects.Cadaver_Effect);
            Effect.transform.position = gameObject.transform.position;
        }
    }

    protected override void MaxTierFunc()
    {
        
    }
}
