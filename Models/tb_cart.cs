namespace MZWlyt.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("购物车编号")]
        public int cid { get; set; }

        [DisplayName("用户姓名")]
        [Required]
        [StringLength(50)]
        public string uname { get; set; }

        [DisplayName("商品编号")]
        public Nullable<int> pid { get; set; }

        [DisplayName("商品名称")]
        [Required]
        [StringLength(50)]
        public string pname { get; set; }

        [DisplayName("商品单价")]
        public Nullable<decimal> price { get; set; }

        [DisplayName("商品数量")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "数量不可为空")]
        [DataType(DataType.Text, ErrorMessage = "数量格式错误")]
        public Nullable<int> nums { get; set; }

        [DisplayName("商品图片")]
        [Required]
        [StringLength(50)]
        public string photo { get; set; }

        public virtual tb_product tb_product { get; set; }
    }
}
