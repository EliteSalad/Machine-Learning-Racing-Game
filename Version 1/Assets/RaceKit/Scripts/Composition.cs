using UnityEngine;

namespace OG_RaceKit
{

public enum ObjectType
{
    None,
    Single,
    FrontFork,
    BackFork,
    Fill
};

public enum FenceType
{
    None,
    Concrete,
    Metal
};

public static class RaceKitService
{
    public static ObjectType[] prefabTypes;
    public static GameObject[] prefabs;
    public static GameObject raceKit;
    public static int creatingID = 0;
    public static int pitBuildingID = 12;
    public static int fill9mID = 13;
    public static int fill15mID = 14;
    public static int carplace15mID = 15;
    public static int startline15mID = 16;
    public static Composition activeComposition;
    public static FenceType fenceType;
    public static GameObject[] activeObjects;
    public static AnimationCurve fillingCurve;

    static RaceKitService()
    {
        fillingCurve = new AnimationCurve(new Keyframe(0, 0, 1.5708f, 0f), new Keyframe(1, 1, 0, -1.5708f));
    }

    public static bool MultipleContains(string name, params string[] words)
    {
        for (int i = 0; i < words.Length; i++)
        {
            if (!name.Contains(words[i]))
                return false;
        }

        return true;
    }

    public static Transform GetChildByName(Transform obj, params string[] words)
    {
        for (int ch = 0; ch < obj.transform.childCount; ch++)
        {
            Transform child = obj.transform.GetChild(ch);
            if (MultipleContains(child.name, words))
                return child;
        }

        return null;
    }

    public static Transform GetChildByName(Transform obj, string name)
    {
        for (int ch = 0; ch < obj.transform.childCount; ch++)
        {
            Transform child = obj.transform.GetChild(ch);
            if (child.name == name)
                return child;
        }

        return null;
    }

    public static void Add<T>(ref T[] array, T item)
    {
        T[] newArray = new T[array.Length + 1];
        for (int i = 0; i < array.Length; i++)
        {
            newArray[i] = array[i];
        }
        newArray[newArray.Length - 1] = item;

        array = newArray;
    }

