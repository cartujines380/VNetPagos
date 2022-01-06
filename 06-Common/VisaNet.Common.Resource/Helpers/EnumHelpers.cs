using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web.Mvc;

namespace VisaNet.Common.Resource.Helpers
{
    public static class EnumHelpers
    {
        public static IEnumerable<SelectListItem> ConvertToSelectList(Type source, ResourceManager resourceManager)
        {
            return (from int value in Enum.GetValues(source)
                    select new SelectListItem
                    {
                        Text = GetName(source, value, resourceManager),
                        Value = value.ToString()
                    }).ToList();
        }
        public static IEnumerable<SelectListItem> ConvertToSelectListSorted(Type source, ResourceManager resourceManager)
        {
            return (from int value in Enum.GetValues(source)
                    select new SelectListItem
                    {
                        Text = GetName(source, value, resourceManager),
                        Value = value.ToString()
                    })
                    .OrderBy(i => i.Text)
                    .ToList();
        }

        //public static IEnumerable<SelectListItem> ConvertToSelectList(Type source, int selectedValue, ResourceManager resourceManager)
        //{
        //    return (from int value in Enum.GetValues(source)
        //            select new SelectListItem
        //            {
        //                Text = GetName(source, value, resourceManager),
        //                Value = value.ToString(),
        //                Selected = (value == selectedValue)
        //            }).ToList();
        //}

        public static SelectList ConvertToSelectList(Type source, int selectedValue, ResourceManager resourceManager)
        {
            var lst = ConvertToSelectList(source, resourceManager).ToList();
            return new SelectList(lst, "Value", "Text", selectedValue.ToString());
        }

        public static string GetName(Type source, int value, ResourceManager resourceManager)
        {
            return resourceManager.GetString(string.Format("{0}_{1}", source.Name, Enum.GetName(source, value)));
        }

        public static List<int> AllItems(Type source)
        {
            return (from int value in Enum.GetValues(source)
                    select value).ToList();
        }

        public static Dictionary<string, int> ConvertToDictionary(Type source, ResourceManager resourceManager)
        {
            var dictionary = new Dictionary<string, int>();
            foreach (int value in Enum.GetValues(source))
            {
                dictionary.Add(GetName(source, value, resourceManager), value);
            }
            
           return dictionary;
        }
    }
}
