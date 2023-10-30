using UnityEngine;

public class InputPupControl : PopupBase
{
    [SerializeField] UIInput input;
    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        input.value = DataManager.instance.curStageIdx().ToString();
        LobbyManager.instance.curPopup = this;
    }

    public override void ClosePupTPTS()
    {
        base.ClosePupTPTS();
        LobbyManager.instance.curPopup = null;
    }

    public void ClickSet()
    {
        int setStage = int.Parse(input.value);
        if (setStage <= 0) setStage = 1;
        else if (setStage > DataManager.maxStageNum) setStage = DataManager.maxStageNum;

        for (int i = 0; i < setStage - 1; i++)
        {
            DataManager.instance.SetStageStarList(i, 3);
            DataManager.instance.SetStageClearList(i, 2);
        }

        for (int j = 999; j >= setStage - 1; j--)
        {
            DataManager.instance.SetStageStarList(j, 0);
            if (j == setStage - 1) DataManager.instance.SetStageClearList(j, 1);
            else DataManager.instance.SetStageClearList(j, 0);
        }

        int cur_index = (setStage - 1) / 20;
        DataManager.instance.SetChapterStateList(cur_index, 0);

        for (int i = DataManager.instance.stageChapterStateList.Count - 1; i > cur_index; i--)
        {
            DataManager.instance.SetChapterStateList(i, -1);
        }

        for (int i = 0; i < cur_index; i++)
        {
            DataManager.instance.SetChapterStateList(i, 1);
        }

        for (int i = 0; i < cur_index / DataManager.housingItem; i++)
        {
            DataManager.instance.SetHousingStateList(i, 10);
        }

        for (int i = (cur_index / DataManager.housingItem) + 1; i < DataManager.housingCount; i++)
        {
            DataManager.instance.SetHousingStateList(i, -1);
        }

        DataManager.instance.TotalStarCount = 3 * (setStage - 1);
        DataManager.instance.CurGameStage = setStage;
        LobbyManager.instance.HousingReset();
        LobbyManager.instance.SetTotalStarCount();
        LobbyManager.instance.CheckStarRewardState(DataManager.instance.GetLobbyRewardCount() <= DataManager.instance.TotalStarCount);
        LobbyManager.instance.TestSetCurStageTxt();
        LobbyManager.instance.HousingCheck();
        LobbyManager.instance.SetMapPos();
        DataManager.instance.ResetStageStarList(DataManager.instance.CurGameStage);
        ClosePupTPTS();
    }
}
