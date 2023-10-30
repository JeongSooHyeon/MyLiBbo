using UnityEngine;

public class DailyRewardNoticePupControl : PopupBase
{
    [SerializeField] UISprite myRewardImg;
    [SerializeField] UILabel myRewardCount;
    [SerializeField] string[] myImgArrr;
    [SerializeField] string[] RewardCountArr;
    PopupBase beforePopup;
    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        if (LobbyManager.instance.curPopup != null)
            beforePopup = LobbyManager.instance.curPopup;
        LobbyManager.instance.curPopup = this;
        string rewardCon = "";
        rewardCon = RewardCountArr[DataManager.instance.DailyRewardCount];
        myRewardCount.text = rewardCon;
        myRewardImg.spriteName = myImgArrr[DataManager.instance.DailyRewardCount];
        myRewardImg.MakePixelPerfect();
    }

    public override void ClosePupTPTS()
    {
        base.ClosePupTPTS();
        LobbyManager.instance.GetDailyReward();
        LobbyManager.instance.curPopup = beforePopup;
        beforePopup = null;
        DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.daily_bonus, 1);
    }

    public void YesBtnClick()
    {
        ClosePupTPTS();
        LobbyManager.instance.CallLoadingPup();
    }
}
