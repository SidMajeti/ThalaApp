using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;

// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class Purchaser : MonoBehaviour, IStoreListener
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void showNativeAlert(String title, String msg, String b1);
    [DllImport("__Internal")]
    private static extern void stopAudioEngine();
    [DllImport("__Internal")]
    private static extern void restartAudioEngine();
#endif

    ITransactionHistoryExtensions m_TransactionHistoryExtensions;
    IStoreController m_StoreController;          // The Unity Purchasing system.
    IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    public static string kProductIDSubscription = "com.sidmajeti.thala.in_app_subs";

    // Apple App Store-specific product identifier for the subscription product.
    private static string kProductNameAppleSubscription = "com.sidmajeti.thala.in_app_subs";

    // Google Play Store-specific product identifier subscription product.
    private static string kProductNameGooglePlaySubscription = "com.sidmajeti.thala.in_app_subs";

    public Dropdown thalaDropdown;
    public GameObject panel;
    public Button subsButton;

    double tstamp;
    bool wasPaused;

    public bool loadCircle;

    bool isSubscribed;

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.

        //Debug.Log("OnInitializedCalled");
        m_TransactionHistoryExtensions = extensions.GetExtension<ITransactionHistoryExtensions>();

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;

        IAppleExtensions m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
        Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();

        foreach (var item in controller.products.all)
        {
            if (item.availableToPurchase)
            {
                //Debug.Log(string.Join(" - ",
                //    new[]
                //    {
                //        item.metadata.localizedTitle,
                //        item.metadata.localizedDescription,
                //        item.metadata.isoCurrencyCode,
                //        item.metadata.localizedPrice.ToString(),
                //        item.metadata.localizedPriceString,
                //        item.transactionID,
                //        item.receipt
                //    }));
                // this is the usage of SubscriptionManager class
                if (item.receipt != null)
                {
                    if (item.definition.type == ProductType.Subscription)
                    {
                        if (checkIfProductIsAvailableForSubscriptionManager(item.receipt))
                        {
                            string intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId)) ? null : introductory_info_dict[item.definition.storeSpecificId];
                            SubscriptionManager p = new SubscriptionManager(item, intro_json);
                            SubscriptionInfo info = p.getSubscriptionInfo();
                            if (info.isSubscribed().ToString().Equals("True"))
                            {
                                // set timestamp each time user is subscribed and you have reinitialized/requeried
                                tstamp = System.DateTime.Now.ToOADate();

                                if (subsButton.gameObject.activeSelf)
                                {
                                    List<string> options = new List<string> { "Adi Thalam", "Khanda Chapu Thalam", "Misra Chapu Thalam", "Ata Thalam" };
                                    thalaDropdown.AddOptions(options);
                                    subsButton.gameObject.SetActive(false);
                                    panel.gameObject.SetActive(false);
                                }
                                isSubscribed = true;
                                //allow user to access full app
                            }
                        }
                        else
                        {
                            //Debug.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
                        }
                    }
                    else
                    {
                        //Debug.Log("the product is not a subscription product");
                    }
                }
                else
                {
                    //Debug.Log("the product should have a valid receipt");
                    //either do this or check whether you can use is expired instead of recalling/requerying each time
                    if (wasPaused)
                    {
                        if (!subsButton.gameObject.activeSelf)
                        {
                            List<string> options = new List<string> { "Adi Thalam", "Khanda Chapu Thalam", "Misra Chapu Thalam", "Ata Thalam" };
                            thalaDropdown.options.RemoveRange(1, 4);
                            thalaDropdown.RefreshShownValue();
                            thalaDropdown.value = 0;
                            subsButton.gameObject.SetActive(true);
                            panel.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }

    }


    //void OnApplicationFocus(bool focus)
    //{
    //if (focus)
    //{
    //    Debug.Log("Entered focus function");
    //    LocationInfo locationInfo = new LocationInfo();
    //    Debug.Log(locationInfo.timestamp - tstamp);
    //    if (locationInfo.timestamp - tstamp >= 20)
    //    {
    //        InitializePurchasing();
    //    }
    //}
    //}

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        //want to initialize again for checking if they are subscribed
        //if (IsInitialized())
        //{
        //    // ... we are done here.
        //    return;
        //}
        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        // if the Product ID was configured differently between Apple and Google stores. Also note that
        // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
        // must only be referenced here.

        builder.AddProduct(kProductIDSubscription, ProductType.Subscription);

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);

        //Debug.Log("FinishedInitializing");
    }


    public bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuySubscription()
    {
        // Buy the subscription product using its the general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        // Notice how we use the general product identifier in spite of this ID being mapped to
        // custom store-specific identifiers above.
        BuyProductID(kProductIDSubscription);
    }


    public void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            loadCircle = true;
            subsButton.interactable = false;
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
            }
        }
        // Otherwise ...
        else
        {
            //Debug.Log("IAP has not initialized yet!");
            showNativeAlert("Network Error", "Please check your connection to the internet and try again.", "OK");
            //show pop-up telling user that IAP has not initialized yet

        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            return;
        }

        // If we are running on an Apple device ... 
