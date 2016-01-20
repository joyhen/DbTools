<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="formdesign.aspx.cs" Inherits="Tools.formdesign" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>表单设计器</title>
    <!-- CSS -->
    <link href="style/css/transdmin.css" rel="stylesheet" type="text/css" media="screen" />
    <link href="js/poshytip-1.2/tip-twitter/tip-twitter.css" rel="stylesheet" />
    <!--[if IE 6]><link rel="stylesheet" type="text/css" media="screen" href="style/css/ie6.css" /><![endif]-->
    <!--[if IE 7]><link rel="stylesheet" type="text/css" media="screen" href="style/css/ie7.css" /><![endif]-->
    <script type="text/javascript" src="js/jquery1.7.2.js"></script>
    <script type="text/javascript" src="style/js/jNice.js"></script>
    <style>
        .bj{}
        .bj a{ font-size:12px; color:#9F9F9F; text-decoration:none; margin-right:5px; margin-left:5px;}
        .bj a:hover{ text-decoration:none;}

        #sortTrue p {margin:5px 0px; border:1px solid #fff;}
        #sortTrue table{border:1px solid #fff;}
        #sortTrue .p_hover{ border:1px dashed #ff6a00; cursor:pointer;}
    </style>
</head>

<body>
	<div id="wrapper">
        <div id="containerHolder">
			<div id="container">
        		<div id="sidebar" style="min-height:500px;">
                    <ul class="sideNav">
                        <li><a href="javascript:void(0);" id="txt">文本框</a></li>
                        <li><a href="javascript:void(0);" id="are">文本域</a></li>
                        <li><a href="javascript:void(0);" id="rad">单选</a></li>
                        <li><a href="javascript:void(0);" id="chk">多选</a></li>
                        <li><a href="javascript:void(0);" id="slt">下拉列表</a></li>
                        <li><a href="javascript:void(0);" id="btn">按钮</a></li>
                        <li><a href="javascript:void(0);" id="tbl">表格</a></li>
                    </ul>
                </div>
                <!-- // #sidebar -->
                <h2 class="bj"><a href="javascript:void(0);" class="ignore">布局设置</a> &raquo; 
                    <a href="javascript:void(0);" id="currenttb" class="active">1:null</a>
                    <a href="javascript:void(0);">2:2</a>
                    <a href="javascript:void(0);">1:3</a>
                    <a href="javascript:void(0);">3:1</a>
                    <a href="javascript:void(0);">1:2:1</a>
                    <a href="javascript:void(0);">2:1:1</a>
                </h2>
                <div id="main">
                	<form action="" class="jNice">
					    <h3 style="font-weight:normal;">辅助工具
                            <span><a title="预览界面" class="toolbar" ccvalue="tinyshowtip" href="javascript:;">@</a></span>
                            <span><a title="生成代码" class="toolbar" ccvalue="codetip" href="javascript:;">$</a></span>
                        </h3>
                        <div id="sortTrue">
                           
                        </div>
                    </form>
                </div>
                <!-- // #main -->
                <div class="clear"></div>
            </div>
            <!-- // #container -->
        </div>
        <%-- // #containerHolder--%>
        <p id="footer" style="margin:0; line-height:40px;">This project was create by Jon. QQ:573741776</p>
    </div>
    <!-- // #wrapper -->

    <script id="tpt_tbl" type="text/template">
        <table id="{{id}}" cctype="table" cellpadding="0" cellspacing="0" class="table">
            <tr>
                <th>字段</th>
                <th>字段</th>
                <th>字段</th>
                <th style="text-align:left;padding-left:20px;">字段</th>
                <th>字段</th>
                <th>字段</th>
                <th>说明</th>
                <th>操作</th>
            </tr>
            <tr>
                <td style="padding:0 10px;">name</td>
                <td style="padding:0 10px;">name</td>
                <td style="padding:0 10px;">name</td>
                <td>name</td>
                <td style="padding:0 10px;">name</td>
                <td>name</td>
                <td class="detail">&raquo;</td><!--&gt;-->
                <td class="action">
                    <a href="javascript:void(0);" class="view">Add</a>
                    <a href="javascript:void(0);" class="edit">Detail</a>
                    <a href="javascript:void(0);" class="delete">Delete</a>
                </td>
            </tr>
        </table>
    </script>

    <script id="tpt_txt" type="text/template">
        <p id="{{id}}" cctype="text">
            <label>标签名：</label>
            <input type="text" readonly="readonly" class="text-long" />
        </p>
    </script>

    <script id="tpt_are" type="text/template">
        <p id="{{id}}" cctype="textarea">
            <label>标签名：</label>
            <textarea name="biz" id="biz" rows="1" cols="1" readonly="readonly"></textarea>
        </p>
    </script>
    <!-- // #template -->

    <script src="js/layer-v2.0/layer/layer.js"></script>
    <script src="js/poshytip-1.2/jquery.poshytip.min.js"></script>
    <script type="text/javascript">
        //获取窗口索引
        var index = parent.layer.getFrameIndex(window.name);
        //工具栏设置
        function settoolbar() {
            var _target = $('.toolbar');

            _target.poshytip({
                className: 'tip-twitter',
                showTimeout: 1,
                alignTo: 'target',
                alignX: 'center',
                offsetY: 5,
                allowTipHover: false,
                fade: false,
                slide: false
            });

            _target.click(function () {
                var _tag = $(this).attr('ccvalue');
                switch (_tag) {
                    default:
                        parent.SuperSite.MsgError('操作无效');
                        break;
                };
            });
        };

        //删除对象
        function removeControl(idx) {
            if (idx) {
                $('#sortTrue #' + idx).remove();
                //让层自适应iframe
                parent.layer.iframeAuto(index);
                layer.msg('删除成功', { icon: 1, time: 1000 });
            }
        };
        //重置控件属性
        function resetControl(idx, arg) {
            if (idx && arg) {
                var $dom = $('#sortTrue #' + idx);

                if (arg.__lbl) {
                    $dom.find('label').text(arg.__lbl + ":");
                };
                if (arg.__id || arg.__name) {
                    $dom.find('input').attr({ id: arg.__id, name: arg.__name });
                };

                layer.msg('更新成功', { icon: 1, time: 1000 });
            }
        };        

        $(function () {

            //工具栏设置
            settoolbar();

            //选择布局
            $('.bj a').click(function () {
                if (!$(this).hasClass('ignore')) {
                    $(this).addClass('active').siblings().removeClass('active');
                }
            });

            //菜单事件
            $('.sideNav').delegate('li a', 'click', function (e) {
                e.stopPropagation();

                $(this).addClass('active');
                $(this).parent().siblings().find('a').removeClass('active');

                var tagid = (new Date().getTime()).toString();

                var template = $('#tpt_' + $(this).attr('id')).html();
                var demo = template.replace('{{id}}', tagid);
                $('#sortTrue').append(demo);

                //让层自适应iframe
                var __h=$('#sortTrue').height();
                if (__h > 390 && __h <= 600) {                    
                    parent.layer.iframeAuto(index);
                };
            });

            //编辑控件
            $('#sortTrue').delegate('p, table', 'mouseenter', function (e) {
                $(this).addClass('p_hover');
            });
            $('#sortTrue').delegate('p, table', 'mouseleave', function (e) {
                $(this).removeClass('p_hover');
            });
            $('#sortTrue').delegate('p, table', 'click', function (e) {
                e.stopPropagation();
                
                var _id = $(this).attr('id');
                var _index = layer.open({
                    type: 2,
                    skin: 'layui-layer-lan',
                    title: '文本框设计',
                    fix: true,
                    maxmin: false,
                    closeBtn: false,
                    shadeClose: true,
                    area: ['400px', '235px'],
                    content: 'formtxt.html?tag=' + _id
                });
            });

            //...

        });
    </script>
</body>
</html>