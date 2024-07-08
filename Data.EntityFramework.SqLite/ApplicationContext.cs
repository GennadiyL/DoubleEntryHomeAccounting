using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Config;
using Microsoft.EntityFrameworkCore;

namespace Data.EntityFramework.SqLite
{
    internal class ApplicationContext : DbContext
    {
        public DbSet<SystemConfig> SystemConfigs => Set<SystemConfig>();
        public DbSet<UserConfig> UserConfigs => Set<UserConfig>();
        public DbSet<Currency> Currencies => Set<Currency>();
        public DbSet<CurrencyRate> CurrencyRates => Set<CurrencyRate>();
        public DbSet<CategoryGroup> CategoryGroups => Set<CategoryGroup>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<CorrespondentGroup> CorrespondentGroups => Set<CorrespondentGroup>();
        public DbSet<Correspondent> Correspondents => Set<Correspondent>();
        public DbSet<ProjectGroup> ProjectGroups => Set<ProjectGroup>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<AccountGroup> AccountGroups => Set<AccountGroup>();
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<TemplateGroup> TemplateGroups => Set<TemplateGroup>();
        public DbSet<Template> Templates => Set<Template>();
        public DbSet<TemplateEntry> TemplateEntries => Set<TemplateEntry>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<TransactionEntry> TransactionEntries => Set<TransactionEntry>();
    }
}
