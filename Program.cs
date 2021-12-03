using System.Reflection;
using System.Xml;
using HandlebarsDotNet;
using System.Linq;

class Program
{

    /// <summary>
    /// A simple command-line application to generate ready-to-use C# class for specified window in glade file
    /// </summary>
    /// <param name="file">The .glade XML input file to generate from</param>
    /// <param name="window">The window id in the glade resource</param>
    /// <param name="withNamespace">Namespace for generated class</param>
    /// <param name="outputFolder">Output location for generated files</param>
    public static void Main(string file, string window, string withNamespace, string outputFolder)
    {
        XmlDocument document = new XmlDocument();
        document.Load(file);
        string templateUIClass = GetEmbeddedResource("GeneratedUI.txt");
        string templateMember = GetEmbeddedResource("GeneratedUI.txt");

        var objectNodes = document.GetElementsByTagName("object");
        for (int i = 0; i < objectNodes.Count; i++)
        {
            var node = objectNodes[i];
            if (node.Attributes != null &&
                node.Attributes["class"] != null &&
                node.Attributes["class"].Value == "GtkWindow" &&
                node.Attributes["id"] != null &&
                node.Attributes["id"].Value == window)
            {
                Dictionary<string, string> members = new Dictionary<string, string>();

                var specifiedWindowNodes = node.SelectNodes("descendant::object[@id]");
                for (int j = 0; j < specifiedWindowNodes.Count; j++)
                {
                    var memberNode = specifiedWindowNodes[j];
                    string className = memberNode.Attributes["class"].Value.Remove(0, 3);
                    string widgetId = memberNode.Attributes["id"].Value;
                    members.Add(widgetId, className);
                }

                var uiTemplate = Handlebars.Compile(templateUIClass);
                var memberTemplate = Handlebars.Compile(templateMember);
                var data = new
                {
                    windowName = window,
                    hasNamespace = withNamespace is not null,
                    withNamespace = withNamespace,
                    widgets = members.Keys.Select(id => new
                    {
                        className = members[id],
                        widgetId = id,
                    })
                };

                var result = uiTemplate(data);
                string outputPath = string.IsNullOrEmpty(outputFolder) ? Path.Join(Environment.CurrentDirectory, $"{window}.UI.cs") : Path.Join(outputFolder, $"{window}.UI.cs");
                File.WriteAllText(outputPath, result);
                Console.WriteLine($"Wrote output to {outputPath}");
            }
        }
    }

    public static string GetEmbeddedResource(string filename)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(filename))
        using (StreamReader reader = new StreamReader(stream))
        {
            string result = reader.ReadToEnd();
            return result;
        }
    }

}