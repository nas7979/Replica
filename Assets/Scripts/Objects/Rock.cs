using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Object
{
	public override void OnCollision(GameObject _Other, ref Tile _TargetTile, out bool _CheckOneMoreTime)
	{
		_CheckOneMoreTime = false;
	}

    public override void SetDestroy(bool _Destroyed = true)
    {
        base.SetDestroy(_Destroyed);
        GameManager.Instance.DecreaseObjectCount(EnumObjects.Rock);
    }

    public override bool OnSlide(InputManager.SlideDirection _Direction, int _Distance = 5)
	{
        bool hasTileSlided = false;
        if (mFlood != null)
        {
            Debug.Log("오브젝트에서 홍수 온슬라이드");
            hasTileSlided = mFlood.OnSlide(_Direction, _Distance);
        }
        return hasTileSlided;
    }
}
