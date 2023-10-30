using UnityEngine;
using System.Collections;

public class Items : MonoBehaviour
{
    public enum ItemInfo    // 아이템 종류
    {
        Ball = 0,
        Width = 1,
        Height = 2,
        Cross = 3,
        Xcross = 4,
        PingPong = 5,
        Coin = 6,
        Balls = 7,
        Hole_In = 8,
        Hole_Out = 9,
        Money = 10,
        None = -1
    }
    public ItemInfo itemType;
    [SerializeField]
    UISprite sprite_;//, shadowSprite_ = null;
    [SerializeField]
    public int idx_;
    public int b_CntIDX;
    string spriteName_;
    bool isFalse;
    [SerializeField]
    bool isToong;
    Items holeTarget;

    public BrickCount counts_;

    public void SetItemType(ItemInfo type_)
    {
        if (counts_ == null) counts_ = GetComponentInParent<BrickCount>();
        itemType = type_;
        isFalse = false;
        counts_.itemInt[idx_] = (int)type_;
        switch (type_) // 여기는 Sprite만 설정해주세요
        {
            case ItemInfo.Ball:
                spriteName_ = "Sprite_Ball+1";
                break;
            case ItemInfo.Balls:
                spriteName_ = "Sprite_Ball+1";
                break;
            case ItemInfo.Width:
                spriteName_ = "Sprite_Hor";
                break;
            case ItemInfo.Height:
                spriteName_ = "Sprite_Ver";
                break;
            case ItemInfo.Cross:
                spriteName_ = "Sprite_Cross";
                break;
            case ItemInfo.Xcross:
                spriteName_ = "Sprite_XCross";
                break;
            case ItemInfo.PingPong:
                spriteName_ = "Sprite_Bounce";
                break;
            case ItemInfo.Coin:
                spriteName_ = "Sprite_Gem";
                break;
            case ItemInfo.Money:
                spriteName_ = "Sprite_Coin";
                break;
        }
        sprite_.spriteName = spriteName_;
    }   // 아이템 타입 셋팅

    public void StageSetting(BlockTypes types_, int b_Idx)  // 스테이지 셋팅
    {
        if (counts_ == null) counts_ = GetComponentInParent<BrickCount>();
        b_CntIDX = b_Idx;
        ItemInfo info_ = ItemInfo.Ball;
        switch (types_)
        {
            case BlockTypes.Laser_Ver:
                info_ = ItemInfo.Height;
                spriteName_ = "Sprite_Ver";
                break;
            case BlockTypes.Laser_Hor:
                info_ = ItemInfo.Width;
                spriteName_ = "Sprite_Hor";
                break;
            case BlockTypes.Laser_XCro:
                info_ = ItemInfo.Xcross;
                spriteName_ = "Sprite_XCross";
                break;
            case BlockTypes.Laser_Cro:
                info_ = ItemInfo.Cross;
                spriteName_ = "Sprite_Cross";
                break;
            case BlockTypes.Bounce:
                info_ = ItemInfo.PingPong;
                spriteName_ = "Sprite_Bounce";
                break;
            case BlockTypes.Coin:
                info_ = ItemInfo.Coin;
                spriteName_ = "Sprite_Coin";
                break;
            case BlockTypes.AddBall:
                info_ = ItemInfo.Ball;
                spriteName_ = "Sprite_Ball+1";
                break;
            case BlockTypes.Hole_In:
                info_ = ItemInfo.Hole_In;
                spriteName_ = "Sprite_BlackHole";
                BrickManager.instance.holeList.Add(this);
                break;
            case BlockTypes.Hole_Out:
                info_ = ItemInfo.Hole_Out;
                spriteName_ = "Sprite_WhiteHole";
                BrickManager.instance.holeList.Add(this);
                break;
        }
        itemType = info_;
        isFalse = false;
        sprite_.spriteName = spriteName_;
        counts_.itemInt[idx_] = (int)info_;
    }

