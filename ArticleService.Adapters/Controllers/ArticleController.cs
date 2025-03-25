using ArticleService.Domain.Ports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using sltlang.Common.ArticleService.Models;

namespace ArticleService.Controllers
{
    [ApiController]
    [Route("/")]
    public class ArticleController(IArticleDb articleDb) : ControllerBase
    {
        [HttpGet(nameof(GetArticle))]
        public async Task<ActionResult<ArticleDto>> GetArticle([FromQuery] string name, bool? save_visit = true)
        {
            var article = await articleDb.GetArticle(name, save_visit ?? true);

            if (article == null)
                return NotFound();

            return Ok(article);
        }

        [HttpGet(nameof(GetArticleHistory))]
        public async Task<ActionResult<List<ArticleDto>>> GetArticleHistory([FromQuery] string name)
        {
            try
            {
                var article = await articleDb.GetArticle(name);

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
        public async Task<ActionResult<ArticleDto>> DeleteArticle([FromQuery] string name)
        {
            try
            {
                var article = await articleDb.GetArticle(name);
                await articleDb.DeleteArticle(article!);
                return Ok();
            }
            catch (Exception ex)
            {

            }

            return BadRequest();
        }

        [HttpGet(nameof(GetRating))]
        public async Task<ActionResult<List<ArticleDto>>> GetRating()
        {
            return Ok(await articleDb.GetRating());
        }
    }
}
