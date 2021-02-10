namespace FunctionAppAzureDeployment
{
    public class Student
    {   public Student()
        {
            this.name = "Stinky Pete";
            this.number = 11;
            this.group = "person";
        }
       
        public string name { get; set; }

        public int number { get; set; }

        public string group { get; set; }
    }
}
