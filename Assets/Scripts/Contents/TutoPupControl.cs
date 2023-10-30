using UnityEngine;

public class TutoPupControl : PopupBase
{
    [SerializeField] GameObject[] tutorialPage;
    [SerializeField] UILabel nowPageText;
    [SerializeField] GameObject nextPageBtn;
    [SerializeField] GameObject prevPageBtn;
    [SerializeField] OptionPopupControl optionPup;
    int pageNum;
    
    void ResetTutoState()
    {
        pageNum = 0;
        for (int i = 0; i < tutorialPage.Length; ++i)
        {
            tutorialPage[i].SetActive(i == pageNum);
        }
        nowPageText.text = (pageNum + 1) + " / " + tutorialPage.Length;
        prevPageBtn.SetActive(false);
        nextPageBtn.SetActive(true);
    }

    public void nextPage()
    {
        SoundManager.instance.TapSound();
        prevPageBtn.SetActive(true);
        tutorialPage[pageNum].SetActive(false);
        ++pageNum;
        tutorialPage[pageNum].SetActive(true);
        nowPageText.text = (pageNum + 1) + " / " + tutorialPage.Length;
        nextPageBtn.SetActive(pageNum < tutorialPage.Length - 1);
    }

    public void prePage()
    {
        SoundManager.instance.TapSound();
        nextPageBtn.SetActive(true);
        tutorialPage[pageNum].SetActive(false);
        --pageNum;
        tutorialPage[pageNum].SetActive(true);
        nowPageText.text = (pageNum + 1) + " / " + tutorialPage.Length;
        prevPageBtn.SetActive(pageNum > 0);
    }

    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        ResetTutoState();
        LobbyManager.instance.curPopup = this;
    }

    public override void ClosePupTPTS()
    {
        base.ClosePupTPTS();
        LobbyManager.instance.curPopup = optionPup;
    }
}
