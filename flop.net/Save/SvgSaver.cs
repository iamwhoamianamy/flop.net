using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Xml;
using flop.net.Model;

namespace flop.net.Save;

public class SvgSaver
{
    private string _filename;
    private string _filepath;
    private XmlTextWriter _xmlWriter;
    private Collection<Layer> _layers;
    public MemoryStream SVGDocument { get; private set; }
    public string Filename => _filename;
    public string Filepath => _filepath;
    
    public SvgSaver(Collection<Layer> layers)
    {
        SVGDocument = new MemoryStream();
        _layers = layers;
        _xmlWriter = new XmlTextWriter(SVGDocument, Encoding.UTF8)
        {
            Formatting = Formatting.Indented
        };

    }

    /*public void Save()
    {
        WriteBegin();
        foreach (var figure in _layers)
        {
            switch (figure.type)
            {
                case "Rectangle":
                    WriteRectangle();
                    break;
                case "Trinagle":
                    WriteTriangle();
                    break;
                case "Line":
                    WriteLine();
                    break;
                case "BrokenLine":
                    WriteBrokenLine();
                    break;
                case "Circle":
                    WriteCircle();
                    break;
            }
        }
        WriteEnd();
    }*/
    private void WriteBegin()
    {
        _xmlWriter.WriteStartDocument();
        _xmlWriter.WriteDocType("svg", "-//W3C//DTD SVG 1.1//EN", "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd", null);
        _xmlWriter.WriteStartElement("svg", "http://www.w3.org/2000/svg");
        _xmlWriter.WriteAttributeString("version", "1.1");
        _xmlWriter.WriteAttributeString("width", 500.ToString());
        _xmlWriter.WriteAttributeString("height", 500.ToString());
    }

    private void WriteEnd()
    {
        _xmlWriter.WriteEndDocument();
        _xmlWriter.Flush();
    }

    private void WriteRectangle()
    {
        
    }

    private void WriteTriangle()
    {
        
    }

    private void WriteLine()
    {
        
    }

    private void WriteEllipse()
    {
        
    }

    private void WriteCircle()
    {
        
    }

    private void WriteBrokenLine()
    {
        
    }
}