using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] Transform whiteHole;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ball"))
        collision.transform.position = whiteHole.position;   
    }
}
