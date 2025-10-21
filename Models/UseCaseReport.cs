using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestcaseGenerator.Models
{
    public class UseCaseReport
    {
        [JsonPropertyName("ucId")]
        public string UcId { get; set; } = "";
        
        [JsonPropertyName("ucName")]
        public string UcName { get; set; } = "";
        
        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; } = "";
        
        [JsonPropertyName("dateCreated")]
        public string DateCreated { get; set; } = "";
        
        [JsonPropertyName("primaryActor")]
        public string PrimaryActor { get; set; } = "";
        
        [JsonPropertyName("secondaryActors")]
        [JsonConverter(typeof(SecondaryActorsConverter))]
        public string SecondaryActors { get; set; } = "";
        
        [JsonPropertyName("trigger")]
        public string Trigger { get; set; } = "";
        
        [JsonPropertyName("description")]
        public string Description { get; set; } = "";
        
        [JsonPropertyName("preconditions")]
        public List<string> Preconditions { get; set; } = new();
        
        [JsonPropertyName("postconditions")]
        public List<string> Postconditions { get; set; } = new();
        
        [JsonPropertyName("normalFlow")]
        public List<FlowStep> NormalFlow { get; set; } = new();
        
        [JsonPropertyName("alternativeFlows")]
        public List<AlternativeFlow> AlternativeFlows { get; set; } = new();
        
        [JsonPropertyName("exceptions")]
        public List<ExceptionFlow> Exceptions { get; set; } = new();
        
        [JsonPropertyName("priority")]
        public string Priority { get; set; } = "";
        
        [JsonPropertyName("frequencyOfUse")]
        public string FrequencyOfUse { get; set; } = "";
        
        [JsonPropertyName("businessRules")]
        public string BusinessRules { get; set; } = "";
        
        [JsonPropertyName("otherInformation")]
        public List<string> OtherInformation { get; set; } = new();
        
        [JsonPropertyName("assumptions")]
        public List<string> Assumptions { get; set; } = new();
    }

    public class FlowStep
    {
        [JsonPropertyName("step")]
        public string Step { get; set; } = "";
        
        [JsonPropertyName("description")]
        public string Description { get; set; } = "";
    }

    public class AlternativeFlow
    {
        [JsonPropertyName("flowId")]
        public string FlowId { get; set; } = "";
        
        [JsonPropertyName("flowName")]
        public string FlowName { get; set; } = "";
        
        [JsonPropertyName("steps")]
        public List<FlowStep> Steps { get; set; } = new();
    }

    public class ExceptionFlow
    {
        [JsonPropertyName("exceptionId")]
        public string ExceptionId { get; set; } = "";
        
        [JsonPropertyName("exceptionName")]
        public string ExceptionName { get; set; } = "";
        
        [JsonPropertyName("descriptions")]
        public List<string> Descriptions { get; set; } = new();
    }

    public class SecondaryActorsConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString() ?? "";
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                var list = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                return string.Join(", ", list ?? new List<string>());
            }
            return "";
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
