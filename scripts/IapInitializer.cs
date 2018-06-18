using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IapInitializer : MonoBehaviour,  IStoreListener
{
	void Awake()
	{
		ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		builder.AddProduct("levelpackfoo", ProductType.NonConsumable);
		UnityPurchasing.Initialize(this, builder);
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {}
	public void OnInitializeFailed(InitializationFailureReason error) {}
	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e) { return PurchaseProcessingResult.Complete; }
	public void OnPurchaseFailed(Product item, PurchaseFailureReason r) {}
}
