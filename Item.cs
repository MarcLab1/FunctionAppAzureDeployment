
namespace FunctionAppAzureDeployment
{
    public class Item
    {
        public string id { get; set; }

        public string name { get; set; }

        public string email { get; set; }

        public Item(string id, string name, string email)
        {
            this.id = id;
            this.name = name;
            this.email = email;
        }
    }
}
