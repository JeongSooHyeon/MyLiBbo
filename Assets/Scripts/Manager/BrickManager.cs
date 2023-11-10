using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BrickManager : MonoBehaviour
{
    public static BrickManager instance;

    // 스케일 구분을 위한 인덱스 값
    public int bIndex;
    public int bCount;

    public List<BrickCount> bundleList;
    public List<Items> holeList;
    public int brickCount, moreCnt_, increseCnt = 2;
    public int brickCombo = 0;
    [SerializeField] int counts_;
    bool isOver;
    public bool isOneMore;
    public bool continueThisStage = false;
    public BrickCount lastMoreCheck;
    [SerializeField] bool[] isEndGame;
    public bool isStage, isClear;
    [SerializeField] Animator[] lineAnim_;
    [SerializeField] GameObject[] bossStage;
    [SerializeField] Transform bossTrans;
    public GameObject warningSprite;
    [SerializeField] SideWall wallCheck;
    public float roTime = 2f;
    public int undoScore = 0;
    [SerializeField] int bossCnt;
    public int addLineNumber = 0;
    int TopbundleNum = 0;
    [SerializeField] ParticleSystem continueFx;
    public List<BossControl> m_Boss;
    [HideInInspector] public int saveComboNum;

    [SerializeField] TweenPosition boss_Pos;

    [SerializeField] float[] bossMovePos;
    float bossMoveY;

    // 브릭에 의한 위험 상태 판단 불값
    [SerializeField] bool isBrickWarn = false;

    // 폭탄 딜레이 상황 판단
    public bool isAllBomb = false;
    public List<Brick> allBombBrickList;

    // 볼 회수된 것을 판단하기 위한 불
    bool isAutoResetBall;

    // 슬라이드 블럭 정지 처리를 위한 리스트
    public List<TweenPosition> slideBrickTP;

    int lastBrickIdx;

    void Awake()
    {
        instance = this;

        if (m_Boss == null)
        {
            m_Boss = new List<BossControl>();
        }
        else
        {
            m_Boss.Clear();
        }

        if (allBombBrickList == null)
        {
            allBombBrickList = new List<Brick>();
        }
        else
        {
            allBombBrickList.Clear();
        }

        if (slideBrickTP == null)
        {
            slideBrickTP = new List<TweenPosition>();
        }
        else
        {
            slideBrickTP.Clear();
        }
    }

    private void Start()
    {
        if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
        {
            for (int i = 0; i < bundleList.Count; i++)
            {
                bundleList[i].SetTweenDuration(LobbyController.instance.tweenTime);
            }
        }
        UIManager.instance.GetPanel(GetComponent<UIPanel>());
        UIManager.instance.BrickPanelSetting();
    }

    private void OnEnable()
    {
        // 현재 스테이지 볼 카운트 세팅

        if (DataManager.instance.CurGameMode == (int)GameMode.STAGE)
        {
            // 현재 스테이지 별 점수 세팅
            DataManager.instance.SetStageStarScore();
        }

        Invoke("DelayStart", 0.1f);

        isAutoResetBall = false;
    }

    private void Update()
    {
        if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
        {
            if (LobbyController.instance.timer >= 60f)
            {
                LobbyController.instance.tweenTime = LobbyController.instance.tweenTime * 0.9f;
                for (int i = 0; i < bundleList.Count; i++)
                {
                    bundleList[i].SetTweenDuration(LobbyController.instance.tweenTime);
                }
                LobbyController.instance.timer = 0f;
            }
        }
    }

    void DelayStart()
    {
        switch ((GameMode)DataManager.instance.CurGameMode)
        {
            case GameMode.CLASSIC:
            case GameMode.SHOOTING:
                int correctionValue = bIndex == 2 ? 3 : 2;
                bCount = 9 + 6 * bIndex;

                SetUpCount(DataManager.instance.GetSaveFile());

                if (!DataManager.instance.ShootingTuto && DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
                {
                    for (int i = 0; i < bundleList.Count; i++)
                    {
                        bundleList[i].tPos.enabled = false;
                        BallManager.instance.SetFire(false);
                    }
                }
                break;
            case GameMode.STAGE:
            case GameMode.BALL100:
                SetStageMode();
                break;
            case GameMode.STAGEBOSS:
                break;
        }

    }

    public void SetBoss()
    {
        ++bossCnt;
    }

    public void EndGameBoss()
    {
        --bossCnt;
        if (bossCnt <= 0)
        {
            isClear = true;
            isAutoResetBall = true;
            SoundManager.instance.ChangeEffects(20);

            RecallBall();
            //BrickClear();
            SetBrickGray(0);
        }
    }


    public void Init()
    {
        brickCount = 0;
        counts_ = 0;
    }

    void SetStageMode()
    {
        int correctionValue = bIndex == 2 ? 3 : 2;
        isStage = true;
        for (int i = 0; i < bundleList.Count - correctionValue; ++i)
        {
            bundleList[i].ShowStageBrick();
        }
        counts_ = 0;
        for (int i = 0; i < holeList.Count; ++i)
        {
            holeList[i].OnHoleTarget(holeList[i == 0 ? 1 : 0]);
        }

        bCount = 9 + 6 * bIndex;
    }




    public void DeleteHoleItem()
    {
        for (int i = 0; i < holeList.Count; ++i)
        {
            holeList[i].BottomTrue();
        }
    }

    void SetBossStage()
    {
        isStage = true;
        int stage_ = DataManager.instance.CurGameStage;

        int bossCnt = 0;
        if (stage_ > 100)
        {
            bossCnt = Mathf.FloorToInt(stage_ * 0.01f);
        }
        stage_ = stage_ - (bossCnt * 100);
        if (stage_ == 0)
        {
            stage_ = 100;
        }
        bossMoveY = bossMovePos[stage_ / 10 - 1];
        GameObject go = Instantiate(bossStage[stage_ / 10 - 1]);
        Vector3 vec_ = go.transform.localPosition;
        go.transform.parent = bossTrans;
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = vec_;
    }

    // 새로운 보스 생성 함수
    public void CreateBossPrefab(int index, Transform transform, int hp = 0, int bCount = 0)
    {
        GameObject bossPrefab = Instantiate(bossStage[index]);
        Transform bossTranform = bossPrefab.transform;

        bossTranform.parent = transform;
        bossTranform.localScale = Vector3.one;
        bossTranform.localPosition = Vector3.zero;

        BossControl hitBoss = bossPrefab.GetComponentInChildren<BossControl>();

        hitBoss.bossHP = hp;
        hitBoss.posCount = bCount;

        StartCoroutine(DelayResetBossTransform(0.1f, bossPrefab));
    }

    IEnumerator DelayResetBossTransform(float time, GameObject boss)
    {
        yield return new WaitForSeconds(time);

        boss.transform.parent = bossTrans;
    }

    public void SetUpCount(bool isBool = false)
    {
        int bundleListCount = bundleList.Count - 1;
        if (isBool)
        {
            brickCount = DataManager.instance.GetSaveBrickCount();
            counts_ = DataManager.instance.GetSaveBrickPosCnt();
            int forCnt = 0;
            isOver = DataManager.instance.GetSaveBrickBool();
            isOneMore = DataManager.instance.GetOneMoreBool();
            moreCnt_ = DataManager.instance.GetSaveOnemoreCnt();
            if (DataManager.instance.GetSaveBrickBool())
            {
                forCnt = 9;
            }
            else
            {
                forCnt = counts_;
            }
            for (int i = 0; i < forCnt; ++i)
            {
                bundleList[i].ShowSaveBrick();
            }
        }
        else
        {
            if (DataManager.instance.CurGameMode != (int)GameMode.SHOOTING || !LobbyController.instance.editMode)
                ++brickCount;
            for (int i = 0; i < bundleList.Count; ++i)
            {
                ++bundleList[i].countNum;
            }

            if (!isStage)
            {
                bundleList[counts_].SetBrick();
                bundleList[counts_].MovingPos(347 + DataManager.instance.stagePosCorrection, 273 + DataManager.instance.stagePosCorrection, bIndex);
                bundleList[counts_].countNum = 0;
            }
            else
            {
                bundleList[bundleListCount - counts_].MovingPos(347 + DataManager.instance.stagePosCorrection, 273 + DataManager.instance.stagePosCorrection, bIndex);
                bundleList[bundleListCount - counts_].countNum = 0;
                if (isAddLineMore())
                {
                    bundleList[bundleListCount - counts_].AddLineBrick(addLineNumber);
                    ++addLineNumber;
                }
            }

            int MoveBundle = 0;
            for (int i = 0; i < bundleList.Count; ++i)
            {
                MoveBundle = i + TopbundleNum;
                if (MoveBundle > bundleListCount) MoveBundle -= bundleList.Count;
                bundleList[MoveBundle].MovingPos(bundleList[MoveBundle].transform.localPosition.y, bundleList[MoveBundle].transform.localPosition.y - 74, bIndex);
            }
            TopbundleNum = TopbundleNum == 0 ? bundleListCount : --TopbundleNum;
            for (int i = 0; i < bundleList.Count; ++i)
            {
                bundleList[i].SaveUndo();
            }

            // 보스 이동
            if (DataManager.instance.curMapDataList.MyGM == GameMode.STAGEBOSS && m_Boss.Count > 0)
            {
                if (brickCount % 2 == 0)
                {
                    for (int i = 0; i < m_Boss.Count; ++i)
                    {
                        m_Boss[i].MoveBoss(bCount + bIndex);
                    }

                    TurnWarningSpriteBoss();
                }
            }

            ++counts_;
            if (counts_ > bundleList.Count - 1)
            {
                counts_ = 0;
                isOver = true;
            }
            if (!isStage)
            {
                if (brickCount % 50 == 0) ++increseCnt;
                DataManager.instance.SetSaveBrickPosCnt(counts_);
                DataManager.instance.SetSaveBrickBool(isOver);
            }

            if (DataManager.instance.CurrentScore > DataManager.instance.GetBestScore())
            {
                DataManager.instance.SetBestScore();
            }
            undoScore = DataManager.instance.CurrentScore;
        }
    }

    public void EndGameCheck(int cnt, bool isTrue = false)
    {
        isEndGame[cnt] = isTrue;
        int mycnt = 0;
        for (int i = 0; i < isEndGame.Length; ++i)
        {
            if (!isEndGame[i]) ++mycnt;
        }
        if (mycnt >= isEndGame.Length)
        {
            if (DataManager.instance.curMapDataList.MyGM != GameMode.STAGEBOSS)
            {
                isClear = true;
                isAutoResetBall = true;
                RecallBall();
            }
            else
            {
                isClear = false;
            }
        }
        else isClear = false;
    }

    public void RecallBall()
    {
        StopCoroutine("DelayResetBall");
        StartCoroutine("DelayResetBall", 0.5f);
    }

    int bgCnt, bgChange;

    public void SetSaveSkillBrick()
    {
        for (int i = 0; i < bundleList.Count; ++i)
        {
            bundleList[i].SetSaveBlock();
        }
    }

    public void OnemoreSet()
    {
        for (int i = 0; i < bundleList.Count; ++i)
        {
            if (bundleList[i] == lastMoreCheck) bundleList[i].OnMoreCheck();
        }
        lastMoreCheck = null;
        DataManager.instance.SetSaveFile(true);
        isOneMore = true;
        DataManager.instance.SetOnemore(isOneMore);
        UIManager.instance.ResultOneMoreBtnDelete();
        DataManager.instance.isGameClear = true;
        LobbyController.instance.SetTween(UIState.ResultSuc, false);
    }
    int scores_ = 10, scoreMutiple;

    public void UpScore(bool undo = false)
    {
        ++scoreMutiple;
        if (undo) DataManager.instance.CurrentScore = undoScore;
        else if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
        {
            DataManager.instance.CurrentScore += scores_;
            //UIManager.instance.SetVolumeScore(scores_);
        }
        else
        {
            DataManager.instance.CurrentScore += scores_ * scoreMutiple;
            UIManager.instance.SetVolumeScore(scores_ * scoreMutiple);
        }
        UIManager.instance.SetScore();
        if ((GameMode)DataManager.instance.CurGameMode == GameMode.STAGE) UIManager.instance.CheckScoreStar();
    }

    public void ResetScore()
    {
        scoreMutiple = 0;
        brickCombo = 0;
    }

    public void ComboAnim(int num)
    {
        int comboBlend = Random.Range(0, 2);
        int nums_ = num % 2;

        lineAnim_[nums_].SetFloat("Blend", comboBlend);
        lineAnim_[nums_].SetTrigger("Combo_0" + (nums_ + 1));
        saveComboNum = num;
    }

    public void BottomBombBtn(bool eff = true)
    {
        for (int i = 0; i < bundleList.Count; ++i)
        {
            for (int j = 0; j < bCount; ++j)
            {
                bundleList[i].OnLaserLineBrick(j, 10);
            }
        }
        for (int i = 0; i < bundleList.Count; ++i)
        {
            bundleList[i].SaveUndo();
        }
        if (eff) GameContents.instance.gameitem(transform.position, 10);
        undoScore = DataManager.instance.CurrentScore;

        if (m_Boss.Count > 0)
        {
            for (int i = 0; i < m_Boss.Count; ++i)
            {
                m_Boss[i].HitBossCount(10);
            }
        }

        if (isClear)
        {
            if (DataManager.instance.CurGameMode == (int)GameMode.CLASSIC
                || (GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING || isAddLineMore())
            {
                if (!isAutoResetBall)  // 자동 회수 한 후에 실행하지 않도록
                {
                    SetUpCount();
                }
            }
            else
            {
                if (DataManager.instance.CurGameMode == (int)GameMode.BALL100)
                {
                    SetBrickGray(2);
                }
                else
                {
                    StartCoroutine("DelayResultSuccess", 1f);
                }
                return;
            }
        }
        else if (DataManager.instance.CurGameMode == (int)GameMode.CLASSIC)
        {
            if (DataManager.instance.CurrentScore > DataManager.instance.GetBestScore())
            {
                DataManager.instance.SetBestScore();
            }
        }

        isAutoResetBall = false;  // 자동 회수 확인 변수 초기화
    }

    IEnumerator DelayResultSuccess(float time)
    {
        yield return new WaitForSeconds(time);
        SetBrickGray(0);
    }

    public void BottomUndoBtn()
    {
        --UIManager.instance.startShow;
        saveComboNum = 0;
        isClear = false;
        SoundManager.instance.ChangeEffects(1);
        BallManager.instance.ResetBall(true);
        for (int i = 0; i < bundleList.Count; ++i)
        {
            bundleList[i].UndoBrickCount();
        }
        UpScore(true);
        UIManager.instance.SetMoney();
    }
    public void BottomeLineUpBtn()
    {
        if (bundleList[0].countNum >= 1)
        {
            SoundManager.instance.ChangeEffects(6);
            for (int i = 1; i < bundleList.Count; ++i)
            {
                bundleList[i].LineUpBrickCount();
            }
            bundleList[0].LineUpBrickCount();
        }
    }

    public void deleteBrickToContinue()
    {
        BallManager.instance.isDelay = false;
        LobbyController.instance.isResult = false;
        if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
        {
            for (int i = 0; i < bundleList.Count; i++)
            {
                bundleList[i].tPos.enabled = true;
            }
            BallManager.instance.SetFire(true);
        }
        UIManager.instance.ClickContinueBtn();
        for (int i = 0; i < bundleList.Count; ++i)
        {
            if (bundleList[i].countNum == bCount + bIndex || bundleList[i].countNum == bCount - 1 + bIndex || bundleList[i].countNum == bCount - 2 + bIndex)
            {
                SpecialBlock(BlockTypes.BombNormal_Hor, i, 0, bundleList[i].transform);
            }
        }
        continueFx.Play();
        for (int i = 0; i < bundleList.Count; ++i)
        {
            bundleList[i].SaveUndo();
        }
        LobbyController.instance.SetTween(UIState.ReStartGame, false);
        continueThisStage = true;
        wallCheck.isFirst = false;
        UIManager.instance.BottmSetting();
        undoScore = DataManager.instance.CurrentScore;
        if (isClear)
        {
            if (isAddLineMore())
            {
                SetUpCount();
            }
            else
            {
                if (DataManager.instance.CurGameMode == (int)GameMode.BALL100)
                {
                    SetBrickGray(2);
                }
                else
                {
                    SetBrickGray(0);
                }
                return;
            }
        }
    }

    public void SpecialBlock(BlockTypes type_, int BrickCountNum, int BrickNum, Transform pos = null) // Bomb의 타입과 위치값을 받아와 BrickCount에 전달(07.22)
    {
        switch (type_)
        {
            case BlockTypes.BombNormal_Hor:
                if (pos != null) GameContents.instance.gameitem(pos.position, 5);
                for (int i = 0; i < bCount; ++i)
                {
                    bundleList[BrickCountNum].OnBombLineBrick(i);
                }
                break;
            case BlockTypes.BombNormal_Ver:
                GameContents.instance.gameitem(pos.position, 6);
                for (int i = 0; i < bundleList.Count; ++i)
                {
                    bundleList[i].OnBombLineBrick(BrickNum);
                }
                break;
            case BlockTypes.BombNormal_Cro:
                GameContents.instance.gameitem(pos.position, 7);
                for (int i = 0; i < bundleList.Count; ++i)
                {
                    if (i == BrickCountNum)
                    {
                        for (int j = 0; j < bCount; ++j)
                        {
                            bundleList[i].OnBombLineBrick(j);
                        }
                    }
                    else
                    {
                        bundleList[i].OnBombLineBrick(BrickNum);
                    }
                }
                break;
            case BlockTypes.BombNormal_Box:
                GameContents.instance.gameitem(pos.position, 9);
                for (int i = BrickCountNum - 1; i < BrickCountNum + 2; ++i)
                {
                    int a;

                    if (i <= -1) a = bundleList.Count - 1;
                    else if (i >= bundleList.Count) a = 0;
                    else a = i;

                    for (int j = BrickNum - 1; j < BrickNum + 2; ++j)
                    {
                        if (j >= 0 && j < bCount)
                        {
                            bundleList[a].OnBombLineBrick(j);
                        }
                    }
                }
                break;
            case BlockTypes.BombNormal_XCro:
                GameContents.instance.gameitem(pos.position, 8);
                for (int i = 0; i < bundleList.Count; ++i)
                {
                    int a = bundleList[BrickCountNum].countNum - bundleList[i].countNum > 0 ? bundleList[BrickCountNum].countNum - bundleList[i].countNum : bundleList[i].countNum - bundleList[BrickCountNum].countNum;
                    if (BrickNum - a >= 0) bundleList[i].OnBombLineBrick(BrickNum - a);
                    if (BrickNum + a < bCount) bundleList[i].OnBombLineBrick(BrickNum + a);
                }
                break;
            case BlockTypes.Speaker:
                for (int i = BrickCountNum - 1; i < BrickCountNum + 2; ++i)
                {
                    int a;

                    if (i <= -1) a = bundleList.Count - 1;
                    else if (i >= bundleList.Count) a = 0;
                    else a = i;

                    for (int j = BrickNum - 1; j < BrickNum + 2; ++j)
                    {
                        if (j >= 0 && j < bCount)
                        {
                            if (a != BrickCountNum || j != BrickNum)
                            {
                                bundleList[a].OnLaserLineBrick(j);
                            }
                        }
                    }
                }
                break;
            case BlockTypes.BombMini:  // 소형 폭탄
                GameContents.instance.gameitem(pos.position, 9); // 이펙트 재생 33이 마지막임, 이펙트 추가해줘야 함
                SoundManager.instance.ChangeEffects(20, 1f);    // 효과음

                for (int i = BrickCountNum - 1; i < BrickCountNum + 2; ++i) // 내 윗줄, 아랫줄
                {
                    int a;  // 번들 idx

                    if (i <= -1) a = bundleList.Count - 1;  // 내가 0번째 번들이면, 맨 밑 번들
                    else if (i >= bundleList.Count) a = 0;  // 내가 맨 밑 번들이면, 맨 위 번들
                    else a = i;

                    // j:brick idx
                    for (int j = BrickNum - 1; j < BrickNum + 2; ++j)   // 내 왼쪽 Brick부터 오른쪽 Brick까지
                    {
                        if (j >= 0 && j < bCount)   // 0부터 번들갯수 범위
                        {
                            if (a != BrickCountNum || j != BrickNum)    // 내 번들이 아니거나, 내 Brick 번호가 아니면
                            {
                                bundleList[a].OnBombLineBrick(j);
                            }
                        }
                    }
                }
                break;
            case BlockTypes.BombAll:
                StartCoroutine(DelayBomb(1f, pos));
                break;
            default:
                break;
        }
    }

    IEnumerator DelayBomb(float time, Transform pos)
    {
        yield return new WaitForSecondsRealtime(time);
        if (pos != null) GameContents.instance.gameItemAllBomb(33);
        SoundManager.instance.ChangeEffects(20, 1f);

        for (int i = 0; i < bundleList.Count; ++i)
        {
            for (int j = 0; j < bCount; ++j)
            {
                bundleList[i].OnBombLineBrick(j);
            }
        }
    }

    public void Laser(Items.ItemInfo type_, int BrickCountNum, int BrickNum) // Bomb의 타입과 위치값을 받아와 BrickCount에 전달(07.22)
    {
        switch (type_)
        {
            case Items.ItemInfo.Width:
                for (int i = 0; i < bCount; ++i)
                {
                    if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING) 
                        bundleList[BrickCountNum].OnLaserLineBrick(i, BallManager.instance.attack);
                    else bundleList[BrickCountNum].OnLaserLineBrick(i);
                }
                break;
            case Items.ItemInfo.Height:
                for (int i = 0; i < bundleList.Count; ++i)
                {
                    if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING) 
                        bundleList[i].OnLaserLineBrick(BrickNum, BallManager.instance.attack);
                    else bundleList[i].OnLaserLineBrick(BrickNum);
                }
                break;
            case Items.ItemInfo.Cross:
                for (int i = 0; i < bundleList.Count; ++i)
                {
                    if (i == BrickCountNum)
                    {
                        for (int j = 0; j < bCount; ++j)
                        {
                            if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING) 
                                bundleList[i].OnLaserLineBrick(j, BallManager.instance.attack);
                            else bundleList[i].OnLaserLineBrick(j);
                        }
                    }
                    else
                    {
                        if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING) 
                            bundleList[i].OnLaserLineBrick(BrickNum, BallManager.instance.attack);
                        else bundleList[i].OnLaserLineBrick(BrickNum);
                    }
                }
                break;
            case Items.ItemInfo.Xcross:
                for (int i = 0; i < bundleList.Count; ++i)
                {
                    int a = BrickCountNum - bundleList[i].countNum > 0 ? BrickCountNum - bundleList[i].countNum : bundleList[i].countNum - BrickCountNum;
                    if (BrickNum - a >= 0)
                    {
                        if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING) 
                            bundleList[i].OnLaserLineBrick(BrickNum - a, BallManager.instance.attack);
                        bundleList[i].OnLaserLineBrick(BrickNum - a);
                    }
                    if (BrickNum + a < bCount)
                    {
                        if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING) 
                            bundleList[i].OnLaserLineBrick(BrickNum + a, BallManager.instance.attack);
                        bundleList[i].OnLaserLineBrick(BrickNum + a);
                    }
                }
                break;
            default:
                break;
        }
        for (int i = 0; i < bundleList.Count; ++i)
        {
            for (int j = 0; j < bundleList[i].bricks_.Length; ++j)
            {
                if ((bundleList[i].bricks_[j].types_ > BlockTypes.TriBounce && bundleList[i].bricks_[j].types_ < BlockTypes.slideBlock_1) || bundleList[i].bricks_[j].types_ == BlockTypes.BombAll)
                {
                    bundleList[i].bricks_[j].takeDamage = false;
                }
            }
        }
    }

    public void LockBlock(BlockTypes type_, BlockColorType blockColor, int BrickCountNum, int BrickNum, int hit, bool isLock, Items.ItemInfo item_ = Items.ItemInfo.None)
    {
        int nextBrickCount = BrickCountNum == 0 ? bundleList.Count - 1 : BrickCountNum - 1;
        if (bundleList[BrickCountNum].countNum == 1)
        {
            bundleList[nextBrickCount].firstMov = true;

        }
        bundleList[nextBrickCount].lockBrick(type_, blockColor, BrickNum, hit, isLock, item_);
        if (hit > 0 && item_ == Items.ItemInfo.None) isEndGame[bundleList[nextBrickCount].brick_idx_] = true;
    }

    public void JumpBlcok(BlockTypes type_, BlockColorType blockColor, int BrickCountNum, int BrickNum, int hit, Items.ItemInfo item_ = Items.ItemInfo.None)
    {
        int nextBrickCount = BrickCountNum == bundleList.Count - 1 ? 0 : BrickCountNum + 1;
        bundleList[nextBrickCount].lockBrick(type_, blockColor, BrickNum, hit, false, item_);
        if (hit > 0 && item_ == Items.ItemInfo.None) isEndGame[bundleList[nextBrickCount].brick_idx_] = true;
    }

    public void TurnWarningSprite(int i)
    {
        isBrickWarn = isEndGame[i];

        warningSprite.SetActive(CheckBossWarning() || isBrickWarn);
    }

    public void TurnWarningSpriteBoss()
    {
        warningSprite.SetActive(CheckBossWarning() || isBrickWarn);
    }

    public bool CheckBossWarning()
    {
        bool isWarn = false;

        if (m_Boss.Count > 0)
        {
            for (int j = 0; j < m_Boss.Count; ++j)
            {
                if (m_Boss[j].mWarning)
                {
                    isWarn = true;
                    break;
                }
            }
        }

        return isWarn;
    }

    public bool isAddLineMore()
    {
        return addLineNumber < DataManager.instance.GetCurStageAddLineCount();
    }

    public bool isJumpBricksOnLockBrick(int brickNum, int brickCountIdx, int bundleCountNum)
    {
        int checkBundleNum = brickCountIdx;
        for (int i = 0; i < bundleCountNum; ++i)
        {
            --checkBundleNum;
            if (checkBundleNum < 0) checkBundleNum = bundleList.Count - 1;
            if (bundleList[checkBundleNum].LockBrickMax(brickNum) == 0)
            {
                return true;
            }
        }
        return false;
    }

    public int NextBrickCheck(int brickNum, int BrickCount)
    {
        int checkBundleNum = BrickCount == 0 ? bundleList.Count - 1 : BrickCount - 1;
        return bundleList[checkBundleNum].LockBrickMax(brickNum);
    }

    public void NextBossStep()
    {
        boss_Pos.from.y = boss_Pos.to.y;
        boss_Pos.to.y -= bossMoveY;
        boss_Pos.ResetToBeginning();
        boss_Pos.PlayForward();
    }

    // 블럭이 다 깨졌을 때 회수하는 코루틴
    IEnumerator DelayResetBall(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        BallManager.instance.ResetBall();
    }

    public void EndBrickCountMove()
    {
        if ((GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING)
        {
            SetUpCount();
        }
    }

    public void SetShootingBrickDown(float time)
    {
        LobbyController.instance.tweenTime = time;
        for (int i = 0; i < bundleList.Count; i++)
        {
            bundleList[i].SetTweenDuration(LobbyController.instance.tweenTime);
        }
        LobbyController.instance.timer = 0f;
    }

    public void SetBrickGray(int num, float time = 1f)
    {
        BallManager.instance.SetSpeedUpOff();
        if(num !=0) SoundManager.instance.ChangeEffects(14);
        StartCoroutine(DelayBrickGray(num, time));
    }

    IEnumerator DelayBrickGray(int num, float time)
    {
        if (DataManager.instance.CurGameMode != (int)GameMode.CLASSIC && DataManager.instance.CurGameMode != (int)GameMode.SHOOTING)
        {
            for (int i = bundleList.Count - 1; i >= 0; i--)
            {
                if (isEndGame[i] && num != 0)
                {
                    yield return new WaitForSeconds(0.05f);
                    bundleList[i].ResetSprite();
                    bundleList[i].SetGray();
                }
                else if (num == 0)
                {
                    bundleList[i].FalseBrick(true);
                }
            }

            for(int i = 0; i < m_Boss.Count; ++i)
            {
                if(m_Boss[i] != null && m_Boss[i].bossHP > 0)
                {
                    yield return new WaitForSeconds(0.01f);

                    m_Boss[i].SetGray();
                }
            }
        }
        else
        {
            for (int i = 0; i < bundleList.Count; i++)
            {
                if (bundleList[i].countNum == bundleList[i].bricks_.Length + 1)
                    lastBrickIdx = i;
            }
            for (int i = lastBrickIdx; i < bundleList.Count; i++)
            {
                yield return new WaitForSeconds(0.05f);
                bundleList[i].ResetSprite();
                bundleList[i].SetGray();
            }

            for (int i = 0; i < lastBrickIdx; i++)
            {
                yield return new WaitForSeconds(0.05f);
                bundleList[i].ResetSprite();
                bundleList[i].SetGray();
            }
        }
        UIManager.instance.SetDelayResult(num, time);
    }

    public void BrickClear(/*bool eff = true*/)
    {
        for (int i = 0; i < bundleList.Count; ++i)
        {
            for (int j = 0; j < bundleList[i].bricks_.Length; ++j)
            {
                if (bundleList[i].bricks_[j] != null)
                    bundleList[i].bricks_[j].FalseMySelf();
            }
        }
        /*for (int i = 0; i < bundleList.Count; ++i)
        {
            bundleList[i].SaveUndo();
        }
        if (eff) GameContents.instance.gameitem(transform.position, 10);
        undoScore = DataManager.instance.CurrentScore;

        if (m_Boss.Count > 0)
        {
            for (int i = 0; i < m_Boss.Count; ++i)
            {
                m_Boss[i].HitBossCount(10);
            }
        }

        if (isClear)
        {
            if (DataManager.instance.CurGameMode == (int)GameMode.CLASSIC
                || (GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING || isAddLineMore())
            {
                if (!isAutoResetBall)  // 자동 회수 한 후에 실행하지 않도록
                {
                    SetUpCount();
                }
            }
            else
            {
                if (DataManager.instance.CurGameMode == (int)GameMode.BALL100)
                {
                    SetBrickGray(2);
                }
                else
                {
                    StartCoroutine("DelayResultSuccess", 1f);
                }
                return;
            }
        }
        else if (DataManager.instance.CurGameMode == (int)GameMode.CLASSIC)
        {
            if (DataManager.instance.CurrentScore > DataManager.instance.GetBestScore())
            {
                DataManager.instance.SetBestScore();
            }
        }

        isAutoResetBall = false;  // 자동 회수 확인 변수 초기화*/
    }
}
