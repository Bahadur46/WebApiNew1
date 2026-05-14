namespace WebApiNew
{
    public class ApiResponse
    {
        
        public bool hasErrors { get; set; }
        public string? errors { get; set; }
        public string? errorCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        
    }
}
