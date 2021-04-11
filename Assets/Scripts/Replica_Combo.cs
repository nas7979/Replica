using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//이 클래스는 레플리카가 사용함
//11월 23일 주석 : 
//콤보 초기화는 매 턴 true로 설정되고, slide시에 타 오브젝트의 콤보 연장 함수의 호출로 초기화 유무가 false값으로 설정된다.
//콤보 초기화에 관한 업데이트 함수는 게임매니저 CR_waitForNextSlide() 코루틴 직전에 호출한다.
public class Replica_Combo : MonoBehaviour
{
    private Replica m_Replica;
    private int m_ComboStack; //콤보 수는 최소 콤보부터 시작한다. 
    private const int m_MinComboStack = 0; //Lost함수가 호출될 때 콤보 스택을 이 값으로 초기화한다.
    private bool m_IsComboLost; //콤보를 초기화해야하는가
	private int m_TotalSlideStack = 0; //총 슬라이드 횟수
    // Start is called before the first frame update
    void Start()
    {
        SetIsComboLost(true);
        m_Replica = GetComponent<Replica>();

        //콤보텍스트 참조 실패시 유니티 에디터 정지
//        if (!m_COMBOTEXT) {
//            m_COMBOTEXT = FindObjectOfType<COMBOTEXT>();
//            if (!m_COMBOTEXT)
//            {
//#if UNITY_EDITOR
//                EditorApplication.isPaused = true;
//                Debug.Log("Replica_Combo : ComboText를 찾지 못했습니다");
//#endif
//            }
//        }
       
    }

    /// <summary>
    /// 콤보 1 추가
    /// </summary>
    /// <param name="value"></param>
    public void Extend(float eatObjScore = 0)
    {
		m_TotalSlideStack++;
		m_ComboStack += 1 + (int)m_Replica.SkinEffect("OnSlide", m_TotalSlideStack);
        m_IsComboLost = false;
        GameManager.Instance.AddTotalScore(eatObjScore * (m_ComboStack - m_MinComboStack) * 0.5f + 1);

        GameObject Temp = ObjectManager.Instance.CreateInCanvas(EnumObjects.ComboText);
        Temp.GetComponent<COMBOTEXT>().Effect(m_ComboStack);
        Temp = ObjectManager.Instance.CreateInCanvas(EnumObjects.ScoreText);
		Temp.GetComponent<SCORETEXT>().Effect(new Vector3(m_Replica.GetTargetTile().transform.position.x, m_Replica.GetTargetTile().transform.position.y + 2), eatObjScore * (m_ComboStack - m_MinComboStack) * 0.5f + 1);
	}
    /// <summary>
    /// 강제로 콤보를 1로 초기화
    /// </summary>
    /// <param name="setValue"></param>
    public void Lost() {
		if (Random.Range(0f, 1f) < m_Replica.SkinEffect("OnComboLost", 0))
			return;
        m_ComboStack = m_MinComboStack;
    }
    /// <summary>
    /// 콤보 초기화에 관한 업데이트 함수
    /// </summary>
    public void ComboSystemUpdate() {
        if (m_IsComboLost == true)
            Lost();
        m_IsComboLost = true;
    }

    /// <summary>
    /// 콤보를 초기화해야하는지 반환한다
    /// </summary>
    public bool GetIsComboLost() {
        return true;
    }

    public void SetIsComboLost(bool value) {
        m_IsComboLost = value;
    }
}
