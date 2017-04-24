using HSAManager.Helpers.BizzaroHelpers;
using Microsoft.Identity.Client;
using TaskClient.Helpers.BizzaroHelpers;

namespace TaskClient
{
    public class BizzaroClient
    {
        public BizzaroClient(AuthenticationResult authenticationResult)
        {
            Receipts = new BizzaroReceipts(authenticationResult);
            Stores = new BizzaroStores(authenticationResult);
            Products = new BizzaroProducts(authenticationResult);
            ShoppingLists = new BizzaroShoppingLists(authenticationResult);
            Aggregate = new BizzaroAggregate(authenticationResult);
            Admin = new BizzaroAdmin(authenticationResult);
        }

        public BizzaroReceipts Receipts { get; set; }
        public BizzaroStores Stores { get; set; }
        public BizzaroProducts Products { get; set; }
        public BizzaroShoppingLists ShoppingLists { get; set; }
        public BizzaroAggregate Aggregate { get; set; }
        public BizzaroAdmin Admin { get; set; }
    }
}