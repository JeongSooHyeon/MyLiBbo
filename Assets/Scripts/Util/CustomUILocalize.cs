using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UIWidget))]
[AddComponentMenu("NGUI/UI/Localize")]
public class CustomUILocalize : MonoBehaviour
 {

	public bool isFront;
    public bool isDouble;
    public int AddCenterData;
    public int AddDataInt;
	public string AddDataStr;

	public string key;
	public string value
	{
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				UIWidget w = GetComponent<UIWidget>();
				UILabel lbl = w as UILabel;
				UISprite sp = w as UISprite;

				if (lbl != null)
				{
					// If this is a label used by input, we should localize its default value instead
					UIInput input = NGUITools.FindInParents<UIInput>(lbl.gameObject);
					if (input != null && input.label == lbl) input.defaultText = value;
					else lbl.text = value;
#if UNITY_EDITOR
					if (!Application.isPlaying) NGUITools.SetDirty(lbl);
#endif
				}
				else if (sp != null)
				{
					UIButton btn = NGUITools.FindInParents<UIButton>(sp.gameObject);
					if (btn != null && btn.tweenTarget == sp.gameObject)
						btn.normalSprite = value;

					sp.spriteName = value;
					sp.MakePixelPerfect();
#if UNITY_EDITOR
					if (!Application.isPlaying) NGUITools.SetDirty(sp);
#endif
				}
			}
		}
	}

	bool mStarted = false;

	/// <summary>
	/// Localize the widget on enable, but only if it has been started already.
	/// </summary>

	void OnEnable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		if (mStarted) OnLocalize();
	}

	/// <summary>
	/// Localize the widget on start.
	/// </summary>

	void Start ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		mStarted = true;
		OnLocalize();
	}

	/// <summary>
	/// This function is called by the Localization manager via a broadcast SendMessage.
	/// </summary>

	public void OnLocalize ()
	{
		// If no localization key has been specified, use the label's text as the key
		if (string.IsNullOrEmpty(key))
		{
			UILabel lbl = GetComponent<UILabel>();
			if (lbl != null) key = lbl.text;
		}

        if(AddCenterData > 0)
        {
            if(isDouble)
            {
                string txt = string.Format("{0:n0}", AddCenterData);
                string txt2 = string.Format("{0:n0}", AddDataInt);
                string des = Localization.Get(key).Replace("%1%", txt).Replace("%2%", txt2);
                value = des;
            }
            else
            {
                string txt = string.Format("{0:n0}", AddCenterData);
                string des = Localization.Get(key).Replace("%%", txt);
                value = des;
            }
        }
        else
        {
            // If we still don't have a key, leave the value as blank
            if (!string.IsNullOrEmpty(key))
            {
                string addTxt = "";
                addTxt = AddDataInt > 0 ? AddDataInt.ToString() : AddDataStr;
                if (isFront)
                    value = string.Format("{0} {1}", addTxt, Localization.Get(key));
                else
                    value = string.Format("{0} {1}", Localization.Get(key),addTxt);
            }
        }
	}
}
