using GLSoft.DoubleEntryHomeAccounting.Business.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using NSubstitute;
using System.Linq;

namespace Business.UnitTests.CategoryGroupTests;

[TestFixture]
public class DeleteCategoryGroupTests
{
    private IUnitOfWorkFactory _unitOfWorkFactory;
    private IUnitOfWork _unitOfWork;
    private ICategoryGroupRepository _groupRepository;
    private ICategoryRepository _elementRepository;
    private CategoryGroupService _service;

    [SetUp]
    public void SetUp()
    {
        _groupRepository = Substitute.For<ICategoryGroupRepository>();
        _elementRepository = Substitute.For<ICategoryRepository>();

        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.GetRepository<IGroupEntityRepository<CategoryGroup, Category>>().Returns(_groupRepository);
        _unitOfWork.GetRepository<IElementEntityRepository<CategoryGroup, Category>>().Returns(_elementRepository);

        _unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        _unitOfWorkFactory.Create().Returns(_unitOfWork);

        _service = new CategoryGroupService(_unitOfWorkFactory);
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork?.Dispose();
    }

    [Test]
    public async Task DeleteCategoryGroupPositiveTest()
    {
        Guid deletedId = Guid.Empty;
        CategoryGroup parent = new CategoryGroup
        {
            Id = Guid.NewGuid(),
            Name = "Parent",
            Description = "description",
            IsFavorite = false,
        };
        CategoryGroup child1 = new CategoryGroup { Id = Guid.NewGuid(), Name = "firstName", ParentId = parent.Id, Order = 1};
        CategoryGroup child2 = new CategoryGroup { Id = Guid.NewGuid(), Name = "secondName", ParentId = parent.Id, Order = 2 };
        parent.Children.Add(child1);
        parent.Children.Add(child2);

        _groupRepository.GetById(child1.Id).Returns(child1);
        _groupRepository.GetByParentId(child1.ParentId).Returns(parent);
        await _groupRepository.Delete(Arg.Do<Guid>(e => deletedId = e));

        await _service.Delete(child1.Id);

        Assert.That(parent.Children.Count, Is.EqualTo(1));
        Assert.That(parent.Children.First().Order, Is.EqualTo(1));
        Assert.That(child1.Id, Is.EqualTo(deletedId));

    }

    [Test]
    public async Task DeleteCategoryGroupWithNullParentPositiveTest()
    {
        Guid deletedId = Guid.Empty;
        CategoryGroup child1 = new CategoryGroup { Id = Guid.NewGuid(), Name = "firstName", ParentId = default, Order = 1 };
        CategoryGroup child2 = new CategoryGroup { Id = Guid.NewGuid(), Name = "secondName", ParentId = default, Order = 2 };

        _groupRepository.GetById(child1.Id).Returns(child1);
        _groupRepository.Where(Arg.Any<Func<CategoryGroup, bool>>()).Returns(new List<CategoryGroup>() {child2});
        await _groupRepository.Delete(Arg.Do<Guid>(e => deletedId = e));

        await _service.Delete(child1.Id);

        Assert.That(child2.Order, Is.EqualTo(1));
        Assert.That(child1.Id, Is.EqualTo(deletedId));
    }

    //[Test]
    //public void DeleteCategoryGroupCheckAndGetEntityByIdExceptionTest()
    //{
    //    var deletedEntity = new CategoryGroup();

    //    var mockEntityDataAccess = new Mock<ICategoryGroupDataAccess>();
    //    mockEntityDataAccess.Setup(eda => eda.Get(It.IsAny<Guid>())).Returns(() => null);
    //    var service = new CategoryGroupService(CreateMockGlobalDataAccess(), mockEntityDataAccess.Object,
    //        CreateMockChildEntityDataAccess());

    //    Assert.ThrowsAsync<ArgumentNullException>(async () => await service.Delete(deletedEntity.Id));
    //}

    //[Test]
    //public void DeleteCategoryGroupСheckExistedChildrenInTheGroupExceptionTest()
    //{
    //    var deletedEntity = new CategoryGroup();
    //    var mockEntityDataAccess = new Mock<ICategoryGroupDataAccess>();
    //    mockEntityDataAccess.Setup(eda => eda.Get(It.IsAny<Guid>())).Returns(() => deletedEntity);
    //    var service = new CategoryGroupService(CreateMockGlobalDataAccess(), mockEntityDataAccess.Object,
    //        CreateMockChildEntityDataAccessForException());

    //    Assert.ThrowsAsync<ArgumentException>(async () => await service.Delete(deletedEntity.Id));
    //}

    //private IGlobalDataAccess CreateMockGlobalDataAccess()
    //{
    //    var mockGlobalDataAccess = new Mock<IGlobalDataAccess>();
    //    mockGlobalDataAccess.Setup(gda => gda.Load());
    //    mockGlobalDataAccess.Setup(gda => gda.Save());
    //    mockGlobalDataAccess.Setup(gda => gda.Get(It.IsAny<Guid>())).Returns(() => null);
    //    return mockGlobalDataAccess.Object;
    //}

    //private ICategoryDataAccess CreateMockChildEntityDataAccessForException()
    //{
    //    var mockChildEntityDataAccess = new Mock<ICategoryDataAccess>();
    //    mockChildEntityDataAccess.Setup(cda => cda.GetCount(It.IsAny<Guid>())).Returns(() => 5);
    //    return mockChildEntityDataAccess.Object;
    //}

    //private ICategoryDataAccess CreateMockChildEntityDataAccess()
    //{
    //    var mockChildEntityDataAccess = new Mock<ICategoryDataAccess>();
    //    mockChildEntityDataAccess.Setup(cda => cda.GetCount(It.IsAny<Guid>())).Returns(() => 0);
    //    return mockChildEntityDataAccess.Object;
    //}
}