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
    private IUnitOfWorkFactory _unitOfWorkFactory;
    private IUnitOfWork _unitOfWork;
    private IGroupEntityRepository<CategoryGroup, Category> _groupRepository;

    [SetUp]
    public void SetUp()
    {
        _groupRepository = Substitute.For<IGroupEntityRepository<CategoryGroup, Category>>();

        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.GetRepository<IGroupEntityRepository<CategoryGroup, Category>>().Returns(_groupRepository);

        _unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        _unitOfWorkFactory.Create().Returns(_unitOfWork);
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork?.Dispose();
    }

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

        _groupRepository.GetById(parent.Id, null).Returns(parent);
        _groupRepository.GetByParentId(parent.Id).Returns(parent);
        _groupRepository.GetMaxOrder(parent.Id).Returns(maxOrder);
        await _groupRepository.Add(Arg.Do<CategoryGroup>(p => entity = p));

        CategoryGroupService service = new CategoryGroupService(_unitOfWorkFactory);
        GroupParam param = new GroupParam
        {
            Name = name,
            Description = description,
            IsFavorite = isFavorite,
            ParentId = parent.Id,
        };

        await service.Add(param);

        Assert.IsNotNull(entity);
        Assert.IsTrue(ReferenceEquals(parent, entity.Parent));
        Assert.IsTrue(parent.Children.Contains(entity));

        Assert.That(name, Is.EqualTo(param.Name));
        Assert.That(entity.Description, Is.EqualTo(description));
        Assert.That(entity.IsFavorite, Is.EqualTo(isFavorite));
        Assert.That(entity.Order, Is.EqualTo(maxOrder + 1));
    }

    [TestCase("Name", "Description", true, 6)]
    [TestCase("Andy", "", false, 12001)]
    [TestCase("StringName", "Words about CategoryGroup", true, 0)]
    public async Task AddCategoryGroupWithNullParentPositiveTest(string name, string description, bool isFavorite, int maxOrder)
    {
        CategoryGroup entity = null;

        _groupRepository.GetByParentId(default).Returns((CategoryGroup)null);
        _groupRepository.GetMaxOrder(default).Returns(maxOrder);
        await _groupRepository.Add(Arg.Do<CategoryGroup>(p => entity = p));

        CategoryGroupService service = new CategoryGroupService(_unitOfWorkFactory);
        GroupParam param = new GroupParam
        {
            Name = name,
            Description = description,
            IsFavorite = isFavorite,
            ParentId = null,
        };

        await service.Add(param);

        Assert.IsNotNull(entity);
        Assert.That(entity.Parent, Is.EqualTo(null));

        Assert.That(name, Is.EqualTo(param.Name));
        Assert.That(entity.Description, Is.EqualTo(description));
        Assert.That(entity.IsFavorite, Is.EqualTo(isFavorite));
        Assert.That(entity.Order, Is.EqualTo(maxOrder + 1));
    }


    //TODO: AddCategoryGroupWithMissingParentNegativeTest

    [Test]
    public void AddCategoryGroupCheckEntityNullNegativeTest()
    {
        CategoryGroupService service = new CategoryGroupService(_unitOfWorkFactory);

        Assert.ThrowsAsync<ArgumentNullException>(async () => await service.Add(null));
    }

    [Test]
    public void AddCategoryGroupCheckEntityNameNullNegativeTest()
    {
        GroupParam param = new GroupParam
        {
            Name = null,
            Description = "description",
            IsFavorite = true
        };

        CategoryGroupService service = new CategoryGroupService(_unitOfWorkFactory);

        Assert.ThrowsAsync<ArgumentNullException>(async () => await service.Add(param));
    }

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