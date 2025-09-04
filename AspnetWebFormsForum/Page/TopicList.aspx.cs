using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AspNetWebFormsForum
{

    public partial class WebForm1 : System.Web.UI.Page
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["ForumDb"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateAuthUI();
                BindGrid();
            }
        }
        private void UpdateAuthUI()
        {
            if (Session["UserID"] == null)
            {
                litHello.Text = "訪客";
                lnkLogin.Visible = true;
                btnLogout.Visible = false;
            }
            else
            {
                litHello.Text = "您好，" + (Session["UserName"] as string);
                lnkLogin.Visible = false;
                btnLogout.Visible = true;
            }
        }

        private void BindGrid(string keyword = "")
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                SELECT
                    t.TopicID,
                    t.Title,
                    t.CreatedAt,
                    u.UserName,
                    COUNT(r.ReplyID) AS ReplyCount
                FROM Topics t
                INNER JOIN Users u ON u.UserID = t.UserID      -- ← 把作者帳號撈出來
                LEFT JOIN Replies r ON r.TopicID = t.TopicID   -- ← 計算回覆數
                WHERE (@kw = N'' OR t.Title LIKE N'%' + @kw + N'%' OR t.Content LIKE N'%' + @kw + N'%')
                GROUP BY t.TopicID, t.Title, t.CreatedAt, u.UserName
                ORDER BY t.CreatedAt DESC;";

                cmd.Parameters.Add("@kw", SqlDbType.NVarChar, 200).Value = keyword ?? string.Empty;

                var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);

                gvTopics.DataSource = dt;
                gvTopics.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid(txtKeyword.Text.Trim());
        }

        protected void gvTopics_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            gvTopics.PageIndex = e.NewPageIndex;
            BindGrid(txtKeyword.Text.Trim());
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear(); Session.Abandon();
            UpdateAuthUI();
            BindGrid(txtKeyword.Text.Trim());
        }
    }
}