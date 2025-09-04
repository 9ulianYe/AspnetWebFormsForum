<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="AspNetWebFormsForum.WebForm4" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>註冊</title>
    <meta charset="utf-8" />
    <style>
        body {
            font-family: Segoe UI,Arial
        }

        .wrap {
            max-width: 520px;
            margin: 30px auto;
            border: 1px solid #ddd;
            padding: 16px;
            border-radius: 8px
        }

        .row {
            margin: 8px 0
        }

        input[type=text], input[type=password], input[type=email] {
            width: 100%
        }

        .msg {
            margin-top: 10px
        }

        .err {
            color: #c00
        }

        .ok {
            color: #060
        }
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <div class="wrap">
            <h3>註冊新帳號</h3>

            <div class="row">
                姓名：
       
                <asp:TextBox ID="txtFullName" runat="server" />
            </div>
            <div class="row">
                帳號：
       
                <asp:TextBox ID="txtUserName" runat="server" />
            </div>
            <div class="row">
                密碼：
       
                <asp:TextBox ID="txtPassword" TextMode="Password" runat="server" />
            </div>
            <div class="row">
                Email：
       
                <asp:TextBox ID="txtEmail" runat="server" />
            </div>

            <asp:Button ID="btnRegister" runat="server" Text="送出註冊" OnClick="btnRegister_Click" />
            &nbsp;<asp:HyperLink runat="server" NavigateUrl="~/Page/Login.aspx">回登入頁</asp:HyperLink>

            <div class="msg">
                <asp:Literal ID="litMsg" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>
