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