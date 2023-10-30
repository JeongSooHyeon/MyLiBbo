using UnityEngine;

public class OptionPopupControl : PopupBase
{
    [SerializeField] UIToggle[] SoundBtns;
    [SerializeField] UIToggle AudoBtn;
    [SerializeField] UILabel[] SaveDatas;
    [SerializeField] UILabel appVersionTxt;
    [SerializeField] GameObject FakeNativeObj;
    [SerializeField] GameObject RestoreBtn;
    [SerializeField] GameObject GameCenterBtn;
    [SerializeField] GameObject[] GoogleBtnObjs;
    [SerializeField] TutoPupControl myTuto;

    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        LobbyManager.instance.curPopup = this;
        appVersionTxt.text = string.Format("{0}", Application.version);
        if (!Application.isEditor)
        {
            AdsManager.instance.BannerEnable(false);
            AdsManager.instance.NativeEnable(true);
            FakeNativeObj.SetActive(false);
        }

#if UNITY_IOS
        GoogleBtnObjs[0].SetActive(false);
        GoogleBtnObjs[1].SetActive(false);
        RestoreBtn.SetActive(true);
#else

        CheckGoogleBtn();
        CheckGameCenterBtn();
#endif
        SetSaveDatas();
        if (GPGSManager.instance.bLogin())
            AudoBtn.value = DataManager.instance.AutoSave;
        else
        {
            GoogleLogoutState();
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
            GoogleLogoutState();
    }
#if UNITY_ANDROID
    public void CheckGoogleBtn()
    {
        if (PlayerPrefsElite.GetBoolean("GoogleLogin_"))
        {
            GoogleBtnObjs[0].SetActive(false);
            GoogleBtnObjs[1].SetActive(true);
        }
        else
        {
            GoogleBtnObjs[0].SetActive(true);
            GoogleBtnObjs[1].SetActive(false);
        }
    }

  
#endif
    public void GoogleLogoutState()
    {
        AudoBtn.value = false;
        DataManager.instance.AutoSave = false;
        defaultSaveData();
    }
    public void CheckGameCenterBtn()
    {
#if UNITY_ANDROID
        RestoreBtn.SetActive(false);
        GameCenterBtn.SetActive(false);
#endif
#if UNITY_IOS
        RestoreBtn.SetActive(true);
        if (GPGSManager.instance.bLogin())
        {
            RestoreBtn.transform.localPosition = RestoreBtnPoss[0];
            GameCenterBtn.SetActive(false);
        }
        else
        {
            RestoreBtn.transform.localPosition = RestoreBtnPoss[1];
            GameCenterBtn.SetActive(true);
        }
#endif

    }
    public override void ClosePupTPTS()
    {
        base.ClosePupTPTS();
        LobbyManager.instance.CloseOptionAndMenuBack();
        if (!Application.isEditor)
        {   
            AdsManager.instance.NativeEnable(false);
            AdsManager.instance.BannerEnable(true);
        }   
    }

    void Start()
    {
        CheckBGMSound();
        CheckEffectSound();
    }

