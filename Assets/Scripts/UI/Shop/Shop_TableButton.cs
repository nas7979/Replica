using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_TableButton : MonoBehaviour
{
	[SerializeField] private Shop_Table[] mTables = new Shop_Table[3];
	private int mCurTable = 0;
	[SerializeField] private Shop_Buttons shopButtons;

	public void OnEnable()
	{
		StartCoroutine(Wait());
	}

	public void ChangeTable(int _Table)
	{
		SoundManager.Instance.PlaySound("Sfx_Button");
		for(int i = 0; i < 3; i++)
		{
			mTables[i].gameObject.SetActive(i == _Table ? true : false);
		}
		mCurTable = _Table;
		StartCoroutine(Wait()); //CurTable이 Start함수에서 CurItem을 할당하도록 (한 프레임을) 기다린 후 UnlockButtion을 업데이트한다.
	}

	public Shop_Table GetCurTable()
	{
		return mTables[mCurTable];
	}

	IEnumerator Wait() {
		yield return null;
		shopButtons.ChangeUnlockButtonSprite(GetCurTable().GetCurItem().IsUnlocked());
		GetCurTable().SetText();
		yield break;
	}
}
