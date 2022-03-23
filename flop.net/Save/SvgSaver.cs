using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;
using flop.net.Enums;
using flop.net.Model;
using Figure=flop.net.Model.Figure;

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
   public void SetXmlWriterFormatSettings(Encoding encoding, bool indent, NewLineHandling newLineHandling, bool newLineOnAttributes)
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
         switch (item.Geometric)
         {
            case Ellipse:
               WriteEllipse(item);
               break;
            case Polygon:
               if(item.Geometric.IsClosed)
                  WritePolygon(item);
               else
                  
               break;
            
         }
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
   private void WritePolygon(Figure figure)
   {
      _xmlWriter.WriteStartElement("polygon");
      _xmlWriter.WriteAttributeString("points", WritePoints(figure.Geometric));
      _xmlWriter.WriteAttributeString("fill", $"{HexConverter(figure.DrawingParameters.Fill)}");
      _xmlWriter.WriteAttributeString("stroke",$"{HexConverter(figure.DrawingParameters.Stroke)}");
      _xmlWriter.WriteAttributeString("stroke-width",$"{figure.DrawingParameters.StrokeThickness}");
      _xmlWriter.WriteEndElement();
   }

   private void WritePolyline(Model.Figure figure)
   {
      _xmlWriter.WriteStartElement("polyline");
      _xmlWriter.WriteAttributeString("points", WritePoints(figure.Geometric));
      _xmlWriter.WriteAttributeString("fill", "none");
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
   private string WritePoint(IGeometric geometric)
   {
      string result = null;
      var xMi = geometric.Points.Min();
      
      return result;
   }
   private void WriteEllipse(Figure figure)
   {
      _xmlWriter.Settings.NewLineOnAttributes = false;
      _xmlWriter.WriteStartElement("ellipse");
      _xmlWriter.WriteAttributeString("cx", String.Format("{0:0}",figure.Geometric.Center.X));
      _xmlWriter.WriteAttributeString("cy", String.Format("{0:0}",figure.Geometric.Center.Y));
      string rx = String.Format("{0:0}", (int) ((figure.Geometric as Ellipse).Width / 2));
      _xmlWriter.WriteAttributeString("rx", rx);
      string ry = String.Format("{0:0}", (int) ((figure.Geometric as Ellipse).Height / 2));
      _xmlWriter.WriteAttributeString("ry", ry);
      _xmlWriter.WriteAttributeString("fill", $"{HexConverter(figure.DrawingParameters.Fill)}");
      _xmlWriter.WriteAttributeString("stroke",$"{HexConverter(figure.DrawingParameters.Stroke)}");
      _xmlWriter.WriteAttributeString("stroke-width",figure.DrawingParameters.StrokeThickness.ToString());
   }

   private void WritePolyline(Figure figure)
   {
      
   }
}