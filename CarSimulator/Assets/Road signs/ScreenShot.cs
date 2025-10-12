using UnityEngine;
using System.Collections;

public class ScreenShot : MonoBehaviour
{
    public string Name;
	void Start () {
	ScreenCapture.CaptureScreenshot(@"C:\Users\matve\OneDrive\Рабочий стол\Дорожные знаки\" + Name+".png");

    }
}
