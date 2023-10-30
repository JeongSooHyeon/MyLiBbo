using UnityEngine;

public class EffectDestroy : MonoBehaviour
{
    void Start()
    {
        Invoke("DestroyThis", 1);
    }

    void DestroyThis()
    {
        Destroy(gameObject);
    }
}
