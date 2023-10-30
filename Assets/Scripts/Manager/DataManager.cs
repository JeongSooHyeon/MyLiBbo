using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using CompleteProject;
//using Firebase;

[System.Serializable]
public class PackageItem
{
    public int cId;//캐릭터
    public int jewelCount;//충전되는 코인 개수
    public int cJewelCount; //캐릭터 대체 코인 개 수
    public int InstanceItem_1; //인게임 아이템 3줄 삭제
    public int InstanceItem_2; //인게임 아이템 더블볼
    //public int InstanceItem_3; //인게임 아이템 undo
}
[System.Serializable]
public class IngameRewardData
{
    public int rewardLevel;
    public int rewardItemScore_1; // 보상 획득 가능 점수
    public int rewardItemCount_1; //보상 개수
    public int rewardItemScore_2; // 보상 획득 가능 점수
    public int rewardItemCount_2; //보상 개수
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    //public Purchaser purchaser_;

    [SerializeField] int[] upgradeStartBallCoins;
    [SerializeField] int[] upgradeFireBallCoins;
    [SerializeField] TextAsset IngameData;
    [SerializeField] Purchaser myPurchaser;
    //public List<int> sBall100AchieveScoreList;

    // 새로운 맵 데이터 v2
    public MapDataList curMapDataList;
    TextAsset mapText;


    // 브릭 컬러 데이터
    public List<BlockColorItem> blockColorLists;

    public List<int> curStageStarScoreList;
    public int maxStageBall;
    public int BossHPCount;
    public StageBossType curBossType;
    public float curBossSpeed;
    public const int maxStageNum = 1000;
    public const int Ball100MaxIdx = 31;
    public const int maxShopMonster = 18;
    public const int costumeMaxIdx = 24;
    public const int maxShopBall = 25;
    public const int maxShopNum = 24;
    public const int BossTermNum = 10;
    public const int housingItem = 10;
    public const int housingCount = 5;
    public List<int> stageStarList;
    public List<int> stageScoreList;
    public List<int> stageLockList;
    public List<int> stageChapterStateList;
    public List<int> charBallList;
    public List<int> charBallPreViewList;
    public List<int> instanceItemList;
    //public List<int> ShopBallPriceList;
    public List<int> ChargeJewelList;
    public List<PackageItem> ChargeJewelPackageList;
    public List<IngameRewardData> IngameRewardList;
    [SerializeField] TextAsset fireMsg;
    public List<string> FireBaseMsgList;

    public List<int> HousingStateList;

    // 스테이지 모드 브릭 셋 위치값 조정을 위한 변수
    public int stagePosCorrection = 0;
    public readonly int[] correctionValue = { 0, 220, 455 };

    public int stageSetIndex = 0;

    public bool isGameClear;
    public bool isTestMode;
    public bool isFirstLoginLobby;

    string Path = "NewMapData/";

    string colorPath = "BlockColorData";

#if UNITY_ANDROID
    public const string TinyGolfURL = "market://details?id=com.Deliciousgames.TinyGolf";
    public const string appURL = "market://details?id=com.Deliciousgames.BbosiraegiBricksBreaker";
#elif UNITY_IOS
    public const string TinyGolfURL = "https://itunes.apple.com/app/id1547420260";
    public const string appURL = "https://itunes.apple.com/app/id1552350114";
#endif
    readonly int LobbyRewardTerm = 100;
    // 현재 로컬라이징 상태 확인 변수
    public Language languageState;

