using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class Adapt_Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private EnumAdapts mEnumAdapt;
    private Adapt mAdapt;
    private string mAdaptInformation;
    [SerializeField]
    private Text mNameText;
    [SerializeField]
    private Text mInformationText;
    [SerializeField]
    public Image mIconImage;


    private void OnEnable()
    {
        CreateAnimation(transform.localScale);
    }

    public void OnClick()
    {
        mAdapt.Tier++;
		DataManager.Instance.AddAdpatCount(mAdapt.GetEnumAdapts());
		//gameObject.SetActive(false);
		transform.parent.transform.parent.gameObject.SetActive(false);
        //transform.parent.gameObject.SetActive(false);
		GameManager.Instance.GetAdaptWindow().AddAdapt(mAdapt.GetEnumAdapts());
        GameManager.Instance.SetSlideAble(true);
		GameManager.Instance.SetInOnAdaptSelect(false);
        GameManager.Instance.StartCoroutine(GameManager.Instance.SetAdaptInformation(mAdaptInformation, true));
        GameManager.Instance.GetAdaptListButtonEffect().SetActive(true);
        GameManager.Instance.GetAdaptListButtonEffect().transform.position = GameManager.Instance.GetAdaptButton().transform.position;

        TutorialManager.Instance.AddProgression(2);
		if (TutorialManager.Instance.GetCurrentStep() >= 3)
		{
			TutorialManager.Instance.transform.Translate(0, 1000, 0);
		}
	}

    public void SetAdapt(Adapt _Adapt)
    {
        mAdapt = _Adapt;
    }

    public void SetInformation(string _Information)
    {
        mAdaptInformation = _Information;
        mInformationText.text = mAdaptInformation;
    }

    public void SetName(string _Name)
    {
        mNameText.text = _Name;
    }

    public string GetName()
    {
        return mNameText.text;
    }

    public void SetIcon(Sprite _Icon)
    {
        mIconImage.sprite = _Icon;
    }

    public void CreateAnimation(Vector3 _TargetScale)
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 1) * _TargetScale.x;
        transform.DOScale(_TargetScale, 0.3f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mIconImage.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mIconImage.color = new Color(1, 1, 1, 1f);
    }
}