    public static int IndexOf<T>(T[] array, T item)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(item))
                return i;
        }

        return -1;
    }

    public static T[] MergeArrays<T>(params T[][] arrays)
    {
        int length = 0;
        for (int i = 0; i < arrays.Length; i++)
        {
            length += arrays[i].Length;
        }

        T[] newArray = new T[length];
        for (int i = 0, j = 0, a = 0; i < length; i++, j++)
        {
            if (j == arrays[a].Length)
            {
                j = 0;
                a++;
            }
            newArray[i] = arrays[a][j];
        }

        return newArray;
    }

    public static void CutArray<T>(ref T[] array, int cutIndex)
    {
        T[] newArray = new T[cutIndex];

        for (int i = 0; i < newArray.Length; i++)
        {
            newArray[i] = array[i];
        }

        array = newArray;
    }

    public static void Swap<T>(ref T obj1, ref T obj2)
    {
        T temp = obj1;
        obj1 = obj2;
        obj2 = temp;
    }

    public static bool CorrectLocation(Transform objBehind, Transform objTowards)
    {
        // Check if parts places not as was expected
        if (objBehind.InverseTransformPoint(objTowards.position).z > 0)
        {
            return true;
        }

        return false;
    }

    public struct TrackPartComps
    {
        public Transform tr;
        public Composition comp;
        public TrackPart tp;
    }

    public struct FillStruct
    {
        public GameObject go;
        public Transform begin;

        public MeshFilter fill_0msh;
        public MeshFilter fence0_0msh;
        public MeshFilter fence1_0msh;
        public Transform  end_0tr;

        public MeshFilter fill_1msh;
        public MeshFilter fence0_1msh;
        public MeshFilter fence1_1msh;
        public Transform  end_1tr;

        public MeshFilter fill_2msh;
        public MeshFilter fence0_2msh;
        public MeshFilter fence1_2msh;
        public Transform  end_2tr;

        public float mainLength;
        public float partLength;
    }

    public static FillStruct InitializeFill(GameObject fill)
    {
        FillStruct fs = new FillStruct();

        for (int i = 0; i < fill.transform.childCount; i++)
        {
            Transform child = fill.transform.GetChild(i);

            // Road meshes
            if (child.name.Contains("fill_0"))
                fs.fill_0msh = child.GetComponent<MeshFilter>();
            if (child.name.Contains("fill_1"))
                fs.fill_1msh = child.GetComponent<MeshFilter>();
            if (child.name.Contains("fill_2"))
                fs.fill_2msh = child.GetComponent<MeshFilter>();

            // Fence meshes
            if (child.name.Contains("fence_concrete_0"))
                fs.fence0_0msh = child.GetComponent<MeshFilter>();
            if (child.name.Contains("fence_concrete_1"))
                fs.fence0_1msh = child.GetComponent<MeshFilter>();
            if (child.name.Contains("fence_concrete_2"))
                fs.fence0_2msh = child.GetComponent<MeshFilter>();
            if (child.name.Contains("fence_metal_0"))
                fs.fence1_0msh = child.GetComponent<MeshFilter>();
            if (child.name.Contains("fence_metal_1"))
                fs.fence1_1msh = child.GetComponent<MeshFilter>();
            if (child.name.Contains("fence_metal_2"))
                fs.fence1_2msh = child.GetComponent<MeshFilter>();

            // End transform key
            if (child.name.Contains("end_0"))
                fs.end_0tr = child;
            if (child.name.Contains("end_1"))
                fs.end_1tr = child;
            if (child.name.Contains("end_2"))
                fs.end_2tr = child;

            // Begin transform key
            if (child.name.Contains("begin"))
                fs.begin = child;
        }

        fs.go = fill;
        fs.partLength = fs.fill_0msh.sharedMesh.bounds.size.z;
        fs.mainLength = fs.partLength * 3;

        // Debug.Log("Fence 0: " + fs.fence_0msh.transform.name + ", fence 1: " + fs.fence_1msh.transform.name + ", fence 2: " + fs.fence_2msh.transform.name);

        return fs;
    }

    public static int IncrementIndex(int i, int max)
    {
        i++;
        if (i > max)
            i = 0;

        return i;
    }

    public static int DecrementIndex(int i, int max)
    {
        i--;
        if (i < 0)
            i = max;

        return i;
    }

    public static bool RoughlyEqual(Vector3 v1, Vector3 v2, float prec)
    {
        if ((v2.x >= v1.x - prec && v2.x <= v1.x + prec) &&
            (v2.y >= v1.y - prec && v2.y <= v1.y + prec) &&
            (v2.z >= v1.z - prec && v2.z <= v1.z + prec))

            return true;

        return false;
    }

    public static Vector3 GetAverage(params Vector3[] points)
    {
        Vector3 tmp = new Vector3();
        for (int i = 0; i < points.Length; i++)
            tmp += points[i];

        return tmp / points.Length;
    }

    public static GameObject[] ExtractGameObjects(GameObject[] gos)
    {
        GameObject[] extracted = new GameObject[] { };

        for (int i = 0; i < gos.Length; i++)
        {
            extracted = MergeArrays(extracted, gos[i].GetComponentsInChildren<GameObject>());
        }

        return extracted;
    }

    public static GameObject MergeGameObjects(string name, bool atCenter, Vector3 additionalScale, params GameObject[] objects)
    {
        Vector3[] vertices = { };
        int[][] subMeshesIDs = { };
        Vector3[] normals = { };
        Vector4[] tangents = { };
        Vector2[] uv = { };
        Material[] materials = { };

        Vector3[] pivots = { };
        Vector3 origin;

        if (atCenter)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                Add(ref pivots, objects[i].transform.position);
            }

            origin = GetAverage(pivots);
        }
        else
        {
            origin = objects[0].transform.position;
        }

        for (int i = 0; i < objects.Length; i++)
        {
            int offset = vertices.Length;

            MeshFilter mf = objects[i].GetComponent<MeshFilter>();
            MeshRenderer mr = objects[i].GetComponent<MeshRenderer>();
            if (mf == null || mr == null)
                continue;

            Vector3[] vertTmp = mf.sharedMesh.vertices;
            Vector3[] normTmp = mf.sharedMesh.normals;
            Vector2[] uvTmp = mf.sharedMesh.uv;
            Vector4[] tangTmp = mf.sharedMesh.tangents;
            Vector3[] tangXYZ = new Vector3[tangTmp.Length];

            if (uvTmp.Length != vertTmp.Length)
            {
                uvTmp = new Vector2[vertTmp.Length];

                for (int u = 0; u < uvTmp.Length; u++)
                {
                    uvTmp[u] = Vector2.zero;
                }
            }

            for (int t = 0; t < tangXYZ.Length; t++)
            {
                tangXYZ[t] = new Vector3(tangTmp[t].x, tangTmp[t].y, tangTmp[t].z);
            }

            // Debug.Log("For object " + objects[i].name + " vertex count is " + vertTmp.Length.ToString() + ", normals count " + normTmp.Length.ToString());

            objects[i].transform.localScale = new Vector3(objects[i].transform.localScale.x * additionalScale.x,
                                                          objects[i].transform.localScale.y * additionalScale.y,
                                                          objects[i].transform.localScale.z * additionalScale.z);

            Matrix4x4 matrix = Matrix4x4.TRS(objects[i].transform.position,
                                             objects[i].transform.rotation,
                                             objects[i].transform.localScale);

            Matrix4x4 r = Matrix4x4.TRS(Vector3.zero, objects[i].transform.rotation, Vector3.one);

            for (int v = 0; v < vertTmp.Length; v++)
            {
                // Apply object transform to normals
                normTmp[v] = r.MultiplyVector(normTmp[v]);
                // Apply object transform to tangents
                tangXYZ[v] = r.MultiplyVector(tangXYZ[v]);
                // Apply object transform to vertices
                vertTmp[v] = matrix.MultiplyPoint3x4(vertTmp[v]);
                // Subtract origin offset
                vertTmp[v] = vertTmp[v] - origin;
            }

            for (int t = 0; t < tangXYZ.Length; t++)
            {
                tangTmp[t].x = tangXYZ[t].x;
                tangTmp[t].y = tangXYZ[t].y;
                tangTmp[t].z = tangXYZ[t].z;
            }

            vertices = MergeArrays(vertices, vertTmp);
            normals = MergeArrays(normals, normTmp);
            tangents = MergeArrays(tangents, tangTmp);
            uv = MergeArrays(uv, uvTmp);

            if (vertices.Length >= 65000)
            {
                Debug.Log("Too many vertices, must be less then 65000.");

                return null;
            }

            // Debug.Log("Materials count: " + mr.sharedMaterials.Length.ToString() + ", submeshes count: " + mf.sharedMesh.subMeshCount.ToString());

            for (int m = 0; m < mf.sharedMesh.subMeshCount; m++)
            {
                // Get all tris in current submesh
                int[] tris = mf.sharedMesh.GetIndices(m);
                // Find or create new material slot
                int matID = IndexOf(materials, mr.sharedMaterials[m]);
                if (matID == -1)
                {
                    Add(ref materials, mr.sharedMaterials[m]);
                    Add(ref subMeshesIDs, new int[0]);
                    matID = materials.Length - 1;
                }
                // Apply indices offset to triangles indices because we've got new vertices
                for (int k = 0; k < tris.Length; k++)
                {
                    tris[k] += offset;
                }

                subMeshesIDs[matID] = MergeArrays(subMeshesIDs[matID], tris);
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = name;
        mesh.vertices = vertices;
        mesh.tangents = tangents;
        mesh.uv = uv;
        mesh.normals = normals;
        mesh.subMeshCount = subMeshesIDs.Length;
        for (int s = 0; s < subMeshesIDs.Length; s++)
        {
            mesh.SetIndices(subMeshesIDs[s], MeshTopology.Triangles, s);

            // Debug.Log("Setting " + s.ToString() + "'s submesh, indices count: " + subMeshesIDs[s].Length.ToString());
        }

        GameObject go = new GameObject();
        go.name = name;
        go.transform.position += origin;
        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshRenderer>().materials = materials;

        return go;
    }
}

