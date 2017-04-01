using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace Dasher
{
	public class ShopManager : IStoreListener
	{
		public static ShopManager Instance { get { return MainProcess.Instance.ShopManager; } }

		const string c_mainSku = "com.antonmakesgames.dasher.quest";
		public const int c_storyBlockade = 2;

		public ShopManager()
		{
			var module = StandardPurchasingModule.Instance();
			ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
			builder.AddProduct(c_mainSku, ProductType.NonConsumable);
			UnityPurchasing.Initialize(this,builder);
		}

		IStoreController m_controller;
		IExtensionProvider m_extensions;

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			m_controller = controller;
			m_extensions = extensions;
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
			Debug.LogError("Shop init error : " + error.ToString());
		}

		public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
		{
			throw new NotImplementedException();
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
			throw new NotImplementedException();
		}

		public void PurchaseMainQuest()
		{

		}
	}
}
