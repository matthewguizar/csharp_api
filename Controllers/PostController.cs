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

        [HttpGet("Posts")]
        public IEnumerable<Post> GetPosts()
        {
            string getPostSql = @"SELECT [PostId],
                                [UserId],
                                [PostTitle],
                                [PostContent],
                                [PostCreated],
                                [PostUpdated]
                                FROM TutorialAppSchema.Posts";
            return _dapper.LoadData<Post>(getPostSql);
        }

        [HttpGet("PostSingle/{postId}")]
        public IEnumerable<Post> GetPostSingle(int postId)
        {
            string getPostSql = @"SELECT [PostId],
                                [UserId],
                                [PostTitle],
                                [PostContent],
                                [PostCreated],
                                [PostUpdated]
                                FROM TutorialAppSchema.Posts
                                WHERE PostId = " + postId.ToString();
            return _dapper.LoadData<Post>(getPostSql);
        }

        [HttpGet("PostUser/{userId}")]
        public IEnumerable<Post> GetPostsByUser(int userId)
        {
            string getPostSql = @"SELECT [PostId],
                                [UserId],
                                [PostTitle],
                                [PostContent],
                                [PostCreated],
                                [PostUpdated]
                                FROM TutorialAppSchema.Posts
                                WHERE UserId = " + userId.ToString();
            return _dapper.LoadData<Post>(getPostSql);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string getPostSql = @"SELECT [PostId],
                                [UserId],
                                [PostTitle],
                                [PostContent],
                                [PostCreated],
                                [PostUpdated]
                                FROM TutorialAppSchema.Posts
                                WHERE UserId = " + this.User.FindFirst("userId")?.Value;
            return _dapper.LoadData<Post>(getPostSql);
        }

        [HttpGet("PostsBySearch/{searchParam}")]
        public IEnumerable<Post> PostsBySearch(string searchParam)
        {
            string getPostSql = @"SELECT [PostId],
                                [UserId],
                                [PostTitle],
                                [PostContent],
                                [PostCreated],
                                [PostUpdated]
                                FROM TutorialAppSchema.Posts
                                WHERE PostTitle LIKE '%" + searchParam + "%'" +
                                " OR PostContent LIKE '%" + searchParam + "%'";
            return _dapper.LoadData<Post>(getPostSql);
        }

        [HttpPost("Post")]
        public IActionResult AddPost(PostToAddDto postToAdd)
        {
            string addPostSql = @"
            INSERT INTO TutorialAppSchema.Posts(
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated]) VALUES(" + this.User.FindFirst("userId")?.Value
                + ", '" + postToAdd.PostTitle
                + "','" + postToAdd.PostContent
                + "', GETDATE(), GETDATE() )";
            if (_dapper.ExecuteSql(addPostSql))
            {
                return Ok();
            }

            throw new Exception("Failed to create new post!");
        }

         [HttpPut("Post")]
        public IActionResult EditPost(PostToEditDto postToEdit)
        {
            string addPostSql = @"
            UPDATE TutorialAppSchema.Posts 
                SET PostContent = '"+ postToEdit.PostContent + 
                "', PostTitle = '" + postToEdit.PostTitle + 
                @"', PostUpdated = GETDATE()
                WHERE PostId = " + postToEdit.PostId.ToString() +
                "AND UserId = " + this.User.FindFirst("userId")?.Value;
            if (_dapper.ExecuteSql(addPostSql))
            {
                return Ok();
            }

            throw new Exception("Failed to edit new post!");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string deletePostSql = @"DELETE FROM TutorialAppSchema.Posts
            Where PostId = " + postId.ToString() +
            "AND UserId = " + this.User.FindFirst("userId")?.Value;

            if (_dapper.ExecuteSql(deletePostSql))
            {
                return Ok();
            }

            throw new Exception("Failed to delete post!");
        }
    }
}