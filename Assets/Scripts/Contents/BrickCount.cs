using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// gameObject.transform.childCount << 이것에 용도는 내 자식 오브젝트의 개수를 가져옵니다.
public class BrickCount : MonoBehaviour
{
    public Brick[] bricks_;
    public Items[] items_;
    int rint;
    public List<int> brickInt;
    public List<int> itemInt;
    public List<int> brickSpecial;
    public List<int> Undo_HitCtn;
    public List<int> Undo_item;
    public int brick_idx_;
    public int countNum;
    public TweenPosition tPos;
    [SerializeField] TweenWidth[] comboTW;
    [SerializeField] TweenScale comboTS;
    [SerializeField] TweenAlpha comboTA;
    [SerializeField] ParticleSystem comboPs;
    [SerializeField] UILabel comboText;
    int currentMutiple = 2;
    public int lockbrickNum = -1;
    public bool firstMov = true;

    // 브릭 파괴 이펙트(파티클) 설정을 위한 변수
    int ballSpriteCount;

    // 파티클을 위한 변수
    [SerializeField] ParticleSystem[] ps_;

    // 슈팅모드 코인을 위한 변수
    public int getCoin;

    private void Start()
    {
        // 시작하면서 볼 스프라이트를 설정 - 체험해보기면 체험해보기로 변환
        ballSpriteCount = DataManager.instance.PreBallSprite > 0 ? DataManager.instance.PreBallSprite : DataManager.instance.BallSprite;

        if ((GameMode)DataManager.instance.CurGameMode != GameMode.STAGE
            && (GameMode)DataManager.instance.CurGameMode != GameMode.STAGEBOSS) ballSpriteCount = DataManager.instance.BallSprite;
    }

    public void SetBrick() // 여기서 블럭을 키는 모든 역활을 합니다.
    {
        Extensions.ShuffleCopy(bricks_, items_);
        if (BrickManager.instance.brickCount > 99) rint = Random.Range(4, bricks_.Length);// 여기서 랜덤값을 뽑아 옵니다.^_^ 
        else rint = Random.Range(1, bricks_.Length);

        FalseBrick();
        for (int i = 0; i < rint; ++i) // 여기서 랜덤값만큼 열어 줍니다.
        {
            bricks_[i].gameObject.SetActive(true); // 여기서 켜줍니다. 누구를? 내자식오브젝트들을 켜줍니다^_^ 
            bricks_[i].b_CntIDX = brick_idx_;
            if (BrickManager.instance.brickCount % 100 == 0)
            {
                ++currentMutiple;
            }
            if (BrickManager.instance.brickCount >= 10)
            {
                int r_ = Random.Range(0, 10);
                if (r_ < 5) bricks_[i].hitCnt = BrickManager.instance.brickCount * currentMutiple; // 여기서는 현재 스테이지의 맞아야할 횟수를 5배로 올려주는 겁니다.
                else bricks_[i].hitCnt = BrickManager.instance.brickCount; // 여기서는 현재 스테이지의 맞아야할 횟수를 정해 주는 겁니다.
            }
            else bricks_[i].hitCnt = BrickManager.instance.brickCount; // 여기서는 현재 스테이지의 맞아야할 횟수를 정해 주는 겁니다.

            bricks_[i].brickCount_ = this; // brickCount 지정 세팅
            //bricks_[i].ps_ = ps_[0];  // 클래식은 기본
            bricks_[i].ps_ = ps_[ballSpriteCount];  // 클래식은 기본
            

            bricks_[i].LabelSetting(); // 여기서 타이밍 에러가 생기지 않도록 라벨을 보여주는 곳을 콜
            bricks_[i].SetNext(); //색을 담당하는 부분입니다.
        }
        SetItem();
        SetSaveBlock();
        if (brickInt.Count(t => t == 0) < 9)
        {
            BrickManager.instance.EndGameCheck(brick_idx_, true);
        }
        getCoin = BrickManager.instance.brickCount;
    }

    public void SetSaveBlock()
    {
        DataManager.instance.SetSaveBlock(brickInt, brick_idx_);
        DataManager.instance.SetSaveSpecial(brickSpecial, brick_idx_);
    }

    void SetSaveItem()
    {
        DataManager.instance.SetSaveItems(itemInt, brick_idx_);
    }

