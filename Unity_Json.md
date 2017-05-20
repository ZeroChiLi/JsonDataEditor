#Unity JSON文件的写入（序列化）与读取（反序列化）

>参考查阅官方教程：[Dictionary, JSON and Streaming Assets](https://unity3d.com/cn/learn/tutorials/topics/scripting/dictionary-json-and-streaming-assets?playlist=17117)
>官方手册：[JSON Serialization](https://docs.unity3d.com/2017.1/Documentation/Manual/JSONSerialization.html)
>使用主要API：[JsonUtility](https://docs.unity3d.com/2017.1/Documentation/ScriptReference/JsonUtility.html)


 　　JSON(JavaScript Object Notation) 常用于网络通信的数据交换还有打包、解压数据。在Unity就内嵌了处理JSON数据的类（UnityEngine.JsonUtility）。

---
#JsonUtility

- 可用于的类型
	- MonoBehaviour的子类
	- ScriptableObject的子类
	- 带[Serializable]特性的**类**或**结构体**。

　　因为不支持Dictionary（字典类型），但支持类或结构体，所以可以把字典封装到类或结构体内来使用。

---

##三个静态方法

| 方法名 | 签名 | 使用 |
| ---- | ---- | ---- |
| FromJson | public static T FromJson(string json) | 将json格式的字符串，转化成T类型数据，并返回T类型数据。 |
| FromJsonOverwrite | public static void FromJsonOverwrite(string json, object objectToOverwrite) | 如果你已经有需要转换成的类对象，可以直接传入该对象作为赋值对象，省了新开数据对象。 |
| ToJson | public static string ToJson(object obj, bool prettyPrint) | 第二个参数可以省略，等价于赋值false。将obj类型解析成Json数据字符串，如果第二个参数为true，这个字符串就会格式化，看起来舒服一点。 |

---
#实例1. 读取.json文件数据，显示在场景中。

##基本数据类 MyData
	
```csharp
///
///		MyData.cs
///

[System.Serializable]
public class MyData
{
    public MyDataItem[] items;
}

[System.Serializable]
public class MyDataItem
{
    public string key;
    public string value;
}
```

##JSON数据读取，显示在Text中。

```csharp
///
///		JsonDataManager
///

using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class JsonDataManager : MonoBehaviour
{
    public Text showText;

    //文件名，在Assets/StreamingAssets目录下，如：myData.json。
    public string fileName;

    private MyData loadedData;

    private void Start()
    {
        LoadJsonDateText();
        showAllJsonData();
    }

    public void LoadJsonDateText()
    {
        //获取文件路径。
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))                          //如果该文件存在。
        {
            string dataAsJson = File.ReadAllText(filePath); //读取所有数据送到json格式的字符串里面。

            loadedData = JsonUtility.FromJson<MyData>(dataAsJson);

            Debug.Log("Data loaded, dictionary contains: " + loadedData.items.Length + " entries");
        }
        else
            Debug.LogError("Cannot find file! Make sure file in \"Assets/StreamingAssets\" path. And file suffix should be \".json \"");
    }

    public void showAllJsonData()
    {
        showText.text = "";
        for (int i = 0; i < loadedData.items.Length; ++i)
        {
            showText.text += loadedData.items[i].key + "  :  " + loadedData.items[i].value +"\n";
        }
    }
}
```
##测试结果

###1. Unity中操作，如图。

![](http://img.my.csdn.net/uploads/201705/20/1495281495_4241.png)

###2. myData.json的内容如下。

![](http://img.my.csdn.net/uploads/201705/20/1495281495_4314.png)

###3. 运行游戏，显示如下。

![](http://img.my.csdn.net/uploads/201705/20/1495281496_8476.png)


---
#实例2.自定义JSON编辑窗口

##同上一个实例一样，基本数据类 MyData
	
```csharp
///
///		MyData.cs
///

[System.Serializable]
public class MyData
{
    public MyDataItem[] items;
}

[System.Serializable]
public class MyDataItem
{
    public string key;
    public string value;
}
```

## JSON数据编辑窗口类 JsonDataEditor，在菜单window->Json Data Editor。
	
```csharp
///
///		JsonDataEditor.cs
///

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

```

##测试结果

###1. 打开自定义窗口。（Window->Json Data Editor）
![](http://img.my.csdn.net/uploads/201705/20/1495283913_7075.png)

###2. 可以读取文件，创建新文件，可以选择是否格式化（ToJson函数第二个参数是否true），还可以保存文件。
![](http://img.my.csdn.net/uploads/201705/20/1495283913_9402.png)

###3. 结合实例1还可以显示在场景画布。
![](http://img.my.csdn.net/uploads/201705/20/1495283913_2030.png)

---