﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T instance;

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<T>() as T;

				if (instance == null)
				{
					instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
				}
			}
			return instance;
		}
	}
}
