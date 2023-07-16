using UnityEngine;
using UnityEditor;


public class DotProductEditor : EditorWindow
{
    public Vector3 m_p0;
    public Vector3 m_p1;
    public Vector3 m_p2;

    private SerializedObject obj;
    private SerializedProperty propP0;
    private SerializedProperty propP1;
    private SerializedProperty propP2;

    private GUIStyle guiStyle = new GUIStyle();




    [MenuItem("Tools/Dot Product")]
    public static void ShowWindow(){
        string windowName = "Dot Product";
        bool isPopUp = false;
        DotProductEditor window = (DotProductEditor) GetWindow(typeof (DotProductEditor), isPopUp, windowName);
        window.Show();

    }
    private void OnEnable() {
        InitializePoints();
        InitializeSerializedProperties();
        InitializeGUIStyle();
        SceneView.duringSceneGui += SceneGUI;
    }

    private void OnDisable() {
        SceneView.duringSceneGui -= SceneGUI;
    }

    private void SceneGUI(SceneView view){
        PaintPoints();
    }

    private void OnGUI() {
        // updates serializedproperties to match local variables
        obj.Update();

        DrawBlockGUI("p0", propP0);
        DrawBlockGUI("p1", propP1);
        DrawBlockGUI("p2", propP2);

        if(obj.ApplyModifiedProperties()){
            // Refreshes the scene view
            SceneView.RepaintAll();
        }
    }





    // Helper functions
    private void InitializePoints(){
        if(m_p1 == Vector3.zero && m_p2 == Vector3.zero){
            m_p0 = Vector3.zero;
            m_p1 = new Vector3(0.0f, 1.0f, 0.0f);
            m_p2 = new Vector3(0.5f, 0.5f, 0.0f);
        }
    }

    private void InitializeSerializedProperties(){
        obj = new SerializedObject(this);
        propP0 = obj.FindProperty("m_p0");
        propP1 = obj.FindProperty("m_p1");
        propP2 = obj.FindProperty("m_p2");
    }


    private Vector3 SetMovePoint(Vector3 initialPos){
        float size = HandleUtility.GetHandleSize(Vector3.zero) * 0.15f;
        Vector3 newPosition = Handles.FreeMoveHandle(initialPos, Quaternion.identity, size, Vector3.zero, Handles.SphereHandleCap);
        return newPosition;
    } 

    private void PaintPoints(){
        Handles.color = Color.white;
        Vector3 p0 = SetMovePoint(m_p0);
        Handles.color = Color.red;
        Vector3 p1 = SetMovePoint(m_p1);
        Handles.color = Color.green;
        Vector3 p2 = SetMovePoint(m_p2);

        // Repainting
        if(p0 != m_p0 || p1 != m_p1 || p2 != m_p2){
            m_p0 = p0;
            m_p1 = p1;
            m_p2 = p2;
            // OnGUI will be called
            Repaint();
        }

        DrawLabel(p0, p1, p2);
    }

    private void DrawBlockGUI(string label, SerializedProperty prop){
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField(label, GUILayout.Width(50));
        EditorGUILayout.PropertyField(prop, GUIContent.none);
        EditorGUILayout.EndHorizontal();
    }

    private void InitializeGUIStyle(){
        guiStyle.fontSize = 25;
        guiStyle.fontStyle = FontStyle.Bold;
        guiStyle.normal.textColor = Color.white;
    }

    private float DotProduct(Vector3 p0, Vector3 p1, Vector3 p2){
        Vector3 a = (p0 - p1).normalized;
        Vector3 b = (p0 - p2).normalized;

        return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
    }

    private void DrawLabel(Vector3 p0, Vector3 p1, Vector3 p2){
        Handles.Label(p0, DotProduct(p0, p1, p2).ToString("F1"), guiStyle);
        Handles.color = Color.black;
        Handles.DrawAAPolyLine(3f, p1, p0);
        Handles.DrawAAPolyLine(3f, p2, p0);

        Vector3 leftVector = new Vector3(0f, 1f, 0f);
        Vector3 cLeft = WorldRotation(p1, p0, leftVector);

        Vector3 RighttVector = new Vector3(0f, -1f, 0f);
        Vector3 cRight = WorldRotation(p1, p0, RighttVector);

        Handles.DrawAAPolyLine(3f, p1, p0);
        Handles.DrawAAPolyLine(3f, p1, p0);
        Handles.DrawAAPolyLine(3f, p0, cLeft);
        Handles.DrawAAPolyLine(3f, p0, cRight);


    }

    private Vector3 WorldRotation(Vector3 p, Vector3 c, Vector3 position){
        
        Vector2 dir = (p-c).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        return c + rotation * position;
    }



}
