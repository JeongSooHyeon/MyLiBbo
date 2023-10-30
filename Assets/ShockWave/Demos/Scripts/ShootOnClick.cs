/*
ShootOnClick.cs
This script is will shoot a projectile on click.
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShootOnClick : MonoBehaviour 
{

    /// <summary>
    /// The projectile.
    /// </summary>
	public GameObject projectile;
 
    /// <summary>
    /// the force of the projectile
    /// </summary>
	public float force = 500f;


	// Update is called once per frame
	void Update () 
	{
		//On Click
		if (Input.GetMouseButtonDown(0))
		{

		    Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
      	
			Transform t = Camera.main.transform;
			GameObject p =  Instantiate(projectile,t.position,t.rotation) as GameObject;
			p.GetComponent<Rigidbody>().AddForce(ray.direction * force);
            
		}
	
	}
}