    public void undoItem(int info)  // 뒤로가기 아이템
    {
        isFalse = false;
        itemType = (ItemInfo)info;
        counts_.itemInt[idx_] = info;
        gameObject.SetActive(true);
    }

    public void LockBlockSetting()  // 락블럭 셋팅
    {
        BrickManager.instance.LockBlock(BlockTypes.Blank, BlockColorType.none, b_CntIDX, idx_, 0, true, itemType);
        isFalse = true;
        FalseMy();
    }

    public void jumpBounceBrick()   // 점프바운스브릭
    {
        BrickManager.instance.JumpBlcok(BlockTypes.Blank, BlockColorType.none, b_CntIDX, idx_, 0, itemType);
        isFalse = true;
        FalseMy();
    }

    public void FalseMy()   // 아이템 bool값 판단 함수
    {
        if (isFalse)
        {
            itemType = ItemInfo.None;
            counts_.itemInt[idx_] = -1;
            gameObject.SetActive(false);
        }
    }

    public void BottomTrue()    // 아래 벽 체크
    {
        itemType = ItemInfo.None;
        counts_.itemInt[idx_] = -1;
        gameObject.SetActive(false);
    }

    public void OnHoleTarget(Items t)   // 
    {
        holeTarget = t;
    }

    void OnTriggerEnter2D(Collider2D cd)
    {
        if (cd.gameObject.CompareTag("Ball"))
        {
            isFalse = true;
            switch (itemType)
            {
                case ItemInfo.Ball:
                    SoundManager.instance.ChangeEffects(13);
                    BallManager.instance.PlusBall(1);
                    FalseMy();
                    break;
                case ItemInfo.Balls:
                    BallManager.instance.PlusBall(10);
                    FalseMy();
                    break;
                case ItemInfo.Width:
                    SoundManager.instance.ChangeEffects(2);
                    GameContents.instance.gameitem(gameObject.transform.position, 0);
                    BrickManager.instance.Laser(itemType, b_CntIDX, idx_);
                    break;
                case ItemInfo.Height:
                    SoundManager.instance.ChangeEffects(2);
                    GameContents.instance.gameitem(gameObject.transform.position, 1);
                    BrickManager.instance.Laser(itemType, b_CntIDX, idx_);
                    break;
                case ItemInfo.Cross:
                    SoundManager.instance.ChangeEffects(2);
                    GameContents.instance.gameitem(gameObject.transform.position, 2);
                    BrickManager.instance.Laser(itemType, b_CntIDX, idx_);
                    break;
                case ItemInfo.Xcross:
                    SoundManager.instance.ChangeEffects(2);
                    GameContents.instance.gameitem(gameObject.transform.position, 3);
                    BrickManager.instance.Laser(itemType, GetComponentInParent<BrickCount>().countNum, idx_);
                    break;
                case ItemInfo.Coin:
                    SoundManager.instance.ChangeEffects(4);
                    DataManager.instance.SetCoin(1);
                    UIManager.instance.SetMoney();
                    gameObject.SetActive(false);
                    break;
                case ItemInfo.PingPong:
                    SoundManager.instance.ChangeEffects(3);
                    cd.gameObject.GetComponent<BallMove>().ball_Pos_ = cd.transform.position;
                    GameContents.instance.pingpongItem(cd.gameObject);
                    break;
                case ItemInfo.Hole_In:
                    Transform parentTransfrom = cd.transform.parent;
                    cd.transform.SetParent(holeTarget.transform);
                    cd.transform.localPosition = Vector3.zero;
                    cd.transform.SetParent(parentTransfrom);
                    cd.GetComponent<BallMove>().RotateBall();
                    isFalse = false;
                    break;
                case ItemInfo.Hole_Out:
                    isFalse = false;
                    break;
                case ItemInfo.Money:
                    SoundManager.instance.ChangeEffects(15);
                    BallManager.instance.GetMoney(counts_.getCoin);
                    FalseMy();
                    break;
            }
        }
    }   // 태그값에 의한 콜라이더 반응
}
