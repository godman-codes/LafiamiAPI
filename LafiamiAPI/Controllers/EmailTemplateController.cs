using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.UnitofWorks;
using LafiamiAPI.Models.Requests;
using LafiamiAPI.Models.Responses;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Extensions;
using LafiamiAPI.Utilities.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
    public class EmailTemplateController : BaseController<AccountController>
    {
        public const string ControllerName = ControllerConstant.EmailTemplate;
        public EmailTemplateController(IMemoryCache memoryCache, ISystemUnitofWork _systemUnitofWork, ILogger<AccountController> logger) : base(memoryCache, logger, _systemUnitofWork)
        {
        }

        [HttpGet]
        [Route("GetEmailTemplateTypes")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(List<EmailTemplateTypeResponse>), StatusCodes.Status200OK)]
        public IActionResult GetEmailTemplateTypes()
        {
            List<EmailTemplateTypeResponse> result = Enum.GetValues(typeof(EmailTypeEnums)).Cast<EmailTypeEnums>()
                .Select(r => new EmailTemplateTypeResponse()
                {
                    Id = (int)r,
                    Name = r.DisplayName(),
                    Description = r.DisplayDescription()
                }).ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetEmailTemplate")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(EmailTemplateResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmailTemplate(EmailTypeEnums emailType)
        {
            string cachename = GetMethodName() + emailType;
            EmailTemplateResponse result = (EmailTemplateResponse)GetFromCache(cachename);
            if (result == null)
            {
                result = await systemUnitofWork.EmailTemplateService.GetEmailTemplateToResponse(emailType);
                result.Template = WebUtility.HtmlDecode(result.Template);

                SavetoCache(result, cachename);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("CreateEmailTemplate")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(NewItemResponse<long>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SaveEmailTemplate([FromBody] NewEmailTemplateRequest model)
        {
            EmailTemplateModel newItem = model.ToEntity();
            systemUnitofWork.EmailTemplateService.Add(newItem);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            NewItemResponse<long> result = new NewItemResponse<long>(newItem.Id, string.Format(Constants.ActionResponse, Constants.EmailTemplate, Constants.Created));

            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateEmailTemplate")]
        [Authorize(Roles = SystemRole.Administrator)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateEmailTemplate([FromBody] ExistingEmailTemplateRequest model)
        {
            Expression<Func<EmailTemplateModel, bool>> where = (r => r.Id == model.Id);
            EmailTemplateModel emailTemplate = await systemUnitofWork.EmailTemplateService.GetEmailTemplate(model.Id);
            if (emailTemplate == null)
            {
                throw new WebsiteException(string.Format(Constants.ItemDoesNotExist, Constants.EmailTemplate));
            }

            model.ToEntity(emailTemplate);

            systemUnitofWork.EmailTemplateService.Update(emailTemplate);
            await systemUnitofWork.SaveAsync();

            ClearCache();
            return Ok(string.Format(Constants.ActionResponse, Constants.EmailTemplate, Constants.Updated));
        }

    }
}
