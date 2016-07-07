using UnityEngine;
namespace Assets
{
	static class FuctionUtils
	{
		public static float damping(float k, float p, float t, float dt)
		{
			return (t - p) / k * dt + p;
		}
		public static Vector3 damping(float k , Vector3 p, Vector3 t, float dt)
		{
			return (t - p) / k * dt + p;
		}

		public static float mod(float a, float b)
		{
			return a - b * Mathf.Floor(a / b);
		}
	}
}