[ExecuteInEditMode]
[System.Serializable]
public class Composition : MonoBehaviour
{
    [SerializeField] [HideInInspector]
    private Composition prevComposition;
    [SerializeField] [HideInInspector]
    private Composition nextComposition;
    [SerializeField] [HideInInspector]
    private ObjectType type;
    [SerializeField] [HideInInspector]
    private TrackPart thisPart;
    [SerializeField] [HideInInspector]
    private TrackPart prevPart;
    [SerializeField] [HideInInspector]
    private ConnectionType connection;
    [SerializeField] [HideInInspector]
    private int variantCount;
    [SerializeField] [HideInInspector]
    private int curVariant;
    [SerializeField] [HideInInspector]
    private bool configured = false;
    [SerializeField] [HideInInspector]
    private bool first = false;
    [SerializeField] [HideInInspector]
    private bool w15m = false;
    [SerializeField] [HideInInspector]
    private bool fenceExists = false;

    private enum ConnectionType
    {
        None,
        SingleToSingle,
        ForkToFork,
        ForkToSingle,
        SingleToFork
    };

    private void Awake()
    {
        if (RaceKitService.fenceType != FenceType.None)
            fenceExists = true;

        if (!configured)
        {
            if (RaceKitService.raceKit.transform.childCount == 0)
                first = true;

            if (gameObject.name.Contains("15m") | gameObject.name.Contains("to15"))
                w15m = true;

            transform.SetParent(RaceKitService.raceKit.transform);
            if (RaceKitService.activeComposition != null)
                transform.SetSiblingIndex(RaceKitService.activeComposition.transform.GetSiblingIndex() + 1);
            connection = ConnectionType.None;
            variantCount = 0;
            curVariant = 0;

            if (RaceKitService.creatingID != -1)
            {
                // Creating regular part

                type = RaceKitService.prefabTypes[RaceKitService.creatingID];
                thisPart = new TrackPart(transform);

                if (!first)
                {
                    prevComposition = RaceKitService.activeComposition;
                    prevPart = prevComposition.thisPart;
                    if (prevPart != null)
                    {
                        connection = GetConnectionType(prevPart.Type(), thisPart.Type());
                        variantCount = GetVariantCount(connection);
                    }
                }

                ConfigureFence();
            }
            else
            {
                // Creating filling part

                if (RaceKitService.activeObjects.Length != 2)
                    return;

                type = ObjectType.Fill;

                prevComposition = RaceKitService.activeObjects[0].GetComponent<Composition>();
                nextComposition = RaceKitService.activeObjects[1].GetComponent<Composition>();

                if (!RaceKitService.CorrectLocation(prevComposition.transform, nextComposition.transform))
                {
                    RaceKitService.Swap(ref prevComposition, ref nextComposition);
                }

                prevPart = prevComposition.thisPart;

                if (prevComposition.gameObject.name.Contains("to9"))
                {
                    w15m = false;
                }
                else
                if (prevComposition.gameObject.name.Contains("to15"))
                {
                    w15m = true;
                }

                nextComposition.prevComposition = null;
                nextComposition.Configure();

                connection = GetConnectionType(prevComposition.Type(), nextComposition.Type());
                variantCount = GetVariantCount(connection);

                // Debug.Log("Prev composition type: " + fillPrevComp.Type().ToString() + ", next composition type: " + fillNextComp.Type().ToString());
            }

            SetConnection();

        }

        configured = true;
    }

