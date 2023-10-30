using UnityEngine;
using System.Collections;



[RequireComponent(typeof(Rigidbody2D))]
public class _2D_Throw_Object : MonoBehaviour 
{
	[Space(10)]

	[SerializeField()]
	float throwForce;

	Rigidbody2D rigid;
	Vector3 init_pos;
	Vector3 init_rot;



	// Use this for initialization
	void Start ()
	{
		rigid = GetComponent<Rigidbody2D> ();
		Save_Pos ();
	}






	// +++++++++++++++++    PUBLIC METHODES    +++++++++++++++++++++++

	public float GetForce()
	{	return throwForce / rigid.mass;	}

	public Vector2 GetDircetion()
	{	return (Vector2)transform.up;	}

	public Vector2 GetPoint()
	{	return (Vector2)transform.position;	}


	/// <summary>
	/// Apply force to Transform.Up vector to throw this object.
	/// </summary>
	public void Throw()
	{
		// Throw
		rigid.isKinematic = false;
		rigid.AddForce(throwForce * transform.up);
	}

	/// <summary>
	/// Save_s the current position and rotation.
	/// </summary>
	public void Save_Pos()
	{
		init_pos = rigid.transform.position;
		init_rot = rigid.transform.eulerAngles;
	}

	/// <summary>
	/// Reset position, rotation and velocity of this object.
	/// </summary>
	public void Reset()
	{
		rigid.transform.position = init_pos;
		rigid.transform.eulerAngles = init_rot;
		rigid.velocity = Vector2.zero;
		rigid.angularVelocity = 0;
	}

	// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++




	void OnDestroy()
	{
		//here you can instantiate blow or any effect/particle prefab
	}




	#if UNITY_EDITOR
	[UnityEditor.MenuItem("Tools/2DColliderPRO/Add/2D Trajectory/Throw Object")]
	static void Add_2D_Line_Trajectory()
	{
		if (UnityEditor.Selection.gameObjects.Length > 0)
		{
			for (int i = 0; i < UnityEditor.Selection.gameObjects.Length; i++)
				UnityEditor.Selection.gameObjects[i].AddComponent<_2D_Throw_Object>();
		}
		else
			Debug.Log ("Select GameObject(s)");
	}
	#endif
}
