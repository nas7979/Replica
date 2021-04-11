using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdaptWindow_Content : MonoBehaviour
{
    public Text[] LevelToolTips;
    public Text Name;
    public Image Icon;
    public Image[] LevelLights;

    [SerializeField]
    private int m_AdaptLevel;

    [SerializeField]
    public string originalName;
    void Start()

    {
        //m_AdaptLevel = 0;
        //originalName = Name.text;
        //m_AdaptLevel = 1;
        SetAdaptLevel(m_AdaptLevel + 1);
    }

    public void SetAdaptLevel(int value) {
        m_AdaptLevel = value;
        Debug.Log(m_AdaptLevel);
        LightImagesUpdate(m_AdaptLevel);
        TooltipUpdate(m_AdaptLevel);
        NameUpdate(m_AdaptLevel);
    }
    public int GetAdaptLevel() {
        return m_AdaptLevel;
    }
    
    
    /// <summary>
    /// 인자로 전달된 레벨에 따라 라이트를 업데이트한다(켜고 끈다)
    /// </summary>
    /// <param name="adaptLevel"></param>
    public void LightImagesUpdate(int adaptLevel) {
        for (int i = 0; i < LevelLights.Length; i++) {
            //현재 레벨까지 Light를 켜고 끈다
            LevelLights[i].gameObject.SetActive((i < adaptLevel));
        }
    }

    /// <summary>
    /// 인자로 전달
    /// </summary>
    /// <param name="adaptLevel"></param>
    public void TooltipUpdate(int adaptLevel) {
        for (int i = 0; i < LevelToolTips.Length; i++)
        {
            //현재 레벨의 Tooltip를 켜고 아닌 것들은 끈다
            LevelToolTips[i].gameObject.SetActive((i+1 == adaptLevel));
        }
    }
    
    public void NameUpdate(int adaptLevel) {
        Name.text = originalName;
        for(int i=0; i<adaptLevel; i++)
            Name.text += "I";
    }
}
