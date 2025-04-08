using Exam.Controllers.Model;
using Microsoft.AspNetCore.Mvc;

namespace Exam.Controllers
{
    [ApiController]
    [Route("api/promotion")]
    public class CalPromotionController : Controller
    {
        [HttpPost("calculate-discount")]
        public IActionResult CalculateDiscount([FromBody] DiscountRequest request)
        {
            var products = request.Products;

            foreach (var product in products)
            {
                foreach (var promo in request.Promotions)
                {
                    if (promo.ProductID != null && promo.ProductID.Contains(product.ProductID))
                    {
                        decimal discountedPrice = 0;

                        if (promo.DiscountType == "Percent")
                        {
                            discountedPrice = product.Price * (1 - promo.Discount / 100);
                        }
                        else if (promo.DiscountType == "Baht")
                        {
                            discountedPrice = product.Price - promo.Discount;
                            if (discountedPrice < 0) discountedPrice = 0;
                        }

                        product.AfterDiscountPrice = discountedPrice * product.Qty;
                        break;
                    }
                }

                if (product.AfterDiscountPrice == 0)
                {
                    product.AfterDiscountPrice = product.Price * product.Qty;
                }
            }

            return Ok(products);
        }
        [HttpPost("calculate-monney")]
        public IActionResult CalculateChange([FromBody] ChangeRequest request)
        {
            var change = request.RecvMoney - request.GrandTotal;

            if (change < 0)
            {
                return BadRequest("ลูกค้าจ่ายเงินไม่ครบ");
            }

            var result = new List<Dictionary<string, string>>();

            var Banknotes = new Dictionary<string, int>
        {
            { "Bank500", 500 },
            { "Bank100", 100 },
            { "Bank50", 50 },
            { "Bank20", 20 },
            { "Coin10", 10 },
            { "Coin5", 5 },
            { "Coin1", 1 }
        };

            foreach (var denom in Banknotes)
            {
                int count = (int)(change / denom.Value);
                if (count > 0)
                {
                    result.Add(new Dictionary<string, string> { { denom.Key, count.ToString() } });
                    change -= count * denom.Value;
                }
            }

            return Ok(result);
        }

        [HttpPost("calculate-people")]
        public IActionResult CalculatePeopleNeeded([FromBody] List<Dictionary<string, int>> taskInput)
        {
            int totalHours = 0;

            foreach (var task in taskInput)
            {
                foreach (var kv in task)
                {
                    switch (kv.Key.ToLower())
                    {
                        case "easy":
                            totalHours += kv.Value * 1;
                            break;
                        case "medium":
                            totalHours += kv.Value * 2;
                            break;
                        case "hard":
                            totalHours += kv.Value * 4;
                            break;
                    }
                }
            }

            const int hoursPerPerson = 8;
            int peopleNeeded = (int)Math.Ceiling((double)totalHours / hoursPerPerson);

            return Ok(new {peopleNeeded });
        }

        [HttpPost("calculate-days")]
        public IActionResult CalculateDays([FromBody] TaskInput input)
        {
            int easyHours = 0;
            int mediumHours = 0;
            int hardHours = 0;

            foreach (var task in input.Tasks)
            {
                foreach (var kv in task)
                {
                    switch (kv.Key.ToLower())
                    {
                        case "easy": easyHours += kv.Value * 1; break;
                        case "medium": mediumHours += kv.Value * 2; break;
                        case "hard": hardHours += kv.Value * 4; break;
                    }
                }
            }

            int seniorCount = 0;
            int juniorCount = 0;

            foreach (var dev in input.Programmers)
            {
                foreach (var kv in dev)
                {
                    if (kv.Key.ToLower() == "senior") seniorCount += kv.Value;
                    if (kv.Key.ToLower() == "junior") juniorCount += kv.Value;
                }
            }

            const int hoursPerDay = 8;

            int totalSeniorHoursAvailablePerDay = seniorCount * hoursPerDay;

            if (seniorCount == 0 && hardHours > 0)
            {
                return BadRequest("No senior developers available for hard tasks.");
            }

            int sharedWorkHours = mediumHours + easyHours;

            int totalTeamHoursPerDay = (seniorCount + juniorCount) * hoursPerDay;

            double daysForHard = hardHours / (double)(seniorCount * hoursPerDay);
            double daysForSharedWork = sharedWorkHours / (double)(totalTeamHoursPerDay);

            double totalDays = Math.Max(daysForHard, daysForSharedWork);

            return Ok(new
            {
                totalDays = Math.Ceiling(totalDays)
            });
        }

    }
}
