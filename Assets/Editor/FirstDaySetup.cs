using BapakSimulator;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

public static class FirstDaySetup
{
    public const string SchedulePath = "Assets/Settings/first-day-schedule.asset";

    public static void AddToScene(PlayerController player)
    {
        var schedule = AssetDatabase.LoadAssetAtPath<DaySchedule>(SchedulePath);
        if (schedule == null)
            throw new System.InvalidOperationException($"Missing Day Schedule at {SchedulePath}.");
        var root = new GameObject("First Day");
        var day = root.AddComponent<PickupDay>();
        var progression = new SerializedObject(root.GetComponent<PlayerProgression>());
        progression.FindProperty("startingCash").intValue = 20000;
        progression.ApplyModifiedPropertiesWithoutUndo();
        var scenario = new SerializedObject(day);
        scenario.FindProperty("schedule").objectReferenceValue = schedule;
        scenario.ApplyModifiedPropertiesWithoutUndo();
        AddMarker(root.transform, "Rumah", new Vector3(-7.6f, -0.08f, -1.5f), 0, day.UseHome);
        AddMarker(root.transform, "Jemput", new Vector3(-4.8f, -0.08f, -4.4f), 180, day.PickUp);
        AddMarker(root.transform, "Kolam", new Vector3(3.6f, -0.08f, -4.4f), 180, day.Fish);

        var canvasObject = new GameObject("Day HUD", typeof(Canvas), typeof(UnityEngine.UI.CanvasScaler));
        canvasObject.transform.SetParent(root.transform, false);
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.GetComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        var panel = new GameObject("Status Background", typeof(UnityEngine.UI.Image));
        panel.transform.SetParent(canvasObject.transform, false);
        var image = panel.GetComponent<UnityEngine.UI.Image>();
        image.color = new Color(0.04f, 0.05f, 0.04f, 0.86f);
        image.raycastTarget = false;
        Place(image.rectTransform, Vector2.zero, Vector2.zero, new Vector2(18, 18), new Vector2(545, 125));
        var status = AddText(panel.transform, "Day Status", font, 18, TextAnchor.MiddleLeft);
        status.rectTransform.anchorMin = Vector2.zero;
        status.rectTransform.anchorMax = Vector2.one;
        status.rectTransform.offsetMin = new Vector2(14, 4);
        status.rectTransform.offsetMax = new Vector2(-14, -4);

        var prompt = AddText(canvasObject.transform, "Interaction Prompt", font, 24, TextAnchor.UpperCenter);
        Place(prompt.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 1),
            new Vector2(0, 14), new Vector2(450, 80));
        prompt.text = "·";
        canvasObject.AddComponent<FirstDayHud>().Configure(day, player, status, prompt);
    }

    public static void AddMarker(Transform parent, string title, Vector3 position, float yaw, UnityAction action)
    {
        var marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        marker.name = title;
        marker.transform.SetParent(parent, false);
        marker.transform.SetPositionAndRotation(position + Vector3.up * 0.78f, Quaternion.Euler(0, yaw, 0));
        marker.transform.localScale = new Vector3(0.75f, 1.56f, 0.45f);
        var target = marker.AddComponent<WorldInteractable>();
        var properties = new SerializedObject(target);
        properties.FindProperty("prompt").stringValue = title;
        properties.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddPersistentListener(target.OnInteract, action);

        var sign = new GameObject(title + " sign").AddComponent<TextMesh>();
        sign.transform.SetParent(parent, false);
        sign.transform.SetPositionAndRotation(
            marker.transform.position - marker.transform.forward * 0.24f + Vector3.up * 0.55f,
            marker.transform.rotation);
        sign.text = title;
        sign.anchor = TextAnchor.MiddleCenter;
        sign.characterSize = 0.12f;
        sign.fontSize = 48;
        sign.color = Color.black;
    }

    private static UnityEngine.UI.Text AddText(Transform parent, string name, Font font, int size, TextAnchor alignment)
    {
        var label = new GameObject(name, typeof(UnityEngine.UI.Text)).GetComponent<UnityEngine.UI.Text>();
        label.transform.SetParent(parent, false);
        label.font = font;
        label.fontSize = size;
        label.alignment = alignment;
        label.color = Color.white;
        label.raycastTarget = false;
        label.gameObject.AddComponent<UnityEngine.UI.Shadow>().effectDistance = new Vector2(1, -1);
        return label;
    }

    private static void Place(RectTransform rect, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
    {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }
}
