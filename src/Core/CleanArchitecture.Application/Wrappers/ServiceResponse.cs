namespace CleanArchitecture.Application.Wrappers
{
    public class ServiceResponse<TData, TId> : BaseResponse<TId> where TId : struct
        where TData : class , new()
    {
        public TData Data { get; set; }

        public ServiceResponse (TData value)
        {
            Data = value;
        }
    }
}