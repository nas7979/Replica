using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Replica_TopPlayer : MonoBehaviour
{
	[SerializeField] private SpriteRenderer mSpriteRenderer = null;
	[SerializeField] private Text mPlace = null;
	[SerializeField] private Image mPlaceIcon = null;
	[SerializeField] private Text mScore = null;
	[SerializeField] private Text mName = null;
	[SerializeField] private Sprite[] mPlaceIcons = new Sprite[3];
	private Vector2 mDirection = Vector2.zero;
	private float mSpeed = 0;

	public void Start()
	{
		transform.position = Random.insideUnitCircle.normalized * 8;
		transform.Rotate(0, 0, Random.Range(0, 180));
		mDirection = (Random.insideUnitCircle.normalized * 2.5f - (Vector2)transform.localPosition).normalized;
		mSpeed = Random.Range(0.75f, 1.5f);
	}

	public void Update()
	{
		transform.Translate(mDirection * mSpeed * Time.deltaTime * transform.localScale.x, Space.World);
		transform.Rotate(0, 0, mSpeed * Time.deltaTime * Mathf.Sign(mDirection.x) * 15);
	}

	public void OnBecameInvisible()
	{
		if(gameObject.activeInHierarchy == true)
			StartCoroutine(CR_WaitForInvisible());
	}

	public void Setting(RankingData _Data)
	{
		mPlace.text = _Data.Rank.ToString();
		mPlaceIcon.sprite = mPlaceIcons[_Data.Rank - 1];
		mScore.text = _Data.Score.ToString();
		mName.text = _Data.Name;
		mSpriteRenderer.sortingOrder = -_Data.Rank;
		transform.GetChild(0).GetComponent<Canvas>().sortingOrder = mSpriteRenderer.sortingOrder;
		transform.localScale = new Vector3(1, 1, 1) * (0.75f + 0.75f / _Data.Rank);
		StartCoroutine(CR_SetImage(_Data));
	}

	private IEnumerator CR_SetImage(RankingData _Data)
	{
		yield return new WaitUntil(() => { return _Data.ExtraData != null; });
		yield return new WaitUntil(() => { return _Data.ExtraData.Skin != null; });
		mSpriteRenderer.sprite = DataManager.Instance.GetItemSprite(_Data.ExtraData.Skin);
	}

	private IEnumerator CR_WaitForInvisible()
	{
		yield return new WaitForSeconds(Random.Range(3f, 6f));
		transform.position = Random.insideUnitCircle.normalized * 12;
		mDirection = (Random.insideUnitCircle.normalized * 2.5f - (Vector2)transform.localPosition).normalized;
		mSpeed = Random.Range(0.75f, 1.5f);
	}
}
