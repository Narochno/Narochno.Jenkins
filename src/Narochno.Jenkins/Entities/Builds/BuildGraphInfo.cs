using Narochno.Jenkins.Entities.Users;
using Narochno.Primitives.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Narochno.Jenkins.Entities.Builds
{
    public class BuildGraphInfo 
    {
        public BuildGraphNode[] Nodes { get; set; }

        public static BuildGraphInfo Deserialize(string serializedBuildGraph, JsonSerializerSettings serializerSettings = null)
        {
            if (serializerSettings == null)
                serializerSettings = new JsonSerializerSettings
                {
                    Converters = new JsonConverter[] { new OptionalJsonConverter(), new StringEnumConverter() }
                };

            var deserialized = JsonConvert.DeserializeObject<DeserializedBuildGraph>(serializedBuildGraph, serializerSettings);
            var deserializedBuildGraphNodes = JsonConvert.DeserializeObject<DeserializedBuildGraphNodes>(deserialized.BuildGraph, serializerSettings);

            var result = new BuildGraphInfo();
            result.Nodes = deserializedBuildGraphNodes.Nodes.SelectMany(n => n.Nodes).ToArray();
            

            return result;
        }
    }

    class DeserializedBuildGraph
    {
        public string Class { get; set; }

        public string BuildGraph { get; set; }
    }

    class DeserializedBuildGraphNodes
    {
        public DeserializedBuildGraphNodesNodes[] Nodes { get; set; }
    }

    class DeserializedBuildGraphNodesNodes
    {
        public BuildGraphNode[] Nodes { get; set; }

    }

    public class BuildGraphNode
    {
        private string buildTitle = string.Empty;
        private string jobName = string.Empty;
        private string buildNumber = string.Empty;

        public string NodeId { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public string Title {
            get { return buildTitle; }
            set {
                buildTitle = value;
                var split = buildTitle.Split(' ');
                jobName = split[0];
                buildNumber = split[1].Substring(1);
            }
        }
        public string JobName
        {
            get { return jobName; }
        }
        public string BuildNumber
        {
            get { return buildNumber; }
        }

        public string Color { get; set; }
        public string BuildUrl { get; set; }
        public string Description { get; set; }
        public string Started { get; set; }
        public string Running { get; set; }
        public string Status { get; set; }
        public string Progress { get; set; }
        public DateTime StartTime { get; set; }
        public string Duration { get; set; }
        public string RrootUrl { get; set; }
        public string Clockpng { get; set; }
        public string Hourglasspng { get; set; }
        public string Terminalpng { get; set; }
        public string TimeStampString { get; set; }

    }
}