using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BlockColorEditor : EditorWindow
{
    public static BlockColorEditor window;

    Color matColor = Color.white;

    private List<Sprite> brickImgs;

    private BlockColorData blockColor;
    string Path = "Assets/Resources/BlockColorData";
    BlockColorType curBColor;
    List<BlockColorItem> BlockColorDatas;
    int maxColorNum = 6;
    [MenuItem("Tools/BlockColor", false, 10)]
    public static void Init()
    {
        window = (BlockColorEditor)GetWindow(typeof(BlockColorEditor));
        window.minSize = new Vector2(550, 620);
    }

    void OnGUI()
    {
        if (window == null) Init();

        if (brickImgs == null)
        {
            brickImgs = new List<Sprite>();
            Sprite[] loadArr = Resources.LoadAll<Sprite>("TintBlock");
            for (int i = 0; i < loadArr.Length; ++i)
            {
                if(loadArr[i].name == "boxin" || loadArr[i].name == "triin")
                    brickImgs.Add(loadArr[i]);
            }
        }
        if (BlockColorDatas == null) BlockColorDatas = new List<BlockColorItem>();

        if (blockColor == null)
        {
            blockColor = new BlockColorData();

            if (blockColor.BlockColorDatas == null)
            {
                blockColor.BlockColorDatas = new List<BlockColorItem>();

                for (int i = 0; i < maxColorNum; ++i)
                {
                    BlockColorItem data = new BlockColorItem();
                    data.myBColorType = (BlockColorType)i;
                    if (data.BlockColorList == null)
                    {
                        data.BlockColorList = new List<BlockColorSet>();
                        CreateData(data.BlockColorList);
                    }
                    blockColor.BlockColorDatas.Add(data);
                }
            }
        }
         
        DefaultUI();
    }

    void CreateData(List<BlockColorSet>  list)
    {
        for(int i = 0; i< 3; ++i)
        {
            BlockColorSet colorD = new BlockColorSet();
            list.Add(colorD);
        }
    }
    void DefaultUI()
    {
        if (GUILayout.Button("저장", new GUILayoutOption[] { GUILayout.Width(150) }))
            SaveData();
        if (GUILayout.Button("불러오기", new GUILayoutOption[] { GUILayout.Width(150) }))
            LoadData();

        GUILayout.BeginVertical();
        curBColor = (BlockColorType)EditorGUILayout.EnumPopup(curBColor, GUILayout.Width(93));
        GUILayout.BeginHorizontal();
        if(BlockColorDatas.Count > 0)
        {
            for (int i = 0; i < BlockColorDatas[(int)curBColor].BlockColorList.Count; ++i)
            {
                SetColorImg(BlockColorDatas[(int)curBColor].BlockColorList[i], i);
            }
            
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
    Texture2D texture;
    void SetColorImg(BlockColorSet data, int idx)
    {
        GUILayout.Space(50);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        data.BlockInColor = EditorGUILayout.ColorField("InColor", data.BlockInColor, new GUILayoutOption[] {
                        GUILayout.Width (70),
                        GUILayout.Height (30)
                    });
        GUILayout.Space(50);
        SetColor(brickImgs[0].texture, data.BlockInColor);
        GUILayout.Button(brickImgs[0].texture, new GUILayoutOption[] { GUILayout.Width(30), GUILayout.Height(30) });
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        data.BlockOutColor = EditorGUILayout.ColorField("OutLine", data.BlockOutColor, new GUILayoutOption[] {
                        GUILayout.Width (70),
                        GUILayout.Height (30)
                    });
        GUILayout.Space(50);
        SetColor(brickImgs[1].texture, data.BlockOutColor);
        GUILayout.Button(brickImgs[1].texture, new GUILayoutOption[] { GUILayout.Width(30), GUILayout.Height(30) });
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }


    void SetColor(Texture2D myT,Color outC)
    {
        for (int y = 0; y < myT.height; y++)
        {
            for (int x = 0; x < myT.width; x++)
            {
                myT.SetPixel(x, y, outC);
            }
        }
        myT.Apply();
    }

    bool LoadData()
    {
        bool ret = true;
        if (blockColor == null)
            blockColor = new BlockColorData();

        blockColor = AssetDatabase.LoadAssetAtPath<BlockColorData>(Path + ".asset");
        BlockColorDatas = blockColor.BlockColorDatas;
        if (blockColor == null) ret = false;

        return ret;
    }

    void SaveData()
    {
        if (!AssetDatabase.Contains(blockColor))
        {
            AssetDatabase.CreateAsset(blockColor, Path + ".asset");
        }else
        {
            BlockColorDatas = blockColor.BlockColorDatas;
            BlockColorData preData = new BlockColorData();
            preData.BlockColorDatas = BlockColorDatas;
            AssetDatabase.DeleteAsset(Path + ".asset");
            AssetDatabase.CreateAsset(preData, Path + ".asset");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
