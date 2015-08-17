namespace MyDiary.Services.Infrastructure
{
    using System;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using System.Threading;

    public class AspNetUserIdProvider : IUserIdProvider
    {
        public string GetUserId()
        {
            return Thread.CurrentPrincipal.Identity.GetUserId();
        }
    }
}