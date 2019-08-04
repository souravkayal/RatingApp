using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RatingApp.Model
{
    public class ProductRating : Product
    {
        public int UserId { get; set; }
        public int Rating { get; set; }
    }

    public class ProductRatingDetail : Product
    {
        public double AverageRating { get; set; }
        public Dictionary<int, double> GroupRating = new Dictionary<int, double>(); 
    }
}
