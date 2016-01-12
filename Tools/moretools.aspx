<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="moretools.aspx.cs" Inherits="Tools.moretools" %>

<!DOCTYPE html>
<html>
<head>
    <title>工具集合</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="style/css/selecttb.css" rel="stylesheet" />
    <style type="text/css">
        a{ color:#333; text-decoration:none;}
    </style>
</head>

<body>
    <div class="container">
        <div id="myform" class="box mt10">
            <ul>
                <li><a href="json.html" target="_blank">json</a></li>
                <li><a href="tools/sql_datetime.html" target="_blank">sql datetime</a></li>
            </ul>
        </div>
    </div>
    <br style="clear:both;" />

    <script src="js/jquery1.7.2.js"></script>
    <script type="text/javascript">
        $(function () {
            $('#myform').delegate('li', 'mouseover', function (e) {
                $(this).addClass('liselectlang').siblings().removeClass('liselectlang');
            });
        });
    </script>

</body>
</html>
