using UnityEngine;

public class PopupBGClick : MonoBehaviour
{
    public PopupBase CurPopup;

    void OnClick()
    {
        if (DataManager.instance.GetSceneType() == SceneType.Ingame && CurPopup.myTypeEnum != PopupType.emptyRewardAd) return;
        if (DataManager.instance.GetSceneType() == SceneType.Lobby)
        {
            switch (CurPopup.myTypeEnum)
            {
                case PopupType.stageScroll:
                case PopupType.tuto:
                    return;
            }
        }
        if (CurPopup != null)
            CurPopup.ClosePupTPTS();
    }
}
