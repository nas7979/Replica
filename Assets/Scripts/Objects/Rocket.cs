using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private Animator mAnimator;
    [SerializeField]
    private float mFallTargetY;
    [SerializeField]
    private float mLandingTargetY;
    [SerializeField]
    private float mFallSpeed;
    [SerializeField]
    private float mDestroySpeed;
    [SerializeField]
    private const float mReplicaSpawnOffsetY = -2.8f;

    private GameObject mReplica; 

    private CameraControl mCameraControl;

    private void Awake()
    {
        mAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        GameManager.Instance.SetSlideAble(false);
        transform.position = new Vector3(0, 12);
        mCameraControl = GameManager.Instance.GetCameraControl();
        mCameraControl.StartCoroutine(mCameraControl.CR_ZoomIn(4f));
        mCameraControl.SetTargetObject(gameObject);
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        mReplica = GameManager.Instance.GetReplica();
        mReplica.SetActive(false);
        StartCoroutine(CR_Fall());
    }

    public IEnumerator CR_Fall()
    {
        while (transform.position.y > mFallTargetY)
        {
            GameManager.Instance.GetReplica().transform.position = transform.position + new Vector3(0, mReplicaSpawnOffsetY, 0);
            transform.position += Vector3.down * mFallSpeed * Time.deltaTime;
            yield return null;
        }
        transform.position = new Vector3(0, mFallTargetY);
        StartCoroutine(CR_Landing());
        yield break;
    }

    public IEnumerator CR_Landing()
    {
        mAnimator.SetTrigger("Landing");
        while (transform.position.y > mLandingTargetY)
        {
            GameManager.Instance.GetReplica().transform.position = transform.position + new Vector3(0, mReplicaSpawnOffsetY, 0);;
            transform.position += Vector3.down * (mFallSpeed * 0.25f) * Time.deltaTime;
            yield return null;
        }
        StartCoroutine(CR_Open());
        transform.position = new Vector3(0, mLandingTargetY);
    }

    public IEnumerator CR_Open()
    {
        mAnimator.SetTrigger("Open");
        while (true)
        {
            if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Rocket_Open_anim") && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                mReplica.SetActive(true);
                //mReplica.GetComponent<Object>().CreateAnimation(mReplica.transform.localScale);
                yield return new WaitForSeconds(1f);
                mCameraControl.StartCoroutine(mCameraControl.CR_ZoomOut());
                StartCoroutine(CR_Destroy());
                GameManager.Instance.SetSlideAble(true);
                yield break;
            }
            yield return null;
        }
    }

    public IEnumerator CR_Destroy()
    {
        SpriteRenderer mSpriteRenderer =  GetComponent<SpriteRenderer>();
        while(mSpriteRenderer.color.a > 0)
        {
            mSpriteRenderer.color -= Time.deltaTime * new Color(0, 0, 0, mDestroySpeed);
            yield return null;
        }
        gameObject.SetActive(false);
        yield break;
    }
}
