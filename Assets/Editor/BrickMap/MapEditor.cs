using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Linq;

public class MapEditor : EditorWindow
{
    static MapEditor window;
    int maxRows = 9;
    int maxCols = 9;
    int levels = 1;
    int maxBalls = 1;
    int[] StarArrs = new int[3];
    int BossHP = 1;
    //int stages = 1;
    private string fileName = "1.txt";
    BlockTypes b_Types;
    GameMode g_Types;
    StageBossType sb_Type;
    BlockUseType bu_Type;
    float stageBossTime;
    bool isStageMode = true;
    bool isBossStage;
    Texture blankTx, normalTx, TriTx_1, TriTx_2, TriTx_3, TriTx_4,SpeTx,HorTx,VerTx,XCrossTx,CrossTx,BounceTx,AddBallTx,CoinTx,HoleInTx,HoleOutTx,BombTx_1,BombTx_2, BombTx_3, BombTx_4, BombTx_5, FixTx,TBounce;
    Vector2 scrollViewVector;
    TextureCover cover_;

    Texture[] arrayTex;

    public List<BlockShow> stageBlocks = new List<BlockShow>();

    // 추가 블록 행 관련
    //bool isAddLine;
    int addLineNum = 0;
    public static BlockShow[] addBlocks;

    // 점수 관련 블록 갯수
    int blockCount = 0;
    // 점수 관련 배수
    int scoreMultiple = 35;
    bool isMultiple;


