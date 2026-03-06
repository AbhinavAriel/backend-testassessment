namespace Assessment.API.Common
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }

        public static ApiResponse<T> Success(T data, string message = "Success")
            => new ApiResponse<T> { IsSuccess = true, Message = message, Data = data };

        public static ApiResponse<T> Fail(string message, Dictionary<string, string[]>? errors = null)
            => new ApiResponse<T> { IsSuccess = false, Message = message, Errors = errors };
    }
}