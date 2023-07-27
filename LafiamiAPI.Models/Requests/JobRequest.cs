using LafiamiAPI.Datas.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LafiamiAPI.Models.Requests
{
    public class JobRequest
    {
        public Guid Id { get; set; } = Guid.Empty;
        [Required(ErrorMessage = "Company Name is required")]
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        [Required(ErrorMessage = "Job Title is required")]
        public string JobTitle { get; set; }
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentJob { get; set; }
        public string UserId { get; set; }

        public JobModel ToDBModel()
        {
            return new JobModel()
            {
                CompanyAddress = CompanyAddress,
                EndDate = EndDate,
                CompanyName = CompanyName,
                IsCurrentJob = IsCurrentJob,
                JobTitle = JobTitle,
                StartDate = StartDate,
                Id = Guid.NewGuid(),
                UserId = UserId
            };
        }
    }

    public class JobIdRequest
    {
        [Required(ErrorMessage = "Id is required")]
        public Guid Id { get; set; }
    }
}
