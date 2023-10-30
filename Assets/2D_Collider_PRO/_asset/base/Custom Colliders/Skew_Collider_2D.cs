using UnityEngine;
using System.Collections;



[ExecuteInEditMode()]
[RequireComponent(typeof(PolygonCollider2D))]
[AddComponentMenu("Physics 2D/Skew Collider 2D")]
public class Skew_Collider_2D : MonoBehaviour 
{

	[Space(15)]

	[SerializeField()]
	float width = 1;
	[SerializeField()]
	float height = 1;
	[SerializeField()]
	float skew_top = 0;
	[SerializeField()]
	float skew_bottom = 0;
	[SerializeField()]
	[Range(0,0.99f)]
	float top_balance = 0;
	[SerializeField()]
	[Range(0,0.99f)]
	float bottom_balance = 0;



	[SerializeField()]
	[HideInInspector()]
	PolygonCollider2D polyCol2D;

	[SerializeField()]
	[HideInInspector()]
	Vector2[] points;





	/// <summary>
	/// Update_s the collider with user values.
	/// </summary>
	/// <param name="_width"> min = 0.05f.</param>
	/// <param name="_height"> min = 0.05f.</param>
	/// <param name="_skew_top">_skew_top.</param>
	/// <param name="_skew_bottom">_skew_bottom.</param>
	/// <param name="_top_balance"> min = 0 , max = 0.99f.</param>
	/// <param name="_bottom_balance"> min = 0 , max = 0.99f.</param>
	public void Update_Collider(float _width=1, float _height=1, float _skew_top=0, float _skew_bottom=0 , float _top_balance=0 , float _bottom_balance=0)
	{
		_width = Mathf.Clamp (_width,0.05f,_width);
		_height = Mathf.Clamp (_height,0.05f,_height);
		_top_balance = Mathf.Clamp (_top_balance,0,0.99f);
		_bottom_balance = Mathf.Clamp (_bottom_balance,0,0.99f);

		width = _width;
		height = _height;
		skew_top = _skew_top;
		skew_bottom = _skew_bottom;
		top_balance = _top_balance;
		bottom_balance = _bottom_balance;

		_Update_Coll ();
	}





	// Updates the collider with inspector values
	void _Update_Coll() 
	{

		points [0] = new Vector2 (polyCol2D.offset.x + skew_top - ((width - top_balance*width)/2) , polyCol2D.offset.y + height/2);
		points [1] = new Vector2 (polyCol2D.offset.x + skew_top + ((width - top_balance*width)/2) , polyCol2D.offset.y + height/2);
		points [2] = new Vector2 (polyCol2D.offset.x + skew_bottom + ((width - bottom_balance*width)/2) , polyCol2D.offset.y - height/2);
		points [3] = new Vector2 (polyCol2D.offset.x + skew_bottom - ((width - bottom_balance*width)/2) , polyCol2D.offset.y - height/2);



		polyCol2D.points = points;
	}


	void OnEnable () 
	{
		if(polyCol2D == null)
		{
			polyCol2D = GetComponent<PolygonCollider2D>();
			points = new Vector2[4];
			
			_Update_Coll() ;
		}
	}



	//Called in the editor only
	void OnValidate()
	{
		if (Application.isPlaying)
			return;

		width = Mathf.Clamp (width,0.05f,width);
		height = Mathf.Clamp (height,0.05f,height);

		_Update_Coll() ;
	}

}
