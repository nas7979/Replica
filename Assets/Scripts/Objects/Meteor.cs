using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : Object
{
    public GameObject MeteorEffect;
    private Animator mEffectAnimator;
    private Object mDestroyObject;
    public float Speed;
    //애니메이션 시간을 받아올 때 사용
    private void Awake()
    {
        mEffectAnimator = MeteorEffect.GetComponent<Animator>();
    }
    private new void OnEnable()
    {
		StopAllCoroutines();
		transform.localScale = new Vector3(1.5f, 1.5f, 1);
        StartCoroutine(CR_Move());
    }
    public override void OnCollision(GameObject _Other, ref Tile _TargetTile, out bool _CheckOneMoreTime)
    {
        _CheckOneMoreTime = false;
    }

    public override bool OnSlide(InputManager.SlideDirection _Direction, int _Distance = 5)
    {
        bool hasTileSlided = false;
        return hasTileSlided;
    }

    private IEnumerator CR_Move()
    {
        MeteorEffect.transform.localPosition = new Vector3(0, 2, 0);
        while (MeteorEffect.transform.localPosition.y > 0.2f)
        {
            MeteorEffect.transform.position -= new Vector3(0, 2, 0) * Time.deltaTime * Speed ;
            yield return null;
        }
        mEffectAnimator.SetTrigger("Break");
		SoundManager.Instance.PlaySound("Sfx_Meteor");
		if (mDestroyObject != null)
		{
			mDestroyObject.Deactivate(); //받아온 오브젝트를 삭제
			mDestroyObject.SetDestroy(false);
			mDestroyObject = null;
		}
		yield return new WaitForSeconds(0.4f);
        gameObject.SetActive(false);
		yield break;
    }

    public void SetDestroyObject(Object _DestroyObject)
    {
        mDestroyObject = _DestroyObject;
    }
}
