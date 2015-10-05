using UnityEngine;
using System.Collections.Generic;

public class AStarTest : MonoBehaviour {
	public GameObject nodePrefab; // NODE Prefab
	
	public AStar.HEURISTIC_METHODS astarMethodType = AStar.HEURISTIC_METHODS.MANHATTAN;
	
	public bool wasGenerated; // if GRID MAP of NODES was Generated
	
	public Material redMat;		// NOT WalkAble Node
	public Material blueMat; 	// Path Node
	public Material greenMat;	// Start or End Node - [A or B point]
	public Material whiteMat;	// Empty Node (WalkAble, Not Start, Not End, Not Path Node)
	
	private bool _wasRender;
	
	private Camera _camera;	// Main Camera
	private NodeState _nodeState; // TEMP Node States Component
	private List<List<NodeState>> _nodeStatesGrid;
	private List<NodeState> _nodeStatesList;
	
	// ROOT PARENT for all GRID MAP of NODES (just for beauty structure)
	private Transform _parentTransform;
	// ROW's PARENTS (just for beauty structure)
	private List<Transform> _parentTransforms;
	
	private List<List<Transform>> _gridTransforms; // Generated GRID MAP of NODES
	private List<Transform> _tmpList; // TEMP List of Transforms
	private NodeIndex _tmpNodeIndex; // TEMP NODE INDEX
	
	private string _numColsString = "7", _numRowsString = "7";
	private int _numCols, _numRows, _tmpInt;
	
	private System.Int64 _timeFindPath; // Time for A* FindPath
	
	// GUI Window Rectangle
	private Rect _windowRect = new Rect(0, 0, 150, 40);
	
	void Awake() {
		wasGenerated = false;
		_wasRender = false;
		_timeFindPath = 0;
		
		AStar.OnPathFinded += HandleAStarOnPathFinded;
		
		_gridTransforms = new List<List<Transform>>();
		_nodeStatesGrid = new List<List<NodeState>>();
		_parentTransforms = new List<Transform>();
		
		_numRows = int.Parse(_numRowsString);
		_numCols = int.Parse(_numColsString);
		
		_camera = Camera.main;
		
		// ROOT PARENT just for BEAUTY STRUCTURE
		GameObject go = new GameObject("GRID_MAP_NODES");
		_parentTransform = go.transform;
	}

	void HandleAStarOnPathFinded (List<Node> path)
	{
		// CallBack Event [Body]
	}
	
	void Update() {
		if (!wasGenerated) {
			_GenerateGrid();
		}
		
		_Render();
	}
	
	private void _GenerateGrid() {
		wasGenerated = true;
		
		// Destroy ALL GameObjects [GRID_MAP_NODES] GENERATED Before (! if they are !)
		foreach (List<Transform> list in _gridTransforms) {
			foreach (Transform t in list) {
				GameObject.Destroy(t.gameObject);
			}
			list.Clear();
		}
		_gridTransforms.Clear();
		
		// CLEAR NodeState's Lists
		foreach (List<NodeState> list in _nodeStatesGrid) {
			list.Clear();
		}
		_nodeStatesGrid.Clear();
		
		// Destroy All ROW's Parents
		foreach (Transform t in _parentTransforms) {
			GameObject.Destroy(t.gameObject);
		}
		_parentTransforms.Clear();
	
		// GENERATE GRID_MAP_NODES
		for (int i = 0; i < _numRows; i++) {
			_tmpList = new List<Transform>();
			_nodeStatesList = new List<NodeState>();
			
			// CREATE ROW PARENT just for BEAUTY STRUCTURE of GameObjects
			GameObject parentGO = new GameObject("ROW_"+i.ToString());
			parentGO.transform.parent = _parentTransform;
			_parentTransforms.Insert(i, parentGO.transform);
			
			for (int j = 0; j < _numCols; j++) {
				GameObject go = GameObject.Instantiate(nodePrefab) as GameObject;
				go.transform.name = go.transform.name.Replace(
					"(Clone)", "["+i.ToString()+", "+j.ToString()+"]"
				);
				go.transform.parent = _parentTransforms[i];
				go.transform.position = new Vector3(
					j * go.transform.localScale.x * 1.25f,
					-i * go.transform.localScale.y * 1.25f,
					go.transform.position.z
				);
				_tmpList.Insert(j, go.transform);
				_nodeStatesList.Insert(j, go.GetComponent<NodeState>());
			}
			_gridTransforms.Insert(i, _tmpList);
			_nodeStatesGrid.Insert(i, _nodeStatesList);
		}
		
		_parentTransform.Translate(new Vector3(
			(-1) * (_numCols - 1) * 0.5f * nodePrefab.transform.localScale.x * 1.25f,
			(_numRows - 1) * 0.5f * nodePrefab.transform.localScale.y * 1.25f,
			0
		));
	}
	
	private void _Render() {
		if (0 == _gridTransforms.Count) {
			return;
		}
		
		if (null == AStar.gridMap || 0 == AStar.gridMap.Count || _wasRender) {
			_RayCastNode();
			return;
		}
		
		_wasRender = true;
		
		// Set suitable Materials for All GRID_MAP's NODES
		for (int i = 0; i < AStar.gridMap.Count; i++) {
			for (int j = 0; j < AStar.gridMap[i].Count; j++) {
				// if Not WalkAble NODE [Set Red Material]
				if (false == AStar.gridMap[i][j]) {
					_SetMaterial(_gridTransforms[i][j].GetComponent<Renderer>(), redMat);
				} else if ( // else if START or END NODE [Set Blue Material]
					AStar.startIndex.i == i && AStar.startIndex.j == j ||
					AStar.endIndex.i == i && AStar.endIndex.j == j
				) {
					_SetMaterial(_gridTransforms[i][j].GetComponent<Renderer>(), blueMat);
				} else {
					_tmpNodeIndex = new NodeIndex();
					_tmpNodeIndex.i = i;
					_tmpNodeIndex.j = j;
					// if PATH NODE [Set Green Material] else [Set White Material]
					if (AStar.ContainsNodeIndex(AStar.pathNodes, _tmpNodeIndex)) {
						_SetMaterial(_gridTransforms[i][j].GetComponent<Renderer>(), greenMat);
					} else {
						_SetMaterial(_gridTransforms[i][j].GetComponent<Renderer>(), whiteMat);
					}
				}
			}
		}
	}
	
