using Dapper;
using RatingApp.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RatingApp.Service
{
    public interface IRatingService
    {
        Task<List<ProductRatingDetail>> GetRating(int[] productId);
        void AddRating(int userId, int productId, int rating);
    }

    public static class Helper
    {
        public static Dictionary<int, double> UpdateMissingRating(this Dictionary<int, double> dic)
        {
            for (int i = 1; i <= 5; i++)
            {
                if (!dic.ContainsKey(i))
                    dic.Add(i, 0);
            }
            return dic;
        }
    }

    public class RatingService
    {
        static string strConnectionString = "Server=DESKTOP-O6KGEJO\\SQLEXPRESS;Database=RatingApp;Trusted_Connection=True;";

        public async Task<List<ProductRatingDetail>> GetRating(int[] productId)
        {
            var productRating = new List<ProductRatingDetail>();

            using (IDbConnection con = new SqlConnection(strConnectionString))
            {
                var result =  await con.QueryAsync<ProductRating>(@"SELECT pr.UserId, prd.ProductId, prd.ProductName , prd.ProductDescription, prd.ProductUrl, 
                              pr.Rating 
                              FROM ProductRating as pr JOIN Products prd on pr.ProductId = prd.ProductId
                              where pr.ProductId in @ids", new { ids = productId });

                foreach (var item in result.GroupBy(f=>f.ProductId))
                {
                    var prodRating = item.GroupBy(f => f.Rating);
                    Dictionary<int, double> ratings = new Dictionary<int, double>();

                    double sumRating = 0;
                    var ratingCount = (double)result.Where(f => f.ProductId == item.Key).Count();
                    foreach (var rating in prodRating)
                    {
                        ratings.Add(rating.Key,  ((double) rating.Count() / ratingCount) *100 );
                        sumRating += rating.Key * rating.Count() ;
                    }

                    ratings = ratings.UpdateMissingRating();
                    productRating.Add(new ProductRatingDetail { ProductId = item.Key, ProductName = item?.First().ProductName, ProductDescription = item?.First().ProductDescription, ProductUrl = item?.First().ProductUrl, GroupRating = ratings, AverageRating = (sumRating / ratingCount) });
                }
                return productRating;
            }
        }

       

        public async Task AddRating(int userId, int rating, int productId)
        {

            using (IDbConnection con = new SqlConnection(strConnectionString))
            {
                var result = await con.QueryAsync<ProductRating>("SELECT COUNT(1) from ProductRating WHERE ProductId = @productId AND UserId = @userId", new { productId = productId, userId = userId });
                if (result != null)
                    await con.ExecuteAsync("UPDATE ProductRating SET Rating = @rating where UserId = @userId AND ProductId = @productId", new { userId = userId, productId = productId, rating = rating });
                else
                    await con.ExecuteAsync("INSERT INTO ProductRating(UserId, ProductId, Rating) VALUES(@userId, @productId , @rating) ", new { userId = userId, productId = productId, rating = rating });
            }
        }
    }
}
