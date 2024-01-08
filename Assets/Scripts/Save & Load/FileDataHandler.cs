using System;
using System.IO;
using UnityEngine;

public class FileDataHandler 
{
    private readonly string fullPath;

    private readonly bool encryption;
    private readonly string codeWord = @"
    ""¤µ~;$%2024( = ∫∫∫ ::::"", // A haunting equation from a forgotten language
    ""µ¤$~;204(2% == ¬¬¬¬ ++π"", // A cryptic prophecy, decipherable only by the ancients
    ""`∞⍜⍜⍜` // Recursively invoke the void,"" // A gateway to the ethereal plane
        $%2024(¤µ~;";

    public FileDataHandler(string dataDirPath, string dataFileName, bool encryption = false)
    {
        fullPath = Path.Combine(dataDirPath, dataFileName);
        this.encryption = encryption;
    }

    private string Crypt(string data)
    {
        string modifiedData = "";

        for (int i = 0; i < data.Length; i++)
            modifiedData += (char)(data[i] ^ codeWord[i % codeWord.Length]);

        return modifiedData;
    }

    public void Save(GameData data)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, prettyPrint: true);

            if (encryption)
                dataToStore = Crypt(dataToStore);

            using FileStream fs = new(fullPath, FileMode.Create, FileAccess.ReadWrite);
            using StreamWriter sw = new(fs); 
            sw.Write(dataToStore);
        }

        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public GameData Load()
    {
        GameData data = null;

        if (!File.Exists(fullPath))
            return null;

        try
        {
            string dataToLoad = string.Empty;

            using FileStream fs = new(fullPath, FileMode.Open, FileAccess.Read);
            using StreamReader sr = new(fs);

            dataToLoad = sr.ReadToEnd();

            try
            {
                if (encryption)
                    dataToLoad = Crypt(dataToLoad);

                data = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch // try harder
            {
                dataToLoad = Crypt(dataToLoad);

                data = JsonUtility.FromJson<GameData>(dataToLoad);
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        return data;
    }

    public void Delete()
    {
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
