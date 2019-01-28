namespace DeBangTool.Model
{
    public class MatchResult
    {


        /// <summary>
        /// 目标
        /// </summary>
        public ColipuSouceModel MatchedInvoice { get; set; }

        /// <summary>
        /// 匹配到的
        /// </summary>
        public DepponSouceModel TargetFund { get; set; }
    }
}
