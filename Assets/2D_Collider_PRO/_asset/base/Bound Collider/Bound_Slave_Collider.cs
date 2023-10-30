using UnityEngine;
using System.Collections;



/// <summary>
/// This component created automatically and runtime . Don't edit.
/// </summary>
public class Bound_Slave_Collider : MonoBehaviour 
{

	_2D_Bound_Collider master_col;
	Collider2D col;


	void Awake()
	{
		col = GetComponent<Collider2D> ();
	}


	
	public void Set_Master_Collider(_2D_Bound_Collider _mater_col)
	{master_col = _mater_col;}
	public Collider2D Get_Collider()
	{return col;}



	void OnTriggerEnter2D(Collider2D col)
	{
		if (master_col.OnTriggerEnter_2D != null)
			master_col.OnTriggerEnter_2D.Invoke (col);
	}
	void OnTriggerExit2D(Collider2D col)
	{
		if (master_col.OnTriggerExit_2D != null)
			master_col.OnTriggerExit_2D.Invoke (col);
	}
	void OnTriggerStay2D(Collider2D col)
	{
		if (master_col.OnTriggerStay_2D != null)
			master_col.OnTriggerStay_2D.Invoke (col);
	}



	void OnCollisionEnter2D(Collision2D col)
	{
		if (master_col.OnCollisionEnter_2D != null)
			master_col.OnCollisionEnter_2D.Invoke (col);
	}
	void OnCollisionExit2D(Collision2D col)
	{
		if (master_col.OnCollisionExit_2D != null)
			master_col.OnCollisionExit_2D.Invoke (col);
	}
	void OnCollisionStay2D(Collision2D col)
	{
		if (master_col.OnCollisionStay_2D != null)
			master_col.OnCollisionStay_2D.Invoke (col);
	}
}
