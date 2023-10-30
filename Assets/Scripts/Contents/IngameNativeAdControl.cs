using UnityEngine;

public class IngameNativeAdControl : MonoBehaviour
{
    [SerializeField] GameObject NativeObj;
    [SerializeField] GameObject FakeNativeObj;

    public void CallNative()
    {
        if (!Application.isEditor)
        {
            AdsManager.instance.BannerEnable(false);
            AdsManager.instance.NativeEnable();
            NativeObj.SetActive(false);
                FakeNativeObj.SetActive(false);
        }
    }

    public void ClickFakeNativeAd()
    {
        Application.OpenURL(DataManager.TinyGolfURL);
    }

    public void CloseNative()
    {
        if (!Application.isEditor)
        {
            AdsManager.instance.BannerEnable(true);
            AdsManager.instance.NativeEnable(false);
        }
    }
}
