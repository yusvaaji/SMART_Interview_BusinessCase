﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace ConstructionProjectManagement.Middleware
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            { 
                context.Result = new BadRequestObjectResult(context.ModelState);
            } 
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}