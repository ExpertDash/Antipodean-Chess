using UnityEngine;

public class BoardConfig : MonoBehaviour {
	[System.Serializable]
	public class Prefabs {
		public GameObject tile, pawn, knight, bishop, rook, queen, king;
	}

	[System.Serializable]
	public class Generation {
		[TooltipAttribute("Sphere radius")]
		public float radius = 5f;

		[TooltipAttribute("Tile thickness")]
		public float thickness = 0.01f;
	}

	[System.Serializable]
	public class Visuals {
		[TooltipAttribute("Delay between tile appearance during generation")]
		public float generationDelay = 0.05f;

		[TooltipAttribute("Whether to procedurally generate the tiles")]
		public bool generateProcedurally = false;

		[TooltipAttribute("Color based on tile position")]
		public Color axialTileColor = Color.gray,
					 primaryTileColor = Color.white,
					 secondaryTileColor = Color.black;

		[TooltipAttribute("Color for skybox based on board state")]
		public Color normalSkyboxColor = Color.gray,
					 inverseSkyboxColor = Color.black;

		public float tileNameHeightOffset = 1f;
		public Color tileNameColor = Color.blue;

		[TooltipAttribute("Tile selection colors")]
		public Color moveTileColor = Color.green,
					 attackTileColor = Color.yellow,
					 checkTileColor = Color.red;

		 [TooltipAttribute("Team piece colors")]
		 public Color teamWhiteColor = Color.white,
		 			  teamBlackColor = Color.black;
	}

	private static BoardConfig instance;

	[TooltipAttribute("The starting and ending columns")] public char _initialColumn = 'A', _finalColumn = 'H';
	[TooltipAttribute("The starting and ending rows")] public int _initialRow = 1, _finalRow = 8;
	[TooltipAttribute("Category of configuration")] [SerializeField] private Prefabs _prefabs;
	[TooltipAttribute("Category of configuration")] [SerializeField] private Generation _generation;
	[TooltipAttribute("Category of configuration")] [SerializeField] private Visuals _visuals;

	//Prevent directly setting the instances since they are shown in the editor
	public static char initialColumn { get { return instance._initialColumn; } }
	public static char finalColumn { get { return instance._finalColumn; } }
	public static int initialRow { get { return instance._initialRow; } }
	public static int finalRow { get { return instance._finalRow; } }
	public static int rows { get { return 1 + (int)finalColumn - (int)initialColumn; } }
	public static int columns { get { return 1 + finalRow - initialRow; } }
	public static int maxTravelDistance { get { return 1 + (rows > columns ? rows : columns); } }
	public static Prefabs prefabs { get { return instance._prefabs; } }
	public static Generation generation { get { return instance._generation; } }
	public static Visuals visuals { get { return instance._visuals; } }

	void Start() {
		if(instance) {
			Debug.Log("Another BoardConfig was attempted to be instantiated");
			Destroy(this);
		} else {
			instance = this;
			Camera.main.backgroundColor = visuals.normalSkyboxColor;
		}
	}
}