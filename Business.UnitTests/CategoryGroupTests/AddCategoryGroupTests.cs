using GLSoft.DoubleEntryHomeAccounting.Business.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using NSubstitute;

namespace Business.UnitTests.CategoryGroupTests;

[TestFixture]
public class AddCategoryGroupTests
{
    [TestCase("Name", "Description", true, 6)]
    [TestCase("Andy", "", false, 12001)]
    [TestCase("StringName", "Words about CategoryGroup", true, 0)]
    public async Task AddCategoryGroupPositiveTest(string name, string description, bool isFavorite, int maxOrder)
    {
        CategoryGroup parent = new CategoryGroup
        {
            Id = Guid.NewGuid(),
            Name = "Group"
        };
        CategoryGroup entity = null;

        IGroupEntityRepository<CategoryGroup, Category> groupRepository = Substitute.For<IGroupEntityRepository<CategoryGroup, Category>>();
        groupRepository.GetById(parent.Id, null).Returns(parent);
        groupRepository.GetByParentId(parent.Id).Returns(parent);
        groupRepository.GetMaxOrder(parent.Id).Returns(maxOrder);
        await groupRepository.Add(Arg.Do<CategoryGroup>(p => entity = p));

        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.GetRepository<IGroupEntityRepository<CategoryGroup,Category>>().Returns(groupRepository);

        IUnitOfWorkFactory unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        unitOfWorkFactory.Create().Returns(unitOfWork);

        var service = new CategoryGroupService(unitOfWorkFactory);
        GroupParam groupParam = new GroupParam
        {
            Name = name,
            Description = description,
            IsFavorite = isFavorite,
            ParentId = parent.Id,
        };

        await service.Add(groupParam);

        Assert.IsNotNull(entity);
        Assert.IsTrue(ReferenceEquals(parent, entity.Parent));
        Assert.IsTrue(parent.Children.Contains(entity));

        Assert.That(name, Is.EqualTo(groupParam.Name));
        Assert.That(entity.Description, Is.EqualTo(description));
        Assert.That(entity.IsFavorite, Is.EqualTo(isFavorite));
        Assert.That(entity.Order, Is.EqualTo(maxOrder + 1));
    }

    //TODO: AddCategoryGroupWithNullParentPositiveTest

    //TODO: AddCategoryGroupWithMissingParentNegativeTest

    //    [Test]
    //    public void AddCategoryGroupCheckEntityNullNegativeTest()
    //    {
    //        var mockEntityDataAccess = new Mock<ICategoryGroupDataAccess>();

    //        var service = new CategoryGroupService(CreateMockGlobalDataAccess(), mockEntityDataAccess.Object,
    //            CreateMockChildEntityDataAccess());
    //        CategoryGroup entity = null;

    //        Assert.ThrowsAsync<ArgumentNullException>(async () => await service.Add(entity));
    //    }

    //    [Test]
    //    public void AddCategoryGroupCheckEntityNameNullNegativeTest()
    //    {
    //        var mockEntityDataAccess = new Mock<ICategoryGroupDataAccess>();

    //        var service = new CategoryGroupService(CreateMockGlobalDataAccess(), mockEntityDataAccess.Object,
    //            CreateMockChildEntityDataAccess());
    //        var entity = new CategoryGroup
    //        {
    //            Name = null,
    //            Description = "description",
    //            IsFavorite = true
    //        };

    //        Assert.ThrowsAsync<ArgumentNullException>(async () => await service.Add(entity));
    //    }

    //    [Test]
    //    public void ExceptionAddCategoryGroupCheckEntityWithSameNameNegativeTest()
    //    {
    //        var entity = new CategoryGroup
    //        {
    //            Name = "name",
    //            Description = "description",
    //            IsFavorite = true
    //        };

    //        var mockEntityDataAccess = new Mock<ICategoryGroupDataAccess>();
    //        mockEntityDataAccess.Setup(eda => eda.GetByName(It.IsAny<string>()))
    //            .Returns<string>(s => new List<CategoryGroup> { new() { Name = s } });

    //        var service = new CategoryGroupService(CreateMockGlobalDataAccess(), mockEntityDataAccess.Object,
    //            CreateMockChildEntityDataAccess());

    //        Assert.ThrowsAsync<ArgumentNullException>(async () => await service.Add(entity));
    //    }



    //    private ICategoryDataAccess CreateMockChildEntityDataAccess()
    //    {
    //        var mockChildEntityDataAccess = new Mock<ICategoryDataAccess>();
    //        return mockChildEntityDataAccess.Object;
    //    }

    //    private IGlobalDataAccess CreateMockGlobalDataAccess()
    //    {
    //        var mockGlobalDataAccess = new Mock<IGlobalDataAccess>();
    //        mockGlobalDataAccess.Setup(gda => gda.Load());
    //        mockGlobalDataAccess.Setup(gda => gda.Save());
    //        mockGlobalDataAccess.Setup(gda => gda.Get(It.IsAny<Guid>())).Returns(() => null);
    //        return mockGlobalDataAccess.Object;
    //    }
}