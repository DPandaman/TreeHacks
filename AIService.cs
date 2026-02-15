using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using System;

public class AIService : MonoBehaviour
{
    [Header("API Settings")]
    //openAI api key 
    public string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY"); 
    public string apiUrl = "https://api.openai.com/v1/chat/completions";

    public void SendPrompt(string systemPrompt, string userPrompt, Action<string> callback)
    {
        StartCoroutine(PostRequest(systemPrompt, userPrompt, callback));
    }

    IEnumerator PostRequest(string systemRole, string userMessage, Action<string> callback)
    {
        // Simple JSON construction for OpenAI Chat format
        string json = $@"
        {{
            ""model"": ""gpt-4o"",
            ""messages"": [
                {{ ""role"": ""system"", ""content"": ""{systemRole}"" }},
                {{ ""role"": ""user"", ""content"": ""{userMessage}"" }}
            ]
        }}";

        var request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // You'll need a tiny JSON parser here or use a library
            // For hackathon speed, just pass the raw text if you're lazy, 
            // but ideally parse 'choices[0].message.content'
            callback(request.downloadHandler.text); 
        }
        else
        {
            Debug.LogError("AI Error: " + request.error);
        }
    }
}