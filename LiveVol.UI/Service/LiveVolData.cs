using System.Security.RightsManagement;

namespace LiveVol.UI.Service
{
    public class LiveVolData
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string Symbol { get; set; }
        public string Strike { get; set; }
        public string Root { get; set; }
        public string Expiration { get; set; }
        public string Option { get; set; }
        public int Qty { get; set; }
        public string OpenInterest { get; set; }
        public string OptionVolume { get; set; }
        public int TradesCount { get; set; }
        public long TotalTradeNotional { get; set; }
        public decimal TotalTradePremium { get; set; }
        public decimal TotalEdge { get; set; }
        public long ExchangeSequenceNumber { get; set; }
        public decimal Price { get; set; }
        public decimal PriceChange { get; set; }
        public decimal Theo { get; set; }
        public decimal TheoIv { get; set; }
        public string Type { get; set; }
        public string Exchange { get; set; }
        public string Condition { get; set; }
        public string CancelledCondition { get; set; }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public string Market { get; set; }
        public decimal Premium { get; set; }
        public decimal SpotMoneyness { get; set; }
        public decimal TimeUntilExpiration { get; set; }
        public decimal TradeIv { get; set; }
        public decimal TradeIvChange { get; set; }
        public decimal TradeDelta { get; set; }
        public decimal TradeGamma { get; set; }
        public decimal TradeVega { get; set; }
        public decimal TradeTheta { get; set; }
        public decimal TradeRho { get; set; }
        public string UnderlyingBid { get; set; }
        public string UnderlyingAsk { get; set; }
        public string MarketUnderlying { get; set; }
        public decimal UnderlyingTradePrice { get; set; }
        public string UnderlyingChangeFromClose { get; set; }
        public string UnderlyingChangeFromOpen { get; set; }
        public string OnBidAsk { get; set; }
        public string MarketCondition { get; set; }
        public int HashCode { get; set; }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.HashCode;
        }

        public override int GetHashCode()
        {
            return HashCode;
        }
    }
}
