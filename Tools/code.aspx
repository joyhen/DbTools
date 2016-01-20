<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="code.aspx.cs" Inherits="Tools.code" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>代码生成</title>
    <link href="js/highlight/styles.css" rel="stylesheet" />
    <link href="js/highlight/styles/monokai_sublime.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <pre style="background-color: #23241f;">
        <code class="cs"><%=coderesult %></code>
        </pre>
    </form>

    <script src="js/jquery1.7.2.js"></script>
    <script src="js/highlight/highlight.pack.js"></script>
    <script>
        hljs.initHighlightingOnLoad();

        $(function () {
            parent.SuperSite.MsgOK('代码已成功生成');
        });
    </script>
</body>
</html>
