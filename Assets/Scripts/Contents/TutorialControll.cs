using UnityEngine;

public class TutorialControll : MonoBehaviour
{
    [SerializeField] GameObject[] tutorialPage;
    [SerializeField] UILabel nowPageText;
    [SerializeField] GameObject nextPageBtn;
    [SerializeField] GameObject prevPageBtn;
    int pageNum;

    void Start()
    {
        pageNum = 0;
        tutorialPage[pageNum].SetActive(true);
        nowPageText.text = (pageNum + 1) + " / " + tutorialPage.Length;
        prevPageBtn.SetActive(false);
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
}
