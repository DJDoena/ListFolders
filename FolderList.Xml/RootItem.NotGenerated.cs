using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace DoenaSoft.FolderList.Xml;

public partial class Document
{
    /// <summary />
    [XmlAnyElement]
    public XmlElement[] AnyElements;

    /// <summary />
    [XmlAnyAttribute]
    public XmlAttribute[] AnyAttributes;
}

public partial class RootItem
{
    /// <summary />
    [XmlAnyElement]
    public XmlElement[] AnyElements;

    /// <summary />
    [XmlAnyAttribute]
    public XmlAttribute[] AnyAttributes;
}

[DebuggerDisplay("{Name}")]
public partial class SubItem
{
    /// <summary />
    [XmlAnyElement]
    public XmlElement[] AnyElements;

    /// <summary />
    [XmlAnyAttribute]
    public XmlAttribute[] AnyAttributes;
}