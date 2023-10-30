using UnityEngine;

public class HouInfoControl : PopupBase
{
    [SerializeField] PuzzleSetItemControl housingView;
    [SerializeField] GameObject[] rewardList;
    int idx;

    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        foreach (GameObject a in rewardList) a.SetActive(false);
        idx = housingView.idx;
        rewardList[idx].SetActive(true);
        LobbyManager.instance.curPopup = this;
    }

    public override void ClosePupTPTS()
    {
        base.ClosePupTPTS();
        rewardList[idx].SetActive(false);
        LobbyManager.instance.curPopup = null;
    }
}
