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
    /// <param name="outputFolder">Output location for generated files (optional, defaults to current directory)</param>
    /// <param name="uiOnly">Generates the UI definitions only (optional, defaults to true)</param>
    public static void Main(string file, string window, string withNamespace, string outputFolder, bool uiOnly = true)
    {
        //Ensure input file and window name are specified.
        if (string.IsNullOrEmpty(file) || (string.IsNullOrEmpty(window)))
        {
            Console.WriteLine("The --file and --window must be specified. Use -h for help");
            Environment.Exit(0);
        }

        XmlDocument document = new XmlDocument();
        try
        {
            document.Load(file);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to load file {file}");
            Environment.Exit(1);
        }

        string templateUIClass = GetEmbeddedResource("GeneratedUI.txt");
        string templateClass = GetEmbeddedResource("Generated.txt");

        //find all object nodes.
        var objectNodes = document.GetElementsByTagName("object");
        for (int i = 0; i < objectNodes.Count; i++) //find window with specified window name
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
                var uiData = new
                {
                    windowName = window,
                    hasNamespace = withNamespace is not null,
                    withNamespace = withNamespace,
                    filename = file,
                    widgets = members.Keys.Select(id => new
                    {
                        className = members[id],
                        widgetId = id,
                    })
                };

                var resultUIClass = uiTemplate(uiData);
                string resultClass = "";
                if (!uiOnly) {
                    var classTemplate = Handlebars.Compile(templateClass);
                    resultClass = classTemplate(uiData);
                }

                string outputUIPath = string.IsNullOrEmpty(outputFolder) ? Path.Join(Environment.CurrentDirectory, $"{window}.UI.cs") : Path.Join(outputFolder, $"{window}.UI.cs");
                string outputClassPath = string.IsNullOrEmpty(outputFolder) ? Path.Join(Environment.CurrentDirectory, $"{window}.cs") : Path.Join(outputFolder, $"{window}.cs");

                try
                {
                    File.WriteAllText(outputUIPath, resultUIClass);
                    Console.WriteLine($"Wrote output to {outputUIPath}");

                    if (!uiOnly) {
                        File.WriteAllText(outputClassPath, resultClass);
                        Console.WriteLine($"Wrote output to {outputClassPath}");
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine("Unable to save output.", ex.Message);
                }
                Environment.Exit(0);
            }
        }

        //Won't reach this part unless no nodes are found.
        Console.WriteLine($"No GtkWindow node with id {window}");
        Environment.Exit(1);
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