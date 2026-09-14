using System;
using System.Linq;
using Mono.Cecil;

var dll = @"G:\GITHUB\Il2CppDumper\Il2CppDumper\bin\Release\net8.0\dump-output\DummyDll\UnityEngine.CoreModule.dll";
var asm = AssemblyDefinition.ReadAssembly(dll);
foreach (var type in asm.MainModule.Types.Where(t => t.FullName.Contains("Object") || t.FullName.Contains("Component") || t.FullName.Contains("Behaviour") || t.FullName.Contains("MonoBehaviour") || t.FullName.Contains("GameObject") || t.FullName.Contains("ScriptableObject")).OrderBy(t => t.FullName))
{
    Console.WriteLine($"TYPE {type.FullName}");
    foreach (var field in type.Fields)
    {
        Console.WriteLine($"  FIELD {field.FieldType.FullName} {field.Name}");
    }
    Console.WriteLine();
}
