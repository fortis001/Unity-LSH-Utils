using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace LSH.Utils
{
    public static class AudioLoader
    {
        public static async Task<AudioClip> LoadClip(string path)
        {
            string uri = path.StartsWith("http") ? path : "file://" + path;

            // 확장자에 따라 AudioType 자동 결정
            AudioType audioType = Path.GetExtension(path).ToLower() switch
            {
                ".ogg" => AudioType.OGGVORBIS,
                ".wav" => AudioType.WAV,
                ".mp3" => AudioType.MPEG,
                _ => AudioType.UNKNOWN
            };

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, audioType))
            {
                var operation = www.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    return DownloadHandlerAudioClip.GetContent(www);
                }
                else
                {
                    Debug.LogError($"[AudioLoader] Load Failed: {path} | Error: {www.error}");
                    return null;
                }
            }
        }
    }
}

