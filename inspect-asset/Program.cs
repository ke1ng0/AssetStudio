using System;
using System.Linq;
using System.Reflection;
using AssetStudio;

var assetPath = @"F:\SteamLibrary\steamapps\common\nice day for fishing\NiceDayForFishing_Data\sharedassets0.assets";
var targetPathId = 49L;
var reader = new FileReader(assetPath);
var manager = new AssetsManager();
var assetsFile = new SerializedFile(reader, manager);
var obj = assetsFile.m_Objects.FirstOrDefault(x => x.m_PathID == targetPathId);
Console.WriteLine($"Asset: {assetPath}");
Console.WriteLine($"Found: {obj != null}");
if (obj == null) return;
Console.WriteLine($"PathID: {obj.m_PathID}");
Console.WriteLine($"ClassID: {obj.classID}");
Console.WriteLine($"ByteSize: {obj.byteSize}");
Console.WriteLine($"Version: {assetsFile.version}");
Console.WriteLine($"Platform: {assetsFile.m_TargetPlatform}");
var field = typeof(SerializedFile).GetField("m_EnableTypeTree", BindingFlags.Instance | BindingFlags.NonPublic);
Console.WriteLine($"TypeTree enabled: {field?.GetValue(assetsFile)}");
var typeTree = obj.serializedType?.m_Type;
Console.WriteLine($"TypeTree present: {typeTree != null}");
Console.WriteLine($"Node count: {typeTree?.m_Nodes?.Count ?? 0}");
for (var i = 0; i < assetsFile.m_Types.Count; i++)
{
    var t = assetsFile.m_Types[i];
    if (t.classID == 114 || t.m_Type?.m_Nodes?.Count > 0)
    {
        Console.WriteLine($"[{i}] classID={t.classID} nodeCount={t.m_Type?.m_Nodes?.Count ?? 0}");
    }
}
