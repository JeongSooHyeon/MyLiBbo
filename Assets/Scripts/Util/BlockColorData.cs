using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BlockColorSet
{
    public Color BlockInColor;
    public Color BlockOutColor;
}
[System.Serializable]
public class BlockColorItem
{
    public BlockColorType myBColorType;
    public List<BlockColorSet> BlockColorList;
}

public class BlockColorData : ScriptableObject
{
    public List<BlockColorItem> BlockColorDatas;
}
