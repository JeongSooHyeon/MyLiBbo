using UnityEngine;

public class ExitPupControl : PopupBase
{
    [SerializeField] GameObject NativeObj;
    [SerializeField] PreloadControl PreControl;
    [SerializeField] GameObject FakeNativeObj;

    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        if (DataManager.instance.GetSceneType() == SceneType.Lobby)
            LobbyManager.instance.curPopup = this;
        if (DataManager.instance.GetSceneType() == SceneType.Preload)
        {
            PreControl.isExitOpen = true;
            Time.timeScale = 0f;
        }

        if (!Application.isEditor)
        {
            if (DataManager.instance.GetSceneType() == SceneType.Lobby)
            {
                AdsManager.instance.BannerEnable(false);
                AdsManager.instance.NativeEnable();
            }

            NativeObj.SetActive(false);
            FakeNativeObj.SetActive(false);
        }
    }

    public override void ClosePupTPTS()
    {
        base.ClosePupTPTS();
        if (DataManager.instance.GetSceneType() == SceneType.Lobby)
            LobbyManager.instance.curPopup = null;

        if (DataManager.instance.GetSceneType() == SceneType.Preload)
        {
            PreControl.isExitOpen = false;
            Time.timeScale = 1f;
        }

        if (!Application.isEditor)
        {
            if (DataManager.instance.GetSceneType() == SceneType.Lobby)
            {
                AdsManager.instance.BannerEnable(true);
                AdsManager.instance.NativeEnable(false);
            }
        }
    }

    public void ClickFakeAds()
    {
        Application.OpenURL(DataManager.TinyGolfURL);
    }

    public void ClickYesBtn()
    {
        ClosePupTPTS();
        Application.Quit();
    }

    public void ClickNoBtn()
    {
        SoundManager.instance.TapSound();
        ClosePupTPTS();
    }

    //public void ClickMoreGame()
    //{
    //    ClickNoBtn();
    //    LobbyManager.instance.CallMoreGame();
    //}
}
