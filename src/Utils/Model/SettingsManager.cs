using OpenAI.Files;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.IO;
using Floai.ApiClients.abs;

namespace Floai.Utils.Model
{
    public class SettingsManager
    {
        // Configuration file path.
        private readonly string filePath;
        public SettingsManager(string filePath) 
        {
            this.filePath = filePath;
        }

        /// <summary>
        /// Saves the provided settings object to the specified nodePath in the configuration file.
        /// </summary>
        /// <param name="obj">The object to be saved.</param>
        /// <param name="nodePath">The path to the node where the settings should be saved, using forward slashes (/) to indicate nested nodes.</param>
        public void SaveNode(object obj, string nodePath)
        {
            var json = File.ReadAllText(filePath);
            var rootNode = JsonNode.Parse(json);

            string[] nodeNames = nodePath.Split('/');
            JsonNode currentNode = rootNode;

            // Traverse the path to create any missing nodes.
            for (int i = 0; i < nodeNames.Length - 1; i++)
            {
                if (currentNode[nodeNames[i]] == null)
                {
                    currentNode[nodeNames[i]] = new JsonObject();
                }
                currentNode = currentNode[nodeNames[i]];
            }

            // Serialize and save the settings object to the specified node.
            currentNode[nodeNames[^1]] = JsonSerializer.SerializeToNode(obj);
            File.WriteAllText(filePath, rootNode.ToString());
        }

        /// <summary>
        /// Reads the value of the specified nodePath from the configuration file and returns it as a JSON-formatted string.
        /// </summary>
        /// <param name="obj">The object representing the configuration settings.</param>
        /// <param name="nodePath">The path to the node whose value needs to be retrieved, using forward slashes (/) to indicate nested nodes.</param>
        /// <returns>A JSON-formatted string representing the value of the specified node.</returns>
        public string ReadNode(object obj, string nodePath)
        {
            string[] nodeNames = nodePath.Split('/');
            var json = File.ReadAllText(filePath);
            var rootNode = JsonNode.Parse(json);
            var jsonNode = rootNode;
            foreach (var name in nodeNames)
            {
                jsonNode = jsonNode[name];
            }
            jsonNode = JsonSerializer.SerializeToNode(obj);
            return jsonNode.ToJsonString(new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        public string ReadApiClientOptionsNode(string apiClientName)
        {
            var json = File.ReadAllText(filePath);
            var rootNode = JsonNode.Parse(json);
            var nodeName = apiClientName.Replace("ApiClientOptions", "");
            var jsonNode = rootNode["apiClientOptions"][nodeName];
            return jsonNode.ToJsonString(new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}
