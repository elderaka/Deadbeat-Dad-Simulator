using System;
using BapakSimulator;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class BlockoutSetup
{
    public const string ScenePath = "Assets/Scenes/kontrakan-blockout.unity";

    public static void Initialize()
    {
        Create();
        BuildWindows();
    }

    [MenuItem("Bapak Simulator/Create Walkable Blockout")]
    public static void Create()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        var importer = (ModelImporter)AssetImporter.GetAtPath("Assets/Housing/kontrakan-row-5-v01.fbx");
        importer.globalScale = 1;
        importer.useFileScale = false;
        importer.SaveAndReimport();
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var asset = AssetDatabase.LoadAssetAtPath<GameObject>(importer.assetPath);
        var compound = (GameObject)PrefabUtility.InstantiatePrefab(asset);
        compound.name = "Kontrakan - provisional five-unit blockout";
        var bounds = new Bounds();
        bool first = true;
        foreach (var mesh in compound.GetComponentsInChildren<MeshFilter>())
        {
            if (mesh.sharedMesh == null) continue;
            mesh.gameObject.AddComponent<MeshCollider>().sharedMesh = mesh.sharedMesh;
            var renderer = mesh.GetComponent<Renderer>();
            if (mesh.name.Contains("U01") && mesh.name.Contains("Floor")) Debug.Log("IMPORT_COORDINATES " + mesh.name + ": " + renderer.bounds);
            if (first) { bounds = renderer.bounds; first = false; }
            else bounds.Encapsulate(renderer.bounds);
        }
        if (bounds.size.x < 18 || bounds.size.x > 21) throw new Exception("Unexpected FBX scale: " + bounds.size);
        var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Safety ground";
        ground.transform.position = new Vector3(0, -0.2f, 2);
        ground.transform.localScale = new Vector3(28, 0.2f, 27);
        var light = new GameObject("Sun").AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 2;
        light.transform.rotation = Quaternion.Euler(50, -30, 0);
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.6f, 0.65f, 0.7f);
        var player = new GameObject("Bapak");
        player.transform.position = new Vector3(-7, 0.3f, -3);
        var controller = player.AddComponent<CharacterController>();
        controller.height = 1.65f;
        controller.center = Vector3.up * 0.825f;
        controller.radius = 0.22f;
        controller.skinWidth = 0.02f;
        controller.stepOffset = 0.2f;
        var camera = new GameObject("First-person camera").AddComponent<Camera>();
        camera.tag = "MainCamera";
        camera.transform.SetParent(player.transform, false);
        camera.transform.localPosition = Vector3.up * 1.55f;
        camera.nearClipPlane = 0.05f;
        camera.farClipPlane = 100;
        camera.fieldOfView = 70;
        camera.backgroundColor = new Color(0.55f, 0.72f, 0.84f);
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.gameObject.AddComponent<AudioListener>();
        player.AddComponent<FirstPersonWalker>().Configure(camera);
        PlayerSettings.companyName = "elderaka";
        PlayerSettings.productName = "Deadbeat Dad Simulator";
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.elderaka.deadbeatdadsimulator");
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        Debug.Log("BLOCKOUT_CREATED: " + bounds.size);
    }

    [MenuItem("Bapak Simulator/Validate Blockout")]
    public static void Validate()
    {
        EditorSceneManager.OpenScene(ScenePath);
        Physics.SyncTransforms();
        // ponytail: physics smoke check for all five units; device feel still needs a hands-on playtest.
        for (int unit = 0; unit < 5; unit++)
        {
            float x = -7 + unit * 3.5f;
            foreach (var door in new[] { new Vector2(x, 0), new Vector2(x + 0.76f, 3), new Vector2(x - 0.055f, 6) })
            {
                if (Physics.SphereCast(new Vector3(door.x, 0.9f, door.y - 0.5f), 0.23f, Vector3.forward, out var hit, 1))
                    throw new Exception("Blocked doorway at " + door + ": " + hit.collider.name);
            }
            if (!Physics.Raycast(new Vector3(x, 1, 1), Vector3.right, out _, 2))
                throw new Exception("Missing wall collider in unit " + unit);
            if (!Physics.Raycast(new Vector3(x, 1, 1), Vector3.down, out _, 2))
                throw new Exception("Missing floor collider in unit " + unit);
        }
        if (UnityEngine.Object.FindAnyObjectByType<CharacterController>().height != 1.65f)
            throw new Exception("Incorrect character scale");
        var probe = new GameObject("Movement test probe");
        try
        {
            probe.transform.position = new Vector3(0, 4, -4);
            probe.AddComponent<CharacterController>();
            var camera = new GameObject("Test camera").AddComponent<Camera>();
            camera.transform.SetParent(probe.transform, false);
            var walker = probe.AddComponent<FirstPersonWalker>();
            walker.Configure(camera);
            walker.SendMessage("Awake");
            var before = probe.transform.position;
            walker.Step(Vector2.one, new Vector2(0, 1000), 0.1f);
            var displacement = probe.transform.position - before;
            if (Mathf.Abs(new Vector2(displacement.x, displacement.z).magnitude - 0.25f) > 0.005f)
                throw new Exception("Diagonal movement exceeds walking speed");
            if (Quaternion.Angle(camera.transform.localRotation, Quaternion.Euler(-80, 0, 0)) > 0.01f)
                throw new Exception("Camera pitch limit failed");
        }
        finally { UnityEngine.Object.DestroyImmediate(probe); }
        Debug.Log("BLOCKOUT_VALIDATION_PASSED: fifteen doorways, five walls, five floors, character scale, movement normalization and camera pitch.");
    }

    public static void BuildWindows()
    {
        Validate();
        var report = BuildPipeline.BuildPlayer(new[] { ScenePath }, "Builds/Windows/Deadbeat-Dad-Simulator.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            throw new Exception("Windows build failed: " + report.summary.result);
        Debug.Log("WINDOWS_BUILD_PASSED");
    }
}