#if UNITY_IOS
        
            // ... begin restoring purchases

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
            }); 
#endif

    }

    private bool checkIfProductIsAvailableForSubscriptionManager(string receipt)
    {
        var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
        if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
        {
            //Debug.Log("The product receipt does not contain enough information");
            return false;
        }
        var store = (string)receipt_wrapper["Store"];
        var payload = (string)receipt_wrapper["Payload"];

        if (payload != null)
        {
            switch (store)
            {
                case GooglePlay.Name:
                    {
                        var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                        if (!payload_wrapper.ContainsKey("json"))
                        {
                            //Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
                            return false;
                        }
                        var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                        if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload"))
                        {
                            //Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
                            return false;
                        }
                        var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                        var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                        if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial"))
                        {
                            //Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                            return false;
                        }
                        return true;
                    }
                case AppleAppStore.Name:
                case AmazonApps.Name:
                case MacAppStore.Name:
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
        return false;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        //show pop-up
        showNativeAlert("Initialization Failed", "Failure Reason: " + error.ToString() + ". Please Try Again.", "OK");
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        //Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        //Debug.Log("InitializationFailed");
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        //Debug.Log("Process Purchase is called"); // A consumable product has been purchased by this user.
        loadCircle = false;
        if (String.Equals(args.purchasedProduct.definition.id, kProductIDSubscription, StringComparison.Ordinal))
        {
            if (subsButton.gameObject.activeSelf)
            {
                //Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                List<string> options = new List<string> { "Adi Thalam", "Khanda Chapu Thalam", "Misra Chapu Thalam", "Ata Thalam" };
                thalaDropdown.AddOptions(options);
                subsButton.gameObject.SetActive(false);
                panel.gameObject.SetActive(false);
            }
            isSubscribed = true;
            tstamp = System.DateTime.Now.ToOADate();
            //fill in the dropdown with values
        }
        // Or ... an unknown product has been purchased by this user. Fill in additional products here....
        else
        {
            //Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        loadCircle = false;
        subsButton.interactable = true;
        if (!failureReason.ToString().Equals("UserCancelled"))
        {
            showNativeAlert("Your Purchase Could Not Be Completed", "For assistance, contact iTunes Support at www.apple.com/support/itunes/ww/.", "OK");
        }
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        //Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        //Debug.Log("Store specific error code: " + m_TransactionHistoryExtensions.GetLastStoreSpecificPurchaseErrorCode());
        if (m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription() != null)
        {
            //Debug.Log("Purchase failure description message: " +
            //m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription().message);
        }
    }
    public void Awake()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
        SplashScreen.Begin();
    }

    void OnApplicationPause(bool pause)
    {
        wasPaused = true;
        if (pause)
        {
            stopAudioEngine();
        }
        //only check when user has subscribed
        else
        {
            //Debug.Log("Restarted Engine from Unity");
            restartAudioEngine();
            if (isSubscribed)
            {
                if (System.DateTime.Now.ToOADate() - tstamp >= 1)
                {
                    InitializePurchasing();
                }
            }
        }
    }
}

