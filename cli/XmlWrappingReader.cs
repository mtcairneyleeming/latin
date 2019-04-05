using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace cli
{
    public class XmlExtendableReader : XmlWrappingReader
    {
        public XmlExtendableReader(Stream input, XmlReaderSettings settings, bool ignoreNamespace = false)
            : base(Create(input, settings))
        {
            IgnoreNamespace = ignoreNamespace;
        }

        private bool IgnoreNamespace { get; }

        public override string NamespaceURI => IgnoreNamespace ? string.Empty : base.NamespaceURI;
    }


    public class XmlWrappingReader : XmlReader, IXmlLineInfo
    {
//
// Fields
//
        private readonly XmlReader _reader;
        private readonly IXmlLineInfo _readerAsIXmlLineInfo;

// 
// Constructor
//
        internal XmlWrappingReader(XmlReader baseReader)
        {
            Debug.Assert(baseReader != null);
            _reader = baseReader;
            _readerAsIXmlLineInfo = baseReader as IXmlLineInfo;
        }

//
// XmlReader implementation
//
        public override XmlReaderSettings Settings => _reader.Settings;

        public override XmlNodeType NodeType => _reader.NodeType;

        public override string Name => _reader.Name;

        public override string LocalName => _reader.LocalName;

        public override string NamespaceURI => _reader.NamespaceURI;

        public override string Prefix => _reader.Prefix;

        public override bool HasValue => _reader.HasValue;

        public override string Value => _reader.Value;

        public override int Depth => _reader.Depth;

        public override string BaseURI => _reader.BaseURI;

        public override bool IsEmptyElement => _reader.IsEmptyElement;

        public override bool IsDefault => _reader.IsDefault;

        public override XmlSpace XmlSpace => _reader.XmlSpace;

        public override string XmlLang => _reader.XmlLang;

        public override Type ValueType => _reader.ValueType;

        public override int AttributeCount => _reader.AttributeCount;

        public override bool EOF => _reader.EOF;

        public override ReadState ReadState => _reader.ReadState;

        public override bool HasAttributes => _reader.HasAttributes;

        public override XmlNameTable NameTable => _reader.NameTable;

        public override bool CanResolveEntity => _reader.CanResolveEntity;

//
// IXmlLineInfo members
//
        public virtual bool HasLineInfo()
        {
            return _readerAsIXmlLineInfo?.HasLineInfo() ?? false;
        }

        public virtual int LineNumber => _readerAsIXmlLineInfo?.LineNumber ?? 0;

        public virtual int LinePosition => _readerAsIXmlLineInfo?.LinePosition ?? 0;

        public override string GetAttribute(string name)
        {
            return _reader.GetAttribute(name);
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            return _reader.GetAttribute(name, namespaceURI);
        }

        public override string GetAttribute(int i)
        {
            return _reader.GetAttribute(i);
        }

        public override bool MoveToAttribute(string name)
        {
            return _reader.MoveToAttribute(name);
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            return _reader.MoveToAttribute(name, ns);
        }

        public override void MoveToAttribute(int i)
        {
            _reader.MoveToAttribute(i);
        }

        public override bool MoveToFirstAttribute()
        {
            return _reader.MoveToFirstAttribute();
        }

        public override bool MoveToNextAttribute()
        {
            return _reader.MoveToNextAttribute();
        }

        public override bool MoveToElement()
        {
            return _reader.MoveToElement();
        }

        public override bool Read()
        {
            return _reader.Read();
        }

        public override void Close()
        {
            _reader.Close();
        }

        public override void Skip()
        {
            _reader.Skip();
        }

        public override string LookupNamespace(string prefix)
        {
            return _reader.LookupNamespace(prefix);
        }

        public override void ResolveEntity()
        {
            _reader.ResolveEntity();
        }

        public override bool ReadAttributeValue()
        {
            return _reader.ReadAttributeValue();
        }
    }
}