using System.IO;
using UnityEngine;

namespace BapakSimulator
{
    internal static class CheckpointFile
    {
        internal static string GetPath(string slot) => Path.Combine(Application.persistentDataPath, "Saves", slot + ".json");

        public static void Write(string path, CheckpointData data, bool keepBackup)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string temporary = path + ".tmp";
            using (var stream = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (var writer = new StreamWriter(stream, new System.Text.UTF8Encoding(false), 1024, true))
                    writer.Write(JsonUtility.ToJson(data, true));
                stream.Flush(true);
            }

            if (File.Exists(path))
                File.Replace(temporary, path, keepBackup ? null : path + ".bak");
            else
                File.Move(temporary, path);
        }
    }
}
