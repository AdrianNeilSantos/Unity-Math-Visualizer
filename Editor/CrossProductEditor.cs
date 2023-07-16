using UnityEngine;
using UnityEditor;


public class CrossProductEditor : CommonEditor, IUpdateSceneGUI
{

    public Vector3 m_p;
    public Vector3 m_q;
    public Vector3 m_pxq;

    private SerializedObject obj;
    private SerializedProperty propP;
    private SerializedProperty propQ;
    private SerializedProperty propPXQ;

    private GUIStyle guiStyle = new GUIStyle();



    [MenuItem("Tools/Cross Product")]
    public static void ShowWindow(){
        string windowName = "Cross Product";
        bool isPopUp = false;
        GetWindow(typeof(CrossProductEditor), isPopUp, windowName);
    }



    private void OnEnable() {
        InitializeVectors();
        InitializeSerializedProperties();
        InitializeGUIStyle();
        SceneView.duringSceneGui += SceneGUI;
    }


    private void OnDisable() {
        SceneView.duringSceneGui -= SceneGUI;
    }

    private void OnGUI(){
        obj.Update();

        DrawBlockGUI("P", propP);
        DrawBlockGUI("Q", propQ);
        DrawBlockGUI("PXQ", propPXQ);

        if(obj.ApplyModifiedProperties()){
            SceneView.RepaintAll();
        }

        if(GUILayout.Button("Reset Values")){
            SetDefaultValues();
        }

    }



    public void SceneGUI(SceneView view){
        Vector3 p = Handles.PositionHandle(m_p, Quaternion.identity);
        Vector3 q = Handles.PositionHandle(m_q, Quaternion.identity);

        Handles.color = Color.blue;
        Vector3 pxq = CrossProduct(p, q);
        Handles.DrawSolidDisc(pxq, Vector3.forward, 0.05f);


        if(m_p != p || m_p != q){
            Undo.RecordObject(this, "Tool Move");

            m_p = p;
            m_q = q;
            m_pxq = pxq;

            RepaintOnGUI();
        }

        // Draws label of vectors p, q, and pxq
        DrawLineGUI(p, "P", Color.green);
        DrawLineGUI(q, "Q", Color.red);
        DrawLineGUI(pxq, "PXQ", Color.blue);


    }



    // helper functions

    private void SetDefaultValues(){
        m_p = new Vector3(0.0f, 1.0f, 0.0f);
        m_q = new Vector3(1.0f, 0.0f, 0.0f);
    }

    private void InitializeVectors(){
        if(m_p == Vector3.zero && m_q == Vector3.zero){
            SetDefaultValues();
        }
    }

    private void InitializeSerializedProperties(){
        obj = new SerializedObject(this);
        propP = obj.FindProperty("m_p");
        propQ = obj.FindProperty("m_q");
        propPXQ = obj.FindProperty("m_pxq");
    }


    private void InitializeGUIStyle(){
        guiStyle.fontSize = 25;
        guiStyle.fontStyle = FontStyle.Bold;
        guiStyle.normal.textColor = Color.white;
    }

    private void DrawLineGUI(Vector3 position, string label, Color color){
        Handles.color = color;
        Handles.Label(position, label, guiStyle);
        Handles.DrawAAPolyLine(3f, position, Vector3.zero);
    }

    private void RepaintOnGUI(){
        Repaint();
    }

    // private Vector3 CrossProduct(Vector3 p, Vector3 q){
    //     float x = (p.y * q.z) - (p.z * q.y);
    //     float y = (p.z * q.x) - (p.x - q.z);
    //     float z = (p.x - q.y) - (p.y - q.x);

    //     return new Vector3(x, y, z);
    // }


    // Linear transformation
    private Vector3 CrossProduct(Vector3 p, Vector3 q){
        Matrix4x4 m = new Matrix4x4();
        
        m[0, 0] = 0;
        m[0, 1] = q.z;
        m[0, 2] = -q.y;

        m[1, 0] = -q.z;
        m[1, 1] = 0;
        m[1, 2] = q.x;

        m[2, 0] = q.y;
        m[2, 1] = -q.x;
        m[2, 2] = 0;

        return m * p;
    }



}
