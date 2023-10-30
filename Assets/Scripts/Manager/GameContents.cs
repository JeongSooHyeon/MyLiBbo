using UnityEngine;
using System.Collections;

public class GameContents : MonoBehaviour
{
    public static GameContents instance;
    [SerializeField] GameObject[] item;
    [SerializeField] int[] itemCall, maximum;
    [SerializeField] ParticleSystem[] boxBombPs;
    [SerializeField] ParticleSystem allBombPs;
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < itemCall.Length; ++i)
        {
            itemCall[i] = i * 3;
            maximum[i] = itemCall[i] + 2;
        }
    }

    public void gameitem(Vector2 pos, int idx)
    {
        item[itemCall[idx]].transform.position = pos;
        if (idx >= 9) boxBombPs[(itemCall[idx] + 3) % 6].Play();
        StartCoroutine(Gameitemoff(itemCall[idx]));
        ++itemCall[idx];
        if (maximum[idx] < itemCall[idx]) itemCall[idx] = maximum[idx] - 2;
    }

    public void gameItemAllBomb(int idx)
    {
        item[idx].transform.position = Vector3.zero;
        allBombPs.Play();
        StartCoroutine("Gameitemoff", idx);
    }

    IEnumerator Gameitemoff(int id)
    {
        if (id > 14) yield return new WaitForSeconds(0.2f);
        else yield return new WaitForSeconds(0.05f);
        item[id].gameObject.transform.localPosition = new Vector2(10000, 10000);
    }

    public void pingpongItem(GameObject cd) // 컬라이더를 인자값으로 넘기는 것은 비용이 비싸기 때문에 하지 않는다. 그래서 게임 오브젝트로 받았다.
    {
        int forcex = Random.Range(-60, 60);
        cd.transform.localEulerAngles = new Vector3(0, 0, forcex);
        cd.GetComponent<BallMove>().rd.velocity = cd.transform.up * BallManager.instance.force;
        cd.transform.localEulerAngles = Vector3.zero; // 축이 틀어져 있는 상태에서 하면 안되기 때문에 다시 바꿔준다.
    }
}

