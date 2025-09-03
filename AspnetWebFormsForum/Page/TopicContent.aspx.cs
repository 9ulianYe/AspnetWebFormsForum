using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace AspNetWebFormsForum
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["ForumDb"].ConnectionString;

        private int TopicID
        {
            get { int id; return int.TryParse(Request.QueryString["ID"], out id) ? id : 0; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (TopicID == 0) Response.Redirect("~/20250829_demo/TopicList.aspx");
            if (!IsPostBack)
            {
                UpdateAuthUI();
                LoadTopic();
                LoadReplies();
                ShowOwnerButtons();
            }
        }

        private void UpdateAuthUI()
        {
            if (Session["UserID"] == null)
            {
                litUser.Text = "訪客";
                pnlReply.Visible = false;
                pnlNeedLogin.Visible = true;
            }
            else
            {
                litUser.Text = "您好，" + (Session["UserName"] as string);
                pnlReply.Visible = true;
                pnlNeedLogin.Visible = false;
            }
        }

        private void LoadTopic()
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT t.Title, t.Content, t.CreatedAt, u.UserName, t.UserID
FROM Topics t INNER JOIN Users u ON u.UserID = t.UserID
WHERE t.TopicID=@id";
                cmd.Parameters.AddWithValue("@id", TopicID);
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) { Response.Redirect("~/20250829_demo/TopicList.aspx"); return; }

                    var title = r.GetString(0);
                    var content = r.GetString(1);
                    var created = r.GetDateTime(2);
                    var author = r.GetString(3);
                    var ownerId = r.GetInt32(4);

                    // 顯示
                    litTitle.Text = HttpUtility.HtmlEncode(title);
                    litContent.Text = HttpUtility.HtmlEncode(content);
                    litMeta.Text = "作者：" + HttpUtility.HtmlEncode(author) +
                                   " ・ 建立時間：" + created.ToString("yyyy/M/d tt hh:mm:ss");

                    // 給編輯模式帶初值 + 記住作者ID
                    txtEditTitle.Text = title;
                    txtEditContent.Text = content;
                    hidOwnerId.Value = ownerId.ToString();
                }
            }
        }

        private void LoadReplies()
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT r.ReplyID, r.Content, r.CreatedAt, u.UserName
                    ,r.UserID   AS ReplyUserID
                    FROM Replies r INNER JOIN Users u ON u.UserID = r.UserID
                    WHERE r.TopicID=@id
                    ORDER BY r.CreatedAt ASC";
                cmd.Parameters.AddWithValue("@id", TopicID);

                var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                rptReplies.DataSource = dt;
                rptReplies.DataBind();
            }
        }

        private void ShowOwnerButtons()
        {
            btnEdit.Visible = btnDelete.Visible = false;
            if (Session["UserID"] == null) return;

            int ownerId;
            if (int.TryParse(hidOwnerId.Value, out ownerId))
            {
                if ((int)Session["UserID"] == ownerId)
                {
                    btnEdit.Visible = true;
                    btnDelete.Visible = true;
                }
            }
        }

        /* --- 新增：編輯 / 儲存 / 取消 / 刪除  --- */

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            // 切換到編輯模式
            pnlRead.CssClass = "hidden";
            pnlEdit.CssClass = ""; // 顯示
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // 取消編輯 → 回到閱讀模式
            pnlEdit.CssClass = "hidden";
            pnlRead.CssClass = "";
            litEditMsg.Text = "";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] == null) { litEditMsg.Text = "請先登入"; return; }

            int uid = (int)Session["UserID"];
            int ownerId;
            if (!int.TryParse(hidOwnerId.Value, out ownerId) || ownerId != uid)
            {
                litEditMsg.Text = "只有作者可以編輯此文章";
                return;
            }

            var newTitle = (txtEditTitle.Text ?? "").Trim();
            var newContent = (txtEditContent.Text ?? "").Trim();
            if (newTitle.Length == 0 || newContent.Length == 0)
            {
                litEditMsg.Text = "標題與內容不可空白";
                return;
            }

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
UPDATE Topics
SET Title=@t, Content=@c
WHERE TopicID=@id AND UserID=@uid;";
                cmd.Parameters.AddWithValue("@t", newTitle);
                cmd.Parameters.AddWithValue("@c", newContent);
                cmd.Parameters.AddWithValue("@id", TopicID);
                cmd.Parameters.AddWithValue("@uid", uid);
                conn.Open();
                int n = cmd.ExecuteNonQuery();
                if (n == 0)
                {
                    litEditMsg.Text = "更新失敗（可能不是作者或文章不存在）";
                    return;
                }
            }

            // 更新成功 → 回到閱讀模式並重新載入
            pnlEdit.CssClass = "hidden";
            pnlRead.CssClass = "";
            LoadTopic();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] == null) return;

            int uid = (int)Session["UserID"];
            int ownerId;
            if (!int.TryParse(hidOwnerId.Value, out ownerId) || ownerId != uid)
            {
                // 非作者，不處理
                return;
            }

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                var tx = conn.BeginTransaction();
                cmd.Transaction = tx;

                try
                {
                    // 先刪回覆（若你有設定 FK ON DELETE CASCADE，可以省略這一步）
                    cmd.CommandText = "DELETE FROM Replies WHERE TopicID=@id;";
                    cmd.Parameters.AddWithValue("@id", TopicID);
                    cmd.ExecuteNonQuery();

                    // 再刪主題（限制必須作者本人）
                    cmd.Parameters.Clear();
                    cmd.CommandText = "DELETE FROM Topics WHERE TopicID=@id AND UserID=@uid;";
                    cmd.Parameters.AddWithValue("@id", TopicID);
                    cmd.Parameters.AddWithValue("@uid", uid);
                    int n = cmd.ExecuteNonQuery();

                    if (n == 0) { tx.Rollback(); return; }
                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }

            // 刪完導回列表
            Response.Redirect("~/20250829_demo/TopicList.aspx");
        }

        /* --- 既有：新增回覆 --- */
        protected void btnAddReply_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] == null) { litMsg.Text = "請先登入"; return; }
            var uid = (int)Session["UserID"];
            var content = (txtReply.Text ?? "").Trim();
            if (content.Length == 0) { litMsg.Text = "留言不可空白"; return; }

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Replies(TopicID, UserID, Content) VALUES(@tid, @uid, @ct)";
                cmd.Parameters.AddWithValue("@tid", TopicID);
                cmd.Parameters.AddWithValue("@uid", uid);
                cmd.Parameters.AddWithValue("@ct", content);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            txtReply.Text = "";
            LoadReplies();
        }

        // 判斷目前登入者是否為這篇文章作者（供 aspx 綁定 Visible 使用）
        protected bool IsOwner
        {
            get
            {
                if (Session["UserID"] == null) return false;
                int ownerId;
                return int.TryParse(hidOwnerId.Value, out ownerId) && (int)Session["UserID"] == ownerId;
            }
        }

        // Repeater 的指令事件：處理刪除回覆

        //        protected void rptReplies_ItemCommand(object source, RepeaterCommandEventArgs e)
        //        {
        //            if (e.CommandName == "DelReply")
        //            {
        //                // 只有發文者可刪
        //                if (!IsOwner) return;

        //                int rid;
        //                if (!int.TryParse(Convert.ToString(e.CommandArgument), out rid)) return;

        //                using (var conn = new SqlConnection(ConnStr))
        //                using (var cmd = conn.CreateCommand())
        //                {
        //                    // 雙重保護：只能刪「此文章」的回覆，且此文章必須是目前使用者的
        //                    cmd.CommandText = @"
        //DELETE r
        //FROM Replies r
        //JOIN Topics t ON t.TopicID = r.TopicID
        //WHERE r.ReplyID = @rid
        //  AND r.TopicID = @tid
        //  AND t.UserID = @uid;";
        //                    cmd.Parameters.AddWithValue("@rid", rid);
        //                    cmd.Parameters.AddWithValue("@tid", TopicID);
        //                    cmd.Parameters.AddWithValue("@uid", (int)Session["UserID"]);
        //                    conn.Open();
        //                    cmd.ExecuteNonQuery();
        //                }

        //                // 重新載入回覆列表
        //                LoadReplies();
        //            }
        //        }
        protected void rptReplies_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "DelReply") return;
            if (Session["UserID"] == null) return;

            int rid; if (!int.TryParse(Convert.ToString(e.CommandArgument), out rid)) return;
            int uid = (int)Session["UserID"];

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
DELETE r
FROM Replies r
JOIN Topics t ON t.TopicID = r.TopicID
WHERE r.ReplyID = @rid
  AND r.TopicID = @tid
  AND (t.UserID = @uid OR r.UserID = @uid);";
                cmd.Parameters.AddWithValue("@rid", rid);
                cmd.Parameters.AddWithValue("@tid", TopicID);
                cmd.Parameters.AddWithValue("@uid", uid);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            LoadReplies();
        }


        protected bool CanDeleteReply(object replyUserIdObj)
        {
            if (Session["UserID"] == null) return false;
            int current = (int)Session["UserID"];

            // 文章作者
            int ownerId;
            bool isOwner = int.TryParse(hidOwnerId.Value, out ownerId) && ownerId == current;

            // 回覆作者
            int replyUserId;
            bool isReplyAuthor = int.TryParse(Convert.ToString(replyUserIdObj), out replyUserId) && replyUserId == current;

            return isOwner || isReplyAuthor;
        }
    }
}