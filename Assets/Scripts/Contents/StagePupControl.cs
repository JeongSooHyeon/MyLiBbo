using System.Collections.Generic;
using UnityEngine;

public class StagePupControl : PopupBase
{
    [SerializeField] public List<StageSetControl> myStageSetList;
    [SerializeField] StagePuzzleRewardPupControl myHousingRewardPup;
    //[SerializeField] UITable[] myTables;
    [SerializeField] UITable myTable;
    //[SerializeField] UIScrollView[] myPanel;
    [SerializeField] UIScrollView myPanel;
    [SerializeField] GameObject[] btns;
    [HideInInspector] public int table_idx;
    [HideInInspector] public readonly int stageCount = 200;
    [HideInInspector] readonly int maxStagePage = 5;
    [SerializeField] UIDragScrollView myDragZone;
    [SerializeField] UILabel stagePage;

    private void Start()
    {
        //LobbyManager.instance.SetMapPos();
        initList((DataManager.instance.GetCurrentChapterIdx() + 1 / 10));
        table_idx = (DataManager.instance.curStageIdx() - 1) / 200;
        SetIndex(1, table_idx);
        LobbyManager.instance.MapStateCheck();
        SetPage();
    }

 
    void initList(int idx) // 리스트 데이터 넣기
    {
        myStageSetList.Clear();

        for (int i = 0; i < myTable.GetChildList().Count; ++i)
        {
            myStageSetList.Add(myTable.GetChildList()[i].GetComponent<StageSetControl>());
            myStageSetList[i].myStagePup = this;
        }

        SetData(idx);
    }
    void SetData(int idx)  // 데이터 셋팅
    {
        int num = 0;
        for (int i = myStageSetList.Count - 1; i >= 0; --i) // 9 ~ 0
        {
            myStageSetList[i].curChapterNum = (num + 1) + (idx * stageCount / 20);  // 역순으로 1~10 + 10 * 테이블
            if ((DataManager.instance.curStageIdx() / 20) > num + (10 * idx) && DataManager.instance.GetChapterStateList(num + (10 * idx)) == StageStarState.none)
                if (DataManager.instance.curStageIdx() % 20 != 0)
                {
                    DataManager.instance.SetChapterStateList(num, 0);
                    Debug.Log(num);
                }

            myStageSetList[i].myChapterState = DataManager.instance.GetChapterStateList(myStageSetList[i].curChapterNum - 1);
            myStageSetList[i].SetData();
            ++num;
            BtnSet();
        }
    }

    public override void CallPupTPTS()  // 팝업 호출
    {
        table_idx = (DataManager.instance.curStageIdx() - 1) / 200;
        SetIndex(1, table_idx);
        base.CallPupTPTS();
        LobbyManager.instance.curPopup = this;
        BtnSet();
        SetPage();
        LobbyManager.instance.MapStateCheck();
        ResetScroll(table_idx);
        
    }

    public void ResetScroll(int idx)   // 스크롤 초기화
    {
        if (idx == (DataManager.instance.curStageIdx() - 1) / 200)
        {
            float posY = 1f - ((float)DataManager.instance.GetCurrentChapterIdx() / 9f);
            myPanel.SetDragAmount(0, posY, false);
        }
        else myPanel.SetDragAmount(0, 1, false);
    }

    public override void ClosePupTPTS() // 팝업 닫기
    {
        base.ClosePupTPTS();
        //foreach (UIScrollView a in myPanel) a.gameObject.SetActive(false);
        LobbyManager.instance.curPopup = null;
    }

    public void CallHousingRewardPup(string img, int cNum)   // 퍼즐 보상 팝업
    {
        myHousingRewardPup.ChapterNum = cNum;
        myHousingRewardPup.imgName = img;
        myHousingRewardPup.CallPupTPTS();
        SoundManager.instance.ChangeEffects(19);
    }

    public void RepositionCall()    // 테이블 리포지션
    {
        //myTables[table_idx].Reposition();
        myTable.Reposition();
    }

    public void NextBtn()
    {
        if (table_idx < maxStagePage - 1)
        {
            table_idx++;
            SetIndex(0, table_idx);
        }
        BtnSet();
        SetPage();
        SoundManager.instance.TapSound();
    }

    public void PrevBtn()
    {
        if (table_idx > 0)
        {
            table_idx--;
            SetIndex(1, table_idx);
        }
        BtnSet();
        SetPage();
        SoundManager.instance.TapSound();
    }

    public void BtnSet()
    {
        btns[0].SetActive(table_idx > 0);
        btns[1].SetActive(table_idx < maxStagePage - 1);
        btns[2].SetActive(!btns[0].activeSelf);
        btns[3].SetActive(!btns[1].activeSelf);
    }

    public void DragActive(int num)
    {
        //myDragZone.scrollView = myPanel[num];
        myDragZone.scrollView = myPanel;
    }

    //public void SetIndex(int btnNum, int idx)
    //{
    //    if (!btns[btnNum].activeSelf)
    //        btns[btnNum].SetActive(true);
    //    foreach (UIScrollView a in myPanel) a.gameObject.SetActive(false);
    //    myPanel[idx].gameObject.SetActive(true);
    //    initList(idx);
    //    ResetScroll(idx);
    //    DragActive(idx);
    //}
    public void SetIndex(int btnNum, int idx)
    {
        if (!btns[btnNum].activeSelf) btns[btnNum].SetActive(true);
        SetData(idx);
        ResetScroll(idx);
        DragActive(idx);
    }

    void SetPage()
    {
        stagePage.text = string.Format("{0} / {1}", table_idx + 1, maxStagePage);
    }

    
}
