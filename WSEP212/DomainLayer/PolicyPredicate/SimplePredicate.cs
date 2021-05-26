using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Newtonsoft.Json;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer.PolicyPredicate
{
    public class SimplePredicate : SalePredicate
    {
        [JsonIgnore]
        [NotMapped]
        public LocalPredicate<PurchaseDetails> predicate { get; set; }

        public string PredAsJson
        {
            get => JsonConvert.SerializeObject(predicate);
            set => predicate = JsonConvert.DeserializeObject<LocalPredicate<PurchaseDetails>>(value);
        }
        
        public SimplePredicate(LocalPredicate<PurchaseDetails> predicate, String predicateDescription) : base(predicateDescription)
        {
            this.predicate = predicate;
            //this.PredAsJson = JsonConvert.SerializeObject(predicate);
        }

        public override bool applyPrediacte(PurchaseDetails purchaseDetails)
        {
            return this.predicate.applyPredicate(purchaseDetails)(purchaseDetails);
        }
    }
}
