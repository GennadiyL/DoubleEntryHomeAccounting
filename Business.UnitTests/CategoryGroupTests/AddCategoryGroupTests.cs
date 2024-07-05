using GLSoft.DoubleEntryHomeAccounting.Business.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using NSubstitute;

namespace Business.UnitTests.CategoryGroupTests;

[TestFixture]
public class AddCategoryGroupTests
{
    //[TestCase("Name", "Description", true, 6)]
    //[TestCase("A", "", false, 12001)]
    //[TestCase("StringName", "Words about CategoryGroup", true, 0)]
    //public async Task AddCategoryGroupPositiveTest(string name, string description, bool isFavorite, int maxOrder)
    //{
    //    CategoryGroup addedEntity = null;


    //    ICategoryGroupRepository categoryGroupRepository = Substitute.For<ICategoryGroupRepository>();
    //    IUnitOfWorkFactory unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
    //    ICategoryGroupRepository groupRepository,
    //    ICategoryRepository elementRepository

    //    categoryGroupRepository.Setup(eda => eda.GetByName(It.IsAny<string>())).Returns(() => new List<CategoryGroup>());
    //    categoryGroupRepository.Setup(eda => eda.GetMaxOrder()).Returns(() => maxOrder);
    //    categoryGroupRepository.Setup(eda => eda.Add(It.IsAny<CategoryGroup>()))
    //        .Callback<CategoryGroup>(cg => addedEntity = cg);

    //    var service = new CategoryGroupService(CreateMockGlobalDataAccess(), categoryGroupRepository.Object,
    //        CreateMockChildEntityDataAccess());
    //    GroupParam category = new GroupParam
    //    {
    //        Name = name,
    //        Description = description,
    //        IsFavorite = isFavorite,
    //        ParentId = Guid.NewGuid(),
    //    };

    //    await service.Add(category);

    //    Assert.IsNotNull(addedEntity);
    //    Assert.IsFalse(ReferenceEquals(addedEntity,
    //        entity)); //checking that it was created a copy, instead the origin was posted

    //    Assert.AreEqual(addedEntity.Id, entity.Id);
    //    Assert.AreEqual(addedEntity.Name, entity.Name);
    //    Assert.AreEqual(addedEntity.Description, entity.Description);
    //    Assert.AreEqual(addedEntity.IsFavorite, entity.IsFavorite);
    //    Assert.AreEqual(addedEntity.Order, maxOrder + 1);
    //}

    //    [Test]
    //    public void ExceptionAddCategoryGroupCheckEntityNullTest()
    //    {
    //        var mockEntityDataAccess = new Mock<ICategoryGroupDataAccess>();

    //        var service = new CategoryGroupService(CreateMockGlobalDataAccess(), mockEntityDataAccess.Object,
    //            CreateMockChildEntityDataAccess());
    //        CategoryGroup entity = null;

    //        Assert.ThrowsAsync<ArgumentNullException>(async () => await service.Add(entity));
    //    }

    //    [Test]
    //    public void ExceptionAddCategoryGroupCheckEntityNameNullTest()
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
    //    public void ExceptionAddCategoryGroupCheckEntityWithSameIdTest()
    //    {
    //        var entity = new CategoryGroup
    //        {
    //            Id = Guid.NewGuid(),
    //            Name = "name",
    //            Description = "description",
    //            IsFavorite = true
    //        };

    //        var mockEntityDataAccess = new Mock<ICategoryGroupDataAccess>();

    //        var mockGlobalDataAccess = new Mock<IGlobalDataAccess>();
    //        mockGlobalDataAccess.Setup(gda => gda.Load());
    //        mockGlobalDataAccess.Setup(gda => gda.Save());
    //        mockGlobalDataAccess.Setup(gda => gda.Get(It.IsAny<Guid>())).Returns<Guid>(id => new Currency { Id = id });

    //        var service = new CategoryGroupService(mockGlobalDataAccess.Object, mockEntityDataAccess.Object,
    //            CreateMockChildEntityDataAccess());

    //        Assert.ThrowsAsync<ArgumentException>(async () => await service.Add(entity));
    //    }

    //    [Test]
    //    public void ExceptionAddCategoryGroupCheckEntityWithSameNameTest()
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