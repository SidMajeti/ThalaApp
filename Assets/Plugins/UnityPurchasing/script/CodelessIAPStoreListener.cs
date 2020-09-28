#if UNITY_PURCHASING || UNITY_UNIFIED_IAP

using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Automatically initializes Unity IAP with the products defined in the IAP Catalog (if enabled in the UI).
    /// Manages IAPButtons and IAPListeners.
    /// </summary>
    public class CodelessIAPStoreListener : IStoreListener
    {
        private static CodelessIAPStoreListener instance;
        private List<IAPButton> activeButtons = new List<IAPButton>();
        private List<IAPListener> activeListeners = new List<IAPListener> ();
        private static bool unityPurchasingInitialized;

        protected IStoreController controller;
        protected IExtensionProvider extensions;
        protected ProductCatalog catalog;

        public static SubscriptionInfo info;

        public static bool isSubscribed;

        // Allows outside sources to know whether the full initialization has taken place.
        public static bool initializationComplete;

        [RuntimeInitializeOnLoadMethod]
        static void InitializeCodelessPurchasingOnLoad() {
            ProductCatalog catalog = ProductCatalog.LoadDefaultCatalog();
            if (catalog.enableCodelessAutoInitialization && !catalog.IsEmpty() && instance == null)
            {
                CreateCodelessIAPStoreListenerInstance();
            }
        }

        public static void InitializePurchasing()
        {
            StandardPurchasingModule module = StandardPurchasingModule.Instance();
            module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;

            ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);

            IAPConfigurationHelper.PopulateConfigurationBuilder(ref builder, instance.catalog);

            UnityPurchasing.Initialize(instance, builder);

            unityPurchasingInitialized = true;
        }

        private CodelessIAPStoreListener()
        {
            catalog = ProductCatalog.LoadDefaultCatalog();
        }

        public static CodelessIAPStoreListener Instance
        {
            get
            {
                if (instance == null)
                {
                    CreateCodelessIAPStoreListenerInstance();
                }
                return instance;
            }
        }

        /// <summary>
        /// Creates the static instance of CodelessIAPStoreListener and initializes purchasing
        /// </summary>
        private static void CreateCodelessIAPStoreListenerInstance()
        {
            instance = new CodelessIAPStoreListener();
            if (!unityPurchasingInitialized)
            {
                Debug.Log("Initializing UnityPurchasing via Codeless IAP");
                InitializePurchasing();
            }
        }

        public IStoreController StoreController
        {
            get { return controller; }
        }

        public IExtensionProvider ExtensionProvider
        {
            get { return extensions; }
        }

        public bool HasProductInCatalog(string productID)
        {
            foreach (var product in catalog.allProducts)
            {
                if (product.id == productID)
                {
                    return true;
                }
            }
            return false;
        }

        public Product GetProduct(string productID)
        {
            if (controller != null && controller.products != null && !string.IsNullOrEmpty(productID))
            {
                return controller.products.WithID(productID);
            }
            Debug.LogError("CodelessIAPStoreListener attempted to get unknown product " + productID);
            return null;
        }

        public void AddButton(IAPButton button)
        {
            activeButtons.Add(button);
        }

        public void RemoveButton(IAPButton button)
        {
            activeButtons.Remove(button);
        }

        public void AddListener(IAPListener listener)
        {
            activeListeners.Add (listener);
        }

        public void RemoveListener(IAPListener listener)
        {
            activeListeners.Remove (listener);
        }

        public void InitiatePurchase(string productID)
        {
            if (controller == null)
            {
                Debug.LogError("Purchase failed because Purchasing was not initialized correctly");

                foreach (var button in activeButtons)
                {
                    if (button.productId == productID)
                    {
                        button.OnPurchaseFailed(null, Purchasing.PurchaseFailureReason.PurchasingUnavailable);
                    }
                }
                return;
            }

            controller.InitiatePurchase(productID);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            initializationComplete = true;
            this.controller = controller;
            this.extensions = extensions;
            foreach (var button in activeButtons)
            {
                button.UpdateText();
            }

            IAppleExtensions m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
            Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();

            Debug.Log("Available items:");
            foreach (var item in controller.products.all)
            {
                if (item.availableToPurchase)
                {
                    Debug.Log(string.Join(" - ",
                        new[]
                        {
                        item.metadata.localizedTitle,
                        item.metadata.localizedDescription,
                        item.metadata.isoCurrencyCode,
                        item.metadata.localizedPrice.ToString(),
                        item.metadata.localizedPriceString,
                        item.transactionID,
                        item.receipt
                        }));
                // this is the usage of SubscriptionManager class
                    if (item.receipt != null) {
                        if (item.definition.type == ProductType.Subscription) {
                            if (checkIfProductIsAvailableForSubscriptionManager(item.receipt)) {
                                string intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId)) ? null : introductory_info_dict[item.definition.storeSpecificId];
                                SubscriptionManager p = new SubscriptionManager(item, intro_json);
                                info = p.getSubscriptionInfo();
                                if (info.isSubscribed().ToString().Equals("True"))
                                {
                                    isSubscribed = true;
                                }
                                Debug.Log("product id is: " + info.getProductId());
                                Debug.Log("purchase date is: " + info.getPurchaseDate());
                                Debug.Log("subscription next billing date is: " + info.getExpireDate());
                                Debug.Log("is subscribed? " + info.isSubscribed().ToString());
                                Debug.Log("is expired? " + info.isExpired().ToString());
                                Debug.Log("is cancelled? " + info.isCancelled());
                                Debug.Log("product is in free trial peroid? " + info.isFreeTrial());
                                Debug.Log("product is auto renewing? " + info.isAutoRenewing());
                                Debug.Log("subscription remaining valid time until next billing date is: " + info.getRemainingTime());
                                Debug.Log("is this product in introductory price period? " + info.isIntroductoryPricePeriod());
                                Debug.Log("the product introductory localized price is: " + info.getIntroductoryPrice());
                                Debug.Log("the product introductory price period is: " + info.getIntroductoryPricePeriod());
                                Debug.Log("the number of product introductory price period cycles is: " + info.getIntroductoryPricePeriodCycles());
                            } else {
                                Debug.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
                            }
                        } else {
                            Debug.Log("the product is not a subscription product");
                        }
                    } else {
                        Debug.Log("the product should have a valid receipt");
                    }
                }
            }

        }

        private bool checkIfProductIsAvailableForSubscriptionManager(string receipt)
        {
            var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
            if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
            {
                Debug.Log("The product receipt does not contain enough information");
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
                                Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
                                return false;
                            }
                            var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                            if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
                                return false;
                            }
                            var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                            var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                            if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
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
            Debug.LogError(string.Format("Purchasing failed to initialize. Reason: {0}", error.ToString()));
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            PurchaseProcessingResult result;

            // if any receiver consumed this purchase we return the status
            bool consumePurchase = false;
            bool resultProcessed = false;

            foreach (IAPButton button in activeButtons)
            {
                if (button.productId == e.purchasedProduct.definition.id)
                {
                    result = button.ProcessPurchase(e);

                    if (result == PurchaseProcessingResult.Complete) {

                        consumePurchase = true;
                    }

                    resultProcessed = true;
                }
            }

            foreach (IAPListener listener in activeListeners)
            {
                result = listener.ProcessPurchase(e);

                if (result == PurchaseProcessingResult.Complete) {

                    consumePurchase = true;
                }

                resultProcessed = true;
            }

            // we expect at least one receiver to get this message
            if (!resultProcessed) {

                Debug.LogError("Purchase not correctly processed for product \"" +
                                 e.purchasedProduct.definition.id +
                                 "\". Add an active IAPButton to process this purchase, or add an IAPListener to receive any unhandled purchase events.");

            }

            return (consumePurchase) ? PurchaseProcessingResult.Complete : PurchaseProcessingResult.Pending;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            bool resultProcessed = false;

            foreach (IAPButton button in activeButtons)
            {
                if (button.productId == product.definition.id)
                {
                    button.OnPurchaseFailed(product, reason);

                    resultProcessed = true;
                }
            }

            foreach (IAPListener listener in activeListeners)
            {
                listener.OnPurchaseFailed(product, reason);

                resultProcessed = true;
            }

            // we expect at least one receiver to get this message
            if (!resultProcessed) {

                Debug.LogError("Failed purchase not correctly handled for product \"" + product.definition.id +
                                  "\". Add an active IAPButton to handle this failure, or add an IAPListener to receive any unhandled purchase failures.");
            }

            return;
        }
    }
}

#endif
