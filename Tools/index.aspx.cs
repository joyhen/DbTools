using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections.Generic;

namespace Tools
{
    using Help;

    public partial class index : System.Web.UI.Page
    {
        protected string connectionstring = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var kv = BufHelp.ProtoBufDeserialize<KeyValue>(KeyCenter.DBConfigFile);
                if (kv != null && kv.key == KeyCenter.DBKey)
                    connectionstring = kv.value;
                else
                    connectionstring = ConfigurationManager.ConnectionStrings[KeyCenter.DBKey].ToString();
            }
        }
    }
}