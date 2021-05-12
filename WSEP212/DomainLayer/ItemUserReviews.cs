using System;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class ItemUserReviews
    {
        public Item item { get; set; }
        public User reviewer { get; set; }
        public ConcurrentLinkedList<string> reviews { get; set; }

        private ItemUserReviews(Item item, User user)
        {
            this.item = item;
            this.reviewer = user;
            reviews = new ConcurrentLinkedList<string>();
        }

        // Checks that there is no other assosiation class for this user and item
        // If there is, return it, else, create new one
        public static ItemUserReviews getItemUserReviews(Item item, User user)
        {
            foreach (ItemUserReviews reviews in item.reviews.Values)
            {
                if(reviews.reviewer.Equals(user))
                {
                    return reviews;
                }
            }
            return new ItemUserReviews(item, user);
        }

        public void addReview(string review)
        {
            this.reviews.TryAdd(review);
        }

        public void removeReview(string review)
        {
            this.reviews.Remove(review, out _);
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