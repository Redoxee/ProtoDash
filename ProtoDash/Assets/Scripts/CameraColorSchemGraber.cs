using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class CameraColorSchemGraber : MonoBehaviour
	{


		void Start()
		{
			Camera camera = GetComponent<Camera>();
			camera.backgroundColor = MainProcess.Instance.m_colorScheme.MainBackGround;
		}

	}
}
