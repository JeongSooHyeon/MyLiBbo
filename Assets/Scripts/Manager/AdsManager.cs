using UnityEngine;
using System.Collections;
//using GoogleMobileAds.Api;
//using AudienceNetwork;
using System;

public class AdsManager : MonoBehaviour
{
    public const string FacebookURL = "https://www.facebook.com/iloveBBosi";
    public const string InstagramURL = "https://instagram.com/bbosiraegi_official";
    public static AdsManager instance;

    public adsType myAdsType;
    public float myFeverNum = 0;

#if UNITY_ANDROID

    ////테스트 코드
    //public const string AdmobBannerID = "ca-app-pub-3940256099942544/6300978111";
    //public const string AdmobinterstitailID = "ca-app-pub-3940256099942544/1033173712";
    //public const string AdmobNativeID = "ca-app-pub-3940256099942544/2247696110";

    //public const string AdmobRewardMainID = "ca-app-pub-3940256099942544/5224354917";
    //public const string AdmobRewardStageID = "ca-app-pub-3940256099942544/5224354917";
    //public const string AdmobRewardAddBallID = "ca-app-pub-3940256099942544/5224354917";
    //public const string AdmobRewardRelayID = "ca-app-pub-3940256099942544/5224354917";
    //public const string AdmobRewardPreviewID = "ca-app-pub-3940256099942544/5224354917";
    //public const string AdmobRewardDailyID = "ca-app-pub-3940256099942544/5224354917";

    //실 코드
    public const string AdmobBannerID = "ca-app-pub-4481686654489037/1727068927";
    public const string AdmobBanner2ID = "ca-app-pub-4481686654489037/8504763092";
    public const string AdmobinterstitailID = "ca-app-pub-4481686654489037/9222415561";
    public const string AdmobRewardMainID = "ca-app-pub-4481686654489037/2848578906";

    public const string FacebookAppID = "1081611435674821";
    public const string FacebookBannerID = "1081611435674821_1082865208882777";
    public const string FacebookBanner2ID = "1081611435674821_1082864855549479";
    public const string FacebookinterstitailID = "1081611435674821_1082864995549465";
    public const string FacebookRewardMainID = "1081611435674821_1081613762341255";

#elif UNITY_IOS
    public const string AdmobBannerID = "ca-app-pub-4481686654489037/1935343094";
    public const string AdmobinterstitailID = "ca-app-pub-4481686654489037/4042631680";
    public const string AdmobBanner2ID = "ca-app-pub-4481686654489037/8309179754";
    public const string AdmobRewardMainID = "ca-app-pub-4481686654489037/2729550018";
    
    public const string FacebookAppID = "1081611435674821";
    public const string FacebookBannerID = "1081611435674821_1096797574156207";
    public const string FacebookinterstitailID = "1081611435674821_1096797877489510";
    public const string FacebookBanner2ID = "1081611435674821_1096797697489528";
    public const string FacebookRewardMainID = "1081611435674821_1096797777489520";

#endif

    public const int GetCoin = 30;
    public int GetRewardCoin = 50;
    public int GetRewardItem = 2;
    ////임시로 만듬.
    //[SerializeField] TweenPosition myTp;
    //[SerializeField] TweenScale myTs;
    public int AdsRewardData;

    //private BannerView bannerView;
    //private BannerView nativeAd;
    //private GoogleMobileAds.Api.InterstitialAd interstitial;

    //private RewardedAd rewardBasedVideo_M;

    //private RewardedVideoAd rewardedVideoAd_M;

    //private AdView adView;
    //private AudienceNetwork.InterstitialAd interstitialAd;
    private bool isLoaded;
    private bool isInterLoaded;
    private bool didClose;

    public bool isBannerLoad;

    public bool isAdsPlay;

