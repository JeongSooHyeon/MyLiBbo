using UnityEngine;

public class BottomWall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D cd)
    {
        if (cd.CompareTag("Ball"))
        {
            BallManager.instance.SetBall(cd.gameObject);
        }
    }
}
