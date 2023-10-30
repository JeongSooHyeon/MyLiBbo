using UnityEngine;
using I2.Loc;

public class CommonPupControl : PopupBase
{
    [SerializeField] Localize myContentTxt;
    [SerializeField] GameObject[] myBtnArr;
    public int SetData;
    public int SubData;
    public CommonState curState;
    public bool isLobby;
    public bool isReward = false;

    [SerializeField] PopupBase beforePopup;

    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        for (int i = 0; i < myBtnArr.Length; ++i)
        {
            myBtnArr[i].SetActive(false);
        }
        if (isLobby)
        {
            if (LobbyManager.instance.curPopup != null)
                beforePopup = LobbyManager.instance.curPopup;
            LobbyManager.instance.curPopup = this;
        }

        if(myContentTxt != null)
        {
            myContentTxt.SetTerm(curState.ToString());
        }

        if (!isReward)
        {
            switch (curState)
            {
                case CommonState.GetGift:
                case CommonState.InternetConnect:
                case CommonState.GameCenterLogin:
                    myBtnArr[2].SetActive(true);
                    break;
                default:
                    myBtnArr[0].SetActive(true);
                    myBtnArr[1].SetActive(true);
                    break;
            }
        }
    }

    public override void ClosePupTPTS()
    {
        base.ClosePupTPTS();
        if (isLobby)
        {
            LobbyManager.instance.curPopup = null;
            if (curState == CommonState.GetGift)
            {
                SoundManager.instance.ChangeEffects(8);
                LobbyManager.instance.GetGiftCoin();
                DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.menu, 4);
            }

            if(curState == CommonState.buyBall || curState == CommonState.buyMonster || curState == CommonState.InternetConnect)
            {
                LobbyManager.instance.curPopup = beforePopup;
            }

            if (curState == CommonState.PreBall)
            {
                if (beforePopup != null) LobbyManager.instance.curPopup = beforePopup;
                else LobbyManager.instance.curPopup = null;
            }
        }
        else if (curState == CommonState.buyPackage)
        {
            Time.timeScale = 1;
        }

        curState = CommonState.none;
        beforePopup = null;
        SetData = 0;
        SubData = 0;
        for (int i = 0; i < myBtnArr.Length; ++i)
        {
            myBtnArr[i].SetActive(false);
        }
    }

    public void YesBtnClick()
    {
        if (isLobby)
        {
            switch (curState)
            {
                case CommonState.InternetConnect:
                case CommonState.GameCenterLogin:
                    ClosePupTPTS();
                    return;
                case CommonState.buyCoin:
                    DataManager.instance.CallPurchaser(SetData, SubData);
                    break;
                case CommonState.coinLack:
                    LobbyManager.instance.CallShopView();
                    break;
                case CommonState.buyBall:
                    LobbyManager.instance.ChargeAndUsedCoin(-SetData);
                    LobbyManager.instance.SetBallSelect(SubData, false, true);
                    if (DataManager.instance.charBallList.Contains((int)ShopState.PreUse))
                        LobbyManager.instance.BallSkillInfoDataSet(SubData, ShopState.UnUse);
                    else
                        LobbyManager.instance.BallSkillInfoDataSet(SubData, ShopState.Use);
                    if (DataManager.instance.AutoSave)
                        DataManager.instance.CallGameDataSave();
                    switch (SubData)
                    {
                        case 4:
                            DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.use_ruby, 11);
                            break;
                        case 5:
                            DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.use_ruby, 12);
                            break;
                        case 3:
                            DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.use_ruby, 13);
                            break;
                        case 1:
                            DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.use_ruby, 14);
                            break;

                    }
                    break;
                case CommonState.buyMonster:
                    LobbyManager.instance.ChargeAndUsedCoin(-SetData);
                    LobbyManager.instance.SetBallSelect(SubData, false, true);
                    if (DataManager.instance.charBallList.Contains((int)ShopState.PreUse))
                        LobbyManager.instance.BallSkillInfoDataSet(SubData, ShopState.UnUse);
                    else
                        LobbyManager.instance.BallSkillInfoDataSet(SubData, ShopState.Use);
                    if (DataManager.instance.AutoSave)
                        DataManager.instance.CallGameDataSave();
                    switch (SubData)
                    {
                        case 4:
                            DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.use_ruby, 11);
                            break;
                        case 5:
                            DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.use_ruby, 12);
                            break;
                        case 3:
                            DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.use_ruby, 13);
                            break;
                        case 1:
                            DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.use_ruby, 14);
                            break;

                    }
                    break;
                case CommonState.GetCoinAds:
                    SoundManager.instance.ChangeEffects(8);
                    LobbyManager.instance.WatchGetCoinAds();
                    DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.menu, 3);
                    break;
                case CommonState.GetGift:
                    break;
                case CommonState.buyPackage:
                    DataManager.instance.CallPurchaser(SetData, SubData);
                    break;
                case CommonState.PreBall:
                    LobbyManager.instance.WatchPreCharBallAds(SetData);
                    break;
                case CommonState.SweetAimPre:
                    LobbyManager.instance.WatchPreSweetAim();
                    break;
                case CommonState.SweetAimBuy:
                    DataManager.instance.CallPurchaser(6, 0);
                    break;
                case CommonState.NoadsBuy:
                    DataManager.instance.CallPurchaser(12, 0);
                    break;
            }
            ClosePupTPTS();
            LobbyManager.instance.CallLoadingPup();
        }
        else
        {
            switch (curState)
            {
                case CommonState.buyCoin:
                    UIManager.instance.ClickRewardBtn1();
                    break;
                case CommonState.buyBall:
                    UIManager.instance.ClickRewardBtn2();
                    break;
                case CommonState.GetCoinAds:
                    UIManager.instance.WatchGetCoinAds();
                    break;
                case CommonState.GetGift:
                    UIManager.instance.GetGiftCoin();
                    DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.game_result, 18);
                    break;
                case CommonState.buyPackage:
                    UIManager.instance.PlusBallAd();
                    break;
            }
            ClosePupTPTS();
            UIManager.instance.CallLoadingPup();
        }
    }

    public void NoBtnClick()
    {
        ClosePupTPTS();
    }
}
