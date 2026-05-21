namespace MZWlyt.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("管理员编号")]
        public int aid { get; set; }

        [DisplayName("管理员姓名")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "姓名不能为空")]
        [StringLength(50)]
        public string aname { get; set; }

        [DisplayName("管理员密码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "密码不能为空")]
        [DataType(DataType.Password, ErrorMessage = "密码格式输入错误")]
        [StringLength(50)]
        public string password { get; set; }

        [DisplayName("联系电话")]
        [Required]
        [StringLength(50)]
        public string tel { get; set; }
    }
}
