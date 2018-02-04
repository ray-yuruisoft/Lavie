using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Xoohoo.ActionResults;
using Xoohoo.Extensions;

namespace Xoohoo.Modules.Project.Controllers
{
    public class PersonInput
    {
        public int? ID { get; set; }

        [Required(ErrorMessage = "名称是必须填写的")]
        [StringLength(20, ErrorMessage = "名称请保持在20个字符以内")]
        [Display(Name = "名称")]
        [DisplayFormat(NullDisplayText = "请输入名称")]
        public string Name { get; set; }

        [Required(ErrorMessage = "年龄是必须的")]
        [DataType(DataType.Url, ErrorMessage = "Url")]
        [Range(1, 200, ErrorMessage = "年龄请保持在 1 - 200 之间")]
        [Display(Name = "年龄")]
        public int Age { get; set; }
        public int NInt { get; set; }
        [Required(ErrorMessage = "请输入NString")]
        [RegularExpression("^a", ErrorMessage = "请输入xxx")]
        public string NString { get; set; }
    }

    public enum ValidationTrigger
    {
        Change,
        Blur
    }

    public class Form
    {

    }

    public class ValidationRule
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Placeholder { get; set; }
        public Dictionary<string, object> Rules { get; set; }
        public ValidationRule(string name)
        {
            Name = name;
            Rules = new Dictionary<string, object>();
        }
    }

    public class ApiController : Controller
    {
        public object Index()
        {
            var result = GetUnobtrusiveValidationAttributes<PersonInput>(this.ControllerContext);

            return this.DateTimeJson(result);
        }

        public object Test(PersonInput personInput)
        {
            return Content(personInput.Name??"NN");
        }

        public List<ValidationRule> GetUnobtrusiveValidationAttributes<T>(ControllerContext context)
        {
            var result = new List<ValidationRule>();

            var propertyMetadataList = ModelMetadataProviders.Current.GetMetadataForProperties(null, typeof(T));
            foreach (var propertyMetadata in propertyMetadataList)
            {
                var validationRule = new ValidationRule(propertyMetadata.PropertyName);
                validationRule.DisplayName = propertyMetadata.DisplayName;
                validationRule.Placeholder = propertyMetadata.NullDisplayText;
                result.Add(validationRule);

                IEnumerable<ModelClientValidationRule> clientRules = ModelValidatorProviders.Providers.GetValidators(propertyMetadata, context).
                SelectMany(v => v.GetClientValidationRules());
                UnobtrusiveValidationAttributesGenerator.GetValidationAttributes(clientRules, validationRule.Rules);
            }

            return result;
        }
    }
}
