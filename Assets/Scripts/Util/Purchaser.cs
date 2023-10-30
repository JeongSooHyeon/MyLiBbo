using UnityEngine;
//using UnityEngine.Purchasing;
//using UnityEngine.Purchasing.Security;

// Placing the Purchaser class in the CompleteProject namespace allows it to interact with ScoreManager, 
// one of the existing Survival Shooter scripts.
namespace CompleteProject
{
    // Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
    public class Purchaser : MonoBehaviour//, IStoreListener
    {
        //private static IStoreController m_StoreController;          // The Unity Purchasing system.
        //private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

        // Product identifiers for all products capable of being purchased: 
        // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
        // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
        // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

        // General product identifiers for the consumable, non-consumable, and subscription products.
        // Use these handles in the code to reference which product to purchase. Also use these values 
        // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
        // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
        // specific mapping to Unity Purchasing's AddProduct, below.
        public static string kProductIDConsumable = "consumable";
        public static string kProductIDNonConsumable = "nonconsumable";
        public static string kProductIDSubscription = "subscription";

        // Apple App Store-specific product identifier for the subscription product.
        //private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";

        // Google Play Store-specific product identifier subscription product.
        //private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";
        public int curPid = -1;
        public int curChargeData = 0;
        public string[] ProductIds;
        //string NoadsIOS = "noads";
        void Start()
        {
            // If we haven't set up the Unity Purchasing reference
            //if (m_StoreController == null)
            //{
            //    // Begin to configure our connection to Purchasing
            //    InitializePurchasing();
            //}
        }

        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            //var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            //// Add a product to sell / restore by way of its identifier, associating the general identifier
            //// with its store-specific identifiers.
            //builder.AddProduct(kProductIDConsumable, ProductType.Consumable);
            //// Continue adding the non-consumable product.
            //builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);
            //// And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
            //// if the Product ID was configured differently between Apple and Google stores. Also note that
            //// one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
            //// must only be referenced here. 
            //builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
            //    { kProductNameAppleSubscription, AppleAppStore.Name },
            //    { kProductNameGooglePlaySubscription, GooglePlay.Name },
            //});

//            for (int i = 0; i < ProductIds.Length; ++i)
//            {
//#if UNITY_ANDROID
//                builder.AddProduct(ProductIds[i], ProductType.Consumable);
//#elif UNITY_IOS
//                if (i == 12)
//                    builder.AddProduct(NoadsIOS, ProductType.NonConsumable);
//                else
//                    builder.AddProduct(ProductIds[i], ProductType.Consumable);
//#endif
//            }

//            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
//            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
//            UnityPurchasing.Initialize(this, builder);
        }


        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return false;// m_StoreController != null && m_StoreExtensionProvider != null;
        }


        public void BuyConsumable()
        {
            // Buy the consumable product using its general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            //BuyProductID(kProductIDConsumable);\
#if UNITY_ANDROID
                BuyProductID(ProductIds[curPid]);
#elif UNITY_IOS
            if (curPid == 12)
                BuyProductID(NoadsIOS);
            else
                BuyProductID(ProductIds[curPid]);
#endif
        }


        public void BuyNonConsumable()
        {
            // Buy the non-consumable product using its general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            BuyProductID(kProductIDNonConsumable);
        }


        public void BuySubscription()
        {
            // Buy the subscription product using its the general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            // Notice how we use the general product identifier in spite of this ID being mapped to
            // custom store-specific identifiers above.
            BuyProductID(kProductIDSubscription);
        }


        void BuyProductID(string productId)
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                //Product product = m_StoreController.products.WithID(productId);
                //// If the look up found a product for this device's store and that product is ready to be sold ... 
                //if (product != null && product.availableToPurchase)
                //{
                //    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                //    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                //    // asynchronously.
                //    m_StoreController.InitiatePurchase(product);
                //}
                //// Otherwise ...
                //else
                //{
                //    // ... report the product look-up failure situation  
                //    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                //}
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }

        public string GetProductPrice(int pId)
        {
            return "";//m_StoreController.products.WithID(ProductIds[pId]).metadata.localizedPriceString;
        }

        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                //var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                //// Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                //// the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                //apple.RestoreTransactions((result) => {
                //    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                //    // no purchases are available to be restored.
                //    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                //});
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }


        //  
        // --- IStoreListener
        //

//        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
//        {
//            // Purchasing has succeeded initializing. Collect our Purchasing references.
//            Debug.Log("OnInitialized: PASS");

