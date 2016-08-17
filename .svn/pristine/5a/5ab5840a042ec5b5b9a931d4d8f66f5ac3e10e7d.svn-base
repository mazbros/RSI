using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace RSI.Helpers
{
    public class DropDownHelper
    {
        public static IEnumerable<SelectListItem> ToSelectListItems(List<string> items, int selectedId = -1)
        {
            return
                items.Select(s =>
                    new SelectListItem
                    {
                        Text = s.ToString(),
                        Value = items.IndexOf(s).ToString(),
                        Selected = selectedId == items.IndexOf(s)
                    });
        }
    }
}