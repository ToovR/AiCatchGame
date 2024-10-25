using System.Collections;

namespace AiCatchGame.Bo.Exceptions
{
    public class ErrorContentDebug : ErrorContent
    {
        public IDictionary? Data { get; set; }
        public string[]? DetailLines { get; set; }
        public string? TraceId { get; set; }

        public static ErrorContentDebug FromErrorContent(ErrorContent errorContent)
        {
            return new ErrorContentDebug()
            {
                Code = errorContent.Code,
                Context = errorContent.Context,
                Message = errorContent.Message,
            };
        }
    }
}