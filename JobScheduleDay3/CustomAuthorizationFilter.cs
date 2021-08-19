using Hangfire.Dashboard;

namespace JobSchedule
{
    public class CustomAuthorizationFilter : IDashboardAuthorizationFilter
    {

        public bool Authorize(DashboardContext context)
        {

            var httpContext = context.GetHttpContext();
            if (httpContext.User.Identity != null && httpContext.User.Identity.IsAuthenticated && httpContext.User.IsInRole("Administrator"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
