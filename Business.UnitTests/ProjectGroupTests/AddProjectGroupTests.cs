using GLSoft.DoubleEntryHomeAccounting.Business.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using NSubstitute;

namespace Business.UnitTests.ProjectGroupTests;

[TestFixture]
public class AddProjectGroupTests
{
    private IUnitOfWorkFactory _unitOfWorkFactory;
    private IUnitOfWork _unitOfWork;
    private IProjectGroupRepository _groupRepository;
    private ProjectGroupService _service;

    [SetUp]
    public void SetUp()
    {
        _groupRepository = Substitute.For<IProjectGroupRepository>();

        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.GetRepository<IGroupRepository<ProjectGroup, Project>>().Returns(_groupRepository);

        _unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        _unitOfWorkFactory.Create().Returns(_unitOfWork);

        _service = new ProjectGroupService(_unitOfWorkFactory);
    }

    [TearDown]
    public void TearDown()
    {
    }

    [TestCase("Name", "Description", true, 6)]
    [TestCase("Andy", "", false, 12001)]
    [TestCase("StringName", "Words about ProjectGroup", true, 0)]
    public async Task AddProjectGroupPositiveTest(string name, string description, bool isFavorite, int maxOrder)
    {
        ProjectGroup parent = new ProjectGroup
        {
            Id = Guid.NewGuid(),
            Name = "Group"
        };
        ProjectGroup entity = null;

        _groupRepository.GetById(parent.Id).Returns(parent);
        _groupRepository.GetParentWithChildrenByParentId(parent.Id).Returns(parent);
        _groupRepository.GetMaxOrderInParent(parent.Id).Returns(maxOrder);
        await _groupRepository.Add(Arg.Do<ProjectGroup>(p => entity = p));

        GroupParam param = new GroupParam
        {
            Name = name,
            Description = description,
            IsFavorite = isFavorite,
            ParentId = parent.Id,
        };
        Guid id = await _service.Add(param);

        Assert.IsNotNull(entity);
        Assert.That(entity.Id, Is.EqualTo(id));
        Assert.IsTrue(ReferenceEquals(parent, entity.Parent));
        Assert.IsTrue(parent.Children.Contains(entity));

        Assert.That(name, Is.EqualTo(param.Name));
        Assert.That(entity.Description, Is.EqualTo(description));
        Assert.That(entity.IsFavorite, Is.EqualTo(isFavorite));
        Assert.That(entity.Order, Is.EqualTo(maxOrder + 1));
    }

    [TestCase("Name", "Description", true, 6)]
    [TestCase("Andy", "", false, 12001)]
    [TestCase("StringName", "Words about ProjectGroup", true, 0)]
    public async Task AddProjectGroupWithNullParentPositiveTest(string name, string description, bool isFavorite, int maxOrder)
    {
        ProjectGroup entity = null;

        _groupRepository.GetParentWithChildrenByParentId(default).Returns((ProjectGroup)null);
        _groupRepository.GetMaxOrderInParent(default).Returns(maxOrder);
        await _groupRepository.Add(Arg.Do<ProjectGroup>(p => entity = p));

        GroupParam param = new GroupParam
        {
            Name = name,
            Description = description,
            IsFavorite = isFavorite,
            ParentId = default,
        };
        await _service.Add(param);

        Assert.IsNotNull(entity);
        Assert.That(entity.Parent, Is.EqualTo(null));

        Assert.That(name, Is.EqualTo(param.Name));
        Assert.That(entity.Description, Is.EqualTo(description));
        Assert.That(entity.IsFavorite, Is.EqualTo(isFavorite));
        Assert.That(entity.Order, Is.EqualTo(maxOrder + 1));
    }

    [Test]
    public void AddProjectGroupCheckNullParamNegativeTest()
    {
        Assert.ThrowsAsync<NullParameterException>(async () => await _service.Add(null));
    }

    [Test]
    public void AddProjectGroupCheckNullParamNameNegativeTest()
    {
        GroupParam param = new GroupParam
        {
            Name = null,
            Description = "description",
            IsFavorite = true
        };
        Assert.ThrowsAsync<NullNameException>(async () => await _service.Add(param));
    }

    [Test]
    public void AddProjectGroupWithMissingParentNegativeTest()
    {
        _groupRepository.GetById(Arg.Any<Guid>()).Returns((ProjectGroup)null);

        GroupParam param = new GroupParam
        {
            Name = "Name",
            Description = "Description",
            IsFavorite = true,
            ParentId = Guid.NewGuid(),
        };

        Assert.ThrowsAsync<MissingEntityException>(async () => await _service.Add(param));
    }

    [Test]
    public void AddProjectGroupCheckEntityWithSameNameNegativeTest()
    {
        const string secondName = "Second Name";
        const string firstName = "First Name";

        ProjectGroup parent = new ProjectGroup
        {
            Id = Guid.NewGuid(),
            Name = "Group"
        };

        parent.Children.Add(new ProjectGroup() { Id = Guid.NewGuid(), Name = firstName });
        parent.Children.Add(new ProjectGroup() { Id = Guid.NewGuid(), Name = secondName });

        _groupRepository.GetById(parent.Id).Returns(parent);
        _groupRepository.GetParentWithChildrenByParentId(parent.Id).Returns(parent);

        GroupParam param = new GroupParam()
        {
            Name = secondName,
            Description = "description",
            IsFavorite = true,
            ParentId = parent.Id
        };
        Assert.ThrowsAsync<DuplicationNameException>(async () => await _service.Add(param));
    }
}