using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    private float mRotateSpeed;
    [SerializeField]
    private float mZoomSpeed;
    private float mOrgCameraSize;
    private Vector3 mOrgCameraPos;

    [SerializeField]
    private GameObject mTargetObject;
    [SerializeField]
    private float mMoveSpeed =  0f;
    private Camera mCamera;

    private void Awake()
    {
        mCamera = GetComponent<Camera>();
        mOrgCameraSize = mCamera.orthographicSize;
        mOrgCameraPos = mCamera.transform.position;
        StartCoroutine(CR_CameraUpdate());
    }

    private void OnEnable()
    {
        StartCoroutine(CR_CameraUpdate());
    }

    public IEnumerator CR_ZoomIn(float _TargetSize)
    {
        while(mCamera.orthographicSize > _TargetSize)
        {
            mCamera.orthographicSize -= Time.deltaTime * mZoomSpeed;
            yield return null;
        }
        mCamera.orthographicSize = _TargetSize;
        yield break;
    }

    public IEnumerator CR_ZoomOut()
    {
        mTargetObject = null;
        while (true)
        {
            mCamera.orthographicSize += Time.deltaTime * mZoomSpeed;
            if(mCamera.orthographicSize >= mOrgCameraSize)
            {
        mCamera.orthographicSize = mOrgCameraSize;
                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator CR_CameraUpdate()
    {
        Vector3 newPos;
        while (gameObject.activeSelf)
        {
            if(mTargetObject)
            {
                Vector3 TargetPos = new Vector3(mTargetObject.transform.position.x, mTargetObject.transform.position.y, transform.position.z);
                newPos = Vector3.Lerp(transform.position, TargetPos, mMoveSpeed);
                newPos.x = Mathf.Clamp(newPos.x, (5.4f - mCamera.orthographicSize *(5.4f/9.6f))*-1, 5.4f - mCamera.orthographicSize * (5.4f / 9.6f));
                newPos.y = Mathf.Clamp(newPos.y, (9.6f - mCamera.orthographicSize) * -1, 9.6f - mCamera.orthographicSize);
                transform.position = newPos;
            }
            else
            {
                newPos = Vector3.Lerp(transform.position, mOrgCameraPos, mMoveSpeed);
                newPos.x = Mathf.Clamp(newPos.x, (5.4f - mCamera.orthographicSize * (5.4f / 9.6f)) * -1, 5.4f - mCamera.orthographicSize * (5.4f / 9.6f));
                newPos.y = Mathf.Clamp(newPos.y, (9.6f - mCamera.orthographicSize) * -1, 9.6f - mCamera.orthographicSize);
                transform.position = newPos;
            }
            yield return null;
        }
        yield break;
    }

    public void SetTargetObject(GameObject _TartgetObject)
    {
        mTargetObject = _TartgetObject;
    }

}
