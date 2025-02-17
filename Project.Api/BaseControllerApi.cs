using Microsoft.AspNetCore.Mvc;
using SERP.Framework.ApiUtils.Controllers;
using SERP.Framework.ApiUtils.Responses;
using SERP.Framework.ApiUtils.Utils;
using SERP.Framework.Common.Exceptions;
using SERP.Framework.Common.Token;
using SERP.Framework.Common;
using SERP.Framework.Models;
using System.Security.Authentication;

namespace Project.Api
{
    public class BaseControllerApi : ApiControllerBase
    {
        public BaseControllerApi(IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger) : base(httpRequestHelper, logger)
        {
        }

        protected  async Task<IActionResult> ExecuteFunction<T>(Func<Task<T>> func)
        {

            try
            {
                return ParseResult(await func());
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, string.Empty);
                return ReturnException<T>(exception);
            }
        }

        private IActionResult ParseResult<T>(T result)
        {
            if (result == null)
            {
                return ResponseUtils.TransformData(new ResponseObject<T>(default(T), "null"));
            }

            if ((object)result is FileResult result2)
            {
                return result2;
            }

            if ((object)result is FileContentResult result3)
            {
                return result3;
            }

            if (result is Pagination<T>)
            {
                return ResponseUtils.TransformData(new ResponsePagination<T>(result as Pagination<T>));
            }

            if ((object)result is IActionResult result4)
            {
                return result4;
            }

            if ((object)result is Response data)
            {
                return ResponseUtils.TransformData(data);
            }

            return ResponseUtils.TransformData(new ResponseObject<T>(result));
        }

        private IActionResult ReturnException<T>(Exception exception)
        {
            if (exception is ArgumentException ex)
            {
                _logger.LogInformation(ex, string.Empty);
                return ResponseUtils.CreateErrorResult(Code.BadRequest, ex.Message, new Dictionary<string, string> { { "exception", ex.Message } });
            }

            if (exception is FileNotFoundException ex2)
            {
                _logger.LogInformation(ex2, string.Empty);
                return ResponseUtils.CreateErrorResult(Code.NotFound, ex2.Message, new Dictionary<string, string> { { "exception", ex2.Message } });
            }

            if (exception is TokenException ex3)
            {
                _logger.LogWarning(ex3, string.Empty);
                return ResponseUtils.CreateErrorResult(Code.Forbidden, ex3.Message, new Dictionary<string, string> { { "exception", ex3.Message } });
            }

            if (exception is InvalidOperationException ex4)
            {
                _logger.LogWarning(ex4, string.Empty);
                return ResponseUtils.CreateErrorResult(Code.Forbidden, ex4.Message, new Dictionary<string, string> { { "exception", ex4.Message } });
            }

            if (exception is ExistException ex5)
            {
                _logger.LogWarning(ex5, string.Empty);
                return ResponseUtils.CreateErrorResult(Code.BadRequest, ex5.Message, new Dictionary<string, string> { { "exception", ex5.Message } });
            }

            if (exception is NotFoundException ex6)
            {
                _logger.LogInformation(ex6, string.Empty);
                return ResponseUtils.CreateErrorResult(Code.NoContent, ex6.Message, new Dictionary<string, string> { { "exception", ex6.Message } });
            }

            if (exception is NullReferenceException ex7)
            {
                _logger.LogInformation(ex7, string.Empty);
                return ResponseUtils.TransformData(new Response(Code.NoContent, ex7.Message));
            }

            if (exception is UnauthorizedAccessException ex8)
            {
                _logger.LogInformation(ex8, string.Empty);
                return ResponseUtils.CreateErrorResult(Code.Unauthorized, ex8.Message, new Dictionary<string, string> { { "exception", ex8.Message } });
            }

            if (exception is AuthenticationException ex9)
            {
                _logger.LogWarning(ex9, string.Empty);
                return ResponseUtils.CreateErrorResult(Code.Unauthorized, ex9.Message, new Dictionary<string, string> { { "exception", ex9.Message } });
            }

            _logger.LogError(exception, string.Empty);
            return ResponseUtils.CreateErrorResult(Code.InternalServerError, "An error was occur, read this message for more details: " + exception.Message + exception.InnerException?.Message, new Dictionary<string, string> { { "exception", exception.Message } });
        }
    }
}
