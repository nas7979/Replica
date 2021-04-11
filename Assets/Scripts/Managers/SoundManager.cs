using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
	private SortedList<string, AudioClip> mSounds = new SortedList<string, AudioClip>();
	[SerializeField]
	private AudioClip[] mAudioClips = new AudioClip[10];
	private float mSFXVolume;
	private float mBGMVolume;
	private LinkedList<AudioSource> mSFXs = new LinkedList<AudioSource>();
	private AudioSource mBGM;

	private void Awake()
	{
		foreach(var iter in mAudioClips)
		{
			if(iter == null)
			{
				break;
			}
			mSounds.Add(iter.name, iter);
		}
		mBGMVolume = 1;
		mSFXVolume = 1;
		DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		LinkedListNode<AudioSource> iter = mSFXs.First;
		for (int i= 0; i < mSFXs.Count; i++)
		{
			if(iter.Value.isPlaying == false)
			{
				iter.Value.gameObject.SetActive(false);
				if (iter.Next != null)
				{
					iter = iter.Next;
					mSFXs.Remove(iter.Previous);
				}
				else
				{
					mSFXs.Remove(iter);
				}
			}
			else
			{
				iter = iter.Next;
			}
		}
	}

	private void OnLevelWasLoaded(int level)
	{
		mSFXs.Clear();
	}

	public void PlaySound(string _Key, bool _IsBGM = false, bool _Loop = false)
	{
		AudioSource Temp;
		Temp = ObjectManager.Instance.CreateOutTile(EnumObjects.SFX).GetComponent<AudioSource>();
		Temp.clip = mSounds[_Key];
		Temp.Play();
		Temp.loop = _Loop;
		if(_IsBGM)
		{
			if (mBGM)
				mBGM.Stop();
			Temp.volume = mBGMVolume;
			mBGM = Temp;
			DontDestroyOnLoad(mBGM.gameObject);
		}
		else
		{
			Temp.volume = mSFXVolume;
			mSFXs.AddFirst(Temp);
		}
	}

	public void SetBGMVolume(float _Volume)
	{
		mBGMVolume = _Volume;
		if (mBGM != null)
		{
			mBGM.volume = _Volume;
		}
	}

	public float GetBGMVolume()
	{
		return mBGMVolume;
	}

	public void SetSFXVolume(float _Volume)
	{
		mSFXVolume = _Volume;
		foreach(var iter in mSFXs)
		{
			iter.volume = _Volume;
		}
	}

	public float GetSFXVolume()
	{
		return mSFXVolume;
	}

	public void PauseAll()
	{
		foreach(var iter in mSFXs)
		{
			iter.Pause();
		}
	}

	public void ResumeAll()
	{
		foreach (var iter in mSFXs)
		{
			iter.UnPause();
		}
	}

	public void StopBGM()
	{
		if(mBGM)
		{
			mBGM.Stop();
			Destroy(mBGM.gameObject);
			mBGM = null;
		}
	}

	public bool IsBGMPlaying()
	{
		return mBGM != null;
	}
}
