using OpenAI.Files;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.IO;
using Floai.ApiClients.abs;

namespace Floai.Utils.Model
{
    public class SettingsManager
    {
        private readonly string filePath;
        public SettingsManager(string filePath) 
        {
            this.filePath = filePath;
        }

        public void SaveNode(object settings, string nodePath)
        {
            var json = File.ReadAllText(filePath);
            var rootNode = JsonNode.Parse(json);

            string[] nodeNames = nodePath.Split('/');
            JsonNode currentNode = rootNode;

            for (int i = 0; i < nodeNames.Length - 1; i++)
            {
                if (currentNode[nodeNames[i]] == null)
                {
                    currentNode[nodeNames[i]] = new JsonObject();
                }
                currentNode = currentNode[nodeNames[i]];
            }
            currentNode[nodeNames[^1]] = JsonSerializer.SerializeToNode(settings);

            File.WriteAllText(filePath, rootNode.ToString());
        }


        public string ReadNode(object settings, string nodePath)
        {
            string[] nodeNames = nodePath.Split('/');
            var json = File.ReadAllText(filePath);
            var rootNode = JsonNode.Parse(json);
            var jsonNode = rootNode;
            foreach (var name in nodeNames)
            {
                jsonNode = jsonNode[name];
            }
            jsonNode = JsonSerializer.SerializeToNode(settings);
            return jsonNode.ToJsonString(new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        public string ReadApiClientOptionsNode(object options, string apiClientName)
        {
            var json = File.ReadAllText(filePath);
            var rootNode = JsonNode.Parse(json);
            var nodeName = apiClientName.Replace("ApiClientOptions", "");
            var jsonNode = rootNode["apiClientOptions"][nodeName];
            jsonNode = JsonSerializer.SerializeToNode(jsonNode, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            return jsonNode.ToJsonString(new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}
