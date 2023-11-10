using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class NewMapEditor : EditorWindow
{
    private List<Sprite> brickImgs;
    public static NewMapEditor window;
    private GameMode curGameMode;
    BlockTypes curBlockType;
    int maxRows = 9, maxCols = 9, rownum = 0, colnum = 0, levels = 0, BossHP = 0 , scoreMultiple = 0, maxBalls=0, addLineNum = 0;
    StageBossType sb_Type;
    float stageBossTime;
    int[] StarArrs = new int[3];
    bool isMultiple;
    List<BlockShow> BlockList;
    List<BlockShow> AddBlockList;
    MapDataList saveData_Stage;
    MapDataList saveData_Ball;
    Vector2 scrollViewVector;
    string Path = "NewMapData";
    string cPath = "Assets/Resources/BlockColorData";
    BlockColorData blockColor;
    List<Sprite> colorImgs;
    [MenuItem("Tools/MapEditor2", false, 10)]
    public static void Init()
    {
        window = (NewMapEditor)GetWindow(typeof(NewMapEditor));
        window.minSize = new Vector2(550, 620);
    }

    void OnGUI()
    {
        if (window == null) Init();
        if (brickImgs == null)
        {
            brickImgs = new List<Sprite>();
            Sprite[] loadArr = Resources.LoadAll<Sprite>("MapEditorIcon");
            for (int i = 0; i < loadArr.Length; ++i)
            {
                brickImgs.Add(loadArr[i]);
            }
        }

        if(colorImgs == null)
        {
            colorImgs = new List<Sprite>();
            Sprite[] loadArr = Resources.LoadAll<Sprite>("TintBlock");
            for (int i = 0; i < loadArr.Length; ++i)
            {
                if (loadArr[i].name != "boxin" || loadArr[i].name != "triin")
                    colorImgs.Add(loadArr[i]);
            }
        }
        if (saveData_Ball == null) saveData_Ball = new MapDataList();

        if (saveData_Stage == null) saveData_Stage = new MapDataList();


        if (BlockList == null)
        {
            BlockList = new List<BlockShow>();
            CreateEmptyBlockData(maxCols * maxRows, BlockList);
        }

        if (AddBlockList == null)
        {
            AddBlockList = new List<BlockShow>();
            CreateEmptyBlockData(maxCols * maxRows, AddBlockList);
        }

        if (blockColor == null)
        {
            blockColor = new BlockColorData();
            blockColor = AssetDatabase.LoadAssetAtPath<BlockColorData>(cPath + ".asset");
        }
        DefaultGUI();

        switch (curGameMode)
        {
            case GameMode.CLASSIC:
                GUILayout.Label("Classic mode does not require map editing.", EditorStyles.boldLabel);
                return;
            case GameMode.BALL100:
                break;
            case GameMode.STAGE:
            case GameMode.STAGEBOSS:
                GUILayout.Space(30);
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.Label("StageBall:", EditorStyles.boldLabel);
                maxBalls = EditorGUILayout.IntField(maxBalls, new GUILayoutOption[] {
                        GUILayout.Width (40),
                        GUILayout.Height (30)
                    });
                GUIStars();
                GUILayout.EndHorizontal();
                if (curGameMode == GameMode.STAGEBOSS)
                {
                    GUILayout.BeginHorizontal();
                    //GUILayout.Space(10);
                    GUILayout.Label("StageBoss:", EditorStyles.boldLabel);
                    GUILayout.Label("BossHP :", EditorStyles.boldLabel);
                    BossHP = EditorGUILayout.IntField(BossHP, new GUILayoutOption[] { GUILayout.Width(100) });
                    GUILayout.Space(10);
                    GUILayout.Label("BossType :", EditorStyles.boldLabel);
                    sb_Type = (StageBossType)EditorGUILayout.EnumPopup(sb_Type, GUILayout.Width(93));
                    GUILayout.Space(10);
                    GUILayout.Label("BossTime :", EditorStyles.boldLabel);
                    stageBossTime = EditorGUILayout.FloatField(stageBossTime, new GUILayoutOption[] { GUILayout.Width(100) });
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                }
                GUILayout.EndVertical();
                break;
        }
        CreateBrickBtns();
    }

    void CreateEmptyBlockData(int maxl, List<BlockShow> target)
    {
        for (int i = 0; i < maxl; ++i)
        {
            BlockShow data = new BlockShow();
            data.colorType = BlockColorType.none;
            target.Add(data);
        }
    }

    void LoadData()
    {
        string tDataName = curGameMode.ToString();
        TextAsset mapText = null;
        if (curGameMode != GameMode.CLASSIC)
        {
            switch (curGameMode)
            {
                case GameMode.BALL100:
                    //saveData_Ball = AssetDatabase.LoadAssetAtPath<MapData>(Path + tDataName + ".asset");
                    mapText = Resources.Load(Path + "/Ball/"+(levels+1)) as TextAsset;
                    if (mapText != null)
                        saveData_Ball = JsonToOject<MapDataList>(mapText.text);
                    else
                        saveData_Ball = new MapDataList();
                    break;
                case GameMode.STAGEBOSS:
                case GameMode.STAGE:
                    if (curGameMode == GameMode.STAGEBOSS) tDataName = GameMode.STAGE.ToString();
                    //if (saveData_Stage == null)
                    //    saveData_Stage = new MapData();
                    //saveData_Stage = AssetDatabase.LoadAssetAtPath<MapData>(Path + tDataName + ".asset");

                    mapText = Resources.Load(Path + "/Stage/" + (levels + 1)) as TextAsset;
                    if (mapText != null)
                        saveData_Stage = JsonToOject<MapDataList>(mapText.text);
                    else
                        saveData_Stage = new MapDataList();
                    break;
            }
        }
    }
    string changeLvl = "";
    string targetBlock = "";
    void DefaultGUI()
    {
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("GameMode : ", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(100) });
        curGameMode = (GameMode)EditorGUILayout.EnumPopup(curGameMode, GUILayout.Width(100));
        GUILayout.Space(10);
        if (GUILayout.Button("저장", new GUILayoutOption[] { GUILayout.Width(150) }))
            MapDataSave();
        if (GUILayout.Button("불러오기", new GUILayoutOption[] { GUILayout.Width(150) }))
            MapDataLoad();
        if (GUILayout.Button("이전데이터불러오기", new GUILayoutOption[] { GUILayout.Width(150) }))
            MapDataPreLoad();

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Level:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(50) });
        GUILayout.Space(10);
        if (GUILayout.Button("<<", new GUILayoutOption[] { GUILayout.Width(50) })) PreviousLevel();

        changeLvl = GUILayout.TextField(changeLvl, new GUILayoutOption[] { GUILayout.Width(50) });
       
        if (GUILayout.Button(">>", new GUILayoutOption[] { GUILayout.Width(50) })) NextLevel();
        if (GUILayout.Button("레벨 이동", new GUILayoutOption[] { GUILayout.Width(70) }))
        {
            int clv = int.Parse(changeLvl);
            if (curGameMode == GameMode.BALL100)
            {
                levels = clv;
            }
            else
            {
                levels = clv;
            }
            MapDataLoad();
        }
        //if (GUILayout.Button("마지막 레벨 삭제", new GUILayoutOption[] { GUILayout.Width(150) }))
        //{
        //    if (curGameMode == GameMode.BALL100)
        //    {   
        //        saveData_Ball.MapDataList.RemoveAt(saveData_Ball.MapDataList.Count - 1);
        //        levels = levels > (saveData_Ball.MapDataList.Count - 1) ? levels - 1 : levels;
        //    }
        //    else
        //    {
        //        saveData_Stage.MapDataList.RemoveAt(saveData_Stage.MapDataList.Count - 1);
        //        levels = levels > (saveData_Ball.MapDataList.Count - 1) ? levels - 1 : levels;
        //    }
        //}
        GUILayout.Space(10);

        GUILayout.Label("X:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(30) });
        colnum = int.Parse(GUILayout.TextField(colnum.ToString(), new GUILayoutOption[] { GUILayout.Width(50) }));

        GUILayout.Label("Y:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(30) });
        rownum = int.Parse(GUILayout.TextField(rownum.ToString(), new GUILayoutOption[] { GUILayout.Width(50) }));

        if (GUILayout.Button("맵 사이즈 수정", new GUILayoutOption[] { GUILayout.Width(100) }))
        {
            if(rownum > 0)
                maxRows = rownum;
            if(colnum> 0)
                maxCols = colnum;

            if(curGameMode == GameMode.BALL100)
            {
                saveData_Ball.curRow = maxRows;
                saveData_Ball.curCol = maxCols;
                SetMapSizeChange(saveData_Ball);
            }
            else if(curGameMode == GameMode.STAGE || curGameMode == GameMode.STAGEBOSS)
            {
                saveData_Stage.curRow = maxRows;
                saveData_Stage.curCol = maxCols;
                SetMapSizeChange(saveData_Stage);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Map Block:", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.BeginVertical();
        int count = 0;
        for (int i = 0; i < brickImgs.Count; ++i)
        {
            if (count == 0) GUILayout.BeginHorizontal();
            if(curGameMode == GameMode.BALL100)
            {
                if (!isBossBlock(((BlockTypes)i)))
                {
                    SetBrickButton(brickImgs.Find(t => t.name == ((BlockTypes)i).ToString()).texture, " - " + ((BlockTypes)i), (BlockTypes)i);
                    if (count > 4)
                    {
                        count = 0;
                        GUILayout.EndHorizontal();
                        continue;
                    }
                    ++count;
                }
            }else
            {
                SetBrickButton(brickImgs.Find(t => t.name == ((BlockTypes)i).ToString()).texture, " - " + ((BlockTypes)i), (BlockTypes)i);
                if (count > 6)
                {
                    count = 0;
                    GUILayout.EndHorizontal();
                    continue;
                }
                ++count;
            }   
        }
        GUILayout.EndVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("CurrentSelect : ", EditorStyles.boldLabel);
        GUILayout.Box(brickImgs.Find(t => t.name == (curBlockType).ToString()).texture, new GUILayoutOption[] {
                        GUILayout.Width (30),
                        GUILayout.Height (30)
                    });
        GUILayout.Label(targetBlock, EditorStyles.boldLabel);
        if (GUILayout.Button("groupDel", new GUILayoutOption[] { GUILayout.Width(70) }))
        {
            for (int i = 0; i < BlockList.Count; ++i)
            {
                if (IsGroupBlock(BlockList[i].type_))
                {
                    BlockList[i].type_ = BlockTypes.Blank;
                    BlockList[i].uType = BlockUseType.Empty;
                    BlockList[i].health = 0;
                }
            }
        }
        if (curGameMode == GameMode.STAGE || curGameMode == GameMode.STAGEBOSS)
        {
            GUILayout.Space(30);
            GUILayout.Label("AddLine:", EditorStyles.boldLabel);
            GUILayout.Space(30);
            addLineNum = EditorGUILayout.IntField(addLineNum, new GUILayoutOption[] { GUILayout.Width(50) });
            GUILayout.Space(30);
            if (GUILayout.Button("LineAdd", new GUILayoutOption[] { GUILayout.Width(70) }))
            {
                int addNum = (AddBlockList.Count / maxRows);
                if (addLineNum> 0 || addNum < addLineNum)
                {
                    AddLineCreat(addLineNum- addNum);
                }
            }
            if (GUILayout.Button("LineDel", new GUILayoutOption[] { GUILayout.Width(70) }))
            {
                if (addLineNum == 0) return;

                --addLineNum;

                for (int i=0; i< maxCols; ++i)
                {
                    AddBlockList.RemoveAt(AddBlockList.Count-1);
                }
            }
            if (GUILayout.Button("AddLineCLEAR", new GUILayoutOption[] { GUILayout.Width(100) }))
            {
                AddBlockClear(); 
            }

            if (GUILayout.Button("ALLCLEAR", new GUILayoutOption[] { GUILayout.Width(80) }))
            {
                BlockClear();
                AddBlockClear();
            }
            if (GUILayout.Button("BossClear", new GUILayoutOption[] { GUILayout.Width(80) }))
            {
                BossAllClear();
            }
            GUILayout.Space(30);
            if (GUILayout.Button("PurpleSet", new GUILayoutOption[] { GUILayout.Width(80) }))
            {
                SetAllColor(BlockColorType.purple);
            }
            if (GUILayout.Button("BlueSet", new GUILayoutOption[] { GUILayout.Width(80) }))
            {
                SetAllColor(BlockColorType.blue);
            }
            if (GUILayout.Button("GreenSet", new GUILayoutOption[] { GUILayout.Width(80) }))
            {
                SetAllColor(BlockColorType.green);
            }
            if (GUILayout.Button("YellowSet", new GUILayoutOption[] { GUILayout.Width(80) }))
            {
                SetAllColor(BlockColorType.yellow);
            }
            if (GUILayout.Button("OrangeSet", new GUILayoutOption[] { GUILayout.Width(80) }))
            {
                SetAllColor(BlockColorType.orange);
            }
            if (GUILayout.Button("PinkSet", new GUILayoutOption[] { GUILayout.Width(80) }))
            {
                SetAllColor(BlockColorType.pink);
            }
        }
        
        GUILayout.EndHorizontal();
    }

    void BossAllClear()
    {
        for (int i = 0; i < BlockList.Count; ++i)
        {
            if(isBossBlock(BlockList[i].type_))
            {
                BlockList[i].type_ = BlockTypes.Blank;
                BlockList[i].uType = BlockUseType.Empty;
                BlockList[i].health = 0;
                BlockList[i].colorType = BlockColorType.none;
            }
        }
    }

    void AddBlockClear()
    {
        for(int i=0; i < AddBlockList.Count; ++i)
        {
            AddBlockList[i].type_ = BlockTypes.Blank;
            AddBlockList[i].uType = BlockUseType.Empty;
            AddBlockList[i].health = 0;
            AddBlockList[i].colorType = BlockColorType.none;
        }
    }
    void BlockClear()
    {
        for (int i = 0; i < BlockList.Count; ++i)
        {
            BlockList[i].type_ = BlockTypes.Blank;
            BlockList[i].uType = BlockUseType.Empty;
            BlockList[i].health = 0;
            BlockList[i].colorType = BlockColorType.none;
        }
    }

    void SetAllColor(BlockColorType cType)
    {
        for (int i = 0; i < BlockList.Count; ++i)
        {
            if (BlockList[i].uType == BlockUseType.Block && !isBossBlock(BlockList[i].type_))
            {
                BlockList[i].colorType = cType;
            }
        }

        for (int i = 0; i < AddBlockList.Count; ++i)
        {
            if (AddBlockList[i].uType == BlockUseType.Block && !isBossBlock(BlockList[i].type_))
            {
                AddBlockList[i].colorType = cType;
            }
        }
    }
    void SetMapSizeChange(MapDataList savedatas)
    {
        if (BlockList.Count > 0)
        {
            int bcount = (maxCols * maxRows) - BlockList.Count;
            if (0 > bcount)
            {
                bcount *= -1;
                for (int i = 0; i < bcount; ++i)
                {
                    savedatas.myBlockDatas.RemoveAt(savedatas.myBlockDatas.Count-1);
                }
            }else
            {
                CreateEmptyBlockData(bcount, savedatas.myBlockDatas);
            }
        }
        if (AddBlockList.Count > 0)
        {
            int acount = (maxCols * maxRows) - AddBlockList.Count;
            if (0 > acount)
            {
                acount *= -1;
                for (int i = 0; i < acount; ++i)
                {
                    savedatas.AddBlockDatas.RemoveAt(savedatas.AddBlockDatas.Count - 1);
                }
            }
            else
            {
                CreateEmptyBlockData(acount, savedatas.AddBlockDatas);
            }
        }
    }

    void GUIStars()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();

        GUILayout.Label("Stars:", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.Label("Star1", new GUILayoutOption[] { GUILayout.Width(100) });
        GUILayout.Label("Star2", new GUILayoutOption[] { GUILayout.Width(100) });
        GUILayout.Label("Star3", new GUILayoutOption[] { GUILayout.Width(100) });
        GUILayout.Label("Multiple", new GUILayoutOption[] { GUILayout.Width(100) });
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        StarArrs[0] = EditorGUILayout.IntField("", StarArrs[0], new GUILayoutOption[] { GUILayout.Width(100) });
        StarArrs[1] = EditorGUILayout.IntField("", StarArrs[1], new GUILayoutOption[] { GUILayout.Width(100) });
        StarArrs[2] = EditorGUILayout.IntField("", StarArrs[2], new GUILayoutOption[] { GUILayout.Width(100) });
        isMultiple = GUILayout.Toggle(isMultiple, "");
        if (isMultiple)
        {
            if (scoreMultiple == 0) scoreMultiple = 35;

            scoreMultiple = EditorGUILayout.IntField("", scoreMultiple, new GUILayoutOption[] { GUILayout.Width(100) });
            if (GUILayout.Button("SET", new GUILayoutOption[] { GUILayout.Width(40), GUILayout.Height(20) }))
            {
                int blockCount = 0;
                if (curGameMode == GameMode.STAGEBOSS)
                {
                    for(int i=0; i< BlockList.Count; ++i)
                    {
                        if(isBossBlock(BlockList[i].type_))
                        {
                            blockCount += BlockList[i].health;
                        }
                    }
                    StarArrs[2] = blockCount * 10;
                }
                else
                {
                    blockCount = BlockList.Count(t => t.uType == BlockUseType.Block) + AddBlockList.Count(t => t.uType == BlockUseType.Block);
                    StarArrs[2] = blockCount * scoreMultiple;
                }
            }
            GUILayout.Label(" 블록 카운트에 배율을 정해서 점수를 자동으로 계산합니다.", new GUILayoutOption[] { GUILayout.Width(500) });
        }
        else
            GUILayout.Label("자동 점수 계산.", new GUILayoutOption[] { GUILayout.Width(500) });

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

    }
    void PreviousLevel()
    {
        if (levels > 0) --levels;
        changeLvl = (levels + 1).ToString();
        CallMapData();
    }

    void NextLevel()
    {   
        ++levels;
        changeLvl = (levels + 1).ToString();
        CallMapData();
    }

    void CallMapData()
    {
        MapDataLoad();
        if (curGameMode == GameMode.BALL100)
        {
            if (!File.Exists("Assets/Resources/NewMapData/Ball/" + (levels + 1) + ".txt"))
            {
                saveData_Ball = new MapDataList();
                CreateMapDataList(saveData_Ball);
            }
        }
        else if (curGameMode == GameMode.STAGE || curGameMode == GameMode.STAGEBOSS)
        {
            if (!File.Exists("Assets/Resources/NewMapData/Stage/" + (levels + 1) + ".txt"))
            {
                saveData_Stage = new MapDataList();
                CreateMapDataList(saveData_Stage);
            }
        }
        SetBlockData();
    }

    void MapDataSave()
    {
        string saveString = "";
        string activeDir = "";
        
        if (curGameMode == GameMode.BALL100)
        {
            activeDir = Application.dataPath + @"/Resources/NewMapData/Ball/";
            //if (!AssetDatabase.Contains(saveData_Ball))
            //{
            //    if (saveData_Ball.MapDataList == null || saveData_Ball.MapDataList.Count == 0) return;
            //    AssetDatabase.CreateAsset(saveData_Ball, Path + "Ball100.asset");
            //}else
            //{
            //    //saveData_Ball.MapDataList[levels].MyGM = curGameMode;
            //    //saveData_Ball.MapDataList[levels].myBlockDatas = BlockList;
            //    //saveData_Ball.MapDataList[levels].AddBlockDatas = AddBlockList;
            //    //saveData_Ball.MapDataList[levels].curCol = maxCols;
            //    //saveData_Ball.MapDataList[levels].curRow = maxRows;
            //    //saveData_Ball.MapDataList[levels].StartArr = StarArrs;

            //    MapData preData = new MapData();
            //    preData.MapDataList = saveData_Ball.MapDataList;

            //    AssetDatabase.DeleteAsset(Path + "Ball100.asset");
            //    AssetDatabase.CreateAsset(preData, Path + "Ball100.asset");
            //}
            saveData_Ball.StageIdx = levels + 1;
            saveData_Ball.MyGM = curGameMode;
            saveData_Ball.curCol = maxCols;
            saveData_Ball.curRow = maxRows;
            saveData_Ball.StartArr = StarArrs;
            saveString = ObjectToJson(saveData_Ball);
        }
        else
        {
            activeDir = Application.dataPath + @"/Resources/NewMapData/Stage/";
            //if (!AssetDatabase.Contains(saveData_Stage))
            //{
            //    if (saveData_Stage.MapDataList == null || saveData_Stage.MapDataList.Count == 0) return;
            //    AssetDatabase.CreateAsset(saveData_Stage, Path + "Stage.asset");
            //}else
            //{
            //    //saveData_Stage.MapDataList[levels].MyGM = curGameMode;
            //    //saveData_Stage.MapDataList[levels].BallCount = maxBalls;
            //    //saveData_Stage.MapDataList[levels].myBlockDatas = BlockList;
            //    //saveData_Stage.MapDataList[levels].AddBlockDatas = AddBlockList;
            //    //saveData_Stage.MapDataList[levels].curCol = maxCols;
            //    //saveData_Stage.MapDataList[levels].curRow = maxRows;
            //    //saveData_Stage.MapDataList[levels].StartArr = StarArrs;
            //    //saveData_Stage.MapDataList[levels].BallCount = maxBalls;

            if (curGameMode == GameMode.STAGEBOSS)
            {
                saveData_Stage.BossHPCount = BossHP;
                saveData_Stage.BossType = sb_Type;
                saveData_Stage.BossSpeed = stageBossTime;
            }

            //    //MapData preData = new MapData();
            //    //preData.MapDataList = saveData_Stage.MapDataList;

            //    //AssetDatabase.DeleteAsset(Path + "Stage.asset");
            //    //AssetDatabase.CreateAsset(preData, Path + "Stage.asset");
            //}
            saveData_Stage.StageIdx = levels + 1;
            saveData_Stage.MyGM = curGameMode;
            saveData_Stage.curCol = maxCols;
            saveData_Stage.curRow = maxRows;
            saveData_Stage.BallCount = maxBalls;
            saveString = ObjectToJson(saveData_Stage);
        }

        string newPath = System.IO.Path.Combine(activeDir, (levels+1) + ".txt");
        StreamWriter sw = new StreamWriter(newPath);
        sw.Write(saveString);
        sw.Close();
        AssetDatabase.Refresh();
    }

    void MapDataLoad()
    {
        LoadData();
        if (curGameMode == GameMode.STAGE || curGameMode == GameMode.STAGEBOSS)
            RealLoadMapData(saveData_Stage);
            
        if (curGameMode == GameMode.BALL100)
            RealLoadMapData(saveData_Ball);
    }

    void RealLoadMapData(MapDataList saveData)
    {
        if (saveData != null)
        {
            if (saveData== null) return;

            if(saveData.myBlockDatas != null)
            {
                BlockList = saveData.myBlockDatas;
                if(BlockList.Count == 0)
                {
                    BlockList = new List<BlockShow>();
                    CreateEmptyBlockData(maxCols * maxRows, BlockList);
                }
            }   
            else
            {
                BlockList = new List<BlockShow>();
                CreateEmptyBlockData(maxCols * maxRows , BlockList);
            }

            maxBalls = saveData.BallCount;
            
            for (int i = 0; i < StarArrs.Length; ++i)
            {
                StarArrs[i] = saveData.StartArr[i];
            }
            CreateBrickBtns();
        }
    }

    void MapDataPreLoad()
    {
        string url = "";
        int maxData = curGameMode == GameMode.BALL100 ? 31 : 1001;
        if (saveData_Stage == null) saveData_Stage = new MapDataList();
        if (saveData_Ball == null) saveData_Ball = new MapDataList();
        MapDataList data = new MapDataList();
        data.StageIdx = 1;
        switch (curGameMode)
        {
            case GameMode.BALL100:
                url = "Balls/" + (levels+1);
                TextAsset mapTextb = Resources.Load(url) as TextAsset;
                string[] linesb = mapTextb.ToString().Split('\n');
                data.MyGM = GameMode.BALL100;
                saveData_Ball = data;
                PreDataMakeList(linesb, saveData_Ball, levels);

                saveData_Ball.curCol = maxCols;
                saveData_Ball.curRow = maxRows;
                break;
            case GameMode.STAGE:
            case GameMode.STAGEBOSS:
                url = "Levels/" + levels;
                TextAsset mapTexts = Resources.Load(url) as TextAsset;
                string[] liness = mapTexts.ToString().Split('\n');
                data.MyGM = GameMode.STAGE;
                saveData_Stage = data;
                PreDataMakeList(liness, saveData_Stage, levels);
                saveData_Stage.curCol = maxCols;
                saveData_Stage.curRow = maxRows;
                break;
        }
        InitBlockData();
    }
    
    void InitBlockData()
    {
        //levels = 0;
        SetBlockData();
    }

    void SetBlockData()
    {
        switch (curGameMode)
        {
            case GameMode.BALL100:
                curGameMode = saveData_Ball.MyGM;
                maxBalls = saveData_Ball.BallCount;
                StarArrs = saveData_Ball.StartArr;
                BlockList = saveData_Ball.myBlockDatas;
                rownum = saveData_Ball.curRow;
                colnum = saveData_Ball.curCol;
                AddBlockList = saveData_Ball.AddBlockDatas;
                addLineNum = 0;
                break;
            case GameMode.STAGE:
            case GameMode.STAGEBOSS:
                curGameMode = saveData_Stage.MyGM;
                if (curGameMode == GameMode.STAGEBOSS)
                {
                    BossHP = saveData_Stage.BossHPCount;
                    sb_Type = saveData_Stage.BossType;
                    stageBossTime = saveData_Stage.BossSpeed;
                }
                maxBalls = saveData_Stage.BallCount;
                StarArrs = saveData_Stage.StartArr;
                rownum = saveData_Stage.curRow;
                colnum = saveData_Stage.curCol;
                BlockList = saveData_Stage.myBlockDatas;
                AddBlockList = saveData_Stage.AddBlockDatas;
                addLineNum =(AddBlockList==null)?0:(AddBlockList.Count/rownum);
                break;
        }
        maxCols = colnum;
        maxRows = rownum;
    }

    void PreDataMakeList(string[] lines, MapDataList mData, int idx)
    {
        for (int j = 0; j < lines.Length; ++j)
        {
            if (lines[j].StartsWith("MODE "))
            {
                string modeString = lines[j].Replace("MODE", string.Empty).Trim();
                if ((int.Parse(modeString) == 1))
                    mData.MyGM = GameMode.STAGEBOSS;
            }
            else if (lines[j].StartsWith("BALL "))
            {
                string modeString = lines[j].Replace("BALL", string.Empty).Trim();
                mData.BallCount = int.Parse(modeString);
            }
            else if (lines[j].StartsWith("STARS "))  // 0813 점수 관련 배율 추가로 인한 수정 김병권
            {
                string blocksString = lines[j].Replace("STARS", string.Empty).Trim();
                string[] starDatas = blocksString.Split(' ');
                string[] blocksNumbers = starDatas[0].Split('/');
                for (int i = 0; i < blocksNumbers.Length; ++i)
                {
                    mData.StartArr[i] = int.Parse(blocksNumbers[i]);
                }
            }
            else if (lines[j].StartsWith("BOSSHP "))
            {
                string modeString = lines[j].Replace("BOSSHP", string.Empty).Trim();
                string[] bossData = modeString.Split('/');
                mData.BossHPCount = int.Parse(bossData[0]);
                if (bossData.Length > 1)
                {
                    mData.BossType = (StageBossType)int.Parse(bossData[1]);
                    mData.BossSpeed = float.Parse(bossData[2]);
                }
            }
            else if (lines[j].StartsWith("ADDLINE "))
            {
                string addString = lines[j].Replace("ADDLINE", string.Empty).Trim();
                string[] st = addString.Split(' ');
                if (mData.AddBlockDatas == null) mData.AddBlockDatas = new List<BlockShow>();
                CreatNSetMapList(st, idx, mData.AddBlockDatas);
            }
            else
            {
                if (mData.myBlockDatas == null) mData.myBlockDatas = new List<BlockShow>();
                string[] st = lines[j].Split(' ');
                //for (int i = 0; i < st.Length; i++)
                //{
                //    string[] dt = st[i].Split(',');
                //    BlockShow data = new BlockShow();
                //    data.type_ = (BlockTypes)int.Parse(dt[0]);
                //    data.health = int.Parse(dt[1]);
                //    if (dt.Length > 2)
                //        data.uType = (BlockUseType)int.Parse(dt[2]);
                //    data.colorType = BlockColorType.none;
                //    mData.MapDataList[idx].myBlockDatas.Add(data);
                //}

                CreatNSetMapList(st, idx, mData.myBlockDatas);
            }
        }
    }

    void CreatNSetMapList(string[] st, int idx, List<BlockShow> setData)
    {
        for (int i = 0; i < st.Length; i++)
        {
            string[] dt = st[i].Split(',');
            BlockShow data = new BlockShow();
            data.type_ = (BlockTypes)int.Parse(dt[0]);
            data.health = int.Parse(dt[1]);
            if (dt.Length > 2)
                data.uType = (BlockUseType)int.Parse(dt[2]);
            data.colorType = BlockColorType.none;
            setData.Add(data);
        }
    }

    void SetBrickButton(Texture cuTx, string btnTxt, BlockTypes curType)
    {
        if (GUILayout.Button(cuTx, new GUILayoutOption[] { GUILayout.Width(30), GUILayout.Height(30) }))
        {
            curBlockType = curType;
        }
        
        GUILayout.Label(btnTxt, EditorStyles.boldLabel);
    }

    void CreateBrickBtns()
    {
        scrollViewVector = GUI.BeginScrollView(new Rect(25, 440, Screen.width - 100, Screen.height - 470), scrollViewVector, new Rect(0, 420, 200* maxCols, 50* (maxRows+addLineNum)));
        if(curGameMode == GameMode.STAGEBOSS)
            GUILayout.Space(50);
        else if(curGameMode == GameMode.STAGE)
            GUILayout.Space(100);
        else
            GUILayout.Space(200);

        AddBlockField();

        GUILayout.BeginVertical();
        for (int i=0; i< maxRows; ++i)
        {
            GUILayout.BeginHorizontal();
            for (int j=0; j< maxCols; ++j)
            {
                GUILayout.Space(10);
                int idx = i * maxCols + j;
                if(idx< BlockList.Count)
                {
                    GUILayout.Label((j + 1).ToString("D2")  + " , " + (i + 1).ToString("D2"), EditorStyles.boldLabel);
                   
                    if (GUILayout.Button(brickImgs.Find(t => t.name == (BlockList[idx].type_).ToString()).texture, new GUILayoutOption[] { GUILayout.Width(30), GUILayout.Height(30) }))
                    {
                        //if (j + 2 >= maxCols || i + 2 >= maxRows)
                        //{
                        //    if(IsGroupBlock(BlockList[idx].type_) && BlockList[idx].type_ != curBlockType)
                        //        curBlockType = BlockList[idx].type_;

                        //    if (IsGroupBlock(curBlockType))
                        //        curBlockType = BlockList[idx].type_;
                        //}

                        BlockList[idx].type_ = curBlockType;
                        if(IsHealthBlock(BlockList[idx].type_))
                            BlockList[idx].uType = BlockUseType.Block;
                        else
                        {
                            if (IsItem(BlockList[idx].type_))
                                BlockList[idx].uType = BlockUseType.Item;
                            else if (BlockList[idx].type_ == BlockTypes.Blank)
                                BlockList[idx].uType = BlockUseType.Empty;
                            else
                                BlockList[idx].uType = BlockUseType.Obstacle;
                            BlockList[idx].colorType = BlockColorType.none;
                        }
                        targetBlock = (j + 1)+ " , " + (i + 1);
                    }
                    BlockList[idx].colorType = (BlockColorType)EditorGUILayout.EnumPopup(BlockList[idx].colorType, GUILayout.Width(20));
                    if(BlockList[idx].colorType != BlockColorType.none)
                    {
                        string imgName = BlockList[idx].colorType.ToString() + SetColorIdx(BlockList[idx].health);
                        GUILayout.Button(colorImgs.Find(t=>t.name == imgName).texture, GUILayout.Width(30), GUILayout.Height(20));
                    }
                    BlockList[idx].health = EditorGUILayout.IntField(BlockList[idx].health, new GUILayoutOption[] { GUILayout.Width(40), GUILayout.Height(30) });
                    if (!IsHealthBlock(BlockList[idx].type_)) BlockList[idx].health = 0;
                }
                GUILayout.Space(10);
                if (j + 1 == maxCols) GUILayout.EndHorizontal();
            }
        }
        SetGroupData();
        SetBossData();
        GUILayout.EndVertical();
        GUI.EndScrollView();
    }
    int SetColorIdx(int health)
    {
        int ret = 0;

        if (50 <= health && health < 100)
            ret = 1;
        if (100 <= health)
            ret = 2;

        return ret;
    }
    void SetGroupData()
    {
        if (string.IsNullOrEmpty(targetBlock) || targetBlock.Contains("Add")) return;

        string[] nums = targetBlock.Split(',');
        int rownum = 0, colnum = 0, curRow = int.Parse(nums[1]) - 1, curCol = int.Parse(nums[0]) - 1;
        int idx = curRow * maxCols + curCol;
        BlockTypes curType = BlockList[idx].type_;
        if (IsGroupBlock(curType))
        {
            switch(curType)
            {
                case BlockTypes.blockS:
                    rownum = 2;
                    colnum = 2;
                    break;
                case BlockTypes.blockM:
                    rownum = 3;
                    colnum = 3;
                    break;
                case BlockTypes.blockL:
                    rownum = 4;
                    colnum = 4;
                    break;
                case BlockTypes.blockRetangle1:
                    rownum = 2;
                    colnum = 1;
                    break;
                case BlockTypes.blockRetangle2:
                    rownum = 1;
                    colnum = 2;
                    break;
                case BlockTypes.slideBlock_1:
                    rownum = 2;
                    colnum = 1;
                    break;
                case BlockTypes.slideBlock_2:
                    rownum = 3;
                    colnum = 1;
                    break;
                case BlockTypes.slideBlock_3:
                    rownum = 1;
                    colnum = 2;
                    break;
                case BlockTypes.slideBlock_4:
                    rownum = 1;
                    colnum = 3;
                    break;
                case BlockTypes.BombAll:
                    rownum = 2;
                    colnum = 2;
                    break;
            }

            for (int i = 0; i < rownum; ++i)
            { 
                for(int j=0; j < colnum; ++j)
                {
                    int IDX = (curRow + i) * maxCols + (curCol + j);
                    if(!IsGroupBlock(BlockList[IDX].type_))
                    {
                        BlockList[IDX].type_ = curType;
                        BlockList[IDX].uType = BlockUseType.Block;
                        BlockList[IDX].health = 0;
                    }
                }
            }
        }
    }
    void SetBossData()
    {
        if (string.IsNullOrEmpty(targetBlock) || targetBlock.Contains("Add")) return;

        string[] nums = targetBlock.Split(',');
        int rownum = 0, colnum = 0, curRow = int.Parse(nums[1]) - 1, curCol = int.Parse(nums[0]) - 1;
        int idx = curRow * maxCols + curCol;
        BlockTypes curType = BlockList[idx].type_;
        if (isBossBlock(curType))
        {
            switch (curType)
            {
                case BlockTypes.bluemon:
                    rownum = 4;
                    colnum = 6;
                    break;
                case BlockTypes.pinkmon:
                    rownum = 2;
                    colnum = 4;
                    break;
                case BlockTypes.chocomon:
                    rownum = 2;
                    colnum = 5;
                    break;
                case BlockTypes.lemon:
                    rownum = 4;
                    colnum = 3;
                    break;
                case BlockTypes.orangemon:
                    rownum = 3;
                    colnum = 3;
                    break;
                case BlockTypes.chocoballmon:
                    rownum = 2;
                    colnum = 2;
                    break;
                case BlockTypes.eggmon:
                    rownum = 2;
                    colnum = 3;
                    break;
                case BlockTypes.popmon_P:
                    rownum = 2;
                    colnum = 2;
                    break;
                case BlockTypes.popmon_W:
                    rownum = 2;
                    colnum = 2;
                    break;
                case BlockTypes.popmon_Y:
                    rownum = 2;
                    colnum = 2;
                    break;
                case BlockTypes.cakemon:
                    rownum = 2;
                    colnum = 3;
                    break;
                case BlockTypes.cookiemon:
                    rownum = 3;
                    colnum = 3;
                    break;
            }

            for (int i = 0; i < colnum; ++i)
            {
                for (int j = 0; j < rownum; ++j)
                {
                    int IDX = (curRow + i) * maxCols + (curCol + j);
                    if (!isBossBlock(BlockList[IDX].type_))
                    {
                        BlockList[IDX].type_ = curType;
                        BlockList[IDX].uType = BlockUseType.Block;
                        BlockList[IDX].health = 0;
                    }
                }
            }
        }
    }

    void AddBlockField()
    {
        if (AddBlockList == null || AddBlockList.Count == 0) return;
        GUILayout.BeginVertical();
        for (int i= addLineNum; i> 0; --i)
        {
            GUILayout.BeginHorizontal();
            for (int j= maxCols-1; j > -1; --j)
            {
                GUILayout.Space(10);
                int idx = (i * maxCols - j) - 1;
               
                if (idx< AddBlockList.Count)
                {
                    GUILayout.Label((maxCols - j) + " , " + i, EditorStyles.boldLabel);
                    if (GUILayout.Button(brickImgs.Find(t => t.name == (AddBlockList[idx].type_).ToString()).texture, new GUILayoutOption[] { GUILayout.Width(30), GUILayout.Height(30) }))
                    {
                        if (IsGroupBlock(curBlockType)) curBlockType = AddBlockList[idx].type_;

                        AddBlockList[idx].type_ = curBlockType;
                        if (IsHealthBlock(AddBlockList[idx].type_))
                            AddBlockList[idx].uType = BlockUseType.Block;
                        else
                        {
                            if (IsItem(AddBlockList[idx].type_))
                                AddBlockList[idx].uType = BlockUseType.Item;
                            else if (AddBlockList[idx].type_ == BlockTypes.Blank)
                                AddBlockList[idx].uType = BlockUseType.Empty;
                            else
                                AddBlockList[idx].uType = BlockUseType.Obstacle;

                            AddBlockList[idx].colorType = BlockColorType.none;
                        }

                        targetBlock = "Add : "+ (j + 1) + " , " + (i + 1);
                    }
                    AddBlockList[idx].colorType = (BlockColorType)EditorGUILayout.EnumPopup(AddBlockList[idx].colorType, GUILayout.Width(20));
                    if (AddBlockList[idx].colorType != BlockColorType.none)
                    {
                        string imgName = AddBlockList[idx].colorType.ToString() + SetColorIdx(AddBlockList[idx].health);
                        GUILayout.Button(colorImgs.Find(t => t.name == imgName).texture, GUILayout.Width(30), GUILayout.Height(20));
                    }
                    AddBlockList[idx].health = EditorGUILayout.IntField(AddBlockList[idx].health, new GUILayoutOption[] { GUILayout.Width(40), GUILayout.Height(30) });
                }
                GUILayout.Space(10);
                if (j == 0) GUILayout.EndHorizontal();
            }
        }
        GUILayout.Space(20);
        GUILayout.Label("---------------------------------------------------------------------------위로는 추가라인------------------------------------------------------------------------------------", EditorStyles.boldLabel);
        GUILayout.Space(20);
        GUILayout.EndVertical();
    }
    void AddLineCreat(int num)
    {
        for (int i = 0; i < num; ++i)
        {
            for (int j = 0; j < maxCols; ++j)
            {
                BlockShow data = new BlockShow();
                data.colorType = BlockColorType.none;
                AddBlockList.Add(data);
            }
        }
    }
    void CreateMapDataList(MapDataList tMapData)
    {
        tMapData.StageIdx = levels;
        tMapData.MyGM = curGameMode;
        tMapData.curCol = maxCols;
        tMapData.curRow = maxRows;
        tMapData.StartArr = StarArrs;
        tMapData.BallCount = maxBalls;
        if (tMapData.myBlockDatas == null) tMapData.myBlockDatas = new List<BlockShow>();
        if (tMapData.AddBlockDatas == null) tMapData.AddBlockDatas = new List<BlockShow>();

        CreateEmptyBlockData(maxCols * maxRows, tMapData.myBlockDatas);
        CreateEmptyBlockData(addLineNum * maxCols, tMapData.AddBlockDatas);

    }

    bool IsHealthBlock(BlockTypes type)
    {
        if (type == BlockTypes.Normal)
            return true;
        if (type == BlockTypes.Triangle1)
            return true;
        if (type == BlockTypes.Triangle2)
            return true;
        if (type == BlockTypes.Triangle3)
            return true;
        if (type == BlockTypes.Triangle4)
            return true;
        if (type == BlockTypes.BombNormal_Hor)
            return true;
        if (type == BlockTypes.BombNormal_Ver)
            return true;
        if (type == BlockTypes.BombNormal_XCro)
            return true;
        if (type == BlockTypes.BombNormal_Cro)
            return true;
        if (type == BlockTypes.BombNormal_Box)
            return true;
        if (type == BlockTypes.Speaker)
            return true;
        if (type == BlockTypes.Fixed)
            return true;
        if (type == BlockTypes.blockS)
            return true;
        if (type == BlockTypes.blockM)
            return true;
        if (type == BlockTypes.blockL)
            return true;
        if (type == BlockTypes.blockRetangle1)
            return true;
        if (type == BlockTypes.blockRetangle2)
            return true;
        if (type == BlockTypes.slideBlock_1)
            return true;
        if (type == BlockTypes.slideBlock_2)
            return true;
        if (type == BlockTypes.slideBlock_3)
            return true;
        if (type == BlockTypes.slideBlock_4)
            return true;
        if (type == BlockTypes.BombAll)
            return true;
        if (type == BlockTypes.bluemon)
            return true;
        if (type == BlockTypes.pinkmon)
            return true;
        if (type == BlockTypes.chocomon)
            return true;
        if (type == BlockTypes.lemon)
            return true;
        if (type == BlockTypes.orangemon)
            return true;
        if (type == BlockTypes.chocoballmon)
            return true;
        if (type == BlockTypes.eggmon)
            return true;
        if (type == BlockTypes.popmon_P)
            return true;
        if (type == BlockTypes.popmon_W)
            return true;
        if (type == BlockTypes.popmon_Y)
            return true;
        if (type == BlockTypes.cakemon)
            return true;
        if (type == BlockTypes.cookiemon)
            return true;
        if (type == BlockTypes.BombMini)
            return true;
        return false;
    }

    bool IsItem(BlockTypes type)
    {
        if (type == BlockTypes.Coin)
            return true;
        if (type == BlockTypes.AddBall)
            return true;
        if (type == BlockTypes.Laser_Cro)
            return true;
        if (type == BlockTypes.Laser_Hor)
            return true;
        if (type == BlockTypes.Laser_Ver)
            return true;
        if (type == BlockTypes.Laser_XCro)
            return true;
        if (type == BlockTypes.Bounce)
            return true;
        if (type == BlockTypes.Hole_In)
            return true;
        if (type == BlockTypes.Hole_Out)
            return true;
        if (type == BlockTypes.TriBounce)
            return true;

        return false;
    }

    bool IsGroupBlock(BlockTypes type)
    {
        if (type == BlockTypes.blockS)
            return true;
        if (type == BlockTypes.blockM)
            return true;
        if (type == BlockTypes.blockL)
            return true;
        if (type == BlockTypes.blockRetangle1)
            return true;
        if (type == BlockTypes.blockRetangle2)
            return true;
        if (type == BlockTypes.slideBlock_1)
            return true;
        if (type == BlockTypes.slideBlock_2)
            return true;
        if (type == BlockTypes.slideBlock_3)
            return true;
        if (type == BlockTypes.slideBlock_4)
            return true;
        if (type == BlockTypes.BombAll)
            return true;

        return false;
    }

    bool isBossBlock(BlockTypes type)
    {
        if (type == BlockTypes.bluemon)
            return true;
        if (type == BlockTypes.pinkmon)
            return true;
        if (type == BlockTypes.chocomon)
            return true;
        if (type == BlockTypes.lemon)
            return true;
        if (type == BlockTypes.orangemon)
            return true;
        if (type == BlockTypes.chocoballmon)
            return true;
        if (type == BlockTypes.eggmon)
            return true;
        if (type == BlockTypes.popmon_P)
            return true;
        if (type == BlockTypes.popmon_W)
            return true;
        if (type == BlockTypes.popmon_Y)
            return true;
        if (type == BlockTypes.cakemon)
            return true;
        if (type == BlockTypes.cookiemon)
            return true;

        return false;
    }

    string ObjectToJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    T JsonToOject<T>(string jsonData)
    {
        return JsonUtility.FromJson<T>(jsonData);
    }

}
