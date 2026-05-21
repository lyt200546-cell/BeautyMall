namespace MZWlyt.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("留言编号")]
        public int mid { get; set; }

        [DisplayName("留言标题")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "标题不可为空")]
        [StringLength(50)]
        public string title { get; set; }

        [DisplayName("留言内容")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "内容不可为空")]
        [StringLength(200)]
        public string mess { get; set; }

        [DisplayName("用户姓名")]
        [Required]
        [StringLength(50)]
        public string uname { get; set; }

        [DisplayName("留言时间")]
        public Nullable<System.DateTime> messDate { get; set; }
    }
}
