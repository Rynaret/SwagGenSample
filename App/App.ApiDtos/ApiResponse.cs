using System.Collections.Generic;

namespace App.ApiDtos
{
    public class ApiResponse<TPayload>
    {
        public Dictionary<string, IEnumerable<string>> ModelState { get; set; }
        public TPayload Payload { get; set; }

        public ApiResponse(TPayload value)
        {
            Payload = value;
        }

        public static implicit operator ApiResponse<TPayload>(TPayload value)
        {
            return new ApiResponse<TPayload>(value);
        }
    }
}
