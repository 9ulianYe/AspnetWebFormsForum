using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetWebFormsForum
{
    public partial class WebForm4 : System.Web.UI.Page
    {

        private string ConnStr => ConfigurationManager.ConnectionStrings["ForumDb"].ConnectionString;

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            var fullName = (txtFullName.Text ?? "").Trim();
            var userName = (txtUserName.Text ?? "").Trim();
            var password = (txtPassword.Text ?? "").Trim();
            var email = (txtEmail.Text ?? "").Trim();

            // 1) 基本檢查
            if (fullName.Length == 0 || userName.Length == 0 || password.Length == 0 || email.Length == 0)
            {
                ShowErr("請完整填寫所有欄位");
                return;
            }
            if (!IsValidEmail(email))
            {
                ShowErr("Email 格式不正確");
                return;
            }

            // 2) 檢查帳號 / Email 是否已存在
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT
    SUM(CASE WHEN UserName = @u THEN 1 ELSE 0 END) AS UserNameDup,
    SUM(CASE WHEN Email    = @e THEN 1 ELSE 0 END) AS EmailDup
FROM Users WITH (NOLOCK);";
                cmd.Parameters.AddWithValue("@u", userName);
                cmd.Parameters.AddWithValue("@e", email);

                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        int uDup = r.IsDBNull(0) ? 0 : Convert.ToInt32(r[0]);
                        int eDup = r.IsDBNull(1) ? 0 : Convert.ToInt32(r[1]);
                        if (uDup > 0) { ShowErr("此帳號已被註冊"); return; }
                        if (eDup > 0) { ShowErr("此 Email 已被註冊"); return; }
                    }
                }
            }

            // 3) 寫入資料庫（密碼雜湊）
            var hash = Sha256Hex(password);

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO Users(UserName, PasswordHash, Email, FullName, CreatedAt)
VALUES(@u, @p, @e, @n, GETDATE());";
                cmd.Parameters.AddWithValue("@u", userName);
                cmd.Parameters.AddWithValue("@p", hash);
                cmd.Parameters.AddWithValue("@e", email);
                cmd.Parameters.AddWithValue("@n", fullName);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    // 若已建立唯一索引，這裡可以攔截重複錯誤
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        ShowErr("帳號或 Email 已存在");
                        return;
                    }
                    throw;
                }
            }

            litMsg.Text = "<span class='ok'>註冊成功！請回登入頁登入。</span>";
        }

        private void ShowErr(string msg) => litMsg.Text = $"<span class='err'>{msg}</span>";

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

        private static bool IsValidEmail(string email)
        {
            // 簡易 Email 驗證（夠用即可）
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}