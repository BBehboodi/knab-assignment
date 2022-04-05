namespace Knab.Assignment.API.Exceptions
{
    public class HttpResponseException : Exception
    {
        public HttpResponseException(int statusCode)
            : this(statusCode, message: null!)
        { }

        public HttpResponseException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; }
    }
}