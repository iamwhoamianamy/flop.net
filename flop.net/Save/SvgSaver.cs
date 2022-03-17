using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Xml;
using flop.net.Enums;
using flop.net.Model;
using Microsoft.Win32;

namespace flop.net.Save;

public class SvgSaver
{
    private string _filename;
    private string _filepath;
    private XmlWriter _xmlWriter;
    private Layer _layers;
    private XmlWriterSettings _settings;
    private int _width;
    private int _height;
    public string Filename => _filename;
    public string Filepath => _filepath;
    
    
    public SvgSaver(Layer layers,int width,int height)
    {
        _layers = layers;
        _width = width;
        _height = height;
        _settings = new XmlWriterSettings();
        SetXmlWriterFormatSettings(Encoding.UTF8,true,NewLineHandling.Replace,true);
        
        SaveFileDialog saveDialog = new SaveFileDialog();
        saveDialog.Filter= "svg files (*.svg)|*.svg";
        saveDialog.RestoreDirectory = true;
        if (saveDialog.ShowDialog() == true)
        {
            _xmlWriter = XmlWriter.Create(saveDialog.FileName,_settings);
            Save();
        }
    }
    
    /// <summary>
    /// Форматированием выходных данных
    /// </summary>
    /// <param name="encoding"></param>
    /// <param name="indent"></param>
    /// <param name="indentChars"></param>
    /// <param name="newLineChars"></param>
    /// <param name="newLineHandling"></param>
    /// <param name="newLineOnAttributes"></param>
    public void SetXmlWriterFormatSettings(Encoding encoding,bool indent,NewLineHandling newLineHandling,bool newLineOnAttributes)
    {
        _settings.Encoding = encoding;
        _settings.Indent = indent;
        _settings.NewLineHandling = newLineHandling;
        _settings.NewLineOnAttributes = newLineOnAttributes;
    }
    
    public void Save()
    {
        WriteBegin();
        WriteRectangle();
        WriteEnd();
    }
    
    private void WriteBegin()
    {
        _xmlWriter.WriteStartDocument();
        _xmlWriter.WriteWhitespace("\n");
        _xmlWriter.WriteDocType("svg", "-//W3C//DTD SVG 1.1//EN", "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd", null);
        _xmlWriter.WriteWhitespace("\n");
        _xmlWriter.WriteStartElement("svg","http://www.w3.org/2000/svg");
        _xmlWriter.WriteAttributeString("xmlns","xlink",null,"http://www.w3.org/1999/xlink");
        _xmlWriter.WriteAttributeString("xmlns","ev",null,"http://www.w3.org/2001/xml-events");
        _xmlWriter.WriteAttributeString("version","1.1");
        _xmlWriter.WriteAttributeString("baseProfile", "full");
        _xmlWriter.WriteAttributeString("width", _width.ToString());
        _xmlWriter.WriteAttributeString("height", _height.ToString());
    }

    private void WriteEnd()
    {
        _xmlWriter.WriteEndDocument();
        _xmlWriter.Flush();
        _xmlWriter.Close();
    }

    private void WriteRectangle()
    {
        _xmlWriter.WriteStartElement("rect");
        _xmlWriter.WriteAttributeString("x",400.ToString());
        _xmlWriter.WriteAttributeString("y",500.ToString());
        _xmlWriter.WriteAttributeString("width",500.ToString());
        _xmlWriter.WriteAttributeString("height",500.ToString());
        _xmlWriter.WriteAttributeString("fill","red");
        _xmlWriter.WriteEndElement();
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