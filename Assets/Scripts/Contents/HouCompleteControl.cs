using UnityEngine;

public class HouCompleteControl : MonoBehaviour
{
    [SerializeField] GameObject[] cha;
    public int idx;
    
    public void PlayAni()
    {
        //foreach (GameObject a in cha) a.SetActive(false);
        cha[idx].SetActive(true);
    }
}
