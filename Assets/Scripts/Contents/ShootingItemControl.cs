using UnityEngine;

public class ShootingItemControl : MonoBehaviour
{
    [SerializeField] ShootingItemBtn[] btns;
    InstanceItem item;
    public void LevelUp(int idx)
    {
        item = btns[idx].item;
        switch (item)
        {
            case InstanceItem.PlusBall:
                btns[0].money = 3 * (int)Mathf.Pow((btns[0].level + 1), 3);
                btns[0].level++;
                break;
            case InstanceItem.AttackUp:
                btns[1].money = 15 * (int)Mathf.Pow((btns[1].level + 1), 3);
                btns[1].level++;
                break;
            case InstanceItem.FireUp:
                btns[2].money = 5 * (int)Mathf.Pow((btns[2].level + 1), 4);
                btns[2].level++;
                break;
            case InstanceItem.ProjUp:
                btns[3].money = 50 * (int)Mathf.Pow((btns[3].level + 1), 4);
                btns[3].level++;
                break;
        }
    }
}
