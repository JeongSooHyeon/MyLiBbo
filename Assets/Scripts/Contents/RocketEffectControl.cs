using UnityEngine;

public class RocketEffectControl : MonoBehaviour {
    [SerializeField]Animator anim_;
    // Use this for initialization
    void Start () {
        if(anim_ == null)
            anim_ = GetComponent<Animator>();
    }

    void Update()
    {
        if(gameObject.activeSelf)
        {
            if (LobbyController.instance.getGameState() == UIState.InGame)
                anim_.SetBool("On", true);
            else
                anim_.SetBool("On", false);
        }
    }
}
