using UnityEngine;

public class ShootingTutoBackBg : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] TweenScale ts;
    int index;
    [SerializeField] GameObject[] step_;

    private void Start()
    {
        index = 0;
    }

    void OnClick()
    {
        if (index < 2)
        {
            anim.SetTrigger("Next");
            index++;
        }
            
        else
        {
            ts.PlayReverse();
            ts.gameObject.SetActive(false);
            for (int i = 0; i < BrickManager.instance.bundleList.Count; i++)
            {
                BrickManager.instance.bundleList[i].tPos.enabled = true;
            }
            LobbyController.instance.SetBack();
            DataManager.instance.ShootingTuto = true;
            BallManager.instance.SetFire(true);
        }
    }
}
