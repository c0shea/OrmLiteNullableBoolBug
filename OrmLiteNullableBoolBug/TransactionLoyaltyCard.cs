using ServiceStack.DataAnnotations;

namespace OrmLiteNullableBoolBug
{
    [Schema("POS")]
    [CompositeIndex(nameof(TransactionHeaderId), nameof(SequenceNumber), nameof(CardNumber), Unique = true)]
    public class TransactionLoyaltyCard
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [Index]
        public int TransactionHeaderId { get; set; }
        
        public int? SequenceNumber { get; set; }

        public long? CardNumber { get; set; }

        public bool? IsVoid { get; set; }
    }
}
