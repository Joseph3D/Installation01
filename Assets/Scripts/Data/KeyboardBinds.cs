using UnityEngine;
using System.Collections;
using XMLEncrypto;

[System.Xml.Serialization.XmlRoot]
public sealed class KeyboardBinds : XMLBase {
	#region Movement Codes
	KeyCode back = KeyCode.None;
	KeyCode forward = KeyCode.None;
	KeyCode left = KeyCode.None;
	KeyCode right = KeyCode.None;

	public KeyCode Back {
		get {
			return this.back;
		} set {
			this.back = value;
		}
	}

	public KeyCode Forward {
		get {
			return this.forward;
		} set {
			this.forward = value;
		}
	}

	public KeyCode Left {
		get {
			return this.left;
		} set {
			this.left = value;
		}
	}

	public KeyCode Right {
		get {
			return this.right;
		} set {
			this.right = value;
		}
	}
	#endregion
	
	#region Action
	KeyCode action = KeyCode.None;
	KeyCode crouch = KeyCode.None;
	KeyCode flashLight = KeyCode.None;
	KeyCode jump = KeyCode.None;
	KeyCode melee = KeyCode.None;
	KeyCode primarySwitch = KeyCode.None;
	KeyCode reload = KeyCode.None;
	KeyCode secondarySwitch = KeyCode.None;
	KeyCode swap = KeyCode.None;

	public KeyCode Action {
		get {
			return this.action;
		} set {
			this.action = value;
		}
	}

	public KeyCode Crouch {
		get {
			return this.crouch;
		} set {
			this.crouch = value;
		}
	}

	public KeyCode FlashLight {
		get {
			return this.flashLight;
		} set {
			this.flashLight = value;
		}
	}

	public KeyCode Jump {
		get {
			return this.jump;
		} set {
			this.jump = value;
		}
	}

	public KeyCode Melee {
		get {
			return this.melee;
		} set {
			this.melee = value;
		}
	}

	public KeyCode PrimarySwitch {
		get {
			return this.primarySwitch;
		} set {
			this.primarySwitch = value;
		}
	}

	public KeyCode Reload {
		get {
			return this.reload;
		} set {
			this.reload = value;
		}
	}

	public KeyCode SecondarySwitch {
		get {
			return this.secondarySwitch;
		} set {
			this.secondarySwitch = value;
		}
	}

	public KeyCode Swap {
		get {
			return this.swap;
		} set {
			this.swap = value;
		}
	}
	#endregion

	#region Mouse
	MouseButtons primary = MouseButtons.LeftClick;
	MouseButtons secondary = MouseButtons.RightClick;
	MouseButtons zoom = MouseButtons.MiddleClick;

	public MouseButtons Primary {
		get {
			return this.primary;
		} set {
			this.primary = value;
		}
	}

	public MouseButtons Secondary {
		get {
			return this.secondary;
		} set {
			this.secondary = value;
		}
	}

	public MouseButtons Zoom {
		get {
			return this.zoom;
		} set {
			this.zoom = value;
		}
	}
	#endregion

	string version;

	[System.Xml.Serialization.XmlAttribute]
	public string Version {
		get {
			return this.version;
		} set {
			this.version = value;
		}
	}

	void Init() {
		this.version = "1.0.0";
		this.DefaultLayout();
	}

	void DefaultLayout() {
		this.Forward = KeyCode.W;
		this.Back = KeyCode.S;
		this.Left = KeyCode.A;
		this.Right = KeyCode.D;

		this.Action = KeyCode.E;
		this.Crouch = KeyCode.LeftControl;
		this.FlashLight = KeyCode.Q;
		this.Jump = KeyCode.Space;
		this.Melee = KeyCode.F;
		this.PrimarySwitch = KeyCode.Tab;
		this.Reload = KeyCode.R;
		this.SecondarySwitch = KeyCode.G;
		this.Swap = KeyCode.E;

		this.Primary = MouseButtons.LeftClick;
		this.Zoom = MouseButtons.MiddleClick;
		this.Secondary = MouseButtons.RightClick;
	}

	static XMLProperties xmlProperties;

	public KeyboardBinds() {
		KeyboardBinds.xmlProperties = new XMLProperties(typeof(KeyboardBinds).ToString(), typeof(KeyboardBinds), this, null, null, false);
	}

	public KeyboardBinds LoadXML() {
		if (DoesFileExits (KeyboardBinds.xmlProperties)) {
			return this.LoadXMLFile(KeyboardBinds.xmlProperties) as KeyboardBinds;
		} else {
			KeyboardBinds kbs = new KeyboardBinds();
			kbs.Init();
			kbs.CreateXMLFile(KeyboardBinds.xmlProperties);

			return this.LoadXMLFile(KeyboardBinds.xmlProperties) as KeyboardBinds;
		}
	}
}
