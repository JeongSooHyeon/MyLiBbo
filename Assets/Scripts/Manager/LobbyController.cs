using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class LobbyController : MonoBehaviour
{
    public static LobbyController instance;
    public List<LobbyLabel> lobbyLabel;
    public List<LobbySprite> lobbySprite;
    [SerializeField] UIState uiState = UIState.InGame;
    public List<UIState> addState;
    [SerializeField] TweenbundleManager tweens_;
    [SerializeField] CommonPupControl[] noticePup;
    [SerializeField] AdsMessagePupControl adsPup;
    [SerializeField] ShootingEditControl editPup;
    [SerializeField] GameObject fakeBannerObj;
    bool isStartIngame = false;

    // 게임 세트 세 가지
    public GameObject[] brickManagers;
    // 게임 세트에 맞춰서 하단 종료 판정 컬라이더 위치 수정을 위한
    [SerializeField] Transform bottomWallTransform;
    // 게임 시작 애니메이션 조정을 위한

    bool adsPupOn = false;
    bool isAD;
    public bool isSave;
    public bool isResult;
    public int noticeNum = 0;

    // 슈팅모드 전용
    public float timer = 0;
    public float tweenTime;
    public bool editMode;

    void Awake()
    {
        NoData();
        instance = this;
#if UNITY_IOS
          if (Device.generation >= DeviceGeneration.iPhoneX)
                myTopAnchor.relativeOffset = myTopPosArr[1];
            else
                myTopAnchor.relativeOffset = myTopPosArr[0];
#endif
    }

    void Start()
    {
        if ((GameMode)DataManager.instance.CurGameMode == GameMode.STAGEBOSS) SoundManager.instance.ChangeBGM(2);
        else SoundManager.instance.ChangeBGM(1);
        Invoke("LateStart", 0.1f);
        Invoke("PauseNotWorking", 1.3f);
        timer = 0;
        tweenTime = 5f;
        isSave = false;
        isResult = false;
        if (DataManager.instance.CurGameStage % 10 != 0 && DataManager.instance.CurGameMode == (int)GameMode.STAGE) 
            DataManager.instance.curMapDataList.MyGM = GameMode.STAGE;
        else if (DataManager.instance.CurGameMode != (int)GameMode.STAGE)
            DataManager.instance.curMapDataList.MyGM = GameMode.STAGE;
    }

    void LateStart()
    {
        if (string.IsNullOrEmpty(DataManager.instance.GetBall(BallManager.instance.ballSpriteCnt)))
        {
            DataManager.instance.SetBall(0);
        }
        SetLabel(UIState.BestScore, string.Format("{0:N0}", DataManager.instance.GetBestScore()));
        SetLabel(UIState.Coin, string.Format("{0:N0}", DataManager.instance.GetCoin()));
        if (uiState != UIState.Tutorial2 && uiState != UIState.ShootingTuto) StartGame();
        if (AdsManager.instance.isBannerLoad)
        {
            AdsManager.instance.BannerEnable();
        }
        AdsManager.instance.NativeEnable(false);
        fakeBannerObj.SetActive(!AdsManager.instance.isBannerLoad);

        switch ((GameMode)DataManager.instance.CurGameMode)
        {
            case GameMode.STAGE:
                int curStageRow = DataManager.instance.curMapDataList.curRow;

                // 맵 데이터 길에 맞춰 게임 세트 활성화
                if (curStageRow == 15)
                {
                    DataManager.instance.stageSetIndex = 1;
                }
                else if (curStageRow == 21)
                {
                    bottomWallTransform.localPosition = new Vector3(0, -430, 0);
                    DataManager.instance.stageSetIndex = 2;
                }
                else
                {
                    DataManager.instance.stageSetIndex = 0;
                }

                brickManagers[DataManager.instance.stageSetIndex].SetActive(true);
                DataManager.instance.stagePosCorrection = DataManager.instance.correctionValue[DataManager.instance.stageSetIndex];
                break;
            case GameMode.STAGEBOSS:
            case GameMode.BALL100:
            case GameMode.CLASSIC:
                DataManager.instance.stagePosCorrection = 0;
                brickManagers[0].SetActive(true);
                break;
            case GameMode.SHOOTING:
                DataManager.instance.stageSetIndex = 1;
                brickManagers[DataManager.instance.stageSetIndex].SetActive(true);
                DataManager.instance.stagePosCorrection = DataManager.instance.correctionValue[DataManager.instance.stageSetIndex];
                break;
            default:
                break;
        }

        BallManager.instance.SetFirstBall();
    }

    void PauseNotWorking()
    {
        isStartIngame = true;
    }

    void FixedUpdate()
    {
        if (uiState != UIState.Pause)
        {
            fakeBannerObj.SetActive(!AdsManager.instance.isBannerLoad);
            AdsManager.instance.NativeEnable(false);
        }

    }
    float saveTimeScale = 0;
    public void SetTween(UIState state_, bool isBool)
    {
        if (state_ == UIState.Pause && !isStartIngame) return;

        if (state_ == UIState.ResultSuc && isBool && addState.Count != 0)
        {
            int c = addState.Count;
            for (int i = 0; i < c; ++i)
            {
                SetTween(uiState, false);
            }
        }

        if (state_ == UIState.Pause && isStartIngame)
        {
            if (isBool)
            {
                saveTimeScale = Time.timeScale;
                Time.timeScale = 0;
                for (int i = 0; i < BrickManager.instance.slideBrickTP.Count; ++i)
                {
                    BrickManager.instance.slideBrickTP[i].enabled = false;
                }
            }
            else
            {
                Time.timeScale = saveTimeScale;
                for (int i = 0; i < BrickManager.instance.slideBrickTP.Count; ++i)
                {
                    BrickManager.instance.slideBrickTP[i].enabled = true;
                }
            }

            if (DataManager.instance.GetSceneType() == SceneType.Ingame)
            {
                UIManager.instance.NativeAdPop(isBool);
            }
        }
        if (isBool)
        {
            addState.Add(uiState);
            uiState = state_;
        }
        else
        {
            uiState = addState[addState.Count - 1];
            addState.Remove(uiState);
        }
        tweens_.ShowBundleObject(state_, isBool);
    }

    public void SetLabel(UIState state_, string text, bool isShow = true)
    {
        for (int i = 0; i < lobbyLabel.Count; ++i)
        {
            if (lobbyLabel[i].state_ == state_)
            {
                lobbyLabel[i].SetLabel(text, isShow);
            }
        }
    }

    public void SetSprite(UIState state_, string str, bool isShow = true)
    {
        for (int i = 0; i < lobbySprite.Count; ++i)
        {
            if (lobbySprite[i].state_ == state_)
            {
                lobbySprite[i].SetSprite(str, isShow);
            }
        }
    }

    public void StartGame()
    {
        uiState = UIState.InGame;
        if (BallManager.instance.isDelay) BallManager.instance.isDelay = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //if (BallManager.instance.isDelay) return;
            if (adsPupOn)
            {
                TurnAdsPup();
                adsPup.ClosePupTPTS();
            }
            else
            {
                switch (noticePup[noticeNum].curState)
                {
                    case CommonState.none:
                        switch (uiState)
                        {
                            case UIState.InGame:
                                if (!isResult) SetTween(UIState.Pause, true);
                                break;
                            case UIState.Tutorial2:
                                SetTween(UIState.Tutorial2, false);
                                break;
                            case UIState.ReStartGame:
                                UIManager.instance.FalseContinue();
                                break;
                            case UIState.ResultFailed:
                            case UIState.ResultSuc:
                            case UIState.BestScore:
                                if (BrickManager.instance.isStage && !isSave) DataManager.instance.SaveGameData();
                                PlayerPrefsElite.SetInt("ResultBtnState", 0);
                                isAD = DataManager.instance.showInterstitialAds();
                                //-------------------test-----------------------
                                if (!isAD)
                                {
                                    if (BrickManager.instance.isStage && !isSave) DataManager.instance.SaveGameData();
                                    UIManager.instance.ResetGame();
                                }
                                break;
                            case UIState.ShootingTuto:
                                break;
                            case UIState.ShootingEdit:
                                editPup.ClosePupTPTS();
                                break;
                            default:
                                SetBack();
                                break;
                        }
                        break;
                    case CommonState.buyPackage:
                    case CommonState.GetCoinAds:
                        noticePup[noticeNum].NoBtnClick();
                        break;
                    default:
                        noticePup[noticeNum].YesBtnClick();
                        break;
                }
            }
        }

        if (uiState == UIState.InGame)
            timer += Time.deltaTime;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {

        }
        else
        {
            if (AdsManager.instance.isAdsPlay) return;

            if (uiState != UIState.Pause && uiState != UIState.Tutorial2 && uiState != UIState.Tutorial && !UIManager.instance.isResult && uiState != UIState.ShootingTuto)
            {
                SetTween(UIState.Pause, true);
                BallManager.instance.CancleFire();
            }
        }
    }

    public void SetBack()
    {
        if (uiState == UIState.Pause)
        {
            AdsManager.instance.BannerEnable(true);
            AdsManager.instance.NativeEnable(false);
        }
        SetTween(uiState, false);
    }

    public void SetExit()
    {
        Application.Quit();
    }

    public UIState getGameState()
    {
        return uiState;
    }

    public void TurnAdsPup()
    {
        adsPupOn = !adsPupOn;
    }

    public void NoData()
    {
        if (DataManager.instance == null)
            SceneManager.LoadSceneAsync((int)SceneType.Preload);
    }

    public void SetPup(UIState ui)
    {
        uiState = ui;
    }
}
