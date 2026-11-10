using System;
using System.IO;
using UnityEngine;

namespace BapakSimulator
{
    internal static class NewGameLaunch
    {
        internal sealed class Request
        {
            public string SceneSlot;
            public string RunSlot;
            public DifficultyPreset Preset;
            public bool IncludeHistory;
        }

        private static Request pending;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() => pending = null;

        private static string Preference(string sceneSlot) => $"continue.{sceneSlot}";

        public static string FindContinueSlot(string sceneSlot)
        {
            string active = PlayerPrefs.GetString(Preference(sceneSlot), sceneSlot);
            if (Exists(active)) return active;
            return active != sceneSlot && Exists(sceneSlot) ? sceneSlot : null;
        }

        private static bool Exists(string slot)
        {
            if (!Guid.TryParseExact(slot, "N", out var parsed)) return false;
            string file = CheckpointFile.GetPath(parsed.ToString("N"));
            return File.Exists(file) || File.Exists(file + ".bak");
        }

        public static void Set(Request request) => pending = request;

        public static Request Take(string sceneSlot)
        {
            if (pending == null || pending.SceneSlot != sceneSlot) return null;
            var request = pending;
            pending = null;
            return request;
        }

        public static void Remember(string sceneSlot, string runSlot)
        {
            if (PlayerPrefs.GetString(Preference(sceneSlot)) == runSlot) return;
            PlayerPrefs.SetString(Preference(sceneSlot), runSlot);
            PlayerPrefs.Save();
        }
    }
}
