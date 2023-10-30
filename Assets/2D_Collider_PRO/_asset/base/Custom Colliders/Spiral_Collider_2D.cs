using UnityEngine;
using System.Collections;


[ExecuteInEditMode()]
[RequireComponent(typeof(EdgeCollider2D))]
[AddComponentMenu("Physics 2D/Spiral Collider 2D")]
public class Spiral_Collider_2D : MonoBehaviour 
{
	[Space(15)]

	[SerializeField()]
	[Range(2,100)]
	int steps = 50;
	[SerializeField()]
	Vector2 start_point = Vector2.zero;
	[SerializeField()]
	float tetha = Mathf.PI*10;
	[SerializeField()]
	float alpha = 1;



	[SerializeField()][HideInInspector()]
	EdgeCollider2D edgeCol2D;
	Vector2[] points;









	/// <summary>
	/// Updates the collider with user values
	/// </summary>
	/// <param name="Steps">Steps.</param>
	/// <param name="startPoint">Start point.</param>
	/// <param name="tetha">Tetha.</param>
	/// <param name="alpha">Alpha.</param>
	public void Update_Collider(int _steps, Vector2 _startPoint, float _tetha, float _alpha)
	{
		steps = _steps;
		start_point = _startPoint;
		tetha = _tetha;
		alpha = _alpha;

		_Update_Coll ();
	}


	






	// Updates the collider with inspector values
	void _Update_Coll()
	{
		points = new Vector2[steps];
		for(int i=0; i < steps; ++i)
		{
			float t = (tetha/steps)*i;
			float a = (alpha/steps)*i;
			Vector2 v = new Vector2(start_point.x+a*Mathf.Cos(t), start_point.y+a*Mathf.Sin(t));
			points[i] = v;
		}
		
		edgeCol2D.points = points;
	}


	void OnEnable () 
	{
		if (edgeCol2D == null) 
		{
			edgeCol2D = GetComponent<EdgeCollider2D> ();
			_Update_Coll ();
		}
	}




	//Called in the editor only
	void OnValidate()
	{
		if (Application.isPlaying)
			return;
		
		_Update_Coll() ;
	}

}
