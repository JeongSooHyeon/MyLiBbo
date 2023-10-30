using System.Collections.Generic;
using UnityEngine;

public class StageSetControl : MonoBehaviour
{
    readonly string[] bgImgArr = { "Btn_Gray", "Btn_Brown", "Btn_White" };
    [SerializeField] List<StageBtnControl> StageBtnList;
    [SerializeField] UISprite myHousingPiece;
    [SerializeField] UISlider myStarProgressBar;
    [SerializeField] UILabel LevelNums;
    [SerializeField] UILabel StarCountNum;
    [SerializeField] GameObject[] RewardBtnObjs;
    [SerializeField] UISprite stageSetBG;

    public StageStarState myChapterState;
    public StagePupControl myStagePup;
    public int curChapterNum;
    public int StageStarAllCount;

    public void SetData()
    {
        StageStarAllCount = 0;
        int stageStar;
        for (int i = StageBtnList.Count - 1; i >= 0; --i)
        {
            int sIdx = (i + 1) + ((curChapterNum - 1) * 20);
            stageStar = DataManager.instance.GetStageStarList(sIdx - 1);
            StageStarAllCount += stageStar;
            if(sIdx == 1)
            {
                if(DataManager.instance.GetStageClearList(sIdx - 1) == false)
                    StageBtnList[i].SetState(true, sIdx, stageStar);
                else
                    StageBtnList[i].SetState(DataManager.instance.GetStageClearList(sIdx - 1), sIdx, stageStar);
            }
            else
                StageBtnList[i].SetState(DataManager.instance.GetStageClearList(sIdx - 1), sIdx, stageStar);
        }
        SetChatperData();

        
        if (DataManager.instance.languageState == Language.Korean)
            LevelNums.text = string.Format("스테이지 {0}-{1}", StageBtnList[0].myIdx, StageBtnList[19].myIdx);
        else if (DataManager.instance.languageState == Language.Japanese)
            LevelNums.text = string.Format("ステージ {0}-{1}", StageBtnList[0].myIdx, StageBtnList[19].myIdx);
        else LevelNums.text = string.Format("STAGE {0}-{1}", StageBtnList[0].myIdx, StageBtnList[19].myIdx);
        myStarProgressBar.value = StageStarAllCount / 60f;
        StarCountNum.text = string.Format("{0} / 60", StageStarAllCount);
        myHousingPiece.spriteName = string.Format("Housing{0:00}_Icon_{1:00}", ((curChapterNum - 1) / 10), ((curChapterNum - 1) % 10));
    }

    void SetChatperData()
    {
        int idx = 0;
        for (int i = 0; i < RewardBtnObjs.Length; ++i)
        {
            RewardBtnObjs[i].SetActive(false);
        }
        switch (myChapterState)
        {
            case StageStarState.collect:
                if (StageStarAllCount == 60)
                {
                    DataManager.instance.SetChapterStateList((curChapterNum - 1), (int)StageStarState.getreward);
                    if (curChapterNum < 50)
                    {
                        if (DataManager.instance.GetChapterStateList(curChapterNum) == StageStarState.none)
                        {
                            DataManager.instance.SetChapterStateList(curChapterNum, (int)StageStarState.collect);
                            Debug.Log(curChapterNum);
                        }
                    }
                    RewardBtnObjs[1].SetActive(true);
                    idx = 2;
                }
                else
                {
                    idx = 1;
                    RewardBtnObjs[0].SetActive(true);
                }
                break;
            case StageStarState.getreward:
                idx = 2;
                RewardBtnObjs[1].SetActive(true);
                break;
            case StageStarState.complete:
                idx = 2;
                RewardBtnObjs[2].SetActive(true);
                break;
            case StageStarState.none:
                RewardBtnObjs[0].SetActive(true);
                break;
        }

        if (myChapterState != StageStarState.none)
            RewardBtnObjs[(int)myChapterState].SetActive(true);
        stageSetBG.spriteName = bgImgArr[idx];
    }
    public void CallBtnSound()
    {
        SoundManager.instance.TapSound();
    }

    public void GetRewardBtns()
    {
        myStagePup.CallHousingRewardPup(myHousingPiece.spriteName, (curChapterNum - 1));
    }
}
