using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class ComposedStretchableCircle : MonoBehaviour
	{
		//[SerializeField]
		//GameObject m_left = null;
		[SerializeField]
		GameObject m_center = null;
		[SerializeField]
		GameObject m_right = null;

		public void SetLength(float length)
		{
			length /= transform.localScale.x * transform.parent.localScale.x;
			var scale = m_center.transform.localScale;
			scale.x = length;
			m_center.transform.localScale = scale;
			m_center.transform.localPosition = new Vector3((length / 2f), 0, 0);
			m_right.transform.localPosition = new Vector3(length + .5f, 0, 0);
		}

		public void SetTwoPoints(Vector3 start, Vector3 end)
		{
			start.z = transform.position.z;
			end.z = transform.position.z;

			transform.position = start;
			float length = Vector3.Distance(start, end);
			SetLength(length);
			//transform.rotation = Quaternion.FromToRotation(Vector3.right, end - start);
			var dir = end - start;
			var angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}

		public void Orient(Vector3 start, Vector3 end)
		{
			start.z = transform.position.z;
			end.z = transform.position.z;
			transform.position = start;
			SetLength(0f);
			//transform.rotation = Quaternion.FromToRotation(Vector3.right, end - start);
			var dir = end - start;
			var angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
	}
}
