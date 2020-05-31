
// Microsoft.VisualStudio.LanguageServices.Razor.Serialization.TagHelperDescriptorJsonConverter
using Microsoft.AspNetCore.Razor.Language;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

internal class TagHelperDescriptorJsonConverter : JsonConverter
{
	public static readonly TagHelperDescriptorJsonConverter Instance = new TagHelperDescriptorJsonConverter();

	public override bool CanConvert(Type objectType)
	{
		return typeof(TagHelperDescriptor).IsAssignableFrom(objectType);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		if (reader.TokenType != JsonToken.StartObject)
		{
			return null;
		}
		JObject jObject = JObject.Load(reader);
		string kind = jObject["Kind"].Value<string>();
		string name = jObject["Name"].Value<string>();
		string assemblyName = jObject["AssemblyName"].Value<string>();
		JArray jArray = jObject["TagMatchingRules"].Value<JArray>();
		JArray jArray2 = jObject["BoundAttributes"].Value<JArray>();
		JArray jArray3 = jObject["AllowedChildTags"].Value<JArray>();
		string documentation = jObject["Documentation"].Value<string>();
		string tagOutputHint = jObject["TagOutputHint"].Value<string>();
		bool caseSensitive = jObject["CaseSensitive"].Value<bool>();
		JArray jArray4 = jObject["Diagnostics"].Value<JArray>();
		JObject jObject2 = jObject["Metadata"].Value<JObject>();
		TagHelperDescriptorBuilder tagHelperDescriptorBuilder = TagHelperDescriptorBuilder.Create(kind, name, assemblyName);
		tagHelperDescriptorBuilder.Documentation = documentation;
		tagHelperDescriptorBuilder.TagOutputHint = tagOutputHint;
		tagHelperDescriptorBuilder.CaseSensitive = caseSensitive;
		foreach (JToken item2 in jArray)
		{
			JObject rule = item2.Value<JObject>();
			tagHelperDescriptorBuilder.TagMatchingRule(delegate (TagMatchingRuleDescriptorBuilder b)
			{
				ReadTagMatchingRule(b, rule, serializer);
			});
		}
		foreach (JToken item3 in jArray2)
		{
			JObject attribute = item3.Value<JObject>();
			tagHelperDescriptorBuilder.BindAttribute(delegate (BoundAttributeDescriptorBuilder b)
			{
				ReadBoundAttribute(b, attribute, serializer);
			});
		}
		foreach (JToken item4 in jArray3)
		{
			JObject tag = item4.Value<JObject>();
			tagHelperDescriptorBuilder.AllowChildTag(delegate (AllowedChildTagDescriptorBuilder childTagBuilder)
			{
				ReadAllowedChildTag(childTagBuilder, tag, serializer);
			});
		}
		foreach (JToken item5 in jArray4)
		{
			JsonReader reader2 = item5.CreateReader();
			RazorDiagnostic item = serializer.Deserialize<RazorDiagnostic>(reader2);
			tagHelperDescriptorBuilder.Diagnostics.Add(item);
		}
		JsonReader reader3 = jObject2.CreateReader();
		foreach (KeyValuePair<string, string> item6 in serializer.Deserialize<Dictionary<string, string>>(reader3))
		{
			tagHelperDescriptorBuilder.Metadata[item6.Key] = item6.Value;
		}
		return tagHelperDescriptorBuilder.Build();
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		TagHelperDescriptor tagHelperDescriptor = (TagHelperDescriptor)value;
		writer.WriteStartObject();
		writer.WritePropertyName("Kind");
		writer.WriteValue(tagHelperDescriptor.Kind);
		writer.WritePropertyName("Name");
		writer.WriteValue(tagHelperDescriptor.Name);
		writer.WritePropertyName("AssemblyName");
		writer.WriteValue(tagHelperDescriptor.AssemblyName);
		writer.WritePropertyName("Documentation");
		writer.WriteValue(tagHelperDescriptor.Documentation);
		writer.WritePropertyName("TagOutputHint");
		writer.WriteValue(tagHelperDescriptor.TagOutputHint);
		writer.WritePropertyName("CaseSensitive");
		writer.WriteValue(tagHelperDescriptor.CaseSensitive);
		writer.WritePropertyName("TagMatchingRules");
		writer.WriteStartArray();
		foreach (TagMatchingRuleDescriptor tagMatchingRule in tagHelperDescriptor.TagMatchingRules)
		{
			WriteTagMatchingRule(writer, tagMatchingRule, serializer);
		}
		writer.WriteEndArray();
		writer.WritePropertyName("BoundAttributes");
		writer.WriteStartArray();
		foreach (BoundAttributeDescriptor boundAttribute in tagHelperDescriptor.BoundAttributes)
		{
			WriteBoundAttribute(writer, boundAttribute, serializer);
		}
		writer.WriteEndArray();
		writer.WritePropertyName("AllowedChildTags");
		writer.WriteStartArray();
		foreach (AllowedChildTagDescriptor allowedChildTag in tagHelperDescriptor.AllowedChildTags)
		{
			WriteAllowedChildTags(writer, allowedChildTag, serializer);
		}
		writer.WriteEndArray();
		writer.WritePropertyName("Diagnostics");
		serializer.Serialize(writer, tagHelperDescriptor.Diagnostics);
		writer.WritePropertyName("Metadata");
		WriteMetadata(writer, tagHelperDescriptor.Metadata);
		writer.WriteEndObject();
	}