    [MenuItem("Window/Map Edior")]
    public static void Init()
    {
        window = (MapEditor)GetWindow(typeof(MapEditor));
        window.Show();
    }
    int isibalSave;
    void OnGUI()
    {
        if (Event.current.keyCode == KeyCode.S)
        {
            if (isibalSave == 0) SaveLevel();
            ++isibalSave;
            if (isibalSave > 3) isibalSave = 0;
          
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save", new GUILayoutOption[] { GUILayout.Width(40), GUILayout.Height(40) })) SaveLevel();
        if (GUILayout.Button("Clear", new GUILayoutOption[] { GUILayout.Width(40), GUILayout.Height(40) })) ClearLavel();
        if (GUILayout.Button("Stage", new GUILayoutOption[] { GUILayout.Width(40), GUILayout.Height(40) }))
        {
            isStageMode = true;
            //stages = 1;
            levels = 1;
            SetStageData();
        }
        if (GUILayout.Button("Balls", new GUILayoutOption[] { GUILayout.Width(40), GUILayout.Height(40) }))
        {
            isStageMode = false;
            levels = 1;
            SetStageData();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(30);
        GUILayout.Label("Stage editor", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });
        GUILevelSelector();
        GUITarget();
        GUIBlocks();
        scrollViewVector = GUI.BeginScrollView(new Rect(25, 510, position.width - 30, position.height - 30), scrollViewVector, new Rect(0, 0, 400, 3700));
        if(isStageMode)
            GUILayout.Space(-500);
        else
            GUILayout.Space(-300);
        GUIAddLine();
        GameField();
        GUI.EndScrollView();
        
        if (isMultiple)
            SetMultiple();

    }

    private void OnFocus()
    {
        Initialize();
        LoadDataFromLocal(levels);
        ChangeTexture();      
    }

    void ChangeTexture()
    {
        if (SceneManager.GetActiveScene().name == "MapEditor")
        {
            if (cover_ == null) cover_ = GameObject.Find("TextureCover").GetComponent<TextureCover>();
            arrayTex = new Texture[cover_.tex_.Length];
            arrayTex = cover_.tex_;
            
            blankTx = arrayTex[0];
            normalTx = arrayTex[1];
            TriTx_1 = arrayTex[2];
            TriTx_2 = arrayTex[3];
            TriTx_3 = arrayTex[4];
            TriTx_4 = arrayTex[5];
            SpeTx = arrayTex[6];
            VerTx = arrayTex[7];
            HorTx = arrayTex[8];
            XCrossTx = arrayTex[9];
            CrossTx = arrayTex[10];
            BounceTx = arrayTex[11];
            AddBallTx = arrayTex[12];
            CoinTx = arrayTex[13];

            // 텍스쳐 변경
            HoleInTx = arrayTex[14];
            HoleOutTx = arrayTex[15];

            // 폭탄블럭 텍스쳐 변경 필요
            BombTx_1 = arrayTex[16];
            BombTx_2 = arrayTex[17];
            BombTx_3 = arrayTex[18];
            BombTx_4 = arrayTex[19];
            BombTx_5 = arrayTex[20];

            FixTx = arrayTex[21];
            TBounce = arrayTex[22];
        }
        else Debug.LogError("PLZ MapEditor Scene ");
    }

    void Initialize()
    {
        //stageBlocks = new BlockShow[maxCols * maxRows];
        if (stageBlocks == null) stageBlocks = new List<BlockShow>();
        else stageBlocks.Clear();

        for (int i = 0; i < maxCols * maxRows; i++)
        {
            BlockShow sqBlocks = new BlockShow();
            stageBlocks.Add(sqBlocks);
        }
        ClearLavel();
    }

    void ClearInfoData()
    {
        isBossStage = false;
        maxBalls = 1;
        BossHP = 1;
        sb_Type = StageBossType.normal;
        stageBossTime = 0;

        isMultiple = false;
        //isAddLine = false;
        addLineNum = 0;
    }

    void ClearLavel()
    {
        ClearInfoData();
        for (int i = 0; i < StarArrs.Length; ++i)
        {
            StarArrs[i] = 0;
        }
        for (int i = 0; i < stageBlocks.Count; i++)
        {
            stageBlocks[i].type_ = BlockTypes.Blank;
            stageBlocks[i].health = 0;
        }
        AddBlockClear();
    }
    void SaveLevel()
    {
        if (!fileName.Contains(".txt"))
            fileName += ".txt";
        SaveMap(fileName);
    }

    public void SaveMap(string fileName)
    {
        string saveString = "";
        if(g_Types != GameMode.BALL100)
        {
            saveString += "MODE " + (isBossStage?1:0);
            saveString += "\r\n";
            saveString += "BALL " + maxBalls;
            saveString += "\r\n";
            if (isMultiple)
                saveString += "STARS " + StarArrs[0] + "/" + StarArrs[1] + "/" + StarArrs[2] + " " + scoreMultiple;
            else
                saveString += "STARS " + StarArrs[0] + "/" + StarArrs[1] + "/" + StarArrs[2];
            saveString += "\r\n";
            if (isBossStage)
            {
                saveString += "BOSSHP " + BossHP + "/" + (int)sb_Type +"/"+stageBossTime;
                saveString += "\r\n";
            }
            if(addLineNum > 0)
            {
                for(int row = 0; row < addLineNum; row++)
                {
                    saveString += "ADDLINE ";
                    for(int col = 0; col < maxCols; col++)
                    {
                        saveString += (int)addBlocks[row * maxCols + col].type_ + "," + addBlocks[row * maxCols + col].health + "," + (int)addBlocks[row * maxCols + col].uType;
                        if (col < (maxCols - 1))
                            saveString += " ";
                    }
                    if (row < (addLineNum))
                        saveString += "\r\n";
                }
            }
        }
        //set map data
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                saveString += (int)stageBlocks[row * maxCols + col].type_ +","+ stageBlocks[row * maxCols + col].health + "," + (int)stageBlocks[row * maxCols + col].uType;
                //if this column not yet end of row, add space between them
                if (col < (maxCols - 1))
                    saveString += " ";
            }
            //if this row is not yet end of row, add new line symbol between rows
            if (row < (maxRows - 1))
                saveString += "\r\n";
        }
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
        {
            //Write to file
            int writecnt;
            string activeDir = "";

            if (g_Types == GameMode.BALL100)
            {
                writecnt = levels;
                activeDir = Application.dataPath + @"/Resources/Balls/";
            }
            else
            {
                //writecnt = stages * 10000;
                //writecnt += levels;
                writecnt = levels;
                activeDir = Application.dataPath + @"/Resources/Levels/";
            }
            
            string newPath = System.IO.Path.Combine(activeDir, writecnt + ".txt");
            StreamWriter sw = new StreamWriter(newPath);
            sw.Write(saveString);
            sw.Close();
        }
        AssetDatabase.Refresh();
    }
    string nIntField;
    int rownum = 0, colnum = 0;
    void GUILevelSelector()
    {
        GUILayout.BeginHorizontal();
        
        GUILayout.Label("Level:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(50) });
        GUILayout.Space(10);
        if (GUILayout.Button("<<", new GUILayoutOption[] { GUILayout.Width(50) })) PreviousLevel();
        string changeLvl = GUILayout.TextField(levels.ToString(), new GUILayoutOption[] { GUILayout.Width(50) });
        try
        {
              levels = int.Parse(changeLvl);
            if (int.Parse(changeLvl) != levels)
            {
                if (!LoadDataFromLocal(levels))
                {
                    ClearLavel();
                    return;
                }
                ChangeTexture();
                LoadDataFromLocal(levels);
            }
        }
        catch (Exception) { throw; }

        if (GUILayout.Button(">>", new GUILayoutOption[] { GUILayout.Width(50) })) NextLevel();
        string url = "Assets/Resouces/";
        if (isStageMode) url += "Levels/";
        else url += "Balls/";
        GUILayout.Label(url, EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(200) });

        //EditorGUI.LabelField(new Rect(startX + 40, startY + 20, 70, 15), cont);
        //curChapterNum = EditorGUI.IntField(new Rect(startX + 110, startY + 20, 40, 15), curChapterNum);
        
        if (GUILayout.Button("사이즈 수정", new GUILayoutOption[] { GUILayout.Width(100) }))
        {
            maxRows = rownum;
            maxCols = colnum;
        }
        
        rownum = int.Parse(GUILayout.TextField(rownum.ToString(), new GUILayoutOption[] { GUILayout.Width(50) }));
        colnum = int.Parse(GUILayout.TextField(colnum.ToString(), new GUILayoutOption[] { GUILayout.Width(50) }));

        GUILayout.EndHorizontal();
    }

    void NextLevel()
    {
        ClearInfoData();
        ++levels;
        LoadDataFromLocal(levels);
        if (!LoadDataFromLocal(levels))
        {
            ClearLavel();
            return;
        }
    }

    void PreviousLevel()
    {
        ClearInfoData();
        --levels;
        if (levels < 1)
            levels = 1;
        SetStageData();
    }

    void SetStageData()
    {
        if (!LoadDataFromLocal(levels))
        {
            ClearLavel();
            return;
        }
        ChangeTexture();
        LoadDataFromLocal(levels);
    }

    public bool LoadDataFromLocal(int currentStage)
    {
        ClearLogConsole();
        Debug.LogError(levels);
        //Read data from text file
        TextAsset mapText;
        string url = "";
        if (isStageMode) url += "Levels/" + levels;
        else url += "Balls/" + levels;

        mapText = Resources.Load(url) as TextAsset;

        if (mapText == null)
        {
            return false;
        }
        ProcessGameDataFromString(mapText.text);
        return true;
    }

    void ProcessGameDataFromString(string mapText)
    {
        string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

        int mapLine = 0;
        int addLine = 0;

        addLineNum = 0;

        foreach (string line in lines)
        {
            if (line.StartsWith("ADDLINE "))
                addLineNum++;
        }
        AddBlockClear();

        foreach (string line in lines)
        {
            if (line.StartsWith("MODE "))
            {
                string modeString = line.Replace("MODE", string.Empty).Trim();
                isBossStage = (int.Parse(modeString) == 1);
            }
            else if (line.StartsWith("BALL "))
            {
                string modeString = line.Replace("BALL", string.Empty).Trim();
                maxBalls = int.Parse(modeString);
            }
            else if (line.StartsWith("STARS "))
            {
                string blocksString = line.Replace("STARS", string.Empty).Trim();
                string[] starDatas = blocksString.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (starDatas.Length == 1)
                {
                    isMultiple = false;
                    scoreMultiple = 35;
                }
                else
                {
                    isMultiple = true;
                    scoreMultiple = int.Parse(starDatas[1]);
                }
                string[] blocksNumbers = starDatas[0].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < blocksNumbers.Length; ++i)
                {
                    StarArrs[i] = (int.Parse(blocksNumbers[i]));
                }
            }
            else if (line.StartsWith("BOSSHP "))
            {
                string modeString = line.Replace("BOSSHP", string.Empty).Trim();
                string[] bossData = modeString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);   
                BossHP = int.Parse(bossData[0]);
                if (bossData.Length > 1)
                {
                    sb_Type = (StageBossType)int.Parse(bossData[1]);
                    stageBossTime = float.Parse(bossData[2]);
                }
            }
            else if(line.StartsWith("ADDLINE "))
            {
                string addString = line.Replace("ADDLINE", string.Empty).Trim();
                string[] st = addString.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < st.Length; i++)
                {
                    string[] dt = st[i].Split(',');
                    addBlocks[addLine * maxCols + i].type_ = (BlockTypes)int.Parse(dt[0]);
                    addBlocks[addLine * maxCols + i].health = int.Parse(dt[1]);
                    if (dt.Length > 2)
                        addBlocks[addLine * maxCols + i].uType = (BlockUseType)int.Parse(dt[2]);
                }
                addLine++;
            }
            else
            {
                string[] st = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < st.Length; i++)
                {
                    string[] dt = st[i].Split(',');
                    stageBlocks[mapLine * maxCols + i].type_ = (BlockTypes)int.Parse(dt[0]);
                    stageBlocks[mapLine * maxCols + i].health = int.Parse(dt[1]);
                    if(dt.Length > 2)
                        stageBlocks[mapLine * maxCols + i].uType = (BlockUseType)int.Parse(dt[2]);
                }
                ++mapLine;
            }
        }

        //if (addLine > 0)
        //    isAddLine = true;

        List<BlockShow> stageBlockList = stageBlocks.ToList();
        List<BlockShow> addBlockList = addBlocks.ToList();

        //Debug.Log(blockTypeQuery.Count());
        Debug.Log(stageBlockList.Count(t => t.uType == BlockUseType.Block));
        //Debug.Log(addTypeQuery.Count());
        Debug.Log(addBlockList.Count(t => t.uType == BlockUseType.Block));

        blockCount = stageBlockList.Count(t => t.uType == BlockUseType.Block) + addBlockList.Count(t => t.uType == BlockUseType.Block);

        Debug.Log(blockCount);

        if(isMultiple)
            StarArrs[2] = blockCount * scoreMultiple;
    }
 
    void GUIBlocks()
    {
        GUILayout.Label("Map Block:", EditorStyles.boldLabel);
        GUILayout.Label("Utils", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        GUI.color = new Color(1, 1, 1, 1f);
        SetBrickButton(blankTx, " - Blank", BlockTypes.Blank);
        SetBrickButton(normalTx, " - Normal", BlockTypes.Normal);
        SetBrickButton(TriTx_1, " - TriLB", BlockTypes.Triangle1);
        SetBrickButton(TriTx_2, " - TriRB", BlockTypes.Triangle2);
        SetBrickButton(TriTx_3, " - TriLT", BlockTypes.Triangle3);
        SetBrickButton(TriTx_4, " - TriRT", BlockTypes.Triangle4);
        SetBrickButton(SpeTx, " - Speaker", BlockTypes.Speaker);

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        SetBrickButton(VerTx, " - LaserV", BlockTypes.Laser_Ver);
        SetBrickButton(HorTx, " - LaserH", BlockTypes.Laser_Hor);
        SetBrickButton(XCrossTx, " - LaserX", BlockTypes.Laser_XCro);
        SetBrickButton(CrossTx, " - LaserC", BlockTypes.Laser_Cro);
        SetBrickButton(BounceTx, " - Bounce", BlockTypes.Bounce);
        SetBrickButton(AddBallTx, " - AddBall", BlockTypes.AddBall);
        SetBrickButton(CoinTx, " - Coin", BlockTypes.Coin);
        SetBrickButton(HoleInTx, " - HoleIn", BlockTypes.Hole_In);
        SetBrickButton(HoleOutTx, " - HoleOut", BlockTypes.Hole_Out);
        // 폭탄
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        SetBrickButton(BombTx_1, " - BombHor", BlockTypes.BombNormal_Hor);
        SetBrickButton(BombTx_2, " - BombVer", BlockTypes.BombNormal_Ver);
        SetBrickButton(BombTx_3, " - BombXCro", BlockTypes.BombNormal_XCro);
        SetBrickButton(BombTx_4, " - BombCro", BlockTypes.BombNormal_Cro);
        SetBrickButton(BombTx_5, " - BombBox", BlockTypes.BombNormal_Box);

        SetBrickButton(FixTx, " - FixedBlock", BlockTypes.Fixed);
        SetBrickButton(TBounce, " - TriBounce", BlockTypes.TriBounce);

        GUILayout.EndHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginHorizontal();
        GUILayout.Label("CurrentSelect : ", EditorStyles.boldLabel);
        GUILayout.Box(SetImgButton(b_Types) as Texture, new GUILayoutOption[] {
                        GUILayout.Width (30),
                        GUILayout.Height (30)
                    });
        if (g_Types == GameMode.STAGE)
        {
            GUILayout.Space(30);
            GUILayout.Label("AddLine:", EditorStyles.boldLabel);
            GUILayout.Space(30);
            addLineNum = EditorGUILayout.IntField(addLineNum, new GUILayoutOption[] { GUILayout.Width(50) });
            GUILayout.Space(30);
            if (GUILayout.Button("CLEAR", new GUILayoutOption[] { GUILayout.Width(50) })) { AddBlockClear(); }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(50);
    }

    void GUIAddLine()
    {
        if(g_Types == GameMode.STAGE)
           AddBlocksField();
        GUILayout.Space(10);
    }

    void AddBlockClear()
    {
        Debug.Log(addLineNum);
        addBlocks = new BlockShow[addLineNum * maxCols];

        for (int i = 0; i < addBlocks.Length; i++)
        {
            BlockShow sqBlocks = new BlockShow();
            addBlocks[i] = sqBlocks;

            addBlocks[i].type_ = BlockTypes.Blank;
            addBlocks[i].health = 0;
        }
    }

    void AddBlockResize()
    {
        for(int i = 0; i < addBlocks.Length; i++)
        {
            if(addBlocks[i] == null)
            {
                BlockShow sqBlocks = new BlockShow();
                addBlocks[i] = sqBlocks;

                addBlocks[i].type_ = BlockTypes.Blank;
                addBlocks[i].health = 0;
            }
                
        }
    }

    void AddBlocksField()
    {
        Array.Resize(ref addBlocks, addLineNum * maxCols);
        AddBlockResize();
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        for (int row = addLineNum - 1; row >= 0; --row)
        {
            GUILayout.BeginHorizontal();

            for (int col = 0; col < maxCols; ++col)
            {
                GUILayout.Label(row + " , " + col, EditorStyles.boldLabel);

                GUI.color = new Color(1, 1, 1, 1f);
                if (GUILayout.Button(SetImgButton(addBlocks[row * maxCols + col].type_) as Texture, new GUILayoutOption[] {
                        GUILayout.Width (30),
                        GUILayout.Height (30)
                    }))
                {
                    SetAddBlockType(col, row);
                }
                if (IsHealthBlock(addBlocks[row * maxCols + col].type_))
                {
                    addBlocks[row * maxCols + col].uType = BlockUseType.Block;
                    addBlocks[row * maxCols + col].health = EditorGUILayout.IntField(addBlocks[row * maxCols + col].health, new GUILayoutOption[] {
                        GUILayout.Width (40),
                        GUILayout.Height (30)
                    });
                    GUILayout.Space(10);
                }
                else
                {
                    if(IsItem(addBlocks[row * maxCols + col].type_))
                    //if (addBlocks[row * maxCols + col].type_ == BlockTypes.Coin || addBlocks[row * maxCols + col].type_ == BlockTypes.AddBall)
                        addBlocks[row * maxCols + col].uType = BlockUseType.Item;
                    else if (addBlocks[row * maxCols + col].type_ == BlockTypes.Blank)
                        addBlocks[row * maxCols + col].uType = BlockUseType.Empty;
                    else
                        addBlocks[row * maxCols + col].uType = BlockUseType.Obstacle;

                    addBlocks[row * maxCols + col].health = EditorGUILayout.IntField(0, new GUILayoutOption[] {
                        GUILayout.Width (40),
                        GUILayout.Height (30)
                    }); ;
                    GUILayout.Space(10);
                }
            }
            GUILayout.EndHorizontal();
        }
        if(addLineNum > 0)
            GUILayout.Label("---------------------------------------------------------------------------위로는 추가라인------------------------------------------------------------------------------------", EditorStyles.boldLabel);

        GUILayout.EndVertical();
    }

    void GameField()
    {
        GUILayout.BeginVertical();
        for (int row = 0; row < maxRows; ++row)
        {
            GUILayout.BeginHorizontal();

            for (int col = 0; col < maxCols; ++col)
            {
                GUILayout.Label(row + " , " + col, EditorStyles.boldLabel);

                GUI.color = new Color(1, 1, 1, 1f);
                if (GUILayout.Button(SetImgButton(stageBlocks[row * maxCols + col].type_) as Texture, new GUILayoutOption[] {
                        GUILayout.Width (30),
                        GUILayout.Height (30)
                    }))
                {
                    SetType(col, row);
                }
                if(IsHealthBlock(stageBlocks[row * maxCols + col].type_))
                {
                    stageBlocks[row * maxCols + col].uType = BlockUseType.Block;
                    stageBlocks[row * maxCols + col].health = EditorGUILayout.IntField(stageBlocks[row * maxCols + col].health, new GUILayoutOption[] {
                        GUILayout.Width (40),
                        GUILayout.Height (30)
                    });
                    GUILayout.Space(10);
                }
                else
                {
                    if(IsItem(stageBlocks[row * maxCols + col].type_))
                        stageBlocks[row * maxCols + col].uType = BlockUseType.Item;
                    else if(stageBlocks[row * maxCols + col].type_ == BlockTypes.Blank)
                        stageBlocks[row * maxCols + col].uType = BlockUseType.Empty;
                    else
                        stageBlocks[row * maxCols + col].uType = BlockUseType.Obstacle;

                    stageBlocks[row * maxCols + col].health = EditorGUILayout.IntField(0, new GUILayoutOption[] {
                        GUILayout.Width (40),
                        GUILayout.Height (30)
                    });
                    GUILayout.Space(10);
                }
                    
            }
            GUILayout.EndVertical();
        }
    }

    void GUITarget()
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        if (isStageMode)
            g_Types = GameMode.STAGE;
        else
            g_Types = GameMode.BALL100;

        if (g_Types == GameMode.STAGE)
        {
            GUILayout.Label("StageBoss:", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.BeginVertical();
            isBossStage = GUILayout.Toggle(isBossStage, "");
            if(isBossStage)
            {
                GUILayout.BeginHorizontal();
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
            GUILayout.Label("StageBall:", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.BeginVertical();
            maxBalls = EditorGUILayout.IntField(maxBalls, new GUILayoutOption[] {
                        GUILayout.Width (40),
                        GUILayout.Height (30)
                    });
            GUIStars();
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
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
        if(isMultiple)
        {
            scoreMultiple = EditorGUILayout.IntField("", scoreMultiple, new GUILayoutOption[] { GUILayout.Width(100) });
            //if (GUILayout.Button("SET", new GUILayoutOption[] { GUILayout.Width(40), GUILayout.Height(20) })) SetMultiple();
            GUILayout.Label(" 블록 카운트에 배율을 정해서 점수를 자동으로 계산합니다.", new GUILayoutOption[] { GUILayout.Width(500) });
        }
        else
            GUILayout.Label("자동 점수 계산.", new GUILayoutOption[] { GUILayout.Width(500) });

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

    }

    void SetMultiple()
    {
        StarArrs[2] = blockCount * scoreMultiple;
    }

    void SetType(int col , int row)
    {
        Debug.LogError(col + " " + row);
        stageBlocks[row * maxCols + col].type_ = b_Types;
    }
    
    void SetAddBlockType(int col, int row)
    {
        Debug.LogError("AddBlock" + col + " " + row);
        addBlocks[row * maxCols + col].type_ = b_Types;
    }

    public static void ClearLogConsole()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        Type logEntries = assembly.GetType("UnityEditor.LogEntries");
        if (logEntries != null)
        {
            MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
            clearConsoleMethod.Invoke(new object(), null);
        }
    }

    void SetBrickButton(Texture cuTx, string btnTxt, BlockTypes curType)
    {
        if (GUILayout.Button(cuTx, new GUILayoutOption[] { GUILayout.Width(30), GUILayout.Height(30) }))
        {
            b_Types = curType;
        }
        GUILayout.Label(btnTxt, EditorStyles.boldLabel);
    }

    object SetImgButton(BlockTypes type)
    {
        var imageButton = new object();
        switch (type)
        {
            case BlockTypes.Blank:
                return imageButton = blankTx;
            case BlockTypes.Normal:
                return imageButton = normalTx;
            case BlockTypes.Triangle1:
                return imageButton = TriTx_1;
            case BlockTypes.Triangle2:
                return imageButton = TriTx_2;
            case BlockTypes.Triangle3:
                return imageButton = TriTx_3;
            case BlockTypes.Triangle4:
                return imageButton = TriTx_4;
            case BlockTypes.Speaker:
                return imageButton = SpeTx;
            case BlockTypes.Laser_Ver:
                return imageButton = VerTx;
            case BlockTypes.Laser_Hor:
                return imageButton = HorTx;
            case BlockTypes.Laser_XCro:
                return imageButton = XCrossTx;
            case BlockTypes.Laser_Cro:
                return imageButton = CrossTx;
            case BlockTypes.Bounce:
                return imageButton = BounceTx;
            case BlockTypes.AddBall:
                return imageButton = AddBallTx;
            case BlockTypes.Coin:
                return imageButton = CoinTx;
            case BlockTypes.Hole_In:
                return imageButton = HoleInTx;
            case BlockTypes.Hole_Out:
                return imageButton = HoleOutTx;

                // 폭탄이미지 추가
            case BlockTypes.BombNormal_Hor:
                return imageButton = BombTx_1;
            case BlockTypes.BombNormal_Ver:
                return imageButton = BombTx_2;
            case BlockTypes.BombNormal_XCro:
                return imageButton = BombTx_3;
            case BlockTypes.BombNormal_Cro:
                return imageButton = BombTx_4;
            case BlockTypes.BombNormal_Box:
                return imageButton = BombTx_5;

            case BlockTypes.Fixed:
                return imageButton = FixTx;
            case BlockTypes.TriBounce:
                return imageButton = TBounce;
        }
        return null;
    }

    bool IsHealthBlock(BlockTypes type)
    {   
        if(type == BlockTypes.Normal)
            return true;
        if (type == BlockTypes.Triangle1)
            return  true;
        if (type == BlockTypes.Triangle2)
            return  true;
        if (type == BlockTypes.Triangle3)
            return  true;
        if (type == BlockTypes.Triangle4)
            return  true;
        if (type == BlockTypes.BombNormal_Hor)
            return  true;
        if (type == BlockTypes.BombNormal_Ver)
            return  true;
        if (type == BlockTypes.BombNormal_XCro)
            return  true;
        if (type == BlockTypes.BombNormal_Cro)
            return  true;
        if (type == BlockTypes.BombNormal_Box)
            return true;
        if (type == BlockTypes.Speaker)
            return true;
        if (type == BlockTypes.Fixed)
            return true;

        return false;
    }

    // 아이템 판단
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
}