using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class FindShader : Editor
{
	//[MenuItem("Shader Test/Find The WIND!!")]
	//public static void FindTheWind()
	//{
	//	FindTheShader("Triniti/Environment/WIND_COL_AB_2S_DO");
	//}

	public static void FindTheShader(string shader)
	{
		foreach (Material mat in AllMaterials)
		{
			if (mat.shader.name == shader)
			{
				Debug.LogError(mat.name);
			}
		}
	}

	public static List<Material> AllMaterials
	{
		get
		{
			return (from guid in AssetDatabase.FindAssets("t:Material") select AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid))).ToList();
		}
	}
}
