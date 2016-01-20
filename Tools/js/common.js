document.write("<script type='text/javascript' src='js/parseUri.1.2.2.js'></script>");
document.write('<script type="text/javascript" src="js/layer-v2.0/layer/layer.js"></script>');
document.write('<script type="text/javascript" src="js/poshytip-1.2/jquery.poshytip.min.js"></script>');
document.write('<script type="text/javascript" src="js/jquery.serializeJSON.js"></script>');
document.write('<script type="text/javascript" src="js/mustache.min.js"></script>');

//get方式的参数对象（暂时没有用到）
var globalAjaxParamGet = 'ajaxparam';

//固定浮动
$.fn.smartFloat = function (width_p) {

    var position = function (element) {
        var top = element.position().top,
            pos = element.css("position");

        $(window).scroll(function () {

            var scrolls = $(this).scrollTop();

            if (scrolls > top) {
                if (window.XMLHttpRequest) {
                    element.css({
                        position: "fixed",
                        'z-index': 999,
                        width: width_p,
                        top: 0
                    });
                } else {
                    element.css({ top: scrolls });
                }
            } else {
                element.css({
                    position: "", //absolute  
                    top: top
                });
            }
        });
    };

    return $(this).each(function () {
        position($(this));
    });
};

//输入检查
function inputcheck(tipfn, chkitem, currenturl) {
    var tag = false;
    var url = currenturl || location.href;
    var checkitem = chkitem || $('input[validate],textarea[validate]');

    if (checkitem.length > 0) {
        checkitem.each(function (idx, dom) {
            var tar = $(dom);
            if ($.trim(tar.val()) == '') {
                if (typeof tipfn == "function") {
                    tipfn(tar.attr('validate'));
                }

                tar.focus();
                tag = false;
                return false;
            } else {
                tag = true;
            }
        });
    }

    return tag;
};

$(function () {

    layer.config({
        extend: '/extend/layer.ext.js',
        //skin: 'layer-ext-moon', //一旦设定，所有弹层风格都采用此主题。
        //extend:'skin/moon/style.css'
    });

    //固定导航条
    $("#mainNav").smartFloat(930);

    ////回到顶部
    //$('body').append('<div class="gototop" style="display:none;"><span title="回到顶部"></span></div>');
    //$('.gototop').click(function () {
    //    $('html,body').animate({ scrollTop: '0px' }, 300);
    //});
    ////回到顶部显示与隐藏
    //$(window).scroll(function () {
    //    if ($(window).scrollTop() >= 100) {
    //        $('.gototop').fadeIn(300);
    //    } else {
    //        $('.gototop').fadeOut(300);
    //    }
    //});

});

//重置数据表
//initTable($('.list_table'), '<span class="td_nodata">尚未设置事件</span>');
function initTable(dom, tipmsg) {
    var tb = dom;
    var trs = tb.find('tr');

    var _msg = tipmsg || '没有数据';
    var _loading = '<tr><td colspan="15" id="defauttd" class="td_nodata">' + _msg + '</td></tr>';

    if (trs.length > 2) {
        tb.find('tr:gt(0)').remove();
    };

    trs = tb.find('tr'); //reset

    if (trs.length == 1) {
        trs.first().after(_loading);
    } else if (trs.length == 2) {
        if (trs.find('#defauttd').length > 0) {
            trs.find('#defauttd').html(tipmsg || loadingimg);
        } else {
            trs.last().html(_loading);
        }
    }
};

//渲染表格
function renderTable(param, $table, template, callback) {
    getJsonData(param, function (result) {
        var trs = $table.find('tr');
        if (trs.length > 1) {
            $table.find('tr:gt(0)').remove();
        }

        if (result.success) {
            if (result.data) {
                renderTemplate(result, template, function (view) { $table.append(view); });
            } else {
                $table.append('<tr><td colspan="15" class="td_nodata">没有数据</td></tr>');
            }
            if (typeof callback == "function") { callback(); }
        } else {
            SuperSite.MsgFailed(result.msg);
        }
    });
};

//渲染模板
function renderTemplate(data, template, callback) {
    Mustache.parse(template);
    var view = Mustache.render(template, data);
    if (typeof callback == "function") { callback(view); }
}

//获取Json数据
function getJsonData(param, callback) {
    var p = param || {};

    $.getJSON(globalRequestUrl, { ajaxparam: JSON.stringify(p) }, function (result) {
        if (typeof callback == "function") { callback(result); }
    });
};