    public void LoadInterstitial()
    {
        //this.interstitialAd = new AudienceNetwork.InterstitialAd(FacebookinterstitailID);
        //this.interstitialAd.Register(this.gameObject);
        //// Set delegates to get notified on changes or when the user interacts with the ad. 
        //this.interstitialAd.InterstitialAdDidLoad = (delegate ()
        //{
        //    Debug.Log("Interstitial ad loaded.");
        //    this.isInterLoaded = true;
        //});
        //interstitialAd.InterstitialAdDidFailWithError = (delegate (string error) {
        //    this.isInterLoaded = false;
        //    Debug.Log("Interstitial ad failed to load with error: " + error); });
        //interstitialAd.InterstitialAdWillLogImpression = (delegate () { Debug.Log("Interstitial ad logged impression."); });
        //interstitialAd.InterstitialAdDidClick = (delegate () { Debug.Log("Interstitial ad clicked."); });
        //this.interstitialAd.interstitialAdDidClose = (delegate ()
        //{
        //    this.isInterLoaded = false;
        //    Debug.Log("Interstitial ad did close.");
        //    if (this.interstitialAd != null) { this.interstitialAd.Dispose(); }
        //});
    }

    public void LoadBanner()
    {
        //if (this.adView)
        //{
        //    this.adView.Dispose();
        //}
        //this.adView = new AdView(FacebookBannerID, AudienceNetwork.AdSize.BANNER_HEIGHT_50);
        //this.adView.Register(this.gameObject);
        //// 델리게이트를 설정하여 변경 사항이 있거나 사용자가 광고와 상호작용할 때 알림을 받습니다. 
        //this.adView.AdViewDidLoad = (delegate () { Debug.Log("Banner loaded."); });
        //adView.AdViewDidFailWithError = (delegate (string error) { Debug.Log("Banner failed to load with error: " + error); });
        //adView.AdViewWillLogImpression = (delegate () { Debug.Log("Banner logged impression."); });
        //adView.AdViewDidClick = (delegate () { Debug.Log("Banner clicked."); });
        //// 요청을 시작하여 광고를 로드합니다. 
        //adView.LoadAd();
    }

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            if (!Application.isEditor)
            {
                if (Application.internetReachability != NetworkReachability.NotReachable)
                    initAds();
            }   
        }
        
    }

    void initAds()
    {
        //MobileAds.Initialize(initStatus => { });

        //RequestBanner();
        //RequestBanner2();
        ////RequestInterstitial();
        //RequestRewardBasedVideo(RewardAdsType.getCoinLobby);

        //AudienceNetworkAds.Initialize();

        //LoadRewardedVideo(RewardAdsType.getCoinLobby);
        //LoadBanner();
        //LoadInterstitial();
    }

    private void RequestBanner()
    {
        //if (bannerView != null)
        //    bannerView.Destroy();

        //bannerView = new BannerView(AdmobBannerID, GoogleMobileAds.Api.AdSize.Banner, GoogleMobileAds.Api.AdPosition.Top);
        //// Called when an ad request has successfully loaded.
        //bannerView.OnAdLoaded += HandleOnAdLoadedBanner;
        //// Called when an ad request failed to load.
        //bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoadBanner;
        //AdRequest request = new AdRequest.Builder().Build();
        //bannerView.LoadAd(request);
        //bannerView.Hide();
    }

    private void RequestBanner2()
    {
        //if (nativeAd != null)
        //    nativeAd.Destroy();

        //int width = Screen.width / 2;
        //nativeAd = new BannerView(AdmobBanner2ID, GoogleMobileAds.Api.AdSize.MediumRectangle, GoogleMobileAds.Api.AdPosition.Top);
        //AdRequest request = new AdRequest.Builder().Build();
        //nativeAd.LoadAd(request);
        //nativeAd.Hide();
    }

    public void RequestinterstitialAd()
    {
        RequestInterstitial();
    }

    private void RequestInterstitial()
    {
        //if (interstitial != null)
        //    interstitial.Destroy();

        //interstitial = new GoogleMobileAds.Api.InterstitialAd(AdmobinterstitailID);
        //// Create an empty ad request.
        //AdRequest request = new AdRequest.Builder().Build();
        //// Load the interstitial with the request.
        //// Called when an ad request has successfully loaded.
        //interstitial.OnAdLoaded += HandleOnAdLoaded;
        //// Called when an ad request failed to load.
        //interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        //// Called when an ad is shown.
        //interstitial.OnAdOpening += HandleOnAdOpened;
        //// Called when the ad is closed.
        //interstitial.OnAdClosed += HandleOnAdClosed;
        //// Called when the ad click caused the user to leave the application.
        //interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;
        //this.interstitial.LoadAd(request);
    }

    private void RequestRewardBasedVideo(RewardAdsType myType)
    {
        // Create an empty ad request.
        //rewardBasedVideo_M = CreateAndLoadRewardedAd(AdmobRewardMainID);
    }

    //public RewardedAd CreateAndLoadRewardedAd(string adUnitId)
    //{
    //    RewardedAd rewardedAd = new RewardedAd(adUnitId);

    //    rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
    //    rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
    //    rewardedAd.OnAdClosed += HandleRewardedAdClosed;

    //    // Create an empty ad request.
    //    AdRequest request = new AdRequest.Builder().Build();
    //    // Load the rewarded ad with the request.
    //    rewardedAd.LoadAd(request);
    //    return rewardedAd;
    //}

    private void LoadRewardedVideo(RewardAdsType myType)
    {
        //rewardedVideoAd_M = new RewardedVideoAd(FacebookRewardMainID);
        //SetRewardVideoSetting(rewardedVideoAd_M);
    }

    //void SetRewardVideoSetting(RewardedVideoAd rewardedVideoAd)
    //{
