using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Lavie.ModelValidation
{

    public static class OpenUnobtrusiveValidationAttributesGenerator
    {
        /*
        public static IDictionary<string, object> GetUnobtrusiveValidationAttributes(string field)
        {
            ModelValidatorProviders.Providers.GetValidators(metadata ?? ModelMetadata.FromStringExpression(name, ViewData), ViewContext)
                .SelectMany(v => v.GetClientValidationRules())

            IEnumerable<ModelClientValidationRule> clientValidationRules = GetClientValidationRules(field);
            Dictionary<string, object> results = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            UnobtrusiveValidationAttributesGenerator.GetValidationAttributes(clientValidationRules, results);
            return results;
        }

        private static IEnumerable<ModelClientValidationRule> GetClientValidationRules(string field)
        {
            List<IValidator> list = null;
            if (!this._validators.TryGetValue(field, out list))
            {
                return Enumerable.Empty<ModelClientValidationRule>();
            }
            return (from item in list
                    let clientRule = item.ClientValidationRule
                    where clientRule != null
                    select clientRule);
        }
        */
        public static void GetValidationAttributes(IEnumerable<ModelClientValidationRule> clientRules, IDictionary<string, object> results)
        {
            // System.Web.WebPages.dll
            if (clientRules == null)
            {
                throw new ArgumentNullException("clientRules");
            }
            if (results == null)
            {
                throw new ArgumentNullException("results");
            }
            bool flag = false;
            foreach (ModelClientValidationRule rule in clientRules)
            {
                flag = true;
                string dictionaryKey = "data-val-" + rule.ValidationType;
                ValidateUnobtrusiveValidationRule(rule, results, dictionaryKey);
                results.Add(dictionaryKey, rule.ErrorMessage ?? string.Empty);
                dictionaryKey = dictionaryKey + "-";
                foreach (KeyValuePair<string, object> pair in rule.ValidationParameters)
                {
                    string key = dictionaryKey + pair.Key;
                    results.Add(key, pair.Value ?? string.Empty);
                }
            }
            if (flag)
            {
                results.Add("data-val", "true");
            }
        }

        private static void ValidateUnobtrusiveValidationRule(ModelClientValidationRule rule, IDictionary<string, object> resultsDictionary, string dictionaryKey)
        {
            // System.Web.WebPages.resources.dll
            if (string.IsNullOrWhiteSpace(rule.ValidationType))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "非介入式客户端验证规则中的验证类型名称不能为空。客户端规则类型: {0}", new object[] { rule.GetType().FullName }));
            }
            if (resultsDictionary.ContainsKey(dictionaryKey))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "非介入式客户端验证规则中的验证类型名称必须唯一。下列验证类型出现重复: { 0 }", new object[] { rule.ValidationType }));
            }
            if (rule.ValidationType.Any<char>(c => !char.IsLower(c)))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "非介入式客户端验证规则中的验证类型名称只能包含小写字母。无效名称:“{0}”，客户端规则类型: {1}", new object[] { rule.ValidationType, rule.GetType().FullName }));
            }
            foreach (string str in rule.ValidationParameters.Keys)
            {
                if (string.IsNullOrWhiteSpace(str))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "非介入式客户端验证规则中的验证参数名称不能为空。客户端规则类型: {0}", new object[] { rule.GetType().FullName }));
                }
                if (!char.IsLower(str.First<char>()) || str.Any<char>(c => (!char.IsLower(c) && !char.IsDigit(c))))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "非介入式客户端验证规则中的验证参数名称必须以小写字母开头，且只能包含小写字母或数字。验证参数名称: {0}，客户端规则类型: {1}", new object[] { str, rule.GetType().FullName }));
                }
            }
        }
    }
}

