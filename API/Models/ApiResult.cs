namespace API.Models
{
    /// <summary>
    /// Generic result class untuk API responses
    /// </summary>
    /// <typeparam name="T">Tipe data yang dikembalikan</typeparam>
    public class ApiResult<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; } = default(T);
        public string Message { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public static ApiResult<T> Success(T data, string message = "")
        {
            return new ApiResult<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResult<T> Error(string errorMessage)
        {
            return new ApiResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }
}