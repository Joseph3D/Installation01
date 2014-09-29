using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

[Serializable]
public class AudioSubscription {
	// owner audiosourcepro
	public AudioSourcePro owner;
	
	// standard event
	public bool isStandardEvent = true;
	public AudioSourceStandardEvent standardEvent;
	
	// custom event
	public Component sourceComponent;
	public string methodName = "";
	private bool isBound = false;
	
	// result action
	public AudioSourceAction actionType = AudioSourceAction.None;
	public string cappedName;
	public bool filterLayers;
	public bool filterTags;
	public bool filterNames;
	public int layerMask;
	public int tagMask;
	public int nameMask;
	public string nameToAdd = "";
	public List<string> tags = new List<string>() { "Default" };
	public List<string> names = new List<string>();
	public List<string> allNames = new List<string>();
	
	// event info
	private Component targetComponent;
	private FieldInfo eventField;
	private Delegate eventDelegate;
	private MethodInfo handlerProxy;
	private ParameterInfo[] handlerParameters;
	
	public void Bind(AudioSourcePro sourcePro)
	{
		if(isBound || isStandardEvent || sourceComponent == null)
			return;
		
		owner = sourcePro;

		if(!componentIsValid)
		{
			Debug.LogError(string.Format( "Invalid binding configuration - Source:{0}", sourceComponent));
			return;
		}
		
		MethodInfo eventHandlerMethodInfo = getMethodInfoForAction(actionType);
		
		targetComponent = owner;
		
		eventField = getField(sourceComponent, methodName);
		if(eventField == null)
		{
			Debug.LogError( "Event definition not found: " + sourceComponent.GetType().Name + "." + methodName );
			return;
		}
		
		try
		{
			var eventMethod = eventField.FieldType.GetMethod("Invoke");
			var eventParams = eventMethod.GetParameters();
			
			eventDelegate = createProxyEventDelegate(targetComponent, eventField.FieldType, eventParams, eventHandlerMethodInfo);
		}
		catch(Exception err)
		{
			Debug.LogError("Event binding failed - Failed to create event handler: " + err.ToString());
			return;
		}
		
		var combinedDelegate = Delegate.Combine( eventDelegate, (Delegate)eventField.GetValue( sourceComponent ) );
		eventField.SetValue( sourceComponent, combinedDelegate );
		
		isBound = true;
	}
	
	public void Unbind()
	{
		if(!isBound)
			return;

		isBound = false;
		
		var currentDelegate = (Delegate)eventField.GetValue(sourceComponent);
		var newDelegate = Delegate.Remove(currentDelegate, eventDelegate);
		eventField.SetValue(sourceComponent, newDelegate);

		eventField = null;
		eventDelegate = null;
		handlerProxy = null;

		targetComponent = null;
	}
	
	public bool componentIsValid {
		get {
			if(standardEventIsValid)
				return true;
			
			var propertiesSet =
				sourceComponent != null &&
				!string.IsNullOrEmpty( methodName );
			
			if(!propertiesSet)
				return false;

			var member = sourceComponent.GetType().GetMember(methodName).FirstOrDefault();
			
			if(member == null)
				return false;

			return true;
		}
	}
	
	public bool standardEventIsValid {
		get {
			if(isStandardEvent && Enum.IsDefined(typeof(AudioSourceStandardEvent), methodName))
				return true;
			return false;
		}
	}
	
	private FieldInfo getField(Component sourceComponent, string fieldName)
	{
		return sourceComponent.GetType()
			.GetAllFieldInfos()
			.Where(f => f.Name == fieldName)
			.FirstOrDefault();
	}
	
	private bool signatureIsCompatible(ParameterInfo[] lhs, ParameterInfo[] rhs)
	{
		if(lhs == null || rhs == null)
			return false;

		if(lhs.Length != rhs.Length)
			return false;

		for(int i = 0; i < lhs.Length; i++)
		{
			if(!areTypesCompatible(lhs[i], rhs[i]))
				return false;
		}

		return true;
	}

	private bool areTypesCompatible(ParameterInfo lhs, ParameterInfo rhs)
	{
		if(lhs.ParameterType.Equals(rhs.ParameterType))
			return true;
		
		if(lhs.ParameterType.IsAssignableFrom(rhs.ParameterType))
			return true;
		
		return false;
	}
	
	[ProxyEvent]
	private void CallbackProxy()
	{
		callProxyEventHandler();
	}
	
	private Delegate createProxyEventDelegate(object target, Type delegateType, ParameterInfo[] eventParams, MethodInfo eventHandler)
	{
		var proxyMethod = typeof(AudioSubscription)
			.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
			.Where(m =>
				m.IsDefined(typeof(ProxyEventAttribute), true ) &&
				signatureIsCompatible(eventParams, m.GetParameters())
			)
			.FirstOrDefault();
		
		if(proxyMethod == null)
			return null;
		
		handlerProxy = eventHandler;
		handlerParameters = eventHandler.GetParameters();

		var eventDelegate = Delegate.CreateDelegate( delegateType, this, proxyMethod, true );
		
		return eventDelegate;
	}
	
	private void callProxyEventHandler(params object[] arguments)
	{
		if(handlerProxy == null)
			return;

		if(handlerParameters.Length == 0)
			arguments = null;

		var result = new object();
		switch(actionType)
		{
		case AudioSourceAction.Play:
			result = handlerProxy.Invoke(targetComponent, new object[] {});
			break;
		case AudioSourceAction.PlayCapped:
			result = handlerProxy.Invoke(targetComponent, new object[] {cappedName});
			break;
		case AudioSourceAction.PlayLoop:
			result = handlerProxy.Invoke(targetComponent, new object[] {});
			break;
		case AudioSourceAction.Stop:
			result = handlerProxy.Invoke(targetComponent, new object[] {});
			break;
		case AudioSourceAction.None:
		default:
			break;
		}
		
		if(result is IEnumerator)
		{
			if(targetComponent is MonoBehaviour)
			{
				((MonoBehaviour)targetComponent).StartCoroutine((IEnumerator)result);
			}
		}
	}
	
	private MethodInfo getMethodInfoForAction(AudioSourceAction act)
	{
		MethodInfo methodinfo = null;
		switch(act)
		{
		case AudioSourceAction.Play:
			methodinfo = typeof(AudioSourcePro).GetMethod( "PlayHandler", 
												BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
												Type.DefaultBinder, 
												new Type[] {}, 
												null) as MethodInfo;
			break;
		case AudioSourceAction.PlayCapped:
			methodinfo = typeof(AudioSourcePro).GetMethod( "PlayCappedHandler", 
												BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
												Type.DefaultBinder, 
												new[] {typeof(string)}, 
												null) as MethodInfo;
			break;
		case AudioSourceAction.PlayLoop:
			methodinfo = typeof(AudioSourcePro).GetMethod( "PlayLoopHandler", 
												BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
												Type.DefaultBinder, 
												new Type[] {}, 
												null) as MethodInfo;
			break;
		case AudioSourceAction.Stop:
			methodinfo = typeof(AudioSourcePro).GetMethod( "StopHandler", 
												BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
												Type.DefaultBinder, 
												new Type[] {}, 
												null) as MethodInfo;
			break;
		case AudioSourceAction.None:
		default:
			break;
		}
		return methodinfo;
	}
}