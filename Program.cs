using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

const string nameSpace = "HellNameSpace";
const string className = "HellClass";
const string methodName = "HellOMethod";

const string codeToRun = @$"
namespace {nameSpace};

public class {className}
{{ 
    public static void {methodName}()
    {{
        System.Console.WriteLine(""Hello Boys"");
    }}
}}";


var syntaxTree = CSharpSyntaxTree.ParseText(codeToRun, new CSharpParseOptions(LanguageVersion.CSharp10));

var coreLocation = typeof(int).Assembly.Location;


var cSharpCompilation = CSharpCompilation.Create("HellAsm",
    new[] { syntaxTree },
    new List<MetadataReference>()
    {
        MetadataReference.CreateFromFile(coreLocation),
        MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(coreLocation)!,"System.Runtime.dll")),

    }, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

await using var stream = new MemoryStream();
var emit = cSharpCompilation.Emit(stream);

if (!emit.Success)
    return;

stream.Seek(0, SeekOrigin.Begin);
var asm = Assembly.Load(stream.ToArray());

var type = asm.GetType($"{nameSpace}.{className}");
if (type is null)
    return;


type.InvokeMember(methodName, BindingFlags.Default | BindingFlags.InvokeMethod, null, null, null);


Console.ReadKey();