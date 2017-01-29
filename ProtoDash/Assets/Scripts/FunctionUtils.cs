using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Dasher
{
	static class FunctionUtils
	{
		public static float damping(float k, float p, float t, float dt)
		{
			return (t - p) / k * dt + p;
		}
		public static Vector3 damping(float k, Vector3 p, Vector3 t, float dt)
		{
			return (t - p) / k * dt + p;
		}

		public static float mod(float a, float b)
		{
			return a - b * Mathf.Floor(a / b);
		}

		public static Vector3 Floor(Vector3 v)
		{
			return new Vector3(Mathf.Floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z));
		}

#if UNITY_WEBPLAYER
    public static string webplayerQuitURL = "http://google.com";
#endif
		public static void Quit()
		{
#if UNITY_EDITOR
			//UnityEditor.EditorApplication.isPlaying = false;
			UnityEditor.EditorApplication.isPaused = true;
#elif UNITY_WEBPLAYER
        Application.OpenURL(webplayerQuitURL);
#else
        Application.Quit();
#endif
		}
	}
}
