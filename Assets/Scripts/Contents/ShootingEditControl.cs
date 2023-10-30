using UnityEngine;

public class ShootingEditControl : PopupBase
{
    [SerializeField] UIInput[] input;

    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        LobbyController.instance.SetPup(UIState.ShootingEdit);

        input[0].value = BallManager.instance.ballCnt.ToString();
        input[1].value = BallManager.instance.attack.ToString();
        input[2].value = BallManager.instance.fireTime.ToString("N2");
        input[3].value = BallManager.instance.force.ToString("N2");
        input[4].value = "1";
        input[5].value = LobbyController.instance.tweenTime.ToString("N2");
    }

    public override void ClosePupTPTS()
    {
        base.ClosePupTPTS();
        LobbyController.instance.SetPup(UIState.Pause);
    }

    public void ClickEdit()
    {
        if (int.Parse(input[0].value) < 1)  // 공 갯수
            BallManager.instance.SetShootingBallCnt(1);
        else if (int.Parse(input[0].value) > 1000)
            BallManager.instance.SetShootingBallCnt(1000);
        else BallManager.instance.SetShootingBallCnt(int.Parse(input[0].value));

        if (int.Parse(input[1].value) < 1)  // 공격력
            BallManager.instance.SetShootingAttack(1);
        else if (int.Parse(input[1].value) > 10000)
            BallManager.instance.SetShootingAttack(10000);
        else BallManager.instance.SetShootingAttack(int.Parse(input[1].value));

        if (float.Parse(input[2].value) < 0.5f)    // 발사 속도
            BallManager.instance.fireTime = 0.5f;
        else if (float.Parse(input[2].value) > 25) 
            BallManager.instance.fireTime = 25;
        else BallManager.instance.fireTime = float.Parse(input[2].value);

        if (float.Parse(input[3].value) < 100)    // 투사체 속도
            BallManager.instance.force = 100;
        else if (float.Parse(input[3].value) > 10000)
                    BallManager.instance.force = 10000;
                else BallManager.instance.force = (int)float.Parse(input[3].value);

        if (int.Parse(input[4].value) < 1)    // 벽돌 체력
            BrickManager.instance.brickCount = 1;
        else if (int.Parse(input[4].value) > 10000) 
            BrickManager.instance.brickCount = 10000;
        else BrickManager.instance.brickCount = int.Parse(input[4].value);

        if (float.Parse(input[5].value) < 0.1f)    // 벽돌 내려가는 속도
            BrickManager.instance.SetShootingBrickDown(0.1f);
        else if (float.Parse(input[5].value) > 10)
            BrickManager.instance.SetShootingBrickDown(10);
        else BrickManager.instance.SetShootingBrickDown(float.Parse(input[5].value)); 

        ClosePupTPTS();
        LobbyController.instance.SetPup(UIState.Pause);
    }
}
