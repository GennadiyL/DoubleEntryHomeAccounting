using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Utils.Models;

public class Roots
{
    public static readonly Guid AccountGroupId = new Guid("22D0BBCC-37EC-4AF4-B5C7-9CAF34FEC1E9");
    public static readonly Guid CategoryGroupId = new Guid("CDB033F6-8686-4222-B33F-66B5A3BF2948");
    public static readonly Guid CorrespondentGroupId = new Guid("9C19A1FE-9C57-4703-9105-79075987EE45");
    public static readonly Guid ProjectGroupId = new Guid("7C9385F6-28F2-4947-8B44-E81DD8D01949");
    public static readonly Guid TemplateGroupId = new Guid("B232A84F-47D8-426E-AA54-BA8771B8B6DE");

    public static AccountGroup AccountGroup { get; } = CreateAccountGroup();
    public static CategoryGroup CategoryGroup { get; } = CreateCategoryGroup();
    public static CorrespondentGroup CorrespondentGroup { get; } = CreateCorrespondentGroup();
    public static ProjectGroup ProjectGroup { get; } = CreateProjectGroup();
    public static TemplateGroup TemplateGroup { get; } = CreateTemplateGroup();

    private static HashSet<Guid> RootIds { get; } = new HashSet<Guid>()
    {
        AccountGroupId,
        CategoryGroupId,
        CorrespondentGroupId,
        ProjectGroupId,
        TemplateGroupId
    };

    private static AccountGroup CreateAccountGroup()
    {
        AccountGroup group = new AccountGroup
        {
            Id = AccountGroupId,
            ParentId = AccountGroupId,
        };
        group.Parent = group;
        return group;
    }
    
    private static CategoryGroup CreateCategoryGroup()
    {
        CategoryGroup group = new CategoryGroup
        {
            Id = CategoryGroupId,
            ParentId = CategoryGroupId,
        };
        group.Parent = group;
        return group;
    }

    private static CorrespondentGroup CreateCorrespondentGroup()
    {
        CorrespondentGroup group = new CorrespondentGroup
        {
            Id = CorrespondentGroupId,
            ParentId = CorrespondentGroupId,
        };
        group.Parent = group;
        return group;
    }

    private static ProjectGroup CreateProjectGroup()
    {
        ProjectGroup group = new ProjectGroup
        {
            Id = ProjectGroupId,
            ParentId = ProjectGroupId,
        };
        group.Parent = group;
        return group;
    }

    private static TemplateGroup CreateTemplateGroup()
    {
        TemplateGroup group = new TemplateGroup
        {
            Id = TemplateGroupId,
            ParentId = TemplateGroupId,
        };
        group.Parent = group;
        return group;
    }

    public static bool IsRoot<TGroup, TElement>(TGroup group)
        where TGroup : class, IGroupEntity<TGroup, TElement>
        where TElement : class, IElementEntity<TGroup, TElement> =>
        RootIds.Contains(group.Id);
}