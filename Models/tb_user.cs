namespace MZWlyt.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class tb_user
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("用户编号")]
        public int uid { get; set; }

        [DisplayName("用户姓名")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "用户名称不能为空")]
        [StringLength(50)]
        public string uname { get; set; }

        [DisplayName("用户密码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "用户密码不能为空")]
        [DataType(DataType.Password, ErrorMessage = "密码格式输入错误")]
        [StringLength(50)]
        public string password { get; set; }

        [NotMapped]
        [Compare("password")]
        [Required]
        [DisplayName("确认密码")]
        [DataType(DataType.Password)]
        public string confirmPassword { get; set; }

        [DisplayName("用户地址")]
        [Required]
        [StringLength(50)]
        public string address { get; set; }

        [DisplayName("用户电话")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "电话号码格式不正确")]
        [Required]
        [StringLength(50)]
        public string tel { get; set; }

        [Required]
        [StringLength(50)]
        public string email { get; set; }
    }
}
