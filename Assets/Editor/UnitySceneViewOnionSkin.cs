using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class UnitySceneViewOnionSkin
{
	static UnitySceneViewOnionSkin()
	{
		SceneView.duringSceneGui += OnGUISceneViewe;
	}

	static RenderTexture renderTexture = null;

	static void Capture(Camera camera)
	{
		var tmp = camera.activeTexture;
		if (renderTexture == null)
		{
			var desc = camera.activeTexture.descriptor;
			renderTexture = new RenderTexture(desc);
		}

		SceneView.lastActiveSceneView.camera.targetTexture = renderTexture;
		SceneView.lastActiveSceneView.camera.Render();
		SceneView.lastActiveSceneView.camera.targetTexture = tmp;
	}

	static void OnGUISceneViewe(SceneView sceneView)
	{
		Handles.BeginGUI();

		var color = GUI.color;

		if (renderTexture != null)
		{
			GUI.color = new Color(1, 1, 1, 0.5f);
			GUI.DrawTexture(new Rect(0, 1, renderTexture.width, renderTexture.height), renderTexture);
		}

		if (GUILayout.Button("Capture", GUILayout.Width(120)))
		{
			Capture(sceneView.camera);
		}

		if (GUILayout.Button("Clear", GUILayout.Width(120)))
		{
			Object.DestroyImmediate(renderTexture);
			renderTexture = null;
		}

		Handles.EndGUI();
	}
}
