#if UNITY_ANDROID
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
//using GooglePlayGames.BasicApi.SavedGame;
#endif
using System;
using System.Text;
#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
using SA.iOS.GameKit;
using SA.Foundation.Templates;
#endif
using UnityEngine;

public class GPGSManager : MonoBehaviour
{
    public static GPGSManager instance;

    public bool onSaving;
    public bool onLoading;
    public string saveData;
    public GpgsLoginType mType;
#if UNITY_IOS
    private List<ISN_GKSavedGame> m_fetchedSavedGames = new List<ISN_GKSavedGame>();
    private List<string> m_conflictedSavedGames = new List<string>();
#endif
    void Awake()
    {
        if (instance == null)
            instance = this;

#if UNITY_ANDROID
        if (Application.internetReachability != NetworkReachability.NotReachable)
            initPlayGame();
#endif
#if UNITY_IOS
        //GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
        ISN_GKLocalPlayerListener.DidModifySavedGame.AddListener(DidModifySavedGame);
        ISN_GKLocalPlayerListener.HasConflictingSavedGames.AddListener(HasConflictingSavedGames);
#endif
    }

    void Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
#if UNITY_ANDROID
            PlayerPrefsElite.SetBoolean("GoogleLogin_", false);
#endif      
        }
        else
        {
#if UNITY_ANDROID
            if (PlayerPrefs.HasKey("GoogleLogin_"))
            {
                if (PlayerPrefsElite.GetBoolean("GoogleLogin_"))
                {
                    mType = GpgsLoginType.first;
                    if (!bLogin()) LoginGPGS();
                }
            }
            else
            {
                mType = GpgsLoginType.first;
                if (!bLogin()) LoginGPGS();
            }
#endif
#if UNITY_IOS
            mType = GpgsLoginType.first;
            LoginGPGS();
           // DataManager.instance.CallLobby();
#endif
        }

    }

    public void initPlayGame()
    {
#if UNITY_ANDROID
       // PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
       ////// enables saving game progress.
       //.EnableSavedGames()
       ////// registers a callback to handle game invitations received while the game is not running.
       ////.WithInvitationDelegate(< callback method >)
       ////// registers a callback for turn based match notifications received while the
       ////// game is not running.
       ////.WithMatchDelegate(< callback method >)
       ////// requests the email address of the player be available.
       ////// Will bring up a prompt for consent.
       ////.RequestEmail()
       ////// requests a server auth code be generated so it can be passed to an
       //////  associated back end server application and exchanged for an OAuth token.
       ////.RequestServerAuthCode(false)
       ////// requests an ID token be generated.  This OAuth token can be used to
       //////  identify the player to other services such as Firebase.
       ////.RequestIdToken()
       //.Build();

       // PlayGamesPlatform.InitializeInstance(config);
       // // recommended for debugging:
       // PlayGamesPlatform.DebugLogEnabled = true;
       // // Activate the Google Play Games platform
       // PlayGamesPlatform.Activate();
#endif
    }

#if UNITY_IOS
     void OnDestroy() {
        ISN_GKLocalPlayerListener.DidModifySavedGame.RemoveListener(DidModifySavedGame);
        ISN_GKLocalPlayerListener.HasConflictingSavedGames.RemoveListener(HasConflictingSavedGames);
    }

     private void DidModifySavedGame(ISN_GKSavedGameSaveResult result) {
            Debug.Log("DidModifySavedGame! Device name = " + result.SavedGame.DeviceName + " | game name = " + result.SavedGame.Name + " | modification Date = " + result.SavedGame.ModificationDate.ToString());
        }
     private void HasConflictingSavedGames(ISN_GKSavedGameFetchResult result) {
            foreach(ISN_GKSavedGame game in result.SavedGames) {
                m_conflictedSavedGames.Add(game.Id);
            }

            foreach (ISN_GKSavedGame game in result.SavedGames) {
                Debug.Log("HasConflictingSavedGames! Device name = " 
                          + game.DeviceName + " | game name = " 
                          + game.Name + " | modification Date = " 
                          + game.ModificationDate.ToString());
            }
        }