//post提交
function doAjaxPost(paramdata, callback, errfn) {
    $.ajax({
        type: "post",
        dataType: "json",
        data: JSON.stringify(paramdata),
        url: globalRequestUrl,
        eache: false,
        success: function (result) {
            if (typeof callback == "function") {
                callback(result);
            }
        },
        error: errfn || function () {
            SuperSite.MsgError('error, please contact the administrator');
        }
    });
};

//获取html
function loadHtml($dom, url, callback, param) {
    var _p = param || {};

    $dom.load(url, _p, function () {
        if (typeof callback == "function") { callback(); }
    });
};

//Confirm窗口
function confirmLayerNormal(msg, callback) {
    layer.confirm(msg, { icon: 3, title: '提示', shadeClose: true, closeBtn: false }, function (_index) {
        if (typeof callback == "function") {
            callback(_index);
        }
    });
};
function confirmLayer(msg, btnarr, f1, f2) {
    layer.confirm(msg, {
        icon: 0,
        title: '提示',
        btn: btnarr, //['确定', '取消']
        closeBtn: false,
        shadeClose: true
    }, function (_index) {
        if (typeof f1 == "function") { f1(_index); }
    }, function (__index) {
        if (typeof f2 == "function") { f2(__index); }
    });
};

//打开iframe
function OpeniframeLayer(opentitle, openurl, layerwh, isclose, showmaxmin, isfull) {
    var _index = layer.open({
        type: 2,
        //skin: 'layui-layer-lan',
        title: opentitle,
        fix: true,
        maxmin: (showmaxmin || false),
        shadeClose: (isclose || false),
        area: layerwh, //['535px', '340px']
        content: openurl
    });

    if (isfull) {
        layer.full(_index);
    }
};

var SuperSite = {
    //0感叹，1对号，2差号，3问号，4凸号，5苦脸，6笑脸
    MsgWarning: function (msg) {
        layer.msg(msg || 'Warning', { icon: 0, time: 1500 });
    },
    MsgOK: function (msg, time) {
        var t = time || 1500;
        layer.msg(msg || 'OK', { icon: 1, time: t });
    },
    MsgError: function (msg) {
        layer.msg(msg || 'Error', { icon: 2, time: 1500 });
    },
    MsgConfirm: function (msg) {
        layer.msg(msg || 'Confirm', { icon: 3, time: 1500 });
    },
    MsgLock: function (msg) {
        layer.msg(msg || 'Lock', { icon: 4, time: 1500 });
    },
    MsgFailed: function (msg) {
        layer.msg(msg || 'Failed', { icon: 5, time: 1500 });
    },
    MsgSuccess: function (msg) {
        layer.msg(msg || 'Success', { icon: 6, time: 1500 });
    }

    //...

};

//当前日期和时间
function setTime($dom) {
    var day = "";
    var month = "";
    var ampm = "";
    var ampmhour = "";
    var myweekday = "";
    var year = "";
    var myHours = "";
    var myMinutes = "";
    var mySeconds = "";
    mydate = new Date();
    myweekday = mydate.getDay();
    mymonth = mydate.getMonth() + 1;
    myday = mydate.getDate();
    myyear = mydate.getYear();
    myHours = mydate.getHours();
    myMinutes = mydate.getMinutes();
    mySeconds = mydate.getSeconds();
    myHours = parseInt(myHours) < 10 ? "0" + myHours : myHours;
    myMinutes = parseInt(myMinutes) < 10 ? "0" + myMinutes : myMinutes;
    mySeconds = parseInt(mySeconds) < 10 ? "0" + mySeconds : mySeconds;
    year = (myyear > 200) ? myyear : 1900 + myyear;

    if (myweekday == 0) {
        weekday = "星期日";
    } else if (myweekday == 1) {
        weekday = "星期一";
    } else if (myweekday == 2) {
        weekday = "星期二";
    } else if (myweekday == 3) {
        weekday = "星期三";
    } else if (myweekday == 4) {
        weekday = "星期四";
    } else if (myweekday == 5) {
        weekday = "星期五";
    } else if (myweekday == 6) {
        weekday = "星期六";
    }
    $dom.html(year + "年" + mymonth + "月" + myday + "日&nbsp;" + weekday + "&nbsp;" + myHours + ":" + myMinutes + ":" + mySeconds);
    setTimeout("setTime()", 1000);
};
