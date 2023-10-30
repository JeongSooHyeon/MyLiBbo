using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[ExecuteInEditMode()]
[RequireComponent(typeof(EdgeCollider2D))]
[AddComponentMenu("Physics 2D/Arc Collider 2D")]
public class Arc_Collider_2D : MonoBehaviour 
{
	[Space(20)]

	[SerializeField()]
	bool sliced = false;

	[Space(15)]

	[SerializeField()]
	[Range(0.1f,360)]
	float arc_angle = 90;
	
	[SerializeField()]
	[Range(0,360)]
	float start_angle = 0;

	[SerializeField()]
	[Range(1,7)]
	int smooth = 7; 

	[SerializeField()]
	float radius = 1;

	[Space(15)]

	[SerializeField()]
	bool ellipse = false;
	[SerializeField()]
	float x_radius = 1;
	[SerializeField()]
	float y_radius = 1;


	// privates
	int size;
	float theta;
	List<Vector2> points;
	[SerializeField()][HideInInspector()] EdgeCollider2D edgeCol2D;
	[SerializeField()][HideInInspector()] float[] smoot_data;









	void Update_Collider()
	{
		if (radius == 0)
			radius = 0.01f;

		if (!ellipse)
			x_radius = y_radius = 1;

		float theta_scale = smoot_data[smooth];
		size = (int)(1f / theta_scale);
		points = new List<Vector2> (size+1);
		theta = 0;

		for(int i = 0; i < size+1; i++)
		{	
			if(i != 0)
				theta += Mathf.Deg2Rad * arc_angle * theta_scale; 

			Vector2 v = Vector2.zero;
			v.x = radius * x_radius * Mathf.Cos(theta + Mathf.Deg2Rad * start_angle);
			v.y = radius * y_radius * Mathf.Sin(theta + Mathf.Deg2Rad * start_angle);
			points.Add(v);
		}

		if(sliced && arc_angle != 360)
		{
			points.Insert(0,Vector2.zero);
			points.Insert(points.Count,Vector2.zero);
		}


		edgeCol2D.points = points.ToArray();
	}



	void OnEnable()
	{
		if(edgeCol2D == null)
		{
			edgeCol2D = GetComponent<EdgeCollider2D>();
			smoot_data = new float[]{1f,0.5f,0.25f,0.125f,0.0625f,0.03125f,0.015625f,0.0078125f};
			Update_Collider();
		}
	}



	//Called in the editor only
	void OnValidate()
	{
		if (Application.isPlaying)
			return;

		Update_Collider ();
	}

}
