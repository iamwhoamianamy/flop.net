using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

using flop.net.Model;

namespace flop.net.Save
{
   public static class JsonSaver
   {
      public static string filePath { get; set; }
      public static string fileName { get; set; }

      public static void SaveGeometricToJson(IGeometric geometric)
      {
         Directory.CreateDirectory(filePath);
         var settings = new JsonSerializerSettings();
         settings.TypeNameHandling = TypeNameHandling.Objects;
         string jsonString = JsonConvert.SerializeObject(geometric, Formatting.Indented, settings);
         File.WriteAllText(filePath + fileName, jsonString);
      }

      public static IGeometric RestoreGeometricFromJson()
      {
         string jsonString = File.ReadAllText(filePath + fileName);
         var settings = new JsonSerializerSettings();
         settings.TypeNameHandling = TypeNameHandling.Objects;
         IGeometric geometric = JsonConvert.DeserializeObject<IGeometric>(jsonString, settings);
         return geometric;
      }  
   }
}
