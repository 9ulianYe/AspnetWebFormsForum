using System;
using System.Web.UI;

namespace AspnetWebFormsForum
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/Page/TopicList.aspx", true);
        }
    }
}