﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

namespace Dasher
{
	public class ShopManager : IStoreListener
	{
		public static ShopManager Instance { get {
				return MainProcess.Instance.ShopManager;
			} }

		const string c_mainSku = "com.antonmakesgames.dasher.mainstory";
		public const int c_storyBlockade = 2;

		IStoreController m_controller;
		IExtensionProvider m_extensions;
		public bool IsInitialized { get { return m_controller != null; } }

		Action<bool> m_shopInitializationAction = null;
		public Action<bool> ShopInitializationCallBack { set { m_shopInitializationAction = value; } }

		Action m_onMainQuestPurchasedAction = null;
		Action m_onMainQuestPurchaseErrorAction = null;

		public ShopManager()
		{
			var module = StandardPurchasingModule.Instance();
			ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
			builder.AddProduct(c_mainSku, ProductType.NonConsumable);
			UnityPurchasing.Initialize(this,builder);
		}


		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			m_controller = controller;
			m_extensions = extensions;
			if (m_shopInitializationAction != null)
			{
				m_shopInitializationAction(true);
			}
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
			Debug.LogError("Shop init error : " + error.ToString());
			if (m_shopInitializationAction != null)
			{
				m_shopInitializationAction(false);
			}
		}

		public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
		{
			if (m_onMainQuestPurchaseErrorAction != null)
			{
				m_onMainQuestPurchaseErrorAction();
			}
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
			bool validPurchase = true; // Presume valid for platforms with no R.V.

			// Unity IAP's validation logic is only included on these platforms.
#if UNITY_ANDROID || UNITY_IOS
			// Prepare the validator with the secrets we prepared in the Editor
			// obfuscation window.
			var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
				AppleTangle.Data(), Application.identifier);
			bool mainQuestUnlocked = false;
			try
			{
				// On Google Play, result has a single product ID.
				// On Apple stores, receipts contain multiple products.
				var result = validator.Validate(e.purchasedProduct.receipt);
				// For informational purposes, we list the receipt(s)
				Debug.Log("Receipt is valid. Contents:");
				foreach (IPurchaseReceipt productReceipt in result)
				{
					Debug.Log(productReceipt.productID);
					Debug.Log(productReceipt.purchaseDate);
					Debug.Log(productReceipt.transactionID);
				}
			}
			catch (IAPSecurityException)
			{
				Debug.Log("Invalid receipt, not unlocking content");
				validPurchase = false;
			}
#endif

			if (validPurchase)
			{
				if (String.Equals(e.purchasedProduct.definition.id, c_mainSku, StringComparison.Ordinal))
				{
					var saveManager = MainProcess.Instance.DataManager;
					saveManager.IsMainStoryUnlocked = true;
					saveManager.Save();
					if (m_onMainQuestPurchasedAction != null)
					{
						m_onMainQuestPurchasedAction();
					}
				}
			}

			return PurchaseProcessingResult.Complete;
		}

		public void PurchaseMainQuest(Action onPurchaseComplete, Action onPurchaseError)
		{
			m_controller.InitiatePurchase(c_mainSku);
			m_onMainQuestPurchasedAction = onPurchaseComplete;
			m_onMainQuestPurchaseErrorAction = onPurchaseError;
		}
	}
}
