<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AspNetWebFormsForum.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>登入</title>
    <meta charset="utf-8" />
     <style>
        body{font-family:Segoe UI,Arial}
        .wrap{max-width:420px;margin:40px auto;border:1px solid #ddd;padding:16px;border-radius:8px}
        .row{margin:10px 0}
        input[type=text],input[type=password]{width:100%}
        .msg{color:#c00;margin-top:10px}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="wrap">
            <h3>登入</h3>
            <div class="row">帳號：<asp:TextBox ID="txtUser" runat="server" /></div>
            <div class="row">密碼：<asp:TextBox ID="txtPwd" TextMode="Password" runat="server" /></div>
            
            <asp:Button ID="btnLogin" runat="server" Text="登入" OnClick="btnLogin_Click" />
            &nbsp;<asp:HyperLink runat="server" NavigateUrl="~/Page/TopicList.aspx">回主題列表</asp:HyperLink>
            &nbsp;<asp:HyperLink ID="lnkRegister" runat="server" NavigateUrl="~/Page/Register.aspx">註冊新帳號</asp:HyperLink>
                <asp:Literal ID="litMsg" runat="server" /></div>
        

        </div>
    </form>
</body>
</html>
