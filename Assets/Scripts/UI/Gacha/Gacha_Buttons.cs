using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Gacha_Buttons : MonoBehaviour
{
	[Header("Normal, Rare, Legend 비율. 합계 100초과해도 됌")]
	public float mSkinRatio_Normal = 83;
	public float mSkinRatio_Rare = 12;
	public float mSkinRatio_Legend = 5;
	
	public float mSkinRatio_NormalStack = 0;
	public float mSkinRatio_RareStack = 0;
	public float mSkinRatio_LegendStack = 0;


	[SerializeField] private const int mGacha_Price = 100;
	[SerializeField] private NOTICETEXT mNeedGoldNoticeText;
	[SerializeField] private NOTICETEXT mMileageAddedNoticeText;

	[Header("애니메이션 매개변수들")]
	public float SpaceShakeTime = 1.0f;
	public int SpaceShakeVibrato = 100;
	public float SpaceShakeElasticity = 1.0f;
	public Ease SpaceShakeType = Ease.OutSine;
	[SerializeField] private bool mIsGachaAnimPlaying = false;
	[SerializeField] private bool mHasConfirmed = false;

	[Header("스크립트 동작에 필요한 것들")]
	[SerializeField] private Text mGold = null;
	[SerializeField] private Text mMileage = null;
	[SerializeField] private Image mSkinClass = null;
	[SerializeField] private Image mNewSkinNotice = null;
	[SerializeField] private GameObject mMileagePopup = null;
	[InspectorName("Normal : 0, Rare : 1, Legend : 2 인덱스 순서로 할당")]
	public Sprite[] mSkinClassSprites = null;

	[SerializeField] private Text mSkinName = null;
	[SerializeField] private Button mButton_Gacha = null;
	[SerializeField] private Button mButton_GachaConfirm = null;
	[SerializeField] private Button mButton_Reward = null;
	[SerializeField] private Replica_Menu mReplica = null;

	[SerializeField] private FadeInFadeOut mFade;
	[SerializeField] private SpriteRenderer mBlackBackground;

	[InspectorName("가챠 버튼 할당")]
	public Button mBuyButton_Enable = null;
	public Button mBuyButton_Disable = null;

	[SerializeField] private Animator mGachaLightAnim = null;
	[SerializeField] private Animator mSpaceshipAnim = null;

	[InspectorName("씬 UI 업데이트를 위해 광고 버튼 스크립트 할당")]
	public AdReward.AdRewardButton mAdRewardButton = null;

	private void Start()
    {
#if UNITY_EDITOR
		//DataManager.Instance.AddGold(-300);
		//Debug.Log("Gacha Scene : 개발자 테스트용 골드 지급" + Time.time);
#endif
		mGold.text = DataManager.Instance.Gold.ToString();
		mMileage.text = "+" + DataManager.Instance.Mileage.ToString();
	}

	private void OnEnable()
	{
		ResetSceneUI();
	}

    private void Update()
    {
		//가챠 스킵
		//if (Input.GetKeyDown(KeyCode.Mouse0) && mIsGachaAnimPlaying == true)
		//	mIsGachaAnimPlaying = false;
	}
    public void Gacha()
	{
		if (DataManager.Instance.Gold >= mGacha_Price)
		{
			SoundManager.Instance.PlaySound("Sfx_Button");

			mButton_Gacha.gameObject.SetActive(false);
			mButton_Reward.interactable = false;

			DataManager.Instance.AddGold(-mGacha_Price);
			mGold.text = DataManager.Instance.Gold.ToString();

			int skinClass = GetSkinClass();
			string skinName = GetSkin(skinClass);
			bool isNewSkin = !DataManager.Instance.GetData(skinName).Equals("Unlocked");

			//뽑은 스킨 잠금 해제
			if (isNewSkin)
				DataManager.Instance.AddUnlockedSkin(skinName);

			mIsGachaAnimPlaying = true;


			StartCoroutine(GachaAnimation(skinClass, skinName, isNewSkin));
		}
		else {
			SoundManager.Instance.PlaySound("Sfx_Error");
			mNeedGoldNoticeText.gameObject.SetActive(true);
			mNeedGoldNoticeText.Effect();
		}
		//ChangeBuyButton(DataManager.Instance.Gold >= mGacha_Price);

	}

	IEnumerator GachaAnimation(int skinClass, string skinName, bool isNewSkin)
    {

        #region 우주선 열기
        if (mIsGachaAnimPlaying == true)
        {
            mSpaceshipAnim.SetTrigger("Open");
            yield return null;
            //Light Animation이 끝날때까지 기다리기
            while (mIsGachaAnimPlaying && mGachaLightAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }
        }
		#endregion

		#region 스킨 등급에 따른 빛 연출과 뽑은 스킨 정보 공개
		if (mIsGachaAnimPlaying == true)
        {
            string[] triggerNames = { "Play_N", "Play_R", "Play_L" };
            mGachaLightAnim.SetTrigger(triggerNames[skinClass]);
            yield return null;
			float spaceShakeTime = SpaceShakeTime;
			//우주선 흔들리는 연출
			mSpaceshipAnim.transform.DOPunchRotation(new Vector3(0, 0, 1) * 10, SpaceShakeTime, SpaceShakeVibrato, SpaceShakeElasticity).SetEase(SpaceShakeType);
			SoundManager.Instance.PlaySound("Sfx_GachaLight");
            //Delay와 Light Animation이 끝날때까지 기다리기
            while (mIsGachaAnimPlaying && (mGachaLightAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f || spaceShakeTime > 0))
            {
				spaceShakeTime -= Time.deltaTime;
                yield return null;
            }
			if (mIsGachaAnimPlaying)
			{
				mFade.GetRenderer().color = new Color(1, 1, 1, 0);
				//Fade Out
				mFade.GetRenderer().DOFade(1f, 1.5f).OnComplete(() =>
				{//흰 화면으로 Fade Out이 끝난 시점
					//뽑은 스킨 정보 공개
					mBlackBackground.color = new Color(0, 0, 0, 0.5f); //배경을 약간 검게

					mGachaLightAnim.SetTrigger("Idle");
					mSkinClass.enabled = true;
					ChangeSkinClassImage(skinClass); //스킨 랭크 출력
					mSkinName.text = ConvertSkinNameToUserText(skinName);
					mReplica.SetImage(skinName); //레플리카 스킨 출력
					mReplica.GetComponent<Image>().enabled = true;
					if (isNewSkin == true)
					{
						SoundManager.Instance.PlaySound("Sfx_Gacha_New");
						mNewSkinNotice.gameObject.SetActive(true);
					}
					else
					{
						float mileage = skinClass == 0 ? 0.05f : skinClass == 1 ? 0.25f : 1f;
						SoundManager.Instance.PlaySound("Sfx_Gacha_Old");
						mMileagePopup.gameObject.SetActive(true);
						mMileageAddedNoticeText.gameObject.SetActive(true);
						mMileageAddedNoticeText.SetText("+" + (mileage).ToString());
						mMileageAddedNoticeText.Effect();
						DataManager.Instance.Mileage += mileage;
						BackEnd.Param param = new BackEnd.Param();
						param.Add("mileage", DataManager.Instance.Mileage);
						param.Add("gamerInDate", DataManager.Instance.InDate);
						DataManager.Instance.SendDataToServerSchema("mileage", param);
						mMileage.text = "+" + DataManager.Instance.Mileage.ToString();
					}

					//확인 버튼 On
					mButton_GachaConfirm.gameObject.SetActive(true);
					mHasConfirmed = false;

					//투명한 화면으로 다시 Fade In 시작
					mFade.GetRenderer().DOFade(0f, 1.5f);
				});
			}
        }
        #endregion

        #region 플레이어가 확인 버튼을 클릭할 때까지 기다리고, 클릭되면 우주선 닫기
        while (mIsGachaAnimPlaying && mHasConfirmed == false)
        {
            //mGachaLightAnim.transform.localEulerAngles += new Vector3(0, 0, 30) * Time.deltaTime;
            yield return null;
        }
		if (mIsGachaAnimPlaying)
		{
			//우주선 닫기 : 자연스러운 화면 초기화를 위해 화면을 하얘지게 한다
			mFade.GetRenderer().DOFade(1f, 1f).OnComplete(() =>
			{
				//화면 초기화
				ResetSceneUI();
				mFade.GetRenderer().DOFade(0f, 1f).OnComplete(() =>
				{
					mFade.GetRenderer().color = new Color(1, 1, 1, 0);
				});
			});
		}
		else
			ResetSceneUI();
		#endregion
	}

	public void Confirm() {
		mHasConfirmed = true;
	}
    public void ResetSceneUI()
    {
		mAdRewardButton.gachaButton = this;

		mIsGachaAnimPlaying = false;
		mHasConfirmed = false;

		mNeedGoldNoticeText.gameObject.SetActive(false);
		mSpaceshipAnim.SetTrigger("Close");
		mBlackBackground.color = new Color(0, 0, 0, 0f);
		mNewSkinNotice.gameObject.SetActive(false);
		//가챠 버튼 On, 확인 버튼 Off
		mButton_Gacha.gameObject.SetActive(true);
		mButton_GachaConfirm.gameObject.SetActive(false);
		mMileagePopup.SetActive(false);
		mButton_Reward.interactable = true;

		mReplica.GetComponent<Image>().enabled = false;

		mGold.text = DataManager.Instance.Gold.ToString();
		mSkinName.text = "";
		mSkinClass.enabled = false;
		mNewSkinNotice.gameObject.SetActive(false);
		mReplica.GetComponent<Image>().enabled = false;
		ChangeBuyButton(DataManager.Instance.Gold >= mGacha_Price);
	}

    public void BackToMenu(float mFadeTime)
	{
		SoundManager.Instance.PlaySound("Sfx_Button");
		mFade.FadeOut("Menu", mFadeTime);
	}

	public void ChangeBuyButton(bool canBuy)
	{
		mBuyButton_Enable.gameObject.SetActive(canBuy);
		mBuyButton_Disable.gameObject.SetActive(!canBuy);

		//mBuyButton.image.sprite = (canBuy) ? mBuyButtonSprite_Enable : mBuyButtonSprite_Disable;
	}
	
	public void ChangeSkinClassImage(int skinClass)
	{
		mSkinClass.sprite = mSkinClassSprites[skinClass];
		mSkinClass.GetComponent<RectTransform>().sizeDelta = mSkinClass.sprite.rect.size;
	}

	/// <summary>
	/// 유저들이 사용하는 한국어 스킨 이름으로 변환해 반환한다
	/// </summary>
	/// <param name="skinName"></param>
	public string ConvertSkinNameToUserText(string skinName)
	{
		Dictionary<string, string> skinNameConverter = new Dictionary<string, string>() {
			{"Item_Normal_Sunglass", "선글라스" },
			{"Item_Normal_Flower", "꽃" },
			{"Item_Normal_DevilHorn", "악마의 뿔" },
			{"Item_Normal_Glasses", "안경" },
			{"Item_Normal_Halo", "헤일로" },
			{"Item_Normal_WitchHat", "마녀 모자" },
			{"Item_Normal_Mushroom", "버섯"},
			{"Item_Normal_Pilot", "파일럿"},
			{"Item_Normal_WhiteMustache", "하얀 콧수염"},
			{"Item_Normal_Headphone", "헤드폰"},
			{"Item_Normal_Cow", "음머"},
			{"Item_Normal_Army", "군모"},
			{"Item_Normal_Candle", "양초"},
			{"Item_Normal_Coffin", "관짝댄서"},
			{"Item_Normal_EyePatch", "수면안대"},
			{"Item_Normal_GraduationCap", "학사모"},
			{"Item_Normal_Kitsune", "키츠네멘"},
			{"Item_Normal_Pirate", "해적"},
			{"Item_Normal_Pot", "화분"},
			{"Item_Normal_Sailor", "선원"},
			{"Item_Rare_Indian", "인디언" }, 
			{"Item_Rare_Mouse", "쥐" },
			{"Item_Legend_Catbot", "고양이 로봇" },
			{"Item_Legend_Kaito", "괴도 레플리카" },
			{"Item_Legend_Fishbowl", "어항" }
		};
		
		return skinNameConverter[skinName];
	}


	/// <summary>
	/// N : 0, R : 1, L : 2를 반환한다.
	/// </summary>
	/// <returns></returns>
	public int GetSkinClass() { 
		int skinClass = 0;
		float [] ratios = { mSkinRatio_Normal + mSkinRatio_NormalStack
				, mSkinRatio_Rare + mSkinRatio_RareStack
				,  mSkinRatio_Legend + mSkinRatio_LegendStack};
		float maxRatio = 0;
		foreach (float f in ratios)
			maxRatio += f;
		

		float random = Random.Range(0, maxRatio); //만약 maxRatio가 100이라면 Range함수는 0 ~ 99까지의 값 반환
		Debug.Log(maxRatio + "  :: " + random + " ratio[0] :: " + ratios[0]);
		//난수가 어떤 등급에 속하는지 확인
		if (random < ratios[0])
			skinClass = 0;
		else if (random < ratios[0] + ratios[1])
			skinClass = 1;
		else if (random < ratios[0] + ratios[1] + ratios[2])
			skinClass = 2;
		Debug.Log(skinClass);

		//float ratioStack = 0;
		//for (int i = 0; i < ratios.Length; i++) {
		//	if (random + ratioStack <  ratios[i]) {
		//		skinClass = (ratios.Length - 1) - i;
		//		break;
		//	}
		//	//다음번 반복할 때, 낮은 등급의 확률을 포함시킨다
		//	ratioStack += ratios[i];
		//}

		//스킨 확률 업데이트
		switch (skinClass) {
			//노말 스킨을 뽑았을 경우
			case 0:
				AddSkinRatioRareStack(0.6f);
				AddSkinRatioLegendStack(0.25f);
				UpdateSkinRatioNormalStack();
				
				break;
			//레어 스킨
			case 1:
				mSkinRatio_RareStack = 0;
				AddSkinRatioLegendStack(0.1f);
				UpdateSkinRatioNormalStack();
				break;
			//레전드
			case 2:
				AddSkinRatioRareStack(0.3f);
				mSkinRatio_LegendStack = 0;
				UpdateSkinRatioNormalStack();

				break;
		}

		return skinClass;
	}

	void AddSkinRatioRareStack(float value) {
		if (mSkinRatio_RareStack + value <= 12)
			mSkinRatio_RareStack += value;
		else
			mSkinRatio_RareStack = 12;
	}
	
	void AddSkinRatioLegendStack(float value) {
		if (mSkinRatio_LegendStack + value <= 5)
			mSkinRatio_LegendStack += value;
		else
			mSkinRatio_LegendStack = 5;
	}

	void UpdateSkinRatioNormalStack() {
		mSkinRatio_NormalStack = -(mSkinRatio_RareStack + mSkinRatio_LegendStack);
	}
	/// <summary>
	/// Skin 등급 (N : 0, R : 1, L : 2) 내에서 Skin 뽑기
	/// </summary>
	/// <param name="skinClass"></param>
	/// <returns></returns>
	public string GetSkin(int skinClass){

		string[] SkinNormalNames = { "Item_Normal_Sunglass", "Item_Normal_Flower", "Item_Normal_DevilHorn", "Item_Normal_Glasses", "Item_Normal_Halo", "Item_Normal_WitchHat",
		"Item_Normal_Mushroom", "Item_Normal_Pilot", "Item_Normal_WhiteMustache", "Item_Normal_Headphone", "Item_Normal_Cow","Item_Normal_Army","Item_Normal_Candle",
		"Item_Normal_Coffin","Item_Normal_EyePatch","Item_Normal_GraduationCap","Item_Normal_Kitusne","Item_Normal_Pirate","Item_Normal_Pot","Item_Normal_Sailor" };
		string [] SkinRareNames = {"Item_Rare_Indian", "Item_Rare_Mouse" };
		string [] SkinLegendNames = { "Item_Legend_Catbot", "Item_Legend_Kaito", "Item_Legend_Fishbowl" };

		string[][] Skinbox = { SkinNormalNames, SkinRareNames,  SkinLegendNames };

		return Skinbox[skinClass][Random.Range(0, Skinbox[skinClass].Length)];
	}
}
