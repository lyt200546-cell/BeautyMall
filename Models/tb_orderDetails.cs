namespace MZWlyt.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_orderDetails
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("详单编号")]
        public int id { get; set; }

        [DisplayName("订单编号")]
        public Nullable<int> oid { get; set; }

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

        [DisplayName("购买数量")]
        public Nullable<int> nums { get; set; }

        [DisplayName("商品图片")]
        [Required]
        [StringLength(50)]
        public string photo { get; set; }

        [DisplayName("商品状态")]
        [Required]
        [StringLength(50)]
        [Column("state")]
        public string states { get; set; }

        public virtual tb_product tb_product { get; set; }
    }
}