    public GameData myCloudData;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        //화면 꺼짐 방지
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        // 자동 로컬라이징
        SetLocalization();
    }

    void Start()
    {
        if (!PlayerPrefsElite.GetBoolean("firstLogin"))
        {
            PlayerPrefs.DeleteAll();
            StartBall = 1;
            FirePower = 1;
            SetCoin(300);
        }
        if (string.IsNullOrEmpty(FirstLoginTime))
        {
            FirstLoginTime = UnbiasedTime.Instance.Now().ToString();
        }
        else
        {
            System.DateTime dt = System.DateTime.Parse(FirstLoginTime);
            System.DateTime dt2 = UnbiasedTime.Instance.Now();
            if (!dt.Day.Equals(dt2.Day))
            {
                FirstLoginTime = UnbiasedTime.Instance.Now().ToString();
                PlayerPrefs.DeleteKey("shopMonsterPreViewList");
                PlayerPrefs.DeleteKey("ballPreViewList");
                ResetPreviewCharBall();
            }
        }
        InitGameData();
        initFireBaseMsg();
        SetBrickColorData();
        //FirebaseApp app = FirebaseApp.DefaultInstance;
        //Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
        //    var dependencyStatus = task.Result;
        //    if (dependencyStatus == Firebase.DependencyStatus.Available)
        //    {
        //        // Create and hold a reference to your FirebaseApp,
        //        // where app is a Firebase.FirebaseApp property of your application class.
        //        app = Firebase.FirebaseApp.DefaultInstance;

        //        // Set a flag here to indicate whether Firebase is ready to use by your app.
        //    }
        //    else
        //    {
        //        UnityEngine.Debug.LogError(System.String.Format(
        //          "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
        //        // Firebase Unity SDK is not safe to use here.
        //    }
        //});
    }

    public void GameResetReward()
    {
        PlayerPrefs.DeleteKey("FirstLoginTime");
        PlayerPrefs.DeleteKey("StageStar");
        PlayerPrefs.DeleteKey("StageScore");
        PlayerPrefs.DeleteKey("StageClear");
        PlayerPrefs.DeleteKey("stageChapterState");
        PlayerPrefs.DeleteKey("ShopBallPreViewList");
        PlayerPrefs.DeleteKey("BallSprite");
        PlayerPrefs.DeleteKey("PreBallSprite");

        if (myCloudData != null)
        {
            if (myCloudData.coin > 0 && GetCoin() == 0)
                PlayerPrefsElite.SetInt("Money", myCloudData.coin);
        }

        SetCoin(3000);
        CurGameStage = 1;
        if (string.IsNullOrEmpty(FirstLoginTime))
        {
            FirstLoginTime = UnbiasedTime.Instance.Now().ToString();
        }
        else
        {
            System.DateTime dt = System.DateTime.Parse(FirstLoginTime);
            System.DateTime dt2 = UnbiasedTime.Instance.Now();
            if (!dt.Day.Equals(dt2.Day))
            {
                FirstLoginTime = UnbiasedTime.Instance.Now().ToString();
                PlayerPrefs.DeleteKey("shopMonsterPreViewList");
                PlayerPrefs.DeleteKey("ballPreViewList");
                ResetPreviewCharBall();
            }
        }
        InitGameData();

        if (PlayerPrefs.HasKey("StartTutorial"))
        {
            PlayerPrefs.DeleteKey("StartTutorial");
        }

        initFireBaseMsg();
        SetBrickColorData();
        SceneLoad();
        SetFirebaseAnalyticsStageClearLog(FirebaseType.new_user, 0);
    }

    void InitGameData()
    {
        if (IngameRewardList == null)
            IngameRewardList = new List<IngameRewardData>();
        else
            IngameRewardList.Clear();

        if (IngameData == null)
            IngameData = Resources.Load<TextAsset>("IngameRewardData");

        string[] lines = IngameData.text.Split('\n');
        if (lines.Length == 0)
            Debug.Log("text data is nothing!!");
        else
        {
            for (int i = 0; i < lines.Length; ++i)
            {
                string[] txtD = lines[i].Split(',');
                IngameRewardData item = SetIngameDataItem(txtD);
                if (item != null)
                {
                    IngameRewardList.Add(item);
                }
            }
        }
        CallStageData();
        charBallList = initUserLocalData("charBallList", maxShopNum, 1, 0, 0);
        charBallPreViewList = initUserLocalData("charBallPreViewList", maxShopNum, 1, 0, 0);
        if (charBallList.Count > maxShopNum)
        {
            PlayerPrefs.DeleteKey("charBallList");
            PlayerPrefs.DeleteKey("charBallPreViewList");
            charBallList = initUserLocalData("charBallList", maxShopNum, 1, 0, 0);
            charBallPreViewList = initUserLocalData("charBallPreViewList", maxShopNum, 1, 0, 0);
        }

        instanceItemList = initUserLocalData("InstanceItemList", 3, 0, 0, 0);
        if (!PlayerPrefsElite.GetBoolean("firstLogin"))
        {
            PlayerPrefsElite.SetBoolean("firstLogin", true);
            SetInstanceItemList(InstanceItem.AllBlockDamage, 3);
            SetInstanceItemList(InstanceItem.DoubleBall, 3);
            SetInstanceItemList(InstanceItem.Undo, 3);
        }

        HousingStateList = initUserLocalData("HousingStateList", 5, 0, -1, 0);

        for (int i = 0; i < HousingStateList.Count; ++i)
        {
            if (GetHousingStateList(i) == 11 && HousingStateCount(i) < 10)
            {
                if (GetHousingStateList(i) > HousingStateCount(i))
                    SetHousingStateList(i, HousingStateCount(i));
            }
        }
    }

    void initFireBaseMsg()
    {
        string[] lines = fireMsg.text.Split('\n');
        if (lines.Length == 0)
            Debug.Log("text data is nothing!!");
        else
        {
            for (int i = 0; i < lines.Length; ++i)
            {
                FireBaseMsgList.Add(lines[i]);
            }
        }
    }

    public void CallStageData()
    {
        //테스트 코드입니다~!!!!!! 190730```````````````````````````````````````````````````````````
        if (isTestMode)
        {
            if (CurGameStage == 0)
                CurGameStage = maxStageNum;

            stageStarList = initUserLocalData("StageStar", maxStageNum, 3, 3, 0);
            stageScoreList = initUserLocalData("StageScore", maxStageNum, 1, 1, 0);
            stageLockList = initUserLocalData("StageClear", maxStageNum, 2, 2, 0);
            stageChapterStateList = initUserLocalData("stageChapterState", maxStageNum / 20, 1, 1, 0);
            //HousingStateList = initUserLocalData("HousingStateList", 5, 10, 10, 0);
            charBallList = initUserLocalData("charBallList", maxShopNum, 0, 0, 0);
            charBallPreViewList = initUserLocalData("charBallPreViewList", maxShopNum, 0, 0, 0);
            charBallList[0] = 1;
        }
        else
        {
            stageStarList = initUserLocalData("StageStar", maxStageNum, 0, 0, 0);
            stageScoreList = initUserLocalData("StageScore", maxStageNum, 0, 0, 0);
            stageLockList = initUserLocalData("StageClear", maxStageNum, 0, 0, 0);
            stageChapterStateList = initUserLocalData("stageChapterState", maxStageNum / 20, 0, -1, 0);
            HousingStateList = initUserLocalData("HousingStateList", 5, 0, -1, 0);
        }
    }

    IngameRewardData SetIngameDataItem(string[] data)
    {
        IngameRewardData ret = new IngameRewardData();

        int.TryParse(data[0], out ret.rewardLevel);
        int.TryParse(data[1], out ret.rewardItemScore_1);
        int.TryParse(data[2], out ret.rewardItemCount_1);
        int.TryParse(data[3], out ret.rewardItemScore_2);
        int.TryParse(data[4], out ret.rewardItemCount_2);

        return ret;
    }

    public void SceneLoad(SceneType scenenum = SceneType.Lobby)
    {
        SceneManager.LoadSceneAsync((int)scenenum);
    }

    public SceneType GetSceneType()
    {
        return (SceneType)SceneManager.GetActiveScene().buildIndex;
    }

    public void GameStartCall(int gMode = 0)
    {
        maxStageBall = 0;
        CurGameMode = gMode;
        BossHPCount = 0;
        curBossType = StageBossType.normal;
        curBossSpeed = 0;
        isGameClear = false;
        if (CurGameMode == (int)GameMode.STAGE || CurGameMode == (int)GameMode.BALL100)
        {
            SetCurSelectMap();
        }
        SetStageBallCount();

        if (CurGameMode == (int)GameMode.STAGE)
        {
            // 현재 스테이지 별 점수 세팅
            SetStageStarScore();
            // 보스 체력 세팅
            SetBossCount();
        }

        CheckIngameReward();
        SceneLoad(SceneType.Ingame);
        AdsManager.instance.RequestinterstitialAd();
    }

    // 스테이지 모드에서 게임 모드 알아내기 - 거의 보스 전용
    public GameMode GetCurStageMode()
    {
        switch (CurGameMode)
        {
            case 0:
            case 1:
            case 2:
                return curMapDataList.MyGM;
            default:
                return GameMode.CLASSIC;
        }
    }

    public void ResetPreviewCharBall()
    {
        PreBallSprite = 0;
        for (int i = 0; i < charBallList.Count; ++i)
        {
            if (charBallList[i] == 3)
                SetCharBallList(i, 0);
        }
    }

    public void SetCoin(int i)
    {
        PlayerPrefsElite.SetInt("Money", GetCoin() + i);
    }

    public int GetCoin()
    {
        if (PlayerPrefs.HasKey("Money"))
            return PlayerPrefsElite.GetInt("Money");
        else
            return 0;
    }

    public void SetBestScore()
    {
        switch (CurGameMode)
        {
            case (int)GameMode.BALL100:
                PlayerPrefsElite.SetInt("Ball100Score", CurrentScore);
                if (GetBestScore(2) >= 10000)
                {
                    GPGSManager.instance.setAchievements(1);
                }
                break;
            case (int)GameMode.CLASSIC:
                PlayerPrefsElite.SetInt("ClassicScore", CurrentScore);
                GPGSManager.instance.setScoreLeaderBoard(0, GetBestScore(3));
                if (GetBestScore(3) >= 5000)
                {
                    GPGSManager.instance.setAchievements(0);
                }
                break;
            case (int)GameMode.STAGE:
            case (int)GameMode.STAGEBOSS:
                PlayerPrefsElite.SetInt("Score", CurrentScore);
                break;
            case (int)GameMode.SHOOTING:
                PlayerPrefsElite.SetInt("ShootingScore", CurrentScore);
                GPGSManager.instance.setScoreLeaderBoard(3, GetBestScore(4)); 
                break;
        }
    }

    public int GetBestScore(int gIdx = -1)
    {
        gIdx = gIdx < 0 ? CurGameMode : gIdx;

        switch (gIdx)
        {
            case (int)GameMode.BALL100:
                return PlayerPrefsElite.GetInt("Ball100Score");
            case (int)GameMode.CLASSIC:
                return PlayerPrefsElite.GetInt("ClassicScore");
            case (int)GameMode.STAGE:
            case (int)GameMode.STAGEBOSS:
                return PlayerPrefsElite.GetInt("Score");
            case (int)GameMode.SHOOTING:
                return PlayerPrefsElite.GetInt("ShootingScore");
        }

        return -1;
    }

    public int IngameRewardLevel
    {
        get
        {
            if (PlayerPrefs.HasKey("IngameRewardLevel"))
                return PlayerPrefsElite.GetInt("IngameRewardLevel");
            else
            {
                IngameRewardLevel = 1;
                return 1;
            }
        }
        set { PlayerPrefsElite.SetInt("IngameRewardLevel", value); }
    }

    public int GetLobbyRewardCount()
    {
        return ((LobbyRewardLevel - 1) * 50) + LobbyRewardTerm;
    }

    public int LobbyRewardLevel
    {
        get
        {
            if (PlayerPrefs.HasKey("LobbyRewardLevel"))
                return PlayerPrefsElite.GetInt("LobbyRewardLevel");
            else
            {
                LobbyRewardLevel = 1;
                return 1;
            }
        }
        set { PlayerPrefsElite.SetInt("LobbyRewardLevel", value); }
    }

    public void SetIngameRewardScore(int data, bool reset = false)
    {
        if (reset)
            PlayerPrefsElite.SetInt("IngameRewardScore", data);
        else
            PlayerPrefsElite.SetInt("IngameRewardScore", GetIngameRewardScore() + data);

    }

    public int GetIngameRewardScore()
    {
        return PlayerPrefsElite.GetInt("IngameRewardScore");
    }

    public string FirstLoginTime
    {
        get { return PlayerPrefsElite.GetString("FirstLoginTime"); }
        set { PlayerPrefsElite.SetString("FirstLoginTime", value); }
    }

    public int CurrentScore
    {
        get { return PlayerPrefsElite.GetInt("CScore"); }
        set { PlayerPrefsElite.SetInt("CScore", value); }
    }

    public bool AutoSave
    {
        get { return PlayerPrefsElite.GetBoolean("AutoSave"); }
        set { PlayerPrefsElite.SetBoolean("AutoSave", value); }
    }

    public bool LobbyTutorial
    {
        get { return PlayerPrefsElite.GetBoolean("LobbyTutorial"); }
        set { PlayerPrefsElite.SetBoolean("LobbyTutorial", value); }
    }
    public int LobbyTutoStep
    {
        get { return PlayerPrefsElite.GetInt("LobbyTutoStep"); }
        set { PlayerPrefsElite.SetInt("LobbyTutoStep", value); }
    }

    public void SetHiddenBall()
    {
        PlayerPrefsElite.SetBoolean("HiddenBall", true);
    }

    public bool GetHiddenBall()
    {
        return PlayerPrefsElite.GetBoolean("HiddenBall");
    }

    public void SetGetCoinAdsTime(string data)
    {
        PlayerPrefsElite.SetString("GetCoinAdsTime", data);
    }

    public string GetGetCoinAdsTime()
    {
        return PlayerPrefsElite.GetString("GetCoinAdsTime");
    }

    public void SetGetCoinGiftTime(string data)
    {
        PlayerPrefsElite.SetString("GetCoinGiftTime", data);
    }

    public string GetGetCoinGiftTime()
    {
        return PlayerPrefsElite.GetString("GetCoinGiftTime");
    }

    public bool GetLobbyTutorial()
    {
        return PlayerPrefsElite.GetBoolean("LobbyTutorial");
    }

    public bool ShootingTuto
    {
        get { return PlayerPrefsElite.GetBoolean("ShootingTuto"); }
        set { PlayerPrefsElite.SetBoolean("ShootingTuto", value); }
    }

    //----------------------------게임 저장 관련----------------------------------------
    /// <summary>
    /// 이거는 Save파일을 설정하는 함수입니다.
    /// </summary>
    public void SetSaveFile(bool b)
    {
        PlayerPrefsElite.SetBoolean("SaveFile", b);
    }
    /// <summary>
    /// 이거는 Save파일이 있는지 확인하는 함수입니다. ture면 있는 겁니다.
    /// </summary>
    public bool GetSaveFile()
    {
        return PlayerPrefsElite.GetBoolean("SaveFile");
    }

    public void SetBrickPos(int i, Vector2 pos)
    {
        PlayerPrefsElite.SetVector2("BlockPos" + i, pos);
    }

    public Vector2 GetBrickPos(int i)
    {
        return PlayerPrefsElite.GetVector2("BlockPos" + i);
    }

    public void SetSaveBlock(List<int> blocks, int i)
    {
        PlayerPrefsElite.SetIntArray("blocks" + i, blocks.ToArray());
    }

    public List<int> GetSaveBlock(int i)
    {
        List<int> list_ = new List<int>(PlayerPrefsElite.GetIntArray("blocks" + i));
        return list_;
    }

    public void SetSaveItems(List<int> Items, int i)
    {
        PlayerPrefsElite.SetIntArray("Items" + i, Items.ToArray());
    }

    public List<int> GetSaveItems(int i)
    {
        List<int> list_ = new List<int>(PlayerPrefsElite.GetIntArray("Items" + i));
        return list_;
    }

    public void SetSaveSpecial(List<int> Items, int i)
    {
        PlayerPrefsElite.SetIntArray("Specials" + i, Items.ToArray());
    }

    public List<int> GetSaveSpecial(int i)
    {
        List<int> list_ = new List<int>(PlayerPrefsElite.GetIntArray("Specials" + i));
        return list_;
    }

    public void SetStageStarList(int i, int data)
    {
        if (stageStarList[i] < data)
        {
            stageStarList[i] = data;
            PlayerPrefsElite.SetIntArray("StageStar", stageStarList.ToArray());
        }
    }

    public int GetStageStarList(int i)
    {
        return stageStarList[i];
    }

    public void SetStageScoreList(int i, int data)
    {
        if (stageScoreList[i] < data)
        {
            stageScoreList[i] = data;
            PlayerPrefsElite.SetIntArray("StageScore", stageScoreList.ToArray());
        }
    }

    public int GetStageScoreList(int i)
    {
        return stageScoreList[i];
    }

    public void SetStageClearList(int i, int data)
    {
        stageLockList[i] = data;
        PlayerPrefsElite.SetIntArray("StageClear", stageLockList.ToArray());
    }

    public bool GetStageClearList(int i)
    {
        return (stageLockList[i] >= 1);
    }

    public void SetChapterStateList(int i, int data)
    {
        stageChapterStateList[i] = data;
        PlayerPrefsElite.SetIntArray("stageChapterState", stageChapterStateList.ToArray());
    }

    public StageStarState GetChapterStateList(int i)
    {
        return (StageStarState)stageChapterStateList[i];
    }

    public int GetCurrentChapterIdx()
    {
        int idx;
        if (stageChapterStateList.Count(t => t == 0) == 0) idx = 0;
        else idx = (stageChapterStateList.FindLastIndex(t => t == 0) % 10);
        return idx;
    }

    public int GetCharBallList(int i)
    {
        return charBallList[i];
    }

    public void SetCharBallList(int i, int data)
    {
        charBallList[i] = data;
        PlayerPrefsElite.SetIntArray("charBallList", charBallList.ToArray());
    }

    public int GetCharBallPreViewList(int i)
    {
        return charBallPreViewList[i];
    }

    public void SetCharBallPreviewList(int i, int data)
    {
        charBallPreViewList[i] = data;
        PlayerPrefsElite.SetIntArray("charBallPreViewList", charBallPreViewList.ToArray());
    }

    public void ClearPreviewBallList()
    {
        int idx = charBallList.FindIndex(t => t == 3);
        if (idx == -1) return;
        charBallList[idx] = 0;
        PlayerPrefsElite.SetIntArray("charBallList", charBallList.ToArray());
    }

    public void SetInstanceItemList(InstanceItem i, int data)
    {
        instanceItemList[(int)i] += data;
        PlayerPrefsElite.SetIntArray("InstanceItemList", instanceItemList.ToArray());
    }

    public int GetInstanceItemList(InstanceItem i)
    {
        return instanceItemList[(int)i];
    }

    public float Ballpos
    {
        get { return PlayerPrefsElite.GetFloat("BallPos"); }
        set { PlayerPrefsElite.SetFloat("BallPos", value); }
    }

    public bool SweetAimUse
    {
        get { return PlayerPrefsElite.GetBoolean("SweetAimUse"); }
        set { PlayerPrefsElite.SetBoolean("SweetAimUse", value); }
    }

    public bool NoAdsUse
    {
        get { return PlayerPrefsElite.GetBoolean("NoAdsUse"); }
        set { PlayerPrefsElite.SetBoolean("NoAdsUse", value); }
    }

    public bool SweetAimPreUse
    {
        get { return PlayerPrefsElite.GetBoolean("SweetAimPreUse"); }
        set { PlayerPrefsElite.SetBoolean("SweetAimPreUse", value); }
    }

    public bool LobbyTipView
    {
        get { return PlayerPrefsElite.GetBoolean("LobbyTipView"); }
        set { PlayerPrefsElite.SetBoolean("LobbyTipView", value); }
    }

    public void SetSaveBrickCount(int i)
    {
        PlayerPrefsElite.SetInt("SaveBrickCnt", i);
    }

    public int GetSaveBrickCount()
    {
        return PlayerPrefsElite.GetInt("SaveBrickCnt");
    }

    public void SetSaveBallCount(int i)
    {
        PlayerPrefsElite.SetInt("SaveBallCnt", i);
    }

    public int GetSaveBallCount()
    {
        return PlayerPrefsElite.GetInt("SaveBallCnt");
    }

    public void SetSaveBrickPosCnt(int i)
    {
        PlayerPrefsElite.SetInt("PosCnt", i);
    }

    public int GetSaveBrickPosCnt()
    {
        return PlayerPrefsElite.GetInt("PosCnt");
    }

    public void SetSaveBrickBool(bool b)
    {
        PlayerPrefsElite.SetBoolean("PosBool", b);
    }

    public bool GetSaveBrickBool()
    {
        return PlayerPrefsElite.GetBoolean("PosBool");
    }

    public void SetAchievement(int b)
    {
        PlayerPrefsElite.SetBoolean(string.Format("Achievement_{0}", b), true);
    }

    public bool GetAchievement(int b)
    {
        if (PlayerPrefs.HasKey(string.Format("Achievement_{0}", b)))
            return PlayerPrefsElite.GetBoolean(string.Format("Achievement_{0}", b));
        else
            return false;
    }

    public void SetOnemore(bool isBool)
    {
        PlayerPrefsElite.SetBoolean("SetOneMore", isBool);
    }

    public bool GetOneMoreBool()
    {
        return PlayerPrefsElite.GetBoolean("SetOneMore");
    }

    public void SetSaveOnemoreCnt(int i)
    {
        PlayerPrefsElite.SetInt("OneMoreCnt", i);
    }

    public int GetSaveOnemoreCnt()
    {
        return PlayerPrefsElite.GetInt("OneMoreCnt");
    }
    //----------------------------게임 저장 관련----------------------------------------

    public void SetBall(int i)
    {
        PlayerPrefsElite.SetInt("Ball", i);
    }

    public string GetBall(int cnt)
    {
        string ballStr_ = "";
        ballStr_ = CharBallDataManager.instance.CBList[cnt].spriteName;
        return ballStr_;
    }

    public int GetBallInt()
    {
        return PlayerPrefsElite.GetInt("Ball");
    }

    public void SetChar(int i)
    {
        PlayerPrefsElite.SetInt("Char", i);
    }

    public int GetChar()
    {
        return PlayerPrefsElite.GetInt("Char");
    }

    public int GetSaveMyChar(int i)
    {
        return GetSaveMyCharList()[i];
    }
    public List<int> GetSaveMyCharList()
    {
        List<int> list_ = new List<int>(PlayerPrefsElite.GetIntArray("MyCharList"));
        return list_;
    }

    public bool GetSkill()
    {
        return PlayerPrefsElite.GetBoolean("SkillBool");
    }

    public void SetSkill(bool isShow)
    {
        PlayerPrefsElite.SetBoolean("SkillBool", isShow);
    }

    public void SetSoundState(bool isBool)
    {
        PlayerPrefsElite.SetBoolean("Sound", isBool);
    }

    public bool GetSoundState()
    {
        return PlayerPrefsElite.GetBoolean("Sound");
    }

    public void SetGamePlayCount()
    {
        PlayerPrefsElite.SetInt("PlayCnt", GetGamePlayCount() + 1);
    }

    public int GetGamePlayCount()
    {
        return PlayerPrefsElite.GetInt("PlayCnt");
    }

    //---------------------------------------------------- 업그레이드 요소를 여기에 넣어준다.-------------------------------------------

    public int StartBall // 현재 시작할 때 볼을 뜻한다.
    {
        get { return PlayerPrefsElite.GetInt("StartBalls"); }
        set { PlayerPrefsElite.SetInt("StartBalls", value); }
    }
    public int FirePower// 현재 공에 맞으면 블럭이 몇이 깎이는지 보여준다.
    {
        get { return PlayerPrefsElite.GetInt("FirePowers"); }
        set { PlayerPrefsElite.SetInt("FirePowers", value); }
    }


    public int ReturenStartBallCoin
    {
        get { return upgradeStartBallCoins[StartBall - 1]; }
    }

    public int ReturenFireBallCoin
    {
        get { return upgradeFireBallCoins[FirePower - 1]; }
    }

    public int BallSprite
    {
        get { return PlayerPrefsElite.GetInt("BallSprite"); }
        set { PlayerPrefsElite.SetInt("BallSprite", value); }
    }

    public int PreBallSprite
    {
        get { return PlayerPrefsElite.GetInt("PreBallSprite"); }
        set { PlayerPrefsElite.SetInt("PreBallSprite", value); }
    }

    public int SelectMoster
    {
        get { return PlayerPrefsElite.GetInt("SelectMoster"); }
        set { PlayerPrefsElite.SetInt("SelectMoster", value); }
    }

    public int PreSelectMoster
    {
        get { return PlayerPrefsElite.GetInt("PreSelectMoster"); }
        set { PlayerPrefsElite.SetInt("PreSelectMoster", value); }
    }

    public int PlaneSprite
    {
        get { return PlayerPrefsElite.GetInt("PlaneSprite"); }
        set { PlayerPrefsElite.SetInt("PlaneSprite", value); }
    }

    public int CurGameMode
    {
        get { return PlayerPrefsElite.GetInt("GameMode"); }
        set { PlayerPrefsElite.SetInt("GameMode", value); }
    }

    public int CurGameStage
    {
        get
        {
            if (!PlayerPrefs.HasKey("CurGameStage"))
                return CurGameStage = 1;
            return PlayerPrefsElite.GetInt("CurGameStage");
        }
        set { PlayerPrefsElite.SetInt("CurGameStage", value); }
    }

    public int CurBallAchieveIdx
    {
        get
        {
            if (!PlayerPrefs.HasKey("BallAchieveIdx"))
                return CurBallAchieveIdx = 0;
            return PlayerPrefsElite.GetInt("BallAchieveIdx");
        }
        set { PlayerPrefsElite.SetInt("BallAchieveIdx", value); }
    }

    //100볼 누적 점수 
    public double Ball100GameScore
    {
        get
        {
            if (!PlayerPrefs.HasKey("Ball100GameScore"))
                return Ball100GameScore = 1;
            return double.Parse(PlayerPrefsElite.GetString("Ball100GameScore"));
        }
        set { PlayerPrefsElite.SetString("Ball100GameScore", value.ToString()); }
    }
    public void SetCurSelectMap()
    {
        curMapDataList = new MapDataList();

        string modePath = "Stage/";
        int ball100Stage = 0;
        switch (CurGameMode)
        {
            case (int)GameMode.STAGE:
                modePath = "Stage/";
                break;
            case (int)GameMode.BALL100:
                modePath = "Ball/";
                ball100Stage = Random.Range(1, Ball100MaxIdx);
                break;
            default:
                break;
        }

        if (CurGameStage < 1)
        {
            CurGameStage = 1;
        }

        if (CurGameMode == (int)GameMode.BALL100)
            mapText = Resources.Load(Path + modePath + ball100Stage) as TextAsset;
        else mapText = Resources.Load(Path + modePath + CurGameStage) as TextAsset;

        if (mapText != null)
        {
            curMapDataList = JsonToOject<MapDataList>(mapText.text);
        }
    }

    // 현재 맵의 브릭 데이터를 참조하기 위한 함수
    public BlockShow GetBlockShowData(int lineIndex, int brickIndex, bool isAdd = false)
    {
        BlockShow blockShow = new BlockShow();
        switch ((GameMode)CurGameMode)
        {
            case GameMode.STAGE:
                if (isAdd)
                {
                    blockShow = curMapDataList.AddBlockDatas[lineIndex * curMapDataList.curRow + brickIndex];
                }
                else
                {
                    blockShow = curMapDataList.myBlockDatas[lineIndex * curMapDataList.curRow + brickIndex];
                }
                return blockShow;

            case GameMode.BALL100:
                blockShow = curMapDataList.myBlockDatas[lineIndex * curMapDataList.curRow + brickIndex];
                return blockShow;
            default:
                return blockShow;
        }
    }

    // 현재 스테이지의 볼 개수를 세팅하기 위한 함수
    public void SetStageBallCount()
    {
        switch ((GameMode)CurGameMode)
        {
            case GameMode.STAGE:
                if (CurGameStage == 0) CurGameStage = 1;
                maxStageBall = curMapDataList.BallCount;
                break;
            case GameMode.BALL100:
                maxStageBall = 100;
                break;
            case GameMode.SHOOTING:
                maxStageBall = 10;
                break;
            default:
                break;
        }
    }

    // 현재 스테이지 별 점수를 세팅하기 위한 함수
    public void SetStageStarScore()
    {
        if (curStageStarScoreList == null)
            curStageStarScoreList = new List<int>();
        else
            curStageStarScoreList.Clear();

        for (int i = 0; i < curMapDataList.StartArr.Length; ++i)
        {
            curStageStarScoreList.Add(curMapDataList.StartArr[i]);
        }
    }

    // 보스 체력 세팅
    public void SetBossCount()
    {
        if (curMapDataList.MyGM == GameMode.STAGEBOSS)
        {
            BossHPCount = curMapDataList.BossHPCount;
        }
    }

    // 현재 스테이지의 ADD LINE 카운트
    public int GetCurStageAddLineCount()
    {
        return curMapDataList.AddBlockDatas.Count / curMapDataList.curRow;
    }

    public void SetBrickColorData()
    {
        if (blockColorLists != null)
        {
            blockColorLists.Clear();
        }
        else
        {
            blockColorLists = new List<BlockColorItem>();
        }

        BlockColorData colorData = Resources.Load(colorPath) as BlockColorData;

        for (int i = 0; i < colorData.BlockColorDatas.Count; ++i)
        {
            blockColorLists.Add(colorData.BlockColorDatas[i]);
        }
    }

    //스테이지 게임 종료시 다음 스테이지 클리어 체크 및 데이터 저장
    public void SaveGameData(bool isNext = false)
    {
        switch ((GameMode)CurGameMode)
        {
            case GameMode.STAGE:
            case GameMode.STAGEBOSS:
                if (isGameClear)
                {
                    if (curMapDataList.MyGM == GameMode.STAGEBOSS)
                    {
                        int idx = CurGameStage / 20;
                        if (idx < 50)
                        {
                            if (GetChapterStateList(idx) == StageStarState.none)
                                SetChapterStateList(idx, 0);
                        }
                    }

                    if (CurrentScore > 0)
                    {
                        int num = isGameClear ? 2 : 1;
                        SaveStageInfo(num, isNext);
                        if (CurGameStage < 1000) ++CurGameStage;
                    }
                    //else if (isGameClear && (GameMode)CurGameMode == GameMode.STAGEBOSS)
                    else if (isGameClear && curMapDataList.MyGM == GameMode.STAGEBOSS)
                    {
                        SaveStageInfo(2, isNext);
                    }
                }
                break;
            case GameMode.BALL100:
                Ball100GameScore = CurrentScore;
                // 달성에 따른 리워드 지급 처리
                break;
        }
        LobbyController.instance.isSave = true;
        if (isNext && AutoSave)
        {
            CallGameDataSave();
        }
    }

    void SaveStageInfo(int num, bool isNext)
    {
        int preTotalStar = GetTotalStarCount();
        SetStageClearList((CurGameStage - 1), num);
        SetStageScoreList((CurGameStage - 1), CurrentScore);

        if (curMapDataList.MyGM == GameMode.STAGEBOSS)
            SetStageStarList((CurGameStage - 1), 3);
        else
            SetStageStarList((CurGameStage - 1), GetStageStarCount());

        if (CurGameStage < maxStageNum)
        {
            if (GetStageStarList(CurGameStage) == 0)
            {
                SetStageStarList(CurGameStage, 0);
                SetStageClearList(CurGameStage, 1);
            }
        }
        if (!isNext) ++CurGameStage;
        int nowTotalStart = GetTotalStarCount();
        if (preTotalStar < nowTotalStart) GPGSManager.instance.setScoreLeaderBoard(2, nowTotalStart);
        if (nowTotalStart >= 500)
        {
            GPGSManager.instance.setAchievements(3);
            PlayerPrefsElite.SetBoolean("500Star", true);
        }
        TotalStarCount += GetStageStarList(CurGameStage - 1);
    }

    //스테이지 클리어 별 개수 체크
    public int GetStageStarCount(bool isResult = false)
    {
        int ret = 0;

        if (!isResult)
        {
            if (GetStageStarList(CurGameStage - 1) > 2)
                return ret = 3;
        }

        float num = CurrentScore > curStageStarScoreList[2] ? 1f : GetStageStarValue();
        if (num < 0.6f && num > 0f)
            ret = 1;
        if (num >= 0.6f && num < 1f)
            ret = 2;
        if (num == 1f)
            ret = 3;

        return ret;
    }

    public float GetStageStarValue()
    {
        return (float)CurrentScore / (float)curStageStarScoreList[2];
    }

    //다음 스테이지 바로 호출용 함수
    public void NextStagePlay()
    {
        Time.timeScale = 1;
        // 맵 데이터 세팅
        SetCurSelectMap();
        if (CurGameMode == (int)GameMode.STAGE)
        {
            // 현재 스테이지 별 점수 세팅
            SetStageStarScore();
            // 보스 체력 세팅
            SetBossCount();
        }
        SetStageBallCount();
        CheckIngameReward();
        SceneLoad(SceneType.Ingame);
    }

    public List<int> initUserLocalData(string pKey, int stanCount, int firstData, int otherData, int initDataType)
    {
        List<int> Arr = null;

        if (PlayerPrefs.HasKey(pKey))
        {
            Arr = new List<int>(PlayerPrefsElite.GetIntArray(pKey));
            if (Arr.Count < stanCount)
            {
                int sNum = stanCount - Arr.Count;
                for (int i = 0; i < sNum; ++i)
                {
                    Arr.Add(0);
                }
                PlayerPrefsElite.SetIntArray(pKey, Arr.ToArray());
            }
        }
        else
        {
            Arr = new List<int>();
            for (int i = 0; i < stanCount; ++i)
            {
                switch (initDataType)
                {
                    case 0:
                        if (i == 0) Arr.Add(firstData);
                        else Arr.Add(otherData);
                        break;
                    case 1:
                        if (i < 2) Arr.Add(firstData);
                        else Arr.Add(otherData);
                        break;
                    case 2:
                        Arr.Add(firstData);
                        break;
                }
            }
            PlayerPrefsElite.SetIntArray(pKey, Arr.ToArray());
        }

        return Arr;
    }

    public int TotalStarCount
    {
        get
        {
            return PlayerPrefsElite.GetInt("TotalStarCount");
        }
        set { PlayerPrefsElite.SetInt("TotalStarCount", value); }
    }

    public int GetTotalStarCount()
    {
        return stageStarList.Sum();
    }

    public int GetTotalCloudStarCount()
    {
        return myCloudData.stageStars.Sum();
    }

    public int GetCurPlayStage()
    {
        int ret = stageLockList.FindLastIndex(t => t == 1);

        if (ret == 0)
            ret = stageLockList.FindLastIndex(t => t == 2);
        if (ret == -1) ret = 0;
        return ret;
    }

    // 시스템 언어 파악해서 시작할 때마다 로컬라이징
    public void SetLocalization()
    {
        SystemLanguage systemLanguage = Application.systemLanguage;

        // 로컬라이징 할 수 있는 언어일 경우 - 한국어, 영어, 일본어, 중국어 간자 번자
        if (systemLanguage == SystemLanguage.Korean || systemLanguage == SystemLanguage.English || systemLanguage == SystemLanguage.Japanese ||
            systemLanguage == SystemLanguage.ChineseSimplified || systemLanguage == SystemLanguage.ChineseTraditional)
        {
            I2.Loc.LocalizationManager.CurrentLanguage = systemLanguage.ToString();
        }
        // 디폴트
        else
        {
            I2.Loc.LocalizationManager.CurrentLanguage = "English";
        }

        switch (I2.Loc.LocalizationManager.CurrentLanguage)
        {
            case "Korean":
                languageState = Language.Korean;
                break;
            case "English":
                languageState = Language.English;
                break;
            case "Japanese":
                languageState = Language.Japanese;
                break;
            default:
                break;
        }
    }

    //게임 실행시 호출
    public void CheckIngameReward()
    {
        SetIngameRewardScore(CurrentScore);
        if (GetIngameRewardItemValue_1() >= 1f && GetIngameRewardState(1) == IngameRewardState.None)
            SetIngameRewardState(1, IngameRewardState.Achieve);
        if (GetIngameRewardItemValue_2() >= 1f && GetIngameRewardState(2) == IngameRewardState.None)
            SetIngameRewardState(2, IngameRewardState.Achieve);
        if (GetIngameRewardState(2) == IngameRewardState.Rewarded && GetIngameRewardState(1) == IngameRewardState.Rewarded)
        {
            if (IngameRewardLevel < 20) IngameRewardLevel++;
            SetIngameRewardState(1, IngameRewardState.None);
            SetIngameRewardState(2, IngameRewardState.None);
            SetIngameRewardScore(0, true);
        }
    }

    public float GetIngameRewardItemValue_1()
    {
        float ret = 0;

        if (GetIngameRewardState(1) == IngameRewardState.None)
            ret = (float)GetIngameRewardScore() / (float)IngameRewardList[IngameRewardLevel - 1].rewardItemScore_1;
        else ret = 1f;

        return ret;
    }

    public float GetIngameRewardItemValue_2()
    {
        float ret = 0;

        if (GetIngameRewardState(2) == IngameRewardState.None)
            ret = (float)GetIngameRewardScore() / (float)IngameRewardList[IngameRewardLevel - 1].rewardItemScore_2;
        else ret = 1f;

        return ret;
    }

    public IngameRewardState GetIngameRewardState(int rewardId)
    {
        return (IngameRewardState)PlayerPrefsElite.GetInt("IngameRewardState" + rewardId);
    }

    public void SetIngameRewardState(int rewardId, IngameRewardState state)
    {
        PlayerPrefsElite.SetInt("IngameRewardState" + rewardId, (int)state);
    }

    public string LobbyRewardTime
    {
        get
        {
            if (PlayerPrefs.HasKey("LobbyRewardTime"))
                return PlayerPrefsElite.GetString("LobbyRewardTime");
            else
                return "";
        }
        set { PlayerPrefsElite.SetString("LobbyRewardTime", value); }
    }

    public bool GetLobbyRewardTime()
    {
        bool ret = false;
        if (string.IsNullOrEmpty(LobbyRewardTime))
            return true;

        System.DateTime dt = System.DateTime.Parse(LobbyRewardTime).AddMinutes(30);
        System.DateTime dt2 = UnbiasedTime.Instance.Now();
        System.TimeSpan ts = dt.Subtract(dt2);

        if (ts.Minutes <= 0 && ts.Seconds <= 0)
        {
            ret = true;
        }
        return ret;
    }

    public string LobbyGetCoinAds
    {
        get
        {
            if (PlayerPrefs.HasKey("LobbyGetCoinAds"))
                return PlayerPrefsElite.GetString("LobbyGetCoinAds");
            else
                return "";
        }
        set { PlayerPrefsElite.SetString("LobbyGetCoinAds", value); }
    }

    public bool GetLobbyGetCoinAdsTime()
    {
        bool ret = false;
        if (string.IsNullOrEmpty(LobbyGetCoinAds))
            return true;

        System.DateTime dt = System.DateTime.Parse(LobbyGetCoinAds).AddMinutes(10);
        System.DateTime dt2 = UnbiasedTime.Instance.Now();
        System.TimeSpan ts = dt.Subtract(dt2);

        if (ts.Minutes <= 0 && ts.Seconds <= 0)
        {
            ret = true;
        }
        return ret;
    }

    public bool isLastStage()
    {
        return (CurGameStage == maxStageNum + 1);
    }

    public float LobbyStageListCurrent()
    {
        if (CurGameStage == maxStageNum) return 1f;
        else return ((float)CurGameStage / (float)maxStageNum);
    }

    // 0827
    public string DailyRewardTime
    {
        get
        {
            if (PlayerPrefs.HasKey("DailyRewardTime"))
                return PlayerPrefsElite.GetString("DailyRewardTime");
            else
                return "";
        }
        set { PlayerPrefsElite.SetString("DailyRewardTime", value); }
    }

    public bool GetDailyRewardTime()
    {
        bool ret = false;
        if (string.IsNullOrEmpty(DailyRewardTime))
            return true;

        System.DateTime dt = System.DateTime.Parse(DailyRewardTime);
        System.DateTime dt2 = UnbiasedTime.Instance.Now();

        if (!dt.Day.Equals(dt2.Day))
        {
            ret = true;
        }
        return ret;
    }

    public int DailyRewardCount
    {
        get
        {
            if (PlayerPrefs.HasKey("DailyRewardCount"))
                return PlayerPrefsElite.GetInt("DailyRewardCount");
            else
                return 0;
        }
        set { PlayerPrefsElite.SetInt("DailyRewardCount", value); }
    }
    public bool DailyRewardAD
    {
        get
        {
            if (PlayerPrefs.HasKey("DailyRewardAD"))
                return PlayerPrefsElite.GetBoolean("DailyRewardAD");
            else
                return false;
        }
        set { PlayerPrefsElite.SetBoolean("DailyRewardAD", value); }
    }

    public void CallPurchaser(int pId, int pData)
    {
        myPurchaser.curPid = pId;
        myPurchaser.curChargeData = pData;
        myPurchaser.BuyConsumable();
    }

    int gameplayCount = 0;
    public bool showInterstitialAds()
    {
        if (!Application.isEditor)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return false;
            if (NoAdsUse) return false;

            if (CurGameMode == (int)GameMode.STAGE || CurGameMode == (int)GameMode.STAGEBOSS)
            {
                ++gameplayCount;
                if (CurGameStage <= 50 && gameplayCount % 3 == 0)
                {
                    if (AdsManager.instance.isInterstitialLoad())
                    {
                        AdsManager.instance.ShowInterstitialAd();
                        gameplayCount = 0;
                        return true;
                    }
                    return false;
                }
                if (CurGameStage > 50 && gameplayCount % 2 == 0)
                {
                    if (AdsManager.instance.isInterstitialLoad())
                    {
                        AdsManager.instance.ShowInterstitialAd();
                        gameplayCount = 0;
                        return true;
                    }
                    return false;
                }
            }
            else
            {
                if (AdsManager.instance.isInterstitialLoad())
                {
                    AdsManager.instance.ShowInterstitialAd();
                    return true;
                }
                return false;
            }
        }
        return false;
    }

    //파이어베이스 통계툴에 로그 출력.
    public void SetFirebaseAnalyticsStageClearLog(FirebaseType myType, int aIdx)
    {
        //if (!Application.isEditor && Application.internetReachability != NetworkReachability.NotReachable)
        //    Firebase.Analytics.FirebaseAnalytics.LogEvent(myType.ToString(), myType.ToString(), FireBaseMsgList[aIdx]);
    }

    public void SetFirebaseAnalyticsStageClearLog(FirebaseType myType, string msg)
    {
        //if (!Application.isEditor && Application.internetReachability != NetworkReachability.NotReachable)
        //    Firebase.Analytics.FirebaseAnalytics.LogEvent(myType.ToString(), myType.ToString(), msg);
    }

    public void CheckFirstPayment()
    {
        PlayerPrefsElite.SetBoolean("FirstPayment", true);
        GPGSManager.instance.setAchievements(4);
    }

    public bool isFirstPayment()
    {
        if (!PlayerPrefs.HasKey("FirstPayment"))
            return PlayerPrefs.HasKey("FirstPayment");
        else
            return PlayerPrefsElite.GetBoolean("FirstPayment");
    }

    #region 클라우드 저장 / 불러오기 기능
    public string SaveGameDataChange()
    {
        GameData data = new GameData();
#if UNITY_ANDROID
        data.UId = Social.localUser.id;
        data.UName = Social.localUser.userName;
#endif
#if UNITY_IOS
        data.UId = ISN_GKLocalPlayer.LocalPlayer.PlayerID;
        data.UName = ISN_GKLocalPlayer.LocalPlayer.DisplayName;
#endif
        data.coin = GetCoin();
        data.ball100Score = GetBestScore(2);
        data.ClassicScore = GetBestScore(3);
        data.shootingScore = GetBestScore(4);
        data.IngameRewardLevel = IngameRewardLevel;
        data.IngameRewardScore = GetIngameRewardScore();
        data.SweetAim = SweetAimUse;
        data.UseNoAds = NoAdsUse;
        data.SelectBall = BallSprite;
        data.SelectMonster = SelectMoster;
        data.GiftGetTime = LobbyRewardTime;
        data.isGiftTime = GetLobbyRewardTime();
        data.GetCoinADsState = LobbyGetCoinAds;
        data.isGetCoinTime = GetLobbyGetCoinAdsTime();
        data.CurGameStage = curStageIdx();
        data.ingameItems = instanceItemList;
        data.stageStars = stageStarList;
        data.stageScores = stageScoreList;
        data.stageLocks = stageLockList;
        data.stageChapterList = stageChapterStateList;
        data.TotalStarCount = TotalStarCount;
        for (int i = 0; i < charBallList.Count; ++i)
        {
            if (charBallList[i] == 3)
                charBallList[i] = 0;
        }

        data.charBallList = charBallList;
        data.housingStateList = HousingStateList;
        data.DailyTime = DailyRewardTime;
        data.DailyCount = DailyRewardCount;
        data.isDailyAds = DailyRewardAD;
        data.isDailyRewardGet = GetDailyRewardTime();
        data.isDailyAdd = true;
        List<bool> achi = new List<bool>();
        for (int i = 0; i < 5; ++i)
        {
            achi.Add(GetAchievement(i));
        }
        data.Achievements = achi;

        return ObjectToJson(data);
    }

    public void GetGameDataChange(string data)
    {
        if (myCloudData == null)
        {
            myCloudData = new GameData();
            myCloudData.isGetCoinTime = true;
            myCloudData.isGiftTime = true;

        }
        else if (!myCloudData.isDailyAdd)
        {
            myCloudData.isDailyRewardGet = true;
            myCloudData.isDailyAds = true;
        }

        if (data == null)
        {
            if (GetSceneType() == SceneType.Preload)
            {
                PreloadControl.instance.CheckLongTimeReward();
                return;
            }
        }
        myCloudData = JsonToOject<GameData>(data);
        if (myCloudData == null)
        {
            if (GetSceneType() == SceneType.Preload)
            {
                PreloadControl.instance.CheckLongTimeReward();
                return;
            }
        }
        switch (GPGSManager.instance.mType)
        {
            case GpgsLoginType.save:
                PreBallSprite = 0;
                PreSelectMoster = 0;
                GetCloudDataLocal();
                break;
            case GpgsLoginType.first:
                PlayerPrefsElite.SetInt("Money", myCloudData.coin);

                if (instanceItemList == null)
                    instanceItemList = new List<int>();
                else
                    instanceItemList.Clear();

                for (int i = 0; i < myCloudData.ingameItems.Count; ++i)
                {
                    instanceItemList.Add(myCloudData.ingameItems[i]);
                }

                PlayerPrefsElite.SetIntArray("InstanceItemList", instanceItemList.ToArray());
                if (GetSceneType() == SceneType.Preload)
                    PreloadControl.instance.CheckLongTimeReward();
                break;
            case GpgsLoginType.login:
                if (GetSceneType() == SceneType.Lobby)
                {
                    LobbyManager.instance.SetOptionToCloudData();
                }
                break;
        }
    }

    public void GetCloudDataLocal()
    {
        PlayerPrefsElite.SetInt("Money", myCloudData.coin);
        PlayerPrefsElite.SetInt("Ball100Score", myCloudData.ball100Score);
        PlayerPrefsElite.SetInt("ClassicScore", myCloudData.ClassicScore);
        PlayerPrefsElite.SetInt("ShootingScore", myCloudData.shootingScore);
        GPGSManager.instance.setScoreLeaderBoard(1, myCloudData.ball100Score);
        GPGSManager.instance.setScoreLeaderBoard(0, myCloudData.ClassicScore);
        GPGSManager.instance.setScoreLeaderBoard(3, myCloudData.shootingScore);
        IngameRewardLevel = myCloudData.IngameRewardLevel;
        SetIngameRewardScore(myCloudData.IngameRewardScore);
        SweetAimUse = myCloudData.SweetAim;
        NoAdsUse = myCloudData.UseNoAds;
        BallSprite = myCloudData.SelectBall;
        SelectMoster = myCloudData.SelectMonster;
        CurGameStage = myCloudData.CurGameStage;
        if (myCloudData.GiftGetTime != null)
        {
            if (myCloudData.isGiftTime)
            {
                LobbyRewardTime = myCloudData.GiftGetTime;
            }
            else
            {
                LobbyRewardTime = UnbiasedTime.Instance.Now().ToString();
                myCloudData.GiftGetTime = UnbiasedTime.Instance.Now().ToString();
            }
        }
        else
            LobbyRewardTime = "";

        if (myCloudData.GetCoinADsState != null)
        {
            if (myCloudData.isGetCoinTime)
            {
                LobbyGetCoinAds = myCloudData.GetCoinADsState;
            }
            else
            {
                LobbyGetCoinAds = UnbiasedTime.Instance.Now().ToString();
                myCloudData.GetCoinADsState = UnbiasedTime.Instance.Now().ToString();
            }
        }
        else
            LobbyGetCoinAds = "";
        DailyRewardAD = myCloudData.isDailyAds;
        DailyRewardCount = myCloudData.DailyCount;

        if (string.IsNullOrEmpty(myCloudData.DailyTime))
        {
            DailyRewardTime = "";
        }
        else
            DailyRewardTime = UnbiasedTime.Instance.Now().ToString();

        GetCloudStageData();
        GetCloudCharBallData();

        if (instanceItemList == null)
            instanceItemList = new List<int>();
        else
            instanceItemList.Clear();

        for (int i = 0; i < myCloudData.ingameItems.Count; ++i)
        {
            instanceItemList.Add(myCloudData.ingameItems[i]);
        }

        PlayerPrefsElite.SetIntArray("InstanceItemList", instanceItemList.ToArray());
        for (int i = 0; i < myCloudData.Achievements.Count; ++i)
        {
            if (myCloudData.Achievements[i])
                GPGSManager.instance.setAchievements(i);
        }
        LobbyTipView = true;
        GPGSManager.instance.mType = GpgsLoginType.none;
        for (int i = 0; i < HousingStateList.Count; ++i)
        {
            if (GetHousingStateList(i) > HousingStateCount(i))
                SetHousingStateList(i, HousingStateCount(i));
        }

        if (GetSceneType() == SceneType.Lobby)
        {
            LobbyManager.instance.SetOptionPupData();
        }
    }

    void GetCloudStageData()
    {
        if (stageStarList == null)
            stageStarList = new List<int>();
        else
            stageStarList.Clear();

        for (int i = 0; i < myCloudData.stageStars.Count; ++i)
        {
            stageStarList.Add(myCloudData.stageStars[i]);
        }
        PlayerPrefsElite.SetIntArray("StageStar", stageStarList.ToArray());

        if (stageScoreList == null)
            stageScoreList = new List<int>();
        else
            stageScoreList.Clear();

        for (int i = 0; i < myCloudData.stageScores.Count; ++i)
        {
            stageScoreList.Add(myCloudData.stageScores[i]);
        }
        PlayerPrefsElite.SetIntArray("StageScore", stageScoreList.ToArray());

        if (stageLockList == null)
            stageLockList = new List<int>();
        else
            stageLockList.Clear();

        for (int i = 0; i < myCloudData.stageLocks.Count; ++i)
        {
            stageLockList.Add(myCloudData.stageLocks[i]);
        }
        PlayerPrefsElite.SetIntArray("StageClear", stageLockList.ToArray());

        if (stageChapterStateList == null)
            stageChapterStateList = new List<int>();
        else
            stageChapterStateList.Clear();

        for (int i = 0; i < myCloudData.stageChapterList.Count; ++i)
        {
            stageChapterStateList.Add(myCloudData.stageChapterList[i]);
        }
        PlayerPrefsElite.SetIntArray("stageChapterState", stageChapterStateList.ToArray());

        if (HousingStateList == null)
            HousingStateList = new List<int>();
        else
            HousingStateList.Clear();

        for (int i = 0; i < myCloudData.housingStateList.Count; ++i)
        {
            if (myCloudData.housingStateList[i] > PlayerPrefsElite.GetIntArray("HousingStateList")[i])
                HousingStateList.Add(myCloudData.housingStateList[i]);
            else
                HousingStateList.Add(PlayerPrefsElite.GetIntArray("HousingStateList")[i]);
        }
        PlayerPrefsElite.SetIntArray("HousingStateList", HousingStateList.ToArray());

        if (HousingStateList == null)
            HousingStateList = new List<int>();
        else
            HousingStateList.Clear();

        for (int i = 0; i < myCloudData.housingStateList.Count; ++i)
        {
            if (myCloudData.housingStateList[i] > PlayerPrefsElite.GetIntArray("housingStateList")[i])
                HousingStateList.Add(myCloudData.housingStateList[i]);
            else
                HousingStateList.Add(PlayerPrefsElite.GetIntArray("housingStateList")[i]);
        }
        PlayerPrefsElite.SetIntArray("HousingStateList", HousingStateList.ToArray());

        if (myCloudData.TotalStarCount < GetTotalStarCount())
            TotalStarCount = GetTotalStarCount();
    }

    int HousingStateCount(int idx)
    {
        int ret = 0;

        for (int i = 1 * (10 * idx); i < (1 * (10 * idx)) + 10; ++i)
        {
            if (GetChapterStateList(i) == StageStarState.complete)
                ++ret;
        }

        return ret;
    }

    void GetCloudCharBallData()
    {
        if (charBallList == null)
            charBallList = new List<int>();
        else
            charBallList.Clear();

        for (int i = 0; i < myCloudData.charBallList.Count; ++i)
        {
            if (i == 0 && myCloudData.SelectCharBall != 0 && myCloudData.charBallList[i] == 0)
                myCloudData.charBallList[i] = 1;

            if (myCloudData.charBallList[i] == 3)
                myCloudData.charBallList[i] = 0;
            else
            {
                if (myCloudData.SelectCharBall == i)
                    myCloudData.charBallList[i] = 2;
                else if (myCloudData.charBallList[i] == 2)
                    myCloudData.charBallList[i] = 1;
            }
            charBallList.Add(myCloudData.charBallList[i]);
        }
        if (charBallList.Count == 0)
        {
            PlayerPrefs.DeleteKey("charBallList");
            charBallList = initUserLocalData("charBallList", maxShopNum, 1, 0, 0);
        }
        else
        {
            if (charBallList.Count < maxShopNum)
                charBallList = initUserLocalData("charBallList", maxShopNum, 1, 0, 0);
        }
    }

    public void HousingRewardCheck(int rIdx)
    {
        switch (rIdx)
        {
            case 0:
                SetCharBallList(4, (int)ShopState.UnUse);
                SetCharBallList(5, (int)ShopState.UnUse);
                LobbyManager.instance.CompleteHousing(4);
                LobbyManager.instance.CompleteHousing(5);
                break;
            case 1:
                SetCharBallList(8, (int)ShopState.UnUse);
                SetCharBallList(9, (int)ShopState.UnUse);
                LobbyManager.instance.CompleteHousing(8);
                LobbyManager.instance.CompleteHousing(9);
                break;
            case 2:
                SetCharBallList(20, (int)ShopState.UnUse);
                SetCharBallList(21, (int)ShopState.UnUse);
                LobbyManager.instance.CompleteHousing(20);
                LobbyManager.instance.CompleteHousing(21);
                break;
            case 3:
                SetCharBallList(18, (int)ShopState.UnUse);
                SetCharBallList(19, (int)ShopState.UnUse);
                LobbyManager.instance.CompleteHousing(18);
                LobbyManager.instance.CompleteHousing(19);
                break;
            case 4:
                SetCharBallList(22, (int)ShopState.UnUse);
                SetCharBallList(23, (int)ShopState.UnUse);
                LobbyManager.instance.CompleteHousing(22);
                LobbyManager.instance.CompleteHousing(23);
                break;
        }
    }

    string ObjectToJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    T JsonToOject<T>(string jsonData)
    {
        return JsonUtility.FromJson<T>(jsonData);
    }

    public void CallGameDataSave()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
            GPGSManager.instance.SaveToCloud();
    }

    public void CloudSaveGameDataSetup()
    {
        if (myCloudData != null)
        {
            myCloudData.CurGameStage = curStageIdx();
            if (myCloudData.stageStars.Count > 0)
                myCloudData.stageStars.Clear();
            else
                myCloudData.stageStars = new List<int>();

            for (int i = 0; i < stageStarList.Count; ++i)
            {
                myCloudData.stageStars.Add(stageStarList[i]);
            }
            myCloudData.coin = GetCoin();
        }
    }
    #endregion
    public int curStageIdx()
    {
        int idx = stageLockList.FindLastIndex(t => t == 1);

        if (idx == -1)
        {
            idx = CurGameStage;
            //idx = 999;
        }
        else
        {
            idx += 1;
        }


        return idx;
    }

    public void CallLobby()
    {
        SceneLoad();
        if (!Application.isEditor)
            SetFirebaseAnalyticsStageClearLog(FirebaseType.new_user, 0);
    }

    public void CallRestore()
    {
        myPurchaser.RestorePurchases();
    }

    public int GetHousingStateList(int i)
    {
        return HousingStateList[i];
    }

    public void SetHousingStateList(int i, int data)
    {
        HousingStateList[i] = data;
        PlayerPrefsElite.SetIntArray("HousingStateList", HousingStateList.ToArray());
    }

    public HousingState GetHousingState(int i)
    {
        if (HousingStateList[i] == 11)
            return HousingState.complete;
        else if (HousingStateList[i] == 10)
            return HousingState.getreward;
        else if (HousingStateList[i] == -1)
            return HousingState.none;
        else
            return HousingState.collect;
    }

    public void ResetStageStarList(int num)
    {
        for (int i = 0; i < num - 1; i++)
        {
            SetStageStarList(i, 3);
        }
        for (int i = stageStarList.Count - 1; i >= num - 1; i--)
        {
            stageStarList[i] = 0;
        }
    }

    public string GetProductPrice(int pId)
    {
        return myPurchaser.GetProductPrice(pId);
    }
}