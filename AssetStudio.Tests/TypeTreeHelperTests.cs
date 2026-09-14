using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using AssetStudio;
using Xunit;

namespace AssetStudio.Tests;

public class TypeTreeHelperTests
{
    [Fact]
    public void ReadType_AdvancesPastDeclaredBytes_ForUnknownNodeTypes()
    {
        var typeTree = new TypeTree
        {
            m_Nodes = new List<TypeTreeNode>
            {
                new TypeTreeNode { m_Type = "Root", m_Name = "Root", m_Level = 0, m_MetaFlag = 0 },
                new TypeTreeNode { m_Type = "UnknownType", m_Name = "payload", m_Level = 1, m_MetaFlag = 0, m_ByteSize = 4 }
            }
        };

        using var stream = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        using var reader = new EndianBinaryReader(stream, EndianType.LittleEndian);
        var objectReader = new ObjectReader(reader, CreateFakeSerializedFile(), CreateFakeObjectInfo());

        var exception = Record.Exception(() => TypeTreeHelper.ReadType(typeTree, objectReader));

        Assert.Null(exception);
        Assert.True(objectReader.Position > objectReader.byteStart);
        Assert.True(objectReader.Position <= objectReader.byteStart + objectReader.byteSize);
    }

    [Fact]
    public void ReadType_ParsesKnownPrimitiveNodes_WithDeclaredByteSize()
    {
        var typeTree = new TypeTree
        {
            m_Nodes = new List<TypeTreeNode>
            {
                new TypeTreeNode { m_Type = "Root", m_Name = "Root", m_Level = 0, m_MetaFlag = 0 },
                new TypeTreeNode { m_Type = "int", m_Name = "value", m_Level = 1, m_MetaFlag = 0, m_ByteSize = 4 }
            }
        };

        using var stream = new MemoryStream(BitConverter.GetBytes(0x12345678));
        using var reader = new EndianBinaryReader(stream, EndianType.LittleEndian);
        var objectReader = new ObjectReader(reader, CreateFakeSerializedFile(), CreateFakeObjectInfo());

        var result = TypeTreeHelper.ReadType(typeTree, objectReader);

        Assert.Equal(0x12345678, result["value"]);
    }

    [Fact]
    public void MonoBehaviour_Constructor_ReadsFallbackFields_WhenNoTypeTreeNodesExist()
    {
        using var stream = new MemoryStream(new byte[64]);
        using var reader = new EndianBinaryReader(stream, EndianType.LittleEndian);
        var objectReader = new ObjectReader(reader, CreateFakeSerializedFile(), CreateFakeObjectInfo());
        objectReader.serializedType = new SerializedType { m_Type = new TypeTree() };

        var exception = Record.Exception(() => new MonoBehaviour(objectReader));

        Assert.Null(exception);
        Assert.True(objectReader.Position > objectReader.byteStart);
    }

    [Fact]
    public void ReadType_UsesDeclaredByteSizeForUnknownNodes_WhenSchemaHasNoChildNodes()
    {
        var typeTree = new TypeTree
        {
            m_Nodes = new List<TypeTreeNode>
            {
                new TypeTreeNode { m_Type = "Root", m_Name = "Root", m_Level = 0, m_MetaFlag = 0 },
                new TypeTreeNode { m_Type = "UnknownType", m_Name = "payload", m_Level = 1, m_MetaFlag = 0, m_ByteSize = 4 }
            }
        };

        using var stream = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        using var reader = new EndianBinaryReader(stream, EndianType.LittleEndian);
        var objectReader = new ObjectReader(reader, CreateFakeSerializedFile(), CreateFakeObjectInfo());

        var result = TypeTreeHelper.ReadType(typeTree, objectReader);

        Assert.Single(result);
        Assert.Equal(3L, result["payload"]);
    }

    private static SerializedFile CreateFakeSerializedFile()
    {
        var file = (SerializedFile)FormatterServices.GetUninitializedObject(typeof(SerializedFile));
        file.header = new SerializedFileHeader { m_Version = SerializedFileFormatVersion.Unknown_7 };
        file.version = new UnityVersion(2020, 3, 0);
        file.m_TargetPlatform = BuildTarget.NoTarget;
        return file;
    }

    private static ObjectInfo CreateFakeObjectInfo()
    {
        return new ObjectInfo
        {
            m_PathID = 1,
            byteStart = 0,
            byteSize = 3,
            classID = 0,
            serializedType = null
        };
    }
}
