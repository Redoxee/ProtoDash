using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class FullScreenScaler : MonoBehaviour
	{
		[SerializeField]
		AnimationCurve m_ScaleAnimation = AnimationCurve.EaseInOut(0, 0, 1, 1);
		
		public Color StartColor = Color.white;
		public Color EndColor = Color.black;
		
		Material m_materialRef;


		Vector3 m_OriginalScale;
		Vector3 m_offsetScale;
		void Awake()
		{
			m_OriginalScale = transform.localScale;
			var mainCam = Camera.main;
			float r = Screen.width > Screen.height ? ((float)Screen.width / (float)Screen.height) : ((float)Screen.height / (float)Screen.width);
			float s = mainCam.orthographicSize * 2f * r * 1.5f;
			m_offsetScale = Vector3.one * s - m_OriginalScale;
			m_offsetScale.z = 1;

			m_materialRef = GetComponent<Renderer>().material;
			m_materialRef.color = StartColor;
		}
		

		public void SetProgression(float progression)
		{
			progression = Mathf.Clamp01(progression);
			transform.localScale = m_OriginalScale + m_offsetScale * m_ScaleAnimation.Evaluate(progression);
			m_materialRef.color = Color.Lerp(StartColor, EndColor, progression);
		}
	}
}
