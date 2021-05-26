using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Newtonsoft.Json;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer.PolicyPredicate
{
    public class SimplePredicate : SalePredicate
    {
        [JsonIgnore]
        private JsonSerializerSettings settings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            SerializationBinder = new KnownTypesBinder
            {
                KnownTypes = new List<Type>
                {
                    typeof(SalePolicy.SalePolicy),
                    typeof(SalePolicyMock),
                    typeof(PurchasePolicy.PurchasePolicy),
                    typeof(PurchasePolicyMock),
                    typeof(ItemImmediatePurchase),
                    typeof(ItemSubmitOfferPurchase),
                    typeof(SimplePredicate),
                    typeof(AndPredicates),
                    typeof(OrPredicates),
                    typeof(ConditioningPredicate),
                    typeof(ConditionalSale),
                    typeof(DoubleSale),
                    typeof(MaxSale),
                    typeof(XorSale),
                    typeof(SimpleSale),
                    typeof(SaleOnAllStore),
                    typeof(SaleOnCategory),
                    typeof(SaleOnItem)
                }
            }
        };
        
        [JsonIgnore]
        [NotMapped]
        public LocalPredicate<PurchaseDetails> predicate { get; set; }

        public string PredAsJson
        {
            get => JsonConvert.SerializeObject(predicate,settings);
            set => predicate = JsonConvert.DeserializeObject<LocalPredicate<PurchaseDetails>>(value,settings);
        }
        
        public SimplePredicate() {}
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
