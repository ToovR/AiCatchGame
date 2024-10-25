namespace AiCatchGame.Bo.Exceptions
{
    public class AiCatchException : Exception
    {
        public AiCatchException(ErrorCodes code) : base()
        {
            Content = new(code);
        }

        public AiCatchException(ErrorContent content) : base()
        {
            Content = content;
        }

        public ErrorCodes Code
        { get { return Content.Code; } }

        public ErrorContent Content { get; }
    }
}