    public void ShowSaveBrick()
    {
        FalseBrick();
        for (int i = 0; i < DataManager.instance.GetSaveBlock(brick_idx_).Count; ++i)
        {
            if (DataManager.instance.GetSaveBlock(brick_idx_)[i] > 0)
            {
                bricks_[i].gameObject.SetActive(true);
                bricks_[i].hitCnt = DataManager.instance.GetSaveBlock(brick_idx_)[i];
                bricks_[i].LabelSetting();
            }
            if (DataManager.instance.GetSaveSpecial(brick_idx_)[i] > 0)
            {
                bricks_[i].SetSpecialBrick(DataManager.instance.GetSaveSpecial(brick_idx_)[i]);
            }
        }
        transform.localPosition = DataManager.instance.GetBrickPos(brick_idx_);
        ShowSaveItem();
    }

    public void ShowStageBrick()
    {
        for (int i = 0; i < bricks_.Length; ++i)
        {
            bricks_[i].gameObject.SetActive(false);
            BlockShow blockShow = new BlockShow();
            blockShow = DataManager.instance.GetBlockShowData(brick_idx_, i);

            // 블록 세팅
            if ((blockShow.type_ < BlockTypes.Laser_Ver || (blockShow.type_ > BlockTypes.Hole_Out && blockShow.type_ < BlockTypes.bluemon)) && blockShow.health > 0 || blockShow.type_ == BlockTypes.TriBounce || blockShow.type_ == BlockTypes.BombAll)
            {
                bricks_[i].gameObject.SetActive(true);
                bricks_[i].StageSetting(blockShow.health, blockShow.type_, blockShow.colorType, this, brick_idx_, ps_[ballSpriteCount]);
            }
            // 큰 블록, 슬라이드 블록 서브 부분 세팅
            else if (blockShow.type_ > BlockTypes.TriBounce && blockShow.health == 0 && (blockShow.type_ < BlockTypes.bluemon || blockShow.type_ == BlockTypes.BombAll))
            {
                bricks_[i].StageSetting(blockShow.health, blockShow.type_, blockShow.colorType, this, brick_idx_, ps_[ballSpriteCount]);
            }
            // 보스 세팅
            else if (blockShow.type_ >= BlockTypes.bluemon && blockShow.health > 0)
            {
                BrickManager.instance.CreateBossPrefab((int)blockShow.type_ - (int)BlockTypes.bluemon, bricks_[i].sprites_[0].transform, blockShow.health, brick_idx_);
            }
        }
        ShowStageItem();
        MovingPos(379 + DataManager.instance.stagePosCorrection, 273 + DataManager.instance.stagePosCorrection - brick_idx_ * 74, DataManager.instance.stageSetIndex);
        SaveUndo();
        if (brickInt.Count(t => t == 0) < bricks_.Length)
        {
            BrickManager.instance.EndGameCheck(brick_idx_, true);
        }
    }

    public void AddLineBrick(int addNum)
    {
        for (int i = 0; i < bricks_.Length; ++i)
        {
            bricks_[i].gameObject.SetActive(false);
            BlockShow blockShow = new BlockShow();
            blockShow = DataManager.instance.GetBlockShowData(addNum, i, true);

            if ((blockShow.type_ < BlockTypes.Laser_Ver || blockShow.type_ > BlockTypes.Hole_Out) && blockShow.health > 0 || blockShow.type_ == BlockTypes.TriBounce)
            {
                bricks_[i].gameObject.SetActive(true);
                bricks_[i].StageSetting(blockShow.health, blockShow.type_, blockShow.colorType, this, brick_idx_, ps_[DataManager.instance.BallSprite]);
            }
        }
        AddLineItem(addNum);
        if (brickInt.Count(t => t == 0) < bricks_.Length)
        {
            BrickManager.instance.EndGameCheck(brick_idx_, true);
        }

    }

