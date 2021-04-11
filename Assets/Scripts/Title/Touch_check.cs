using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Touch_check : MonoBehaviour
{
    [SerializeField]
    private GameObject mBlackMask;
    private FadeInFadeOut mFade;
    // Update is called once per frame
    private void Start()
    {
        mFade = mBlackMask.GetComponent<FadeInFadeOut>();
        Screen.SetResolution(Screen.width, (Screen.width / 9) * 16, true);
		GameManager.Clear();
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
			if (DataManager.Instance.GetData("IsFirstPlay") == "False")
				mFade.FadeOut("Menu", 0.7f);
			else
				mFade.FadeOut("Ingame", 0.7f);
        }
	}
}
