public enum BlockTypes
{
    Blank, Normal, Triangle1, Triangle2, Triangle3, Triangle4, Speaker, Laser_Ver, Laser_Hor, Laser_XCro, Laser_Cro, Bounce, AddBall,
    Coin, Hole_In, Hole_Out, BombNormal_Hor, BombNormal_Ver, BombNormal_XCro, BombNormal_Cro, BombNormal_Box, Fixed, TriBounce,
    blockS,blockM,blockL,blockRetangle1, blockRetangle2,slideBlock_1,slideBlock_2,slideBlock_3,slideBlock_4,bluemon,pinkmon,chocomon,
    lemon,orangemon,chocoballmon,eggmon,popmon_P, popmon_W, popmon_Y, cakemon, cookiemon, BombAll
}
public enum GameMode
{
    STAGE, STAGEBOSS, BALL100, CLASSIC, SHOOTING
}

public enum SceneType
{
    Preload, Lobby, Ingame
}

public enum StageBossType
{
    normal, twin, speed
}

public enum BlockUseType
{
    Empty, Block, Item, Obstacle
}

public enum PopupType
{
    none,
    option,
    exit,
    common,
    loading,
    tuto,
    dailyReward,
    dailyNotice,
    lobbyTip,
    ballSkillInfo,
    emptyRewardAd,
    stageScroll,
    lobbyMenu,
    moreGame,
    getDailyReward,
    housing,
    housingReward,
    stageReward,
    stageSetting,
    housingInfo,
    ShootingEdit
}

public enum Language
{
    English,
    Korean,
    Japanese,
    //Chinese_Simplified,
    //Chinese_Traditional
}

public enum ShopState
{
    Buy,
    UnUse,
    Use,
    PreUse
}

public enum CommonState
{
    none = -1,
    buyCoin,
    coinLack,
    buyBall,
    GetCoinAds,
    GetGift,
    buyPackage,
    GetDailyReward,
    PreBall,
    SweetAimPre,
    SweetAimBuy,
    InternetConnect,
    GameCenterLogin,
    NoadsBuy,
    buyMonster,
    ShootingEdit
}

public enum InstanceItem
{
    DoubleBall,
    AllBlockDamage,
    PowerUp,
    LineUp,
    Undo,
    AdBall,
    PlusBall,
    AttackUp,
    FireUp,
    ProjUp,
    
}

public enum UIState
{
    None,
    Back,
    Coin,
    Exit,
    ResultSuc,
    Pause,
    InGame,
    BestScore,
    Plane,
    Tutorial,
    DownBall,
    ResultFailed,
    ReStartGame,
    SoundMute,
    Tutorial2,
    EffectSoundMute,
    ShootingTuto,
    ShootingEdit
}

public enum ButtonState
{
    None,
    TweenBtn,
}

public enum IngameRewardState
{
    None,
    Achieve,
    Rewarded
}

public enum adsType
{
    none = -1,
    getCoin,
    oneMore,
    rewardCoin,
    rewardItem,
    plusBall,
    continueAD,
    dailyReward,
    preMonster,
    SweetAim,
    preBall,
    preCharBall
}

public enum RewardAdsType
{
    getCoinLobby,
    getCoinIngame,
    plusBall,
    relay,
    preCharBall,
    daily
}

public enum GpgsLoginType
{
    none =-1,
    first,
    login,
    save
}

public enum FirebaseType
{
    new_user,
    daily_bonus,
    menu,
    use_item,
    use_ruby,
    game_result
}

public enum BlockColorSize
{
    small,
    medium,
    large
}

public enum BlockColorType
{
    none = -1,
    purple,
    blue,
    green,
    yellow,
    orange,
    pink
}

public enum CharaterSkilltype
{
    none = -1,
    blueSkill,
    eggSkill,
    lemonSkill,
    orangeSkill,
    pinkSkill,
    chocoSkill,
    blueSkill_2,
    eggSkill_2,
    lemonSkill_2,
    orangeSkill_2,
    pinkSkill_2,
    chocoSkill_2
}

public enum HousingState
{
    none = -1,
    collect,
    getreward,
    complete
}

public enum StageStarState
{
    none = -1,
    collect,
    getreward,
    complete
}

public enum Monster
{
    none = -1,
    normalBlue,
    normalEgg,
    normalLemon,
    normalOrange,
    normalPink,
    normalChoco,
    hipBlue,
    hipEgg,
    hipLemon,
    hipOrange,
    hipPink,
    hipChoco,
    gangBlue,
    gangEgg,
    gangLemon,
    gangOrange,
    gangPink,
    gangChoco
}

public enum LobbyState 
{
    Shop,
    Character,
    Lobby,
    Housing,
    Mode
}
