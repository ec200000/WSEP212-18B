using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class ItemReview
    {
        [JsonIgnore]
        private JsonSerializerSettings settings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        
        [Key]
        public string UserNameRef{ get; set; }
        
        [ForeignKey("UserNameRef")]
        public User reviewer { get; set; }
        [NotMapped]
        public ConcurrentLinkedList<string> reviews { get; set; }
        
        public string ReviewsAsJson
        {
            get => JsonConvert.SerializeObject(reviews,settings);
            set => reviews = JsonConvert.DeserializeObject<ConcurrentLinkedList<string>>(value);
        }

        public ItemReview(User user)
        {
            this.reviewer = user;
            UserNameRef = user.userName;
            reviews = new ConcurrentLinkedList<string>();
            SystemDBAccess.Instance.ItemReviewes.Add(this);
            SystemDBAccess.Instance.SaveChanges();
        }
        
        public ItemReview(){}

        public bool addReview(string review)
        {
            var res = false;
            var result = SystemDBAccess.Instance.ItemReviewes.SingleOrDefault(i => i.ReviewsAsJson == this.ReviewsAsJson);
            if (result != null)
            {
                res = this.reviews.TryAdd(review);
                result.reviews = reviews;
                result.ReviewsAsJson = this.ReviewsAsJson;
                SystemDBAccess.Instance.SaveChanges();
            }
            return res;
        }

        public override string ToString()
        {
            return "The user " + this.reviewer.userName + " wrote:\n" +
                   reviewsToString();
        }

        private string reviewsToString()
        {
            if (reviews.First != null)
            {
                Node<string> node = reviews.First;
                string reviewsStr = "";
                while(node.Next != null)
                {
                    reviewsStr += node.Value + "\n";
                    node = node.Next;
                }
                return reviewsStr;
            }
            return "";
        }
        
        private string[] listToArray(ConcurrentLinkedList<string> lst)
        {
            string[] arr = new string[lst.size];
            int i = 0;
            Node<string> node = lst.First;
            while(node.Next != null)
            {
                arr[i] = node.Value;
                node = node.Next;
                i++;
            }
            return arr;
        }
    }
}