using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Lavie.ModelValidation.Attributes
{
    public class HttpURLAttribute : ModelClientValidationRegularExpressionAttribute
    {
        // ^https?://(?:[^./\\s'\"<)\\]]+\\.)+[^./\\s'\"<\")\\]]+(?:/[^'\"<]*)*$
        // ^(http|ftp|https):\\/\\/[\\w\\-_]+(\\.[\\w\\-_]+)+([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])?$
        // ^|[^\\w'\"]|\\G)(?<uri>(?:https?|ftp)(?:&#58;|:)(?:&#47;&#47;|//)(?:[^./\\s'\"<)\\]]+\\.)+[^./\\s'\"<)\\]]+(?:(?:&#47;|/).*?)?)(?:[\\s\\.,\\)\\]'\"]?(?:\\s|\\.|\\)|\\]|,|<|$)

        public HttpURLAttribute() : base("^https?:\\/\\/[\\w\\-_]+(\\.[\\w\\-_]+)+([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])?$") { }
    }
}
