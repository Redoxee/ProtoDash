using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dasher
{
	public class PurchaseErrorPopup : MonoBehaviour
	{

		public void ShowPopup()
		{
			gameObject.SetActive(true);
		}

		public void HidePopup()
		{
			gameObject.SetActive(false);
		}
	}
}
