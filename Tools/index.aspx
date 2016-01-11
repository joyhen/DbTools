<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Tools.index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>开发工具-数据字典</title>
    <!-- CSS -->
    <link href="style/css/transdmin.css" rel="stylesheet" type="text/css" media="screen" />
    <link href="style/css/msg.css" rel="stylesheet" type="text/css" media="screen" />
    <link href="js/poshytip-1.2/tip-violet/tip-violet.css" rel="stylesheet" />
    <link href="js/poshytip-1.2/tip-twitter/tip-twitter.css" rel="stylesheet" />
    <link href="js/poshytip-1.2/tip-yellowsimple/tip-yellowsimple.css" rel="stylesheet" />
    <!--[if IE 6]><link rel="stylesheet" type="text/css" media="screen" href="style/css/ie6.css" /><![endif]-->
    <!--[if IE 7]><link rel="stylesheet" type="text/css" media="screen" href="style/css/ie7.css" /><![endif]-->
    <script type="text/javascript" src="js/jquery1.7.2.js"></script>
    <script type="text/javascript" src="style/js/jNice.js"></script>
</head>

<body>
	<div id="wrapper">
        <h1 style="width:400px;">数据字典开发工具 preview 1.0</h1>
        <ul id="mainNav" style="margin-bottom:15px;">
        	<li><a href="#" class="active">数据库</a></li>
        	<li><a href="#" class="template">工具</a></li>
            <li><a href="#" class="codelib">代码库</a></li>
            <li><a href="#" class="formdesign">表单设计</a></li>
        	<li><a href="#" class="setting">设置</a></li>
        	<li><a href="#" class="help">帮助</a></li>
        	<li class="logout"><a href="#">退出</a></li>
        </ul>
        <!-- // #end mainNav -->

        <div class="connect">
            <!--<input type="text" class="" />-->
            连接字符串：<input type="text" title="双击保存" class="cctext-long w770" value="<%=connectionstring %>" />
        </div>
        
        <div id="containerHolder">
			<div id="container">
        		<div id="sidebar">
                    <ul class="sideNav"></ul>
                </div>
                <!-- // #sidebar -->
                
                <!-- h2 stays for breadcrumbs -->
                <h2><a href="javascript:void(0);">数据库</a> &raquo; <a href="javascript:void(0);" id="currenttb" class="active"></a></h2>
                <div id="main">
                	<form action="" class="jNice">
					    <h3 style="font-weight:normal;">表信息
                            <span><a title="精简模式" class="toolbar" ccvalue="tinyshowtip" href="javascript:;">@</a></span>
                            <span class="tiny">|</span>
                            <span><a title="全选/反选所有字段" class="toolbar" ccvalue="alltip" href="javascript:;">#</a></span>
                            <span><a title="添加字段" class="toolbar" ccvalue="addtip" href="javascript:;">+</a></span>
                            <span><a title="生成代码" class="toolbar" ccvalue="codetip" href="javascript:;">$</a></span>
                            <span><a title="导入代码库" class="toolbar" ccvalue="libtip" href="javascript:;">→</a></span>
                        </h3>
                        <table cellpadding="0" cellspacing="0" class="table">
                            <tr>
                                <th>字段</th>
                                <th>索引</th>
                                <th>主键</th>
                                <th style="text-align:left;padding-left:20px;">类型</th>
                                <!--<th>长度</th>-->
                                <th>可空</th>
                                <th>默认值</th>
                                <th>说明</th>
                                <th>操作</th>
                            </tr>
                            <tr><td colspan="15" id="defauttd" class="td_nodata">没有数据</td></tr>
                        </table>
                    </form>
                </div>
                <!-- // #main -->
                <div class="clear"></div>
            </div>
            <!-- // #container -->
        </div>	
        <!-- // #containerHolder -->
        <br />
        <div class="warning_box">
            1.系统已经过滤掉以【conflict】打头的表，如要全部显示请联系维护人员；2.建议您用谷歌浏览器预览
        </div>
        <br />
        <p id="footer">This project was create by Jon. QQ:573741776</p>
    </div>
    <!-- // #wrapper -->

    <!-- //#template-->
    <script type="text/javascript">
        var CurrentSelectTD = ''; // 当前选中的行字段名
        var globalRequestUrl = 'ajax/requestaction.ashx';
        $.get("template/Tindex.txt", function (result) {
            $("#wrapper").append(result);
        });
    </script>

    <!-- //#javascript-->    
    <script type="text/javascript" src="js/common.js"></script>
    <script type="text/javascript">
        //保存链接字符串提示
        function saveconstr() {
            $('.w770').poshytip({
                className: 'tip-yellowsimple',
                showOn: 'focus',
                alignTo: 'target',
                alignX: 'right',
                alignY: 'center',
                offsetX: 5,
                showTimeout: 100
            });
        };

        //获取数据库表
        function loadtable() {
            var lft = $('#sidebar').offset().left + 60;
            var loading = layer.load(0, { shade: [0.2, '#fff'], offset: ['215px', lft + 'px'] });
            renderTable(
                { action: "getalltable" },
                $('.sideNav'),
                $('#tmptb').html(),
                function () {
                    layer.close(loading);
                    if ($('.sideNav li').length == 20) {
                        $('.sideNav').append('<li><a class="more" href="javascript:void(0);">更多&raquo;&raquo;</a></li>');
                    }
                }
            );
        };

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
                var tb = $('#currenttb').text();
                if ($.trim(tb) == '') return;

                switch (_tag) {
                    case 'tinyshowtip':
                        OpeniframeLayer(tb, 'tinytable.html?tb=' + tb, ['1100px', '580px'], true, true, false);
                        break;
                    case 'alltip':
                        $('.table tr:gt(0)').find('td').toggleClass('selected');
                        break;
                    case 'addtip':
                        OpeniframeLayer(tb, 'addfield.html?tb=' + tb, ['445px', '540px'], false);
                        break;
                    case 'codetip':
                        if (checkselected()) {
                            OpeniframeLayer('生成代码', 'code.aspx?tb=' + tb + '&fd=' + CurrentSelectTD, ['900px', '500px'], true, true, false);
                        };
                        break;
                    case 'libtip':
                        if (checkselected()) { go2codelib(tb); };
                        break;
                    default:
                        SuperSite.MsgError('操作无效');
                        break;
                };
            });
        };

        //导入代码库
        function go2codelib(table) {
            var paramdata = {
                action: "gencodelib",
                arg: { tb: table, fd: CurrentSelectTD }
            };
            doAjaxPost(paramdata, function (result) {
                if (result.success) {
                    SuperSite.MsgSuccess('导入成功');
                } else {
                    SuperSite.MsgError(result.msg);
                }
            });
        }

        //显示字段描述
        function showdescript() {
            $('.detail').poshytip({
                className: 'tip-violet',
                showTimeout: 1,
                alignTo: 'target',
                alignX: 'left',
                alignY: 'center'
            });
            $('.table tr:odd').addClass('odd');
        };

        //加载表信息
        function loadtableinfo(_n) {
            $('#currenttb').text(_n); //设置当前表
            //获取表信息
            var paramdata = {
                action: 'gettableinfo',
                arg: { tbname: _n }
            };
            renderTable(paramdata, $('.table'), $('#template').html(), showdescript);
        };

        //检查是否选中行
        function checkselected() {
            var count = 0;
            var selecttr = $('.table tr:gt(0)');
            CurrentSelectTD = selecttr.map(function (idx, dom) {
                if ($(dom).find('td[class*="selected"]').length > 0) {
                    count++;
                    return $(dom).find('td:first').text();
                };
            }).get().join(',');
            
            if (count == 0) {
                SuperSite.MsgWarning('您尚未选中任何字段');
                return false;
            }

            return true;
        };

        //快速取消选中行
        function quickdisselect() {
            var tb = $('.table');
            tb.find('tr:first').dblclick(function () {
                var targettr = tb.find('tr:gt(0)');
                targettr.find('td').removeClass('selected');
            });
        };

        $(function () {
            saveconstr();
            loadtable();
            settoolbar();
            quickdisselect();

            //修改数据库链接字符串
            $('.connect input').dblclick(function () {
                var _target = $(this);
                var constr = $.trim(_target.val());
                confirmLayerNormal('更改为当前数据库链接吗？', function (index) {
                    var paramdata = {
                        action: "updateconstr",
                        arg: { cs: constr }
                    };
                    doAjaxPost(paramdata, function (result) {
                        layer.close(index);
                        if (result.success) {
                            location.reload(true); //_target.focus();
                        } else {
                            SuperSite.MsgError(result.msg);
                        }
                    });
                });
            });

            //导航事件
            $('#mainNav li[class!="logout"]').find('a').click(function () {
                $(this).addClass('active');
                $(this).parent().siblings().find('a').removeClass('active');

                //显示隐藏数据库链接文本
                if ($(this).hasClass('setting')) {
                    $('.connect').fadeIn();
                } else {
                    $('.connect').fadeOut();
                    if ($(this).hasClass('help')) {
                        OpeniframeLayer('快捷窗口', 'help.html', ['600px', '300px'], true);
                    }
                    if ($(this).hasClass('codelib')) {
                        OpeniframeLayer('代码库', 'codelib.html', ['750px', '400px'], true, true, false);
                    }
                    if ($(this).hasClass('template')) {
                        OpeniframeLayer('json代码处理', 'json.html', ['530px', '530px'], true);
                    }
                    if ($(this).hasClass('formdesign')) {
                        OpeniframeLayer('表单设计器', 'formdesign.aspx', ['1000px', '600px'], true, true, false);
                    }                    
                }
            });

            //菜单事件
            $('.sideNav').delegate('li a', 'click', function (e) {
                e.stopPropagation();

                if ($(this).hasClass('more')) {
                    OpeniframeLayer('选择表', 'alltable.html', ['1200px', '600px'], true, true, false);
                    return;
                }

                $(this).addClass('active');
                $(this).parent().siblings().find('a').removeClass('active');
                loadtableinfo($(this).text());
            });
                        
            //单击表格行选中
            $('.table').delegate('tr:gt(0)', 'click', function (e) {
                e.stopPropagation();
                $(this).find('td').toggleClass('selected');
            });

            //编辑字段说明、编辑字段业务场景
            $('.table').delegate('.view, .edit', 'click', function (e) {
                e.stopPropagation();

                var tb = $('#currenttb').text();
                var fd = $(this).attr('ccvalue');
                if ($.trim(tb) == '') return;

                if ($(this).hasClass('view')) {
                    OpeniframeLayer(tb, 'mark.html?tb=' + tb + '&fd=' + fd, ['400px', '200px'], true);
                } else {
                    OpeniframeLayer(tb, 'scene.html?tb=' + tb + '&fd=' + fd, ['430px', '335px'], true);
                }

            });

            //删除字段
            $('.table').delegate('.delete', 'click', function (e) {
                e.stopPropagation();

                var _field = $(this).attr('ccvalue');
                var _target = $(this).parent().parent();

                confirmLayerNormal('确定删除此字段吗？', function (index) {
                    var paramdata = {
                        action: "deletefield",
                        arg: { tb: $('#currenttb').text(), fd: _field }
                    };
                    doAjaxPost(paramdata, function (result) {
                        layer.close(index);
                        if (result.success) {
                            _target.fadeOut();
                        } else {
                            SuperSite.MsgError(result.msg);
                        }                        
                    });
                });
            });

            //...

        });
    </script>
</body>
</html>