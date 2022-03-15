using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

using flop.net.Model;

namespace flop.net.Save
{
   public class JsonSaver
   {
      public JsonSaver(string filePath, string fileName)
      {
         FilePath = filePath;
         FileName = fileName;
      }
      public string FilePath { get; private set; }
      public string FileName { get; private set; }

      public void SaveLayersToJson(Collection<Layer> layers)
      {
         Directory.CreateDirectory(FilePath);
         var settings = new JsonSerializerSettings();
         settings.TypeNameHandling = TypeNameHandling.All;
         settings.Formatting = Formatting.Indented;
         string jsonString = JsonConvert.SerializeObject(layers, settings);
         File.WriteAllText(FilePath + FileName + ".json", jsonString);
      }

      public ObservableCollection<Layer> RestoreLayersFromJson()
      {
         string jsonString = File.ReadAllText(FilePath + FileName + ".json");
         var settings = new JsonSerializerSettings();
         settings.TypeNameHandling = TypeNameHandling.Objects;
         ObservableCollection<Layer> layers = JsonConvert.DeserializeObject<ObservableCollection<Layer>>(jsonString, settings);
         return layers;
      }

      public void CompressJson()
      {
         using (var zip = ZipFile.Open(FilePath + FileName + ".zip", ZipArchiveMode.Create))
         {
            zip.CreateEntryFromFile(FilePath + FileName + ".json", FileName + ".json");
         }
         File.Delete(FilePath + FileName + ".json");
      }

      public void DecompressJson()
      {
         if (File.Exists(FilePath + FileName + ".zip"))
         {
            ZipFile.ExtractToDirectory(FilePath + FileName + ".zip", FilePath);
            File.Delete(FilePath + FileName + ".zip");
         }
      }
   }
}
