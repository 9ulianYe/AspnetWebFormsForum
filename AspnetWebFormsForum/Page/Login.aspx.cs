using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace AspNetWebFormsForum
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private string ConnStr => ConfigurationManager.ConnectionStrings["ForumDb"].ConnectionString;
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            var user = (txtUser.Text ?? "").Trim();
            var pwd = (txtPwd.Text ?? "").Trim();
            if (user.Length == 0 || pwd.Length == 0) { litMsg.Text = "請輸入帳號與密碼"; return; }

            var hash = Sha256Hex(pwd); // 與資料庫一致的 SHA256
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT UserID FROM Users WHERE UserName=@u AND PasswordHash=@p";
                cmd.Parameters.AddWithValue("@u", user);
                cmd.Parameters.AddWithValue("@p", hash);
                conn.Open();
                var obj = cmd.ExecuteScalar();
                if (obj == null) { litMsg.Text = "帳號或密碼錯誤"; return; }

                Session["UserID"] = (int)obj;
                Session["UserName"] = user;
                Response.Redirect("~/Page/TopicList.aspx");
            }
        }

        private static string Sha256Hex(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder();
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

    }
}