#endif

    public bool bLogin()
    {
#if UNITY_IOS
        return ISN_GKLocalPlayer.LocalPlayer.Authenticated;
#endif
#if UNITY_ANDROID
        return Social.localUser.authenticated;
#endif
    }
    // GPGS를 로그인 합니다.
    public void LoginGPGS()
    {
        if (Application.isEditor) return;
        //Debug.Log("LoginGPGS");
        //// 로그인이 안되어 있으면
        //// request auth
        ///

#if UNITY_ANDROID
        Social.localUser.Authenticate((bool _success) =>
        {
            PlayerPrefsElite.SetBoolean("GoogleLogin_", _success);
            // if login success

            if (DataManager.instance.GetSceneType() == SceneType.Lobby)
            {
                LobbyManager.instance.OptionGoogleBtnCheck();
            }

            if (_success)
            {
                Debug.Log("Authenticate is succeeded!");
                LoadFromCloud();
            }
            else
            {
                Debug.Log("Authenticate is failed!");
                PreloadControl.instance.CheckLongTimeReward();
            }

        });
#endif
#if UNITY_IOS
        ISN_GKLocalPlayer.Authenticate((SA_Result result) => {
            PlayerPrefsElite.SetBoolean("GoogleLogin_", result.IsSucceeded);
            FetchCloudData();
            
            if (result.IsSucceeded)
            {
                Debug.Log("Authenticate is succeeded!");
            }
            else
            {
                Debug.Log("Authenticate is failed! Error with code: " + result.Error.Code + " and description: " + result.Error.Message);
                PreloadControl.instance.CheckLongTimeReward();
            }
            //DataManager.instance.CallLobby();
        });
#endif
    }

    public void showLeaderBoard()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (!bLogin()) LoginGPGS();
#if UNITY_ANDROID
            Social.ShowLeaderboardUI();
#endif
#if UNITY_IOS
        ISN_GKGameCenterViewController viewController = new ISN_GKGameCenterViewController();
        viewController.ViewState = ISN_GKGameCenterViewControllerState.Achievements;
        viewController.Show();
#endif
        }
    }
    public void showAchievements()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (!bLogin()) LoginGPGS();
#if UNITY_ANDROID
            Social.ShowAchievementsUI();
#endif
#if UNITY_IOS
        ISN_GKGameCenterViewController viewController = new ISN_GKGameCenterViewController();
        viewController.ViewState = ISN_GKGameCenterViewControllerState.Achievements;
        viewController.Show();
#endif
        }
    }

    public void setScoreLeaderBoard(int lId, int Score)
    {
        //int myScore = DataManager.instance.GetBestScore(); // 수정
        string leId = "";
        switch (lId)
        {
            case 0: leId = Bbosiraegi.leaderboard_best_score_of_classic; break;
            case 1: leId = Bbosiraegi.leaderboard_cumulative_score_of_100ball; break;
            case 2: leId = Bbosiraegi.leaderboard_number_of_stage_clear_stars; break;
            case 3: leId = Bbosiraegi.leaderboard_best_score_of_shooting; break;    // 슈팅모드 임시
        }
#if UNITY_ANDROID
        Social.ReportScore(Score, leId, (bool success) =>
        {
            //handle success or failure
        });
#endif
#if UNITY_IOS
        ISN_GKScore scoreReporter= new ISN_GKScore(leId);
        scoreReporter.Value = Score;
        scoreReporter.Context = 1;

        scoreReporter.Report((result) => {
            if (result.IsSucceeded) {
                Debug.Log("Score Report Success");
            } else {
                Debug.Log("Score Report failed! Code: " + result.Error.Code + " Message: " + result.Error.Message);
            }
        });
#endif
    }

    public void setAchievements(int idx = 0)
    {
        if (DataManager.instance.GetAchievement(idx)) return;

        string achIDx = "";
        switch (idx)
        {
            case 0: achIDx = Bbosiraegi.achievement_reach_5000_score_in_classic; break;
            case 1: achIDx = Bbosiraegi.achievement_reach_10000_score_in_100ball; break;
            case 2: achIDx = Bbosiraegi.achievement_100_stage_clear; break;
            case 3: achIDx = Bbosiraegi.achievement_500_stars_in_stage; break;
            case 4: achIDx = Bbosiraegi.achievement_first_payment; break;
            //case 5: achIDx = Bbosiraegi.leaderboard_best_score_of_shooting; break;
        }
#if UNITY_ANDROID
        Social.ReportProgress(achIDx, 100.0f, (bool success) =>
        {
            // handle success or failure
            DataManager.instance.SetAchievement(idx);
        });
#endif
#if UNITY_IOS
        ISN_GKAchievement achievement = new ISN_GKAchievement(achIDx);
        achievement.PercentComplete = 100.0f;
        achievement.Report((result) => {
	        if(result.IsSucceeded) {
		        Debug.Log("Achievement reported");
	        } else {
		        Debug.Log("Achievement report failed! Code: " + result.Error.Code + " Message: " + result.Error.Message);
	        }
        });
#endif
    }

    public void LogoutGoogle()
    {
#if UNITY_ANDROID
        PlayerPrefsElite.SetBoolean("GoogleLogin_", false);
        LobbyManager.instance.OptionGoogleBtnCheck();
        //PlayGamesPlatform.Instance.SignOut();
#endif
    }
