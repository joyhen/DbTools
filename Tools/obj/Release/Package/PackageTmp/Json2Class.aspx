<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Json2Class.aspx.cs" Inherits="Tools.Json2Class" %>

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
        <code class="cs"><%=coderesult  %></code>
        </pre>
    </form>

    <script src="js/highlight/highlight.pack.js"></script>
    <script>hljs.initHighlightingOnLoad();</script>
</body>
</html>
