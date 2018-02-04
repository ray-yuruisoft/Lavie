using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class ProtocolTimeInfo
    {
        ///<summary>
        /// 协议次数ID
        ///</summary>
        [Column(@"ProtocolTimeID", Order = 1, TypeName = "int")]
        [Required]
        [Key]
        [Display(Name = "Protocol time ID")]
        public int ProtocolTimeID { get; set; } // ProtocolTimeID (Primary key)

        ///<summary>
        /// 协议次数
        ///</summary>
        [Required]
        [MaxLength(20)]
        [StringLength(20)]
        [Display(Name = "Name")]
        public string Name { get; set; } // Name (length: 20)
    }
}
