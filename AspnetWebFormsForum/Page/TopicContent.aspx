<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TopicContent.aspx.cs" Inherits="AspNetWebFormsForum.WebForm2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>文章內容</title>
    <meta charset="utf-8" />
    <style>
        body {
            font-family: Segoe UI,Arial
        }

        .wrap {
            max-width: 1000px;
            margin: 10px auto
        }

        .title {
            font-size: 20px;
            font-weight: 700;
            margin: 8px 0
        }

        .meta {
            color: #666;
            margin-bottom: 12px
        }

        .content {
            line-height: 1.6;
            border: 1px solid #ddd;
            padding: 12px
        }

        ul {
            list-style: none;
            padding: 0
        }

        li {
            border-bottom: 1px solid #eee;
            padding: 10px 0
        }

        .small {
            color: #888;
            font-size: 12px
        }

        textarea {
            width: 100%;
            height: 120px
        }

        .topbar {
            margin-bottom: 8px
        }

        a {
            color: #0645ad;
            text-decoration: none
        }

            a:hover {
                text-decoration: underline
            }

        .warn {
            color: #c00
        }

        .btns {
            float: right
        }

        .row {
            margin: 8px 0
        }

        input[type=text] {
            width: 100%
        }

        .hidden {
            display: none
        }

        li.reply {
            position: relative;
        }
        /* 讓每一筆回覆成為定位容器 */
        .btn-del {
            position: absolute;
            right: 10px;
            top: 8px;
        }
        /* 讓刪除鍵靠右 */
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="wrap">
            <div class="topbar">
                <asp:HyperLink runat="server" NavigateUrl="~/Page/TopicList.aspx" Text="&laquo; 回主題列表" />
                <span class="btns">
                    <!-- 僅作者可見 -->
                    <asp:Button ID="btnEdit" runat="server" Text="編輯文章" OnClick="btnEdit_Click" Visible="false" />
                    <asp:Button ID="btnDelete" runat="server" Text="刪除文章" OnClick="btnDelete_Click" Visible="false"
                        OnClientClick="return confirm('確定要刪除這篇文章（含所有回覆）？');" />
                </span>
                <span style="float: right; margin-right: 10px;">
                    <asp:Literal ID="litUser" runat="server" /></span>
            </div>

            <!-- 閱讀模式 -->
            <asp:Panel ID="pnlRead" runat="server">
                <div class="title">
                    <asp:Literal ID="litTitle" runat="server" />
                </div>
                <div class="meta">
                    <asp:Literal ID="litMeta" runat="server" />
                </div>
                <div class="content">
                    <asp:Literal ID="litContent" runat="server" />
                </div>
            </asp:Panel>

            <!-- 編輯模式（只有作者能看到） -->
            <asp:Panel ID="pnlEdit" runat="server" CssClass="hidden">
                <div class="row">標題：<asp:TextBox ID="txtEditTitle" runat="server" /></div>
                <div class="row">內容：<asp:TextBox ID="txtEditContent" runat="server" TextMode="MultiLine" Height="220" /></div>
                <asp:Button ID="btnSave" runat="server" Text="儲存修改" OnClick="btnSave_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="取消" OnClick="btnCancel_Click" CausesValidation="false" />
                <span class="warn">
                    <asp:Literal ID="litEditMsg" runat="server" /></span>
            </asp:Panel>

            <h4>回覆</h4>
            <%--<asp:Repeater ID="rptReplies" runat="server">
                <ItemTemplate>
                    <li>
                        <div class="content"><%# Eval("Content") %></div>
                        <div class="small">by <%# Eval("UserName") %> ・ <%# string.Format("{0:yyyy/M/d tt hh:mm:ss}", Eval("CreatedAt")) %></div>
                    </li>
                </ItemTemplate>
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>--%>
            <%--<asp:Repeater ID="rptReplies" runat="server" OnItemCommand="rptReplies_ItemCommand">
                <ItemTemplate>
                    <li style="position: "position:relative">
                        <div class="content"><%# Eval("Content") %></div>
                        <div class="small">by <%# Eval("UserName") %> ・ <%# string.Format("{0:yyyy/M/d tt hh:mm:ss}", Eval("CreatedAt")) %></div>

                        <!-- 只有發文者可見的刪除回覆按鈕（靠右） -->
                        <asp:Button ID="btnDelReply" runat="server" Text="刪除回覆"
                            CssClass="btn-del" CausesValidation="false"
                            Visible="<%# IsOwner %>"
                            CommandName="DelReply"
                            CommandArgument='<%# Eval("ReplyID") %>'
                            OnClientClick="return confirm('確定要刪除此回覆？');"
                            Style="position: absolute; right: 10px; top: 8px;" />
                          

                    </li>
                </ItemTemplate>
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>--%>
            <asp:Repeater ID="rptReplies" runat="server" OnItemCommand="rptReplies_ItemCommand">
                <ItemTemplate>
                    <li class="reply">
                        <div class="content"><%# Eval("Content") %></div>
                        <div class="small">
                            by <%# Eval("UserName") %> ・ <%# string.Format("{0:yyyy/M/d tt hh:mm:ss}", Eval("CreatedAt")) %>
                        </div>

                        <asp:Button ID="btnDelReply" runat="server" Text="刪除回覆"
                            CssClass="btn-del" CausesValidation="false"
                            CommandName="DelReply"
                            CommandArgument='<%# Eval("ReplyID") %>'
                            Visible='<%# CanDeleteReply(Eval("ReplyUserID")) %>'
                            OnClientClick="return confirm('確定要刪除此回覆？');" />
                    </li>
                </ItemTemplate>
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>

            <h4>新增回覆</h4>
            <asp:Panel ID="pnlReply" runat="server">
                <asp:TextBox ID="txtReply" runat="server" TextMode="MultiLine"></asp:TextBox><br />
                <asp:Button ID="btnAddReply" runat="server" Text="送出回覆" OnClick="btnAddReply_Click" />
                <span class="warn">
                    <asp:Literal ID="litMsg" runat="server" /></span>
            </asp:Panel>
            <asp:Panel ID="pnlNeedLogin" runat="server" Visible="false" CssClass="warn">
                需要 <a href="<%= ResolveUrl("~/Page/Login.aspx") %>">登入</a> 才能留言
            </asp:Panel>

            <!-- 用來在 PostBack 間記住作者 UserID -->
            <asp:HiddenField ID="hidOwnerId" runat="server" />
        </div>
    </form>
</body>
</html>
