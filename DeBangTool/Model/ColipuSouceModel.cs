namespace DeBangTool.Model
{
    public class ColipuSouceModel
    {
        //出库单号 商品编号    数量 金额  签收日期 是否已核销

        /// <summary>
        /// 出库单号
        /// </summary>    
        
        public string DOrder { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public string Quantity { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// 签收日期
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 是否已核销
        /// </summary>
        public string IsHeXiao { get; set; }

    }
}
