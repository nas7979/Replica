using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 광고 쿨타임 등 글로벌 코루틴을 관리하는 매니저
/// </summary>
public class GlobalCoroutineManager : Singleton<GlobalCoroutineManager>
{

    //C# 델리게이트(c++ 함수 포인터와 비슷함) : https://www.csharpstudy.com/CSharp/CSharp-delegate.aspx
    public delegate void FuncDelegate();
    public delegate bool FuncConditionDelegate();

    public Dictionary<string, float> Dictionary_Variables;

    private void Awake()
    {
        GameObject Temp = GameObject.Find(gameObject.name);
        if (Temp != gameObject)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        Dictionary_Variables = new Dictionary<string, float>();
    }

    public void StartGlobalRoutine(IEnumerator routine) {
        StartCoroutine(routine);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="conditionFunc">true일때까지 코루틴을 반복한다.</param>
    /// <param name="startEvents">start일 때 실행할 함수들</param>
    /// <param name="playEvents">playing중일 때 실행할 함수들</param>
    /// <param name="endEvents">end일 때 실행할 함수들</param>
    /// <returns></returns>
    public IEnumerator AnyRoutine(FuncConditionDelegate conditionFunc, List<FuncDelegate> startEvents = null, 
        List<FuncDelegate> playEvents = null, List<FuncDelegate> endEvents = null)
    {
        foreach (FuncDelegate e in startEvents)
        {
            e();
        }

        while (conditionFunc())
        {
            foreach (FuncDelegate e in playEvents)
            {
                e();
            }
            yield return null;
        }

        foreach (FuncDelegate e in endEvents)
        {
            e();
        }
        yield break;
    }
}
