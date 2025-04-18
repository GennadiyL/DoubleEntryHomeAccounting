using GLSoft.DoubleEntryHomeAccounting.Business.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using NSubstitute;
using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;

namespace Business.UnitTests.CategoryGroupTests;

[TestFixture]
public class UpdateCategoryGroupTests
{
    private IUnitOfWorkFactory _unitOfWorkFactory;
    private IUnitOfWork _unitOfWork;
    private ICategoryGroupRepository _groupRepository;
    private CategoryGroupService _service;

    [SetUp]
    public void SetUp()
    {
        _groupRepository = Substitute.For<ICategoryGroupRepository>();

        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.GetRepository<IGroupRepository<CategoryGroup, Category>>().Returns(_groupRepository);

        _unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        _unitOfWorkFactory.Create().Returns(_unitOfWork);

        _service = new CategoryGroupService(_unitOfWorkFactory);
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork?.Dispose();
    }

    [TestCase("Name", "Description", true, "Mom", "All", false)]
    [TestCase("Dad", "For all family", false, "Child", "school", true)]
    public async Task UpdateCategoryGroupPositiveTest(
        string newName, string newDescription, bool newIsFavorite,
        string originalName, string originalDescription, bool originalIsFavorite)
    {
        Guid id = Guid.NewGuid();

        CategoryGroup parent = new CategoryGroup
        {
            Id = Guid.NewGuid(),
            Name = "Parent",
            Description = "description",
            IsFavorite = false,
        };

        CategoryGroup entity = new CategoryGroup
        {
            Name = originalName,
            Description = originalDescription,
            IsFavorite = originalIsFavorite,
            ParentId = parent.Id
        };

        _groupRepository.GetById(id).Returns(entity);
        _groupRepository.GetParentWithChildrenByParentId(entity.ParentId).Returns(parent);

        GroupParam param = new GroupParam
        {
            Name = newName,
            Description = newDescription,
            IsFavorite = newIsFavorite
        };

        await _service.Update(id, param);

        Assert.That(entity.Name, Is.EqualTo(param.Name));
        Assert.That(entity.Description, Is.EqualTo(param.Description));
        Assert.That(entity.IsFavorite, Is.EqualTo(param.IsFavorite));
    }

    [TestCase("Name", "Description", true, "Mom", "All", false)]
    [TestCase("Dad", "For all family", false, "Child", "school", true)]
    public async Task UpdateCategoryGroupWithNullParentPositiveTest(
        string newName, string newDescription, bool newIsFavorite,
        string originalName, string originalDescription, bool originalIsFavorite)
    {
        Guid id = Guid.NewGuid();

        CategoryGroup entity = new CategoryGroup
        {
            Name = originalName,
            Description = originalDescription,
            IsFavorite = originalIsFavorite,
            ParentId = default
        };

        _groupRepository.GetById(id).Returns(entity);
        _groupRepository.GetParentWithChildrenByParentId(default).Returns((CategoryGroup)null);

        GroupParam param = new GroupParam
        {
            Name = newName,
            Description = newDescription,
            IsFavorite = newIsFavorite
        };

        await _service.Update(id, param);

        Assert.That(entity.Name, Is.EqualTo(param.Name));
        Assert.That(entity.Description, Is.EqualTo(param.Description));
        Assert.That(entity.IsFavorite, Is.EqualTo(param.IsFavorite));
    }


    [TestCase("Description", true, "Mom", "All", false)]
    [TestCase("For all family", false, "Child", "school", true)]
    public async Task UpdateCategoryGroupCheckWithSameNameButSelfParentPositiveTest(
        string newDescription, bool newIsFavorite,
        string originalName, string originalDescription, bool originalIsFavorite)
    {
        Guid id = Guid.NewGuid();

        CategoryGroup parent = new CategoryGroup
        {
            Id = Guid.NewGuid(),
            Name = "Parent",
            Description = "description",
            IsFavorite = false,
        };

        CategoryGroup entity = new CategoryGroup
        {
            Name = originalName,
            Description = originalDescription,
            IsFavorite = originalIsFavorite,
            ParentId = parent.Id
        };

        _groupRepository.GetById(id).Returns(entity);
        _groupRepository.GetParentWithChildrenByParentId(entity.ParentId).Returns(parent);

        GroupParam param = new GroupParam
        {
            Name = originalName,
            Description = newDescription,
            IsFavorite = newIsFavorite
        };

        await _service.Update(id, param);

        Assert.That(entity.Name, Is.EqualTo(param.Name));
        Assert.That(entity.Description, Is.EqualTo(param.Description));
        Assert.That(entity.IsFavorite, Is.EqualTo(param.IsFavorite));
    }

    [Test]
    public void UpdateCategoryGroupCheckNullParamNegativeTest()
    {
        Assert.ThrowsAsync<NullParameterException>(async () => await _service.Update(Guid.NewGuid(), null));
    }

    [Test]
    public void UpdateCategoryGroupCheckNullParamNameNegativeTest()
    {
        GroupParam param = new GroupParam
        {
            Name = null,
            Description = "description",
            IsFavorite = true
        };
        Assert.ThrowsAsync<NullNameException>(async () => await _service.Update(Guid.NewGuid(), param));
    }

    [Test]
    public void UpdateCategoryGroupWithMissingEntityNegativeTest()
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

        GroupParam param = new GroupParam
        {
            Name = null,
            Description = "description",
            IsFavorite = true
        };

        Assert.ThrowsAsync<NullNameException>(async () => await _service.Update(passedIdGuid, param));
    }

    [Test]
    public void UpdateCategoryGroupWithMissingParentNegativeTest()
    {
        _groupRepository.GetById(Arg.Any<Guid>()).Returns((CategoryGroup)null);

        GroupParam param = new GroupParam
        {
            Name = "Name",
            Description = "Description",
            IsFavorite = true,
            ParentId = Guid.NewGuid(),
        };

        Assert.ThrowsAsync<MissingEntityException>(async () => await _service.Update(Guid.NewGuid(), param));
    }

    [Test]
    public void UpdateCategoryGroupCheckWithSameNameNegativeTest()
    {
        const string firstName = "First Name";
        const string secondName = "Second Name";

        CategoryGroup parent = new CategoryGroup
        {
            Id = Guid.NewGuid(),
            Name = "Group"
        };

        CategoryGroup child1 = new CategoryGroup { Id = Guid.NewGuid(), Name = firstName, ParentId = parent.Id };
        CategoryGroup child2 = new CategoryGroup { Id = Guid.NewGuid(), Name = secondName, ParentId = parent.Id };
        parent.Children.Add(child1);
        parent.Children.Add(child2);

        _groupRepository.GetById(child1.Id).Returns(child1);
        _groupRepository.GetById(child2.Id).Returns(child2);
        _groupRepository.GetParentWithChildrenByParentId(parent.Id).Returns(parent);

        var param = new GroupParam
        {
            Name = secondName,
            Description = "description",
            IsFavorite = true,
            ParentId = parent.Id
        };
        Assert.ThrowsAsync<DuplicationNameException>(async () => await _service.Update(child1.Id, param));
    }
}