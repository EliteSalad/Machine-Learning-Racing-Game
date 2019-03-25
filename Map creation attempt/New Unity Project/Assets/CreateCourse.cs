using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OG_RaceKit;

public class CreateCourse : MonoBehaviour
{

    private static string[] buttonsGUInames = { "fwd_9m", "fwd_15m", "9to15", "15to9",
                                                "trnL_9m", "trnR_9m", "trnL_15m", "trnR_15m",
                                                "split_9m", "union_9m", "split_15m", "union_15m", "pitstop", "startline_15m", "carplace_15m" };
    private static Texture2D[] buttons = new Texture2D[buttonsGUInames.Length];
    private static string raceKitRootName = "TackBuild Container";
    private static string pathToModels;
    private static string startPath = "Resources/TrackParts";
    // GUI
    //private Vector2 minButtonsSize = new Vector2(64, 64);
    //private Vector2 maxButtonsSize = new Vector2(64, 64);
    //private ButtonsGrid buttonsGridSize = new ButtonsGrid(4, 3);
    // Some keys
    private static bool performsAllowed = true;
    private static bool correctObjectSelected = false;
    private static bool correctPath = false;
    private static bool raceKitExists = false;
    // Some errors
    private static bool errorLoadingModels = false;
    private static bool errorPlacingPart = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetAllowKey()
    {
        if (raceKitExists && correctObjectSelected)
            performsAllowed = true;
        else
            performsAllowed = false;
    }

    public void FindRaceKit()
    {
        RaceKitService.raceKit = GameObject.Find(raceKitRootName);
        if (RaceKitService.raceKit != null)
            raceKitExists = true;
        else
            raceKitExists = false;
    }

    public  void CreateRaceKit()
    {
        RaceKitService.raceKit = new GameObject(raceKitRootName);
        //EditorUtility.SetDirty(RaceKitService.raceKit);
    }

    public void LoadModels()
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

    #region Treatment

    public void StartTreatment()
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


    public void SelectionTreatment()
    {
        // Check for root object
        if (RaceKitService.raceKit == null)
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

    public void ConfigureAll()
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

    #endregion

    #region Path 
    public bool GetRelativePath(ref string path)
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

    public void CheckPath()
    {
        if (GetRelativePath(ref pathToModels))
            correctPath = true;
        else
            correctPath = false;
    }

    public void RestorePath()
    {
        pathToModels = startPath;
        CheckPath();
    }

    public void Reset()
    {
        RestorePath();
        StartTreatment();
    }
    #endregion
}
