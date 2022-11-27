using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class UserProfile
{
    public static string Login = "  ";
    public static int Points = 0;

    public static void SaveData()
    {
        PlayerPrefs.SetInt(Login, Points);
        using StreamWriter file = new("Logins.txt", append: true);
        file.Write(Login + '\n');

        Debug.Log($"{Login}: {PlayerPrefs.GetInt(Login)}");
    }

    public static Dictionary<string, int> LoadData()
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        if (!File.Exists("Logins.txt")) return null;
        
        var logins = File.ReadAllLines("Logins.txt");
        foreach (var login in logins)
        {
            try
            {
                result.Add(login, PlayerPrefs.GetInt(login));
            }
            catch
            {
                continue;
            }
        }
        return result;
    }
}