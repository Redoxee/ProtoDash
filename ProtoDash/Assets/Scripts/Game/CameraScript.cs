using System;
using UnityEngine;

namespace Dasher
{
	public class CameraScript : MonoBehaviour
	{

		[SerializeField]
		private Character character = null;

		[SerializeField]
		private float xDampingFactor = 0.5f;
		[SerializeField]
		private float yDampingFactor = 0.25f;

		[SerializeField]
		private float cameraYOffset = 6;
		[SerializeField]
		private float wantedXOffset = 6;

		[SerializeField]
		private float centerXoffset = 0.0f;


		[SerializeField]
		private float timeToPosX = 1.5f;
		[SerializeField]
		private AnimationCurve repositionXCurve = null;

		private float maxVerticalOffset = 9f;

		private float lastOffsetX = 0.0f;
		private float lastOrientation = 0.0f;
		private float timerPosX = 0.0f;


		void Start()
		{
			InitStates();
			GameProcess.Instance.RegisterCamera(this);
		}

		public void OnDisable()
		{
			GameProcess.Instance.UnregisterCamera();
		}
		

		public void ManualUpdate()
		{
			m_currentState.Update();
		}


		public void ManualLateUpdate()
		{
			m_currentState.LateUpdate();
		}

		#region In Game
		void Game_Update()
		{
			if (character.isPaused)
			{
				return;
			}

			if (lastOrientation != character.getFacingSign())
			{
				lastOrientation = character.getFacingSign();
				timerPosX = timeToPosX;
				lastOffsetX = transform.position.x - character.transform.position.x;
			}
			if (timerPosX > 0)
			{
				float dt = GameProcess.Instance.GameTime.GetGameDeltaTime();
				timerPosX = Mathf.Max(0, timerPosX - dt);
			}
		}

		private void _DrawCross(float x, float y, Color col)
		{
			Debug.DrawRay(new Vector3(x - .5f, y, 0.0f), new Vector3(1f, 0.0f, 0.0f), col);
			Debug.DrawRay(new Vector3(x, y - .5f, 0.0f), new Vector3(0f, 1f, 0.0f), col);
		}

		void Game_LateUpdate()
		{
			if (character.isPaused)
			{
				return;
			}

			float orientation = character.getFacingSign();
			float currentOffsetX = wantedXOffset * orientation;
			if (timerPosX > 0)
			{
				float progression = 1.0f - repositionXCurve.Evaluate(1.0f - timerPosX / timeToPosX);

				currentOffsetX = currentOffsetX + (lastOffsetX - currentOffsetX) * progression;
				orientation = lastOrientation;

			}

			Vector3 p = character.transform.position;
			float tpx = p.x + currentOffsetX + centerXoffset;
			float tpy = p.y + cameraYOffset;
			_DrawCross(tpx, tpy, Color.yellow);
			float dt = GameProcess.Instance.GameTime.GetGameDeltaTime();
			tpx = FunctionUtils.damping(xDampingFactor, transform.position.x, tpx, dt);
			tpy = FunctionUtils.damping(yDampingFactor, transform.position.y, tpy, dt);
			_DrawCross(p.x + wantedXOffset * orientation, p.y + cameraYOffset, Color.gray);
			_DrawCross(tpx, tpy, Color.cyan);

			if (transform.position.y - p.y > maxVerticalOffset)
			{
				tpy = p.y + maxVerticalOffset;
			}

			transform.position = new Vector3(tpx, tpy, transform.position.z);
		}
		#endregion

		#region End Game 

		Vector2 m_targetEndPosition;

		public void NotifyEndGame(Vector2 endGamePosition)
		{
			m_targetEndPosition = endGamePosition;
			m_currentState = EndGameSate;
		}

		void EndGame_Update()
		{
		}

		void EndGame_LateUpdate()
		{
			float dt = Time.deltaTime;
			float tpx = FunctionUtils.damping(xDampingFactor, transform.position.x, m_targetEndPosition.x, dt);
			float tpy = FunctionUtils.damping(yDampingFactor, transform.position.y, m_targetEndPosition.y, dt);

			transform.position = new Vector3(tpx, tpy, transform.position.z);
		}

		#endregion

		#region DeathState

		FSM_STATE DeathState;
		Vector3 m_deathPos;
		public void NotifyDeath(Vector3 pos)
		{
			m_deathPos = pos;
			m_currentState = DeathState;
		}

		void Death_Update()
		{
		}

		void Death_LateUpdate()
		{
			float dt = Time.deltaTime;
			float tpx = FunctionUtils.damping(xDampingFactor, transform.position.x, m_deathPos.x, dt);
			float tpy = FunctionUtils.damping(yDampingFactor, transform.position.y, m_deathPos.y, dt);
			transform.position = new Vector3(tpx, tpy, transform.position.z);
			_DrawCross(m_deathPos.x, m_deathPos.y, Color.black);
		}

		#endregion
		#region FSM

		private class FSM_STATE
		{
			public Action Update;
			public Action LateUpdate;

			public FSM_STATE(Action udpt, Action lateUdpt)
			{
				Update = udpt;
				LateUpdate = lateUdpt;
			}
		}

		FSM_STATE GameState;
		FSM_STATE EndGameSate;

		FSM_STATE m_currentState;

		void InitStates()
		{
			GameState = new FSM_STATE(Game_Update, Game_LateUpdate);
			EndGameSate = new FSM_STATE(EndGame_Update, EndGame_LateUpdate);
			DeathState = new FSM_STATE(Death_Update, Death_LateUpdate);

			m_currentState = GameState;
		}

		#endregion
	}


}
