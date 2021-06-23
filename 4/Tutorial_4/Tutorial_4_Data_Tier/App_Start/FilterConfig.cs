using System.Web;
using System.Web.Mvc;

namespace Tutorial_4_Data_Tier
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
