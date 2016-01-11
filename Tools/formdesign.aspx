﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="formdesign.aspx.cs" Inherits="Tools.formdesign" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>表单设计器</title>

    <!-- CSS -->
    <link href="style/css/transdmin.css" rel="stylesheet" type="text/css" media="screen" />
    <!--[if IE 6]><link rel="stylesheet" type="text/css" media="screen" href="style/css/ie6.css" /><![endif]-->
    <!--[if IE 7]><link rel="stylesheet" type="text/css" media="screen" href="style/css/ie7.css" /><![endif]-->
    <!-- JavaScripts-->
    <script type="text/javascript" src="js/jquery1.7.2.js"></script>
    <script type="text/javascript" src="style/js/jNice.js"></script>    
</head>
<body>
    <div id="wrapper" style="width:100%;">
        <div id="main" style="width:100%; padding-right:0;">
            <form action="" class="jNice" id="myform">
                <table cellpadding="0" cellspacing="0" style="width:100%;" class="table">
                    <tbody></tbody>
                </table>
            </form>
        </div>
        <!-- // #main -->
    </div>
    <!-- // #wrapper -->

    <!-- // #template -->
    <script id="template" type="x-tmpl-mustache">
        {{#data}}
        <tr>
            <td width="120">{{key}}</td>
            <td>{{value}}</td>
            <td class="action">
                <a href="javascript:void(0);" ccvalue="{{key}}" class="delete">Delete</a>
            </td>
        </tr>
        {{/data}}
    </script>

    <script type="text/javascript">
        $(function () {
            var index = parent.layer.getFrameIndex(window.name);

            parent.renderTable(
                { action: "getcodelib" },
                $('.table tbody'),
                $('#template').html(),
                function () { $('tr:even').addClass('odd') }
            );

            $('.table').delegate('tr', 'dblclick', function (e) {
                var tb = $(this).find('td:first').text();
                var fd = $(this).find('td:eq(1)').text();
                parent.OpeniframeLayer('生成代码', 'code.aspx?tb=' + tb + '&fd=' + fd, ['900px', '500px'], true, true, false);
                parent.layer.close(index);
            });

            $('.table').delegate('.delete', 'click', function (e) {
                e.stopPropagation();

                var _target = $(this).parent().parent();
                var paramdata = {
                    action: "deletecodelib",
                    arg: { tb: $(this).attr('ccvalue') }
                };
                parent.doAjaxPost(paramdata, function (result) {
                    if (result.success) {
                        _target.fadeOut();
                    } else {
                        parent.SuperSite.MsgError(result.msg);
                    }
                });
            });

        });
    </script>

    <!---->
</body>
</html>
