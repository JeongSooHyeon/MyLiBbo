using UnityEngine;

public class CharSkill : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Brick"))
        {
            col.gameObject.GetComponent<BrickCollider>().brick_.hitCnt -= 1500;
            if (col.gameObject.GetComponent<BrickCollider>().brick_.hitCnt <= 0)
            {
                col.gameObject.GetComponent<BrickCollider>().brick_.FalseMySelf();
            }
            else col.gameObject.GetComponent<BrickCollider>().brick_.LabelSetting();

            BrickManager.instance.SetSaveSkillBrick();
        }
    }
}
