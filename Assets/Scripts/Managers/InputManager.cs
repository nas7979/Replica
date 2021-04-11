using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
	public enum SlideDirection
	{
		Right,
		Down,
		Left,
		Up,
		None
	}

	private SlideDirection mSlideDirection = SlideDirection.None;
	private Vector2 mSlideStartPos;
	[SerializeField]
	private float mSlideMinDist;
	private bool mIsSliding = false;
	private bool mIsIgnoreInput = false;
	private Vector2[] mSlideDirectionVector = new Vector2[4] { new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(0, 1) };

    // Update is called once per frame
    void Update()
    {
		mSlideDirection = SlideDirection.None;

		if (mIsIgnoreInput == false)
		{
			if (Input.GetMouseButtonDown(0))
			{
				mSlideStartPos = Input.mousePosition;
				mIsSliding = true;
			}

			if (mIsSliding == true && Input.GetMouseButtonUp(0))
			{
				mIsSliding = false;
				if (Vector2.Distance(mSlideStartPos, Input.mousePosition) >= mSlideMinDist)
				{
					int Direction = Mathf.FloorToInt((Vector2.SignedAngle(mSlideStartPos - (Vector2)Input.mousePosition, Vector2.right) + 225) % 360 / 90);
					mSlideDirection = (SlideDirection)Direction;
				}
			}

			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				mSlideDirection = SlideDirection.Up;
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				mSlideDirection = SlideDirection.Down;
			}
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				mSlideDirection = SlideDirection.Left;
			}
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				mSlideDirection = SlideDirection.Right;
			}
		}
    }

	public SlideDirection GetSlideDirection()
	{
		return mSlideDirection;
	}

	public Vector2 GetSlideDirectionVector()
	{
		return mSlideDirectionVector[(int)mSlideDirection];
	}

	public void SetIsIgnoreInput(bool _Ignore)
	{
		mIsIgnoreInput = _Ignore;
	}
}
