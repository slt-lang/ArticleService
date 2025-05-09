using ArticleService.Domain.Ports;
using Microsoft.AspNetCore.Mvc;
using sltlang.Common.ArticleService.Models;

namespace ArticleService.Controllers
{
    [ApiController]
    [Route("/")]
    public class ArticleController(IArticleDb articleDb) : ControllerBase
    {
        [HttpGet(nameof(GetArticle))]
        public async Task<ActionResult<ArticleDto>> GetArticle([FromQuery] string? culture_key, string name, bool? save_visit = true)
        {
            var article = await articleDb.GetArticle(culture_key, name, save_visit ?? true);

            if (article == null)
                return NotFound();

            return Ok(article);
        }

        [HttpGet(nameof(GetArticleHistory))]
        public async Task<ActionResult<List<ArticleDto>>> GetArticleHistory([FromQuery] string? culture_key, string name)
        {
            try
            {
                var article = await articleDb.GetArticle(culture_key, name);

                if (article == null)
                    return NotFound();

                var history = await articleDb.GetArticleHistory(article!);

                return Ok(history);
            }
            catch (Exception ex)
            {

            }

            return BadRequest();
        }

        [HttpPost(nameof(UpsertArticle))]
        public async Task<ActionResult<ArticleDto>> UpsertArticle([FromBody] ArticleDto articleDto)
        {
            await articleDb.UpsertArticle(articleDto);
            return Ok();
        }

        [HttpPost(nameof(RebaseArticle))]
        public async Task<ActionResult<List<ArticleDto>>> RebaseArticle([FromQuery] int historyId)
        {
            try
            {
                await articleDb.RebaseArticle(historyId);
                return Ok();
            }
            catch (Exception ex)
            {

            }

            return BadRequest();
        }

        [HttpDelete(nameof(DeleteArticle))]
        public async Task<ActionResult<ArticleDto>> DeleteArticle([FromQuery] string? culture_key, string name)
        {
            try
            {
                var article = await articleDb.GetArticle(culture_key, name);
                await articleDb.DeleteArticle(article!);
                return Ok();
            }
            catch (Exception ex)
            {

            }

            return BadRequest();
        }

        [HttpGet(nameof(GetRating))]
        public async Task<ActionResult<List<ArticleDto>>> GetRating([FromQuery] string? culture_key)
        {
            return Ok(await articleDb.GetRating(culture_key));
        }

        [HttpGet(nameof(GetUserEditions))]
        public async Task<ActionResult<List<ArticleDto>>> GetUserEditions([FromQuery] int userId)
        {
            return Ok(await articleDb.GetArticlesByUserId(userId));
        }
    }
}
