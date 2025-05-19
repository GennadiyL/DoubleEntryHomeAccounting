using GLSoft.DoubleEntryHomeAccounting.Business.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using NSubstitute;

namespace Business.UnitTests.AccountGroupTests;

[TestFixture]
public class UpdateAccountGroupTests
{
    private IUnitOfWorkFactory _unitOfWorkFactory;
    private IUnitOfWork _unitOfWork;
    private IAccountGroupRepository _groupRepository;
    private AccountGroupService _service;

    [SetUp]
    public void SetUp()
    {
        _groupRepository = Substitute.For<IAccountGroupRepository>();

        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.GetRepository<IGroupRepository<AccountGroup, Account>>().Returns(_groupRepository);

        _unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        _unitOfWorkFactory.Create().Returns(_unitOfWork);

        _service = new AccountGroupService(_unitOfWorkFactory);
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork?.Dispose();
    }

    [TestCase("Name", "Description", true, "Mom", "All", false)]
    [TestCase("Dad", "For all family", false, "Child", "school", true)]
    public async Task UpdateAccountGroupPositiveTest(
        string newName, string newDescription, bool newIsFavorite,
        string originalName, string originalDescription, bool originalIsFavorite)
    {
        Guid id = Guid.NewGuid();

        AccountGroup parent = new AccountGroup
        {
            Id = Guid.NewGuid(),
            Name = "Parent",
            Description = "description",
            IsFavorite = false,
        };

        AccountGroup entity = new AccountGroup
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
    public async Task UpdateAccountGroupWithNullParentPositiveTest(
        string newName, string newDescription, bool newIsFavorite,
        string originalName, string originalDescription, bool originalIsFavorite)
    {
        Guid id = Guid.NewGuid();

        AccountGroup entity = new AccountGroup
        {
            Name = originalName,
            Description = originalDescription,
            IsFavorite = originalIsFavorite,
            ParentId = default
        };

        _groupRepository.GetById(id).Returns(entity);
        _groupRepository.GetParentWithChildrenByParentId(default).Returns((AccountGroup)null);

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
    public async Task UpdateAccountGroupCheckWithSameNameButSelfParentPositiveTest(
        string newDescription, bool newIsFavorite,
        string originalName, string originalDescription, bool originalIsFavorite)
    {
        Guid id = Guid.NewGuid();

        AccountGroup parent = new AccountGroup
        {
            Id = Guid.NewGuid(),
            Name = "Parent",
            Description = "description",
            IsFavorite = false,
        };

        AccountGroup entity = new AccountGroup
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
    public void UpdateAccountGroupCheckNullParamNegativeTest()
    {
        Assert.ThrowsAsync<NullParameterException>(async () => await _service.Update(Guid.NewGuid(), null));
    }

    [Test]
    public void UpdateAccountGroupCheckNullParamNameNegativeTest()
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
    public void UpdateAccountGroupWithMissingEntityNegativeTest()
    {
        AccountGroup entity = new AccountGroup
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
    public void UpdateAccountGroupWithMissingParentNegativeTest()
    {
        _groupRepository.GetById(Arg.Any<Guid>()).Returns((AccountGroup)null);

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
    public void UpdateAccountGroupCheckWithSameNameNegativeTest()
    {
        const string firstName = "First Name";
        const string secondName = "Second Name";

        AccountGroup parent = new AccountGroup
        {
            Id = Guid.NewGuid(),
            Name = "Group"
        };

        AccountGroup child1 = new AccountGroup { Id = Guid.NewGuid(), Name = firstName, ParentId = parent.Id };
        AccountGroup child2 = new AccountGroup { Id = Guid.NewGuid(), Name = secondName, ParentId = parent.Id };
        parent.Children.Add(child1);
        parent.Children.Add(child2);

        _groupRepository.GetById(child1.Id).Returns(child1);
        _groupRepository.GetById(child2.Id).Returns(child2);
        _groupRepository.GetParentWithChildrenByParentId(parent.Id).Returns(parent);

        GroupParam param = new GroupParam
        {
            Name = secondName,
            Description = "description",
            IsFavorite = true,
            ParentId = parent.Id
        };
        Assert.ThrowsAsync<DuplicationNameException>(async () => await _service.Update(child1.Id, param));
    }
}