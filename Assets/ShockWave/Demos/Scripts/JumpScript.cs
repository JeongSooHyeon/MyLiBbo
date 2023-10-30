/*
JumpScript.cs
Used to make the block jump!
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpScript : MonoBehaviour {


    public float jumpForce;
    public GameObject prefab;
    public Vector3 swPositionOffset;

	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameObject.GetComponent<Rigidbody>().AddForce(0f,jumpForce,0f);


            //create a shockwave by creating an object from a prefab
            GameObject obj = GameObject.Instantiate(prefab);

            //move the shockwave to the correct location and rotation
            //obj.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            //obj.transform.position = hit.point;
            obj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            obj.transform.position = transform.position + swPositionOffset;
        }
	}
}
