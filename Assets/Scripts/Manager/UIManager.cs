using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [SerializeField] UISprite touchSprite = null;
    [SerializeField] _2D_Reflection LineRef;
    [SerializeField] UILabel ballCount = null, moveLabel = null, endScoreLabel = null, oneMoreLabel = null, centerScore, bestScoreLabel = null, centerCombo;
    [SerializeField] UILabel[] stageLabel;
    [SerializeField] TweenAlpha moveLabelAlpha, volumAlpha;
    [SerializeField] TweenPosition charTween;
    [SerializeField] TweenScale volumeLabel;
    [SerializeField] TweenAlpha scoreGlowTA;
    [SerializeField] UISlider ScoreProgressBar;
    [SerializeField] UISprite[] ScoreStarImgs;
    [SerializeField] UISprite[] ScoreStarImgsSuc;
    [SerializeField] UISprite[] ScoreStarImgsFail;
    [SerializeField] UISprite[] modeImg;
    [SerializeField] GameObject[] TopHud;
    [SerializeField] UILabel[] scoreLabelList;
    [SerializeField] UILabel[] coinLabelList;
    [SerializeField] UILabel moneyLabel; // 슈팅모드 재화
    [SerializeField] UILabel[] starLabelList;
    [SerializeField] ParticleSystem[] starFx;
    public bool isResult = false;
    public bool isShowLine;
    public bool isContinueGame = false;
    public bool continueTodayAD = false;
    public string today;
    public int startShow = 0;
    float time_ = 0;
    public float continueTime;
    public static UIManager instance;
    [SerializeField] UIToggle[] soundToggles_;
    [SerializeField] UIPanel speedUpPanel;
    [SerializeField] GameObject[] showPlayObj;
    public List<BottomItemBtn> showPlayItem;
    [SerializeField] BottomItemBtn plusBallAdsBtn;
    [SerializeField] GameObject[] startPlayObj;
    [SerializeField] GameObject arrowSprite;
    [SerializeField] UIButton oneMoreBtn;
    [SerializeField] CommonPupControl myCommonPup;
    [SerializeField] CommonPupControl rewardPup;
    [SerializeField] CommonPupControl plusBallPup;
    [SerializeField] GameObject[] AdsBtnArr;
    [SerializeField] GameObject[] GiftBtnArr;
    [SerializeField] LoadingPupControl loading;
    [SerializeField] UISlider[] rewardBar;
    [SerializeField] UISlider restartTimeBar;
    [SerializeField] GameObject[] rewardObj1;
    [SerializeField] GameObject[] rewardObj2;
    [SerializeField] GameObject BottomWall;
    [SerializeField] GameObject[] rewardObj3;
    [SerializeField] GameObject[] rewardObj4;
    [SerializeField] GameObject[] continueObj1;
    [SerializeField] BottomBtnSet b_BtnSet;
    [SerializeField] GameObject[] startTutorialNum;
    int clickAdsBtn = 0;
    [SerializeField] NewCurrentShipShow cheerAnim;
    [SerializeField] TweenPosition AdsPupTP;
    [SerializeField] TweenScale AdsPupTS;
    [SerializeField] IngameNativeAdControl nativeCtr;
    [SerializeField] AdsMessagePupControl myAdsMessPup;
    [HideInInspector] public bool isShowAds = false;
    [SerializeField] GameObject[] noShootObj;

    // 쇼크웨이브 파라미터
    [SerializeField] Camera thisCamera;
    [SerializeField] float MaxRadius;
    [SerializeField] float Speed;
    [SerializeField] float Amp;
    [SerializeField] float WS;

    // 카메라 트윈
    [SerializeField] TweenPosition cameraTP;

    // 슈팅모드
    public GameObject cannon;
    [SerializeField] GameObject editBtn;
    [SerializeField] GameObject[] topWall;
    [SerializeField] GameObject[] bundle_Bg;
    [SerializeField] UIPanel brickPanel;

    public int totalBossHp = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Time.timeScale = 1;
        SetMoney();
        if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING) BallManager.instance.ballHit = BallManager.instance.attack;
        else BallManager.instance.ballHit = DataManager.instance.FirePower;
        /*if (PlayerPrefsElite.GetInt("First") == 1)
        {
            LobbyController.instance.SetTween(UIState.Tutorial, true);
            Debug.Log("First");
            PlayerPrefsElite.SetInt("First", 2);
        }*/
        if (!DataManager.instance.GetSaveFile()) DataManager.instance.CurrentScore = 0;
        SetScore();
        Invoke("SoundSetting", 0.21f);
        SetHideBtn();
        switch ((GameMode)DataManager.instance.CurGameMode)
        {
            case GameMode.CLASSIC:
                TopHud[1].SetActive(true);
                break;
            case GameMode.SHOOTING:
                TopHud[3].SetActive(true);
                break;
            case GameMode.BALL100:
                TopHud[2].SetActive(true);
                break;
            case GameMode.STAGE:
                TopHud[0].SetActive(true);
                break;
            default:
                TopHud[0].SetActive(true);
                break;
        }
        ShowDownBtn(false);

        if (DataManager.instance.SweetAimPreUse || DataManager.instance.SweetAimUse)
        {
            if (DataManager.instance.SweetAimPreUse) DataManager.instance.SweetAimPreUse = false;
            LineRef.Set_Rays_Count(4);
        }

        else
        {
            LineRef.Set_Rays_Count(2);
        }
        today = UnbiasedTime.Instance.Now().ToString("yyyy-MM-dd");
        if (PlayerPrefs.HasKey("continueDateAD"))
        {
            continueTodayAD = today.Equals(PlayerPrefsElite.GetString("continueDateAD")) ? true : false;
        }
        if ((GameMode)DataManager.instance.CurGameMode == GameMode.STAGE || (GameMode)DataManager.instance.CurGameMode == GameMode.STAGEBOSS)
            StartTutorial();
        if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
        {
            if (!DataManager.instance.ShootingTuto) LobbyController.instance.SetTween(UIState.ShootingTuto, true);    // 슈팅모드 튜토리얼
            ballCount.transform.localPosition = new Vector3(0, 100, 0);
        }
        cannon.SetActive(DataManager.instance.CurGameMode == (int)GameMode.SHOOTING);
        editBtn.SetActive(DataManager.instance.CurGameMode == (int)GameMode.SHOOTING && DataManager.instance.isTestMode);
        ShootingUI();
        for (int i = 0; i < noShootObj.Length; i++)
            noShootObj[i].SetActive(DataManager.instance.CurGameMode != (int)GameMode.SHOOTING);
    }

    public void StartTutorial() // 튜토리얼 셋팅
    {
        bool tuto = true;
        switch (DataManager.instance.CurGameStage)
        {
            case 1:
                if (PlayerPrefs.HasKey("StartTutorial"))
                {
                    tuto = false;
                }
                else
                {
                    startTutorialNum[0].SetActive(true);
                }
                break;
            case 2:
                if (PlayerPrefsElite.GetInt("StartTutorial") < DataManager.instance.CurGameStage)
                {
                    startTutorialNum[2].SetActive(true);
                }
                else
                {
                    tuto = false;
                }
                break;
            case 3:
                if (PlayerPrefsElite.GetInt("StartTutorial") < DataManager.instance.CurGameStage)
                {
                    startTutorialNum[3].SetActive(true);
                }
                else
                {
                    tuto = false;
                }
                break;
            case 4:
                if (PlayerPrefsElite.GetInt("StartTutorial") < DataManager.instance.CurGameStage)
                {
                    startTutorialNum[1].SetActive(true);
                }
                else
                {
                    tuto = false;
                }
                break;
            case 5:
                if (PlayerPrefsElite.GetInt("StartTutorial") < DataManager.instance.CurGameStage)
                {
                    startTutorialNum[4].SetActive(true);
                }
                else
                {
                    tuto = false;
                }
                break;
            case 6:
                if (PlayerPrefsElite.GetInt("StartTutorial") < DataManager.instance.CurGameStage)
                {
                    startTutorialNum[7].SetActive(true);
                }
                else
                {
                    tuto = false;
                }
                break;
            case 7:
                if (PlayerPrefsElite.GetInt("StartTutorial") < DataManager.instance.CurGameStage)
                {
                    startTutorialNum[5].SetActive(true);
                }
                else
                {
                    tuto = false;
                }
                break;
            case 9:
                if (PlayerPrefsElite.GetInt("StartTutorial") < DataManager.instance.CurGameStage)
                {
                    startTutorialNum[11].SetActive(true);
                }
                else
                {
                    tuto = false;
                }
                break;
            case 10:
                if (PlayerPrefsElite.GetInt("StartTutorial") < DataManager.instance.CurGameStage)
                {
                    startTutorialNum[12].SetActive(true);
                }
                else
                {
                    tuto = false;
                }
                break;
            case 12:
                if (PlayerPrefsElite.GetInt("StartTutorial") < DataManager.instance.CurGameStage)
                {
                    startTutorialNum[14].SetActive(true);
                }
                else
                {
                    tuto = false;
                }
                break;
            case 18:
                if (PlayerPrefsElite.GetInt("StartTutorial") < DataManager.instance.CurGameStage)
                {
                    startTutorialNum[13].SetActive(true);
                }
                else
                {
                    tuto = false;
                }
                break;
            case 22:
                if (PlayerPrefsElite.GetInt("StartTutorial") < DataManager.instance.CurGameStage)
                {
                    startTutorialNum[8].SetActive(true);
                }
                else
                {
                    tuto = false;
                }
                break;
            case 42:
                if (PlayerPrefsElite.GetInt("StartTutorial") < DataManager.instance.CurGameStage)
                {
                    startTutorialNum[6].SetActive(true);
                }
                else
                {
                    tuto = false;
                }
                break;
            case 46:
                if (PlayerPrefsElite.GetInt("StartTutorial") < DataManager.instance.CurGameStage)
                {
                    startTutorialNum[9].SetActive(true);
                }
                else
                {
                    tuto = false;
                }
                break;
            case 54:
                if (PlayerPrefsElite.GetInt("StartTutorial") < DataManager.instance.CurGameStage)
                {
                    startTutorialNum[10].SetActive(true);
                }
                else
                {
                    tuto = false;
                }
                break;
            default:
                tuto = false;
                break;
        }

        if (tuto)
        {
            LobbyController.instance.SetTween(UIState.Tutorial2, true);
            PlayerPrefsElite.SetInt("StartTutorial", DataManager.instance.CurGameStage);
        }
    }
    public void SetCharPosSetting() // 볼 갯수 라벨 셋팅
    {
        charTween.from.x = charTween.transform.localPosition.x;
        charTween.to.x = BallManager.instance.RetunrnFirstBall();
        charTween.ResetToBeginning();
        charTween.PlayForward();
    }

    void SoundSetting() // 사운드 셋팅
    {
        if ((GameMode)DataManager.instance.CurGameMode == GameMode.STAGE)
        {
            DataManager.instance.ResetPreviewCharBall();
        }
        ResultOneMoreBtnDelete();
        soundToggles_[0].value = !SoundManager.instance.BGMSoundState;
        soundToggles_[1].value = !SoundManager.instance.FXSoundState;
    }

    public void BottmSetting()  // bottom, 콤보애니 셋팅
    {
        b_BtnSet.SetReset();
        cheerAnim.SetComboAnim();
    }

    public void ArrowSpriteSetting(bool isShow, float eulerZ)   // 가이드라인 셋팅
    {
        touchSprite.enabled = isShow;
        arrowSprite.transform.localEulerAngles = new Vector3(0, 0, eulerZ);
        plusGuidePos = eulerZ;
    }

    public void SetSprite(bool isShow, Vector3 pos)
    {
        isShowLine = isShow;
        arrowSprite.transform.localPosition = new Vector2(pos.x, arrowSprite.transform.localPosition.y);
    }

    float plusGuidePos;


    public void SetTouchSprite(Vector3 pos)
    {
        touchSprite.transform.position = pos;
    }

    public void SetSpeedPanel()
    {
        speedUpPanel.enabled = true;
        StartCoroutine(ShowSpeedPanel());
    }

    IEnumerator ShowSpeedPanel()
    {
        yield return new WaitForSeconds(0.7f);
        speedUpPanel.enabled = false;
    }

    public void ShowDownBtn(bool isTrue)
    {
        if (DataManager.instance.CurGameMode != (int)GameMode.SHOOTING)
        {
            showPlayObj[0].SetActive(isTrue);
            showPlayObj[1].SetActive(!isTrue);
            showPlayObj[5].SetActive(false); // 슈팅모드 전용 bottom UI
            setStartShow();
        }
        else
        {
            showPlayObj[0].SetActive(false);
            showPlayObj[1].SetActive(false);
            showPlayObj[5].SetActive(true); // 슈팅모드 전용 bottom UI
        }
    }

    public void setStartShow()
    {
        if (startShow == 0)
        {
            startPlayObj[0].SetActive(false);
            startPlayObj[1].SetActive(true);
        }
        else if (startShow > 0)
        {
            startPlayObj[0].SetActive(true);
            startPlayObj[1].SetActive(false);
        }
    }

    void SetHideBtn()   // Bottom 부분 버튼 셋팅
    {
        if ((GameMode)DataManager.instance.CurGameMode == GameMode.BALL100)
        {
            showPlayObj[2].SetActive(false);
            showPlayObj[3].SetActive(false);
            showPlayObj[4].SetActive(false);
        }

        if (DataManager.instance.curMapDataList.MyGM == GameMode.STAGEBOSS)
        {
            showPlayObj[2].SetActive(false);
            showPlayObj[3].SetActive(false);
        }
        startPlayObj[0].SetActive(false);
    }

    public void setBottomBtn()
    {
        for (int i = 0; i < showPlayItem.Count; ++i)
        {
            showPlayItem[i].SetBottomShow();
        }
    }


    public void Continue(int tweenNum)
    {
        if (isResult)
        {
            return;
        }
        isResult = true;
        switch (tweenNum)
        {
            case 0:
                clickAdsBtn = 0;
                RewardSetting(clickAdsBtn);
                DataManager.instance.SetSaveFile(false);
                DataManager.instance.isGameClear = true;
                DataManager.instance.SaveGameData(true);
                starLabelList[tweenNum].text = string.Format("{0:N0}", DataManager.instance.TotalStarCount.ToString());
                LobbyController.instance.SetTween(UIState.ResultSuc, true);
                GiftBtnEnable(DataManager.instance.GetLobbyRewardTime());
                SoundManager.instance.ChangeEffects(7);
                setClearStageLog();
                break;
            case 1:
                Time.timeScale = 0;
                clickAdsBtn = 1;
                DataManager.instance.isGameClear = false;
                starLabelList[tweenNum].text = string.Format("{0:N0}", DataManager.instance.TotalStarCount.ToString());
                LobbyController.instance.SetTween(UIState.ResultFailed, true);
                SoundManager.instance.ChangeEffects(5);
                GiftBtnEnable(DataManager.instance.GetLobbyRewardTime());
                RewardSetting(clickAdsBtn);
                setClearStageLog();
                break;
            case 2:
                Time.timeScale = 0;
                if (DataManager.instance.CurGameMode == (int)GameMode.BALL100)
                {
                    int samScore = 0;
                    if (PlayerPrefs.HasKey("Sam100BallScore"))
                        samScore = PlayerPrefsElite.GetInt("Sam100BallScore");
                    samScore += DataManager.instance.CurrentScore;
                    GPGSManager.instance.setScoreLeaderBoard(1, samScore);
                    PlayerPrefsElite.SetInt("Sam100BallScore", samScore);
                }
                else if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
                {
                    if (DataManager.instance.CurrentScore > DataManager.instance.GetBestScore()) DataManager.instance.SetBestScore();
                    int shootingScore = 0;
                    if (PlayerPrefs.HasKey("SumShootingScore"))
                        shootingScore = PlayerPrefsElite.GetInt("SumShootingScore");
                    shootingScore += DataManager.instance.CurrentScore;
                    GPGSManager.instance.setScoreLeaderBoard(3, shootingScore);
                    PlayerPrefsElite.SetInt("SumShootingScore", shootingScore);
                }

                modeImg[0].enabled = (DataManager.instance.CurGameMode == (int)GameMode.BALL100);
                modeImg[1].enabled = (DataManager.instance.CurGameMode == (int)GameMode.CLASSIC);
                modeImg[2].enabled = (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING);

                clickAdsBtn = 2;
                DataManager.instance.isGameClear = false;
                bestScoreLabel.text = string.Format("{0:N0}", DataManager.instance.GetBestScore());
                LobbyController.instance.SetTween(UIState.BestScore, true);
                SoundManager.instance.ChangeEffects(7);
                GiftBtnEnable(DataManager.instance.GetLobbyRewardTime());
                RewardSetting(clickAdsBtn);
                break;
            case 3:
                time_ = continueTime;
                isContinueGame = true;
                continueObj1[0].SetActive(continueTodayAD || !AdsManager.instance.isRewardLoad(adsType.continueAD));
                continueObj1[1].SetActive(!continueTodayAD && AdsManager.instance.isRewardLoad(adsType.continueAD));
                DataManager.instance.isGameClear = false;
                LobbyController.instance.SetTween(UIState.ReStartGame, true);
                if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
                    BallManager.instance.ShootingResetBall(false);
                BallManager.instance.isDelay = false;
                LobbyController.instance.isResult = true;
                BallManager.instance.isArrow = false;
                BallManager.instance.isMove = false;
                touchSprite.enabled = false;
                break;
        }

        if (!DataManager.instance.NoAdsUse)
        {
            for (int i = 0; i < AdsBtnArr.Length; ++i)
            {
                AdsBtnArr[i].SetActive(DataManager.instance.GetLobbyGetCoinAdsTime());
            }
        }
    }

    public void setClearStageLog()
    {
        if (DataManager.instance.CurGameStage <= 30)
        {
            string msg = string.Format("스테이지 {0} 클리어", DataManager.instance.CurGameStage);
            DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.new_user, msg);
        }
        switch (DataManager.instance.CurGameStage)
        {
            case 100:
                Stage100Clear();
                break;
        }
    }

    void Stage100Clear()
    {
        if (DataManager.instance.isGameClear)
        {
            GPGSManager.instance.setAchievements(2);
        }
    }

    private void Update()
    {
        if (isContinueGame)
        {
            if (!isShowAds) time_ -= Time.deltaTime;
            restartTimeBar.value = time_ / continueTime;
            if (time_ <= 0)
            {
                FalseContinue();
            }
        }


    }

    public void FalseContinue()
    {
        LobbyController.instance.SetTween(UIState.ReStartGame, false);
        isContinueGame = false;
        isResult = false;
        if (DataManager.instance.CurGameMode == (int)GameMode.CLASSIC
            || DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
        {
            BrickManager.instance.SetBrickGray(2);
        }

        else
        {
            BrickManager.instance.SetBrickGray(1);
        }
    }

    public void SetFXSound()
    {
        SoundManager.instance.toggleFXSound(!soundToggles_[1].value);

    }
    public void SetBGMSound()
    {
        SoundManager.instance.toggleBGM(!soundToggles_[0].value);
    }

    public void Result()
    {
        DataManager.instance.SetSaveFile(false);
        DataManager.instance.SetOnemore(false);
    }


    public void ResetGame()
    {
        Time.timeScale = 1;
        GC.Collect();
        Init();
        DataManager.instance.SceneLoad();
    }

    public void BottomWallTrue(bool isTrue)
    {
        BottomWall.SetActive(isTrue);
    }

    public void CallCommonPup(int sIdx, int cData = 0, int sData = 0)
    {
        LobbyController.instance.noticeNum = 0;
        myCommonPup.curState = (CommonState)sIdx;
        if (cData > 0)
            myCommonPup.SetData = cData;
        if (sData > 0)
            myCommonPup.SubData = sData;
        myCommonPup.CallPupTPTS();
    }

    public void CallRewardPup(int sIdx, int cData = 0, int sData = 0)
    {
        LobbyController.instance.noticeNum = 1;
        rewardPup.curState = (CommonState)sIdx;
        if (cData > 0)
            rewardPup.SetData = cData;
        if (sData > 0)
            rewardPup.SubData = sData;
        rewardPup.CallPupTPTS();
    }

    public void CallPlusBallPup(int sIdx, int cData = 0, int sData = 0)
    {
        LobbyController.instance.noticeNum = 2;
        plusBallPup.curState = (CommonState)sIdx;
        if (cData > 0)
            plusBallPup.SetData = cData;
        if (sData > 0)
            plusBallPup.SubData = sData;
        plusBallPup.CallPupTPTS();
        Time.timeScale = 0;
    }

    public void PlusBallAd()
    {
        if (Application.isEditor)
        {
            plusBallAdsBtn.plusBallAd();
        }
        else
        {
            if (AdsManager.instance.isRewardLoad(adsType.plusBall))
                AdsManager.instance.ShowRewardedAd(adsType.plusBall);
            else
            {
                CallAdsMesPup();
            }
        }
    }

    public void ClickWatchAdsBtn()
    {
        Time.timeScale = 1;
        if (AdsBtnArr[clickAdsBtn].activeSelf)
        {
            CallCommonPup(3);
        }
    }

    public void ClickGiftBtn()
    {
        Time.timeScale = 1;
        if (GiftBtnArr[clickAdsBtn].activeSelf)
        {
            CallCommonPup(4);
        }
    }

    public void ClickPlusBallAd()
    {
        Time.timeScale = 0;
        CallPlusBallPup(5);
    }

    public void WatchGetCoinAds()
    {
        if (Application.isEditor)
        {
            DataManager.instance.LobbyGetCoinAds = UnbiasedTime.Instance.Now().ToString();
            DataManager.instance.SetCoin(AdsManager.GetCoin);
            SetMoney();

            AdsBtnArr[clickAdsBtn].SetActive(false);
        }
        else
        {
            if (AdsManager.instance.isRewardLoad(adsType.getCoin))
            {
                AdsManager.instance.ShowRewardedAd(adsType.getCoin);
                SetMoney();
                AdsBtnArr[clickAdsBtn].SetActive(false);
            }
            else
            {
                CallAdsMesPup();
            }
        }
    }

    public void GetGiftCoin()
    {
        ChargeAndUsedCoin(30);
        DataManager.instance.LobbyRewardTime = UnbiasedTime.Instance.Now().ToString();
        GiftBtnEnable(false);
    }

    public void GiftBtnEnable(bool isable)
    {
        GiftBtnArr[clickAdsBtn].SetActive(isable);
    }

    public void RewardSetting(int a)
    {
        DataManager.instance.CheckIngameReward();
        rewardBar[a].value = DataManager.instance.GetIngameRewardItemValue_2();
        rewardObj1[a].SetActive(DataManager.instance.GetIngameRewardState(1) != 0);
        rewardObj2[a].SetActive(DataManager.instance.GetIngameRewardState(2) != 0);

        if ((int)DataManager.instance.GetIngameRewardState(1) == 1)
        {
            CallRewardPup(0);
            rewardObj3[0].SetActive(true);
            rewardObj4[0].SetActive(true);
            rewardObj2[a].SetActive(false);
        }
        else if ((int)DataManager.instance.GetIngameRewardState(2) == 1)
        {
            CallRewardPup(2);
            rewardObj3[1].SetActive(true);
            rewardObj4[1].SetActive(true);
        }
    }

    public void ClickRewardBtn1()
    {
        Time.timeScale = 1;
        ChargeAndUsedCoin(5);
        DataManager.instance.SetIngameRewardState(1, IngameRewardState.Rewarded);
    }

    public void ClickRewardAdBtn1()
    {
        Time.timeScale = 1;
        if (Application.isEditor)
        {
            ChargeAndUsedCoin(AdsManager.instance.GetRewardCoin);
            DataManager.instance.SetIngameRewardState(1, IngameRewardState.Rewarded);
            rewardPup.ClosePupTPTS();
            CallLoadingPup();
        }
        else
        {
            if (AdsManager.instance.isRewardLoad(adsType.rewardCoin))
            {
                AdsManager.instance.ShowRewardedAd(adsType.rewardCoin);
                rewardPup.ClosePupTPTS();
                CallLoadingPup();
            }
            else
            {
                CallAdsMesPup();
            }
        }
    }


    public void ClickRewardBtn2()
    {
        Time.timeScale = 1;
        DataManager.instance.SetInstanceItemList(InstanceItem.AllBlockDamage, 1);
        DataManager.instance.SetIngameRewardState(2, IngameRewardState.Rewarded);
    }

    public void ClickRweardAdBtn2()
    {
        Time.timeScale = 1;

        if (Application.isEditor)
        {
            DataManager.instance.SetInstanceItemList(InstanceItem.AllBlockDamage, AdsManager.instance.GetRewardItem);
            DataManager.instance.SetIngameRewardState(2, IngameRewardState.Rewarded);
            rewardPup.ClosePupTPTS();
            CallLoadingPup();
        }
        else
        {
            if (AdsManager.instance.isRewardLoad(adsType.rewardItem))
            {
                AdsManager.instance.ShowRewardedAd(adsType.rewardItem);
                rewardPup.ClosePupTPTS();
                CallLoadingPup();
            }
            else
            {
                CallAdsMesPup();
            }
        }
    }

    public void ContinueToday()
    {
        continueTodayAD = true;
        PlayerPrefsElite.SetString("continueDateAD", today);
    }

    public void ChargeAndUsedCoin(int coin)
    {
        DataManager.instance.SetCoin(coin);
        SetMoney();
    }

    public void CallLoadingPup(float time = 1f)
    {
        loading.time = time;
        loading.CallPupTPTS();
    }

    public void SetScore()  // 점수 셋팅
    {
        for (int i = 0; i < scoreLabelList.Length; ++i)
        {
            scoreLabelList[i].text = string.Format("{0:N0}", DataManager.instance.CurrentScore);
        }
        endScoreLabel.text = string.Format("{0:N0}", DataManager.instance.CurrentScore);

        int stage = DataManager.instance.CurGameStage;

        for (int i = 0; i < stageLabel.Length; ++i)
        {
            stageLabel[i].text = string.Format("{0}", stage);
        }
    }

    public void SetVolumeScore(int s)
    {
        centerScore.text = s.ToString();
        volumAlpha.ResetToBeginning();
        volumAlpha.PlayForward();
        volumeLabel.ResetToBeginning();
        volumeLabel.PlayForward();
    }

    public void Init()
    {
        if (!BrickManager.instance.isStage) DataManager.instance.SetSaveFile(false);
        BrickManager.instance.Init();
    }

    public void SetMoney()  // 보석 라벨 셋팅
    {
        for (int i = 0; i < coinLabelList.Length; ++i)
        {
            if (DataManager.instance.GetCoin() > 99999)
                coinLabelList[i].text = "99,999+";
            else coinLabelList[i].text = string.Format("{0:N0}", DataManager.instance.GetCoin());
        }
    }

    public void SetBallCount(int cnt) // UI에서 공의 갯수를 세주는 함수
    {
        if (cnt == 0)
        {
            if (DataManager.instance.CurGameMode != (int)GameMode.SHOOTING)
                ballCount.enabled = false;
            else ballCount.text = "x 0";
        }
            
        else
        {
            ballCount.text = string.Format("x {0}", cnt);
        }
    }

    public void SetStartBall()
    {
        ballCount.enabled = true;
    }

    public void SetLabelPos(int cnt)
    {
        if (cnt > 0)
        {
            moveLabel.text = "+ " + cnt;
            moveLabelAlpha.ResetToBeginning();
            moveLabelAlpha.PlayForward();
        }

    }

    public void ResultOneMoreBtnDelete()
    {
        if (DataManager.instance.GetOneMoreBool())
        {
            oneMoreBtn.isEnabled = false;
            oneMoreLabel.color = Color.gray;
        }
    }

    public void CheckScoreStar()
    {
        ScoreProgressBar.value = DataManager.instance.GetStageStarValue();
        for (int i = 0; i < ScoreStarImgs.Length; ++i)
        {
            if (i == 0 || i < DataManager.instance.GetStageStarCount(true))
            {
                ScoreStarImgs[i].enabled = true;
                ScoreStarImgsSuc[i].enabled = true;
                ScoreStarImgsFail[i].enabled = true;
                starFx[i].Play();
            }
            else
            {
                ScoreStarImgs[i].enabled = false;
                ScoreStarImgsSuc[i].enabled = false;
                ScoreStarImgsFail[i].enabled = false;
            }
        }

        scoreGlowTA.ResetToBeginning();
        scoreGlowTA.PlayForward();
    }

    public void ResetScoreGlow()
    {
        scoreGlowTA.ResetToBeginning();
    }

    int losBossHp = 0;
    float bossScore = 0;
    int ret = 0;
    public void CheckBossStar()
    {
        ++losBossHp;
        DataManager.instance.CurrentScore += 10;
        SetScore();
        bossScore = (float)losBossHp / totalBossHp;
        ScoreProgressBar.value = bossScore;
        if (bossScore < 0.6f && bossScore > 0f) ret = 1;
        else if (bossScore >= 0.6f && bossScore < 1f) ret = 2;
        else if (bossScore == 1f) ret = 3;
        for (int i = 0; i < ScoreStarImgs.Length; ++i)
        {
            if (i == 0 || i < ret)
            {
                ScoreStarImgs[i].enabled = true;
                ScoreStarImgsSuc[i].enabled = true;
                ScoreStarImgsFail[i].enabled = true;
            }
            else
            {
                ScoreStarImgs[i].enabled = false;
                ScoreStarImgsSuc[i].enabled = false;
                ScoreStarImgsFail[i].enabled = false;
            }
        }
    }



    public void AdsPupCall()
    {
        AdsPupTP.PlayForward();
        AdsPupTS.PlayForward();
    }

    public void AdsPupClose()
    {
        AdsPupTP.PlayReverse();
        AdsPupTS.PlayReverse();
        ClickBtnResult();
    }

    public void AdsPupCloseBtnClick()
    {
        AdsManager.instance.CloseAdsPup();
    }

    public void sumBossHp(int hp)
    {
        totalBossHp += hp;
    }

    public void NativeAdPop(bool set)
    {
        if (set) nativeCtr.CallNative();
        else nativeCtr.CloseNative();
    }

    IEnumerator CallBackInterstitial()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        int state = PlayerPrefsElite.GetInt("ResultBtnState");
        switch (state)
        {
            case 0: // ResultButton : OnClick() - case 1, LobbyButton : OnClick() - case UIState.Exit
                if (BrickManager.instance.isStage && !LobbyController.instance.isSave) DataManager.instance.SaveGameData();
                ResetGame();
                break;
            case 1: // ResultButton : OnClick() - case 2
                if (DataManager.instance.isLastStage()) ResetGame();
                else
                {
                    DataManager.instance.SaveGameData(true);
                    DataManager.instance.NextStagePlay();
                }
                break;
            case 2: // LobbyButton : OnClick() - case UIState.ReStartGame
                if (!BrickManager.instance.isStage) DataManager.instance.SetSaveFile(false);
                if ((GameMode)DataManager.instance.CurGameMode == GameMode.BALL100) DataManager.instance.NextStagePlay();
                else DataManager.instance.SceneLoad(SceneType.Ingame);
                break;
        }
    }
    public void ClickBtnResult()
    {
        int state = PlayerPrefsElite.GetInt("ResultBtnState");
        switch (state)
        {
            case 0: // ResultButton : OnClick() - case 1, LobbyButton : OnClick() - case UIState.Exit
                ResetGame();
                break;
            case 1: // ResultButton : OnClick() - case 2
                if (DataManager.instance.isLastStage()) ResetGame();
                else
                {
                    DataManager.instance.NextStagePlay();
                }
                break;
            case 2: // LobbyButton : OnClick() - case UIState.ReStartGame
                if (!BrickManager.instance.isStage) DataManager.instance.SetSaveFile(false);
                if ((GameMode)DataManager.instance.CurGameMode == GameMode.BALL100) DataManager.instance.NextStagePlay();
                else DataManager.instance.SceneLoad(SceneType.Ingame);
                break;
        }
        AdsManager.instance.isAdsPlay = false;
    }

    public void CallAdsMesPup()
    {
        LobbyController.instance.TurnAdsPup();
        myAdsMessPup.CallPupTPTS();
    }

    public void ClickContinueBtn()
    {
        isContinueGame = false;
        isResult = false;
    }

    public void ShowShockWave(Vector3 pos)
    {
        cameraTP.ResetToBeginning();
        cameraTP.PlayForward();

        ShockWave.Get(thisCamera).StartIt(pos, false, Speed, MaxRadius, Amp, WS);
        StartCoroutine(SecondShock(0.1f, pos));
    }

    IEnumerator SecondShock(float time, Vector3 pos)
    {
        yield return new WaitForSecondsRealtime(time);

        ShockWave.Get(thisCamera).StartIt(pos, false, Speed, MaxRadius, Amp, WS);
    }

    public void PlayStarFxSuc(ParticleSystem fx, bool on)
    {
        if (on)
            fx.Play();
    }

    public void SetShootingMoney(int m)
    {
        //moneyLabel.text = string.Format("{0:N0}", m);
        moneyLabel.text = TextChange(m);
    }

    string[] CountUnit = { "", "K", "M", "G", "T", "P", "E", "Z", "Y", "I", "J", "A", "L", "B", "N", "O", "F", "Q", "R", "S", "D", "U", "V", "W", "X", "H", "C" };

    public string TextChange(double num)
    {

        int nZeros = (int)(System.Math.Log10(num));
        int prefixIndex = (int)(((nZeros) / 3));

        if (nZeros < 3)
            return num.ToString("0");
        else if (prefixIndex > 19)
            prefixIndex = 19;

        string prefix = CountUnit[prefixIndex];
        double number = num / (System.Math.Pow(10, ((prefixIndex) * 3)));
        string returnvalue = (System.Math.Truncate(number * 100) / 100).ToString();
        returnvalue += prefix;
        return returnvalue;
    }

    IEnumerator DelayResult(int num, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        Continue(num);
    }

    public void SetDelayResult(int num, float time)
    {
        StartCoroutine(DelayResult(num, time));
    }

    void ShootingUI()   // 슈팅모드 전용 UI 셋팅
    {
        topWall[0].SetActive(DataManager.instance.CurGameMode != (int)GameMode.SHOOTING);
        topWall[1].SetActive(DataManager.instance.CurGameMode == (int)GameMode.SHOOTING);
        bundle_Bg[0].SetActive(DataManager.instance.CurGameMode != (int)GameMode.SHOOTING);
        bundle_Bg[1].SetActive(DataManager.instance.CurGameMode == (int)GameMode.SHOOTING);
    }

    public void GetPanel(UIPanel p)
    {
        brickPanel = p;
    }

    public void BrickPanelSetting()
    {
        if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
        {
            brickPanel.clipping = UIDrawCall.Clipping.SoftClip;
            brickPanel.clipSoftness = new Vector2(4, 4);
            brickPanel.baseClipRegion = new Vector4(0, -125, 1130, 1300);
        }
        else
        {
            brickPanel.clipping = UIDrawCall.Clipping.None;
        }
    }

    public void SetTouchSprite(bool b)
    {
        touchSprite.enabled = b;
    }

    public void SetContinueBtn()
    {
        continueObj1[0].SetActive(continueTodayAD || !AdsManager.instance.isRewardLoad(adsType.continueAD));
        continueObj1[1].SetActive(!continueTodayAD && AdsManager.instance.isRewardLoad(adsType.continueAD));
    }
}