    public void Configure()
    {
        if(prevComposition == null)
        {
            connection = ConnectionType.None;
            GetVariantCount(connection);
            if (RaceKitService.raceKit.transform.childCount > 1)
                first = false;
            curVariant = 0;
        }
    }

    private void ConfigureFence()
    {
        Transform[] allTransforms = GetComponentsInChildren<Transform>();
        switch (RaceKitService.fenceType)
        {
            case FenceType.None:
                for (int i = 0; i < allTransforms.Length; i++)
                {
                    if (allTransforms[i].name.Contains("fence") && (allTransforms[i].name.Contains("concrete") || allTransforms[i].name.Contains("metal")))
                    {
                        DestroyImmediate(allTransforms[i].gameObject);
                    }
                }
                break;
            case FenceType.Metal:
                for (int i = 0; i < allTransforms.Length; i++)
                {
                    if (allTransforms[i].name.Contains("fence") && allTransforms[i].name.Contains("concrete"))
                    {
                        DestroyImmediate(allTransforms[i].gameObject);
                    }
                }
                break;
            case FenceType.Concrete:
                for (int i = 0; i < allTransforms.Length; i++)
                {
                    if (allTransforms[i].name.Contains("fence") && allTransforms[i].name.Contains("metal"))
                    {
                        DestroyImmediate(allTransforms[i].gameObject);
                    }
                }
                break;
            default:
                break;
        }
    }

    public ObjectType Type()
    {
        return type;
    }

    public TrackPart GetTrackPart()
    {
        return thisPart;
    }

    private int GetVariantCount(ConnectionType con)
    {
        switch (con)
        {
            case ConnectionType.None:
                return 0;
            case ConnectionType.SingleToSingle:
                return 1;
            case ConnectionType.ForkToFork:
                if (type == ObjectType.Fill)
                    return 4;
                else
                    return 3;
            case ConnectionType.ForkToSingle:
                return 2;
            case ConnectionType.SingleToFork:
                return 2;
            default:
                return 0;
        }
    }

    public void NextConnection()
    {
        if (RaceKitService.raceKit.transform.childCount > 1)
        {
            curVariant = RaceKitService.IncrementIndex(curVariant, variantCount - 1);
            SetConnection();
        }
    }

