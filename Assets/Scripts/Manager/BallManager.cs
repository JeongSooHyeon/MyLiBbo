using UnityEngine;
using System.Collections.Generic;

public class BallManager : MonoBehaviour
{
    public static BallManager instance;
    [SerializeField] float forceX;
    public float force;
    public List<BallMove> ballList;
    [HideInInspector] public bool isPlayGame = true;
    [SerializeField] GameObject ballInstant, bundleTrans, firstBall = null;
    public int ballHit, ballY;
    [SerializeField] public int ballCnt, upCountBall, plusBallCnt/*, colorIdx*/;
    float ballPosX, undoPosX = 0;
    public bool isArrow;
    bool isFire, isFirstShot = true, isSpeedUp;
    float halfWidth, forceY, fireDelay = 0, delaySpeedUp;
    public bool isUndo_ = false;
    public bool startboss = false;
    public float roTime = 2;
    BoxCollider2D box_;
    [SerializeField] int fireCnt = 0;
    List<BallMove> delBall;
    [SerializeField] UISlider sd;
    public int ballSpriteCnt;
    public int monsterSpriteCnt;
    bool onPause = false;
    [SerializeField] _2D_Reflection rf2D;

    // 레이저 데미지 처리를 위해
    public const int normalHit = 1;

    // 슈팅모드
    [SerializeField] CannonControl cannon;
    public int money = 0;
    public int attack = 1;
    public float fireTime = 2f;

    // 슈팅모드 전용 bool
    public bool isMove;

    // 딜레이 전용 bool
    public bool isDelay;

    void Awake()
    {
        instance = this;
        if (bundleTrans == null) bundleTrans = gameObject;
    }


