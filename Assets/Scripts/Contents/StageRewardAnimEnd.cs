using UnityEngine;

public class StageRewardAnimEnd : MonoBehaviour
{
    [SerializeField] StagePuzzleRewardPupControl pupControl;
    public void RewardFinished()
    {
        pupControl.isAniPlay = false;
        pupControl.ClosePupTPTS();
    }
}
