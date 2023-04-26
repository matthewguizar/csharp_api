using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Post")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None")
        {
            string getPostSql = @"EXEC TutorialAppSchema.spPosts_Get";
            string stringParameter = "";
            
            DynamicParameters sqlParameters = new DynamicParameters();
            if (postId != 0)
            {
                stringParameter += ", @PostId=" + postId.ToString();
                sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);
            }
            if (userId !=0)
            {
                stringParameter += ", @UserId=" + userId.ToString();
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }
            if (searchParam.ToLower() != "none")
            {
                stringParameter += ", @SearchValue='" + searchParam + "'";
                sqlParameters.Add("@SearchValueParameter", searchParam, DbType.String);
            }
            if (stringParameter.Length > 0)
            {
                getPostSql += stringParameter.Substring(1);
            }
            return _dapper.LoadDataWithParameters<Post>(getPostSql, sqlParameters);
        }


      [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId =@UserIdParameter";
            DynamicParameters sqlParmeters = new DynamicParameters();
            sqlParmeters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);

            return _dapper.LoadDataWithParameters<Post>(sql, sqlParmeters);
        }


     [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postToUpsert)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
                @UserId=@UserIdParameter, 
                @PostTitle=@PostTitleParameter, 
                @PostContent=@PostContentParameter";
            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
            sqlParameters.Add("@PostTitleParameter", postToUpsert.PostTitle, DbType.String);
            sqlParameters.Add("@PostContentParameter", postToUpsert.PostContent, DbType.String);

            if (postToUpsert.PostId > 0) {
                sql +=  ", @PostId =@PostIdParameter";
                sqlParameters.Add("@PostIdParameter", postToUpsert.PostId, DbType.Int32);
            }

            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
            {
                return Ok();
            }

            throw new Exception("Failed to upsert post!");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"EXEC TutorialAppSchema.spPost_Delete 
                @UserId=@UserIdParameter, 
                @PostId=@PostIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
            sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);

            
            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
            {
                return Ok();
            }

            throw new Exception("Failed to delete post!");
        }
    }
}