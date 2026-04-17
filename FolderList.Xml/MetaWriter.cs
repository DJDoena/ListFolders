using DoenaSoft.ToolBox.Generics;

namespace DoenaSoft.FolderList.Xml;

public static class MetaWriter
{
    public static void Write(RootItem rootItem, string fileName)
    {
        (new XsltSerializer<RootItem>(new RootItemXsltSerializerDataProvider())).Serialize(fileName, rootItem);
    }
}