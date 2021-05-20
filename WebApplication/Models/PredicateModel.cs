using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class PredicateModel
    {
        public string firstPred { get; set; }

        public string secondPred { get; set; }

        public string compositionType { get; set; }
        
        public string predicate { get; set; }
        
        public int numbersOfProducts { get; set; }
        public int priceOfShoppingBag { get; set; }
        public int ageOfUser { get; set; }
        
    }

}