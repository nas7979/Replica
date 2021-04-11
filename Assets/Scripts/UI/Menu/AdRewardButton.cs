using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdReward
{
    using Func_void = GlobalCoroutineManager.FuncDelegate;
    using Func_bool = GlobalCoroutineManager.FuncConditionDelegate;

    public class AdRewardButton : MonoBehaviour
    {
        [InspectorName("광고 시청 쿨타임(단위:초)")]
        public float AdWatchCoolMax = 60;
        [InspectorName("광고 시청 쿨타임 표시 게이지")]
        public Slider Gauge;

        [SerializeField] WindowOpenClose adWindow;
        [SerializeField] Text text_AdWindowGold;
        [SerializeField] Text text_MainMenuGold;
        [HideInInspector] public Gacha_Buttons gachaButton;
        [HideInInspector] public Shop_Buttons shopButtons;

        [SerializeField] int rewardGoldRangeMin = 50;
        [SerializeField] int rewardGoldRangeMax = 150;

        //광고 시청 쿨타임 코루틴 중 실행할 함수들
        List<Func_void> adCoolDownStartFuncs;
        List<Func_void> adCoolDownPlayFuncs;
        List<Func_void> adCoolDownEndFuncs;
        Func_bool adCoolDownCondition;
        Func_void adCoolDownGlobalStart;

        //광고 시청 코루틴 종료 시 실행할 함수들
        List<Func_void> adWatchEndFuncs;

        GlobalCoroutineManager GCM;

        private void Awake()
        {
			if(!DataManager.Instance.GetData("LastLogined").Remove(10).Equals(System.DateTime.Now.ToString().Remove(10)))
			{
				AdsManager.Instance.RewardAdsLimit = 0;
				DataManager.Instance.SetData("LastLogined", System.DateTime.Now.ToString());
			}
            GCM = GlobalCoroutineManager.Instance;
#if !UNITY_EDITOR//플랫폼이 유니티 에디터가 아닐 경우에만 광고 제한 작동
            if (AdsManager.Instance.RewardAdsLimit == 5)
				gameObject.SetActive(false);
#endif
		}

        public void Start()
        {
            adCoolDownStartFuncs = new List<Func_void>();
            adCoolDownPlayFuncs = new List<Func_void>();
            adCoolDownEndFuncs = new List<Func_void>();
            adWatchEndFuncs = new List<Func_void>();

#region 광고 쿨타임 코루틴을 글로벌로 사용(씬 전환되어도 삭제되지 않게)
            //광고 쿨타임 글로벌 변수를 찾기 위해 "AdCool" Key를 사용한다.
            if (GCM.Dictionary_Variables.ContainsKey("AdCool") == false)
                GCM.Dictionary_Variables.Add("AdCool", 0);
            if (GCM.Dictionary_Variables.ContainsKey("AdCoinReward") == false)
                GCM.Dictionary_Variables.Add("AdCoinReward", 0);

            adCoolDownCondition = () => GCM.Dictionary_Variables["AdCool"] > 0;
            adCoolDownStartFuncs.Add(() => GCM.Dictionary_Variables["AdCool"] = AdWatchCoolMax);
            adCoolDownPlayFuncs.Add(() => GCM.Dictionary_Variables["AdCool"] -= Time.deltaTime);
            adCoolDownEndFuncs.Add(SetRandomRewardGold);
            adCoolDownEndFuncs.Add(AdWindowGoldUIUpdate);

            //StartCoroutine으로 코루틴 상태를 재할당
            adCoolDownGlobalStart = () => GCM.StartGlobalRoutine(GCM.AnyRoutine(adCoolDownCondition, adCoolDownStartFuncs, adCoolDownPlayFuncs, adCoolDownEndFuncs));

#endregion
            

            adWatchEndFuncs.Add(GiveReward);
            adWatchEndFuncs.Add(MainMenuGoldUIUpdate);
            adWatchEndFuncs.Add(GachaUIUpdate);
            adWatchEndFuncs.Add(ShopUIUpdate);
            adWatchEndFuncs.Add(adCoolDownGlobalStart);

            if (GCM.Dictionary_Variables["AdCoinReward"] == 0)
            {
                //광고 쿨타임 관련 변수들 초기화
                foreach (var e in adCoolDownEndFuncs)
                {
                    e();
                }
            }
        }

        public void Update()
        {
            Gauge.value = 1 - (GCM.Dictionary_Variables["AdCool"] / AdWatchCoolMax);
        }


        public void SetRandomRewardGold()
        {
            //min인자가 0, max인자가 2이면 0이상 2미만을 반환하므로 2가 나오게 max에 1을 더한다. (int반환 Random.Range함수)
            GCM.Dictionary_Variables["AdCoinReward"] = Random.Range(rewardGoldRangeMin, rewardGoldRangeMax + 1);
        }

        public void GiveReward()
        {
            DataManager.Instance.AddGold((int)GCM.Dictionary_Variables["AdCoinReward"]);
        }

        public void MainMenuGoldUIUpdate()
        {
            if (text_MainMenuGold != null)
                text_MainMenuGold.text = DataManager.Instance.Gold.ToString();
        }
        
        public void GachaUIUpdate()
        {
            if (gachaButton != null)
                gachaButton.ResetSceneUI();
                    //text_GachaMenuGold.text = DataManager.Instance.Gold.ToString();
        }
        
        public void ShopUIUpdate()
        {
            if (shopButtons != null)
                shopButtons.ResetSceneUI();
                    //text_GachaMenuGold.text = DataManager.Instance.Gold.ToString();
        }

        public void AdWindowGoldUIUpdate()
        {
            if(text_AdWindowGold != null)
                text_AdWindowGold.text = GCM.Dictionary_Variables["AdCoinReward"].ToString();
        }

        public void AdRewardButtonDown()
        {
            if (GCM.Dictionary_Variables["AdCool"] <= 0)
            {
                adWindow.gameObject.SetActive(true);
                AdWindowGoldUIUpdate();
            }

#if UNITY_EDITOR//플랫폼이 유니티 에디터일 경우
            else
            {
                GCM.Dictionary_Variables["AdCool"] -= 3f; //버튼 누를 때마다 쿨타임 감소
                Debug.Log("Menu Scene : 개발자 테스트용 쿨타임 감소" + Time.time);
            }
#endif
        }

        public void AdWatchStart()
        {
            StartCoroutine(AdWatch(adWatchEndFuncs));
        }

        /// <summary>
        /// 광고를 시청한다. 광고 시청이 종료될 경우 코루틴 종료
        /// </summary>
        /// <param name="funcEndCoroutine">광고 시청 종료 후 실행할 코루틴</param>
        /// <param name="funcEndEvents">광고 시청 종료 후 실행할 함수</param>
        /// <returns></returns>
        public IEnumerator AdWatch(List<Func_void> funcEndEvents = null)
        {
			//광고 시청~
			//while () { yield return null; }
			AdsManager.Instance.ShowRewardAd();
			if (AdsManager.Instance.RewardAdsLimit == 5)
				gameObject.transform.Translate(10000, 0, 0);

            adWindow.CloseWindow();

			yield return new WaitUntil(() => { return AdsManager.Instance.IsUserEarnedReward; });
			AdsManager.Instance.IsUserEarnedReward = false;
			SoundManager.Instance.PlaySound("Sfx_Buy");
            foreach (Func_void e in funcEndEvents)
            {
                e();
            }
			if (AdsManager.Instance.RewardAdsLimit == 5)
				gameObject.SetActive(false);
			yield break;
        }
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;


//public class AdRewardButton : MonoBehaviour
//{
//    [InspectorName("광고 시청 쿨타임(단위:초)")]
//    public float AdWatchCoolMax = 60;
//    [InspectorName("광고 시청 쿨타임 표시 게이지")]
//    public Slider Gauge;

//    [SerializeField] WindowOpenClose adWindow;
//    [SerializeField] Text text_AdWindowGold;
//    [SerializeField] Text text_MainMenuGold;

//    [SerializeField] float adWatchCool = 0;
//    [SerializeField] int rewardGold = 100;
//    [SerializeField] int rewardGoldRangeMin = 50;
//    [SerializeField] int rewardGoldRangeMax = 150;

//    //광고 시청 쿨타임 코루틴 종료 시 실행할 함수들
//    List<GlobalCoroutineManager.FuncDelegate> adCoolDownEndEvents = new List<GlobalCoroutineManager.FuncDelegate>();

//    //광고 시청 코루틴 종료 시 실행할 함수들
//    List<GlobalCoroutineManager.FuncDelegate> adWatchEndEvents = new List<GlobalCoroutineManager.FuncDelegate>();

//    public void Start()
//    {
//        adCoolDownEndEvents.Add(SetRandomRewardGold);
//        adCoolDownEndEvents.Add(AdWindowGoldUIUpdate);

//        adWatchEndEvents.Add(GiveReward);
//        adWatchEndEvents.Add(MainMenuGoldUIUpdate);

//        //광고 시청 쿨타임 코루틴 종료 시 실행할 함수들
//        foreach (var e in adCoolDownEndEvents)
//        {
//            e();
//        }
//    }


//    public void SetRandomRewardGold()
//    {
//        //min인자가 0, max인자가 2이면 0이상 2미만을 반환하므로 2가 나오게 max에 1을 더한다. (int반환 Random.Range함수)
//        rewardGold = Random.Range(rewardGoldRangeMin, rewardGoldRangeMax + 1);
//    }

//    public void GiveReward()
//    {
//        DataManager.Instance.AddGold(rewardGold);
//    }

//    public void MainMenuGoldUIUpdate()
//    {
//        text_MainMenuGold.text = DataManager.Instance.Gold.ToString();
//    }

//    public void AdWindowGoldUIUpdate()
//    {
//        text_AdWindowGold.text = rewardGold.ToString();
//    }

//    public void AdRewardButtonDown()
//    {
//        if (adWatchCool <= 0)
//        {
//            adWindow.gameObject.SetActive(true);
//        }

//#if UNITY_EDITOR//플랫폼이 유니티 에디터일 경우
//        else
//        {
//            adWatchCool -= 3f; //버튼 누를 때마다 쿨타임 감소
//            Debug.Log("Menu Scene : 개발자 테스트용 쿨타임 감소" + Time.time);
//        }
//#endif
//    }

//    public void AdWatchStart()
//    {
//        IEnumerator adWatchEndCoroutine = AdCoolDownClock(AdWatchCoolMax, adCoolDownEndEvents);

//        StartCoroutine(AdWatch(adWatchEndCoroutine, adWatchEndEvents));
//    }

//    /// <summary>
//    /// 광고를 시청한다. 광고 시청이 종료될 경우 코루틴 종료
//    /// </summary>
//    /// <param name="funcEndCoroutine">광고 시청 종료 후 실행할 코루틴</param>
//    /// <param name="funcEndEvents">광고 시청 종료 후 실행할 함수</param>
//    /// <returns></returns>
//    public IEnumerator AdWatch(IEnumerator funcEndCoroutine = null, List<GlobalCoroutineManager.FuncDelegate> funcEndEvents = null)
//    {
//        //광고 시청~
//        //while () { yield return null; }


//        adWindow.CloseWindow();

//        foreach (GlobalCoroutineManager.FuncDelegate e in funcEndEvents)
//        {
//            e();
//        }

//        if (funcEndCoroutine != null)
//            StartCoroutine(funcEndCoroutine);
//        yield break;
//    }

//    /// <summary>
//    /// 광고 쿨타임, 게이지UI을 카운트 및 업데이트한다. 광고 쿨타임이 0 이하일 경우 코루틴 종료
//    /// </summary>
//    /// <param name="SetTime">광고 쿨타임 초기화 값</param>
//    /// <param name="funcEndEvents">광고 쿨타임이 0일 경우 실행할 함수</param>
//    /// <returns></returns>
//    private IEnumerator AdCoolDownClock(float SetTime, List<GlobalCoroutineManager.FuncDelegate> funcEndEvents = null)
//    {
//        adWatchCool = SetTime;
//        while (adWatchCool > 0)
//        {
//            adWatchCool -= Time.deltaTime;
//            Gauge.value = 1 - (adWatchCool / AdWatchCoolMax);
//            yield return null;
//        }

//        foreach (GlobalCoroutineManager.FuncDelegate e in funcEndEvents)
//        {
//            e();
//        }
//        yield break;
//    }

//}


