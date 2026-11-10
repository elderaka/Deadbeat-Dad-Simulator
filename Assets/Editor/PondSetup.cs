using System;
using BapakSimulator;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public static class PondSetup
{
    private const string ScenePath = "Assets/Scenes/kampung-foothill-blockout.unity";
    private const string MaterialPath = "Assets/Fishing/pond-water.mat";

    [MenuItem("Bapak Simulator/Install Physical Pond")]
    public static void Install()
    {
        if (EditorApplication.isPlaying)
            throw new InvalidOperationException("Stop Play Mode before installing the pond.");
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path != ScenePath) scene = EditorSceneManager.OpenScene(ScenePath);
        if (UnityEngine.Object.FindAnyObjectByType<PondWater>() != null)
        {
            Debug.Log("POND_ALREADY_INSTALLED");
            return;
        }

        var ground = GameObject.Find("Village ground");
        var oldWater = GameObject.Find("Pond water placeholder");
        var oldFish = UnityEngine.Object.FindAnyObjectByType<FishingFish>();
        var rod = UnityEngine.Object.FindAnyObjectByType<FishingRod>();
        var household = UnityEngine.Object.FindAnyObjectByType<PickupDay>();
        var catchItem = AssetDatabase.LoadAssetAtPath<ItemDefinition>("Assets/Economy/pond-fish.asset");
        var shader = Shader.Find("Bapak Simulator/Pond Surface");
        var fight = AssetDatabase.LoadAssetAtPath<FishingFightTuning>("Assets/Fishing/pond-fight.asset");
        if (ground == null || oldWater == null || oldFish == null || rod == null || household == null || catchItem == null || shader == null || fight == null)
            throw new InvalidOperationException("Missing the expected village pond, rod, fish, household, catch item, shader, or fight profile.");

        var terrain = ground.GetComponent<BoxCollider>().bounds;
        var footprint = oldWater.GetComponent<Renderer>().bounds;
        var soil = ground.GetComponent<Renderer>().sharedMaterial;
        float left = footprint.min.x, right = footprint.max.x;
        float far = footprint.min.z, near = footprint.max.z;
        const float bottom = -1.42f;
        const float surface = -0.23f;
        var parent = new GameObject("Physical Pond").transform;

        Box("Ground west", terrain.min.x, left, bottom, terrain.max.y, terrain.min.z, terrain.max.z, soil, parent);
        Box("Ground east", right, terrain.max.x, bottom, terrain.max.y, terrain.min.z, terrain.max.z, soil, parent);
        Box("Ground north", left, right, bottom, terrain.max.y, near, terrain.max.z, soil, parent);
        Box("Ground south", left, right, bottom, terrain.max.y, terrain.min.z, far, soil, parent);
        Box("Pond bed", left, right, bottom - .24f, bottom, far, near, soil, parent);
        Box("Shallow exit shelf", right - 1.35f, right, bottom, -.68f, near - 2.1f, near, soil, parent);

        var water = new GameObject("Pond Water");
        water.transform.SetParent(parent, false);
        var volume = water.AddComponent<BoxCollider>();
        volume.isTrigger = true;
        volume.center = new Vector3((left + right) * .5f, (bottom + surface) * .5f, (far + near) * .5f);
        volume.size = new Vector3(right - left, surface - bottom, near - far);
        var pond = water.AddComponent<PondWater>();
        var obstacle = water.AddComponent<NavMeshObstacle>();
        obstacle.shape = NavMeshObstacleShape.Box;
        obstacle.center = volume.center;
        obstacle.size = volume.size;
        obstacle.carving = true;
        obstacle.carveOnlyStationary = false;

        var paint = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
        if (paint == null)
        {
            paint = new Material(shader);
            AssetDatabase.CreateAsset(paint, MaterialPath);
        }

        var plane = Shape("Rippling surface", PrimitiveType.Plane,
            new Vector3(volume.center.x, surface, volume.center.z),
            new Vector3(volume.size.x / 10f, 1, volume.size.z / 10f), paint, water.transform);
        plane.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        var bright = AssetDatabase.LoadAssetAtPath<Material>("Assets/Fishing/float-red.mat");
        var gold = AssetDatabase.LoadAssetAtPath<Material>("Assets/Housing/Foothill/landmark.mat");
        AddFish("Pond fish A", new Vector3(left + 3, surface - .35f, near - 3), 0, bright, pond, fight, catchItem, parent);
        AddFish("Pond fish B", new Vector3(left + 7, surface - .52f, near - 5), 2.1f, gold, pond, fight, catchItem, parent);
        AddFish("Pond fish C", new Vector3(right - 3, surface - .68f, far + 3), 4.2f, bright, pond, fight, catchItem, parent);

        var bag = household.GetComponent<ItemInventory>() ?? household.gameObject.AddComponent<ItemInventory>();
        var rig = new SerializedObject(rod);
        rig.FindProperty("catchBag").objectReferenceValue = bag;
        rig.FindProperty("water").objectReferenceValue = pond;
        rig.FindProperty("attractionRadius").floatValue = 7f;
        rig.ApplyModifiedPropertiesWithoutUndo();

        UnityEngine.Object.DestroyImmediate(ground);
        UnityEngine.Object.DestroyImmediate(oldWater);
        UnityEngine.Object.DestroyImmediate(oldFish.gameObject);
        AssetDatabase.SaveAssets();
        if (!EditorSceneManager.SaveScene(scene)) throw new InvalidOperationException("Could not save village pond.");
        Debug.Log("POND_INSTALLED: recessed basin, transparent surface, three Rigidbody fish.");
    }

    private static void AddFish(string name, Vector3 position, float phase, Material paint, PondWater pond,
        FishingFightTuning fight, ItemDefinition catchItem, Transform parent)
    {
        var root = new GameObject(name);
        root.transform.SetParent(parent, false);
        root.transform.position = position;
        var body = root.AddComponent<Rigidbody>();
        body.mass = .55f;
        body.useGravity = true;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        var collider = root.AddComponent<CapsuleCollider>();
        collider.direction = 2;
        collider.radius = .09f;
        collider.height = .46f;

        Shape("Body", PrimitiveType.Sphere, position, new Vector3(.21f, .16f, .46f), paint, root.transform);
        Shape("Tail", PrimitiveType.Cube, position + Vector3.back * .23f,
            new Vector3(.07f, .12f, .14f), paint, root.transform).transform.localRotation = Quaternion.Euler(0, 30, 0);

        var fish = new SerializedObject(root.AddComponent<FishingFish>());
        fish.FindProperty("catchItem").objectReferenceValue = catchItem;
        fish.FindProperty("pond").objectReferenceValue = pond;
        fish.FindProperty("fightTuning").objectReferenceValue = fight;
        fish.FindProperty("swimSpeed").floatValue = 2.7f;
        fish.FindProperty("swimAcceleration").floatValue = 28f;
        fish.FindProperty("halfHeight").floatValue = .09f;
        fish.FindProperty("buoyancy").floatValue = 1.06f;
        fish.FindProperty("waterDrag").floatValue = 2f;
        fish.FindProperty("roamRadius").floatValue = 1.45f;
        fish.FindProperty("swimDepth").floatValue = pond.SurfaceY - position.y;
        fish.FindProperty("patrolRate").floatValue = .62f;
        fish.FindProperty("patrolPhase").floatValue = phase;
        fish.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void Box(string name, float x0, float x1, float y0, float y1, float z0, float z1,
        Material paint, Transform parent)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.position = new Vector3((x0 + x1) * .5f, (y0 + y1) * .5f, (z0 + z1) * .5f);
        obj.transform.localScale = new Vector3(x1 - x0, y1 - y0, z1 - z0);
        obj.GetComponent<Renderer>().sharedMaterial = paint;
    }

    private static GameObject Shape(string name, PrimitiveType primitive, Vector3 position, Vector3 scale,
        Material paint, Transform parent)
    {
        var obj = GameObject.CreatePrimitive(primitive);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = paint;
        UnityEngine.Object.DestroyImmediate(obj.GetComponent<Collider>());
        return obj;
    }
}
