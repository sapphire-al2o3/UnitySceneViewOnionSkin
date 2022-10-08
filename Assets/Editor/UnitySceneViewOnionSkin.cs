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
    static float alpha = 0.5f;

    static void Capture(Camera camera)
    {
        var tmp = camera.activeTexture;
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(tmp.descriptor);
        }
        var clearFlags = camera.clearFlags;
        camera.clearFlags = CameraClearFlags.Depth;
        camera.targetTexture = renderTexture;
        camera.Render();
        camera.targetTexture = tmp;
        camera.clearFlags = clearFlags;
    }

    static void OnGUISceneViewe(SceneView sceneView)
    {
        Handles.BeginGUI();

        var color = GUI.color;

        if (renderTexture != null)
        {
            GUI.color = new Color(1, 1, 1, alpha);
            GUI.DrawTexture(new Rect(0, 1, renderTexture.width, renderTexture.height), renderTexture);
            GUI.color = color;
        }

        alpha = EditorGUILayout.Slider(alpha, 0.0f, 1.0f, GUILayout.Width(120));

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
