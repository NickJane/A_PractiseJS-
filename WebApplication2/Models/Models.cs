using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class SysUser
    {
        [Key]
        public int ID { get; set; }

        public string name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }


        public ICollection<SysRole> SysRoles { get; set; }
    }

    public class SysRole {
        [Key]
        public int RoleID { get; set; }
        public string RoleName { get; set; }

        public ICollection<SysUser> SysUsers { get; set; }
    }
}