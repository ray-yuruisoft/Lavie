using Lavie.ModelValidation;
using System.Web.Mvc;

namespace Lavie.Models
{
    public class ModelResult<T> : ModelResult
    {
        public ModelResult(ModelErrorCollection errors)
            : base(errors)
        {
        }

        public ModelResult(T result, ModelErrorCollection errors)
            : base(errors)
        {
            Item = result;
        }

        public T Item { get; private set; }
    }
}
