<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TopicList.aspx.cs" Inherits="AspNetWebFormsForum.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>論壇 - 主題列表</title>
    <meta charset="utf-8" />
    <style>
        

        .author {
            font-size: 12px;
            color: #999;
            margin-left: 20px;
        }

        body {
            font-family: Segoe UI,Arial
        }

        .toolbar {
            margin: 10px 0
        }

        table {
            border-collapse: collapse;
            width: 100%
        }

        th, td {
            border: 1px solid #ccc;
            padding: 8px
        }

        th {
            background: #f5f5f5
        }

        .right {
            float: right
        }

        a {
            color: #0645ad;
            text-decoration: none
        }

            a:hover {
                text-decoration: underline
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="toolbar">
            搜尋：<asp:TextBox ID="txtKeyword" runat="server" Width="260" />
            <asp:Button ID="btnSearch" runat="server" Text="送出" OnClick="btnSearch_Click" />
            &nbsp;&nbsp;<asp:HyperLink ID="lnkPost" runat="server" NavigateUrl="~/Page/TopicPost.aspx">發表主題</asp:HyperLink>
            <span class="right">
                <asp:Literal ID="litHello" runat="server" />
                &nbsp;
           
                <asp:HyperLink ID="lnkLogin" runat="server" NavigateUrl="~/Page/Login.aspx">登入</asp:HyperLink>
                <asp:LinkButton ID="btnLogout" runat="server" Text="登出" OnClick="btnLogout_Click" Visible="false" />
            </span>
        </div>
        <asp:GridView ID="gvTopics" runat="server" AutoGenerateColumns="False"
            AllowPaging="True" PageSize="20"
            OnPageIndexChanging="gvTopics_PageIndexChanging">
            <Columns>
                <asp:TemplateField HeaderText="標題">
                    <ItemTemplate>
                        <a href='<%# ResolveUrl("~/Page/TopicContent.aspx?ID=" + Eval("TopicID")) %>'>
                            <%# Eval("Title") %>
                         </a> 
                        <%--<div class="author"><%# Eval("UserName") %></div>--%>
                        <span class="author"> — <%# Eval("UserName") %></span>
                    </ItemTemplate>

                    <%-- <ItemTemplate>
                        <asp:HyperLink runat="server"
                            NavigateUrl='<%# "~/20250829_demo/TopicContent.aspx?ID=" + Eval("TopicID") %>'
                            Text='<%# Eval("Title") %>' />
                    </ItemTemplate>--%>
                </asp:TemplateField>
                <asp:BoundField DataField="CreatedAt" HeaderText="建立時間"
                    DataFormatString="{0:yyyy/M/d tt hh:mm:ss}" HtmlEncode="False" />
                <asp:BoundField DataField="ReplyCount" HeaderText="回覆筆數" />
            </Columns>
        </asp:GridView>
    </form>
</body>
</html>
