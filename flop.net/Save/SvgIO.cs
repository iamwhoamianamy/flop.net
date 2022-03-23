using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using flop.net.Model;
using Figure = flop.net.Model.Figure;
using Polygon = flop.net.Model.Polygon;

namespace flop.net.Save;

public class SvgIO
{
   private readonly string _filepath;
   private readonly XmlWriter _xmlWriter;
   private readonly Layer _layers;
   private readonly XmlWriterSettings _settings;
   private readonly int _width;
   private readonly int _height;

   public string Filepath => _filepath;

   #region SaveSVG

   public SvgIO(string filepath, Layer layers, int width, int height)
   {
      _layers = layers;
      _width = width;
      _height = height;
      _filepath = filepath;
      _settings = new XmlWriterSettings();
      SetXmlWriterFormatSettings(Encoding.UTF8, true, NewLineHandling.Replace, true);
      _xmlWriter = XmlWriter.Create(filepath, _settings);
      _settings = _xmlWriter.Settings;
   }

   /// Форматированием выходных данных
   public void SetXmlWriterFormatSettings(Encoding encoding, bool indent, NewLineHandling newLineHandling,
      bool newLineOnAttributes)
   {
      _settings.Encoding = encoding;
      _settings.Indent = indent;
      _settings.NewLineHandling = newLineHandling;
      _settings.NewLineOnAttributes = newLineOnAttributes;
   }

   public void Save() //todo async
   {
      WriteBegin();
      foreach (var item in _layers.Figures)
      {
         if (item.Geometric.IsClosed)
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

   private void WritePolygon(Figure figure)
   {
      _xmlWriter.WriteStartElement("polygon");
      _xmlWriter.WriteAttributeString("points", WritePoints(figure.Geometric));
      _xmlWriter.WriteAttributeString("fill", $"{HexConverter(figure.DrawingParameters.Fill)}");
      _xmlWriter.WriteAttributeString("stroke", $"{HexConverter(figure.DrawingParameters.Stroke)}");
      _xmlWriter.WriteAttributeString("stroke-width", figure.DrawingParameters.StrokeThickness.ToString());
      _xmlWriter.WriteAttributeString("opacity", figure.DrawingParameters.Opacity.ToString());
      _xmlWriter.WriteEndElement();
   }

   private void WritePolyline(Figure figure)
   {
      _xmlWriter.WriteStartElement("polyline");
      _xmlWriter.WriteAttributeString("points", WritePoints(figure.Geometric));
      _xmlWriter.WriteAttributeString("fill", "none");
      _xmlWriter.WriteAttributeString("stroke", $"{HexConverter(figure.DrawingParameters.Fill)}");
      _xmlWriter.WriteAttributeString("stroke-width", $"{figure.DrawingParameters.StrokeThickness.ToString()}");
      _xmlWriter.WriteAttributeString("opacity", figure.DrawingParameters.Opacity.ToString());
      _xmlWriter.WriteEndElement();
   }

   private void WriteEnd()
   {
      _xmlWriter.WriteEndDocument();
      _xmlWriter.Flush();
      _xmlWriter.Close();
   }

   private string WritePoints(IGeometric geometric)
   {
      string result = null;
      int count = 1;
      Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
      foreach (var item in geometric.Points)
      {
         //result += item.X + "," + (int)item.Y;
         result += $"{Convert.ToDouble(item.X)},{Convert.ToDouble(item.Y)}";
         if (count != geometric.Points.Count)
            result += " ";
         else
            return result;
         count++;
      }

      return result;
   }

   #endregion

   #region OpenSVG

   public SvgIO(string filepath) //todo async
   {
      _filepath = filepath;
   }

   public Layer Open()
   {
      Layer layer = new Layer();
      using (XmlReader reader = XmlReader.Create(_filepath, SetXmlReaderSetting()))
      {
         bool itsFigure = false;
         bool itsPolyline = false;
         int currentFigure = -1;
         while (reader.MoveToNextAttribute() || reader.Read())
         {
            switch (reader.NodeType)
            {
               case XmlNodeType.Element:
                  switch (reader.Name)
                  {
                     case "polygon":
                        itsFigure = true;
                        itsPolyline = false;
                        layer.Figures.Add(new Figure(new Polygon(), new DrawingParameters()));
                        currentFigure++;
                        break;
                     case "polyline":
                        itsPolyline = true;
                        itsFigure = false;
                        layer.Figures.Add(new Figure(new Polygon(), new DrawingParameters()));
                        currentFigure++;
                        break;
                     default:
                        itsFigure = false;
                        itsPolyline = false;
                        break;
                  }

                  break;
               case XmlNodeType.Attribute:
                  if (itsFigure || itsPolyline)
                  {
                     switch (reader.Name)
                     {
                        case "points":
                           layer.Figures[currentFigure].Geometric.Points = SetPoints(reader.Value);
                           if (itsFigure) layer.Figures[currentFigure].Geometric.IsClosed = true;
                           break;
                        case "fill":
                           if (reader.Value == "none")
                           {
                              layer.Figures[currentFigure].DrawingParameters.Fill = Colors.White;
                              break;
                           }

                           layer.Figures[currentFigure].DrawingParameters.Fill =
                              (Color) ColorConverter.ConvertFromString(reader.Value);
                           break;
                        case "stroke":
                           if (reader.Value != "none")
                              layer.Figures[currentFigure].DrawingParameters.Stroke =
                                 (Color) ColorConverter.ConvertFromString(reader.Value);
                           break;
                        case "stroke-width":
                           if (reader.Value != "none")
                              layer.Figures[currentFigure].DrawingParameters.StrokeThickness = int.Parse(reader.Value);
                           break;
                        case "Opacity":
                           if (reader.Value != "none")
                              layer.Figures[currentFigure].DrawingParameters.Opacity = double.Parse(reader.Value);
                           break;
                     }
                  }

                  break;
            }
         }
      }
      return layer;
   }

   private PointCollection SetPoints(string points)
   {
      PointCollection pointCollection = new PointCollection();
      var digits = Regex.Matches(points, @"-?\d+(?:\.\d+)?");
      Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
      for (int i = 0; i < digits.Count; i += 2)
      {
         //pointCollection.Add(new Point(double.Parse(digits[i].Value),double.Parse(digits[i+1].Value)));
         pointCollection.Add(new Point(Convert.ToDouble(digits[i].Value), Convert.ToDouble(digits[i + 1].Value)));
      }

      return pointCollection;
   }

   private XmlReaderSettings SetXmlReaderSetting()
   {
      XmlReaderSettings settings = new XmlReaderSettings();
      settings.DtdProcessing = DtdProcessing.Parse;
      settings.MaxCharactersFromEntities = 1024;
      return settings;
   }

   private bool CheckIsIn(bool[] set)
   {
      for (int i = 0; i < set.GetLength(0); i++)
      {
         if (!set[i]) return false;
      }

      return true;
   }

   #endregion

   private String HexConverter(Color c)
   {
      return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
   }
}