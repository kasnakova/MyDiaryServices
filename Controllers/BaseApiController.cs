namespace MyDiary.Services.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;

    using MyDiary.Data;

    [Authorize]
    public abstract class BaseApiController : ApiController
    {
        protected IMyDiaryData data;

        protected BaseApiController(IMyDiaryData data)
        {
            this.data = data;
        }
    }
}