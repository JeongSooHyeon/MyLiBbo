using UnityEngine;

public class PinBallManager : MonoBehaviour
{
    public static PinBallManager instance;
    [SerializeField] PinBallHit[] pinBalls_;
    int plusCnt;
    private void Awake()
    {
        instance = this;
        plusCnt = 3;
    }

    public void SetShowBall()
    {
        int[] ranCnt_ = new int[plusCnt];
        int cnt_ = 0;
        int ran_ = Random.Range(0, plusCnt);
       
        for(int i = 0; i < pinBalls_.Length; ++i)
        {
            if(ranCnt_[cnt_] == i)
            {
                ++cnt_;
                pinBalls_[i].SetShwoBall(true);
            }
        }
        ++plusCnt;
        if (plusCnt >= pinBalls_.Length) plusCnt = 3;
    }
}
