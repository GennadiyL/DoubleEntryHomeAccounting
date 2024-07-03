namespace GLSoft.DoubleEntryHomeAccounting.Common.Infos;

public class LedgerInfo
{
    public List<AccountGroupInfo> AccountGroups { get; } = new();
    public List<AccountInfo> Accounts { get; } = new();
    public List<CategoryGroupInfo> CategoryGroups { get; } = new();
    public List<CategoryInfo> Categories { get; } = new();
    public List<CorrespondentGroupInfo> CorrespondentGroups { get; } = new();
    public List<CorrespondentInfo> Correspondents { get; } = new();
    public List<CurrencyInfo> Currencies { get; } = new();
    public List<CurrencyRateInfo> CurrencyRates { get; } = new();
    public List<ProjectGroupInfo> ProjectGroups { get; } = new();
    public List<ProjectInfo> Projects { get; } = new();
    public List<TemplateGroupInfo> TemplateGroups { get; } = new();
    public List<TemplateInfo> Templates { get; } = new();
    public List<TemplateEntryInfo> TemplateEntries { get; } = new();
    public List<TransactionInfo> Transactions { get; } = new();
    public List<TransactionEntryInfo> TransactionEntries { get; } = new();
}