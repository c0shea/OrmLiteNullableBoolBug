using System;
using ServiceStack.OrmLite;

namespace OrmLiteNullableBoolBug
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbFactory = new OrmLiteConnectionFactory("Data Source=analysis;Initial Catalog=Staging;Integrated Security=True;", SqlServer2014Dialect.Provider);

            using (var db = dbFactory.Open())
            {
                var headerId = 1;
                var cardNumber = 12345;

                var query = db.From<TransactionLoyaltyCard>()
                    .Where(tlc => tlc.TransactionHeaderId == headerId &&
                                  tlc.CardNumber == cardNumber &&
                                  cardNumber > 0 &&
                                  !(tlc.IsVoid ?? false))
                    .OrderBy(tlc => tlc.SequenceNumber)
                    .Take(1);

                // Although not the same condition, it works if I explicitly check for null
                // (tlc.IsVoid == null || !tlc.IsVoid.Value))

                var loyaltyCards = db.Select(query);
                ;

                /*
                 Throws exception: 
System.Data.SqlClient.SqlException: 'An expression of non-boolean type specified in a context where a condition is expected, near ')'.
Invalid usage of the option NEXT in the FETCH statement.'

                SQL Generated:

SELECT "Id", "TransactionHeaderId", "SequenceNumber", "CardNumber", "IsVoid" 
FROM "POS"."TransactionLoyaltyCard"
WHERE (((("TransactionHeaderId" = @0) AND ("CardNumber" = @1)) AND (1=1)) AND NOT (COALESCE("IsVoid",@2)))
ORDER BY "SequenceNumber" OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY

                What the SQL Should Be:
SELECT "Id", "TransactionHeaderId", "SequenceNumber", "CardNumber", "IsVoid" 
FROM "POS"."TransactionLoyaltyCard"
WHERE (((("TransactionHeaderId" = @0) AND ("CardNumber" = @1)) AND (1=1)) AND NOT (COALESCE("IsVoid",@2)) = 0)
ORDER BY "SequenceNumber" OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY
                 */
            }
        }
    }
}
