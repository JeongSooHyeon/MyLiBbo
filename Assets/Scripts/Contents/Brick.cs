using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Brick : MonoBehaviour
{
    public int idx_;
    public int hitCnt; // 맞아야할 횟수
    [SerializeField]
    UILabel label_; // 횟수를 보여줄 label입니다^_^
    public UISprite[] sprites_; // 색깔을 바꿔주기 위해서 찾아 놓은 Sprite입니다^_^
    [SerializeField]
    TweenAlpha[] tweenA;
    bool isIncre, isReset;
    [SerializeField] int brickStateCnt, triRandomPiece, increaseCnt;
    [SerializeField]
    Vector3[] triPos;
    [SerializeField]
    Vector2[] labelPos;
    public BrickCount brickCount_;
    public ParticleSystem ps_;
    public BlockTypes types_;
    public BlockColorType colorType;
    [SerializeField] TweenScale speaker_Scale;
    UISprite speaker_Sprite;
    public int b_CntIDX;

    // 전체 폭탄 글로우 앵커를 위한 변수
    [SerializeField] int normalAnchor;

    // 전체 폭탄 글로우 효과
    [SerializeField] GameObject allBombGlow;
    [SerializeField] TweenAlpha allBombTA;
    int myOriginHit;

    // 큰 크기 블록 대응을 위한 브릭 변수
    public Brick myOriginBrick;
    public BrickManager myBrickManager;
    public List<Brick> mySubBricks;

    // 아이템 처리를 위한 bool
    public bool takeDamage;

    [SerializeField] BoxCollider2D brickCollider;

    // 슬라이드 브릭을 움직이기 위한 
    [SerializeField] TweenPosition myTP;

    // 슈팅모드 코인을 위한 변수
    public int getCoin;

    private void Start()
    {
        getCoin = hitCnt;
    }

    public void LabelSetting(bool isBossStage = false) // 현재 나의 label을 셋팅해 줍니다.^_^ 
    {
        if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
            label_.text = UIManager.instance.TextChange(hitCnt);
        else label_.text = hitCnt.ToString();

        if (hitCnt > 0)
        {
            if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
                brickCount_.SetChildeHitCount(idx_, hitCnt, false);
            else brickCount_.SetChildeHitCount(idx_, hitCnt);

            if (types_ >= BlockTypes.blockS && mySubBricks != null)
            {
                for (int i = 1; i < mySubBricks.Count; ++i)
                {
                    mySubBricks[i].LabelSetting();
                }
            }
        }
        else if (types_ >= BlockTypes.blockS && myOriginBrick != this)
        {
            if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING || isBossStage)
                brickCount_.SetChildeHitCount(idx_, myOriginBrick.hitCnt, false);
            else brickCount_.SetChildeHitCount(idx_, myOriginBrick.hitCnt);
        }

        if ((GameMode)DataManager.instance.CurGameMode == GameMode.CLASSIC 
            || (GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING)
        {
            BrickColor();
        }
        else
        {
            StageBrickColor();
        }

    }

    public void StageSetting(int hit, BlockTypes bloc_, BlockColorType blockColor, BrickCount pa, int bIDX, ParticleSystem ps)
    {
        brickCount_ = pa;
        hitCnt = hit;
        myOriginHit = hit;
        types_ = bloc_;
        // 블럭 컬러 타입 추가
        colorType = blockColor != BlockColorType.none ? blockColor : BlockColorType.purple;
        b_CntIDX = bIDX;
        label_.gameObject.SetActive(true);
        label_.enabled = true;
        label_.leftAnchor.absolute = 0;
        label_.bottomAnchor.absolute = 0;
        myOriginBrick = null;
        // 다른 줄 블록에 접근하기 위해 참조
        myBrickManager = LobbyController.instance.brickManagers[DataManager.instance.stageSetIndex].GetComponent<BrickManager>();

        if (brickCount_ == null)
        {
            brickCount_ = GetComponentInParent<BrickCount>();
        }

        switch (types_)
        {
            case BlockTypes.Blank:
                gameObject.SetActive(false);
                break;
            case BlockTypes.Normal:
            case BlockTypes.BombNormal_Hor:
            case BlockTypes.BombNormal_Ver:
            case BlockTypes.BombNormal_XCro:
            case BlockTypes.BombNormal_Cro:
            case BlockTypes.BombNormal_Box:
                sprites_[0].gameObject.SetActive(true);
                sprites_[1].gameObject.SetActive(false);
                sprites_[2].gameObject.SetActive(false);
                label_.transform.localPosition = Vector2.zero;
                brickStateCnt = 0;
                break;
            case BlockTypes.Triangle1:
            case BlockTypes.Triangle2:
            case BlockTypes.Triangle3:
            case BlockTypes.Triangle4:
                TriSetting((int)types_ - 2);
                break;
            case BlockTypes.Fixed:
                sprites_[0].gameObject.SetActive(true);
                sprites_[1].gameObject.SetActive(false);
                sprites_[2].gameObject.SetActive(false);
                brickCount_.lockbrickNum = idx_;
                SpecialBrickSprite();
                label_.transform.localPosition = Vector2.zero;
                break;
            case BlockTypes.Speaker:
                sprites_[0].gameObject.SetActive(true);
                sprites_[1].gameObject.SetActive(false);
                sprites_[2].gameObject.SetActive(false);
                SpecialBrickSprite();
                break;
            case BlockTypes.TriBounce:
                sprites_[0].gameObject.SetActive(false);
                sprites_[1].gameObject.SetActive(false);
                sprites_[2].gameObject.SetActive(true);
                sprites_[2].GetComponent<TweenRotation>().duration = BrickManager.instance.roTime;
                brickCount_.lockbrickNum = idx_;
                hitCnt = 0;
                brickCount_.SetChildeHitCount(idx_, 0, false);
                label_.enabled = false;
                break;
            case BlockTypes.blockS:
            case BlockTypes.BombAll:
                SettingBigSizeBrick(2, 2);
                break;
            case BlockTypes.blockM:
                SettingBigSizeBrick(3, 3);
                break;
            case BlockTypes.blockL:
                SettingBigSizeBrick(4, 4);
                break;
            case BlockTypes.blockRetangle1:
                SettingBigSizeBrick(1, 2);
                break;
            case BlockTypes.blockRetangle2:
                SettingBigSizeBrick(2, 1);
                break;
            case BlockTypes.slideBlock_1:
                SettingSlideBrick(1, 2);
                break;
            case BlockTypes.slideBlock_2:
                SettingSlideBrick(1, 3);
                break;
            case BlockTypes.slideBlock_3:
                SettingSlideBrick(2, 1);
                break;
            case BlockTypes.slideBlock_4:
                SettingSlideBrick(3, 1);
                break;
        }

        gameObject.SetActive(true);

        LabelSetting();
        ps_ = ps;
    }

    // 큰 브릭 세팅
    void SettingBigSizeBrick(int width, int height)
    {
        if (hitCnt > 0)
        {
            sprites_[0].gameObject.SetActive(true);
            sprites_[1].gameObject.SetActive(false);
            sprites_[2].gameObject.SetActive(false);
            label_.transform.localPosition = Vector2.zero;
            brickStateCnt = 0;
            sprites_[0].height *= height;
            sprites_[0].width *= width;
            sprites_[3].height = sprites_[0].height;
            sprites_[3].width = sprites_[0].width;
            brickCollider.size = new Vector2(sprites_[0].width, sprites_[0].height);
            brickCollider.offset = new Vector2(sprites_[0].width / 2, -(sprites_[0].height / 2));

            if (mySubBricks != null)
            {
                mySubBricks.Clear();
            }
            else
            {
                mySubBricks = new List<Brick>();
            }

            if (types_ == BlockTypes.BombAll)
            {
                brickCollider.size = new Vector2(104, 104);
                SetSpriteAnchor(tweenA[brickStateCnt].GetComponent<UISprite>(), 0);
                allBombGlow.SetActive(true);
                allBombTA.duration = 1;
                tweenA[brickStateCnt].GetComponent<UISprite>().spriteName = "Sprite_Dynamite_Glow";

                BrickManager.instance.allBombBrickList.Add(this);
            }
            else
            {
                allBombGlow.SetActive(false);
                tweenA[brickStateCnt].GetComponent<UISprite>().spriteName = "Sprite_Brickbg";
            }

            myOriginBrick = this;
            mySubBricks.Add(this);
        }
        else
        {
            for (int i = brickCount_.brick_idx_; i > brickCount_.brick_idx_ - height; --i)
            {
                int lineCount = i < 0 ? 0 : i;

                for (int j = idx_; j > idx_ - width; --j)
                {
                    int brickCount = j < 0 ? 0 : j;

                    if (myBrickManager.bundleList[lineCount].bricks_[brickCount].hitCnt > 0 && myBrickManager.bundleList[lineCount].bricks_[brickCount].types_ == types_)
                    {
                        myOriginBrick = myBrickManager.bundleList[lineCount].bricks_[brickCount];
                        break;
                    }
                }
            }

            sprites_[0].gameObject.SetActive(false);
            label_.gameObject.SetActive(false);

            myOriginBrick.mySubBricks.Add(this);
        }

        takeDamage = false;
    }

    // 움직이는 브릭 세팅
    void SettingSlideBrick(int width, int height)
    {
        if (hitCnt > 0)
        {
            sprites_[0].gameObject.SetActive(true);
            sprites_[1].gameObject.SetActive(false);
            sprites_[2].gameObject.SetActive(false);
            label_.transform.localPosition = Vector2.zero;
            brickStateCnt = 0;

            myTP.from = transform.localPosition;
            myTP.to = new Vector3(transform.localPosition.x + (74 * (width - 1)), transform.localPosition.y - (74 * (height - 1)));

            float durationTime = width > height ? width : height;

            myTP.duration = durationTime;
            myTP.enabled = true;

            BrickManager.instance.slideBrickTP.Add(myTP);

            if (mySubBricks != null)
            {
                mySubBricks.Clear();
            }
            else
            {
                mySubBricks = new List<Brick>();
            }

            myOriginBrick = this;
            mySubBricks.Add(this);
        }
        else
        {
            for (int i = brickCount_.brick_idx_; i > brickCount_.brick_idx_ - height; --i)
            {
                int lineCount = i < 0 ? 0 : i;

                for (int j = idx_; j > idx_ - width; --j)
                {
                    int brickCount = j < 0 ? 0 : j;

                    if (myBrickManager.bundleList[lineCount].bricks_[brickCount].hitCnt > 0 && myBrickManager.bundleList[lineCount].bricks_[brickCount].types_ == types_)
                    {
                        myOriginBrick = myBrickManager.bundleList[lineCount].bricks_[brickCount];
                        break;
                    }
                }
            }

            sprites_[0].gameObject.SetActive(false);
            label_.gameObject.SetActive(false);

            myOriginBrick.mySubBricks.Add(this);
        }
    }

    public void LockBlockSetting()
    {
        if (types_ == BlockTypes.TriBounce)
        {
            BrickManager.instance.LockBlock(types_, colorType, b_CntIDX, idx_, 0, false);
            brickCount_.lockbrickNum = -1;
            if (types_ == BlockTypes.TriBounce) gameObject.SetActive(false);
        }
        else
        {
            bool maxcheck = BrickManager.instance.isJumpBricksOnLockBrick(idx_, b_CntIDX, brickCount_.countNum);
            int max = BrickManager.instance.NextBrickCheck(idx_, b_CntIDX);
            BrickManager.instance.LockBlock(types_, colorType, b_CntIDX, idx_, hitCnt, maxcheck);
            if (speaker_Sprite != null) speaker_Sprite.enabled = false;
            if (brickCount_.lockbrickNum == idx_) brickCount_.lockbrickNum = -1;
            if (max != 1 || maxcheck)
            {
                brickCount_.SetChildeHitCount(idx_, 0, false);
                brickCount_.brickSpecial[idx_] = 0;
                gameObject.SetActive(false);
            }
            StageBrickColor();
        }
    }

    public void jumpBounceBrick()
    {
        BrickManager.instance.JumpBlcok(types_, colorType, b_CntIDX, idx_, hitCnt);
        brickCount_.SetChildeHitCount(idx_, 0, false);
        brickCount_.brickSpecial[idx_] = 0;
        brickCount_.lockbrickNum = -1;
        gameObject.SetActive(false);
    }

    public void SetCollider(GameObject go) // 여기에 맞으면 때려줍니다.
    {
        if (go.CompareTag("Ball"))
        {
            if (types_ == BlockTypes.TriBounce)
            {
                brickCount_.SoundChanger();
            }
            else
            {
                if (hitCnt <= 0) return;
                if (types_ == BlockTypes.Speaker)
                {
                    SoundManager.instance.ChangeEffects(11);
                    speaker_Scale.ResetToBeginning();
                    speaker_Scale.PlayForward();
                    GameContents.instance.gameitem(transform.position, 4);
                    BrickManager.instance.SpecialBlock(types_, b_CntIDX, idx_, transform);
                }
                else
                {
                    brickCount_.SoundChanger();
                }

                hitCnt -= BallManager.instance.ballHit; // 이거는 나의 맞아야할 횟수를 깎아줍니다. 제너릭하다고 봐주세요~
                if (hitCnt > 0) alphaOn();

                if (hitCnt <= 0) // 암튼 그래~ 장그래~
                {
                    hitCnt = 0;
                    if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
                        BallManager.instance.GetMoney(getCoin);
                    if (DataManager.instance.curMapDataList.MyGM != GameMode.STAGEBOSS)
                    {
                        BrickManager.instance.UpScore();
                    }

                    Invoke("FalseMySelf", 0.02f);

                }
                else
                {
                    LabelSetting(); // 라벨을 보여주기 위해서
                    if (types_ == BlockTypes.BombAll)
                    {
                        allBombTA.duration = (float)hitCnt / (float)myOriginHit;
                    }
                }
            }
        }
    }

    public void OnLaser(int multiDamage = 1)
    {
        // 체력이 0이면 리턴
        if (hitCnt <= 0 && types_ < BlockTypes.blockS)
        {
            return;
        }

        if (types_ >= BlockTypes.blockS && myOriginBrick != null)
        {
            if (myOriginBrick.hitCnt <= 0)
            {
                return;
            }
        }


        // 스피커 타입일 때 아이템 효과 처리
        if (types_ == BlockTypes.Speaker)
        {
            speaker_Scale.ResetToBeginning();
            speaker_Scale.PlayForward();
            GameContents.instance.gameitem(transform.position, 4);
            BrickManager.instance.SpecialBlock(types_, b_CntIDX, idx_);
        }

        // 큰 크기 블럭일 경우 레이저가 모든 블럭을 때리지 않고 한 번만 하도록 bool 값 처리
        if ((types_ >= BlockTypes.blockS && types_ < BlockTypes.slideBlock_1) || types_ == BlockTypes.BombAll)
        {
            for (int i = 0; i < myOriginBrick.mySubBricks.Count; ++i)
            {
                if (myOriginBrick.mySubBricks[i].takeDamage == true)
                {
                    takeDamage = true;
                    break;
                }
            }
        }

        // 큰 크기 블럭의 서브들이 맞았을 때 본체가 데미지를 받도록 처리
        if (((types_ >= BlockTypes.blockS && types_ < BlockTypes.slideBlock_1) || types_ == BlockTypes.BombAll) && takeDamage == false && this != myOriginBrick && myOriginBrick.hitCnt > 0)
        {
            if (myOriginBrick != null)
            {
                myOriginBrick.OnLaser(multiDamage);
            }
            takeDamage = true;
        }
        // 일반 블럭과 큰 크기 블럭 본체의 경우에 처리
        else if (types_ < BlockTypes.blockS || this == myOriginBrick)
        {
            hitCnt -= BallManager.normalHit * multiDamage;
            if (hitCnt > 0) alphaOn();
            if (hitCnt <= 0)
            {
                hitCnt = 0;
                BrickManager.instance.UpScore();
                if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
                    BallManager.instance.GetMoney(getCoin);
                FalseMySelf();
            }
            else
            {
                LabelSetting();
            }
            brickCount_.SoundChanger();
        }
    }

    public void UndoBrick(int hit)
    {
        hitCnt = hit;
        if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
            brickCount_.SetChildeHitCount(idx_, hit, false);
        else brickCount_.SetChildeHitCount(idx_, hit);
        LabelSetting();
        if (types_ == BlockTypes.Fixed || types_ == BlockTypes.TriBounce)
        {
            brickCount_.lockbrickNum = idx_;
        }
        if (types_ == BlockTypes.Fixed || types_ == BlockTypes.Speaker)
        {
            SpecialBrickSprite();
        }
        gameObject.SetActive(true);
    }

    public void alphaOn() // 알파를 켜줍니다.
    {
        tweenA[brickStateCnt].ResetToBeginning();
        tweenA[brickStateCnt].PlayForward();
    }

    void TriSetting(int num)
    {
        sprites_[0].gameObject.SetActive(false);
        sprites_[1].gameObject.SetActive(true);
        sprites_[2].gameObject.SetActive(false);
        brickStateCnt = 1;
        sprites_[brickStateCnt].gameObject.transform.localEulerAngles = triPos[num];
        label_.leftAnchor.absolute = (int)labelPos[num].x;
        label_.bottomAnchor.absolute = (int)labelPos[num].y;
    }

    // 폭탄 처리를 위한 함수 추가
    public void OnBomb()
    {
        if ((types_ >= BlockTypes.blockS && types_ < BlockTypes.bluemon) || types_ == BlockTypes.BombAll)
        {
            for (int i = 0; i < myOriginBrick.mySubBricks.Count; ++i)
            {
                if (myOriginBrick.mySubBricks[i].takeDamage == true)
                {
                    takeDamage = true;
                    break;
                }
            }
        }

        if (((types_ >= BlockTypes.blockS && types_ < BlockTypes.bluemon) || types_ == BlockTypes.BombAll) && myOriginBrick != this)
        {
            if (myOriginBrick != null && takeDamage == false)
            {
                myOriginBrick.FalseMySelf();
                takeDamage = true;
            }
        }
        else if (types_ < BlockTypes.slideBlock_1)
        {
            FalseMySelf();
        }
    }

    public void FalseMySelf() // 여기는 꺼주는 역활을 합니다.
    {
        isReset = true;
        hitCnt = 0;
        if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
            brickCount_.SetChildeHitCount(idx_, 0, false);
        else brickCount_.SetChildeHitCount(idx_, 0);
        brickCount_.brickSpecial[idx_] = 0;
        // 큰 크기 블럭의 서브 블럭들이 다중 처리 되는 것 방지
        if (types_ >= BlockTypes.blockS && myOriginBrick != this)
        {
            types_ = BlockTypes.Blank;
            myOriginBrick = null;
            mySubBricks.Clear();
            gameObject.SetActive(false);

            return;
        }

        if ((int)types_ > 13)
        {
            BrickManager.instance.SpecialBlock(types_, b_CntIDX, idx_, transform); // 블록이 꺼질때 타입을 전달하며 Bomb 실행(07.22)
        }
        if (types_ == BlockTypes.Fixed)
        {
            brickCount_.lockbrickNum = -1;
        }
        if (speaker_Sprite != null) speaker_Sprite.enabled = false;
        if (ps_ != null)  // 파괴 이펙트 색상 지정해주기 위한 colorInt 값 - 컬러 타입 값으로 변경되도록 수정해야함 // 볼 마다 다른 이펙트로 수정
        {
            ps_.transform.position = new Vector3(transform.position.x + sprites_[0].width / 2, transform.position.y + sprites_[0].height / 2, transform.position.z);

            ps_.Play();
        }

        // 큰 크기 블럭 본체가 체력이 0이 될 때 다른 서브 블럭들의 체력 BrickCount의 brickInt 값 초기화 위해서 호출
        if (types_ >= BlockTypes.blockS && myOriginBrick == this)
        {
            for (int i = 1; i < mySubBricks.Count; ++i)
            {
                mySubBricks[i].FalseMySelf();
            }
            mySubBricks.Clear();
            tweenA[brickStateCnt].ResetToBeginning();

            if(types_ != BlockTypes.BombAll)
            {
                sprites_[0].height = 74;
                sprites_[0].width = 74;
                sprites_[3].height = sprites_[0].height;
                sprites_[3].width = sprites_[0].width;
                brickCollider.size = new Vector2(sprites_[0].width, sprites_[0].height);
                brickCollider.offset = new Vector2(sprites_[0].width / 2, -(sprites_[0].height / 2));
                myOriginBrick = null;
                types_ = BlockTypes.Blank;
            }
        }
        
        // 전체 폭탄 블럭 처리 딜레이
        if (types_ == BlockTypes.BombAll)
        {
            label_.text = "0";

            if(!BrickManager.instance.isAllBomb)
            {
                BrickManager.instance.isAllBomb = true;
                for(int i = 0; i < BrickManager.instance.allBombBrickList.Count; ++i)
                {
                    if(BrickManager.instance.allBombBrickList[i] != this)
                    {
                        BrickManager.instance.allBombBrickList[i].FalseMySelf();
                    }
                }
            }
            else
            {
                allBombTA.duration = 0.001f;
            }

            StartCoroutine("DelayObjectFalse", 1f);

        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator DelayObjectFalse(float time)
    {
        yield return new WaitForSecondsRealtime(time);

        sprites_[0].height = 74;
        sprites_[0].width = 74;
        sprites_[3].height = sprites_[0].height;
        sprites_[3].width = sprites_[0].width;
        brickCollider.size = new Vector2(sprites_[0].width, sprites_[0].height);
        brickCollider.offset = new Vector2(sprites_[0].width / 2, -(sprites_[0].height / 2));
        myOriginBrick = null;
        SetSpriteAnchor(tweenA[brickStateCnt].GetComponent<UISprite>(), normalAnchor);
        allBombGlow.SetActive(false);
        types_ = BlockTypes.Blank;
        tweenA[brickStateCnt].GetComponent<UISprite>().spriteName = "Sprite_Brickbg";

        UIManager.instance.ShowShockWave(transform.position);

        gameObject.SetActive(false);

        BrickManager.instance.RecallBall();
    }

    public void SetNext() // 이거는 나의 색상을 스테이지 마다 변화를 시켜주기 위해서 존재
    {
        if (isReset)
        {
            ResetBrick();
            isReset = false;
        }
        if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING && isIncre) isIncre = false;
        if (isIncre)
        {
            sprites_[0].color = Color.white;
            sprites_[0].spriteName = "Sprite_Brick_Rrecovery";
            sprites_[3].enabled = false;
        }
        else
        {
            sprites_[0].spriteName = "Sprite_Brick_Bg";
        }
    }

    void SpecialBrickSprite()
    {
        brickStateCnt = 0;
        string s = "";
        if (speaker_Sprite == null) speaker_Sprite = speaker_Scale.GetComponent<UISprite>();
        speaker_Sprite.enabled = true;
        switch (types_)
        {
            case BlockTypes.Speaker:
                s = "Sprite_Speaker";
                speaker_Sprite.width = 66;
                speaker_Sprite.height = 66;
                break;
            case BlockTypes.Fixed:
                s = "Sprite_FixBrick";
                speaker_Sprite.width = 68;
                speaker_Sprite.height = 68;
                break;
            default:
                speaker_Sprite.enabled = false;
                break;
        }
        speaker_Sprite.spriteName = s;

    }

    // 스테이지 모드 색상 정해주는 곳 컬러 타입을 받아서 세팅해주기
    void StageBrickColor()
    {
        string c = "";
        if (((int)types_ > 13 && (int)types_ < 23 && types_ != BlockTypes.Fixed) || types_ == BlockTypes.BombAll)
        {
            switch (types_)
            {
                case BlockTypes.BombNormal_Hor:
                    c = "Sprite_Bomb_Hor";
                    break;
                case BlockTypes.BombNormal_Ver:
                    c = "Sprite_Bomb_Ver";
                    break;
                case BlockTypes.BombNormal_Cro:
                    c = "Sprite_Bomb_Cross";
                    break;
                case BlockTypes.BombNormal_XCro:
                    c = "Sprite_Bomb_XCross";
                    break;
                case BlockTypes.BombNormal_Box:
                    c = "Sprite_Bomb_3x3";
                    break;
                case BlockTypes.BombAll:
                    c = "Sprite_Dynamite";
                    break;
            }
            sprites_[brickStateCnt + 3].enabled = false;
        }
        else
        {
            c = brickStateCnt == 0 ? "Sprite_Brick_Bg" : "Sprite_Tri_Bg";

            int colorIdx = 0;
            if (hitCnt > 99)
            {
                colorIdx = 2;
            }
            else if (hitCnt > 49)
            {
                colorIdx = 1;
            }
            else
            {
                colorIdx = 0;
            }

            sprites_[brickStateCnt].color = DataManager.instance.blockColorLists[(int)colorType].BlockColorList[colorIdx].BlockInColor;
            sprites_[brickStateCnt + 3].color = DataManager.instance.blockColorLists[(int)colorType].BlockColorList[colorIdx].BlockOutColor;
            sprites_[brickStateCnt + 3].enabled = true;
        }

        sprites_[brickStateCnt].spriteName = c;
    }

    void BrickColor()
    {
        if (isIncre || hitCnt == 0) return;
        float f = (float)hitCnt / BrickManager.instance.brickCount;
        string c = "";
        if (types_ >= BlockTypes.Hole_In && types_ <= BlockTypes.TriBounce && types_ != BlockTypes.Fixed)
        {
            switch (types_)
            {
                case BlockTypes.BombNormal_Hor:
                    c = "Sprite_Bomb_Hor";
                    break;
                case BlockTypes.BombNormal_Ver:
                    c = "Sprite_Bomb_Ver";
                    break;
                case BlockTypes.BombNormal_Cro:
                    c = "Sprite_Bomb_Cross";
                    break;
                case BlockTypes.BombNormal_XCro:
                    c = "Sprite_Bomb_XCross";
                    break;
                case BlockTypes.BombNormal_Box:
                    c = "Sprite_Bomb_3x3";
                    break;
            }
        }
        else
        {
            c = brickStateCnt == 0 ? "Sprite_Brick_Bg" : "Sprite_Tri_Bg";
        }

        int colorIdx;
        if (f > 1)
        {
            colorIdx = 0;
        }
        else if (f > 0.9f)
        {
            colorIdx = 1;
        }
        else if (f > 0.75f)
        {
            colorIdx = 2;
        }
        else if (f > 0.5f)
        {
            colorIdx = 3;
        }
        else if (f > 0.25f)
        {
            colorIdx = 4;
        }
        else
        {
            colorIdx = 5;
        }
        //sprites_[brickStateCnt].color = NGUIText.ParseColor(c); // 이거는 ngui함수입니다. 필요하시면 읽어보세요!
        sprites_[brickStateCnt].spriteName = c;

        sprites_[brickStateCnt].color = DataManager.instance.blockColorLists[colorIdx].BlockColorList[0].BlockInColor;
        sprites_[brickStateCnt + 3].color = DataManager.instance.blockColorLists[colorIdx].BlockColorList[0].BlockOutColor;
    }

    public void IncreseBrick()
    {
        if (isIncre && hitCnt > 0)
        {
            hitCnt += BrickManager.instance.increseCnt;
            LabelSetting();
        }
        if ((GameMode)DataManager.instance.CurGameMode == GameMode.CLASSIC
             || (GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING)
            BrickColor();
    }

    public void SetSpecialBrick(int i)
    {
        brickCount_.brickSpecial[idx_] = i;
        isIncre = false;
        if (i > 1)
        {
            brickStateCnt = 1;
            sprites_[0].gameObject.SetActive(false);
            sprites_[1].gameObject.SetActive(true);
            sprites_[1].gameObject.transform.localEulerAngles = triPos[i - 2];
            label_.leftAnchor.absolute = (int)labelPos[i - 2].x;
            label_.bottomAnchor.absolute = (int)labelPos[i - 2].y;
        }
        switch (i)
        {
            case 0:
                break;
            case 1:
                isIncre = true;
                sprites_[0].color = Color.white;
                sprites_[0].spriteName = "Sprite_Brick07";
                break;
        }
        BrickColor();
    }

    void ResetBrick()
    {
        brickCount_.brickSpecial[idx_] = 0;
        int triRandom = Random.Range(0, 101);
        isIncre = false;
        if (triRandom < 21)
        {
            triRandomPiece = Random.Range(0, 4);
            sprites_[0].gameObject.SetActive(false);
            sprites_[1].gameObject.SetActive(true);
            brickStateCnt = 1;
            sprites_[brickStateCnt].gameObject.transform.localEulerAngles = triPos[triRandomPiece];
            label_.leftAnchor.absolute = (int)labelPos[triRandomPiece].x;
            label_.bottomAnchor.absolute = (int)labelPos[triRandomPiece].y;
            brickCount_.brickSpecial[idx_] = triRandomPiece + 2;
        }
        else
        {
            increaseCnt = Random.Range(0, 101);
            if (increaseCnt < 11)
            {
                brickCount_.brickSpecial[idx_] = 1;
                isIncre = true;
            }
            brickStateCnt = 0;
            sprites_[0].gameObject.SetActive(true);
            sprites_[1].gameObject.SetActive(false);

            sprites_[3].enabled = true;

            label_.leftAnchor.absolute = 0;
            label_.bottomAnchor.absolute = 0;
            label_.gameObject.SetActive(true);

            label_.transform.localPosition = Vector2.zero;
        }
        BrickColor();
    }

    void SetSpriteAnchor(UISprite sprite, int num)
    {
        sprite.leftAnchor.absolute = sprite.bottomAnchor.absolute = -num;
        sprite.rightAnchor.absolute = sprite.topAnchor.absolute = num;
    }

    public void ResetSprite()
    {
        if (sprites_[0].spriteName != "Sprite_Brick_Bg")
            sprites_[0].spriteName = "Sprite_Brick_Bg";
    }

    public void SetGray()
    {
        for (int i = 0; i < sprites_.Length; i++)
        {
            if (i < 3)
                sprites_[i].color = new Color(90 / 255f, 90 / 255f, 90 / 255f);
            else
                sprites_[i].color = new Color(35 / 255f, 35 / 255f, 35 / 255f);
        } 
    }
}
