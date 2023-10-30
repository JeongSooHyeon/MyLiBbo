using UnityEngine;

public class PreloadControl : MonoBehaviour
{
    public static PreloadControl instance;
    [SerializeField] UIToggle TestModeBtn;
    [SerializeField] UIToggle ResetStageBtn;
    [SerializeField] UILabel TestModeTxt;
    [SerializeField] ExitPupControl exitPup;
    public bool isExitOpen;

    private void Start()
    {
        if (instance == null)
            instance = this;

        TestModeBtn.gameObject.SetActive(Application.isEditor);
        ResetStageBtn.gameObject.SetActive(Application.isEditor);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isExitOpen)
                exitPup.CallPupTPTS();
            else
                exitPup.ClosePupTPTS();
        }
    }

    public void CheckTestModeStart()
    {
        DataManager.instance.isTestMode = TestModeBtn.value;
        if (TestModeBtn.value)
        {
            TestModeTxt.text = "TEST MODE";
            DeleteData();
            for (int i = 0; i < DataManager.instance.stageStarList.Count; i++)
            {
                DataManager.instance.TotalStarCount += DataManager.instance.stageStarList[i];
            }
        }
        else
        {
            TestModeTxt.text = "LIVE MODE";
            CheckTestResetStage();
        }

        DataManager.instance.CallStageData();
    }

    public void CheckTestResetStage()
    {
        if (ResetStageBtn.value)
        {
            DeleteData();
        }
        DataManager.instance.CallStageData();
    }

    public void CheckLongTimeReward()
    {
        DataManager.instance.SceneLoad();
        //DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.new_user, 0);
    }

    void DeleteData()
    {
        PlayerPrefs.DeleteAll();
        DataManager.instance.StartBall = 1;
        DataManager.instance.FirePower = 1;
        DataManager.instance.CurGameStage = 1;
        DataManager.instance.SetCoin(300);

        if (TestModeBtn.value)
        {
            DataManager.instance.LobbyTutorial = true;
            DataManager.instance.SetCoin(99700);
        }
    }
}