#if UNITY_ANDROID
    #region Save

    public void SaveToCloud()
    {
        if (!bLogin()) LoginGPGS();

        Debug.Log("Try to save data");
        onSaving = true;

        string id = Social.localUser.id;
        string fileName = string.Format("{0}_DATA", id);
        saveData = DataManager.instance.SaveGameDataChange();
        Debug.Log("??>" + saveData);
        OpenSavedGame(fileName, true);
    }

    void OpenSavedGame(string _fileName, bool _saved)
    {
        //ISavedGameClient savedClient = PlayGamesPlatform.Instance.SavedGame;

        //if (_saved == true)
        //{
        //    savedClient.OpenWithAutomaticConflictResolution(_fileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpendToSave);
        //}
        //else
        //{
        //    savedClient.OpenWithAutomaticConflictResolution(_fileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToRead);
        //}
    }

    //void OnSavedGameOpendToSave(SavedGameRequestStatus _status, ISavedGameMetadata _data)
    //{
    //    if (_status == SavedGameRequestStatus.Success)
    //    {
    //        byte[] b = Encoding.UTF8.GetBytes(saveData);
    //        SaveGame(_data, b, DateTime.Now.TimeOfDay);
    //    }
    //    else
    //    {
    //        Debug.Log("Fail");
    //    }
    //}

    //void SaveGame(ISavedGameMetadata _data, byte[] _byte, TimeSpan _playTime)
    //{
    //    ISavedGameClient savedClient = PlayGamesPlatform.Instance.SavedGame;
    //    SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();

    //    builder = builder.WithUpdatedPlayedTime(_playTime).WithUpdatedDescription("Saved at " + DateTime.Now);

    //    SavedGameMetadataUpdate updateData = builder.Build();
    //    savedClient.CommitUpdate(_data, updateData, _byte, OnSavedGameWritten);
    //}

    //void OnSavedGameWritten(SavedGameRequestStatus _status, ISavedGameMetadata _data)
    //{
    //    onSaving = false;
    //    if (_status == SavedGameRequestStatus.Success)
    //    {
    //        Debug.Log("Save Complete");
    //        DataManager.instance.CloudSaveGameDataSetup();
    //        if (DataManager.instance.GetSceneType() == SceneType.Lobby)
    //        {
    //            LobbyManager.instance.SetOptionPupData();
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Save Fail");
    //    }
    //}

    #endregion

    #region Load

    public void LoadFromCloud()
    {
        if (!bLogin()) LoginGPGS();

        onLoading = true;
        string id = Social.localUser.id;
        string fileName = string.Format("{0}_DATA", id);

        OpenSavedGame(fileName, false);
    }

    //void OnSavedGameOpenedToRead(SavedGameRequestStatus _status, ISavedGameMetadata _data)
    //{
    //    if (_status == SavedGameRequestStatus.Success)
    //    {
    //        Debug.Log("Success" + _status);
    //        if (_data == null)
    //        {
    //            if (DataManager.instance.GetSceneType() == SceneType.Preload)
    //                PreloadControl.instance.CheckLongTimeReward();
    //        }
    //        else
    //            LoadGameData(_data);
    //    }
    //    else
    //    {
    //        Debug.Log("Fail");
    //    }
    //}

    //void LoadGameData(ISavedGameMetadata _data)
    //{
    //    ISavedGameClient savedClient = PlayGamesPlatform.Instance.SavedGame;
    //    savedClient.ReadBinaryData(_data, OnSavedGameDataRead);
    //}

    //void OnSavedGameDataRead(SavedGameRequestStatus _status, byte[] _byte)
    //{
    //    if (_status == SavedGameRequestStatus.Success)
    //    {
    //        if (_byte == null)
    //        {
    //            if (DataManager.instance.GetSceneType() == SceneType.Preload)
    //                PreloadControl.instance.CheckLongTimeReward();
    //        }
    //        else
    //        {
    //            string data = Encoding.Default.GetString(_byte);
    //            DataManager.instance.GetGameDataChange(data);
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Load Fail");
    //    }
    //}
    #endregion
