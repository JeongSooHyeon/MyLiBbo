using UnityEngine;

public class CannonControl : MonoBehaviour
{
    Vector3 dir;
    bool isTouch;
    public bool move;
    Animator anim;

    private void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
    }

    private void Update()
    {
        IsTouchOut();
        if (move && BallManager.instance.isMove)
        {
            isTouch = Input.GetMouseButton(0);
            //if (LobbyController.instance.getGameState() == UIState.InGame && Input.mousePosition.y >= 150)
               
           // else isTouch = false;
            if (isTouch)
            {
                dir = UICamera.lastWorldPosition - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
                if (UICamera.lastWorldPosition.x < 0 && angle <= -80) angle = 80;
                else if (UICamera.lastWorldPosition.x > 0 && angle >= 80) angle = -80;
                transform.localEulerAngles = new Vector3(0, 0, Mathf.Clamp(angle, -80, 80));
            }
        }
    }

    void IsTouchOut()
    {
        if (Input.GetMouseButtonDown(0))
            move = Input.mousePosition.y >= 150;
    }

    public void FireBall()
    {
        anim.SetTrigger("Shoot");
    }

    public void StopBall()
    {
        anim.SetTrigger("Stop");
    }
}
