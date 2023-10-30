using UnityEngine;

public class GaChaBall : MonoBehaviour
{
    public Rigidbody2D rd;

    void Start()
    {
        rd = GetComponent<Rigidbody2D>();
        GaChaManager.instance.gaChaBall.Add(this);
    }
}
