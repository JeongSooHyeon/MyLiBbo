using UnityEngine;

public class LobbyTipsPupControl : PopupBase
{
    [SerializeField] GameObject[] StepObjArr;
    int curIdx;

    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        LobbyManager.instance.curPopup = this;
        curIdx = DataManager.instance.LobbyTutoStep;
        StepObjArr[curIdx].SetActive(true);
    }

    public void GameStartClick()
    {
        base.ClosePupTPTS();
        StepObjArr[0].SetActive(false);
        DataManager.instance.LobbyTutoStep = 1;
        Invoke("GameStart", 0.2f);
    }

    void GameStart()
    {
        SoundManager.instance.TapSound(1);
        DataManager.instance.CurGameStage = 1;
        DataManager.instance.GameStartCall();
    }

    public override void ClosePupTPTS()
    {
        base.ClosePupTPTS();
        StepObjArr[2].SetActive(false);
        LobbyManager.instance.curPopup = null;
        DataManager.instance.LobbyTutorial = true;
        LobbyManager.instance.CallLoadingPup();
        LobbyManager.instance.DailyMissionCall();
    }

    public void ClickNext()
    {
        StepObjArr[1].SetActive(false);
        DataManager.instance.LobbyTutoStep = 2;
        StepObjArr[2].SetActive(true);
    }
}
