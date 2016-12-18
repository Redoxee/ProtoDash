using UnityEngine;
using System.Collections.Generic;

namespace Dasher
{
	public class InputManager:MonoBehaviour
	{
		[SerializeField]
		private float m_swipeInputDistance = .25f;
		private float m_squareSwipeInputTrigger;

		[SerializeField]
		private float m_swipeDeadZone = .4f;

		//[SerializeField]
		private int m_dashFrameBuffer = 5;

		private int m_dashFrameCount = 0;

		//[SerializeField]
		private List<Rect> m_buttonsList = new List<Rect>();
		public void RegisterButton(Rect rect)
		{
			if (!m_buttonsList.Contains(rect))
				m_buttonsList.Add(rect);
		}
		public void UnregisterButton(Rect rect)
		{
			var i = m_buttonsList.IndexOf(rect);
			if (i > -1)
				m_buttonsList.RemoveAt(i);
		}


		private Vector2 m_currentPointerPosition;


		private bool m_isMouseDown = false;
		private bool m_isMousePressed = false;
		private bool m_isSweeping = false;

		#region Accessor
		public bool IsRequestingJump { get { return m_isMouseDown; } }
		public bool IsRequestingDash { get { return m_isSweeping || m_dashFrameCount > 0; } }
		#endregion


		private float m_screenRatio;
		private Vector2 m_tapPosition;

		void Awake()
		{

			Rect cameraRect = Camera.main.pixelRect;
			if (cameraRect.width < cameraRect.height)
			{
				m_screenRatio = 1.0f / cameraRect.width;
			}
			else
			{
				m_screenRatio = 1.0f / cameraRect.height;
			}
		}

		public void Initialize()
		{
			m_tapPosition = Input.mousePosition;
			m_currentPointerPosition = Input.mousePosition;
			m_squareSwipeInputTrigger = m_swipeInputDistance * m_swipeInputDistance;
		}
		bool IsInButtons(Vector2 pos)
		{
			int rectCount = m_buttonsList.Count;
			for (int i = 0; i < rectCount; ++i)
			{
				var r = m_buttonsList[i];
				
				//if (RectTransformUtility.ScreenPointToWorldPointInRectangle(r, pos))
				if (r.Contains(pos))
						return true;
			}

			return false;
		}
		public void ManualUpdate()
		{
			m_isMousePressed = Input.GetMouseButton(0);
			m_currentPointerPosition = Input.mousePosition;

			if (!IsInButtons(m_currentPointerPosition))
			{
				if (Input.GetMouseButtonDown(0))
				{
					m_isMouseDown = true;
					m_tapPosition = m_currentPointerPosition;
				}

				Vector2 slideVector = m_tapPosition - m_currentPointerPosition;
				slideVector.x *= m_screenRatio;
				slideVector.y *= m_screenRatio;

				m_isSweeping = (!m_isMouseDown) && m_isMousePressed && m_squareSwipeInputTrigger < slideVector.sqrMagnitude;
				if (m_isSweeping)
				{
					m_dashFrameCount = m_dashFrameBuffer;
				}
			}
		}

		public void ManualFixedUpdate()
		{
			m_isMouseDown = false;
			if (m_dashFrameCount > 0)
			{
				//Debug.Log("db : " + m_dashFrameCount);
				m_dashFrameCount -= 1;
			}
		}

		public void NotifyBeginDash()
		{
			m_dashFrameCount = 0;
			m_tapPosition = m_currentPointerPosition;
		}

		public void NotifyEndDash()
		{
			m_tapPosition = m_currentPointerPosition;
		}

		public Vector3 GetDashVector()
		{
			Vector3 dv = GetDashVectorFromSwipe((m_currentPointerPosition - m_tapPosition).normalized);
			
			return dv;
		}

		private Vector3 GetDashVectorFromSwipe(Vector2 sw)
		{
			Vector3 res = new Vector3(0, 0, 0);
			if (sw.x > m_swipeDeadZone)
			{
				res.x += 1;
			}
			else if (sw.x < -m_swipeDeadZone)
			{
				res.x -= 1;
			}
			if (sw.y > m_swipeDeadZone)
			{
				res.y += 1;
			}
			else if (sw.y < -m_swipeDeadZone)
			{
				res.y -= 1;
			}
			return res;
		}
	}
}