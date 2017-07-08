/*
 * Responsive GUI Script
 * By: Lucas Pasarella
 * How to use:
 * -Manually set default width and height in the script (Width and Height variables)
 * -All scaling will be handeled by the script
 * -Start of OnGUI() call use StartResizer() function
 * -End of OnGUI() call use EndResizer() function
 * -In the OnGUI() call DO NOT use Screen.width() and Screen.height(), this will create sizing errors
 * instead use getNativeWidth() and getNativeHeight() 
*/

using UnityEngine;
using System.Collections;

public class ResponsiveGui {
	//private static int defaultFontSize = 12;
	private static float Width = 1920;
	private static float Height = 1080;
	private float aspect;
	private Vector3 scale;
	private Matrix4x4 origMatrix;

	//Use This At The Start Of A GUI Script
	public void StartResizer() {
		scale.x = Screen.width / Width;
		scale.y = Screen.height / Height;
		scale.z = 1;
		aspect = scale.x / scale.y;
		origMatrix = GUI.matrix;
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, scale);

	}

	//Use This At The End OF A GUI Script
	public void EndResizer() {
		GUI.matrix = origMatrix;
	}

	//Use instead of Screen.width()
	public float getNativeWidth(){
		return Width;
	}

	//Use instead of Screen.height()
	public float getNativeHeight(){
		return Height;
	}
	
}