    public void checkAutoSaveBtn()
    {
        if (Application.isEditor)
        {
            LobbyManager.instance.CallCommonPup((int)CommonState.InternetConnect);
            AudoBtn.value = false;
        }
        else
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                LobbyManager.instance.CallCommonPup((int)CommonState.InternetConnect);
                AudoBtn.value = false;
            }
            else
            {

                if (GPGSManager.instance.bLogin())
                {
                    SoundManager.instance.TapSound();
                    DataManager.instance.AutoSave = AudoBtn.value;
                    if (DataManager.instance.AutoSave)
                    {
                        DataManager.instance.CallGameDataSave();

                        SetSaveGameData();
                    }
                }else
                {
                    AudoBtn.value = false;
                    DataManager.instance.AutoSave = AudoBtn.value;
                }   
            }
        }
    }

    public void SetSaveDatas()
    {
        if(DataManager.instance.myCloudData != null && DataManager.instance.myCloudData.IngameRewardLevel > 0)
        {
            SaveDatas[0].text = DataManager.instance.myCloudData.CurGameStage.ToString();
            SaveDatas[1].text = DataManager.instance.GetTotalCloudStarCount().ToString();
            SaveDatas[2].text = DataManager.instance.myCloudData.coin.ToString();
        }else
        {
            defaultSaveData();
        }
        SaveDatas[3].text = DataManager.instance.curStageIdx().ToString();
        SaveDatas[4].text = DataManager.instance.GetTotalStarCount().ToString();
        SaveDatas[5].text = DataManager.instance.GetCoin().ToString();
        LobbyManager.instance.CloseLoadingPup();
    }

    void defaultSaveData()
    {
        SaveDatas[0].text = "0";
        SaveDatas[1].text = "0";
        SaveDatas[2].text = "0";
    }

    public void SetSaveGameData()
    {
        SaveDatas[0].text = (DataManager.instance.curStageIdx()).ToString();
        SaveDatas[1].text = DataManager.instance.GetTotalStarCount().ToString();
        SaveDatas[2].text = DataManager.instance.GetCoin().ToString();
        SaveDatas[3].text = (DataManager.instance.curStageIdx()).ToString();
        SaveDatas[4].text = DataManager.instance.GetTotalStarCount().ToString();
        SaveDatas[5].text = DataManager.instance.GetCoin().ToString();
    }

    public void SaveGameDataCloud()
    {
        if(Application.isEditor)
        {
            LobbyManager.instance.CallCommonPup((int)CommonState.InternetConnect);
        }
        else
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                LobbyManager.instance.CallCommonPup((int)CommonState.InternetConnect);
            }
            else
            {
                SoundManager.instance.TapSound();
#if UNITY_ANDROID
                LobbyManager.instance.CallLoadingPup();
                DataManager.instance.CallGameDataSave();
                SetSaveGameData();
#endif
#if UNITY_IOS
                if (GPGSManager.instance.bLogin())
                {
                    LobbyManager.instance.CallLoadingPup();
                    DataManager.instance.CallGameDataSave();
                    SetSaveGameData();
                }else
                {
                    LobbyManager.instance.CallCommonPup((int)CommonState.GameCenterLogin);
                }
#endif
            }
        }
    }

    public void CallGPGSLoad()
    {
        if (Application.isEditor)
        {
            LobbyManager.instance.CallCommonPup((int)CommonState.InternetConnect);
        }
        else
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                LobbyManager.instance.CallCommonPup((int)CommonState.InternetConnect);
            }
            else
            {
                SoundManager.instance.TapSound();
#if UNITY_ANDROID
                LobbyManager.instance.CallLoadingPup();
                GPGSManager.instance.mType = GpgsLoginType.save;
                GPGSManager.instance.LoadFromCloud();
#endif
#if UNITY_IOS
                if (GPGSManager.instance.bLogin())
                {
                    LobbyManager.instance.CallLoadingPup();
                    GPGSManager.instance.mType = GpgsLoginType.save;
                    GPGSManager.instance.LoadFromCloud();
                }else
                {
                    LobbyManager.instance.CallCommonPup((int)CommonState.GameCenterLogin);
                }
#endif
            }
        }
    }

    public void ClickBGMBtn()
    {
        SoundManager.instance.TapSound();
        SoundManager.instance.toggleBGM(SoundBtns[0].value);
    }

    public void ClickEffectBtn()
    {
        SoundManager.instance.TapSound();
        SoundManager.instance.toggleFXSound(SoundBtns[1].value);
    }

    public void ClickRateUs()
    {
        SoundManager.instance.TapSound();
        Application.OpenURL(DataManager.appURL);
    }

    public void ClickFaceBook()
    {
        SoundManager.instance.TapSound();
        Application.OpenURL("https://www.facebook.com/DeliciousGame");
    }

    void CheckBGMSound()
    {
        SoundBtns[0].value = SoundManager.instance.BGMSoundState;
    }

    void CheckEffectSound()
    {
        SoundBtns[1].value = SoundManager.instance.FXSoundState;
    }

    public void ClickIOSRestore()
    {
        SoundManager.instance.TapSound();
        DataManager.instance.CallRestore();
    }

    public void ClickGameCenterBtn()
    {
        SoundManager.instance.TapSound();
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            LobbyManager.instance.CallCommonPup((int)CommonState.GameCenterLogin);
        }else
        {
            LobbyManager.instance.CallCommonPup((int)CommonState.InternetConnect);
        }
    }

    public void ClickGoogleLogin()
    {
        SoundManager.instance.TapSound();
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            LobbyManager.instance.CallLoadingPup();
            GPGSManager.instance.mType = GpgsLoginType.login;
            GPGSManager.instance.LoginGPGS();
        }   
    }

    public void ClickGoogleLogout()
    {
        SoundManager.instance.TapSound();
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            defaultSaveData();
            GPGSManager.instance.LogoutGoogle();
            AudoBtn.value = false;
            DataManager.instance.AutoSave = false;
        }
            
    }

    public void ClickLobbyTuto()
    {
        myTuto.CallPupTPTS();
    }
}