    public void lockBrick(BlockTypes type_, BlockColorType blockColor, int brickNum, int hit, bool bounce, Items.ItemInfo item_)
    {
        if (brickInt[brickNum] > 0)
        {
            if (bounce) bricks_[brickNum].LockBlockSetting();
            else if (type_ == BlockTypes.Fixed || type_ == BlockTypes.TriBounce) bricks_[brickNum].jumpBounceBrick();
        }
        if (type_ != BlockTypes.Blank) bricks_[brickNum].StageSetting(hit, type_, blockColor, this, brick_idx_, ps_[ballSpriteCount]);

        BlockTypes itemtype = BlockTypes.Blank;
        if (itemInt[brickNum] > -1)
        {
            if (bounce) items_[brickNum].LockBlockSetting();
            else if (type_ == BlockTypes.Fixed || type_ == BlockTypes.TriBounce) items_[brickNum].jumpBounceBrick();
        }
        switch (item_)
        {
            case Items.ItemInfo.Height:
                itemtype = BlockTypes.Laser_Ver;
                break;
            case Items.ItemInfo.Width:
                itemtype = BlockTypes.Laser_Hor;
                break;
            case Items.ItemInfo.Xcross:
                itemtype = BlockTypes.Laser_XCro;
                break;
            case Items.ItemInfo.Cross:
                itemtype = BlockTypes.Laser_Cro;
                break;
            case Items.ItemInfo.PingPong:
                itemtype = BlockTypes.Bounce;
                break;
            case Items.ItemInfo.Coin:
                itemtype = BlockTypes.Coin;
                break;
            case Items.ItemInfo.Ball:
                itemtype = BlockTypes.AddBall;
                break;
            case Items.ItemInfo.Hole_In:
                itemtype = BlockTypes.Hole_In;
                break;
            case Items.ItemInfo.Hole_Out:
                itemtype = BlockTypes.Hole_Out;
                break;
        }
        if (itemtype != BlockTypes.Blank)
        {
            items_[brickNum].gameObject.SetActive(true);
            items_[brickNum].StageSetting(itemtype, brick_idx_);
        }
    }

    void ShowStageItem()
    {
        for (int i = 0; i < bricks_.Length; ++i)
        {
            items_[i].gameObject.SetActive(false); // 아이템 오브젝트들을 꺼줍니다. 일단 한번

            BlockShow blockShow = new BlockShow();
            blockShow = DataManager.instance.GetBlockShowData(brick_idx_, i);

            if (blockShow.type_ > BlockTypes.Speaker && blockShow.type_ < BlockTypes.BombNormal_Hor)
            {

                items_[i].gameObject.SetActive(true);
                items_[i].StageSetting(blockShow.type_, brick_idx_);
            }
        }
    }

    void AddLineItem(int addNum)
    {
        for (int i = 0; i < bricks_.Length; ++i)
        {
            items_[i].gameObject.SetActive(false); // 아이템 오브젝트들을 꺼줍니다. 일단 한번

            BlockShow blockShow = new BlockShow();
            blockShow = DataManager.instance.GetBlockShowData(addNum, i, true);

            if (blockShow.type_ > BlockTypes.Speaker && blockShow.type_ < BlockTypes.BombNormal_Hor)
            {

                items_[i].gameObject.SetActive(true);
                items_[i].StageSetting(blockShow.type_, brick_idx_);
            }
        }
    }
    int idSound = 3;
    public void SoundChanger()
    {
        if (Time.timeScale > 1)
        {
            if (idSound > 2)
            {
                SoundManager.instance.ChangeEffects(9, 0.7f);
                idSound = 0;
            }
            ++idSound;
        }
        else
        {
            SoundManager.instance.ChangeEffects(9, 0.7f);
        }

    }

    public void SetChildeHitCount(int idx, int hitCnt, bool isAnim = true)
    {
        brickInt[idx] = hitCnt;

        if (hitCnt <= 0)
        {
            if (brickInt.Count(t => t == 0) >= bricks_.Length)
            {
                if (isAnim)
                {
                    playComboEffect();
                    BrickManager.instance.ComboAnim(BrickManager.instance.brickCombo);
                }
                BrickManager.instance.EndGameCheck(brick_idx_);
                if (countNum == bricks_.Length - 1) BrickManager.instance.TurnWarningSprite(brick_idx_);
            }
        }

    }

    void ShowSaveItem()
    {
        for (int i = 0; i < items_.Length; ++i) // 혹시나 하는 마음에..
        {
            items_[i].gameObject.SetActive(false); // 아이템 오브젝트들을 꺼줍니다. 일단 한번
        }

        for (int i = 0; i < DataManager.instance.GetSaveItems(brick_idx_).Count; ++i) // 혹시나 하는 마음에..
        {
            if (DataManager.instance.GetSaveItems(brick_idx_)[i] > -1)
            {
                items_[i].gameObject.SetActive(true);
                items_[i].SetItemType((Items.ItemInfo)DataManager.instance.GetSaveItems(brick_idx_)[i]);
            }
        }
    }

