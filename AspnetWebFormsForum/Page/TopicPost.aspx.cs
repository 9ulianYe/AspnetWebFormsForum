using System;
using System.Configuration;
using System.Data.SqlClient;

namespace AspNetWebFormsForum
{
    public partial class WebForm3 : System.Web.UI.Page
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["ForumDb"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var isLogin = Session["UserID"] != null;
                pnlForm.Visible = isLogin;
                pnlNeedLogin.Visible = !isLogin;
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] == null) { litMsg.Text = "請先登入"; return; }

            var title = (txtTitle.Text ?? "").Trim();
            var content = (txtContent.Text ?? "").Trim();
            if (title.Length == 0 || content.Length == 0)
            {
                litMsg.Text = "標題與內容不可空白";
                return;
            }

            int newId;
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO Topics(Title, Content, UserID)
OUTPUT INSERTED.TopicID
VALUES(@t, @c, @uid);";
                cmd.Parameters.AddWithValue("@t", title);
                cmd.Parameters.AddWithValue("@c", content);
                cmd.Parameters.AddWithValue("@uid", (int)Session["UserID"]);
                conn.Open();
                newId = (int)cmd.ExecuteScalar();
            }

            Response.Redirect("~/Page/TopicContent.aspx?ID=" + newId);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtTitle.Text = "";
            txtContent.Text = "";
        }
    }
}