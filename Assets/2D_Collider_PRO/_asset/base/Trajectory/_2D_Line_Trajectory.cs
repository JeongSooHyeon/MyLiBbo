using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[RequireComponent(typeof(LineRenderer))]
public class _2D_Line_Trajectory : MonoBehaviour
{
	[Space(10)]
	
	[SerializeField()]
	[Range(3,50)]
	int pointCount = 15;
	[SerializeField()]
	float point_distance = 1;
	[SerializeField()]
	LayerMask collision_layers;

	[SerializeField()]
	_2D_Throw_Object Throw_Object;

	LineRenderer pathLine;
	Collider2D hitCollider;
	Vector2 direction;
	float force;
	Vector2 first_point;
	List<Vector3> linePoints;
	bool calculate;

	void Awake()
	{
		pathLine = GetComponent<LineRenderer> ();
		linePoints = new List<Vector3> ();
	}

	void FixedUpdate()
	{
		if (!calculate)
			return;

		visualizePath();
	}

	void visualizePath()
	{
		Vector3[] points = new Vector3[pointCount];
		linePoints.Clear ();

		points[0] = first_point;
		
		//initial velocity
		Vector3 pointVelocity = direction * force * Time.deltaTime;
		
		// reset hit object
		hitCollider = null;


		for (int i = 1; i < pointCount; i++)
		{
			float pointTime;
			if(pointVelocity.sqrMagnitude != 0)
				pointTime = point_distance / pointVelocity.magnitude;
			else
				pointTime = 0;

			pointVelocity = pointVelocity + (Vector3)Physics2D.gravity * pointTime;
			
			RaycastHit2D hit = Physics2D.Raycast(points[i - 1], pointVelocity, point_distance , collision_layers.value);
			if (hit.collider)
			{
				hitCollider = hit.collider;
				
				points[i] = points[i - 1] + pointVelocity.normalized * hit.distance * 0.92f;
				pointVelocity = pointVelocity - (Vector3)Physics2D.gravity * (point_distance - hit.distance) / pointVelocity.magnitude;
				pointVelocity = Vector3.Reflect(pointVelocity, hit.normal);

				linePoints.Add(points[i]);
			}
			else
			{
				points[i] = points[i - 1] + pointVelocity * pointTime;
				linePoints.Add(points[i]);
			}
		}

		// Update LineRenderer points
		pathLine.positionCount = linePoints.Count;
		for (int i = 0; i < linePoints.Count; i++)
			pathLine.SetPosition(i, linePoints[i]);
	}






	// +++++++++++++++++    PUBLIC METHODES    +++++++++++++++++++++++

	/// <summary>
	/// Show the Trajectory Line.
	/// </summary>
	public void Show()
	{
		pathLine.enabled = true;
		calculate = true;
	}

	/// <summary>
	/// Hide the Trajectory Line.
	/// </summary>
	public void Hide()
	{
		pathLine.enabled = false;
		calculate = false;
	}

	/// <summary>
	/// Update_s the Trajectory path.
	/// </summary>
	public void Update_Values()
	{
		if (!Throw_Object)
			return;
		
		force = Throw_Object.GetForce ();
		direction = Throw_Object.GetDircetion ();
		first_point = Throw_Object.GetPoint ();
	}

	/// <summary>
	/// Set_s new ThrowObject reference.
	/// </summary>
	public void Set_Throw_Object(_2D_Throw_Object new_throw_obj)
	{
		if(new_throw_obj)
			Throw_Object = new_throw_obj;
	}
	// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++






	#if UNITY_EDITOR
	[UnityEditor.MenuItem("Tools/2DColliderPRO/Add/2D Trajectory/Line Trajectory" , false , 1)]
	static void Add_2D_Line_Trajectory()
	{
		if (UnityEditor.Selection.gameObjects.Length == 1)
		{
			UnityEditor.Selection.activeGameObject.AddComponent<_2D_Line_Trajectory>();
		}
		else
			Debug.Log ("Select only 1 GameObject");
	}
	#endif

}
