namespace WebApplication9.Services
{
    public class OrderHelper
    {
        public static decimal ShippingFee { get; } = 5;
        public static Dictionary<string, string> PaymentMethods = new()
        {
            { "Cash", "Cash on Delivery" },
            {"Paypal", "Paypal" },
            {"CreditCard", "CreditCard" }
        };
        public static List<string> PaymentStatuses = new()
        {
           "Pending", "Canceled", "Accepted"
        };
        public static List<string> OrderStatuses = new()
        {
            "Created", "Accepted", "Canceled", "Shipped", "Delivered", "Returned"
        };
        public static Dictionary<int, int> getdictionary(string productsIdentifiers)
        {
            Dictionary<int, int> dictionary = new();
            string[] products = productsIdentifiers.Split('-');
            foreach(var product in products) {
                try
                {
                    int pid = int.Parse(product);
                    if (dictionary.ContainsKey(pid))
                        dictionary[pid] += 1;
                    else dictionary.Add(pid, 1);
                }
                catch(Exception)
                {

                }
            }
            return dictionary;
        }
    }
}
