namespace MZWlyt.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_orders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("订单编号")]
        public int oid { get; set; }

        [DisplayName("用户姓名")]
        [Required]
        [StringLength(50)]
        public string uname { get; set; }

        [DisplayName("下单时间")]
        public Nullable<System.DateTime> orderTime { get; set; }

        [DisplayName("订单总价")]
        public Nullable<decimal> allPrice { get; set; }

        [DisplayName("收货地址")]
        [Required]
        [StringLength(50)]
        public string address { get; set; }

        [DisplayName("联系电话")]
        [Required]
        [StringLength(50)]
        public string tel { get; set; }

        [DisplayName("商品件数")]
        public Nullable<int> pcounts { get; set; }
    }
}
