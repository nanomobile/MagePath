using UnityEngine;

public class FPS : MonoBehaviour {
	private Rect windowRect = new Rect(0, 0, 150, 40);
	
	public int fpsLimit = 300;
	void Awake() {
		Application.targetFrameRate = fpsLimit;
	}
	
	// Make the onscreen GUI to let the player switch control between Lerpz and the spaceship.
	void OnGUI () {
		//DrawWindowFPS();
	}
	
	//var texture : Texture2D;
	void DrawWindowFPS() {
		// Make a popup window
		windowRect = GUILayout.Window (0, windowRect, DoControlsWindow, "DEBUG Info");
		//windowRect = GUILayout.Window (0, windowRect, DoControlsWindow, texture);
		
		// The window can be dragged around by the users - make sure that it doesn't go offscreen.
		//windowRect.x = Mathf.Clamp(windowRect.x, 0.0f, Screen.width - windowRect.width);
		//windowRect.y = Mathf.Clamp(windowRect.y, 0.0f, Screen.height - windowRect.height);
	}
	
	// Make the contents of the window
	void DoControlsWindow (int windowID) {
		// Make the window be draggable in the top 40 pixels.
		GUI.DragWindow(new Rect(0,0, (float)System.Decimal.MaxValue, 40));
		
		GUILayout.Label("FPS: " + format);
		
		if (GUILayout.Button("Restart", GUILayout.Height(70))) {
			Application.LoadLevel(Application.loadedLevelName);
		}
		
		if (GUILayout.Button("Quit", GUILayout.Height(70))) {
			Application.Quit();
		}
	}
	
	public float updateInterval = 0.5f;
	 
	private float accum = 0; // FPS accumulated over the interval
	private int frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval
	private string format;
	private float fps;
	 
	void Start()
	{
	    timeleft = updateInterval;  
	}
	
	void Update()
	{			
	    timeleft -= Time.deltaTime;
	    accum += Time.timeScale/Time.deltaTime;
	    ++frames;
	    
	    // Interval ended - update GUI text and start new interval
	    if( timeleft <= 0.0f)
	    {
	        // display two fractional digits (f2 format)
		    fps = accum/frames;
		    format = System.String.Format("{0:F2}",fps);
	
		    //  DebugConsole.Log(format,level);
	        timeleft = updateInterval;
	        accum = 0.0F;
	        frames = 0;
	    }
	}
}
