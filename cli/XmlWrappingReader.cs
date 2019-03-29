using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;

namespace cli
{
    public class XmlWrappingReader : XmlReader, IXmlLineInfo
    {
//
// Fields
//
        protected XmlReader reader;
        protected IXmlLineInfo readerAsIXmlLineInfo;

// 
// Constructor
//
        internal XmlWrappingReader(XmlReader baseReader)
        {
            Debug.Assert(baseReader != null);
            this.reader = baseReader;
            this.readerAsIXmlLineInfo = baseReader as IXmlLineInfo;
        }

//
// XmlReader implementation
//
        public override XmlReaderSettings Settings => reader.Settings;

        public override XmlNodeType NodeType => reader.NodeType;

        public override string Name => reader.Name;

        public override string LocalName => reader.LocalName;

        public override string NamespaceURI => reader.NamespaceURI;

        public override string Prefix => reader.Prefix;

        public override bool HasValue => reader.HasValue;

        public override string Value => reader.Value;

        public override int Depth => reader.Depth;

        public override string BaseURI => reader.BaseURI;

        public override bool IsEmptyElement => reader.IsEmptyElement;

        public override bool IsDefault => reader.IsDefault;

        public override XmlSpace XmlSpace => reader.XmlSpace;

        public override string XmlLang => reader.XmlLang;

        public override System.Type ValueType => reader.ValueType;

        public override int AttributeCount => reader.AttributeCount;

        public override bool EOF => reader.EOF;

        public override ReadState ReadState => reader.ReadState;

        public override bool HasAttributes => reader.HasAttributes;

        public override XmlNameTable NameTable => reader.NameTable;

        public override bool CanResolveEntity => reader.CanResolveEntity;

//
// IXmlLineInfo members
//
        public virtual bool HasLineInfo()
        {
            return (readerAsIXmlLineInfo == null) ? false : readerAsIXmlLineInfo.HasLineInfo();
        }

        public virtual int LineNumber => (readerAsIXmlLineInfo == null) ? 0 : readerAsIXmlLineInfo.LineNumber;

        public virtual int LinePosition => (readerAsIXmlLineInfo == null) ? 0 : readerAsIXmlLineInfo.LinePosition;

        public override string GetAttribute(string name)
        {
            return reader.GetAttribute(name);
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            return reader.GetAttribute(name, namespaceURI);
        }

        public override string GetAttribute(int i)
        {
            return reader.GetAttribute(i);
        }

        public override bool MoveToAttribute(string name)
        {
            return reader.MoveToAttribute(name);
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            return reader.MoveToAttribute(name, ns);
        }

        public override void MoveToAttribute(int i)
        {
            reader.MoveToAttribute(i);
        }

        public override bool MoveToFirstAttribute()
        {
            return reader.MoveToFirstAttribute();
        }

        public override bool MoveToNextAttribute()
        {
            return reader.MoveToNextAttribute();
        }

        public override bool MoveToElement()
        {
            return reader.MoveToElement();
        }

        public override bool Read()
        {
            return reader.Read();
        }

        public override void Close()
        {
            reader.Close();
        }

        public override void Skip()
        {
            reader.Skip();
        }

        public override string LookupNamespace(string prefix)
        {
            return reader.LookupNamespace(prefix);
        }

        public override void ResolveEntity()
        {
            reader.ResolveEntity();
        }

        public override bool ReadAttributeValue()
        {
            return reader.ReadAttributeValue();
        }

#if !SILVERLIGHT
        public override IXmlSchemaInfo SchemaInfo => reader.SchemaInfo;

        public override char QuoteChar => reader.QuoteChar;
#endif
    }
}