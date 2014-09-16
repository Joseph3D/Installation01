using UnityEngine;
using System.Collections;

/// <summary>
/// Very basic script designed for use with UIButtonMessage.
/// It quits the game when the button this is attached to is pressed.
/// </summary>

public class EndGameScript : MonoBehaviour
{
	void Quit ()
    {
		Application.Quit();
	}
}