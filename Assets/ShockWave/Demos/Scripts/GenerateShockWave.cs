/*
GenerateShockWave.cs
This script generates the shockwave and the particleSystem in demo-scene4.
*/

using UnityEngine;

public class GenerateShockWave : MonoBehaviour {

    /// <summary>
    /// The ShockWave prefab.
    /// </summary>
    public GameObject prefab;

    /// <summary>
    /// The particle system.
    /// </summary>
    public new GameObject particleSystem;

    /// <summary>
    /// The mask for the raycast
    /// </summary>
    public LayerMask mask = -1;

    /// <summary>
    /// Raises the collision enter event.
    /// </summary>
    /// <param name="collision">Collision.</param>
    void OnCollisionEnter(Collision collision) 
    {
        //get the direction the projectile is moving
        Vector3 dir = collision.contacts[0].point - gameObject.transform.position;

        //create a Ray
        Ray ray = new Ray(gameObject.transform.position,dir);

        //the output value
        RaycastHit hit;

        //use the raycast
        if (Physics.Raycast (ray,out hit, 100,mask)) 
        {
            //create a shockwave by creating an object from a prefab
            GameObject obj = GameObject.Instantiate(prefab);

            //move the shockwave to the correct location and rotation
            obj.transform.rotation = Quaternion.FromToRotation (Vector3.forward, hit.normal);
            obj.transform.position = hit.point;
            obj.transform.position += obj.transform.forward * 0.1f;

            //if you need an example of how to use delegates...
            /*
            obj.GetComponent<ShockWave_WorldSpace>().OnAnimationComplete += delegateExample.printName;
            obj.GetComponent<ShockWave_WorldSpace>().OnAnimationComplete += delegateExample.printPosition;
            obj.GetComponent<ShockWave_WorldSpace>().OnAnimationComplete += delegateExample.reset;
            */
        }

        //make the particleSystem
        Instantiate(particleSystem,gameObject.transform.position,gameObject.transform.rotation);

        //destory the projectile
        Destroy(gameObject);
    }


}
