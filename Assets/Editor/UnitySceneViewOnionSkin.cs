using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[InitializeOnLoad]
public static class UnitySceneViewOnionSkin
{
    static UnitySceneViewOnionSkin()
    {
        SceneView.duringSceneGui += OnGUISceneViewe;
        alpha = EditorPrefs.GetFloat("alpha", 0.5f);
        clearDepth = EditorPrefs.GetBool("clearDepth", false);
        layer = EditorPrefs.GetInt("layer", -1);
        frame = EditorPrefs.GetInt("frame", 30);
        step = EditorPrefs.GetInt("step", 1);
    }

    static RenderTexture renderTexture = null;
    static float alpha = 0.5f;
    static bool clearDepth = false;
    static bool fold = true;
    static bool visible = true;
    static int layer = -1;
    static int frame = 30;
    static int step = 1;

    static void Capture(Camera camera)
    {
        var tmp = camera.activeTexture;
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(tmp.descriptor);
        }
        var clearFlags = camera.clearFlags;
        var mask = camera.cullingMask;
        camera.clearFlags = clearDepth ? CameraClearFlags.Depth : CameraClearFlags.SolidColor;
        camera.cullingMask = layer;
        camera.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        camera.targetTexture = renderTexture;
        camera.Render();
        camera.targetTexture = tmp;
        camera.clearFlags = clearFlags;
        camera.cullingMask = mask;
    }

    static void ContinueCapture(Camera camera, int frame, int step)
    {
        clearDepth = true;

        int i = 0;
        int frameCount = 0;
        EditorApplication.CallbackFunction update = null;
        update = () =>
        {
            if (i >= frame)
            {
                EditorApplication.update -= update;
                return;
            }
            if (frameCount != Time.frameCount)
            {
                if (i % step == 0)
                {
                    Debug.Log($"{Time.frameCount}");
                    Capture(camera);
                }
                frameCount = Time.frameCount;
                i++;
            }
        };

        EditorApplication.update += update;
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

        EditorGUILayout.BeginVertical(style, GUILayout.MinWidth(160), GUILayout.MinHeight(20));
        fold = EditorGUILayout.Foldout(fold, "OnionSkin", true);

        if (fold)
        {
            alpha = EditorGUILayout.Slider(alpha, 0.0f, 1.0f, GUILayout.Width(160));
            visible = EditorGUILayout.ToggleLeft("Visible", visible);
            clearDepth = EditorGUILayout.ToggleLeft("Clear Only Depth", clearDepth);
            layer = EditorGUILayout.MaskField(layer, InternalEditorUtility.layers, GUILayout.Width(160));

            EditorPrefs.SetFloat("alpha", alpha);
            EditorPrefs.SetBool("clearDepth", clearDepth);
            EditorPrefs.SetInt("layer", layer);

            if (GUILayout.Button("Capture", GUILayout.Width(120)))
            {
                Capture(sceneView.camera);
            }

            if (GUILayout.Button("Clear", GUILayout.Width(120)))
            {
                Object.DestroyImmediate(renderTexture);
                renderTexture = null;
            }

            frame = EditorGUILayout.IntField("Frame", frame);
            step = EditorGUILayout.IntField("Step", step);

            EditorPrefs.SetInt("frame", frame);
            EditorPrefs.SetInt("step", step);

            if (GUILayout.Button("Burst", GUILayout.Width(120)))
            {
                ContinueCapture(sceneView.camera, frame, step);
            }
        }

        EditorGUILayout.EndVertical();

        Handles.EndGUI();
    }
}
