using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class ProtocolTypeInfo
    {
        ///<summary>
        /// 协议类型ID
        ///</summary>
        [Column(@"ProtocolTypeID", Order = 1, TypeName = "int")]
        [Required]
        [Key]
        [Display(Name = "Protocol type ID")]
        public int ProtocolTypeID { get; set; } // ProtocolTypeID (Primary key)

        ///<summary>
        /// 协议类型
        ///</summary>
        [Required]
        [MaxLength(20)]
        [StringLength(20)]
        [Display(Name = "Name")]
        public string Name { get; set; } // Name (length: 20)
    }
}
