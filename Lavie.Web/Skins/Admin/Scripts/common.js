//V1 method
String.prototype.format = function() {
    var args = arguments;
    return this.replace(/\{(\d+)\}/g,
        function(m, i) {
            return args[i];
        });
}

//V2 static
String.format = function() {
    if (arguments.length == 0)
        return null;

    var str = arguments[0];
    for (var i = 1; i < arguments.length; i++) {
        var re = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
        str = str.replace(re, arguments[i]);
    }
    return str;
}
//javascript获取指定参数及其对应的值  
function getParameter(paraStr, url)  
{  
    var result = "";  
    //获取URL中全部参数列表数据  
    var str = "&" + url.split("?")[1];  
    var paraName = paraStr + "=";  
    //判断要获取的参数是否存在  
    if(str.indexOf("&"+paraName)!=-1)  
    {  
        //如果要获取的参数到结尾是否还包含“&”  
        if(str.substring(str.indexOf(paraName),str.length).indexOf("&")!=-1)  
        {  
            //得到要获取的参数到结尾的字符串  
            var TmpStr=str.substring(str.indexOf(paraName),str.length);  
            //截取从参数开始到最近的“&”出现位置间的字符  
            result=TmpStr.substr(TmpStr.indexOf(paraName),TmpStr.indexOf("&")-TmpStr.indexOf(paraName));    
        }  
        else  
        {    
            result=str.substring(str.indexOf(paraName),str.length);    
        }  
    }    
    else  
    {    
        result="";    
    }    
    return (result.replace("&",""));
}  
 //可以根据需要，配合以下方法实现自己要实现的功能；  
 //var hostname = location.hostname; //获取当前域名(不包含http://)  
 //var localurl = location.href;   //获取当前完整的URL地址信息(包含http://，域名，路径，具体文件和全部传递参数)  
 //var referurl = document.referrer; //获取上一页完整的URL信息(包含http://，域名，路径，具体文件和全部传递参数)  

$(document).ready(function () {
    //if ($.browser.msie) {
    //    $('input:checkbox').click(function () {
    //        this.blur();
    //        this.focus();
    //    });
    //    $('input:radio').click(function () {
    //        this.blur();
    //        this.focus();
    //    });
    //}

    $(".maxlength").keyup(function() {
        var area = $(this);
        var max = parseInt(area.attr("maxlength"), 10);
        if (max > 0) {
            if (area.val().length > max) {
                area.val(area.val().substr(0, max));
            }
        }
    });

    $('.sdate').datepicker({  
        dateFormat: 'yy-mm-dd'
        //,buttonImage: 'calendar.gif'  //按钮的图片路径，自己设置
        //,buttonImageOnly: true //Show an image trigger without any button.
        //,showOn: 'both'//触发条件，both表示点击文本域和图片按钮都生效
        ,yearRange: '2008:2020'//年份范围
        ,clearText: '清除'
        ,clearStatus: '清除已选日期'
        ,closeText: '关闭'
        ,closeStatus: '不改变当前选择'
        ,prevText: '<上月'
        ,prevStatus: '显示上月'
        ,prevBigText: '<<'
        ,prevBigStatus: '显示上一年'
        ,nextText: '下月>'
        ,nextStatus: '显示下月'
        ,nextBigText: '>>'
        ,nextBigStatus: '显示下一年'
        ,currentText: '今天'
        //,weekHeader: '周'
        //,showWeek:true
        ,firstDay: 1
        ,yearSuffix: '年'
        ,selectOtherMonths: true
        ,showMonthAfterYear: true
        //,changeMonth:true
        ,changeYear:true
        ,monthNames: ['一月','二月','三月','四月','五月','六月', '七月','八月','九月','十月','十一月','十二月']
        ,monthNamesShort: ['一','二','三','四','五','六', '七','八','九','十','十一','十二']
        ,dayNames: ['星期日','星期一','星期二','星期三','星期四','星期五','星期六']
        ,dayNamesShort: ['周日','周一','周二','周三','周四','周五','周六']
        ,dayNamesMin: ['日','一','二','三','四','五','六']
    });
    $("#btnQuick1").click(function () {
        var date = GetDateStrint(0);
        $("#BeginDate").val(date);
        $("#EndDate").val(date);
        $("#searchForm").submit();
    });
    $("#btnQuick2").click(function () {
        var date = GetDateStrint(-1);
        $("#BeginDate").val(date);
        $("#EndDate").val(date);
        $("#searchForm").submit();
    });
    function GetDateStrint(addDayCount) {
        var dd = new Date();
        dd.setDate(dd.getDate() + addDayCount);
        var y = dd.getFullYear();
        var m = dd.getMonth() + 1;
        var d = dd.getDate();
        return y + "-" + m + "-" + d;
    }
});

function AddFav(title, url) {
    //var title = document.title
    //var url = document.location.href
    if (window.sidebar) window.sidebar.addPanel(title, url, "");
    else if (window.opera && window.print) {
        var mbm = document.createElement('a');
        mbm.setAttribute('rel', 'sidebar');
        mbm.setAttribute('href', url);
        mbm.setAttribute('title', title);
        mbm.click();
    }
    else if (document.all) window.external.AddFavorite(url, title);
}
function SetHomepage(url) {
    if (document.all) {
        document.body.style.behavior = 'url(#default#homepage)';
        document.body.setHomePage(url);

    }
    else if (window.sidebar) {
        if (window.netscape) {
            try {
                netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");
            }
            catch (e) {
                alert("该操作被浏览器拒绝，如果想启用该功能，请在地址栏内输入 about:config,然后将项 signed.applets.codebase_principal_support 值该为true");
            }
        }
        var prefs = Components.classes['@mozilla.org/preferences-service;1'].getService(Components.interfaces.nsIPrefBranch);
        prefs.setCharPref('browser.startup.homepage', url);
    }
}
