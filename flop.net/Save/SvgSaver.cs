using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;
using flop.net.Model;

namespace flop.net.Save;

public class SvgSaver
{
   private string _fullFilename;
   private string _filepath;
   private XmlWriter _xmlWriter;
   private Layer _layers;
   private XmlWriterSettings _settings;
   private int _width;
   private int _height;
   public string FullFilename => _fullFilename;

   public SvgSaver(string fullFilename, Layer layers, int width, int height)
   {
      _layers = layers;
      _width = width;
      _height = height;
      _fullFilename = fullFilename;
      _settings = new XmlWriterSettings();
      SetXmlWriterFormatSettings(Encoding.UTF8, true, NewLineHandling.Replace, true);
      _xmlWriter = XmlWriter.Create(fullFilename, _settings);
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
   public void SetXmlWriterFormatSettings(Encoding encoding, bool indent, NewLineHandling newLineHandling,
      bool newLineOnAttributes)
   {
      _settings.Encoding = encoding;
      _settings.Indent = indent;
      _settings.NewLineHandling = newLineHandling;
      _settings.NewLineOnAttributes = newLineOnAttributes;
   }
   //todo async
   public void Save()
   {
      WriteBegin();
      foreach (var item in _layers.Figures)
      {
         if (item.Geometric.IsClosed == true)
            WritePolygon(item);
         else
            WritePolyline(item);
      }
      WriteEnd();
   }
   
   private void WriteBegin()
   {
      _xmlWriter.WriteStartDocument();
      _xmlWriter.WriteWhitespace("\n");
      _xmlWriter.WriteDocType("svg", "-//W3C//DTD SVG 1.1//EN", "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd",
         null);
      _xmlWriter.WriteWhitespace("\n");
      _xmlWriter.WriteStartElement("svg", "http://www.w3.org/2000/svg");
      _xmlWriter.WriteAttributeString("xmlns", "xlink", null, "http://www.w3.org/1999/xlink");
      _xmlWriter.WriteAttributeString("xmlns", "ev", null, "http://www.w3.org/2001/xml-events");
      _xmlWriter.WriteAttributeString("version", "1.1");
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
   private void WritePolygon(Model.Figure figure)
   {
      _xmlWriter.WriteStartElement("polygon");
      _xmlWriter.WriteAttributeString("points", WritePoints(figure.Geometric));
      _xmlWriter.WriteAttributeString("fill", $"{HexConverter(figure.DrawingParameters.Fill)}");
      _xmlWriter.WriteAttributeString("opacity", $"{figure.DrawingParameters.Opacity.ToString().Replace(",", ".")}");
      _xmlWriter.WriteAttributeString("stroke",$"{HexConverter(figure.DrawingParameters.Stroke)}");
      _xmlWriter.WriteAttributeString("stroke-width",$"{figure.DrawingParameters.StrokeThickness}");
      _xmlWriter.WriteEndElement();
   }

   private void WritePolyline(Model.Figure figure)
   {
      _xmlWriter.WriteStartElement("polyline");
      _xmlWriter.WriteAttributeString("points", WritePoints(figure.Geometric));
      _xmlWriter.WriteAttributeString("fill", "none");
      _xmlWriter.WriteAttributeString("opacity", $"{figure.DrawingParameters.Opacity.ToString().Replace(",", ".")}");
      _xmlWriter.WriteAttributeString("stroke", $"{HexConverter(figure.DrawingParameters.Fill)}");
      _xmlWriter.WriteAttributeString("stroke-width", $"{figure.DrawingParameters.StrokeThickness}");
      _xmlWriter.WriteEndElement();
   }
   private String HexConverter(Color c)
   {
      return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
   }
   private string WritePoints(IGeometric geometric)
   {
      string result = null;
      foreach (var item in geometric.Points)
      {
         result += (int)item.X + "," + (int)item.Y;
         result += " ";
      }
      return result;
   }
   // inactive methods
   private void WriteRectangle(IGeometric geometric)
   {
      _xmlWriter.WriteStartElement("rect");
      _xmlWriter.WriteAttributeString("x", "123");
      _xmlWriter.WriteAttributeString("y", "123");
      _xmlWriter.WriteAttributeString("width", "123");
      _xmlWriter.WriteAttributeString("height", 500.ToString());
      _xmlWriter.WriteAttributeString("fill", "red");
      _xmlWriter.WriteEndElement();
   }

   private void WriteTriangle()
   {
   }
   
 
   private void WriteEllipse()
   {
   }

   private void WriteCircle()
   {
   }
}