    void SetItem()
    {
        for (int i = 0; i < items_.Length; ++i) // 혹시나 하는 마음에..
        {
            items_[i].gameObject.SetActive(false); // 아이템 오브젝트들을 꺼줍니다. 일단 한번
            items_[i].b_CntIDX = brick_idx_;
        }
        if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
        {
            items_[items_.Length - 1].gameObject.SetActive(true);
            items_[items_.Length - 1].SetItemType(Items.ItemInfo.Money); // money item을 생성 해주는 곳 여기서 처음에 공하나 준다.
        }
        else
        {
            items_[items_.Length - 1].gameObject.SetActive(true);
            items_[items_.Length - 1].SetItemType(Items.ItemInfo.Ball); // ball item을 생성 해주는 곳 여기서 처음에 공하나 준다.
        }

        int randomItem = Random.Range(0, 100);

        if (randomItem > 39)
        {
            for (int i = 0; i < bricks_.Length - 1; ++i)
            {
                if (!bricks_[i].gameObject.activeSelf)
                {
                    int rnd = Random.Range(0, 10);
                    switch (rnd)
                    {
                        case 0:
                            items_[items_.Length - 2].gameObject.SetActive(true);
                            items_[items_.Length - 2].SetItemType(Items.ItemInfo.Width);
                            break;
                        case 1:
                            items_[items_.Length - 2].gameObject.SetActive(true);
                            items_[items_.Length - 2].SetItemType(Items.ItemInfo.Height);
                            break;
                        case 2:
                            items_[items_.Length - 2].gameObject.SetActive(true);
                            items_[items_.Length - 2].SetItemType(Items.ItemInfo.Cross);
                            break;
                        case 3:
                            items_[items_.Length - 2].gameObject.SetActive(true);
                            items_[items_.Length - 2].SetItemType(Items.ItemInfo.Xcross);
                            break;
                        case 4:
                            items_[items_.Length - 2].gameObject.SetActive(true);
                            items_[items_.Length - 2].SetItemType(Items.ItemInfo.PingPong);
                            break;
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                            items_[items_.Length - 2].gameObject.SetActive(true);
                            items_[items_.Length - 2].SetItemType(Items.ItemInfo.Coin);
                            break;

                    }
                }
            }
        }
        SetSaveItem();
    }

