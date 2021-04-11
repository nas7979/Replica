using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] mEmptyButton;
    [SerializeField]
    private GameObject SelectButtonStore;
    public void CreateButton()
    {
        Replica Replica = GameManager.Instance.GetReplica().GetComponent<Replica>();
        List<Adapt> Adapts;
            int AllMaxCheck = 0;
        for (int i = 0; i < (int)EnumParts.End; i++)
        {
            Adapts = Replica.GetAdapts((EnumParts)i);
            for (int j = 0; j < Adapts.Count; j++)
            {
                if (Adapts[j].Tier == Adapts[j].GetMaxTier())
                {
                    AllMaxCheck++;
                }
            }
        }
        if (AllMaxCheck == (int)EnumParts.End)
            return;
        do
        {
            for (int i = 0; i < 2; i++)
            {
                bool LoopAdatp;
                do
                {
                    LoopAdatp = false;
                    EnumParts randParts = (EnumParts)Random.Range(0, (int)EnumParts.End);
                    Adapts = Replica.GetAdapts(randParts);
                    for (int j = 0; j < Adapts.Count; j++)
                    {
                        if (Adapts[j].Tier == Adapts[j].GetMaxTier())
                            LoopAdatp = true;
                    }
                } while (LoopAdatp);

                int RandAdapt = Random.Range(0, Adapts.Count);
                Adapt_Button Cur_Adapt_Button = mEmptyButton[i].GetComponent<Adapt_Button>();
                Cur_Adapt_Button.SetIcon(Adapts[RandAdapt].GetIconSprite());
                Cur_Adapt_Button.SetAdapt(Adapts[RandAdapt]);
                Cur_Adapt_Button.SetInformation(Adapts[RandAdapt].GetInformation());
                Cur_Adapt_Button.SetName(Adapts[RandAdapt].GetName());
                mEmptyButton[i].SetActive(true);
            }
        } while (mEmptyButton[0].GetComponent<Adapt_Button>().GetName() == mEmptyButton[1].GetComponent<Adapt_Button>().GetName());
        gameObject.SetActive(true);
        StartCoroutine(OpenSelectButton());
		GameManager.Instance.SetInOnAdaptSelect(true);
		if (TutorialManager.Instance.GetCurrentStep() >= 3)
		{
			TutorialManager.Instance.transform.Translate(0, -1000, 0);
		}
    }

    private IEnumerator OpenSelectButton()
    {
        GameManager.Instance.StartCoroutine(GameManager.Instance.SetAdaptInformation("적응을 선택하세요", false));
        yield return new WaitForSeconds(0.5f);
        SelectButtonStore.gameObject.SetActive(true);
    }
}
