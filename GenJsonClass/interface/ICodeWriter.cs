using System.IO;

namespace GenJsonClass
{
    public interface ICodeWriter
    {
        string FileExtension { get; }
        string DisplayName { get; }
        string GetTypeName(JsonType type, IJsonClassConfig config);
        void WriteClass(IJsonClassConfig config, TextWriter sw, JsonType type);
        void WriteFileStart(IJsonClassConfig config, TextWriter sw);
        void WriteFileEnd(IJsonClassConfig config, TextWriter sw);
        void WriteNamespaceStart(IJsonClassConfig config, TextWriter sw, bool root);
        void WriteNamespaceEnd(IJsonClassConfig config, TextWriter sw, bool root);
    }
}