	private static void WriteAllowedChildTags(JsonWriter writer, AllowedChildTagDescriptor allowedChildTag, JsonSerializer serializer)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("Name");
		writer.WriteValue(allowedChildTag.Name);
		writer.WritePropertyName("DisplayName");
		writer.WriteValue(allowedChildTag.DisplayName);
		writer.WritePropertyName("Diagnostics");
		serializer.Serialize(writer, allowedChildTag.Diagnostics);
		writer.WriteEndObject();
	}

	private static void WriteBoundAttribute(JsonWriter writer, BoundAttributeDescriptor boundAttribute, JsonSerializer serializer)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("Kind");
		writer.WriteValue(boundAttribute.Kind);
		writer.WritePropertyName("Name");
		writer.WriteValue(boundAttribute.Name);
		writer.WritePropertyName("TypeName");
		writer.WriteValue(boundAttribute.TypeName);
		writer.WritePropertyName("IsEnum");
		writer.WriteValue(boundAttribute.IsEnum);
		writer.WritePropertyName("IndexerNamePrefix");
		writer.WriteValue(boundAttribute.IndexerNamePrefix);
		writer.WritePropertyName("IndexerTypeName");
		writer.WriteValue(boundAttribute.IndexerTypeName);
		writer.WritePropertyName("Documentation");
		writer.WriteValue(boundAttribute.Documentation);
		writer.WritePropertyName("Diagnostics");
		serializer.Serialize(writer, boundAttribute.Diagnostics);
		writer.WritePropertyName("Metadata");
		WriteMetadata(writer, boundAttribute.Metadata);
		writer.WritePropertyName("BoundAttributeParameters");
		writer.WriteStartArray();
		foreach (BoundAttributeParameterDescriptor boundAttributeParameter in boundAttribute.BoundAttributeParameters)
		{
			WriteBoundAttributeParameter(writer, boundAttributeParameter, serializer);
		}
		writer.WriteEndArray();
		writer.WriteEndObject();
	}

	private static void WriteBoundAttributeParameter(JsonWriter writer, BoundAttributeParameterDescriptor boundAttributeParameter, JsonSerializer serializer)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("Kind");
		writer.WriteValue(boundAttributeParameter.Kind);
		writer.WritePropertyName("Name");
		writer.WriteValue(boundAttributeParameter.Name);
		writer.WritePropertyName("TypeName");
		writer.WriteValue(boundAttributeParameter.TypeName);
		writer.WritePropertyName("IsEnum");
		writer.WriteValue(boundAttributeParameter.IsEnum);
		writer.WritePropertyName("Documentation");
		writer.WriteValue(boundAttributeParameter.Documentation);
		writer.WritePropertyName("Diagnostics");
		serializer.Serialize(writer, boundAttributeParameter.Diagnostics);
		writer.WritePropertyName("Metadata");
		WriteMetadata(writer, boundAttributeParameter.Metadata);
		writer.WriteEndObject();
	}

	private static void WriteMetadata(JsonWriter writer, IReadOnlyDictionary<string, string> metadata)
	{
		writer.WriteStartObject();
		foreach (KeyValuePair<string, string> metadatum in metadata)
		{
			writer.WritePropertyName(metadatum.Key);
			writer.WriteValue(metadatum.Value);
		}
		writer.WriteEndObject();
	}

	private static void WriteTagMatchingRule(JsonWriter writer, TagMatchingRuleDescriptor ruleDescriptor, JsonSerializer serializer)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("TagName");
		writer.WriteValue(ruleDescriptor.TagName);
		writer.WritePropertyName("ParentTag");
		writer.WriteValue(ruleDescriptor.ParentTag);
		writer.WritePropertyName("TagStructure");
		writer.WriteValue(ruleDescriptor.TagStructure);
		writer.WritePropertyName("Attributes");
		writer.WriteStartArray();
		foreach (RequiredAttributeDescriptor attribute in ruleDescriptor.Attributes)
		{
			WriteRequiredAttribute(writer, attribute, serializer);
		}
		writer.WriteEndArray();
		writer.WritePropertyName("Diagnostics");
		serializer.Serialize(writer, ruleDescriptor.Diagnostics);
		writer.WriteEndObject();
	}

	private static void WriteRequiredAttribute(JsonWriter writer, RequiredAttributeDescriptor requiredAttribute, JsonSerializer serializer)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("Name");
		writer.WriteValue(requiredAttribute.Name);
		writer.WritePropertyName("NameComparison");
		writer.WriteValue(requiredAttribute.NameComparison);
		writer.WritePropertyName("Value");
		writer.WriteValue(requiredAttribute.Value);
		writer.WritePropertyName("ValueComparison");
		writer.WriteValue(requiredAttribute.ValueComparison);
		writer.WritePropertyName("Diagnostics");
		serializer.Serialize(writer, requiredAttribute.Diagnostics);
		writer.WritePropertyName("Metadata");
		WriteMetadata(writer, requiredAttribute.Metadata);
		writer.WriteEndObject();
	}

	private static void ReadTagMatchingRule(TagMatchingRuleDescriptorBuilder builder, JObject rule, JsonSerializer serializer)
	{
		string tagName = rule["TagName"].Value<string>();
		JArray jArray = rule["Attributes"].Value<JArray>();
		string parentTag = rule["ParentTag"].Value<string>();
		int tagStructure = rule["TagStructure"].Value<int>();
		JArray jArray2 = rule["Diagnostics"].Value<JArray>();
		builder.TagName = tagName;
		builder.ParentTag = parentTag;
		builder.TagStructure = (TagStructure)tagStructure;
		foreach (JToken item2 in jArray)
		{
			JObject attibuteValue = item2.Value<JObject>();
			builder.Attribute(delegate (RequiredAttributeDescriptorBuilder b)
			{
				ReadRequiredAttribute(b, attibuteValue, serializer);
			});
		}
		foreach (JToken item3 in jArray2)
		{
			JsonReader reader = item3.CreateReader();
			RazorDiagnostic item = serializer.Deserialize<RazorDiagnostic>(reader);
			builder.Diagnostics.Add(item);
		}
	}

	private static void ReadRequiredAttribute(RequiredAttributeDescriptorBuilder builder, JObject attribute, JsonSerializer serializer)
	{
		string name = attribute["Name"].Value<string>();
		int nameComparisonMode = attribute["NameComparison"].Value<int>();
		string value = attribute["Value"].Value<string>();
		int valueComparisonMode = attribute["ValueComparison"].Value<int>();
		JArray jArray = attribute["Diagnostics"].Value<JArray>();
		JObject jObject = attribute["Metadata"].Value<JObject>();
		builder.Name = name;
		builder.NameComparisonMode = (RequiredAttributeDescriptor.NameComparisonMode)nameComparisonMode;
		builder.Value = value;
		builder.ValueComparisonMode = (RequiredAttributeDescriptor.ValueComparisonMode)valueComparisonMode;
		foreach (JToken item2 in jArray)
		{
			JsonReader reader = item2.CreateReader();
			RazorDiagnostic item = serializer.Deserialize<RazorDiagnostic>(reader);
			builder.Diagnostics.Add(item);
		}
		JsonReader reader2 = jObject.CreateReader();
		foreach (KeyValuePair<string, string> item3 in serializer.Deserialize<Dictionary<string, string>>(reader2))
		{
			builder.Metadata[item3.Key] = item3.Value;
		}
	}

	private static void ReadAllowedChildTag(AllowedChildTagDescriptorBuilder builder, JObject childTag, JsonSerializer serializer)
	{
		string name = childTag["Name"].Value<string>();
		string displayName = childTag["DisplayName"].Value<string>();
		JArray jArray = childTag["Diagnostics"].Value<JArray>();
		builder.Name = name;
		builder.DisplayName = displayName;
		foreach (JToken item2 in jArray)
		{
			JsonReader reader = item2.CreateReader();
			RazorDiagnostic item = serializer.Deserialize<RazorDiagnostic>(reader);
			builder.Diagnostics.Add(item);
		}
	}

	private static void ReadBoundAttribute(BoundAttributeDescriptorBuilder builder, JObject attribute, JsonSerializer serializer)
	{
		attribute["Kind"].Value<string>();
		string name = attribute["Name"].Value<string>();
		string typeName = attribute["TypeName"].Value<string>();
		bool flag = attribute["IsEnum"].Value<bool>();
		string text = attribute["IndexerNamePrefix"].Value<string>();
		string valueTypeName = attribute["IndexerTypeName"].Value<string>();
		string documentation = attribute["Documentation"].Value<string>();
		JArray jArray = attribute["Diagnostics"].Value<JArray>();
		JObject jObject = attribute["Metadata"].Value<JObject>();
		JArray jArray2 = attribute["BoundAttributeParameters"].Value<JArray>();
		builder.Name = name;
		builder.TypeName = typeName;
		builder.Documentation = documentation;
		if (text != null)
		{
			builder.AsDictionary(text, valueTypeName);
		}
		if (flag)
		{
			builder.IsEnum = true;
		}
		foreach (JToken item2 in jArray)
		{
			JsonReader reader = item2.CreateReader();
			RazorDiagnostic item = serializer.Deserialize<RazorDiagnostic>(reader);
			builder.Diagnostics.Add(item);
		}
		JsonReader reader2 = jObject.CreateReader();
		foreach (KeyValuePair<string, string> item3 in serializer.Deserialize<Dictionary<string, string>>(reader2))
		{
			builder.Metadata[item3.Key] = item3.Value;
		}
		foreach (JToken item4 in jArray2)
		{
			JObject parameter = item4.Value<JObject>();
			builder.BindAttributeParameter(delegate (BoundAttributeParameterDescriptorBuilder b)
			{
				ReadBoundAttributeParameter(b, parameter, serializer);
			});
		}
	}

	private static void ReadBoundAttributeParameter(BoundAttributeParameterDescriptorBuilder builder, JObject parameter, JsonSerializer serializer)
	{
		parameter["Kind"].Value<string>();
		string name = parameter["Name"].Value<string>();
		string typeName = parameter["TypeName"].Value<string>();
		bool num = parameter["IsEnum"].Value<bool>();
		string documentation = parameter["Documentation"].Value<string>();
		JArray jArray = parameter["Diagnostics"].Value<JArray>();
		JObject jObject = parameter["Metadata"].Value<JObject>();
		builder.Name = name;
		builder.TypeName = typeName;
		builder.Documentation = documentation;
		if (num)
		{
			builder.IsEnum = true;
		}
		foreach (JToken item2 in jArray)
		{
			JsonReader reader = item2.CreateReader();
			RazorDiagnostic item = serializer.Deserialize<RazorDiagnostic>(reader);
			builder.Diagnostics.Add(item);
		}
		JsonReader reader2 = jObject.CreateReader();
		foreach (KeyValuePair<string, string> item3 in serializer.Deserialize<Dictionary<string, string>>(reader2))
		{
			builder.Metadata[item3.Key] = item3.Value;
		}
	}
}