#endif
#if UNITY_IOS
    #region save
        public void SaveToCloud()
        {
            if (!bLogin()) LoginGPGS();

            Debug.Log("Try to save data");
            onSaving = true;

            string id = ISN_GKLocalPlayer.LocalPlayer.PlayerID;
            string fileName = string.Format("{0}_DATA", id);
            saveData = DataManager.instance.SaveGameDataChange();
            //Debug.Log("??>"+ saveData);
            OpenSavedGame(fileName, true);
        }

    #endregion
    #region load

        public void LoadFromCloud()
        {
            if (!bLogin()) LoginGPGS();

            onLoading = true;
            string id = ISN_GKLocalPlayer.LocalPlayer.PlayerID;
            string fileName = string.Format("{0}_DATA", id);

            OpenSavedGame(fileName, false);
        }

    #endregion
        void OpenSavedGame(string _fileName, bool _saved)
        {
            if(_saved)
            {
                 byte[] b = Encoding.UTF8.GetBytes(saveData);
                        ISN_GKLocalPlayer.SavedGame(_fileName, b, (ISN_GKSavedGameSaveResult result) => {
                            if (result.IsSucceeded) {
                                Debug.Log("we made it!");
                                //DataManager.instance.CloudSaveGameDataSetup();
                                //if (DataManager.instance.GetSceneType() == SceneType.Lobby)
                                //{
                                //    LobbyManager.instance.SetOptionPupData();
                                //}
                                FetchCloudData();
                            } else {
                                Debug.Log("SavedGame is failed! With: " + result.Error.Code + " and description: " + result.Error.Message);
                            }
                        });
            }else
            {
                string fileName = string.Format("{0}_DATA", ISN_GKLocalPlayer.LocalPlayer.PlayerID);
                int mIdx = m_fetchedSavedGames.FindIndex(t => t.Name == fileName);
                ISN_GKLocalPlayer.LoadGameData(m_fetchedSavedGames[mIdx], (ISN_GKSavedGameLoadResult result) => {
                    if (result.IsSucceeded) {
                        Debug.Log("Loading game data is succeeded! " +
                                  "StringData = " + result.StringData + " " +
                                  "byte array length " + result.BytesArrayData.Length);
                        string data = Encoding.Default.GetString(result.BytesArrayData);
                        Debug.Log("Load Success");//+data);
                        DataManager.instance.GetGameDataChange(data);
                    } else {
                        Debug.Log("Loading game data is failed! Error with code: " + result.Error.Code + " and description: " + result.Error.Message);
                    }
                });
            }
        }

    void FetchCloudData()
    {
         ISN_GKLocalPlayer.FetchSavedGames((ISN_GKSavedGameFetchResult result) => {
                    if (result.IsSucceeded) {
                        Debug.Log("Loaded " + result.SavedGames.Count + " saved games");
                        m_fetchedSavedGames = result.SavedGames;
                         string fileName = string.Format("{0}_DATA", ISN_GKLocalPlayer.LocalPlayer.PlayerID);
                        int mIdx = m_fetchedSavedGames.FindIndex(t => t.Name == fileName);
                         m_fetchedSavedGames[mIdx].Load((dataResult) => {
                            if(dataResult.IsSucceeded) {
                                string data = Encoding.Default.GetString(dataResult.BytesArrayData);
                                 Debug.Log("FetchSavedGames Load Success");// + data);
                                DataManager.instance.GetGameDataChange(data);
                            } else {
                                Debug.Log("Error: " + dataResult.Error.FullMessage);
                            }
                        });
                    }
                    else {
                        Debug.Log("Fetching saved games is failed! " +
                        "With: " + result.Error.Code + " and description: " + result.Error.Message);
                        //SaveToCloud();
                    }
                });
    }

    public void DelectSaveGame()
    {
        string fileName = string.Format("{0}_DATA", ISN_GKLocalPlayer.LocalPlayer.PlayerID);
        int mIdx = m_fetchedSavedGames.FindIndex(t => t.Name == fileName);
        ISN_GKLocalPlayer.DeleteSavedGame(m_fetchedSavedGames[mIdx], (SA_Result result) => {
            if (result.IsSucceeded)
            {
                Debug.Log("DeleteSavedGame is succeeded!");
            }
            else
            {
                Debug.Log("DeleteSavedGame is failed! Error with code: " + result.Error.Code + " and description: " + result.Error.Message);
            }
        });
    }
#endif
}