    void Start()
    {
        isPlayGame = true;
        // 선택된 볼 카운트 세팅
        if (DataManager.instance.PreBallSprite > 0)
        {
            ballSpriteCnt = DataManager.instance.PreBallSprite;
        }
        else
        {
            ballSpriteCnt = DataManager.instance.BallSprite;
        }
        // 선택된 몬스터 카운트 세팅
        if (DataManager.instance.PreSelectMoster > 0)
        {
            monsterSpriteCnt = DataManager.instance.PreSelectMoster;
        }
        else
        {
            monsterSpriteCnt = DataManager.instance.SelectMoster;
        }

        if ((GameMode)DataManager.instance.CurGameMode != GameMode.STAGE
            && (GameMode)DataManager.instance.CurGameMode != GameMode.STAGEBOSS) ballSpriteCnt = DataManager.instance.BallSprite;
        GameBallset();
        delBall = new List<BallMove>();
        box_ = GetComponent<BoxCollider2D>();

        money = 0;
        attack = 1;
        fireTime = 2f;

        UIManager.instance.SetShootingMoney(money);
        isMove = false;
        isFire = DataManager.instance.CurGameMode == (int)GameMode.SHOOTING;

        if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING) force = 680;
        else force = 1700;
    }

    public void Init()
    {
        for (int i = 0; i < ballList.Count; ++i)
        {
            ballList[i].StopBall();
        }
    }

    void StageStart()
    {
        plusBallCnt = DataManager.instance.maxStageBall - 1;
        for (int i = 0; i < plusBallCnt; ++i)
        {
            GameObject go = Instantiate(ballInstant);
            go.transform.parent = bundleTrans.transform;
            go.transform.localScale = Vector2.one;
            go.transform.localPosition = firstBall.transform.localPosition;
        }
        upCountBall = 0;
        ballCnt += plusBallCnt;
        UIManager.instance.SetBallCount(ballCnt);
        firstBall = null;
        plusBallCnt = 0;
        System.GC.Collect();
    }

    void GameBallset()
    {
        GameObject go = Instantiate(ballInstant);
        go.transform.parent = bundleTrans.transform;
        go.transform.localScale = Vector2.one;
        go.transform.localPosition = new Vector2(0, ballY);
        firstBall = go;
        ballPosX = firstBall.transform.localPosition.x;
        UIManager.instance.SetCharPosSetting();
    }

    public float RetunrnFirstBall()
    {
        if (isUndo_) ballPosX = undoPosX;
        return ballPosX;
    }

    void AddUpgradeBall()
    {
        plusBallCnt = DataManager.instance.StartBall - 1;
        for (int i = 0; i < plusBallCnt; ++i)
        {
            GameObject go = Instantiate(ballInstant);
            go.transform.parent = bundleTrans.transform;
            go.transform.localScale = Vector2.one;
            go.transform.localPosition = new Vector3(0, ballY, 0);
        }
        ballCnt += plusBallCnt;
        UIManager.instance.SetBallCount(ballCnt);
        plusBallCnt = 0;
        System.GC.Collect(); // 이건 쓰레기 값을 버리는 코드 입니다. 이때는 잠시 멈출 테니까..
    }

    void OnPress(bool isBool)
    {
        if (!isDelay && LobbyController.instance.getGameState() == UIState.InGame)
        {
            if ((GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING)
            {
                isMove = isBool;
                rf2D.CheckLineOn(true);
                isArrow = isBool;
                if (!isFire && LobbyController.instance.getGameState() == UIState.InGame)
                    isFire = true;
                UIManager.instance.ArrowSpriteSetting(false, forceX);
            }
            else
            {
                rf2D.CheckLineOn(true);
                if (isPlayGame) return;
                isArrow = isBool;
                if (!isPlayGame) UIManager.instance.BottomWallTrue(!isBool);
                if (onPause)
                {
                    onPause = !onPause;
                    UIManager.instance.SetSprite(isArrow, ballList[0].transform.localPosition);
                    return;
                }
                if (!isBool)
                {
                    UIManager.instance.SetTouchSprite(new Vector3(1000, 1000, 0));
                    if (forceY >= 150f)
                    {
                        isSliderFire = true;
                        SetFire();
                    }
                }
                UIManager.instance.SetSprite(isArrow, ballList[0].transform.localPosition);
            }
        }
        else
        {
            rf2D.CheckLineOn(false);
            isArrow = false;
            UIManager.instance.BottomWallTrue(true);
            UIManager.instance.SetTouchSprite(new Vector3(1000, 1000, 0));
            UIManager.instance.SetSprite(isArrow, ballList[0].transform.localPosition);
        }
    }

    public void SetFire()
    {
        if (!isSliderFire) return;
        isFire = true;
        isSpeedUp = true;
        isPlayGame = true;
        box_.enabled = false;
        firstBall = null;
        isSliderFire = false;
        UIManager.instance.ArrowSpriteSetting(false, forceX);
        UIManager.instance.BottomWallTrue(true);
        UIManager.instance.ShowDownBtn(true);
        sd.value = 0.5f;
    }

    public void ResetBall(bool undo = false)
    {
        isUndo_ = undo;
        if ((Monster)monsterSpriteCnt == Monster.hipChoco && isFirstShot)
        {
            if (firstBall == null)
            {
                firstBall = ballList[0].gameObject;
                firstBall.transform.localPosition = new Vector2(0, ballY);
            }
        }
        for (int i = 0; i < ballList.Count; ++i)
        {
            if (isUndo_ || ballList[i].gameObject != firstBall)
            {
                if (i == 0)
                {
                    ballList[i].isFirst = true;
                    ballList[i].spriteOn();
                }
                ballList[i].StopBall(true);
            }
        }
        if (isUndo_) UIManager.instance.SetCharPosSetting();
        fireCnt = 0;
        isPlayGame = false;
        Invoke("SetFalseGame", 0.2f);

        if ((GameMode)DataManager.instance.CurGameMode != GameMode.SHOOTING)
        {
            isFire = false;
            LastSetting();
        }
    }


    void FixedUpdate()
    {
        if (isFire)
        {
            if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
            {
                fireDelay += Time.deltaTime;
                if (fireDelay >= 1 / fireTime)
                {
                    WaitFires();
                    fireDelay = 0;
                }
            }
            else
            {
                fireDelay += Time.deltaTime;
                if (fireDelay >= 0.04f)
                {
                    WaitFires();
                    fireDelay = 0;
                }
            }
        }
        else if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
        {
            cannon.StopBall();
        }
        if (isSpeedUp)
        {
            if (Time.timeScale > 0) delaySpeedUp += Time.fixedUnscaledDeltaTime;
            if (delaySpeedUp >= 2)
            {
                if (Time.timeScale >= 3f)
                {
                    Time.timeScale = 3f;
                    isSpeedUp = false;
                }
                else
                {
                    Time.timeScale += 1f;
                    UIManager.instance.SetSpeedPanel();
                }
                delaySpeedUp = 0;
            }
            return;
        }
    }

    private void LateUpdate()
    {
        if (isArrow)
        {
            Vector3 pos = UICamera.lastWorldPosition;
            Vector3 player_pos;
            if ((GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING)
            {
                if (ballList.Count != 0)
                    player_pos = ballList[0].transform.position;
                else player_pos = UIManager.instance.cannon.transform.position;
            }
            else player_pos = ballList[0].transform.position;
            Vector2 mouse_pos = new Vector2(pos.x - player_pos.x, pos.y - player_pos.y);
            float rad = Mathf.Atan2(mouse_pos.x, mouse_pos.y);

            forceX = Mathf.Clamp((rad * 180) / Mathf.PI, -80, 80);
            UIManager.instance.SetTouchSprite(UICamera.lastWorldPosition);
            forceY = Input.mousePosition.y;
            if (forceY >= 150)
            {
                UIManager.instance.ArrowSpriteSetting(true, -forceX);
                rf2D.CheckLineOn(true);
            }
            if (forceY < 150)
            {
                UIManager.instance.ArrowSpriteSetting(false, forceX);
                rf2D.CheckLineOn(false);
            }
        }

        if (isSliderSet)
        {

            if (isPlayGame) return;
            UIManager.instance.BottomWallTrue(!isSliderSet);
            forceX = (-sliderFloat * 170 + 80) * -1;
            forceX = Mathf.Clamp(forceX, -80, 80);
            forceY = Input.mousePosition.y;
            if ((GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING)
            {
                UIManager.instance.SetSprite(isSliderSet, new Vector3(0, -440, 0));
            }
            else
            {
                UIManager.instance.SetSprite(isSliderSet, ballList[0].transform.localPosition);
            }
            if (forceY >= 410f)
            {
                noneFire = true;
                isSliderFire = false;
                UIManager.instance.ArrowSpriteSetting(false, forceX);

            }
            else
            {
                isSliderFire = true;
                rf2D.CheckLineOn(true);
                UIManager.instance.ArrowSpriteSetting(isSliderSet, -forceX);
            }

        }
    }

    public void CancleFire()
    {
        onPause = true;
        UIManager.instance.ArrowSpriteSetting(false, forceX);

    }

    bool isSliderFire, isSliderSet, noneFire;
    float sliderFloat;
    public void SliderPos(bool isTrue, float sliderF)
    {
        rf2D.CheckLineOn(isTrue);
        if (isTrue && noneFire)
        {
            isSliderFire = false;
            isSliderSet = false;
            sd.value = 0.5f;
            return;
        }
        else if (!isTrue)
        {
            noneFire = false;
            isSliderFire = false;
            isSliderSet = false;
            sd.value = 0.5f;
        }

        sliderFloat = sliderF;
        isSliderSet = isTrue;
    }

    void WaitFires()
    {
        if ((GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING)
        {
            if (ballList.Count > 0)
            {
                cannon.FireBall();
                ballList[0].SetMove(-forceX, force);
                ballList.RemoveAt(0);
                UIManager.instance.SetBallCount(ballList.Count);
                SoundManager.instance.ChangeEffects(9);
            }
            else
            {
                cannon.StopBall();
            }
        }
        else
        {
            if (fireCnt < ballList.Count)
            {
                ballList[fireCnt].SetMove(-forceX, force);
                UIManager.instance.SetBallCount(ballCnt - (fireCnt + 1));
                ++fireCnt;
            }
            else
            {
                isFire = false;
                fireCnt = 0;
            }
        }
    }

    public void SetBall(GameObject ball)
    {
        if ((GameMode)DataManager.instance.CurGameMode != GameMode.SHOOTING)
        {
            if (firstBall == null)
            {
                firstBall = ball;
                isUndo_ = false;
                float fx = Mathf.Clamp(firstBall.transform.localPosition.x, -295, 295);
                firstBall.transform.localPosition = new Vector2(fx, firstBall.transform.localPosition.y);
                ballPosX = firstBall.transform.localPosition.x;
                UIManager.instance.SetCharPosSetting();
            }
            ++upCountBall;
            if (upCountBall == ballCnt)
            {
                isPlayGame = false;
                Invoke("SetFalseGame", 0.2f);

                LastSetting();
            }
        }
        else
        {
            --fireCnt;
            ballList.Add(ball.GetComponent<BallMove>());
            UIManager.instance.SetBallCount(ballList.Count);
        }

        for (int i = 0; i < ballList.Count; ++i)
        {
            if (ballList[i].gameObject == ball)
            {
                if (ball == firstBall) ballList[i].isFirst = true;
                ballList[i].StopBall();
            }
        }
    }

    void SetFalseGame() // invoke 함수
    {
        box_.enabled = true;
    }

    public void SetFirstBall()
    {
        isPlayGame = false;
        if ((GameMode)DataManager.instance.CurGameMode != GameMode.CLASSIC)
        {
            if ((Monster)monsterSpriteCnt != Monster.hipChoco)
            {
                StageStart();
            }
            else
            {
                if (DataManager.instance.curMapDataList.MyGM != GameMode.STAGE || BrickManager.instance.allBombBrickList.Count > 0)
                {
                    StageStart();
                }
            }
        }

        UIManager.instance.SetStartBall();
        ballList[0].GetComponent<UISprite>().enabled = true;

        if (DataManager.instance.CurGameMode == (int)GameMode.STAGE && DataManager.instance.curMapDataList.MyGM != GameMode.STAGEBOSS && BrickManager.instance.allBombBrickList.Count <= 0)
        {
            if (BrickManager.instance.allBombBrickList.Count > 0 && (Monster)monsterSpriteCnt == Monster.hipChoco)
            {
                return;
            }
        }
    }

    public void PlusBall(int i)
    {
        if ((GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING)
        {
            for (int x = 0; x < i; x++)
            {
                GameObject go = Instantiate(ballInstant);
                go.transform.parent = bundleTrans.transform;
                go.transform.localScale = Vector2.one;
                go.transform.localPosition = new Vector3(RetunrnFirstBall(), ballY, 0);
            }
        }
        else
        {
            plusBallCnt += i;
        }
    }
    public void SetSaveFileBall()
    {
        plusBallCnt = DataManager.instance.GetSaveBallCount() - 1; // datamanager에는 처음 볼까지 포함돼있기 때문에 한개를 빼준다.
        for (int i = 0; i < plusBallCnt; ++i)
        {
            GameObject go = Instantiate(ballInstant);
            go.transform.parent = bundleTrans.transform;
            go.transform.localScale = Vector2.one;
            go.transform.localPosition = firstBall.transform.localPosition;
        }
        upCountBall = 0;
        ballCnt += plusBallCnt;
        UIManager.instance.SetBallCount(ballCnt);
        firstBall = null;
        plusBallCnt = 0;
        System.GC.Collect();
    }

    public void ItemPlusBall()
    {
        if (delBall.Count > 0) return;
        float halfBallCount = ballList.Count / 2f;
        plusBallCnt = (int)(Mathf.Round(halfBallCount * 10) / 10);
        for (int i = 0; i < plusBallCnt; ++i)
        {
            GameObject go = Instantiate(ballInstant);
            delBall.Add(go.GetComponent<BallMove>());
            go.transform.parent = bundleTrans.transform;
            go.transform.localScale = Vector2.one;
            go.transform.localPosition = new Vector3(RetunrnFirstBall(), ballY, 0);
        }
        ballCnt += plusBallCnt;
        plusBallCnt = 0;
        UIManager.instance.SetBallCount(ballCnt);
    }

    public void AdPlusBall()
    {
        if (delBall.Count > 0) return;
        plusBallCnt = 20;
        for (int i = 0; i < plusBallCnt; ++i)
        {
            GameObject go = Instantiate(ballInstant);
            delBall.Add(go.GetComponent<BallMove>());
            go.transform.parent = bundleTrans.transform;
            go.transform.localScale = Vector2.one;
            go.transform.localPosition = new Vector3(RetunrnFirstBall(), ballY, 0);
        }
        ballCnt += plusBallCnt;
        plusBallCnt = 0;
        UIManager.instance.SetBallCount(ballCnt);
    }

    void CharPlusBall(int multiple = 0, bool notMulti = false)
    {
        if (delBall.Count > 0) return;
        if (notMulti)
        {
            plusBallCnt = multiple;
        }
        else
        {
            plusBallCnt = DataManager.instance.maxStageBall * (multiple - 1);
        }
        for (int i = 0; i < plusBallCnt; ++i)
        {
            GameObject go = Instantiate(ballInstant);
            delBall.Add(go.GetComponent<BallMove>());
            go.transform.parent = bundleTrans.transform;
            go.transform.localScale = Vector2.one;
            go.transform.localPosition = new Vector3(RetunrnFirstBall(), ballY, 0);
        }
        ballCnt += plusBallCnt;
        plusBallCnt = 0;
        UIManager.instance.SetBallCount(ballCnt);
    }

    void DelItemPlusBall()
    {
        for (int i = 0; i < delBall.Count; ++i)
        {
            Destroy(delBall[i].gameObject);
            ballList.Remove(delBall[i]);
        }
        ballCnt -= delBall.Count;
        delBall.Clear();
        UIManager.instance.SetBallCount(ballCnt);
    }

    public void LastSetting()
    {
        if (DataManager.instance.CurGameMode != (int)GameMode.CLASSIC
            && (GameMode)DataManager.instance.CurGameMode != GameMode.SHOOTING
            && BrickManager.instance.isClear && !BrickManager.instance.isAddLineMore())
        {
            if (DataManager.instance.CurGameMode == (int)GameMode.BALL100)
            {
                BrickManager.instance.SetUpCount();
                UIManager.instance.Continue(2);
            }
            else
            {
                if (!UIManager.instance.isResult)
                {
                    BrickManager.instance.SetBrickGray(0);
                }
            }
            return;
        }
        if ((GameMode)DataManager.instance.CurGameMode == GameMode.BALL100 && !isUndo_)
        {
            BrickManager.instance.SetUpCount();
            UIManager.instance.Continue(2);
        }

        BrickManager.instance.ResetScore();
        if (!isUndo_)
        {
            for (int i = 0; i < plusBallCnt; ++i)
            {
                GameObject go = Instantiate(ballInstant);
                go.transform.parent = bundleTrans.transform;
                go.transform.localScale = Vector2.one;
                go.transform.localPosition = new Vector3(RetunrnFirstBall(), ballY, 0);
            }
            if (delBall.Count > 0) DelItemPlusBall();
            ballCnt += plusBallCnt;
            if (firstBall == null) firstBall = ballList[0].gameObject;
            firstBall.GetComponent<BallMove>().isFirst = true;
            Init();
            if (!BrickManager.instance.isAllBomb)
            {
                BrickManager.instance.SetUpCount();
            }
            undoPosX = ballPosX;

            if (isFirstShot && DataManager.instance.CurGameMode == (int)GameMode.STAGE)
            {
                isFirstShot = false;
            }
        }
        ++UIManager.instance.startShow;
        UIManager.instance.ShowDownBtn(false);
        delaySpeedUp = 0;
        upCountBall = 0;
        UIManager.instance.SetBallCount(ballCnt);
        UIManager.instance.SetStartBall();
        UIManager.instance.BottmSetting();
        isSpeedUp = false;
        Time.timeScale = 1;
        firstBall = null;
        plusBallCnt = 0;
        BrickManager.instance.isAllBomb = false;
        System.GC.Collect(); // 이건 쓰레기 값을 버리는 코드 입니다. 이때는 잠시 멈출 테니까..
    }

    public void UsePlusItem()
    {
        PlusBall(1);
    }

    public void UseAttackUp(int att)
    {
        attack += att;
        ballHit = attack;
    }

    public void UseFireUp()
    {
        fireTime += 0.5f;
    }

    public void UseProjSpeedUp()
    {
        force += 100;
    }

    public void GetMoney(int m)
    {
        money += m;
        UIManager.instance.SetShootingMoney(money);
    }

    public void SetFire(bool b)
    {
        isFire = b;
    }

    public void SetShootingBallCnt(int cnt)
    {
        bundleTrans.transform.DestroyChildren();
        ballList.Clear();
        ballCnt = cnt;
        PlusBall(ballCnt);
    }

    public void SetShootingAttack(int att)
    {
        attack = att;
        ballHit = attack;
    }

    public void ShootingResetBall(bool fire = true)
    {
        isFire = fire;
        ballList.Clear();
        for (int i = 0; i < bundleTrans.transform.childCount; i++)
        {
            bundleTrans.transform.GetChild(i).GetComponent<BallMove>().StopBall();
            ballList.Add(bundleTrans.transform.GetChild(i).GetComponent<BallMove>());
        }
        UIManager.instance.SetBallCount(ballList.Count);
    }

    public void SetSpeedUpOff()
    {
        isSpeedUp = false;
        delaySpeedUp = 0;
        Time.timeScale = 1;
        isDelay = true;
        LobbyController.instance.isResult = true;
        isMove = false;
        isArrow = false;
        UIManager.instance.SetTouchSprite(false);
    }
}
