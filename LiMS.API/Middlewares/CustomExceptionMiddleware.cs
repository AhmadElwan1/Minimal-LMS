namespace LiMS.API.Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidOperationException ex)
            {
                context.Response.ContentType = "application/json";

                if (ex.Message.Contains("Invalid JSON"))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    var errorResponse = new
                    {
                        Message = "Wrong entry type. Please check the data types of your input."
                    };
                    await context.Response.WriteAsJsonAsync(errorResponse);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    var errorResponse = new
                    {
                        Message = "A bad request occurred. Please check your input."
                    };
                    await context.Response.WriteAsJsonAsync(errorResponse);
                }
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var errorResponse = new
                {
                    Message = "An unexpected error occurred. Please try again."
                };
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
