using LafiamiAPI.Datas.Models;
using LafiamiAPI.Utilities.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LafiamiAPI.Datas
{
    public partial class LafiamiContext : IdentityDbContext<UserViewModel,
        ApplicationRoleModel, string, IdentityUserClaim<string>,
        ApplicationUserRoleModel, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public LafiamiContext(DbContextOptions<LafiamiContext> options)
              : base(options)
        {
        }

        public virtual DbSet<CompareLogModel> CompareLogs { get; set; }
        public virtual DbSet<VisitModel> Visits { get; set; }
        public virtual DbSet<PlanVisitModel> PlanVisits { get; set; }
        public virtual DbSet<UserSessionVisitModel> UserSessionVisits { get; set; }
        public virtual DbSet<HospitalInsurancePlanModel> HospitalInsurancePlans { get; set; }
        public virtual DbSet<HospitalModel> Hospitals { get; set; }
        public virtual DbSet<PlanCompanyExtraResultModel> PlanCompanyExtraResults { get; set; }
        public virtual DbSet<CompanyExtraModel> CompanyExtras { get; set; }
        public virtual DbSet<UserCompanyExtraResultModel> UserCompanyExtraResults { get; set; }
        public virtual DbSet<BankInformationModel> Banks { get; set; }
        public virtual DbSet<JobModel> Jobs { get; set; }
        public virtual DbSet<IdentificationModel> Identifications { get; set; }
        public virtual DbSet<NextOfKinModel> NextOfKins { get; set; }
        //public virtual DbSet<AiicoOrderModel> AiicoOrders { get; set; }
        public virtual DbSet<ReviewModel> Reviews { get; set; }
        public virtual DbSet<InsuranceCompanyModel> InsuranceCompanies { get; set; }
        public virtual DbSet<CategoryModel> Categorys { get; set; }
        public virtual DbSet<PaymentModel> Payments { get; set; }

        public virtual DbSet<CityModel> Cities { get; set; }
        public virtual DbSet<CountryModel> Countries { get; set; }

        public virtual DbSet<EmailModel> EmailLogs { get; set; }


        public virtual DbSet<OrderModel> Orders { get; set; }
        public virtual DbSet<OrderNoteModel> OrderNotes { get; set; }

        public virtual DbSet<InsurancePlanModel> InsurancePlans { get; set; }
        public virtual DbSet<InsuranceCategory> InsuranceCategories { get; set; }
        public virtual DbSet<InsurancePriceModel> InsurancePrices { get; set; }

        public virtual DbSet<CartModel> Carts { get; set; }
        public virtual DbSet<StateModel> States { get; set; }



        public virtual DbSet<WalletModel> Wallets { get; set; }
        public virtual DbSet<WalletTransactionModel> WalletTransactions { get; set; }


        public virtual DbSet<EmailTemplateModel> EmailTemplates { get; set; }


        public virtual DbSet<RatingModel> Ratings { get; set; }
        public virtual DbSet<RatingTypeModel> RatingTypes { get; set; }
        public virtual DbSet<TotalRatingModel> TotalRatings { get; set; }


        public virtual DbSet<ManageCacheModel> ManageCaches { get; set; }
        public virtual DbSet<FIndAPlanQuestionModel> FIndAPlanQuestions { get; set; }
        public virtual DbSet<FindAPlanQuestionAnswerModel> FindAPlanQuestionAnswers { get; set; }
        public virtual DbSet<InsurancePlanAnswerAsTagModel> InsurancePlanAnswerAsTags { get; set; }
        public virtual DbSet<InsuranceAuditModel> InsuranceAudits { get; set; }
        public virtual DbSet<InsuranceAuditQuestionAnswerModel> InsuranceAuditQuestionAnswers { get; set; }
        public virtual DbSet<InsuranceAuditCategoryModel> InsuranceAuditCategories { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory()) // requires Microsoft.Extensions.Configuration.Json
                   .AddJsonFile("appsettings.json") // requires Microsoft.Extensions.Configuration.Json
                    .Build();

                string connectionstring = configuration.GetConnectionString("LafiamiConnection");
                optionsBuilder.UseSqlServer(connectionstring);
            }
        }

        private static IEnumerable<UniqueKeyAttribute> GetUniqueKeyAttributes(IMutableEntityType entityType, IMutableProperty property)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }
            else if (entityType.ClrType == null)
            {
                throw new ArgumentNullException(nameof(entityType.ClrType));
            }
            else if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }
            else if (property.Name == null)
            {
                throw new ArgumentNullException(nameof(property.Name));
            }
            PropertyInfo propInfo = entityType.ClrType.GetProperty(
                property.Name,
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly);
            if (propInfo == null)
            {
                return null;
            }
            return propInfo.GetCustomAttributes<UniqueKeyAttribute>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                #region Convert UniqueKeyAttribute on Entities to UniqueKey in DB
                IEnumerable<IMutableProperty> properties = entityType.GetProperties();
                if ((properties != null) && (properties.Any()))
                {
                    foreach (IMutableProperty property in properties)
                    {
                        IEnumerable<UniqueKeyAttribute> uniqueKeys = GetUniqueKeyAttributes(entityType, property);
                        if (uniqueKeys != null)
                        {
                            foreach (UniqueKeyAttribute uniqueKey in uniqueKeys.Where(x => x.Order == 0))
                            {
                                // Single column Unique Key
                                if (string.IsNullOrWhiteSpace(uniqueKey.GroupId))
                                {
                                    entityType.AddIndex(property).IsUnique = true;
                                }
                                // Multiple column Unique Key
                                else
                                {
                                    List<IMutableProperty> mutableProperties = new List<IMutableProperty>();
                                    properties.ToList().ForEach(x =>
                                    {
                                        IEnumerable<UniqueKeyAttribute> uks = GetUniqueKeyAttributes(entityType, x);
                                        if (uks != null)
                                        {
                                            foreach (UniqueKeyAttribute uk in uks)
                                            {
                                                if ((uk != null) && (uk.GroupId == uniqueKey.GroupId))
                                                {
                                                    mutableProperties.Add(x);
                                                }
                                            }
                                        }
                                    });
                                    entityType.AddIndex(mutableProperties).IsUnique = true;
                                }
                            }
                        }
                    }
                }
                #endregion Convert UniqueKeyAttribute on Entities to UniqueKey in DB
            }

            #region Composite Keys


            modelBuilder.Entity<InsuranceCategory>()
                .HasKey(c => new { c.CategoryId, c.InsurancePlanId });

            modelBuilder.Entity<InsurancePlanAnswerAsTagModel>()
                .HasKey(c => new { c.FindAPlanQuestionAnswerId, c.InsurancePlanId });

            modelBuilder.Entity<InsuranceAuditCategoryModel>()
                .HasKey(c => new { c.CategoryId, c.InsuranceAuditId });

            modelBuilder.Entity<InsuranceAuditQuestionAnswerModel>()
                .HasKey(c => new { c.FindAPlanQuestionAnswerId, c.InsuranceAuditId });

            modelBuilder.Entity<UserCompanyExtraResultModel>()
                .HasKey(c => new { c.UserId, c.CompanyExtraId, c.OrderId });

            modelBuilder.Entity<PlanCompanyExtraResultModel>()
               .HasKey(c => new {c.CompanyExtraId, c.InsurancePlanId });

            modelBuilder.Entity<HospitalInsurancePlanModel>()
               .HasKey(c => new { c.HospitalId, c.InsurancePlanId });

            #endregion


            modelBuilder.Entity<RatingModel>(r =>
            {
                r.HasOne(t => t.User)
                    .WithMany(t => t.Ratings)
                    .HasForeignKey(t => t.UserId);
            });

            modelBuilder.Entity<ApplicationUserRoleModel>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });
                userRole.HasOne(ur => ur.Role)
                    .WithMany(ur => ur.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                   .WithMany(ur => ur.UserRoles)
                   .HasForeignKey(ur => ur.UserId)
                   .IsRequired();
            });
        }
    }
}
