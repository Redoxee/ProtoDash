using System;
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
		public static ShopManager Instance { get { return MainProcess.Instance.ShopManager; } }

		const string c_mainSku = "com.antonmakesgames.dasher.mainstory";
		public const int c_storyBlockade = 2;

		Action m_onMainQuestPurchasedAction = null;

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
			bool validPurchase = true; // Presume valid for platforms with no R.V.

			// Unity IAP's validation logic is only included on these platforms.
#if UNITY_ANDROID || UNITY_IOS
			// Prepare the validator with the secrets we prepared in the Editor
			// obfuscation window.
			var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
				AppleTangle.Data(), Application.bundleIdentifier);
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

		public void PurchaseMainQuest(Action onPurchaseComplete)
		{
			m_controller.InitiatePurchase(c_mainSku);
			m_onMainQuestPurchasedAction = onPurchaseComplete;
		}
	}
}
