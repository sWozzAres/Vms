using System.Net;
using System.Net.Http.Json;

namespace Utopia.Blazor.Application.Common.Services;

public abstract partial class PostResponse(HttpResponseMessage response)
{
    public HttpResponseMessage Response { get; private set; } = response;

    public class Success(HttpResponseMessage response) : PostResponse(response)
    {
    }

    //public class Forbid(HttpResponseMessage response) : PostResponse(response)
    //{
    //}

    public class Created(HttpResponseMessage response) : PostResponse(response)
    {

    }

    public class Failure(HttpResponseMessage response) : PostResponse(response)
    {
    }

    public class BadRequest : PostResponse
    {
        public string ErrorMessage { get; private set; }
        public BadRequest(HttpResponseMessage response) : base(response)
        {
            var result = response.Content.ReadFromJsonAsync<BadRequestResponse>().GetAwaiter().GetResult();

            ErrorMessage = result?.Detail ?? "Unexpected error was returned from the server.";
        }

        class BadRequestResponse
        {
            public string Title { get; set; } = null!;
            public int Status { get; set; }
            public string Detail { get; set; } = null!;
        }
    }

    //public class UnprocessableEntity : PostResponse
    //{
    //    readonly Dictionary<string, List<string>> _validationErrors;
    //    public Dictionary<string, List<string>> ValidationErrors => _validationErrors;
    //    public UnprocessableEntity(HttpResponseMessage response) : base(response)
    //    {
    //        var ue = response.Content.ReadFromJsonAsync<UnprocessableEntityResponse>().GetAwaiter().GetResult();

    //        _validationErrors = ue?.Errors ?? new Dictionary<string, List<string>>();
    //    }

    //    class UnprocessableEntityResponse
    //    {
    //        public string Title { get; set; } = null!;
    //        public int Status { get; set; }
    //        public Dictionary<string, List<string>> Errors { get; set; } = null!;
    //    }
    //}
    public class UnprocessableEntity : PostResponse
    {
        readonly Dictionary<string, List<string>> _validationErrors;
        public Dictionary<string, List<string>> ValidationErrors => _validationErrors;
        public UnprocessableEntity(HttpResponseMessage response) : base(response)
        {
            var ue = response.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>().GetAwaiter().GetResult();

            _validationErrors = ue ?? new Dictionary<string, List<string>>();
        }
    }


    public static PostResponse Create(HttpResponseMessage response)
        => response.StatusCode switch
        {
            HttpStatusCode.OK => new Success(response),
            HttpStatusCode.Created => new Created(response),
            //HttpStatusCode.Forbidden => new Forbid(response),
            HttpStatusCode.BadRequest => new BadRequest(response),
            HttpStatusCode.UnprocessableEntity => new UnprocessableEntity(response),
            _ => new Failure(response)
        };


}

