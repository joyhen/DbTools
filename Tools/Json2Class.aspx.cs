using System;

namespace Tools
{
    using Help;

    public partial class Json2Class : System.Web.UI.Page
    {
        protected string coderesult = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var data = CacheUtil.GetCache(KeyCenter.GenJsonClass);
                if (data != null) coderesult = data.ToString();
            }
        }


    }
}