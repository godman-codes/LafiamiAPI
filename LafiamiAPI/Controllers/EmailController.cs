using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.UnitofWorks;
using LafiamiAPI.Models.Internals;
using LafiamiAPI.Models.Requests;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Extensions;
using LafiamiAPI.Utilities.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace LafiamiAPI.Controllers
{
    [Route(Constants.Route)]
    [ApiController]
    public class EmailController : BaseController<EmailController>
    {
        public const string ControllerName = ControllerConstant.Email;
        private readonly IBusinessUnitofWork businessUnitofWork;
        private readonly WebsiteSettings websiteSettings;
        public EmailController(IMemoryCache memoryCache, ILogger<EmailController> logger, ISystemUnitofWork systemUnitofWork, IBusinessUnitofWork businessUnitofWork, IOptions<WebsiteSettings> _websiteconfig) : base(memoryCache, logger, systemUnitofWork)
        {
            this.businessUnitofWork = businessUnitofWork;
            websiteSettings = _websiteconfig.Value;
        }

        [HttpGet]
        [Route("GetEmails")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<EmailResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmails(string keyword, int page = 0, int pageSize = 0)
        {
            string cachename = GetMethodName() + keyword + Constants.Underscore + Constants.Underscore + page + Constants.Underscore + pageSize;
            List<EmailResponse> emails = (List<EmailResponse>)GetFromCache(cachename);
            if (emails == null)
            {
                emails = new List<EmailResponse>();

                Expression<Func<EmailModel, bool>> where = (r => true);
                IQueryable<EmailModel> queryable = businessUnitofWork.EmailService.GetQueryable(where);
                if (!string.IsNullOrEmpty(keyword))
                {
                    string[] keywordlist = keyword.Split(",");
                    queryable = queryable.Where(r => keywordlist.Contains(r.Emailaddresses) || keywordlist.Contains(r.Subject));
                }
                if (pageSize > 0)
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate)
                        .Skip(page * pageSize)
                        .Take(pageSize);
                }
                else
                {
                    queryable = queryable.OrderByDescending(r => r.CreatedDate);
                }

                List<EmailModel> _emails = await queryable.AsNoTracking().ToListAsync();

                foreach (EmailModel email in _emails)
                {
                    string subject = email.Subject;
                    EmailContent liteEmail = await businessUnitofWork.EmailService.GetEmailContent(email, websiteSettings);
                    if (liteEmail != null)
                    {
                        subject = liteEmail.Subject;
                    }

                    emails.Add(new EmailResponse()
                    {
                        Id = email.Id,
                        Emailaddresses = email.Emailaddresses,
                        Subject = subject,
                        Status = email.Status.DisplayName(),
                        Date = (((email.Status == MessageStatusEnums.Sent)) ? (email.Sentdate) : ((((email.Status == MessageStatusEnums.Failed)) ? (email.FailedDate) : (null)))),
                        IsSent = (email.Status == MessageStatusEnums.Sent),
                        IsPending = (email.Status == MessageStatusEnums.Pending),
                    });
                }


                SavetoCache(emails, cachename);
            }

            return Ok(emails);
        }

        [HttpGet]
        [Route("GetFailureReasonForEmail")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFailureReasonForEmail(Guid id)
        {
            string cachename = GetMethodName() + id;
            string response = (string)GetFromCache(cachename);
            if (response == null)
            {
                Expression<Func<EmailModel, bool>> where = (r => r.Id == id);
                IQueryable<EmailModel> queryable = businessUnitofWork.EmailService.GetQueryable(where);

                response = await queryable.AsNoTracking()
                                .Select(r => r.ResponseMessage)
                                .SingleOrDefaultAsync();

                SavetoCache(response, cachename);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("ResendEmail")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ResendEmail([FromBody] Guid id)
        {
            EmailModel email = await businessUnitofWork.EmailService.GetQueryable((r => r.Id == id)).SingleOrDefaultAsync();
            if (email == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.Email));
            }

            email.Status = MessageStatusEnums.Pending;
            email.UpdatedDate = DateTime.Now;

            businessUnitofWork.EmailService.Update(email);
            await businessUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Email, Constants.Queued));
        }

        [HttpPost]
        [Route("ContactUs")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ContactUs([FromBody] ContactUsRequest model)
        {
            string companyInfo = (string.IsNullOrEmpty(model.CompanyName)) ? string.Empty : (string.Join(Constants.Empty, Constants.OpenBracket, model.CompanyName, Constants.CloseBracket));

            EmailModel email = new EmailModel()
            {
                Emailaddresses = model.Email,
                Id = Guid.NewGuid(),
                Message = WebUtility.HtmlEncode(string.Join(Constants.NewLine, string.Join(Constants.Space, Constants.From, model.Surname, model.Firstname, model.Phone, companyInfo), model.Message)),
                Status = MessageStatusEnums.Pending,
                Subject = string.Join(Constants.Space, Constants.From, model.Surname, model.Firstname, model.Phone, companyInfo),
            };

            businessUnitofWork.EmailService.Add(email);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.Message, Constants.Sent));
        }


    }
}
