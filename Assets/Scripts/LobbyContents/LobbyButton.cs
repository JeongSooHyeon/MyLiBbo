using UnityEngine;

public class LobbyButton : MonoBehaviour
{
    [SerializeField] UIState state_;
    [SerializeField] ButtonState btnState_;
    [SerializeField] bool isPlay = true;
    [SerializeField] int idx;
    bool isAd;

    void OnClick()
    {
        SoundManager.instance.TapSound();
        if (btnState_ == ButtonState.TweenBtn && !LobbyController.instance.isResult) LobbyController.instance.SetTween(state_, isPlay);

        switch (state_)
        {
            case UIState.Back:
                LobbyController.instance.SetBack();
                break;
            case UIState.Exit:
                PlayerPrefsElite.SetInt("ResultBtnState", 0);
                LobbyController.instance.SetTween(UIState.Pause, false);
                Time.timeScale = 1;
                UIManager.instance.CallLoadingPup();
                isAd = DataManager.instance.showInterstitialAds();

                //-------------------test-----------------------
                if (!isAd)
                {
                    UIManager.instance.ResetGame();
                }
                break;
            case UIState.Coin:
                break;
            case UIState.Tutorial:
                if (isPlay) LobbyController.instance.SetTween(state_, true);
                break;
            case UIState.DownBall:
                SoundManager.instance.ChangeEffects(1);
                BallManager.instance.ResetBall();
                break;
            case UIState.InGame:
                DataManager.instance.SceneLoad(SceneType.Ingame);
                break;
            case UIState.ReStartGame:
                PlayerPrefsElite.SetInt("ResultBtnState", 2);
                UIState uiState = UIState.Pause;
                switch (idx)
                {
                    case 0:
                        uiState = UIState.Pause;
                        break;
                    case 1:
                        uiState = UIState.ResultFailed;
                        break;
                    case 2:
                        uiState = UIState.ResultSuc;
                        if (DataManager.instance.CurGameStage != DataManager.maxStageNum) --DataManager.instance.CurGameStage;
                        break;
                    case 3:
                        uiState = UIState.BestScore;
                        break;
                }
                LobbyController.instance.SetTween(uiState, false);
                Time.timeScale = 1;
                UIManager.instance.CallLoadingPup();
                isAd = DataManager.instance.showInterstitialAds();
                //-------------------test-----------------------
                if (!isAd)
                {
                    if (!BrickManager.instance.isStage) DataManager.instance.SetSaveFile(false);
                    if ((GameMode)DataManager.instance.CurGameMode != GameMode.STAGE
                        && (GameMode)DataManager.instance.CurGameMode != GameMode.STAGEBOSS)
                        DataManager.instance.NextStagePlay();
                    else DataManager.instance.GameStartCall();
                }
                DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.game_result, 15);
                break;
            case UIState.SoundMute:
                UIManager.instance.SetBGMSound();
                break;
            case UIState.EffectSoundMute:
                UIManager.instance.SetFXSound();
                break;
            case UIState.Pause:
                if (!isPlay)
                {
                    AdsManager.instance.BannerEnable(true);
                    AdsManager.instance.NativeEnable(false);
                }
                break;
            case UIState.ShootingEdit:
                LobbyController.instance.SetTween(UIState.ShootingEdit, true);
                break;
        }
    }
}
