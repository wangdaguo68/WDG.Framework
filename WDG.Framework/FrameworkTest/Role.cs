/* 
 * Author:
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Migrations.Model;
using System.Linq;

namespace FrameworkTest
{
    
    [Table("Role")]
    public class Role 
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "角色名不能为空")]
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public string Info { get; set; }
        public string BusinessPermissionString { get; set; }

        
    }
}
