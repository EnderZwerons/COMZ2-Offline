using System.IO;
using UnityEngine;

public class Utils
{
	private static string m_SavePath;

	private static string m_DataPath;

	static Utils()
	{
		m_SavePath = Application.persistentDataPath;
		m_DataPath = Application.dataPath;
		if (m_SavePath[m_SavePath.Length - 1] != '/')
		{
			m_SavePath += "/";
		}
		if (m_DataPath[m_DataPath.Length - 1] != '/')
		{
			m_DataPath += "/";
		}
	}

	public static string SavePath()
	{
		return m_SavePath;
	}

	public static string DataPath()
	{
		return m_DataPath;
	}

	public static string LoadResourcesFileForText(string filename)
	{
		TextAsset textAsset = Resources.Load(filename, typeof(TextAsset)) as TextAsset;
		if (null != textAsset)
		{
			return textAsset.text;
		}
		return string.Empty;
	}

	public static void FileWriteString(string FileName, string WriteString)
	{
		try
		{
			FileStream fileStream = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(WriteString);
			streamWriter.Flush();
			streamWriter.Close();
			fileStream.Close();
		}
		catch
		{
			Debug.Log("File write error:" + FileName);
		}
	}

	public static bool FileReadString(string FileName, ref string content)
	{
		if (!File.Exists(FileName))
		{
			Debug.LogWarning("file: " + FileName + " is not exist!");
			return false;
		}
		try
		{
			FileStream fileStream = new FileStream(FileName, FileMode.Open);
			StreamReader streamReader = new StreamReader(fileStream);
			content = streamReader.ReadToEnd();
			streamReader.Close();
			fileStream.Close();
		}
		catch
		{
			Debug.Log("Load" + FileName + " error");
		}
		return true;
	}
}
