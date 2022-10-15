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
    static bool clearDepth = false;
    static bool fold = true;
    static bool visible = true;

    static void Capture(Camera camera)
    {
        var tmp = camera.activeTexture;
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(tmp.descriptor);
        }
        var clearFlags = camera.clearFlags;
        camera.clearFlags = clearDepth ? CameraClearFlags.Depth : CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        camera.targetTexture = renderTexture;
        camera.Render();
        camera.targetTexture = tmp;
        camera.clearFlags = clearFlags;
    }

    static void OnGUISceneViewe(SceneView sceneView)
    {
        Handles.BeginGUI();

        var color = GUI.color;

        if (renderTexture != null && visible)
        {
            GUI.color = new Color(1, 1, 1, alpha);
            GUI.DrawTexture(new Rect(0, 1, renderTexture.width, renderTexture.height), renderTexture);
            GUI.color = color;
        }
        var style = GUI.skin.window;
        style.padding.top = style.padding.bottom;
        style.margin.left = 10;
        EditorGUILayout.BeginVertical(style, GUILayout.MinWidth(120), GUILayout.MinHeight(20));
        fold = EditorGUILayout.Foldout(fold, "OnionSkin", true);

        if (fold)
        {

            alpha = EditorGUILayout.Slider(alpha, 0.0f, 1.0f, GUILayout.Width(160));

            visible = EditorGUILayout.ToggleLeft("Visible", visible);
            clearDepth = EditorGUILayout.ToggleLeft("Clear Only Depth", clearDepth);

            if (GUILayout.Button("Capture", GUILayout.Width(120)))
            {
                Capture(sceneView.camera);
            }

            if (GUILayout.Button("Clear", GUILayout.Width(120)))
            {
                Object.DestroyImmediate(renderTexture);
                renderTexture = null;
            }
        }

        EditorGUILayout.EndVertical();

        Handles.EndGUI();
    }
}
