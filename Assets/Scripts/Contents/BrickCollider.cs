using UnityEngine;

public class BrickCollider : MonoBehaviour
{
    public Brick brick_;
    void Start()
    {
        brick_ = GetComponentInParent<Brick>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            if (brick_ == null)
            {
                brick_ = GetComponentInParent<Brick>();
            }

            brick_.SetCollider(collision.gameObject);
        }
    }

    public void PassBall(GameObject go)
    {
        brick_.SetCollider(go);
    }
}
