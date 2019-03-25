using UnityEngine;
using UnityEditor;

namespace OG_RaceKit
{

public class RaceKitWindow : EditorWindow
{
    private static string[] buttonsGUInames = { "fwd_9m", "fwd_15m", "9to15", "15to9",
                                                "trnL_9m", "trnR_9m", "trnL_15m", "trnR_15m",
                                                "split_9m", "union_9m", "split_15m", "union_15m", "pitstop", "startline_15m", "carplace_15m" };
    private static Texture2D[] buttons = new Texture2D[buttonsGUInames.Length];
    private static string raceKitRootName = "RaceKit";
    private static string pathToModels;
    private static string startPath = "Resources/TrackParts";
    // GUI
    private Vector2 minButtonsSize = new Vector2(64, 64);
    private Vector2 maxButtonsSize = new Vector2(64, 64);
    private ButtonsGrid buttonsGridSize = new ButtonsGrid(4, 3);
    // Some keys
    private static bool performsAllowed = true;
    private static bool correctObjectSelected = false;
    private static bool correctPath = false;
    private static bool raceKitExists = false;
    // Some errors
    private static bool errorLoadingModels = false;
    private static bool errorPlacingPart = false;

    private struct ButtonsGrid
    {
        public int x;
        public int y;

        public ButtonsGrid(int xVal, int yVal)
        {
            x = xVal;
            y = yVal;
        }
    }

    [MenuItem("Window/Race Kit")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        RaceKitWindow window = (RaceKitWindow)GetWindow(typeof(RaceKitWindow));
        window.Show();
        RestorePath();
        StartTreatment();
    }