	private void _SetMaterial(Renderer r, Material m) {
		if (r.sharedMaterial.color != m.color) {
			r.sharedMaterial = m;
		}
	}
	
	private void _RayCastNode() {
		if (Input.touchCount >= 1) {
			RaycastHit hit = new RaycastHit();
	        for (int i = 0; i < Input.touchCount; ++i) {
				// Construct a ray from the current touch coordinates
				Ray ray = _camera.ScreenPointToRay(Input.GetTouch(i).position);
				
	            if (Input.GetTouch(i).phase.Equals(TouchPhase.Ended)) {
		            if (Physics.Raycast(ray, out hit)) {
	                	_ChangeNodeState(hit);
	              	}
	            }
	        }
			return;
		}
		
		if (Input.GetMouseButtonUp(0)) {
			RaycastHit hit = new RaycastHit();
	        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
			
			if (Physics.Raycast(ray, out hit)) {
            	_ChangeNodeState(hit);
          	}
			return;
		}
	}
	
	private void _ChangeNodeState(RaycastHit hit) {
		_nodeState = hit.transform.gameObject.GetComponent<NodeState>();
		switch (_nodeState.state) {
			case NodeState.STATES.RED:
				_nodeState.state = NodeState.STATES.BLUE;
				_SetMaterial(_nodeState.GetComponent<Renderer>(), blueMat);
				break;
			
			case NodeState.STATES.BLUE:
				_nodeState.state = NodeState.STATES.WHITE;
				_SetMaterial(_nodeState.GetComponent<Renderer>(), whiteMat);
				break;
			
			case NodeState.STATES.WHITE:
				_nodeState.state = NodeState.STATES.RED;
				_SetMaterial(_nodeState.GetComponent<Renderer>(), redMat);
				break;
			
			default:
				break;
		}
	}
	
	private void _ClearAStarGridMap() {
		if (null != AStar.gridMap) {
			foreach (List<bool> list in AStar.gridMap) {
				list.Clear();
			}
			AStar.gridMap.Clear();
		} else {
			AStar.gridMap = new List<List<bool>>();
		}
	}
	
	private void _FindPath() {
		_ClearAStarGridMap();
		
		if (null == AStar.startIndex) {
			AStar.startIndex = new NodeIndex();
		}
		
		if (null == AStar.endIndex) {
			AStar.endIndex = new NodeIndex();
		}
			
		_tmpInt = 0;
		for (int i = 0; i < _nodeStatesGrid.Count; i++) {
			List<bool> boolList = new List<bool>();
			for (int j = 0; j < _nodeStatesGrid[0].Count; j++) {
				if (NodeState.STATES.BLUE == _nodeStatesGrid[i][j].state) {
					if (++_tmpInt > 2) return;
					
					if (1 == _tmpInt) {
						AStar.startIndex.i = i;
						AStar.startIndex.j = j;
					} else if (2 == _tmpInt) {
						AStar.endIndex.i = i;
						AStar.endIndex.j = j;
					}
					
					boolList.Insert(j, true);
				} else if (NodeState.STATES.RED == _nodeStatesGrid[i][j].state) {
					boolList.Insert(j, false);
				} else { // WHITE or GREEN
					boolList.Insert(j, true);
				} 
			}
			AStar.gridMap.Insert(i, boolList);
		}
		
		if (_tmpInt != 2) {
			_ClearAStarGridMap();
			return;
		}
		
		_timeFindPath = System.DateTime.Now.Millisecond + System.DateTime.Now.Second * 1000;
		
		AStar.heuristicType = astarMethodType;
		AStar.FindPath();
		
		_timeFindPath = System.DateTime.Now.Millisecond + System.DateTime.Now.Second * 1000 - _timeFindPath;
	}
	
	void OnGUI() {
		_windowRect = GUILayout.Window (1, _windowRect, WindowCallback, "A* TEST");
	}
	
	void WindowCallback (int windowID) {
		// Make the window be draggable in the top 40 pixels.
		GUI.DragWindow(new Rect(0,0, (float)System.Decimal.MaxValue, 40));
		
		GUILayout.Label("Rows [0 : 100]");
		_numRowsString = GUILayout.TextField(_numRowsString.ToString(), 3);
		_numRows = Mathf.Clamp(
			int.TryParse(_numRowsString, out _numRows) ? _numRows : 0, 0, 100
		);
		
		GUILayout.Label("Columns [0 : 100]");
		_numColsString = GUILayout.TextField(_numColsString.ToString(), 3);
		_numCols = Mathf.Clamp(
			int.TryParse(_numColsString, out _numCols) ? _numCols : 0, 0, 100
		);
		
		if (GUILayout.Button("Generate Grid Map", GUILayout.Height(55))) {
			wasGenerated = false;
			_wasRender = false;
			_timeFindPath = 0;
			_ClearAStarGridMap();
		}
		
		if (GUILayout.Button("Find Path", GUILayout.Height(55))) {
			_wasRender = false;
			_timeFindPath = 0;
			_FindPath();
		}
		
		GUILayout.Label("MilliSeconds: " + _timeFindPath.ToString());
	}
}