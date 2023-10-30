using I2.Loc;
using System.Linq;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance = null;
    public ToggleMenuChecker check;
    [SerializeField] UILabel myCoin;
    [SerializeField] UILabel Ball100BestScore;
    [SerializeField] UILabel ClassicBestScore;
    [SerializeField] UILabel ShootingBestScore;
    [SerializeField] PopupBase[] popupList;
    [SerializeField] UILabel TotalStarCountTxt;
    [SerializeField] ScrollListBtn ShopBtn;
    [SerializeField] TweenPosition AdsPupTP;
    [SerializeField] TweenScale AdsPupTS;
    [SerializeField] LoadingPupControl loading;
    [SerializeField] ShopControl myShop;
    [SerializeField] BallViewControl[] myBallView;
    [SerializeField] CommonPupControl myCommonPup;
    [SerializeField] AdsMessagePupControl myAdsMessPup;
    [SerializeField] DailyRewardNoticePupControl myDRCommonPup;
    [SerializeField] GameObject fakeBannerObj;
    [SerializeField] OptionPopupControl optionPop;
    [SerializeField] GameObject[] NewSignObj;
    [SerializeField] UILabel[] curScoreLabels;
    [SerializeField] GameObject[] StarRewardBtnObjs;
    [SerializeField] UILabel StarRewardCountTxt;
    [SerializeField] PuzzlePupControl housingPup;
    [SerializeField] PuzzleSetItemControl housingList;

    [SerializeField] UILabel[] startButtonLabel;

    public PopupBase curPopup;
    public bool isFirstLogin;
    public LobbyState state;
    [SerializeField] GameObject stageSetBtn;
    [SerializeField] HouInfoControl houInfo;
    [SerializeField] LobbyTipsPupControl lobbyTuto;
    public bool fakeAd;
    public bool isLoad;

    public UILabel testLocLabel;
    public GameObject testLocButton;

    void Awake()
    {
        NoData();
        if (instance == null)
            instance = this;
        Time.timeScale = 1;

#if UNITY_IOS
        // for(int i=0; i< myTopAnchor.Length; ++i)
        //{
        //    if (Device.generation >= DeviceGeneration.iPhoneX)
        //        myTopAnchor[i].relativeOffset = myTopPosArr[1];
        //    else
        //        myTopAnchor[i].relativeOffset = myTopPosArr[0];
        //}
        Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        //MoreBtnObj[0].transform.localPosition = MoreBtnPos[0];
        //MoreBtnObj[1].transform.localPosition = MoreBtnPos[1];
        //MoreBtnObj[2].SetActive(false);
#endif
    }

    void Start()
    {
        LobbyManager.instance.DailyStateCheck();
        Application.targetFrameRate = 60;
        SoundManager.instance.ChangeBGM(0);

        initLobby();
        if (DataManager.instance.LobbyTipView)
        {
            popupList[5].gameObject.SetActive(false);
            CallLoadingPup();
            DailyMissionCall();
        }
        state = LobbyState.Lobby;

        stageSetBtn.SetActive(DataManager.instance.isTestMode);

        if (!DataManager.instance.LobbyTutorial) CallLobbyTuto();

        fakeAd = false;
        isLoad = false;

        testLocButton.SetActive(DataManager.instance.isTestMode);
        testLocLabel.text = string.Format("{0}", DataManager.instance.languageState);
    }

    public void DailyMissionCall(float CallNum = 1f)
    {
        if (DataManager.instance.GetDailyRewardTime() && !DataManager.instance.isFirstLoginLobby)
        {
            DataManager.instance.isFirstLoginLobby = true;
            DailyStateCheck();
            Invoke("ClickDailyRewardBtn", CallNum);
        }
    }

    void initLobby()
    {
        if (!isFirstLogin)
        {
            isFirstLogin = true;

        }
        SetMyCoin();
        SetTotalStarCount();
        Set100BallBestScore();
        SetShootingBestScore();
        if (!Application.isEditor)
        {
            AdsManager.instance.BannerEnable();
            AdsManager.instance.NativeEnable(false);
            fakeBannerObj.SetActive(!AdsManager.instance.isBannerLoad);
        }
        curScoreLabels[0].text = string.Format("{0:N0}", DataManager.instance.GetBestScore(2));//.ToString("#,###");
        curScoreLabels[1].text = string.Format("{0:N0}", DataManager.instance.GetBestScore(3));//.ToString("#,###");
        SetStartBtnTxt();
        curScoreLabels[3].text = string.Format("{0:N0}", DataManager.instance.GetBestScore(4));
        CheckStarRewardState(DataManager.instance.GetLobbyRewardCount() <= DataManager.instance.TotalStarCount);
        MapStateCheck();
        DailyStateCheck();
    }
    public void CheckStarRewardState(bool isState)
    {
        StarRewardBtnObjs[0].SetActive(!isState);
        StarRewardBtnObjs[1].SetActive(isState);
        string data = DataManager.instance.GetLobbyRewardCount().ToString();
        StarRewardCountTxt.text = LocalizationManager.GetTermTranslation("Lobby_Gift_Misson").Replace("$$", data);
    }

    public void ClickStarCollectReward()
    {
        //별 보상 받기
        CallCommonPup((int)CommonState.GetGift);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isLoad) return;

            if (!fakeAd)
            {
                if (curPopup == null)
                    popupList[1].CallPupTPTS();
                else if (curPopup.myTypeEnum != PopupType.lobbyTip)
                    curPopup.ClosePupTPTS();
            }
            else AdsPupCloseBtnClick();
        }
        fakeBannerObj.SetActive(!AdsManager.instance.isBannerLoad);
    }

    public void SetHousingNewSign(bool enable)
    {
        NewSignObj[0].SetActive(enable);
    }

    public void SetMapNewSign(bool enable)
    {
        NewSignObj[1].SetActive(enable);
    }
    public void SetMenuNewSign(bool enable)
    {
        NewSignObj[2].SetActive(enable);
    }
    public void SetDailyNewSign(bool enable)
    {
        NewSignObj[3].SetActive(enable);
    }

    public void MapStateCheck()
    {
        bool enable = DataManager.instance.stageChapterStateList.Count(t => t == 1) > 0 ? true : false;
        SetMapNewSign(enable);
    }

    public void DailyStateCheck()
    {
        bool enable = DataManager.instance.GetDailyRewardTime();
        SetDailyNewSign(enable);
        SetMenuNewSign(enable);
    }
    public void CallScrollView()
    {
        check.CheckDrag();
    }

    void SetMyCoin()
    {
        if (DataManager.instance.GetCoin() > 99999)
            myCoin.text = "99,999+";
        else myCoin.text = string.Format("{0:N0}", DataManager.instance.GetCoin());
    }

    void Set100BallBestScore()
    {
        Ball100BestScore.text = string.Format("{0:N0}", DataManager.instance.GetBestScore(2));
    }

    void SetClassicBestScore()
    {
        ClassicBestScore.text = string.Format("{0:N0}", DataManager.instance.GetBestScore(3));
    }

    void SetShootingBestScore()
    {
        ShootingBestScore.text = string.Format("{0:N0}", DataManager.instance.GetBestScore(4));
    }

    public void ClickOptionBtn()
    {
        popupList[(int)PopupType.option - 1].CallPupTPTS();
    }

    public void SetTotalStarCount()
    {
        if (DataManager.instance.TotalStarCount > 99999)
            TotalStarCountTxt.text = "99,999+";
        else TotalStarCountTxt.text = string.Format("{0:N0}", DataManager.instance.TotalStarCount);
    }

    //public void CallMoreGame()
    //{
    //    popupList[8].CallPupTPTS();
    //}

    public void ChargeAndUsedCoin(int coin)
    {
        DataManager.instance.SetCoin(coin);
        SetMyCoin();
    }

    public void CallShopView()
    {
        ShopBtn.gameObject.GetComponent<UIToggle>().value = true;
        myShop.CallJewelChargeView();
        ShopBtn.SelectScrollBtn();
    }

    public void CallCommonPup(int sIdx, int cData = 0, int sData = 0)
    {
        myCommonPup.curState = (CommonState)sIdx;
        if (cData > 0)
            myCommonPup.SetData = cData;
        if (sData > 0)
            myCommonPup.SubData = sData;
        myCommonPup.CallPupTPTS();
    }

    // 데일리 보상 전용 팝업
    public void CallDRCommonPup()
    {
        myDRCommonPup.CallPupTPTS();
        DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.daily_bonus, 2);
    }

    public void SetBallSelect(int myidx, bool isPre = false, bool isBuy = false)
    {
        myBallView[0].ResetBallSelectBtns(myidx, isPre, isBuy);
    }

    public void CharBallPreViewSetData()
    {
        myBallView[0].SetCharBallPreData();
    }

    public void ClickWatchAdsBtn()
    {
        SoundManager.instance.TapSound();
        if (AdsManager.instance.isRewardLoad(adsType.getCoin))
            AdsManager.instance.ShowRewardedAd(adsType.getCoin);
        else
            CallAdsMesPup();
    }

    public void WatchGetCoinAds()
    {
        if (AdsManager.instance.isRewardLoad(adsType.getCoin))
            AdsManager.instance.ShowRewardedAd(adsType.getCoin);
        else
            CallAdsMesPup();
    }

    public void WatchPreMonsterAds(int SetData)
    {
        if (AdsManager.instance.isRewardLoad(adsType.preMonster))
        {
            AdsManager.instance.AdsRewardData = SetData;
            AdsManager.instance.ShowRewardedAd(adsType.preMonster);
        }
        else
            CallAdsMesPup();
    }

    public void WatchPreCharBallAds(int SetData)
    {
        if (AdsManager.instance.isRewardLoad(adsType.preCharBall))
        {
            AdsManager.instance.AdsRewardData = SetData;
            AdsManager.instance.ShowRewardedAd(adsType.preCharBall);
        }
        else
            CallAdsMesPup();
    }

    public void WatchPreSweetAim()
    {
        if (AdsManager.instance.isRewardLoad(adsType.SweetAim))
            AdsManager.instance.ShowRewardedAd(adsType.SweetAim);
        else
            CallAdsMesPup();
    }

    public void GetGiftCoin()
    {
        CallLoadingPup();
        ChargeAndUsedCoin(30);
        DataManager.instance.LobbyRewardTime = UnbiasedTime.Instance.Now().ToString();
        DataManager.instance.LobbyRewardLevel += 1;
        CheckStarRewardState(DataManager.instance.GetLobbyRewardCount() <= DataManager.instance.TotalStarCount);
        if (DataManager.instance.AutoSave)
            DataManager.instance.CallGameDataSave();
    }

    public void CallLoadingPup(float time = 1f)
    {
        loading.time = time;
        loading.CallPupTPTS();
        isLoad = true;
    }

    public void CloseLoadingPup()
    {
        loading.ClosePupTPTS();
        isLoad = false;
    }

    public void BuySetPackageItemData(int pIdx)
    {
        ChargeAndUsedCoin(DataManager.instance.ChargeJewelPackageList[pIdx].jewelCount);

        int item_1, item_2;
        item_1 = DataManager.instance.ChargeJewelPackageList[pIdx].InstanceItem_1;
        item_2 = DataManager.instance.ChargeJewelPackageList[pIdx].InstanceItem_2;
        if (0 < item_1)
            DataManager.instance.SetInstanceItemList(InstanceItem.AllBlockDamage, item_1);
        if (0 < item_2)
            DataManager.instance.SetInstanceItemList(InstanceItem.DoubleBall, item_2);
    }

    public void GetDailyReward()
    {
        switch (DataManager.instance.DailyRewardCount)
        {
            case 0:
                DataManager.instance.SetInstanceItemList(InstanceItem.AllBlockDamage, 1);
                break;
            case 1:
                DataManager.instance.SetCoin(50);
                break;
            case 2:
                DataManager.instance.SetInstanceItemList(InstanceItem.Undo, 1);
                break;
            case 3:
                DataManager.instance.SetCoin(100);
                break;
            case 4:
                DataManager.instance.SetInstanceItemList(InstanceItem.DoubleBall, 1);
                break;
            case 5:
                DataManager.instance.SetCoin(150);
                break;
        }

        if (DataManager.instance.DailyRewardCount > 4)
            DataManager.instance.DailyRewardCount = 0;
        else
            DataManager.instance.DailyRewardCount++;

        if (DataManager.instance.GetDailyRewardTime())
            DataManager.instance.DailyRewardTime = UnbiasedTime.Instance.Now().ToString();

        SetMyCoin();

        popupList[(int)PopupType.dailyReward - 2].GetComponent<DailyRewardControl>().ResetRewardPup();
        popupList[(int)PopupType.dailyReward - 2].GetComponent<DailyRewardControl>().SetButtonState();

        if (DataManager.instance.AutoSave)
            DataManager.instance.CallGameDataSave();
    }

    public void ClickDailyRewardBtn()
    {
        popupList[(int)PopupType.dailyReward - 2].CallPupTPTS();
        popupList[(int)PopupType.dailyReward - 2].GetComponent<DailyRewardControl>().isFirstCall = true;
    }

    public void CallDailyRewardPup()
    {
        popupList[(int)PopupType.dailyReward - 2].CallPupTPTS();
        popupList[(int)PopupType.dailyReward - 2].GetComponent<DailyRewardControl>().isFirstCall = false;
    }

    public void UseSweetAim(bool isPreAim = false)
    {
        myShop.ChangeAimState(isPreAim);
    }

    public void UseNoads()
    {
        myShop.ChangeNoAdsState(true);
    }

    public void AdsPupCall()
    {
        AdsPupTP.PlayForward();
        AdsPupTS.PlayForward();
        fakeAd = true;
    }

    public void AdsPupClose()
    {
        AdsPupTP.PlayReverse();
        AdsPupTS.PlayReverse();
        fakeAd = false;
    }

    public void AdsPupCloseBtnClick()
    {
        AdsManager.instance.CloseAdsPup();
        fakeAd = false;
    }

    public void BallSkillInfoDataSet(int bid, ShopState myState)
    {
        myBallView[0].BallSkillCall(bid, myState);
    }

    public void ClickFakeBanner()
    {
        Application.OpenURL(DataManager.TinyGolfURL);
    }

    public void ClickLeaderboard()
    {
        SoundManager.instance.TapSound();
        GPGSManager.instance.showLeaderBoard();
    }

    public void ClickAchievement()
    {
        SoundManager.instance.TapSound();
        GPGSManager.instance.showAchievements();
    }

    // 스테이지 팝업 추가로 인한 호출 함수
    public void ClickStageMapBtn()
    {
        SoundManager.instance.TapSound();
        popupList[6].CallPupTPTS();
    }

    public void CallAdsMesPup()
    {
        myAdsMessPup.CallPupTPTS();
    }

    public void OptionGoogleBtnCheck()
    {
#if UNITY_ANDROID
        optionPop.CheckGoogleBtn();
        if (!GPGSManager.instance.bLogin())
            optionPop.GoogleLogoutState();
#endif
    }

    public void SetOptionPupData()
    {
        initLobby();
        popupList[(int)PopupType.dailyReward - 2].GetComponent<DailyRewardControl>().ResetRewardPup();
        popupList[(int)PopupType.dailyReward - 2].GetComponent<DailyRewardControl>().SetButtonState();
        SetOptionToCloudData();
        myBallView[0].SetShopCloudData();
        housingList.initData();
    }

    public void CloseDailyRewardPup()
    {
        popupList[(int)PopupType.dailyReward - 2].GetComponent<DailyRewardControl>().ClosePupTPTS();
    }

    public void SetOptionToCloudData()
    {
        optionPop.SetSaveDatas();
    }
    public void TestDelectData()
    {
#if UNITY_IOS
        GPGSManager.instance.DelectSaveGame();
#endif
    }

    public void CloseOptionAndMenuBack()
    {
        curPopup = popupList[7];
    }

    public void CallLobbyAdsBtnClick()
    {
        SoundManager.instance.TapSound();
        if (AdsManager.instance.isRewardLoad(adsType.getCoin))
            AdsManager.instance.ShowRewardedAd(adsType.getCoin);
        else
            CallAdsMesPup();
    }

    public void StartStageGame()
    {
        SoundManager.instance.TapSound(1);
        DataManager.instance.CurGameStage = DataManager.instance.curStageIdx();
        DataManager.instance.GameStartCall();
    }

    public void OpenHousing(int hIdx)
    {
        housingPup.rewardIdx = hIdx;
        housingPup.CallPupTPTS();
    }

    void NoData()   // 데이터매니저가 없을시 프리로더로 이동
    {
        if (DataManager.instance == null)
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync((int)SceneType.Preload);
    }

    public void ClickSet()
    {
        popupList[10].CallPupTPTS();
    }

    public void HousingReset()
    {
        housingList.idx = 0;
        housingList.initData();
        housingList.BtnSet();
    }

    public void HousingIdxSet()
    {
        housingList.idx = DataManager.instance.HousingStateList.FindLastIndex(t => t >= 0);
        housingList.initData();
        housingList.StateCheck();
    }

    public void FacebookBtnClick()
    {
        Application.OpenURL(AdsManager.FacebookURL);
    }

    public void InstagramBtnClick()
    {
        Application.OpenURL(AdsManager.InstagramURL);
    }

    public void HousingHelp()
    {
        houInfo.CallPupTPTS();
    }

    public void CompleteHousing(int idx)
    {
        myBallView[0].ResetBallSelectBtns(idx);
    }

    public void TestSetCurStageTxt()
    {
        curScoreLabels[2].text = string.Format("{0} {1}", "STAGE", DataManager.instance.curStageIdx());
    }

    public void CallLobbyTuto()
    {
        lobbyTuto.CallPupTPTS();
    }

    public void HousingCheck()
    {
        housingList.NewSignCheck();
    }

    public void DR_ADChangeCurPup()
    {
        curPopup = myDRCommonPup;
    }

    public void SetMapPos()
    {
        if (!DataManager.instance.stageChapterStateList.Contains(0))
        {
            int index = DataManager.instance.stageChapterStateList.FindLastIndex(t => t == 1) + 1;
            if (index < DataManager.instance.stageChapterStateList.Count)
                DataManager.instance.SetChapterStateList(index, 0);
        }
    }

    public void TestLocalizationButtonClick()
    {
        if (DataManager.instance.languageState < Language.Japanese)
        {
            ++DataManager.instance.languageState;
        }
        else
        {
            DataManager.instance.languageState = Language.English;
        }
        testLocLabel.text = string.Format("{0}", DataManager.instance.languageState);
        LocalizationManager.CurrentLanguage = string.Format("{0}", DataManager.instance.languageState);
        SetStartBtnTxt();
    }

    void SetStartBtnTxt()
    {
        startButtonLabel[0].text = string.Format("{0} {1}", LocalizationManager.GetTranslation("Lobby_Tab_Stage"), DataManager.instance.curStageIdx());
        startButtonLabel[1].text = string.Format("{0} {1}", LocalizationManager.GetTranslation("Lobby_Tab_Stage"), 1);
    }
}