using UnityEngine;

public class GameStartBtn : MonoBehaviour {
    [SerializeField] GameMode myIdx;

    void OnClick()
    {
        SoundManager.instance.TapSound(1);
        DataManager.instance.GameStartCall((int)myIdx);
        switch(myIdx)
        {
            case GameMode.BALL100:
                DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.menu, 5);
                break;
            case GameMode.CLASSIC:
                DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.menu, 6);
                break;
        }
        
    }
}