    public void Fill(Transform firstKey, Transform lastKey, bool w15m)
    {
        if (!RaceKitService.RoughlyEqual(firstKey.forward, lastKey.forward, .01f))
        {
            Debug.Log("Directions must be same!");
            DestroyImmediate(gameObject);

            return;
        }

        RaceKitService.FillStruct fillSource;

        if (!w15m)
        {
            fillSource = RaceKitService.InitializeFill(RaceKitService.prefabs[RaceKitService.fill9mID]);
        }
        else
        {
            fillSource = RaceKitService.InitializeFill(RaceKitService.prefabs[RaceKitService.fill15mID]);
        }

        // Get distances
        Vector3 relativePos = firstKey.InverseTransformPoint(lastKey.position);

        int fillCount = (int)(relativePos.z / fillSource.mainLength);
        int partsCount = Mathf.RoundToInt(relativePos.z % fillSource.mainLength / fillSource.partLength);

        if (fillCount == 0 && partsCount == 0)
        {
            DestroyImmediate(gameObject);

            return;
        }

        // To destroy all childs in editor mode we need temporary array of them
        GameObject[] temp = new GameObject[transform.childCount];

        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = transform.GetChild(i).gameObject;
        }

        foreach (GameObject child in temp)
        {
            DestroyImmediate(child);
        }

        float pureLength = fillCount * fillSource.mainLength + partsCount * fillSource.partLength;
        float scaleK = relativePos.z / pureLength;

        GameObject[] allFills;
        if (partsCount > 0)
        {
            allFills = new GameObject[fillCount + 1];
        }
        else
        {
            allFills = new GameObject[fillCount];
        }

        MeshFilter[] allFillM  = new MeshFilter[fillCount * 3 + partsCount];
        GameObject[] allFillG  = new GameObject[fillCount * 3 + partsCount];
        GameObject[] allFence0 = new GameObject[fillCount * 3 + partsCount];
        GameObject[] allFence1 = new GameObject[fillCount * 3 + partsCount];

        RaceKitService.FillStruct prevFill = new RaceKitService.FillStruct();
        for (int i = 0; i < fillCount; i++)
        {
            RaceKitService.FillStruct curFill = RaceKitService.InitializeFill(Instantiate(fillSource.go));
            curFill.go.transform.localScale = new Vector3(1f, 1f, scaleK);

            if (prevFill.go == null)
            {
                TrackPart.PlacePart(curFill.go.transform, curFill.begin, firstKey);
            }
            else
            {
                TrackPart.PlacePart(curFill.go.transform, curFill.begin, prevFill.end_2tr);
            }

            allFills[i] = curFill.go;

            allFillM[i * 3 + 0] = curFill.fill_0msh;
            allFillM[i * 3 + 1] = curFill.fill_1msh;
            allFillM[i * 3 + 2] = curFill.fill_2msh;

            allFillG[i * 3 + 0] = curFill.fill_0msh.gameObject;
            allFillG[i * 3 + 1] = curFill.fill_1msh.gameObject;
            allFillG[i * 3 + 2] = curFill.fill_2msh.gameObject;

            if (fenceExists)
            {
                allFence0[i * 3 + 0] = curFill.fence0_0msh.gameObject;
                allFence0[i * 3 + 1] = curFill.fence0_1msh.gameObject;
                allFence0[i * 3 + 2] = curFill.fence0_2msh.gameObject;

                allFence1[i * 3 + 0] = curFill.fence1_0msh.gameObject;
                allFence1[i * 3 + 1] = curFill.fence1_1msh.gameObject;
                allFence1[i * 3 + 2] = curFill.fence1_2msh.gameObject;
            }

            prevFill = curFill;
        }

        if (partsCount != 0)
        {
            RaceKitService.FillStruct lastFill = RaceKitService.InitializeFill(Instantiate(fillSource.go));
            lastFill.go.transform.localScale = new Vector3(1f, 1f, scaleK);

            allFills[fillCount] = lastFill.go;

            allFillM[3 * fillCount] = lastFill.fill_0msh;
            allFillG[3 * fillCount] = lastFill.fill_0msh.gameObject;
            if (fenceExists)
            {
                allFence0[3 * fillCount] = lastFill.fence0_0msh.gameObject;
                allFence1[3 * fillCount] = lastFill.fence1_0msh.gameObject;
            }

            if (partsCount >= 2)
            {
                allFillM[3 * fillCount + 1] = lastFill.fill_1msh;
                allFillG[3 * fillCount + 1] = lastFill.fill_1msh.gameObject;
                if (fenceExists)
                {
                    allFence0[3 * fillCount + 1] = lastFill.fence0_1msh.gameObject;
                    allFence1[3 * fillCount + 1] = lastFill.fence1_1msh.gameObject;
                }
            }

            if (partsCount == 3)
            {
                allFillM[3 * fillCount + 2] = lastFill.fill_2msh;
                allFillG[3 * fillCount + 2] = lastFill.fill_2msh.gameObject;
                if (fenceExists)
                {
                    allFence0[3 * fillCount + 2] = lastFill.fence0_2msh.gameObject;
                    allFence1[3 * fillCount + 2] = lastFill.fence1_2msh.gameObject;
                }
            }

            // Set new fill part at previous's end point
            if (prevFill.go != null)
            {
                TrackPart.PlacePart(lastFill.go.transform, lastFill.begin, prevFill.end_2tr);
            }
            else
            {
                TrackPart.PlacePart(lastFill.go.transform, lastFill.begin, firstKey);
            }

            // Destroy unnecessary parts
            if (partsCount <= 2)
            {
                DestroyImmediate(lastFill.fill_2msh.gameObject);
                DestroyImmediate(lastFill.end_2tr.gameObject);
                if (fenceExists)
                {
                    DestroyImmediate(lastFill.fence0_2msh.gameObject);
                    DestroyImmediate(lastFill.fence1_2msh.gameObject);
                }
            }
            if (partsCount < 2)
            {
                DestroyImmediate(lastFill.fill_1msh.gameObject);
                DestroyImmediate(lastFill.end_1tr.gameObject);
                if (fenceExists)
                {
                    DestroyImmediate(lastFill.fence0_1msh.gameObject);
                    DestroyImmediate(lastFill.fence1_1msh.gameObject);
                }
            }
        }

