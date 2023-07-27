using LafiamiAPI.Datas.Models;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LafiamiAPI.Datas
{
    public class SeedData
    {
        public static async Task SetupDatabase(LafiamiContext context, UserManager<UserViewModel> userManager, RoleManager<ApplicationRoleModel> roleManager)
        {
           
            if (context.Users.Any(r => r.IsDeleted && (r.Email != r.UserName))) {
                var users = await context.Users.Where(r => r.IsDeleted && (r.Email != r.UserName)).ToListAsync();
                foreach (var user in users)
                {
                    user.Email = user.UserName;
                    user.NormalizedEmail = user.NormalizedUserName;
                }
                await context.SaveChangesAsync();
            }


            if (context.InsuranceCompanies.Any(r => !r.UseSystemHospitals && (r.Company == CompanyEnum.Lafiami))) {
                var company = await context.InsuranceCompanies.FirstOrDefaultAsync(r => !r.UseSystemHospitals && (r.Company == CompanyEnum.Lafiami));
                if (company != null) {
                    company.UseSystemHospitals = true;
                    await context.SaveChangesAsync();
                }
            }
            if (context.InsuranceCompanies.Any(r => !r.UseSystemHospitals && (r.Company == CompanyEnum.RelainceHMO)))
            {
                var company = await context.InsuranceCompanies.FirstOrDefaultAsync(r => !r.UseSystemHospitals && (r.Company == CompanyEnum.RelainceHMO));
                if (company != null)
                {
                    company.UseSystemHospitals = true;
                    await context.SaveChangesAsync();
                }
            }

           
            if (!context.InsuranceCompanies.Any(r => r.Company == CompanyEnum.Aiico))
            {
                InsuranceCompanyModel insuranceCompany = new InsuranceCompanyModel()
                {
                    HasExtraInformation = true,
                    Company = CompanyEnum.Aiico,
                    OrderBy = 0,
                    RequiredBankInformation = true,
                    RequiredIdenitication = true,
                    RequiredJobInformation = true,
                    RequiredNextOfKin = true,
                    CompanyExtras = new List<CompanyExtraModel>()
                };

                insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                {
                    Name = "titleId",
                    DisplayName = "Title",
                    OrderBy = 0,
                    AnswerType = AnswerTypeEnum.Dropdown,                    
                });
                insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                {
                    Name = "wef",
                    DisplayName = "Policy Start Date",
                    OrderBy = 0,
                    AnswerType = AnswerTypeEnum.DateTime,
                });
                insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                {
                    Name = "effectiveDate",
                    DisplayName = "Effective Date",
                    OrderBy = 0,
                    AnswerType = AnswerTypeEnum.DateTime,
                });
                //insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                //{
                //    Name = "wet",
                //    DisplayName = "Policy End Date",
                //    OrderBy = 0,
                //    AnswerType = AnswerTypeEnum.DateTime,
                //});
                insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                {
                    Name = "sumAssured",
                    DisplayName = "Sum Assured",
                    OrderBy = 0,
                    AnswerType = AnswerTypeEnum.Decimal,
                    ForProductSetup = true,
                });
               
                insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                {
                    Name = "LGAId",
                    DisplayName = "LGA",
                    OrderBy = 0,
                    AnswerType = AnswerTypeEnum.Dropdown,
                    DependentName = "Hospital",
                    HasDependent = true,
                });
                insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                {
                    Name = "Hospital",
                    DisplayName = "Hospital",
                    OrderBy = 0,
                    AnswerType = AnswerTypeEnum.Dropdown,
                    LoadingToBeTrigger = true,
                });
                insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                {
                    Name = "Genders",
                    DisplayName = "Gender",
                    OrderBy = 0,
                    AnswerType = AnswerTypeEnum.Dropdown,
                    DropdownListGetUrl = "/Aiico/GetGenders"
                });
                insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                {
                    Name = "utilityBillName",
                    DisplayName = "Utility Bill Name",
                    OrderBy = 0,
                    AnswerType = AnswerTypeEnum.String,
                });

                insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                {
                    Name = "utilityBillUrl",
                    DisplayName = "Utility Bill",
                    OrderBy = 0,
                    AnswerType = AnswerTypeEnum.Image
                });
                context.InsuranceCompanies.Add(insuranceCompany);
                await context.SaveChangesAsync();
            }
                       
            if (!context.InsuranceCompanies.Any(r => r.Company == CompanyEnum.Lafiami))
            {
                InsuranceCompanyModel insuranceCompany = new InsuranceCompanyModel()
                {
                    HasExtraInformation = false,
                    Company = CompanyEnum.Lafiami,
                    OrderBy = 0,
                    RequiredBankInformation = true,
                    RequiredIdenitication = true,
                    RequiredJobInformation = true,
                    RequiredNextOfKin = true,
                    CompanyExtras = new List<CompanyExtraModel>()
                };

                context.InsuranceCompanies.Add(insuranceCompany);
                await context.SaveChangesAsync();
            }

            if (!context.InsuranceCompanies.Any(r => r.Company == CompanyEnum.Hygeia))
            {
                InsuranceCompanyModel insuranceCompany = new InsuranceCompanyModel()
                {
                    HasExtraInformation = true,
                    Company = CompanyEnum.Hygeia,
                    OrderBy = 0,
                    RequiredBankInformation = false,
                    RequiredIdenitication = false,
                    RequiredJobInformation = false,
                    RequiredNextOfKin = false,
                    CompanyExtras = new List<CompanyExtraModel>()
                };

                context.InsuranceCompanies.Add(insuranceCompany);
                await context.SaveChangesAsync();
            }

            if (context.InsuranceCompanies.Any(r => r.RequiredBankInformation && (r.Company == CompanyEnum.Hygeia)))
            {
                InsuranceCompanyModel insuranceCompany = await context.InsuranceCompanies.Where(r => r.RequiredBankInformation && (r.Company == CompanyEnum.Hygeia)).SingleOrDefaultAsync();
                if (insuranceCompany != null) {
                    insuranceCompany.RequiredBankInformation = false;

                    context.InsuranceCompanies.Update(insuranceCompany);
                    await context.SaveChangesAsync();
                }
            }

            if (context.InsuranceCompanies.Any(r => (r.Company == CompanyEnum.Hygeia) && (!r.HasExtraInformation))) {
                var insuranceCompany = await context.InsuranceCompanies.Where(r => (r.Company == CompanyEnum.Hygeia) && (!r.HasExtraInformation)).Include(r=> r.CompanyExtras).SingleOrDefaultAsync();

                if (insuranceCompany != null) {
                    insuranceCompany.HasExtraInformation = true;

                    insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                    {
                        Name = "PlanId",
                        DisplayName = "Plan",
                        OrderBy = 0,
                        AnswerType = AnswerTypeEnum.Dropdown,
                        DropdownListGetUrl = "/Hygeia/GetHygeiaPlans",
                        ForProductSetup = true,
                    });

                    insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                    {
                        Name = "IsDependant",
                        DisplayName = "Is Dependant",
                        OrderBy = 0,
                        AnswerType = AnswerTypeEnum.Boolean,
                        ForProductSetup = true,
                    });

                    insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                    {
                        Name = "StateId",
                        DisplayName = "State",
                        OrderBy = 0,
                        AnswerType = AnswerTypeEnum.Dropdown,
                        DropdownListGetUrl = "/Hygeia/GetStates",
                        DependentName = "LGAId",
                        HasDependent = true,
                    });

                    insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                    {
                        Name = "LGAId",
                        DisplayName = "LGA",
                        OrderBy = 0,
                        AnswerType = AnswerTypeEnum.Dropdown,
                        DropdownListGetUrl = "/Hygeia/GetLGAs?stateId=",
                        LoadingToBeTrigger = true,                        
                    });

                    insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                    {
                        Name = "MaritalStatusId",
                        DisplayName = "Marital Status",
                        OrderBy = 0,
                        AnswerType = AnswerTypeEnum.Dropdown,
                        DropdownListGetUrl = "/Hygeia/GetMaritalStatuses"
                    });

                    insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                    {
                        Name = "TitleId",
                        DisplayName = "Title",
                        OrderBy = 0,
                        AnswerType = AnswerTypeEnum.Dropdown,
                        DropdownListGetUrl = "/Hygeia/GetTitles"
                    });

                    insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                    {
                        Name = "CoverageClassId",
                        DisplayName = "Coverage Class",
                        OrderBy = 0,
                        AnswerType = AnswerTypeEnum.Dropdown,
                        DropdownListGetUrl = "/Hygeia/GetConvergeClasses",
                        ForProductSetup = true
                    });

                    insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                    {
                        Name = "DependantTypeId",
                        DisplayName = "Dependant Type",
                        OrderBy = 3,
                        AnswerType = AnswerTypeEnum.Dropdown,
                        DropdownListGetUrl = "/Hygeia/GetDependentTypes",
                    });

                    insuranceCompany.CompanyExtras.Add(new CompanyExtraModel()
                    {
                        Name = "PrincipalMemberId",
                        DisplayName = "Principal Member Id",
                        OrderBy = 4,
                        AnswerType = AnswerTypeEnum.String,
                    });

                    context.InsuranceCompanies.Update(insuranceCompany);
                    await context.SaveChangesAsync();
                }
            }

            if (context.CompanyExtras.Any(r => (r.Name == "DependantTypeId") && r.ForProductSetup))
            {
                var dbobj = await context.CompanyExtras.Where(r => (r.Name == "DependantTypeId") && r.ForProductSetup).SingleOrDefaultAsync();
                if (dbobj != null)
                {
                    dbobj.ForProductSetup = false;
                    dbobj.OrderBy = 3;
                    context.CompanyExtras.Update(dbobj);
                    await context.SaveChangesAsync();
                }
            }

            if (context.CompanyExtras.Any(r => (r.Name == "PrincipalMemberId") && (r.OrderBy == 0)))
            {
                var dbobj = await context.CompanyExtras.Where(r => (r.Name == "PrincipalMemberId") && (r.OrderBy == 0)).SingleOrDefaultAsync();
                if (dbobj != null)
                {
                    dbobj.OrderBy = 4;
                    context.CompanyExtras.Update(dbobj);
                    await context.SaveChangesAsync();
                }
            }

            if (context.CompanyExtras.Any(r => (r.InsuranceCompany.Company == CompanyEnum.Hygeia) && (r.Name == "PlanId") && (r.AnswerType !=  AnswerTypeEnum.Dropdown)))
            {
                var extra = await context.CompanyExtras.Where(r => (r.InsuranceCompany.Company == CompanyEnum.Hygeia) && (r.Name == "PlanId") && (r.AnswerType != AnswerTypeEnum.Dropdown)).SingleOrDefaultAsync();

                if (extra != null)
                {
                    extra.AnswerType = AnswerTypeEnum.Dropdown;
                    extra.DropdownListGetUrl = "/Hygeia/GetHygeiaPlans";
                    extra.UpdatedDate = DateTime.UtcNow;

                    context.CompanyExtras.Update(extra);
                    await context.SaveChangesAsync();
                }
            }

           
            if (!context.InsuranceCompanies.Any(r => r.Company == CompanyEnum.RelainceHMO))
            {
                InsuranceCompanyModel insuranceCompany = new InsuranceCompanyModel()
                {
                    HasExtraInformation = false,
                    Company = CompanyEnum.RelainceHMO,
                    OrderBy = 0,
                    RequiredBankInformation = true,
                    RequiredIdenitication = false,
                    RequiredJobInformation = false,
                    RequiredNextOfKin = false,
                    CompanyExtras = new List<CompanyExtraModel>()
                };

                context.InsuranceCompanies.Add(insuranceCompany);
                await context.SaveChangesAsync();
            }

            if (!context.InsuranceCompanies.Any(r => r.Company == CompanyEnum.AxaMansand))
            {
                InsuranceCompanyModel insuranceCompany = new InsuranceCompanyModel()
                {
                    HasExtraInformation = false,
                    Company = CompanyEnum.AxaMansand,
                    OrderBy = 0,
                    RequiredBankInformation = true,
                    RequiredIdenitication = false,
                    RequiredJobInformation = false,
                    RequiredNextOfKin = false,
                    CompanyExtras = new List<CompanyExtraModel>()
                };

                context.InsuranceCompanies.Add(insuranceCompany);
                await context.SaveChangesAsync();
            }

            if (!context.Users.Any(r => r.UserRoles.Any(r => r.Role.Name == SystemRole.Administrator)))
            {
                CountryModel country = new CountryModel()
                {
                    CreatedDate = DateTime.Now,
                    Name = "Nigeria",
                    IsDeleted = false,
                    UpdatedDate = DateTime.Now,
                    ThreeLetterIsoCode = "NIG",
                    TwoLetterIsoCode = "NG",

                };
                StateModel state = new StateModel()
                {
                    CreatedDate = DateTime.Now,
                    Name = "Lagos",
                    IsDeleted = false,
                    UpdatedDate = DateTime.Now,
                    Country = country
                };
                CityModel city = new CityModel()
                {
                    CreatedDate = DateTime.Now,
                    Name = "Ikeja",
                    IsDeleted = false,
                    UpdatedDate = DateTime.Now,
                    State = state
                };

                UserViewModel user = new UserViewModel()
                {
                    Address = "22 Surulere street, Cele Busstop, Alagbole, Ojodu Berger",
                    Email = "samuel@igunleinnovations.com",
                    Firstname = "Samuel",
                    IsActive = true,
                    MiddleName = "",
                    PhoneNumber = "08034451727",
                    Sex = SexEnums.Male,
                    Surname = "Adekoya",
                    UserName = "samuel@igunleinnovations.com",
                    MustChangePassword = false,
                    City = city,
                    State = state,
                    Country = country,
                };

                IdentityResult result = await userManager.CreateAsync(user, "Password1.,");
                if (result.Succeeded)
                {
                    if (!roleManager.RoleExistsAsync(SystemRole.Administrator).GetAwaiter().GetResult())
                    {
                        await roleManager.CreateAsync(new ApplicationRoleModel() { Name = SystemRole.Administrator });
                    }

                    await userManager.AddToRoleAsync(user, SystemRole.Administrator);
                    await context.SaveChangesAsync();
                }
            }

        }


    }
}
