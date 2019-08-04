using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RatingApp.Model;
using RatingApp.Service;

namespace RatingApp.Controllers
{
    [Route("api/rating")]
    [ApiController]
    public class RatingController : Controller
    {
        public RatingService _ratingService;

        public RatingController()
        {
            this._ratingService = new RatingService();
        }

        [HttpPost]
        [Route("readRating")]
        public Task<List<ProductRatingDetail>> ReadRating(int [] pId)
        {
            var result = this._ratingService.GetRating(pId);
            return result;
        }

        [Route("updateRating/{productId}/{userId}/{rating}")]
        public async Task UpdateRating(int productId, int userId, int rating)
        {
            await this._ratingService.AddRating(userId, rating, productId);
        }

    }
}
