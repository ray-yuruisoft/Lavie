﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Lavie.Infrastructure.FastLambda
{
    public class FastPartialEvaluator : PartialEvaluatorBase
    {
        public FastPartialEvaluator()
            : base(new FastEvaluator())
        { }
    }
}
