using UnityEngine;

public class PuzzlePupControl : PopupBase
{
    [SerializeField] Animator myAnim;
    [SerializeField] PuzzleRewardPupControl RewardPup;
    public int rewardIdx;
    public bool isAniPlay;
    [SerializeField] HouCompleteControl aniEvent;
    [SerializeField] GameObject[] cha;
    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        LobbyManager.instance.curPopup = this;
        foreach (GameObject a in cha) a.SetActive(false);
        myAnim.SetTrigger("Start");
        isAniPlay = true;
        aniEvent.idx = rewardIdx;
        SoundManager.instance.ChangeEffects(10);
    }

    public void ClickRewardGet()
    {
        isAniPlay = false;
        ClosePupTPTS();
        RewardPup.CallPupTPTS();
    }

    public override void ClosePupTPTS()
    {
        if (isAniPlay) return;
        base.ClosePupTPTS();
        LobbyManager.instance.curPopup = null;
        myAnim.SetTrigger("End");
    }
}
