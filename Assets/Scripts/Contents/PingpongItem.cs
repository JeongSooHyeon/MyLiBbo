using UnityEngine;
using System.Collections;

public class PingpongItem : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D cd)
    {
        if (cd.gameObject.CompareTag("Ball"))
        {
            SoundManager.instance.ChangeEffects(3);
            cd.gameObject.GetComponent<BallMove>().ball_Pos_ = cd.transform.position;
            GameContents.instance.pingpongItem(cd.gameObject);
        }
    }

    public void SetPos(bool isTrue, Vector2 pos)
    {
        gameObject.SetActive(isTrue);
        transform.localPosition = pos;
    }
}
