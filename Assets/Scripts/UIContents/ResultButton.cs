using UnityEngine;

public class ResultButton : MonoBehaviour
{
    [SerializeField]
    int idx;
    bool isAD;
    void OnClick()
    {
        switch (idx)
        {
            case 0:
                UIManager.instance.ResetGame();
                LobbyController.instance.SetTween(UIState.Pause, false);
                break;
            case 1:
                PlayerPrefsElite.SetInt("ResultBtnState", 0);
                isAD = DataManager.instance.showInterstitialAds();
                if (!isAD)
                {
                    UIManager.instance.ResetGame();
                }
                DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.game_result, 16);
                LobbyController.instance.SetTween(LobbyController.instance.getGameState(), false);
                UIManager.instance.CallLoadingPup();
                break;
            case 2:
                PlayerPrefsElite.SetInt("ResultBtnState", 1);
                LobbyController.instance.SetTween(UIState.ResultSuc, false);
                UIManager.instance.CallLoadingPup(2f);
                isAD = DataManager.instance.showInterstitialAds();
                if (!isAD)
                {
                    if (DataManager.instance.isLastStage()) UIManager.instance.ResetGame();
                    else
                    {
                        DataManager.instance.GameStartCall();
                    }
                }
                break;
            case 3:
                if (!BrickManager.instance.isStage) DataManager.instance.SetSaveFile(false);
                if ((GameMode)DataManager.instance.CurGameMode == GameMode.BALL100) DataManager.instance.NextStagePlay();
                else DataManager.instance.SceneLoad(SceneType.Ingame);
                break;
            case 4:
                ChargeManager.intance.BuyTween(true, 220);
                break;
            case 5:
                ChargeManager.intance.BuyTween(true, 500);
                break;
            case 6:
                ChargeManager.intance.BuyTween(true, 1200); // 코인 구매 
                break;
            case 7:
                if (!UIManager.instance.continueTodayAD)
                {
                    if (Application.isEditor)
                    {
                        BrickManager.instance.deleteBrickToContinue();
                        UIManager.instance.ContinueToday();
                    }

                    if (AdsManager.instance.isRewardLoad(adsType.continueAD))
                    {
                        UIManager.instance.isShowAds = true;
                        AdsManager.instance.ShowRewardedAd(adsType.continueAD);
                        UIManager.instance.SetContinueBtn();
                    }
                    else if (!AdsManager.instance.isRewardLoad(adsType.continueAD) && DataManager.instance.GetCoin() >= 300)
                    {
                        DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.use_ruby, 7);
                        DataManager.instance.SetCoin(-300);
                        UIManager.instance.SetMoney();
                        BrickManager.instance.deleteBrickToContinue();
                    }
                }
                else if (DataManager.instance.GetCoin() >= 300)
                {
                    DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.use_ruby, 7);
                    DataManager.instance.SetCoin(-300);
                    UIManager.instance.SetMoney();
                    BrickManager.instance.deleteBrickToContinue();
                }
                break;
            case 8:

                break;
            case 9:
                UIManager.instance.Result(); // Continue End Game
                UIManager.instance.SetMoney();
                break;
            case 10:

                GaChaManager.instance.gaChaBundle();
                break;
            case 11:
                GaChaManager.instance.BallDraw(); // BallDraw 뽑기 버튼
                break;
            case 12:
                break;
            case 13:
                GaChaManager.instance.DrawBallPOP_(false);
                break;
            case 14:
                ChargeManager.intance.BuySelec(); // 코인 구매 확인 팝업창
                ChargeManager.intance.BuyTween(false);
                break;
            case 15:
                ChargeManager.intance.BuyTween(false);  // 코인 구매 팝업창 닫기
                break;
            case 16:
                GaChaManager.instance.DrawKick(); // BallDraw 킥 버튼
                break;
            case 17:
                UIManager.instance.Result();
                break;
            case 18:
                break;
            case 19:
                GaChaManager.instance.GetballInfoOff();
                break;
            case 20:
                //GPGSManager.instance.showLeaderBoard(); // 랭킹 버튼
                break;
            case 21:

                break;
            case 22:
                //GPGSManager.instance.showAchievements(); // 업적 버튼
                break;
            case 23:
                ChargeManager.intance.ReBuyTween(true, 100); // 코인 구매
                break;
            case 24:
                ChargeManager.intance.ReBuyTween(true, 220);
                break;
            case 25:
                ChargeManager.intance.ReBuyTween(true, 500);
                break;
            case 26:
                ChargeManager.intance.ReBuyTween(true, 1200); // 코인 구매 
                break;
            case 27:
                ChargeManager.intance.ReBuySelec();
                ChargeManager.intance.ReBuyTween(false);
                break;
            case 28:
                ChargeManager.intance.ReBuyTween(false); // 코인 구매 
                break;
            case 29:
                GaChaManager.instance.noballPOP(false);
                break;
        }
        SoundManager.instance.TapSound();
    }

}
