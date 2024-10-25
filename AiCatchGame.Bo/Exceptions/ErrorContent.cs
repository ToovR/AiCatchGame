namespace AiCatchGame.Bo.Exceptions
{
    public class ErrorContent
    {
        public ErrorContent(ErrorCodes code)
        {
            Code = code;
        }

        public ErrorContent()
        {
        }

        public ErrorContent(string message, ErrorCodes code, string context)
        {
            Message = message;
            Code = code;
            Context = context;
        }

        public ErrorCodes Code { get; set; }
        public string Context { get; set; } = "";

        public string Message { get; set; } = "";
    }
}