using UnityEngine;

public class DailyRewardControl : PopupBase
{
    [SerializeField] LobbyManager lobby;
    [SerializeField] GameObject[] rewardButtonArr;
    [SerializeField] GameObject[] rewardOffArr;
    [SerializeField] GameObject[] rewardSlot;
    [SerializeField] UIButton[] rewardSlotBtn;
    [SerializeField] GameObject[] rewardClear;
    [SerializeField] GameObject[] rewardOn;
    [SerializeField] GameObject[] rewardOff;
    [SerializeField] UILabel coolTimeLabel;
    public bool isFirstCall;

    void Start()
    {
        ResetRewardPup();
        SetButtonState();
    }
    void Update()
    {
        if (!DataManager.instance.GetDailyRewardTime())
        {
            CoolTime();
        }
        else
        {
            SetButtonState();
        }
    }

    public void ClickDailyRewardGet()
    {
        if(rewardButtonArr[0].activeSelf)
        {
            LobbyManager.instance.CallDRCommonPup();
        }
    }

    public void ClickADRewardGet()
    {
        if (rewardButtonArr[1].activeSelf)
        {
            if (AdsManager.instance.isRewardLoad(adsType.dailyReward))
                AdsManager.instance.ShowRewardedAd(adsType.dailyReward);
            else
                LobbyManager.instance.CallAdsMesPup();
        }
        DataManager.instance.DailyRewardAD = true;
        LobbyManager.instance.DR_ADChangeCurPup();
    }

    public void ClickRewardSlot()
    {
        if (rewardButtonArr[0].activeSelf)
        {
            LobbyManager.instance.CallDRCommonPup();
        }
        else if(!rewardButtonArr[0].activeSelf && rewardButtonArr[1].activeSelf)
        {
            if (AdsManager.instance.isRewardLoad(adsType.dailyReward))
            {
                AdsManager.instance.ShowRewardedAd(adsType.dailyReward);
                DataManager.instance.DailyRewardAD = true;
            }
            else
                LobbyManager.instance.CallAdsMesPup();
        }
    }

    public void ResetRewardPup()
    {
        for(int i = 0; i < rewardSlot.Length; i++)
        {
            rewardSlotBtn[i].enabled = (i == DataManager.instance.DailyRewardCount);
            rewardClear[i].SetActive(i < DataManager.instance.DailyRewardCount);
            rewardOn[i].SetActive(i == DataManager.instance.DailyRewardCount);
            rewardOff[i].SetActive(!rewardOn[i].activeSelf);
            LobbyManager.instance.DailyStateCheck();
        }
    }

    public void SetButtonState()
    {
        if(DataManager.instance.GetDailyRewardTime())
        {
            LobbyManager.instance.DailyStateCheck();
            DataManager.instance.DailyRewardAD = false;
            rewardButtonArr[0].SetActive(true);
            rewardOffArr[0].SetActive(false);
            rewardButtonArr[1].SetActive(false);
            rewardOffArr[1].SetActive(true);
        }
        else if(!DataManager.instance.GetDailyRewardTime() && !DataManager.instance.DailyRewardAD)
        {
            rewardButtonArr[0].SetActive(false);
            rewardOffArr[0].SetActive(true);
            rewardButtonArr[1].SetActive(true);
            rewardOffArr[1].SetActive(false);
        }
        else
        {
            rewardButtonArr[0].SetActive(false);
            rewardOffArr[0].SetActive(true);
            rewardButtonArr[1].SetActive(false);
            rewardOffArr[1].SetActive(true);
        }
    }

    void CoolTime()
    {
        System.DateTime dt2 = UnbiasedTime.Instance.Now();
        if((24 <= dt2.Hour))
        {
            coolTimeLabel.text = "";
        }
        else
        {
            coolTimeLabel.text = string.Format("{0:00}:{1:00}:{2:00}", (23 - dt2.Hour), (59- dt2.Minute), (59- dt2.Second));
        }
    }

    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        lobby.curPopup = this;
    }

    public override void ClosePupTPTS()
    {
        base.ClosePupTPTS();
        if (!isFirstCall)
        {
            LobbyManager.instance.CloseOptionAndMenuBack();
            LobbyManager.instance.DailyStateCheck();
        }
            
        else
            lobby.curPopup = null;
    }
}
