using UnityEngine;
using System.Collections;
namespace Dasher
{
	public class GameCanvasHolder : MonoBehaviour
	{
		[HeaderAttribute("Right")]
		public GameObject RightCanvas;
		public GameObject RightTimeText;

		[Space]
		[HeaderAttribute("Left")]
		public GameObject LeftCanvas;
		public GameObject LeftTimeText;
	}
}
