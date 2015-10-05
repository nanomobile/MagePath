using UnityEngine;

public class CameraMovement : MonoBehaviour {

	//edge of screen
    private enum edgeState { LEFT, LEFT_UP, LEFT_DOWN, UP, DOWN, RIGHT, RIGHT_UP, RIGHT_DOWN }
    private edgeState EdgeState;
    public int edgeBuffer = 10;
    public int screenMoveRate = 10;
	
    //zoom camera
    public float zoomRate = 10;
    public int zoomLimit = 1;
    public float zoomOriginal = 100;

    // Update is called once per frame
    void Update()
    {
        //check for user input on the following functions
        MoveCamera(); //execute any movement that needs to be done on the camera
        ZoomCamera(); //zooms the camera in/out
    }
	
	
    private void ZoomCamera()
    {
        float scrollVal = Input.GetAxis("Mouse ScrollWheel");
		if (0 != scrollVal) {
			if (Camera.main.orthographic) {
				if (
					Camera.main.orthographicSize - scrollVal * zoomRate >= zoomLimit && 
					Camera.main.orthographicSize - scrollVal * zoomRate <= zoomOriginal
				) {
		            Camera.main.orthographicSize -= scrollVal * zoomRate;
				}
			} else {
				if (
					Camera.main.fieldOfView - scrollVal * zoomRate >= zoomLimit && 
					Camera.main.fieldOfView - scrollVal * zoomRate <= zoomOriginal
				) {
		            Camera.main.fieldOfView -= scrollVal * zoomRate;
				}
			}
		}
    }

    /// <summary>
    /// Executes all movement of the camera
    /// </summary>
    private void MoveCamera()
    {
        //move the camera if arrow keys are pressed
        float horizontalVal = Input.GetAxis("Horizontal");
        float verticalVal = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(screenMoveRate * horizontalVal, screenMoveRate * verticalVal, 0) * Time.deltaTime);

        //move camera as long as mouse is at edge of screen
        if (Input.GetMouseButton(0) && (0 == GUIUtility.hotControl) && CursorAtEdge())
        {
            switch (EdgeState)
            {
                case edgeState.LEFT:
                    transform.Translate(new Vector3(-screenMoveRate, 0, 0) * Time.deltaTime);
                    break;
                case edgeState.LEFT_UP:
                    transform.Translate(new Vector3(-screenMoveRate, screenMoveRate, 0) * Time.deltaTime);
                    break;
                case edgeState.LEFT_DOWN:
                    transform.Translate(new Vector3(-screenMoveRate, -screenMoveRate, 0) * Time.deltaTime);
                    break;
                case edgeState.UP:
                    transform.Translate(new Vector3(0, screenMoveRate, 0) * Time.deltaTime);
                    break;
                case edgeState.DOWN:
                    transform.Translate(new Vector3(0, -screenMoveRate, 0) * Time.deltaTime);
                    break;
                case edgeState.RIGHT:
                    transform.Translate(new Vector3(screenMoveRate, 0, 0) * Time.deltaTime);
                    break;
                case edgeState.RIGHT_UP:
                    transform.Translate(new Vector3(screenMoveRate, screenMoveRate, 0) * Time.deltaTime);
                    break;
                case edgeState.RIGHT_DOWN:
                    transform.Translate(new Vector3(screenMoveRate, -screenMoveRate, 0) * Time.deltaTime);
                    break;
                default:
                    break;
            }
        }
    }
	

    /// <summary>
    /// Checks if the cursor is at the edge of the screen, if so then the EdgeState is updated
    /// </summary>
    private bool CursorAtEdge()
    {
        //NOTE (0,0) is the bottom left edge of the screen

        //check if mouse is at the left edge of the screen
        if (Input.mousePosition.x - edgeBuffer <= 0)
        {
            //check if at top or bottom of the screen as well
            if (Input.mousePosition.y + edgeBuffer >= Screen.height)
                EdgeState = edgeState.LEFT_UP;
            else if (Input.mousePosition.y - edgeBuffer <= 0)
                EdgeState = edgeState.LEFT_DOWN;
            else
                EdgeState = edgeState.LEFT;
            return true;
        }

        //check if mouse is at the right edge of the screen
        if (Input.mousePosition.x + edgeBuffer >= Screen.width)
        {
            //check if at top or bottom of the screen as well
            if (Input.mousePosition.y + edgeBuffer >= Screen.height)
                EdgeState = edgeState.RIGHT_UP;
            else if (Input.mousePosition.y - edgeBuffer <= 0)
                EdgeState = edgeState.RIGHT_DOWN;
            else
                EdgeState = edgeState.RIGHT;
            return true;
        }

        //check if mouse is at the top edge of the screen
        if (Input.mousePosition.y + edgeBuffer >= Screen.height)
        {
            EdgeState = edgeState.UP;
            return true;
        }

        //check if mouse is at the bottom edge of the screen
        if (Input.mousePosition.y - edgeBuffer <= 0)
        {
            EdgeState = edgeState.DOWN;
            return true;
        }

        return false;
    }
	
	
}