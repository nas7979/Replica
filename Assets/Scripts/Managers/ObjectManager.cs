using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	public enum EnumObjects
	{
		Tile,
		Replica,
		Predator,
		Food,
		Rock,
		Flood,
        Meteor,
		SFX,
        FloodView,
        MeteorView,
        PlusEXP,
        LevelUpEffect,
		HitPow,
		Resurrection,
        Prognosis_Effect,
        Cadaver_Effect,
        Threat_Effect,
        Fish_Effect,
		ComboText,
		ScoreText,
		End
	}
public class ObjectManager : Singleton<ObjectManager>
{
    [SerializeField]
    private GameObject mCanvas = null;
	private List<List<GameObject>> mObjects = new List<List<GameObject>>();
	[SerializeField]
	private GameObject[] mPrefabs = new GameObject[(int)EnumObjects.End];

	private void Awake()
	{
		for(int i = 0; i < (int)EnumObjects.End; i++)
		{
			mObjects.Add(new List<GameObject>());
		}
		DontDestroyOnLoad(gameObject);
	}

	private void OnLevelWasLoaded(int level)
	{
		Clear();
	}

	public GameObject Create(EnumObjects _Name, Tile _Tile)
	{
		GameObject Temp = null;

		for(int i = 0; i < mObjects[(int)_Name].Count; i++)
		{
			if (mObjects[(int)_Name][i].activeInHierarchy == false)
			{
				Temp = mObjects[(int)_Name][i];
				Temp.SetActive(true);
				Temp.transform.position = _Tile.transform.position;
				Temp.GetComponent<Object>().SetTile(_Tile);
				Temp.GetComponent<Object>().SetEnumTag(_Name);
				return Temp;
			}
		}

		for(int i = 0; i < 10; i++)
		{
			Temp = Instantiate(mPrefabs[(int)_Name]);
			Temp.name = mPrefabs[(int)_Name].name;
			Temp.SetActive(false);
			mObjects[(int)_Name].Add(Temp);
		}
		Temp.SetActive(true);
		Temp.transform.position = _Tile.transform.position;
		Temp.GetComponent<Object>().SetTile(_Tile);
        Temp.GetComponent<Object>().SetEnumTag(_Name);
        //Temp.GetComponent<Object>().CreateAnimation(Temp.transform.localScale);
		return Temp;
	}

	public GameObject CreateOutTile(EnumObjects _Name)
	{
		GameObject Temp = null;

		for (int i = 0; i < mObjects[(int)_Name].Count; i++)
		{
			if (mObjects[(int)_Name][i].activeInHierarchy == false)
			{
				Temp = mObjects[(int)_Name][i];
				Temp.SetActive(true);
				return Temp;
			}
		}

		for (int i = 0; i < 10; i++)
		{
			Temp = Instantiate(mPrefabs[(int)_Name]);
			Temp.name = mPrefabs[(int)_Name].name;
			Temp.SetActive(false);
			mObjects[(int)_Name].Add(Temp);
		}
		Temp.SetActive(true);
		return Temp;
	}

    public GameObject CreateInCanvas(EnumObjects _Name)
    {
        GameObject Temp = null;

		for (int i = 0; i < mObjects[(int)_Name].Count; i++)
		{
			if (mObjects[(int)_Name][i].activeInHierarchy == false)
			{
				Temp = mObjects[(int)_Name][i];
				Temp.SetActive(true);
				return Temp;
			}
		}

		for (int i = 0; i < 10; i++)
        {
            Temp = Instantiate(mPrefabs[(int)_Name], mCanvas.transform);
            Temp.name = mPrefabs[(int)_Name].name;
            Temp.SetActive(false);
			mObjects[(int)_Name].Add(Temp);
		}
        Temp.SetActive(true);
        return Temp;
    }

	public GameObject GetObject(EnumObjects _Name)
	{
		return mPrefabs[(int)_Name];
	}

	public List<GameObject> GetObjects(EnumObjects _Name)
	{
		return mObjects[(int)_Name];
	}

	public void SetCanvas(GameObject _Canvas)
	{
		mCanvas = _Canvas;
	}

	public void Clear()
	{
		for (int i = 0; i < mObjects.Count; i++)
		{
			mObjects[i].Clear();
		}
	}
}
