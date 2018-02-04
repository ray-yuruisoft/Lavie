using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Lavie.Extensions;
using System.Runtime.Serialization;

namespace Lavie.Models
{
    [DataContract]
    public class ModelResult
    {
        private static ModelResult s_Empty = new ModelResult();

        [DataMember]
        public ModelErrorCollection Errors { get; private set; }

        [DataMember]
        public List<String> Infos { get; private set; }

        #region Constructor

        public ModelResult()
            : this(new ModelErrorCollection(),new List<String>())
        {
        }
        public ModelResult(ModelErrorCollection errors)
            : this(errors, new List<String>())
        {
        }
        public ModelResult(List<String> infos)
            : this(new ModelErrorCollection(), infos)
        {
        }
        public ModelResult(ModelErrorCollection errors, List<String> infos)
        {
            Errors = errors;
            Infos = infos;
        }

        #endregion

        [DataMember]
        public bool HasInfo
        {
            get
            {
                return !Infos.IsNullOrEmpty();
            }
        }

        [DataMember]
        public bool IsValid
        {
            get
            {
                return Errors == null || Errors.Count == 0;
            }
        }

        public ModelError GetFirstError()
        {
            return !IsValid ? Errors.First() : null;
        }

        public string GetFirstInfo()
        {
            return HasInfo?Infos.First():null;
        }

        public static ModelResult Empty
        {
            get { return s_Empty; }
        }
    }
}
