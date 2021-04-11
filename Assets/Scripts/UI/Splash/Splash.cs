using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
    public Image SchoolLogo;
    public Image LinkedListLogo;
    public Image BackendLogo;

    public float LogoTime = 1;
    public enum SplashTransition {
        ScreenFade = 0, LogoOnlyFade, Dissolve
    }
    public SplashTransition SplashTrans = SplashTransition.ScreenFade;
    public float LogoDissolveTime = 0.3f;

    public float InFadeTime = 0.4f;
    public float OutFadeTime = 0.4f;

    public SpriteRenderer BlackMask;
    // Start is called before the first frame update
    void Start()
    {
        Splash1Active();


        BlackMask = BlackMask.GetComponent<SpriteRenderer>();
        BlackMask.DOFade(0f, InFadeTime).OnComplete(() => {
            StartCoroutine(SplashPlay());
        });
    }

    private void Update()
    {
        //스플래시 스킵
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            BlackMask.color = new Color(0, 0, 0, BlackMask.color.a);//Color.black
            FadeOut("Title", OutFadeTime);
        }
    }

    public void FadeOut(string _SceneName, float _Time)
    {
        BlackMask.DOFade(1f, _Time).OnComplete(() => { SceneManager.LoadScene(_SceneName); });
    }

    void Splash1Active() {
        SchoolLogo.gameObject.SetActive(true);
        LinkedListLogo.gameObject.SetActive(false);
        BackendLogo.gameObject.SetActive(false);
    }
    void Splash2Active() {
        SchoolLogo.gameObject.SetActive(false);
        LinkedListLogo.gameObject.SetActive(true);
        BackendLogo.gameObject.SetActive(true);
    }

    IEnumerator SplashPlay()
    {
        float logoTime = LogoTime;
        float outFadeT = OutFadeTime;
        float inFadeT = InFadeTime;

        yield return null;
        switch (SplashTrans)
        {
            case SplashTransition.ScreenFade:
                while (logoTime > 0)
                {
                    logoTime -= Time.deltaTime;

                    yield return null;
                }

                BlackMask.color = new Color(1, 1, 1, 0);//Color.white
                BlackMask.DOFade(1f, OutFadeTime);
                while (outFadeT > 0)
                {
                    outFadeT -= Time.deltaTime;

                    yield return null;
                }

                Splash2Active();

                BlackMask.color = new Color(1, 1, 1, 1);//Color.white
                BlackMask.DOFade(0f, InFadeTime);
                while (inFadeT > 0)
                {
                    inFadeT -= Time.deltaTime;

                    yield return null;
                }

                logoTime = LogoTime;
                while (logoTime > 0)
                {
                    logoTime -= Time.deltaTime;

                    yield return null;
                }

                break;
            case SplashTransition.LogoOnlyFade:
            case SplashTransition.Dissolve:
                break;
        }

        BlackMask.color = new Color(0, 0, 0, 0);//Color.black
        FadeOut("Title", OutFadeTime);
    }
}
