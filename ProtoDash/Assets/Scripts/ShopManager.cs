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
			ProductType productType = ProductType.NonConsumable;

#if DASHER_IAP_CANCELABLE
			productType = ProductType.Consumable;
#endif

			builder.AddProduct(c_mainSku, productType);
#if !UNITY_EDITOR && !DASHER_NO_IAP
			UnityPurchasing.Initialize(this,builder);
#endif
		}


		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			m_controller = controller;
			m_extensions = extensions;
			if (m_shopInitializationAction != null)
			{
				m_shopInitializationAction(true);
			}
			if (IsMainQuestOwned())
			{
				var saveManager = MainProcess.Instance.DataManager;
				saveManager.IsMainStoryUnlocked = true;
				saveManager.Save();
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
			Debug.LogFormat("Purchase failed : {0}", p);
			if (p == PurchaseFailureReason.DuplicateTransaction)
			{
				if (IsMainQuestOwned())
				{
					if (m_onMainQuestPurchasedAction != null)
					{
						m_onMainQuestPurchasedAction();
						var saveManager = MainProcess.Instance.DataManager;
						saveManager.IsMainStoryUnlocked = true;
						saveManager.Save();
					}
					return;
				}
			}
			if (m_onMainQuestPurchaseErrorAction != null)
			{
				m_onMainQuestPurchaseErrorAction();
			}
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
			bool validPurchase = true; // Presume valid for platforms with no R.V.
			Debug.LogFormat("Purchase success : {0}", e);
			// Unity IAP's validation logic is only included on these platforms.
#if UNITY_ANDROID || UNITY_IOS
			// Prepare the validator with the secrets we prepared in the Editor
			// obfuscation window.
			var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
				AppleTangle.Data(), Application.identifier);
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
			catch (IAPSecurityException exception)
			{
				Debug.LogFormat("Invalid receipt, not unlocking content : {0}" , exception);
				validPurchase = false;
			}
#endif

			if (validPurchase)
			{
				Debug.Log("Valide purchase !");
				//if (String.Equals(e.purchasedProduct.definition.id, c_mainSku, StringComparison.Ordinal))
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
			else
			{
				Debug.Log("Purchase non valide :(");
			}


			return PurchaseProcessingResult.Complete;
		}

		public void PurchaseMainQuest(Action onPurchaseComplete, Action onPurchaseError)
		{
			m_controller.InitiatePurchase(c_mainSku);
			m_onMainQuestPurchasedAction = onPurchaseComplete;
			m_onMainQuestPurchaseErrorAction = onPurchaseError;
		}

		public bool IsMainQuestOwned()
		{
			var product = m_controller.products.WithID(c_mainSku);
			return product.hasReceipt;
		}

		public void RequestRestorePurchase()
		{
#if UNITY_IOS || UNITY_EDITOR
			m_extensions.GetExtension<IAppleExtensions>().RestoreTransactions(result => {
				if (result)
				{
					Debug.Log("RestorePurchase success !");
					// This does not mean anything was restored,
					// merely that the restoration process succeeded.
				}
				else
				{
					Debug.Log("Restore purchase failed !!!");
					// Restoration failed.
				}
			});
#endif
		}

		public void CancelIAPPurchase()
		{
#if DASHER_IAP_CANCELABLE
			//if (IsMainQuestOwned())
			{
				var product = m_controller.products.WithID(c_mainSku);
				m_controller.ConfirmPendingPurchase(product);

				var saveManager = MainProcess.Instance.DataManager;
				saveManager.IsMainStoryUnlocked = false;
				saveManager.Save();
			}
#endif
		}

	}
}
