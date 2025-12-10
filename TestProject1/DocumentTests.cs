using DoenaSoft.FolderList.Xml;
using DoenaSoft.ToolBox.Generics;

namespace TestProject1;

[TestClass]
public class DocumentTests
{
    [TestMethod]
    public void Deserialize()
    {
        //const string FileName = @"N:\Drive1\TVShows\9-1-1 Lone Star\Season 5\HD\911LS 5x01 [ Both Sides, Now ].720.mkv.xml";
        const string FileName = @"N:\Drive1\Movies\movies.xml";

        //using var fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None);

        //using var xr = XmlReader.Create(fs, new()
        //{
        //    DtdProcessing = DtdProcessing.Parse,
        //});

        ////var s = new System.Xml.Serialization.XmlSerializer(typeof(VideoInfoDocument));
        //var s = new System.Xml.Serialization.XmlSerializer(typeof(Document));

        ////var doc = (VideoInfoDocument)s.Deserialize(xr);

        //var doc = (Document)s.Deserialize(xr); 

        var doc = XsltSerializer<Document>.Deserialize(FileName);
    }
}
