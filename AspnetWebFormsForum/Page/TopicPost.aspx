<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TopicPost.aspx.cs" Inherits="AspNetWebFormsForum.WebForm3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>發表主題</title><meta charset="utf-8" />
    <style>
        body{font-family:Segoe UI,Arial}
        .wrap{max-width:900px;margin:10px auto}
        input[type=text]{width:100%}
        textarea{width:100%;height:250px}
        .row{margin:8px 0}
        .warn{color:#c00}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="wrap">
            <asp:HyperLink runat="server" NavigateUrl="~/Page/TopicList.aspx" Text="&laquo; 回主題列表" /><br />
            <br />
            <asp:Panel ID="pnlForm" runat="server">
                <div class="row">標題：<asp:TextBox ID="txtTitle" runat="server" /></div>
                <div class="row">文章內容：<asp:TextBox ID="txtContent" runat="server" TextMode="MultiLine" /></div>
                <asp:Button ID="btnSubmit" runat="server" Text="送出" OnClick="btnSubmit_Click" />
                <asp:Button ID="btnClear" runat="server" Text="清除" OnClick="btnClear_Click" CausesValidation="false" />
                <span class="warn">
                    <asp:Literal ID="litMsg" runat="server" /></span>
            </asp:Panel>
            <asp:Panel ID="pnlNeedLogin" runat="server" Visible="false" CssClass="warn">
                需先 <a href="<%= ResolveUrl("~/Page/Login.aspx") %>">登入</a>才能發文
            </asp:Panel>
        </div>
    </form>
</body>
</html>
