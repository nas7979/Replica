using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public GameObject mObjectOnTile = null;
	private Vector2 mIndexInArray;

	public GameObject GetObjectOnTile()
	{
		return mObjectOnTile;
	}

	public void SetObjectOnTile(GameObject _Object)
	{
		mObjectOnTile = _Object;
	}

	public Vector2 GetIndexInArray()
	{
		return mIndexInArray;
	}

	public void SetIndexInArray(Vector2 _Pos)
	{
		mIndexInArray = _Pos;
	}
}
