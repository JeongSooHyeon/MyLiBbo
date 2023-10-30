using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockShow
{
    public int mIdx;
    public BlockTypes type_;
    public BlockColorType colorType;
    public int health;
    public BlockUseType uType;
}

[System.Serializable]
public class MapDataList
{
    public int curCol;
    public int curRow;
    public int StageIdx;
    public GameMode MyGM;
    public int BallCount;
    public int BossHPCount;
    public StageBossType BossType;
    public float BossSpeed;
    public int[] StartArr = new int[3];
    public List<BlockShow> myBlockDatas;
    public List<BlockShow> AddBlockDatas;
}

public class MapData : ScriptableObject
{
    public List<MapDataList> MapDataList;
}