    static void StartTreatment()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i] = Resources.Load("GUI/" + buttonsGUInames[i]) as Texture2D;
        }

        FindRaceKit();
        if (!raceKitExists)
        {
            CreateRaceKit();
            Selection.activeGameObject = RaceKitService.raceKit;
        }
        LoadModels();
        SelectionTreatment();
    }

    void Reset()
    {
        RestorePath();
        StartTreatment();
    }

    private void OnEnable()
    {
        EditorApplication.hierarchyWindowChanged += ConfigureAll;
        Selection.selectionChanged += SelectionTreatment;
        RestorePath();
        StartTreatment();
    }

    private void OnDisable()
    {
        EditorApplication.hierarchyWindowChanged -= ConfigureAll;
        Selection.selectionChanged -= SelectionTreatment;
    }

    static void SelectionTreatment()
    {
        // Check for root object
        if(RaceKitService.raceKit == null)
        {
            correctObjectSelected = false;

            return;
        }

        // Check for selected objects count
        if (Selection.objects.Length == 0 || Selection.objects.Length > 2)
        {
            correctObjectSelected = false;

            return;
        }

        // Check if object selected not in the scene
        foreach (var obj in Selection.objects)
        {
            if (AssetDatabase.Contains(obj))
            {
                correctObjectSelected = false;

                return;
            }
        }

        Transform activeObject = Selection.activeTransform;

        // Check if root object selected 
        if (activeObject != null && activeObject.Equals(RaceKitService.raceKit.transform) && activeObject.childCount == 0)
        {
            correctObjectSelected = true;

            return;
        }

        // Check for every selected object is root's child
        foreach (var obj in Selection.transforms)
        {
            if (obj.parent != RaceKitService.raceKit.transform)
            {
                correctObjectSelected = false;

                return;
            }
        }

        RaceKitService.activeObjects = Selection.gameObjects;

        Transform objectParent = Selection.activeTransform.parent;

        if (objectParent != null && objectParent.Equals(RaceKitService.raceKit.transform))
        {
            RaceKitService.activeComposition = Selection.activeGameObject.GetComponent<Composition>();

            correctObjectSelected = true;
        }
        else
        {
            correctObjectSelected = false;
        }
    }

    static void ConfigureAll()
    {
        if (RaceKitService.raceKit != null)
        {
            for (int i = 0; i < RaceKitService.raceKit.transform.childCount; i++)
            {
                Composition comp = RaceKitService.raceKit.transform.GetChild(i).GetComponent<Composition>();
                if (comp != null)
                {
                    comp.Configure();
                }
            }
        }
    }
    
    static void LoadModels()
    {
        bool failed = false;
        RaceKitService.prefabs = Resources.LoadAll<GameObject>(pathToModels);
        RaceKitService.prefabTypes = new ObjectType[RaceKitService.prefabs.Length];
        if (RaceKitService.prefabs.Length < 17)
        {
            failed = true;
        }
        GameObject[] TmpModels = new GameObject[17];

        for (int i = 0; i < RaceKitService.prefabs.Length; i++)
        {
            if (RaceKitService.prefabs[i].name.Contains("fwd") && RaceKitService.prefabs[i].name.Contains("9m"))
            {
                TmpModels[0] = RaceKitService.prefabs[i];
                RaceKitService.prefabTypes[0] = ObjectType.Single;
            }
            else if (RaceKitService.prefabs[i].name.Contains("fwd") && RaceKitService.prefabs[i].name.Contains("15m"))
            {
                TmpModels[1] = RaceKitService.prefabs[i];
                RaceKitService.prefabTypes[1] = ObjectType.Single;
            }
            else if (RaceKitService.prefabs[i].name.Contains("9to15"))
            {
                TmpModels[2] = RaceKitService.prefabs[i];
                RaceKitService.prefabTypes[2] = ObjectType.Single;
            }
            else if (RaceKitService.prefabs[i].name.Contains("15to9"))
            {
                TmpModels[3] = RaceKitService.prefabs[i];
                RaceKitService.prefabTypes[3] = ObjectType.Single;
            }
            else if (RaceKitService.prefabs[i].name.Contains("trnL") && RaceKitService.prefabs[i].name.Contains("9m"))
            {
                TmpModels[4] = RaceKitService.prefabs[i];
                RaceKitService.prefabTypes[4] = ObjectType.Single;
            }
            else if (RaceKitService.prefabs[i].name.Contains("trnR") && RaceKitService.prefabs[i].name.Contains("9m"))
            {
                TmpModels[5] = RaceKitService.prefabs[i];
                RaceKitService.prefabTypes[5] = ObjectType.Single;
            }
            else if (RaceKitService.prefabs[i].name.Contains("trnL") && RaceKitService.prefabs[i].name.Contains("15m"))
            {
                TmpModels[6] = RaceKitService.prefabs[i];
                RaceKitService.prefabTypes[6] = ObjectType.Single;
            }
            else if (RaceKitService.prefabs[i].name.Contains("trnR") && RaceKitService.prefabs[i].name.Contains("15m"))
            {
                TmpModels[7] = RaceKitService.prefabs[i];
                RaceKitService.prefabTypes[7] = ObjectType.Single;
            }
            else if (RaceKitService.prefabs[i].name.Contains("split") && RaceKitService.prefabs[i].name.Contains("9m"))
            {
                TmpModels[8] = RaceKitService.prefabs[i];
                RaceKitService.prefabTypes[8] = ObjectType.FrontFork;
            }
            else if (RaceKitService.prefabs[i].name.Contains("union") && RaceKitService.prefabs[i].name.Contains("9m"))
            {
                TmpModels[9] = RaceKitService.prefabs[i];
                RaceKitService.prefabTypes[9] = ObjectType.BackFork;
            }
            else if (RaceKitService.prefabs[i].name.Contains("split") && RaceKitService.prefabs[i].name.Contains("15m"))
            {
                TmpModels[10] = RaceKitService.prefabs[i];
                RaceKitService.prefabTypes[10] = ObjectType.FrontFork;
            }
            else if (RaceKitService.prefabs[i].name.Contains("union") && RaceKitService.prefabs[i].name.Contains("15m"))
            {
                TmpModels[11] = RaceKitService.prefabs[i];
                RaceKitService.prefabTypes[11] = ObjectType.BackFork;
            }
            else if (RaceKitService.prefabs[i].name.Contains("pitbuilding"))
            {
                TmpModels[RaceKitService.pitBuildingID] = RaceKitService.prefabs[i];
                RaceKitService.prefabTypes[RaceKitService.pitBuildingID] = ObjectType.Single;
            }
            else if (RaceKitService.prefabs[i].name.Contains("fill") && RaceKitService.prefabs[i].name.Contains("9m"))
            {
                TmpModels[RaceKitService.fill9mID] = RaceKitService.prefabs[i];
            }
            else if (RaceKitService.prefabs[i].name.Contains("fill") && RaceKitService.prefabs[i].name.Contains("15m"))
            {
                TmpModels[RaceKitService.fill15mID] = RaceKitService.prefabs[i];
            }
            else if (RaceKitService.prefabs[i].name.Contains("startline") && RaceKitService.prefabs[i].name.Contains("15m"))
            {
                TmpModels[RaceKitService.startline15mID] = RaceKitService.prefabs[i];
                RaceKitService.prefabTypes[RaceKitService.startline15mID] = ObjectType.Single;
            }
            else if (RaceKitService.prefabs[i].name.Contains("carplace") && RaceKitService.prefabs[i].name.Contains("15m"))
            {
                TmpModels[RaceKitService.carplace15mID] = RaceKitService.prefabs[i];
                RaceKitService.prefabTypes[RaceKitService.carplace15mID] = ObjectType.Single;
            }
            else
                failed = true;
        }

        RaceKitService.prefabs = TmpModels;
        if (failed)
            errorLoadingModels = true;
        else
            errorLoadingModels = false;
    }

    static bool GetRelativePath(ref string path)
    {
        // Find slice point to get relative path
        int slicePointer = path.IndexOf("Resources");
        if (slicePointer != -1)
        {
            // Check if user set root resource folder
            if (slicePointer + "Resources".Length == path.Length)
                path = "";
            // Correct path
            else
                path = path.Substring(slicePointer + "Resources".Length + 1);

            return true;
        }

        return false;
    }

    static void CheckPath()
    {
        if (GetRelativePath(ref pathToModels))
            correctPath = true;
        else
            correctPath = false;
    }

    static void RestorePath()
    {
        pathToModels = startPath;
        CheckPath();
    }

    static void SetAllowKey()
    {
        if (raceKitExists && correctObjectSelected)
            performsAllowed = true;
        else
            performsAllowed = false;
    }

    static void FindRaceKit()
    {
        RaceKitService.raceKit = GameObject.Find(raceKitRootName);
        if (RaceKitService.raceKit != null)
            raceKitExists = true;
        else
            raceKitExists = false;
    }

    static void CreateRaceKit()
    {
        RaceKitService.raceKit = new GameObject(raceKitRootName);
        EditorUtility.SetDirty(RaceKitService.raceKit);
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0f, 0f, 270f, 600f));

        GUILayout.Label("Welcome to Race Kit!");
        GUILayout.Space(15);

        GUILayout.BeginHorizontal();

        GUILayout.Label("Resources path: ");
        if (correctPath)
            GUILayout.Label("Resources/" + pathToModels);
        else
        {
            if (pathToModels.Length >= 17)
                GUILayout.Label("..." + pathToModels.Substring(pathToModels.Length - 17));
            else
                GUILayout.Label(pathToModels);
        }

        if (GUILayout.Button("...", GUILayout.MaxHeight(20), GUILayout.MaxWidth(30)))
        {
            pathToModels = EditorUtility.OpenFolderPanel("Set track parts resources folder", Application.dataPath, "");
            CheckPath();

            LoadModels();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.Label("Fence type:");
        RaceKitService.fenceType = (FenceType)EditorGUILayout.EnumPopup(RaceKitService.fenceType);
        
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        if (performsAllowed & GUILayout.Button(buttons[buttons.Length - 2], GUILayout.MaxWidth(maxButtonsSize.x * 2 + 5f), GUILayout.MaxHeight(maxButtonsSize.y)))
        {
            RaceKitService.creatingID = RaceKitService.startline15mID;

            Selection.activeGameObject = Instantiate(RaceKitService.prefabs[RaceKitService.startline15mID]).AddComponent<Composition>().gameObject;

            Undo.RegisterCreatedObjectUndo(Selection.activeGameObject, "Creating part");
            EditorUtility.SetDirty(RaceKitService.raceKit);
        }
        if (performsAllowed & GUILayout.Button(buttons[buttons.Length - 1], GUILayout.MaxWidth(maxButtonsSize.x * 2 + 5f), GUILayout.MaxHeight(maxButtonsSize.y)))
        {
            RaceKitService.creatingID = RaceKitService.carplace15mID;

            Selection.activeGameObject = Instantiate(RaceKitService.prefabs[RaceKitService.carplace15mID]).AddComponent<Composition>().gameObject;

            Undo.RegisterCreatedObjectUndo(Selection.activeGameObject, "Creating part");
            EditorUtility.SetDirty(RaceKitService.raceKit);
        }

        GUILayout.EndHorizontal();

        for (int y = 0; y < buttonsGridSize.y; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < buttonsGridSize.x; x++)
            {
                int buttonID = buttonsGridSize.x * y + x;
                if (performsAllowed & GUILayout.Button(buttons[buttonID], GUILayout.MaxWidth(maxButtonsSize.x),
                                                                              GUILayout.MaxHeight(maxButtonsSize.y),
                                                                              GUILayout.MinWidth(minButtonsSize.x),
                                                                              GUILayout.MinHeight(minButtonsSize.y)))
                {
                    RaceKitService.creatingID = buttonID;

                    Selection.activeGameObject = Instantiate(RaceKitService.prefabs[buttonID]).AddComponent<Composition>().gameObject;

                    Undo.RegisterCreatedObjectUndo(Selection.activeGameObject, "Creating part");
                    EditorUtility.SetDirty(RaceKitService.raceKit);
                }
            }
            GUILayout.EndHorizontal();
        }

        if (performsAllowed & GUILayout.Button(buttons[RaceKitService.pitBuildingID], GUILayout.MaxWidth(maxButtonsSize.x * 4 + 10f), GUILayout.MaxHeight(maxButtonsSize.y)))
        {
            RaceKitService.creatingID = RaceKitService.pitBuildingID;

            Selection.activeGameObject = Instantiate(RaceKitService.prefabs[RaceKitService.pitBuildingID]).AddComponent<Composition>().gameObject;

            Undo.RegisterCreatedObjectUndo(Selection.activeGameObject, "Creating part");
            EditorUtility.SetDirty(RaceKitService.raceKit);
        }

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Switch", GUILayout.MinWidth(120), GUILayout.MinHeight(30), GUILayout.MaxWidth(50)))
        {
            if (performsAllowed && RaceKitService.activeComposition != null)
                RaceKitService.activeComposition.NextConnection();
        }

        GUILayout.Label("");

        if (GUILayout.Button("MERGE", GUILayout.MinWidth(120), GUILayout.MinHeight(30), GUILayout.MaxWidth(50)))
        {
            if (Selection.gameObjects.Length == 0)
                return;

            if (Selection.gameObjects.Length == 1 && Selection.gameObjects[0].transform.childCount == 0)
                return;

            MeshFilter[] allFilters = new MeshFilter[] { };

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                ArrayUtility.AddRange(ref allFilters, Selection.gameObjects[i].GetComponentsInChildren<MeshFilter>());
            }

            if (allFilters.Length == 0)
                return;

            GameObject[] allGOS = new GameObject[allFilters.Length];

            for (int i = 0; i < allFilters.Length; i++)
            {
                allGOS[i] = allFilters[i].gameObject;
            }

            GameObject merged = RaceKitService.MergeGameObjects("Merged", true, Vector3.one, allGOS);

            if (merged == null)
                return;

            // To destroy all gameobjects in editor mode correctly we need temporary array of them
            GameObject[] temp = new GameObject[Selection.gameObjects.Length];

            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = Selection.gameObjects[i];
            }

            for (int i = 0; i < temp.Length; i++)
            {
                Undo.DestroyObjectImmediate(temp[i]);
            }

            Selection.activeGameObject = merged;

            Undo.RegisterCreatedObjectUndo(merged, "Merge");

            EditorUtility.SetDirty(merged);
        }

        GUILayout.Label("");

        if (GUILayout.Button("Fill", GUILayout.MinWidth(120), GUILayout.MinHeight(30), GUILayout.MaxWidth(50)))
        {
            if (performsAllowed && Selection.objects.Length == 2)
            {
                RaceKitService.creatingID = -1;
                GameObject fill = new GameObject("fill");
                fill.AddComponent<MeshFilter>();
                fill.AddComponent<MeshRenderer>();
                fill.AddComponent<Composition>();
                Selection.activeGameObject = fill;

                Undo.RegisterCreatedObjectUndo(fill, "Merge");
                EditorUtility.SetDirty(RaceKitService.raceKit);
            }
        }

        GUILayout.EndHorizontal();

        if (!raceKitExists)
            EditorGUILayout.HelpBox("RaceKit Object not found, please reload addon!", MessageType.Warning);
        else if (!correctObjectSelected)
            EditorGUILayout.HelpBox("Please select proper object on the scene.", MessageType.Info);
        if (errorLoadingModels)
            EditorGUILayout.HelpBox("Models was not loaded properly!", MessageType.Warning);
        if (errorPlacingPart)
            EditorGUILayout.HelpBox("Placing error, check prefab model!", MessageType.Error);

        FindRaceKit();
        SetAllowKey();

        GUILayout.EndArea();
    }
}

}