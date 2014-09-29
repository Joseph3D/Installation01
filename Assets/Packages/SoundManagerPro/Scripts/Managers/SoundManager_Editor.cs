using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using antilunchbox;

public partial class SoundManager : Singleton<SoundManager> {
	/// EDITOR variables. DO NOT TOUCH.
	public bool viewAll {
		get {
			return _viewAll;
		} set {
			_viewAll = value;
			List<string> keys = new List<string>();
	        foreach (DictionaryEntry de in songStatus)
	            keys.Add(de.Key.ToString());
	
	        foreach(string key in keys)
	        {
				if(_viewAll)
				{
	            	songStatus[key] = VIEW;
				} else {
					songStatus[key] = HIDE;
				}
	        }
		}
	}
	/// EDITOR variables. DO NOT TOUCH.
	public const string VIEW = "view";
	/// EDITOR variables. DO NOT TOUCH.
	public const string EDIT = "edit";
	/// EDITOR variables. DO NOT TOUCH.
	public const string HIDE = "hide";
	private bool _viewAll = false;
	/// EDITOR variables. DO NOT TOUCH.
	[SerializeField]
	public Hashtable songStatus = new Hashtable();
	/// EDITOR variables. DO NOT TOUCH.
	public bool helpOn = false;
	/// EDITOR variables. DO NOT TOUCH.
	public bool showInfo = true;
	/// EDITOR variables. DO NOT TOUCH.
	public bool showDev = true;
	/// EDITOR variables. DO NOT TOUCH.
	public bool showList = true;
	/// EDITOR variables. DO NOT TOUCH.
	public bool showAdd = true;
	/// EDITOR variables. DO NOT TOUCH.
	public bool showSFX = true;
	/// EDITOR variables. DO NOT TOUCH.
	public List<bool> showSFXDetails = new List<bool>();
	/// EDITOR variables. DO NOT TOUCH.
	public int groupAddIndex = 0;
	/// EDITOR variables. DO NOT TOUCH.
	public int autoPrepoolAmount = 0;
	/// EDITOR variables. DO NOT TOUCH.
	public bool showAsGrouped = false;
}