        for (int i = 0; i < allFillM.Length; i++)
        {
            Mesh roadMsh  = Instantiate(allFillM[i].sharedMesh);
            Vector3[] roadVerts = roadMsh.vertices;

            for (int v = 0; v < roadVerts.Length; v++)
            {
                // Very complex math
                float newX = roadVerts[v].x + RaceKitService.fillingCurve.Evaluate((i + roadVerts[v].z / fillSource.partLength) / allFillM.Length) * relativePos.x;
                roadVerts[v] = new Vector3(newX, roadVerts[v].y, roadVerts[v].z);
            }

            roadMsh.vertices = roadVerts;
            allFillM[i].mesh = roadMsh;

            if (fenceExists)
            {
                Mesh fence0Msh = Instantiate(allFence0[i].GetComponent<MeshFilter>().sharedMesh);
                Mesh fence1Msh = Instantiate(allFence1[i].GetComponent<MeshFilter>().sharedMesh);
                Vector3[] fence0Verts = fence0Msh.vertices;
                Vector3[] fence1Verts = fence1Msh.vertices;

                for (int v = 0; v < fence0Verts.Length; v++)
                {
                    // Very complex math
                    float newX = fence0Verts[v].x + RaceKitService.fillingCurve.Evaluate((i + fence0Verts[v].z / fillSource.partLength) / allFence0.Length) * relativePos.x;
                    fence0Verts[v] = new Vector3(newX, fence0Verts[v].y, fence0Verts[v].z);
                }

                for (int v = 0; v < fence1Verts.Length; v++)
                {
                    // Very complex math
                    float newX = fence1Verts[v].x + RaceKitService.fillingCurve.Evaluate((i + fence1Verts[v].z / fillSource.partLength) / allFence1.Length) * relativePos.x;
                    fence1Verts[v] = new Vector3(newX, fence1Verts[v].y, fence1Verts[v].z);
                }

                fence0Msh.vertices = fence0Verts;
                fence1Msh.vertices = fence1Verts;
                allFence0[i].GetComponent<MeshFilter>().mesh = fence0Msh;
                allFence1[i].GetComponent<MeshFilter>().mesh = fence1Msh;
            }
        }

        GameObject roadGO  = RaceKitService.MergeGameObjects("fill",  false, new Vector3(1f, 1f, scaleK), allFillG);

        transform.position = roadGO.transform.position;
        transform.rotation = roadGO.transform.rotation;
        transform.localScale = roadGO.transform.localScale;

        roadGO.transform.SetParent(transform);

        if (fenceExists)
        {
            GameObject fenceConcreteGO = RaceKitService.MergeGameObjects("fence_concrete", false, new Vector3(1f, 1f, scaleK), allFence0);
            fenceConcreteGO.transform.SetParent(transform);

            fenceConcreteGO.transform.localPosition = new Vector3();
            fenceConcreteGO.transform.localRotation = Quaternion.identity;
            fenceConcreteGO.transform.localScale = roadGO.transform.localScale;

            GameObject fenceMetalGO = RaceKitService.MergeGameObjects("fence_metal", false, new Vector3(1f, 1f, scaleK), allFence1);
            fenceMetalGO.transform.SetParent(transform);

            fenceMetalGO.transform.localPosition = new Vector3();
            fenceMetalGO.transform.localRotation = Quaternion.identity;
            fenceMetalGO.transform.localScale = roadGO.transform.localScale;
        }

        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();

