// Microsoft.VisualStudio.LanguageServices.Razor.Serialization.RazorDiagnosticJsonConverter
using Microsoft.AspNetCore.Razor.Language;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

internal class RazorDiagnosticJsonConverter : JsonConverter
{
	public static readonly RazorDiagnosticJsonConverter Instance = new RazorDiagnosticJsonConverter();

	private const string RazorDiagnosticMessageKey = "Message";

	public override bool CanConvert(Type objectType)
	{
		return typeof(RazorDiagnostic).IsAssignableFrom(objectType);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		if (reader.TokenType != JsonToken.StartObject)
		{
			return null;
		}
		JObject jObject = JObject.Load(reader);
		string id = jObject["Id"].Value<string>();
		int severity = jObject["Severity"].Value<int>();
		string message = jObject["Message"].Value<string>();
		JObject jObject2 = jObject["Span"].Value<JObject>();
		string filePath = jObject2["FilePath"].Value<string>();
		int absoluteIndex = jObject2["AbsoluteIndex"].Value<int>();
		int lineIndex = jObject2["LineIndex"].Value<int>();
		int characterIndex = jObject2["CharacterIndex"].Value<int>();
		int length = jObject2["Length"].Value<int>();
		RazorDiagnosticDescriptor descriptor = new RazorDiagnosticDescriptor(id, () => message, (RazorDiagnosticSeverity)severity);
		SourceSpan span = new SourceSpan(filePath, absoluteIndex, lineIndex, characterIndex, length);
		return RazorDiagnostic.Create(descriptor, span);
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		RazorDiagnostic razorDiagnostic = (RazorDiagnostic)value;
		writer.WriteStartObject();
		WriteProperty(writer, "Id", razorDiagnostic.Id);
		WriteProperty(writer, "Severity", (int)razorDiagnostic.Severity);
		WriteProperty(writer, "Message", razorDiagnostic.GetMessage(CultureInfo.CurrentCulture));
		writer.WritePropertyName("Span");
		writer.WriteStartObject();
		WriteProperty(writer, "FilePath", razorDiagnostic.Span.FilePath);
		WriteProperty(writer, "AbsoluteIndex", razorDiagnostic.Span.AbsoluteIndex);
		WriteProperty(writer, "LineIndex", razorDiagnostic.Span.LineIndex);
		WriteProperty(writer, "CharacterIndex", razorDiagnostic.Span.CharacterIndex);
		WriteProperty(writer, "Length", razorDiagnostic.Span.Length);
		writer.WriteEndObject();
		writer.WriteEndObject();
	}

	private void WriteProperty<T>(JsonWriter writer, string key, T value)
	{
		writer.WritePropertyName(key);
		writer.WriteValue(value);
	}
}
