using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AdaptWindow : MonoBehaviour
{
	private bool mIsTouchAble;
	//[SerializeField]
	//private GameObject[] mAdaptIcons = new GameObject[(int)EnumAdapts.End];
	[SerializeField]
	private RectTransform mView;
	[SerializeField]
	private RectTransform mContent;
    [SerializeField]
	private RectTransform mAdapt;
	private int mAdaptCount;

	public AdaptWindow_Content[] Contents;

	public void Init()
	{
		mAdaptCount = 0;
	}

	private void OnEnable()
	{
		mIsTouchAble = false;
		StartCoroutine(CR_OpenWindow());
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (!GetComponent<BoxCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)) && mIsTouchAble)
			{
				StartCoroutine(CR_CloseWindow());
				mIsTouchAble = false;
			}
		}
	}

	public void AddAdapt(EnumAdapts _Adapt)
	{
        //gameObject.SetActive(true);
        //GameObject Temp = mAdaptIcons[(int)_Adapt];
        //if (Temp.activeInHierarchy == false)
        //{
        //	Temp.transform.localPosition = new Vector3(0, 150f - 480f * mAdaptCount, 0);
        //	Temp.SetActive(true);
        //	mAdaptCount++;
        //}
        //else
        //{
        //	Temp.transform.GetChild(1).GetComponent<Text>().text = Temp.transform.GetChild(1).GetComponent<Text>().text + "I";
        //	if(Temp.transform.GetChild(2).gameObject.activeInHierarchy == true)
        //	{
        //		Temp.transform.GetChild(2).gameObject.SetActive(false);
        //		Temp.transform.GetChild(3).gameObject.SetActive(true);
        //	}
        //	else if (Temp.transform.GetChild(3).gameObject.activeInHierarchy == true)
        //	{
        //		Temp.transform.GetChild(3).gameObject.SetActive(false);
        //		Temp.transform.GetChild(4).gameObject.SetActive(true);
        //	}
        //}
        //if(mAdaptCount > 3)
        //{
        //	mContent.sizeDelta = new Vector2(1080, 1500 + 600 * (mAdaptCount - 3));
        //	mView.transform.localPosition = new Vector2(0, 100 + 300 * (mAdaptCount - 3));
        //}
        //gameObject.SetActive(false);

        gameObject.SetActive(true);
        AdaptWindow_Content Temp = Contents[(int)_Adapt];
        

        //Temp.originalName = Temp.Name.text;
        //변이 콘텐츠들의 포지션 초기화
        if (Temp.gameObject.activeSelf == false)
        {

            Temp.transform.localPosition = new Vector3(0, mAdapt.sizeDelta.y/2 - mAdapt.sizeDelta.y * mAdaptCount, 0);
            mAdaptCount++;
            
            Temp.gameObject.SetActive(true);

        }
        //인게임에서 변이 콘텐츠 추가
        else
        {
            Temp.SetAdaptLevel(Temp.GetAdaptLevel() + 1);
        }

        mContent.sizeDelta = new Vector2(mContent.sizeDelta.x, mAdapt.sizeDelta.y * (mAdaptCount));
        mContent.position = new Vector2(mContent.position.x, mAdapt.sizeDelta.y - mAdapt.sizeDelta.y/2 * (mAdaptCount - 1));
        if (mAdaptCount > 2)
        {
            mView.transform.localPosition = new Vector2(0, mAdapt.sizeDelta.y/2 * (mAdaptCount - 2));
        }

        
        gameObject.SetActive(false);
    }

	IEnumerator CR_OpenWindow()
	{
		transform.localScale = new Vector3(0.1f, 0.1f, 1);
		while (transform.localScale.x < 1)
		{
			transform.localScale += new Vector3(5f, 5f, 0) * Time.deltaTime;
			yield return null;
		}
		transform.localScale = new Vector3(1, 1, 1);
		mIsTouchAble = true;
	}

	IEnumerator CR_CloseWindow()
	{
		while (transform.localScale.x > 0.1)
		{
			transform.localScale -= new Vector3(5f, 5f, 0) * Time.deltaTime;
			yield return null;
		}
		gameObject.SetActive(false);
		if (GameManager.Instance.GetInOnAdaptSelect() == false)
		{
			GameManager.Instance.SetSlideAble(true);
		}
        GameManager.Instance.GetAdaptButton().GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 0), 0.5f);
        yield break;
	}
}