        mf.mesh = roadGO.GetComponent<MeshFilter>().sharedMesh;
        mr.materials = roadGO.GetComponent<MeshRenderer>().sharedMaterials;

        DestroyImmediate(roadGO);

        for (int i = 0; i < allFills.Length; i++)
        {
            DestroyImmediate(allFills[i]);
        }

        GameObject beginObj = new GameObject("begin");
        beginObj.transform.position = firstKey.transform.position;
        beginObj.transform.rotation = firstKey.transform.rotation;
        beginObj.transform.SetParent(transform);

        GameObject endObj = new GameObject("end");
        endObj.transform.position = lastKey.transform.position;
        endObj.transform.rotation = lastKey.transform.rotation;
        endObj.transform.SetParent(transform);

        thisPart = new TrackPart(transform);
        nextComposition.prevComposition = this;
    }

    private void SetConnection()
    {
        switch (connection)
        {
            case ConnectionType.None:
                if (first)
                {
                    TrackPart.PlacePart(transform);
                }
                break;
            case ConnectionType.SingleToSingle:
                if (type == ObjectType.Fill)
                {
                    Fill(prevComposition.thisPart.endKeyMiddle, nextComposition.thisPart.beginKeyMiddle, prevComposition.w15m);
                }
                else
                {
                    TrackPart.PlacePart(transform, thisPart.beginKeyMiddle, prevPart.endKeyMiddle);
                }
                break;
            case ConnectionType.ForkToFork:
                if (type == ObjectType.Fill)
                {
                    if (curVariant == 0)
                    {
                        Fill(prevComposition.thisPart.endKeyLeft, nextComposition.thisPart.beginKeyLeft, prevComposition.w15m);
                    }
                    else
                    if (curVariant == 1)
                    {
                        Fill(prevComposition.thisPart.endKeyLeft, nextComposition.thisPart.beginKeyRight, prevComposition.w15m);
                    }
                    else
                    if (curVariant == 2)
                    {
                        Fill(prevComposition.thisPart.endKeyRight, nextComposition.thisPart.beginKeyLeft, prevComposition.w15m);
                    }
                    else
                    if (curVariant == 3)
                    {
                        Fill(prevComposition.thisPart.endKeyRight, nextComposition.thisPart.beginKeyRight, prevComposition.w15m);
                    }
                }
                else
                {
                    if (curVariant == 0)
                    {
                        TrackPart.PlacePart(transform, thisPart.beginKeyLeft, prevPart.endKeyLeft);
                    }
                    else
                    if (curVariant == 1)
                    {
                        TrackPart.PlacePart(transform, thisPart.beginKeyLeft, prevPart.endKeyRight);
                    }
                    else
                    if (curVariant == 2)
                    {
                        TrackPart.PlacePart(transform, thisPart.beginKeyRight, prevPart.endKeyLeft);
                    }
                }
                break;
            case ConnectionType.ForkToSingle:
                if (type == ObjectType.Fill)
                {
                    if (curVariant == 0)
                    {
                        Fill(prevComposition.thisPart.endKeyMiddle, nextComposition.thisPart.beginKeyLeft, prevComposition.w15m);
                    }
                    else
                    if (curVariant == 1)
                    {
                        Fill(prevComposition.thisPart.endKeyMiddle, nextComposition.thisPart.beginKeyRight, prevComposition.w15m);
                    }
                }
                else
                {
                    if (curVariant == 0)
                    {
                        TrackPart.PlacePart(transform, thisPart.beginKeyLeft, prevPart.endKeyMiddle);
                    }
                    else
                    if (curVariant == 1)
                    {
                        TrackPart.PlacePart(transform, thisPart.beginKeyRight, prevPart.endKeyMiddle);
                    }
                }
                break;
            case ConnectionType.SingleToFork:
                if (type == ObjectType.Fill)
                {
                    if (curVariant == 0)
                    {
                        Fill(prevComposition.thisPart.endKeyLeft, nextComposition.thisPart.beginKeyMiddle, prevComposition.w15m);
                    }
                    else
                    if (curVariant == 1)
                    {
                        Fill(prevComposition.thisPart.endKeyRight, nextComposition.thisPart.beginKeyMiddle, prevComposition.w15m);
                    }
                }
                else
                {
                    if (curVariant == 0)
                    {
                        TrackPart.PlacePart(transform, thisPart.beginKeyMiddle, prevPart.endKeyLeft);
                    }
                    else
                    if (curVariant == 1)
                    {
                        TrackPart.PlacePart(transform, thisPart.beginKeyMiddle, prevPart.endKeyRight);
                    }
                }
                break;
            default:
                break;
        }

        if(type == ObjectType.Fill)
        {
            ConfigureFence();
        }
    }

    private static ConnectionType GetConnectionType(ObjectType prevObjType, ObjectType curObjType)
    {
        if (curObjType == ObjectType.Single && prevObjType == ObjectType.Single)
            return ConnectionType.SingleToSingle;
        else
        if (curObjType == ObjectType.Single && prevObjType == ObjectType.FrontFork)
            return ConnectionType.SingleToFork;
        else
        if (curObjType == ObjectType.Single && prevObjType == ObjectType.BackFork)
            return ConnectionType.SingleToSingle;
        else
        if (curObjType == ObjectType.FrontFork && prevObjType == ObjectType.Single)
            return ConnectionType.SingleToSingle;
        else
        if (curObjType == ObjectType.FrontFork && prevObjType == ObjectType.FrontFork)
            return ConnectionType.SingleToFork;
        else
        if (curObjType == ObjectType.FrontFork && prevObjType == ObjectType.BackFork)
            return ConnectionType.SingleToSingle;
        else
        if (curObjType == ObjectType.BackFork && prevObjType == ObjectType.Single)
            return ConnectionType.ForkToSingle;
        else
        if (curObjType == ObjectType.BackFork && prevObjType == ObjectType.FrontFork)
            return ConnectionType.ForkToFork;
        else
        if (curObjType == ObjectType.BackFork && prevObjType == ObjectType.BackFork)
            return ConnectionType.ForkToSingle;
        if (curObjType == ObjectType.Single && prevObjType == ObjectType.Fill)
            return ConnectionType.SingleToSingle;
        else
        if (curObjType == ObjectType.FrontFork && prevObjType == ObjectType.Fill)
            return ConnectionType.SingleToSingle;
        else
        if (curObjType == ObjectType.BackFork && prevObjType == ObjectType.Fill)
            return ConnectionType.ForkToSingle;
        else
            return ConnectionType.None;
    }
}

