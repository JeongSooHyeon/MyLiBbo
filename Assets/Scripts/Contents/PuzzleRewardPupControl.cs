using UnityEngine;

public class PuzzleRewardPupControl : PopupBase
{
    [SerializeField] GameObject[] RewardObjSetArr;
    [SerializeField] PuzzlePupControl myHousingPup;
    [SerializeField] PuzzleSetItemControl myHousingView;

    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        LobbyManager.instance.curPopup = this;
        ResetReward();
        foreach (GameObject a in RewardObjSetArr) a.SetActive(false);
        RewardObjSetArr[myHousingPup.rewardIdx].SetActive(true);
        SoundManager.instance.ChangeEffects(0);
    }

    void ResetReward()
    {
        for(int i=0;i < RewardObjSetArr.Length; ++i)
        {
            RewardObjSetArr[i].SetActive(false);
        }
    }

    public override void ClosePupTPTS()
    {
        DataManager.instance.HousingRewardCheck(myHousingPup.rewardIdx);
        DataManager.instance.SetHousingStateList(myHousingPup.rewardIdx, 11);
        if (myHousingPup.rewardIdx < 4)
        {
            if (DataManager.instance.GetHousingStateList(myHousingPup.rewardIdx + 1) < 0)
                DataManager.instance.SetHousingStateList(myHousingPup.rewardIdx + 1, 0);
        }
        base.ClosePupTPTS();
        myHousingView.initData();
        myHousingView.NewSignCheck();
        LobbyManager.instance.curPopup = null;
        if(!Application.isEditor)
            DataManager.instance.CallGameDataSave();
    }
}
