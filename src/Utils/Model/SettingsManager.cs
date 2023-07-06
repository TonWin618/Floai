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
            string[] nodeNames = nodePath.Split('/');
            var json = File.ReadAllText(filePath);
            var jsonNode = JsonNode.Parse(json);
            foreach (var name in nodeNames)
            {
                jsonNode = jsonNode[name];
            }
            jsonNode = JsonSerializer.SerializeToNode(settings);
            File.WriteAllText(filePath, jsonNode.ToString());
        }
    }
}