[System.Serializable]
public class TrackPart
{
    [SerializeField] [HideInInspector]
    public Transform beginKeyLeft;
    [SerializeField] [HideInInspector]
    public Transform beginKeyRight;
    [SerializeField] [HideInInspector]
    public Transform beginKeyMiddle;
    [SerializeField] [HideInInspector]
    public Transform endKeyLeft;
    [SerializeField] [HideInInspector]
    public Transform endKeyRight;
    [SerializeField] [HideInInspector]
    public Transform endKeyMiddle;
    [SerializeField] [HideInInspector]
    private ObjectType type;

    private bool debug = false;

    public TrackPart(Transform transform)
    {
        /// <summary> Creating wrap for existing track object </summary>

        beginKeyLeft   = RaceKitService.GetChildByName(transform, "begin", "left" );
        if (debug && beginKeyLeft != null)
            Debug.Log(beginKeyLeft.name);
        beginKeyRight  = RaceKitService.GetChildByName(transform, "begin", "right");
        if (debug && beginKeyRight != null)
            Debug.Log(beginKeyRight.name);
        beginKeyMiddle = RaceKitService.GetChildByName(transform, "begin");
        if (debug && beginKeyMiddle != null)
            Debug.Log("Begin Middle: " + beginKeyMiddle.name);

        endKeyLeft     = RaceKitService.GetChildByName(transform, "end", "left");
        if (debug && endKeyLeft != null)
            Debug.Log(endKeyLeft.name);
        endKeyRight    = RaceKitService.GetChildByName(transform, "end", "right");
        if (debug && endKeyRight != null)
            Debug.Log(endKeyRight.name);
        endKeyMiddle   = RaceKitService.GetChildByName(transform, "end");
        if (debug && endKeyMiddle != null)
            Debug.Log("End Middle: " + endKeyMiddle.name);

        if (RaceKitService.creatingID != -1)
        {
            type = RaceKitService.prefabTypes[RaceKitService.creatingID];
        }
        else
        {
            type = ObjectType.Fill;
        }
    }

    public ObjectType Type()
    {
        return type;
    }

    public static void PlacePart(Transform curPart, Transform curPartBeginKey, Transform prevPartEndKey)
    {
        curPart.rotation = prevPartEndKey.rotation;
        curPart.position = prevPartEndKey.position - curPartBeginKey.position + curPart.position;
    }

    public static void PlacePart(Transform curPart)
    {
        curPart.localPosition = new Vector3();
        curPart.localRotation = Quaternion.identity;
    }
}
}
