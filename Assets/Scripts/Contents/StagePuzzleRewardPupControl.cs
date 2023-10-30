using UnityEngine;

public class StagePuzzleRewardPupControl : PopupBase
{
    [SerializeField] Animator myAni;
    [SerializeField] UISprite myImg;
    [SerializeField] StagePupControl myStage;
    [SerializeField] PuzzleSetItemControl myHousingView;
    public string imgName;
    public int ChapterNum;
    public bool isAniPlay;

    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        isAniPlay = true;
        myStage.ClosePupTPTS();
        myAni.SetTrigger("Start");
        myImg.spriteName = imgName;
        LobbyManager.instance.curPopup = this;
    }

    public override void ClosePupTPTS()
    {
        if (isAniPlay) return;
        DataManager.instance.SetChapterStateList(ChapterNum, 2);
        if(DataManager.instance.GetChapterStateList(ChapterNum + 1) == StageStarState.none)
            DataManager.instance.SetChapterStateList(ChapterNum + 1, 0);
        int data = DataManager.instance.GetHousingStateList((ChapterNum / 10));
        if (data == -1) data = 0;
        DataManager.instance.SetHousingStateList((ChapterNum / 10), data + 1);
        myHousingView.initData();
        myHousingView.NewSignCheck();
        LobbyManager.instance.MapStateCheck();
        LobbyManager.instance.DailyStateCheck();
        base.ClosePupTPTS();
        myStage.CallPupTPTS();
    }
}