    public int brick;
    public void MovingPos(float startF, float endF, int brickCount = 0) //트윈 포지션을 사용해서 다음 포지션으로 이동을 시킨다.
    {
        brick = 9 + brickCount * 6;
        tPos.from.y = startF;
        tPos.to.y = endF;
        tPos.ResetToBeginning();
        tPos.PlayForward();
        if (DataManager.instance.CurGameMode != (int)GameMode.SHOOTING) FalseItemAll();
        if (firstMov)
        {
            firstMov = false;
        }
        else if (lockbrickNum >= 0)
        {
            bricks_[lockbrickNum].LockBlockSetting();
        }

        if ((GameMode)DataManager.instance.CurGameMode == GameMode.CLASSIC
             || (GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING)
        {
            if (countNum == brick - 1)
            {
                int a = 0;
                for (int i = 0; i < brick; ++i)
                {
                    if (brickInt[i] > 0)
                    {
                        BrickManager.instance.warningSprite.SetActive(true);
                        break;
                    }
                    ++a;
                }
                if (a == brick) BrickManager.instance.warningSprite.SetActive(false);
            }
            DataManager.instance.SetBrickPos(brick_idx_, new Vector2(0, endF));
            SetSaveBlock();
            SetSaveItem();
        }
        else
        {
            if (countNum == brick - 1 + brickCount)
            {
                BrickManager.instance.TurnWarningSprite(brick_idx_);
            }
        }
    }

    public void SaveUndo()
    {
        for (int i = 0; i < bricks_.Length; ++i)
        {
            Undo_HitCtn[i] = brickInt[i];
            Undo_item[i] = itemInt[i];
        }
    }

    public void FalseItemAll()
    {
        for (int i = 0; i < items_.Length; ++i)
        {
            items_[i].FalseMy();
            bricks_[i].IncreseBrick();
        }
    }
    public void FalseBrick(bool isBossStage = false)
    {
        for (int i = 0; i < bricks_.Length; ++i) // 혹시나 하는 마음에..
        {
            bricks_[i].hitCnt = 0;
            bricks_[i].LabelSetting(isBossStage);
            bricks_[i].gameObject.SetActive(false); // 내 자식 오브젝트들을 꺼줍니다. 일단 한번
        }
    }

    public void OnMoreCheck()
    {
        for (int i = 0; i < bricks_.Length; ++i) // 혹시나 하는 마음에..
        {
            bricks_[i].hitCnt = 0;
            bricks_[i].LabelSetting();
            bricks_[i].gameObject.SetActive(false); // 내 자식 오브젝트들을 꺼줍니다. 일단 한번
        }

        SetSaveBlock();
    }

    public void OnBombLineBrick(int brickNum) // Bomb라인 위의 Brick들을 제거(07.22)
    {
        if (brickInt[brickNum] != 0)
        {
            if (DataManager.instance.CurGameMode == (int)GameMode.CLASSIC
                || (GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING)
            {
                for (int i = 0; i < bricks_.Length; ++i)
                {
                    if (bricks_[i].idx_ == brickNum)
                    {
                        BrickManager.instance.UpScore();
                        bricks_[i].OnBomb();
                        break;
                    }
                }
            }
            else
            {
                BrickManager.instance.UpScore();
                bricks_[brickNum].OnBomb();
            }
        }
    }

    public void OnLaserLineBrick(int brickNum, int damage = 1)
    {
        if (brickInt[brickNum] != 0 || ((bricks_[brickNum].types_ > BlockTypes.TriBounce && bricks_[brickNum].types_ < BlockTypes.slideBlock_1) || bricks_[brickNum].types_ == BlockTypes.BombAll))
        {
            if (DataManager.instance.CurGameMode == (int)GameMode.CLASSIC
                || (GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING)
            {
                for (int i = 0; i < bricks_.Length; ++i)
                {
                    if (bricks_[i].idx_ == brickNum)
                    {
                        bricks_[i].OnLaser(damage);
                        break;
                    }
                }
            }
            else
            {
                bricks_[brickNum].OnLaser(damage);
            }
        }
    }

    public void UndoBrickCount()
    {
        for (int i = 0; i < bricks_.Length; ++i)
        {
            if (Undo_HitCtn[i] != 0)
            {
                if (DataManager.instance.CurGameMode == (int)GameMode.CLASSIC
                    || (GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING)
                {
                    for (int j = 0; j < bricks_.Length; ++j)
                    {
                        if (bricks_[j].idx_ == i)
                        {
                            bricks_[j].UndoBrick(Undo_HitCtn[i]);
                            BrickManager.instance.EndGameCheck(brick_idx_, true);
                            BrickManager.instance.isClear = false;
                        }
                    }
                }
                else
                {
                    bricks_[i].UndoBrick(Undo_HitCtn[i]);
                    BrickManager.instance.EndGameCheck(brick_idx_, true);
                    BrickManager.instance.isClear = false;
                }
            }
            else if (Undo_item[i] != -1)
            {
                if (DataManager.instance.CurGameMode == (int)GameMode.CLASSIC
                    || (GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING)
                {
                    for (int j = 0; j < bricks_.Length; ++j)
                    {
                        if (items_[j].idx_ == i)
                        {
                            items_[j].undoItem(Undo_item[i]);
                        }
                    }
                }
                else
                {
                    items_[i].undoItem(Undo_item[i]);
                }
            }
        }
    }

    public int LockBrickMax(int a)
    {
        if (brickInt[a] > 0)
        {
            return 1;
        }
        else if (itemInt[a] > -1)
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }

    public void playComboEffect()
    {
        comboText.text = string.Format("{0}x", ++BrickManager.instance.brickCombo);
        comboTA.ResetToBeginning();
        comboTA.PlayForward();
        comboTW[0].ResetToBeginning();
        comboTW[0].PlayForward();
        comboTW[1].ResetToBeginning();
        comboTW[1].PlayForward();
        comboTS.ResetToBeginning();
        comboTS.PlayForward();
        comboPs.Play();
    }

    public void SetTweenDuration(float t)
    {
        tPos.duration = t;
    }

    public void SetGray()
    {
        for (int i = 0; i < bricks_.Length; i++)
        {
            bricks_[i].SetGray();
        }
    }

    public void ResetSprite()
    {
        for (int i = 0; i < bricks_.Length; i++)
        {
            if (bricks_[i].sprites_[0].spriteName != "Sprite_Brick_Bg")
                bricks_[i].sprites_[0].spriteName = "Sprite_Brick_Bg";
        }
    }
}
