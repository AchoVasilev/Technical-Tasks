namespace Infrastructure;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class DateFilter : ActionFilterAttribute
{
    private int startHour;
    private int endHour;
    public DateFilter(int startHour, int endHour)
    {
        this.startHour = startHour;
        this.endHour = endHour;
    }
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (DateTime.UtcNow.Hour > this.endHour && this.startHour < DateTime.UtcNow.Hour)
        {
            context.Result = new ObjectResult(!context.ModelState.IsValid)
            {
                StatusCode = StatusCodes.Status503ServiceUnavailable
            };
        }    
    }
}