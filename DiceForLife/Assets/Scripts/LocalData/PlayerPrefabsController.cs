using UnityEngine;

public class PlayerPrefabsController
{
    private static string startCheck = "CuongVM";
    private static string tempData;

    public static string GetStringData(string Key, string defaultValue = "")
    {
        if (PlayerPrefs.HasKey(Key))
        {
            tempData = StringCipher.Decrypt(PlayerPrefs.GetString(Key));
            if (tempData.StartsWith(startCheck))//Data nguyen ven
            {
                return tempData.Substring(7);
            }
            else return defaultValue;
        }
        return defaultValue;
    }
    public static void SetStringData(string Key, string _strData)
    {
        PlayerPrefs.SetString(Key, StringCipher.Encrypt(startCheck + _strData));
    }

    public static void Test()
    {
        string dataTest = "qwerasdfzxcv" + Random.Range(20, 50);
        Debug.Log("data save: " + dataTest);
        PlayerPrefabsController.SetStringData("string", dataTest);
        string dataget = PlayerPrefabsController.GetStringData("string");
        Debug.Log("data get : " + dataget);
        if (dataTest.Equals(dataget)) Debug.Log("Data is good");
        else Debug.Log("Data is bad");
    }
}
