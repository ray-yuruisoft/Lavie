(function ($) {
    var $jQval = $.validator;
    var $jQvalunobtrusive = $.validator.unobtrusive;

    //Compare
    $jQvalunobtrusive.adapters.add("compareto", ["other", "value", "datatype", "operator"], function (options) {
        var prefix = getModelPrefix(options.element.name),
            other = options.params.other;

        //targetElement 目标元素
        var targetElement;
        if (other) {
            var fullOtherName = appendModelPrefix(other, prefix);
            targetElement = $(options.form).find(":input[name=" + fullOtherName + "]")[0];
        }

        var params = {
            targetElement: targetElement,
            value: options.params.value,
            datatype: options.params.datatype,
            operator: options.params.operator
        };

        setValidationValues(options, "compareTo", params);
    });
    $jQval.addMethod("compareTo", function (value, element, param) {
        //result 比较结果(默认通过验证)
        var result = true;
        //datatype 数据类型
        var datatype = param.datatype;
        //operator 比较操作
        var operator = param.operator;

        //param.targetElement 目标元素
        if (param.targetElement) {
            var target = $(param.targetElement);
            target.unbind(".validate-compareTo").bind("blur.validate-compareTo", function () {
               //element 当前元素
               $(element).valid();
            });
            //进行控件值比较
            result = compare(value, target.val(), datatype, operator);
        }
        //如果result为false，说明上面对控件的验证已经失败，没必要继续
        if (result && param.value) {
            //进行具体值比较
            result = compare(value, param.value, datatype, operator);
        }
        return result;
    });
    function compare(value, targetValue, datatype, operator) {
        var num;
        //整数 /^-?\d+$/.test(value)
        //小数 /^-?\d+\.\d+$/.test(value)
        switch (datatype) {
            case "String":
                num = compareValue(value, targetValue);
                break
            case "Integer":
                num = compareValue(parseInt(value), parseInt(targetValue));
                break
            case "Double":
                num = compareValue(parseInt(value), parseInt(targetValue));
                break
                break
            case "Date": //日期可以按字符串的规则比较
                num = compareValue(value, targetValue);
                break
            case "Currency":
                num = compareValue(Number(value), Number(targetValue));
                break
            default:
                num = NaN;
        }

        //注意这里不再进行格式校验，如果格式非法(num为NaN)，认为校验成功
        //比如用户尝试输入两个相同的非数字字符串，而进行是整型的相等比较，
        //如果这里也校验格式，反而让用户不知道到底是输入格式错误还是比较错误
        if (num != 0 && !num) {
            return true;
        }

        var result = false;
        switch (operator) {
            case "Equal":
                result = (num == 0);
                break
            case "NotEqual":
                result = (num != 0);
                break
            case "GreaterThan":
                result = (num > 0);
                break
            case "GreaterThanEqual":
                result = (num >= 0);
                break
            case "LessThan":
                result = (num < 0);
                break
            case "LessThanEqual":
                result = (num <= 0);
                break
            default:
                result = false;
        }
        return result;
    }
    function compareValue(valA, valB) {
        if (valA == valB) return 0;
        if (valA > valB) return 1;
        if (valA < valB) return -1;
    }
    //以下方法提取自jquery.validate.unobtrusive.js
    function getModelPrefix(fieldName) {
        return fieldName.substr(0, fieldName.lastIndexOf(".") + 1);
    }

    function appendModelPrefix(value, prefix) {
        if (value.indexOf("*.") === 0) {
            value = value.replace("*.", prefix);
        }
        return value;
    }
    function setValidationValues(options, ruleName, value) {
        options.rules[ruleName] = value;
        if (options.message) {
            options.messages[ruleName] = options.message;
        }
    }

    //ByteLength

} (jQuery));