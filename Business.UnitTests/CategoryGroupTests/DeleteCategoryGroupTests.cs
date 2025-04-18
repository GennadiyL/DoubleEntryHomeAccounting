using GLSoft.DoubleEntryHomeAccounting.Business.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
using NSubstitute;


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
        _unitOfWork.GetRepository<IGroupRepository<CategoryGroup, Category>>().Returns(_groupRepository);
        _unitOfWork.GetRepository<IElementRepository<CategoryGroup, Category>>().Returns(_elementRepository);

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
        _groupRepository.GetParentWithChildrenByParentId(child1.ParentId).Returns(parent);
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

    [Test]
    public void DeleteCategoryGroupWithMissingEntityNegativeTest()
    {
        CategoryGroup entity = new CategoryGroup
        {
            Name = "originalName",
            Description = "originalDescription",
            IsFavorite = true,
            ParentId = default
        };
        Guid entityIdGuid = Guid.NewGuid();
        Guid passedIdGuid = Guid.NewGuid();

        _groupRepository.GetById(entityIdGuid).Returns(entity);

        Assert.ThrowsAsync<MissingEntityException>(async () => await _service.Delete(passedIdGuid));
    }

    [Test]
    public void DeleteCategoryGroupWithExistedSubGroupsNegativeTest()
    {
        CategoryGroup entity = new CategoryGroup
        {
            Name = "originalName",
            Description = "originalDescription",
            IsFavorite = true,
            ParentId = default
        };
        CategoryGroup child1 = new CategoryGroup { Id = Guid.NewGuid(), Name = "firstName", ParentId = entity.Id, Order = 1 };
        CategoryGroup child2 = new CategoryGroup { Id = Guid.NewGuid(), Name = "secondName", ParentId = entity.Id, Order = 2 };
        entity.Children.Add(child1);
        entity.Children.Add(child2);

        _groupRepository.GetById(entity.Id).Returns(entity);
        _groupRepository.GetCountInGroup(entity.Id).Returns(entity.Children.Count);

        Assert.ThrowsAsync<GroupContainsSubGroupsException>(async () => await _service.Delete(entity.Id));
    }

    [Test]
    public void DeleteCategoryGroupWithExistedElementsNegativeTest()
    {
        CategoryGroup entity = new CategoryGroup
        {
            Name = "originalName",
            Description = "originalDescription",
            IsFavorite = true,
            ParentId = default
        };
        Category child1 = new Category { Id = Guid.NewGuid(), Name = "firstName", GroupId = entity.Id, Order = 1 };
        Category child2 = new Category { Id = Guid.NewGuid(), Name = "secondName", GroupId = entity.Id, Order = 2 };
        entity.Elements.Add(child1);
        entity.Elements.Add(child2);

        _groupRepository.GetById(entity.Id).Returns(entity);
        _elementRepository.GetCountInGroup(entity.Id).Returns(entity.Elements.Count);

        Assert.ThrowsAsync<GroupContainsElementException>(async () => await _service.Delete(entity.Id));
    }

    //TODO: Cannot find parent
}