//            // Overall Purchasing system, configured with products for this application.
//            m_StoreController = controller;
//            // Store specific subsystem, for accessing device-specific store features.
//            m_StoreExtensionProvider = extensions;
//        }


//        public void OnInitializeFailed(InitializationFailureReason error)
//        {
//            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
//            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
//        }


//        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
//        {
//            bool validPurchase = true; // Presume valid for platforms with no R.V.

//            // Unity IAP's validation logic is only included on these platforms.
//#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
//            // Prepare the validator with the secrets we prepared in the Editor
//            // obfuscation window.
//            //var validator = new CrossPlatformValidator(null, null, Application.identifier);
//            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

//            try
//            {
//                // On Google Play, result will have a single product Id.
//                // On Apple stores receipts contain multiple products.
//                var result = validator.Validate(args.purchasedProduct.receipt);
//                // For informational purposes, we list the receipt(s)
//                Debug.Log("Receipt is valid. Contents:");
//                foreach (IPurchaseReceipt productReceipt in result)
//                {
//                    CheckPurchaser(args);
//                    Debug.Log(productReceipt.productID);
//                    Debug.Log(productReceipt.purchaseDate);
//                    Debug.Log(productReceipt.transactionID);
//                }
//            }
//            catch (IAPSecurityException)
//            {
//                Debug.Log("Invalid receipt, not unlocking content");
//                validPurchase = false;
//            }
//#endif
//            if(Application.isEditor)
//            {
//                CheckPurchaser(args);
//            }

//            if (validPurchase)
//            {
//                // Unlock the appropriate content here.
//            }

//            return PurchaseProcessingResult.Complete;
//        }

//        void CheckPurchaser(PurchaseEventArgs args)
//        {
//            string pName = ProductIds[curPid];
//#if UNITY_IOS
//            if (curPid == 12)
//                pName = NoadsIOS;
//#endif
//            if (string.Equals(args.purchasedProduct.definition.id, pName))
//            {
//                if (curPid < 6)
//                    setJewelData();
//                else if (6 == curPid)
//                    setSweetAim();
//                else if (12 == curPid)
//                    setNoads();
//                else
//                    setPackageData();

//                //DataManager.instance.isBuyInAppItem = true;
//                curPid = -1;
//            }
//            // Or ... an unknown product has been purchased by this user. Fill in additional products here....
//            else
//            {
//                Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
//            }
//        }
        public void setSweetAim()
        {
            DataManager.instance.SweetAimUse = true;
            LobbyManager.instance.UseSweetAim(true);
            //if (!DataManager.instance.isFirstPayment())
            if (!Application.isEditor)
            {
                DataManager.instance.CheckFirstPayment();
                DataManager.instance.CallGameDataSave();
            }
        }

        public void setNoads()
        {
            DataManager.instance.NoAdsUse = true;
            LobbyManager.instance.UseNoads();
            //if (!DataManager.instance.isFirstPayment())
            if(!Application.isEditor)
            {
                DataManager.instance.CheckFirstPayment();
                DataManager.instance.CallGameDataSave();
            }   
        }
        public void setJewelData()
        {
            //SoundManager.instance.ChangeEffects(9);
            //UIManager.instance.CallCommonPup(CommonPupType.buyjewel_2, curChargeData);
            //int preJewel, nextJewel = 0;
            //preJewel = DataManager.instance.MyJewel;
            //DataManager.instance.MyJewel = curChargeData;
            //nextJewel = DataManager.instance.MyJewel;
            //UIManager.instance.setJewelData(true, preJewel, nextJewel);
            
            SoundManager.instance.ChangeEffects(8);
            LobbyManager.instance.ChargeAndUsedCoin(curChargeData);
            curChargeData = 0;
            //if (!DataManager.instance.isFirstPayment())
                DataManager.instance.CheckFirstPayment();
            if (DataManager.instance.AutoSave)
                DataManager.instance.CallGameDataSave();
        }
        public void setPackageData()
        {
            //SoundManager.instance.ChangeEffects(11);
            //UIManager.instance.CallCommonPup(CommonPupType.charge_2);
            //DataManager.instance.setCurPackageItemBuy(true);
            LobbyManager.instance.BuySetPackageItemData(curChargeData);
            //if (!DataManager.instance.isFirstPayment())
                DataManager.instance.CheckFirstPayment();
            if (DataManager.instance.AutoSave)
                DataManager.instance.CallGameDataSave();
        }

        //public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        //{
        //    // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        //    // this reason with the user to guide their troubleshooting actions.
        //    Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        //}
    }
}