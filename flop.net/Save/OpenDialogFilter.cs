using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using flop.net.Enums;

namespace flop.net.Save
{
   public static class OpenDialogFilter
   {
      public static string GetFilter()
      {
         string filter = "All files (*.*)|*.*|"; //Можно оставить пустым, но тогда придётся каждый раз выбирать именно нужный формат
         var values = Enum.GetValues(typeof(OpenTypes));
         foreach (var value in values)
         {
            filter += $"{value.ToString().ToLower()} files (*.{value.ToString().ToLower()})|*.{value.ToString().ToLower()}|";
         }
         filter = filter.Remove(filter.Length - 1);
         return filter;
      }
   }
}
