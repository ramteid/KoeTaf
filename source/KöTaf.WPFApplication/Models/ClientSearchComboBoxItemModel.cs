
namespace KöTaf.WPFApplication.Models
{
    public class ClientSearchComboBoxItemModel
    {
        public enum Type
        {

            FullName = 0,        
            ResidentialAddress,        
            TableNo
        }

        public string Value { get; set; }

        public Type SearchType { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
