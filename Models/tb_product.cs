namespace MZWlyt.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tb_product()
        {
            tb_cart = new HashSet<tb_cart>();
            tb_orderDetails = new HashSet<tb_orderDetails>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("商品编号")]
        public int pid { get; set; }

        [DisplayName("商品名称")]
        [Required]
        [StringLength(50)]
        public string pname { get; set; }

        [DisplayName("商品图片")]
        [Required]
        [StringLength(50)]
        public string photo { get; set; }

        [DisplayName("商品价格")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "价格不能为空")]
        [DataType(DataType.Currency, ErrorMessage = "价格格式输入错误")]
        public Nullable<decimal> price { get; set; }

        [DisplayName("商品库存")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "库存不能为空")]
        public Nullable<int> pnums { get; set; }

        [DisplayName("商品销量")]
        public Nullable<int> salenums { get; set; }

        [DisplayName("商品描述")]
        [Required]
        [StringLength(500)]
        public string mess { get; set; }

        [DisplayName("商品状态")]
        [Required]
        [StringLength(50)]
        public string state { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_cart> tb_cart { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_orderDetails> tb_orderDetails { get; set; }
    }
}
