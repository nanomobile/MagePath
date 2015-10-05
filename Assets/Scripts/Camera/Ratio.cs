using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class Ratio : MonoBehaviour {
	public float WIDTH = 960.0f, HEIGHT = 640.0f;
	
	private float _targetAspect, _windowAspect, _scaleHeight, _scaleWidth;
	private Rect _rect;
	
	private Camera _camera;
	
	void Awake() {
		_camera = GetComponent<Camera>();
	}

	// Use this for initialization
	void Update() 
	{
		//WIDTH = Screen.currentResolution.width;
		//HEIGHT = Screen.currentResolution.height;
		
	    _targetAspect = WIDTH / HEIGHT;
	
	    // determine the game window's current aspect ratio
	    _windowAspect = (float)Screen.width / (float)Screen.height;
	
	    // current viewport height should be scaled by this amount
		_scaleHeight = _windowAspect / _targetAspect;
	
	    // if scaled height is less than current height, add letterbox
	    if (_scaleHeight < 1.0f)
	    {  
	        _rect = _camera.rect;
	
	        _rect.width = 1.0f;
	        _rect.height = _scaleHeight;
	        _rect.x = 0;
	        _rect.y = (1.0f - _scaleHeight) / 2.0f;
	        
	        _camera.rect = _rect;
	    }
	    else // add pillarbox
	    {
	        _scaleWidth = 1.0f / _scaleHeight;
	
	        _rect = _camera.rect;
	
			_rect.width = _scaleWidth;
	        _rect.height = 1.0f;
	        _rect.x = (1.0f - _scaleWidth) / 2.0f;
	        _rect.y = 0;
	
	        _camera.rect = _rect;
	    }
	}
}
