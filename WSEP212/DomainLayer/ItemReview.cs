using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class ItemReview
    {
        public string UserNameRef{ get; set; }
        [Key]
        [ForeignKey("UserNameRef")]
        [JsonIgnore]
        public User reviewer { get; set; }
        [NotMapped]
        public ConcurrentLinkedList<string> reviews { get; set; }
        
        public string LinkedListAsJson
        {
            get => JsonConvert.SerializeObject(reviews);
            set => reviews = JsonConvert.DeserializeObject<ConcurrentLinkedList<string>>(value);
        }

        public ItemReview(User user)
        {
            this.reviewer = user;
            UserNameRef = user.userName;
            reviews = new ConcurrentLinkedList<string>();
        }
        
        public ItemReview(){}

        public bool addReview(string review)
        {
            return this.reviews.TryAdd(review);
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