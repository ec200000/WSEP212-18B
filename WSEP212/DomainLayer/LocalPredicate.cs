using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Serialize.Linq.Extensions;
using Serialize.Linq.Serializers;

namespace WSEP212.DomainLayer
{
    public class LocalPredicate<PurchaseDetails>
    {
        [NotMapped]
        [JsonIgnore]
        public Expression<Func<PurchaseDetails, double>> method { set; get; }
        public int biggerThan { set; get; }
        
        private ExpressionSerializer ec = new ExpressionSerializer(new Serialize.Linq.Serializers.JsonSerializer());
        public string methodAsJson
        {
            get => method.ToJson();
            set => ec.DeserializeText(value).ToExpressionNode().ToExpression();
        }

        public LocalPredicate(Expression<Func<PurchaseDetails, double>> method, int biggerThan)
        {
            this.method = method;
            this.biggerThan = biggerThan;
        }

        public Predicate<PurchaseDetails> applyPredicate(PurchaseDetails details)
        {
            Predicate<PurchaseDetails> newPred = pd => method.Compile().Invoke(details) >= biggerThan;
            return newPred;
        }
    }
}