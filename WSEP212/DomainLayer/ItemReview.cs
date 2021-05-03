using System;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class ItemReview
    {
        public User reviewer { get; set; }
        public ConcurrentLinkedList<string> reviews { get; set; }

        public ItemReview(User user)
        {
            this.reviewer = user;
            reviews = new ConcurrentLinkedList<string>();
        }
        
        public ItemReview()
        {
            this.reviewer = null;
            reviews = new ConcurrentLinkedList<string>();
        }

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
    }
}