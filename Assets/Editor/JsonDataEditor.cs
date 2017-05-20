using UnityEngine;
using UnityEditor;
using System.IO;

public class JsonDataEditor : EditorWindow
{
    public MyData myData;

    private bool isPrettyPrint = false;
    private string filePath = "";

    [MenuItem("Window/Json Data Editor")]
    static void Init()
    {
        GetWindow(typeof(JsonDataEditor)).Show();
    }

    private void OnGUI()
    {
        #region LoadData Button & CreateData Button & PrettyPrint Toggle & SaveData Button

        EditorGUILayout.Space();

        if (GUILayout.Button("Load json data"))
            LoadJsonData();

        EditorGUILayout.Space();

        if (GUILayout.Button("Create json data"))
            CreateNewData();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();

        isPrettyPrint = GUILayout.Toggle(isPrettyPrint, "Pretty Print");

        if (GUILayout.Button("Save data"))
            SaveJsonData();

        GUILayout.EndHorizontal();

        #endregion

        EditorGUILayout.Space();

        #region Handle Data Box
        GUILayout.BeginVertical("Box");
        if (myData != null)
        {
            GUILayout.Label(filePath);
            //序列化myData对象，让它可以显示在编辑窗口。
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("myData");
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();                     //应用修改的属性
        }
        GUILayout.EndVertical();
        #endregion

    }

    private void LoadJsonData()
    {
        //使用系统文件打开窗口。
        filePath = EditorUtility.OpenFilePanel("Select json data file", Application.streamingAssetsPath, "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);                 //读取文件
            myData = JsonUtility.FromJson<MyData>(dataAsJson);              //反序列化JSON
        }
    }

    private void SaveJsonData()
    {
        //使用系统文件保存窗口。
        filePath = EditorUtility.SaveFilePanel("Save json data file", Application.streamingAssetsPath, "", "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(myData, isPrettyPrint);  //序列化JSON
            File.WriteAllText(filePath, dataAsJson);                        //写入文件
        }
    }

    private void CreateNewData()
    {
        filePath = "";
        myData = new MyData();
    }

}