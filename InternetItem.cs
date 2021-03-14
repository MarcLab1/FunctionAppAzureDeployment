
namespace FunctionAppAzureDeployment
{
    public class InternetItem
    {
        public InternetItem(string name, string ipaddress, string date, string group)
        {
            this.name = name;
            this.ipaddress = ipaddress;
            this.date = date;
            this.group = group;
        }

        public string name { get; set; }
        public string ipaddress { get; set; }
        public string date { get; set; }
        public string group { get; set; }
    }
}
