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
    }
}