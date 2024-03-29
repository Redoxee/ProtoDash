﻿using UnityEngine;
using System.Collections.Generic;

namespace Dasher
{
	public class TutorialAnimator : MonoBehaviour
	{

		[SerializeField]
		float animationTime = 2f;

		[SerializeField]
		private List<ExpandableCircle> animatedCircles;
		[SerializeField]
		private float animatedTimeStart = 0f;

		private bool hasNotifiedCircles = false;


		private float animationTimer;

		// Use this for initialization
		void Start()
		{
			animationTimer = 0f;
		}

		// Update is called once per frame
		void Update()
		{
			float dt = GameProcess.Instance.GameTime.GetGameDeltaTime();
			animationTimer = (animationTimer + dt);
			if (animationTimer > animationTime)
			{
				hasNotifiedCircles = false;
				animationTimer %= animationTime;
			}

			if (animationTimer >= animatedTimeStart && !hasNotifiedCircles)
			{
				for (int i = 0; i < animatedCircles.Count; ++i)
				{
					animatedCircles[i].StartAnimation();
				}
				hasNotifiedCircles = true;
			}
		}
	}
}