//        rewardedVideoAd.Register(gameObject);

//        rewardedVideoAd.RewardedVideoAdDidLoad = delegate ()
//        {
//            Debug.Log("RewardedVideo ad loaded.");
//            isLoaded = true;
//            didClose = false;
//        };
//        rewardedVideoAd.RewardedVideoAdDidFailWithError = delegate (string error)
//        {
//            Debug.Log("RewardedVideo ad failed to load with error: " + error);
//        };
//        rewardedVideoAd.RewardedVideoAdWillLogImpression = delegate ()
//        {
//            Debug.Log("RewardedVideo ad logged impression.");
//        };
//        rewardedVideoAd.RewardedVideoAdDidClick = delegate ()
//        {
//            Debug.Log("RewardedVideo ad clicked.");
//        };

//        rewardedVideoAd.RewardedVideoAdDidSucceed = delegate ()
//        {
//            Debug.Log("Rewarded video ad validated by server");
//        };

//        rewardedVideoAd.RewardedVideoAdDidFail = delegate ()
//        {
//            Debug.Log("Rewarded video ad not validated, or no response from server");
//        };

//        rewardedVideoAd.RewardedVideoAdDidClose = delegate ()
//        {
//            Debug.Log("Rewarded video ad did close.");
//            didClose = true;
//            if (rewardedVideoAd != null)
//            {
//                rewardedVideoAd.Dispose();
//            }
//        };
//        rewardedVideoAd.RewardedVideoAdComplete = delegate ()
//            {
//                Debug.Log("Rewarded video ad Complete.");
//                StartCoroutine(CallBackRewardVideo());
//            };
//#if UNITY_ANDROID
//        /*
//         * Only relevant to Android.
//         * This callback will only be triggered if the Rewarded Video activity
//         * has been destroyed without being properly closed. This can happen if
//         * an app with launchMode:singleTask (such as a Unity game) goes to
//         * background and is then relaunched by tapping the icon.
//         */
//        rewardedVideoAd.RewardedVideoAdActivityDestroyed = delegate ()
//        {
//            if (!didClose)
//            {
//                Debug.Log("Rewarded video activity destroyed without being closed first.");
//                Debug.Log("Game should resume. User should not get a reward.");
//            }
//        };
//#endif
//        // Initiate the request to load the ad.
//        rewardedVideoAd.LoadAd();
    //}

    public void BannerEnable(bool isFlag = true)
    {
        //if (Application.internetReachability == NetworkReachability.NotReachable)
        //    return;
        //if (DataManager.instance.NoAdsUse) return;

        //if (!Application.isEditor)
        //{
        //    if(adView != null)
        //    {
        //        if (adView.IsValid())
        //        {
        //            if (isFlag)
        //                adView.Show(AudienceNetwork.AdPosition.TOP);
        //            bannerView.Hide();
        //        }
        //        else
        //        {
        //            if (isFlag)
        //                bannerView.Show();
        //            else
        //                bannerView.Hide();
        //        }

        //    }
        //    else
        //    {
        //        if (isFlag)
        //            bannerView.Show();
        //        else
        //            bannerView.Hide();
        //    }
        //}
    }

    public void NativeEnable(bool isFlag = true)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return;

        if (!Application.isEditor)
        {
            //if (isFlag)
            //    nativeAd.Show();
            //else
            //    nativeAd.Hide();
        }
    }

    public void ShowInterstitialAd()
    {
        //if (isInterLoaded)
        //{
        //    interstitialAd.LoadAd();
        //}
        //else if (interstitial.IsLoaded())
        //{
        //    interstitial.Show();
        //    LoadInterstitial();
        //}
        //else
        //{
        //    if(Application.isEditor)
        //    {
        //        if (DataManager.instance.GetSceneType() == SceneType.Lobby)
        //            LobbyManager.instance.AdsPupCall();
        //        else UIManager.instance.AdsPupCall();
        //    }
        //    isAdsPlay = true;
        //    RequestInterstitial();
        //}
    }

    public void ShowRewardedAd(adsType idx)
    {
        myAdsType = idx;
        ShowUnityAds();
    }

    public bool isRewardLoad(adsType myType)
    {
        if (Application.isEditor)
            return true;
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return false;
        //if (rewardedVideoAd_M != null)
        //{
        //    if (rewardedVideoAd_M.IsValid()) return true;
        //}

        //if (rewardBasedVideo_M.IsLoaded())
        //    return rewardBasedVideo_M.IsLoaded();

        return false;
    }

    public bool isInterstitialLoad()
    {
        //if(interstitial.IsLoaded())
        //    return true;

        return false;
    }

    public void ShowUnityAds()
    {
        PlayerPrefsElite.SetInt("myAdsType_", (int)myAdsType);
        if (Application.isEditor)
        {
            if (DataManager.instance.GetSceneType() == SceneType.Lobby)
                LobbyManager.instance.AdsPupCall();
            else UIManager.instance.AdsPupCall();
        }
        else
        {
            isAdsPlay = true;
            //if(rewardedVideoAd_M != null)
            //{
            //    if (rewardedVideoAd_M.IsValid())
            //    {
            //        rewardedVideoAd_M.Show();
            //        isLoaded = false;
            //    }
            //    else if (rewardBasedVideo_M.IsLoaded())
            //    {
            //        rewardBasedVideo_M.Show();
            //        LoadRewardedVideo(RewardAdsType.getCoinLobby);
            //    }
            //    else
            //    {
            //        RequestRewardBasedVideo(RewardAdsType.getCoinLobby);
            //    }
            //}
            //else if (rewardBasedVideo_M.IsLoaded())
            //{
            //    rewardBasedVideo_M.Show();
            //    LoadRewardedVideo(RewardAdsType.getCoinLobby);
            //}
            //else
            //{
            //    RequestRewardBasedVideo(RewardAdsType.getCoinLobby);
            //}
        }
    }

    void checkMyUseType()
    {
        myAdsType = (adsType)PlayerPrefsElite.GetInt("myAdsType_");
        switch (myAdsType)
        {
            case adsType.getCoin:
                if (DataManager.instance.GetSceneType() == SceneType.Lobby)
                {
                    DataManager.instance.LobbyGetCoinAds = UnbiasedTime.Instance.Now().ToString();
                    LobbyManager.instance.ChargeAndUsedCoin(GetCoin);
                    //LobbyManager.instance.GetCoinAdsBtnDisable(false);
                    if (DataManager.instance.AutoSave)
                        DataManager.instance.CallGameDataSave();
                }
                else
                {
                    DataManager.instance.LobbyGetCoinAds = UnbiasedTime.Instance.Now().ToString();
                    DataManager.instance.SetCoin(GetCoin);
                    UIManager.instance.SetMoney();
                    if (DataManager.instance.AutoSave)
                        DataManager.instance.CallGameDataSave();
                    DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.game_result, 17);
                }
                //LobbyController.instance.SetLabel(EnumBase.UIState.Coin, string.Format("{0:N0}", DataManager.instance.GetCoin()));
                //ChargeManager.intance.CoinEffect();
                break;
            case adsType.oneMore:
                //UIManager.instance.Continue(false);
                BrickManager.instance.OnemoreSet();
                break;
            case adsType.rewardCoin:
                UIManager.instance.ChargeAndUsedCoin(GetRewardCoin);
                DataManager.instance.SetIngameRewardState(1, IngameRewardState.Rewarded);
                break;
            case adsType.rewardItem:
                DataManager.instance.SetInstanceItemList(InstanceItem.AllBlockDamage, GetRewardItem);
                DataManager.instance.SetIngameRewardState(2, IngameRewardState.Rewarded);
                break;
            case adsType.plusBall:
                GameObject.Find("ADBall").GetComponent<BottomItemBtn>().plusBallAd();
                BallManager.instance.startboss = true;
                break;
            case adsType.continueAD:
                BrickManager.instance.deleteBrickToContinue();
                UIManager.instance.ContinueToday();
                break;
            case adsType.dailyReward:
                if (DataManager.instance.GetSceneType() == SceneType.Lobby)
                {
                    LobbyManager.instance.CloseDailyRewardPup();
                    LobbyManager.instance.CallDRCommonPup();
                }   
                break;
            /*case adsType.preMonster:
                //DataManager.instance.SetMonsterList(DataManager.instance.SelectMoster, (int)ShopState.PreUse);
                DataManager.instance.SetMonsterPreviewList(AdsRewardData, 1);
                DataManager.instance.ClearPreviewMonsterList();
                LobbyManager.instance.SetBallSelect(AdsRewardData, *//*false, *//*true);
                LobbyManager.instance.BallNMonPreViewSetData(false);
                LobbyManager.instance.CheckNewSign();
                AdsRewardData = 0;
                break;
            case adsType.preBall:
                //DataManager.instance.SetBallList(DataManager.instance.BallSprite, (int)ShopState.PreUse);
                DataManager.instance.SetBallPreViewList(AdsRewardData, 1);
                DataManager.instance.ClearPreviewBallList();
                LobbyManager.instance.SetBallSelect(AdsRewardData,*//* true,*//* true);
                LobbyManager.instance.BallNMonPreViewSetData(true);
                LobbyManager.instance.CheckNewSign();
                AdsRewardData = 0;
                break;*/
            case adsType.preCharBall:
                DataManager.instance.SetCharBallPreviewList(AdsRewardData, 1);
                DataManager.instance.ClearPreviewBallList();
                LobbyManager.instance.SetBallSelect(AdsRewardData, true);
                LobbyManager.instance.CharBallPreViewSetData();
                AdsRewardData = 0;
                break;
            case adsType.SweetAim:
                DataManager.instance.SweetAimPreUse = true;
                LobbyManager.instance.UseSweetAim(true);
                break;
        }
        isAdsPlay = false;
    }

    public void CloseAdsPup()
    {
        checkMyUseType();
        if (!Application.isEditor)
            RequestRewardVideo();
        if (DataManager.instance.GetSceneType() == SceneType.Lobby)
            LobbyManager.instance.AdsPupClose();
        else UIManager.instance.AdsPupClose();
    }

    void RequestRewardVideo()
    {
        switch (myAdsType)
        {
            case adsType.getCoin:
                if (DataManager.instance.GetSceneType() == SceneType.Lobby)
                {
                    RequestRewardBasedVideo(RewardAdsType.getCoinLobby);
                }
                else
                {
                    RequestRewardBasedVideo(RewardAdsType.getCoinIngame);
                }
                break;
            case adsType.oneMore:
                RequestRewardBasedVideo(RewardAdsType.relay);
                break;
            case adsType.rewardCoin:
                RequestRewardBasedVideo(RewardAdsType.getCoinIngame);
                break;
            case adsType.rewardItem:
                RequestRewardBasedVideo(RewardAdsType.getCoinIngame);
                break;
            case adsType.plusBall:
                RequestRewardBasedVideo(RewardAdsType.plusBall);
                break;
            case adsType.continueAD:
                RequestRewardBasedVideo(RewardAdsType.relay);
                break;
            case adsType.dailyReward:
                RequestRewardBasedVideo(RewardAdsType.daily);
                break;
            case adsType.preCharBall:
                RequestRewardBasedVideo(RewardAdsType.preCharBall);
                break;
            case adsType.SweetAim:
                RequestRewardBasedVideo(RewardAdsType.getCoinLobby);
                break;
        }
        myAdsType = adsType.none;
        PlayerPrefsElite.SetInt("myAdsType_", (int)myAdsType);
    }
    //#region 광고 콜백 함수

    ////public bool unifiedNativeAdLoaded;

    ////private void HandleUnifiedNativeAdLoaded(object sender, UnifiedNativeAdEventArgs args)
    ////{
    ////    MonoBehaviour.print("Unified Native ad loaded.");
    ////    this.nativeAd = args.nativeAd;
    ////    unifiedNativeAdLoaded = true;
    ////}

    //public void HandleOnAdLoadedBanner(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleAdLoaded event received");
    //    isBannerLoad = true;
    //}

    //public void HandleOnAdFailedToLoadBanner(object sender, AdFailedToLoadEventArgs args)
    //{
    //    MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
    //                        + args.Message);
    //    //isBannerLoad = false;
    //}

    //public void HandleOnAdLoaded(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleAdLoaded event received");
    //}

    //public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    //{
    //    MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
    //                        + args.Message);
    //}

    //public void HandleOnAdOpened(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleAdOpened event received");
    //    //RequestInterstitial();
    //}

    //public void HandleOnAdClosed(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleAdClosed event received");
    //    interstitial.Destroy();
    //    UIManager.instance.ClickBtnResult();
    //    RequestinterstitialAd();
    //}

    //public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleAdLeavingApplication event received");
    //}
    //#endregion
    //#region 동영상광고 콜백함수   
    //public void HandleRewardedAdLoaded(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleRewardedAdLoaded event received");
    //}

    //public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    //{
    //    MonoBehaviour.print(
    //        "HandleRewardedAdFailedToLoad event received with message: "
    //                         + args.Message);
    //}

    //public void HandleRewardedAdOpening(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleRewardedAdOpening event received");
    //}

    //public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    //{
    //    MonoBehaviour.print(
    //        "HandleRewardedAdFailedToShow event received with message: "
    //                         + args.Message);
    //}

    //public void HandleRewardedAdClosed(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleRewardedAdClosed event received");
    //}

    //public void HandleUserEarnedReward(object sender, Reward args)
    //{
    //    string type = args.Type;
    //    double amount = args.Amount;
    //    MonoBehaviour.print(
    //        "HandleRewardedAdRewarded event received for "
    //                    + amount.ToString() + " " + type);
    //    StartCoroutine(CallBackRewardVideo());
    //}
    //#endregion

    IEnumerator CallBackRewardVideo()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        checkMyUseType();
        yield return new WaitForSecondsRealtime(0.5f);
        //if (!isRewardLoad(myAdsType))
            RequestRewardVideo();
        //StopCoroutine(CallBackRewardVideo());
        //yield return null;
    }
}
