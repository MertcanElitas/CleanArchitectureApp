namespace CleanArchitecture.Application.Wrappers
{
    public class BaseResponse<T> where T : struct
     {
        public T Id { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}