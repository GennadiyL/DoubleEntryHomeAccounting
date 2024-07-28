using GLSoft.DoubleEntryHomeAccounting.Business.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using NSubstitute;

namespace Business.UnitTests.ProjectGroupTests;

[TestFixture]
public class UpdateProjectGroupTests
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
        _unitOfWork.GetRepository<IGroupEntityRepository<ProjectGroup, Project>>().Returns(_groupRepository);

        _unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        _unitOfWorkFactory.Create().Returns(_unitOfWork);

        _service = new ProjectGroupService(_unitOfWorkFactory);
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork?.Dispose();
    }

    [TestCase("Name", "Description", true, "Mom", "All", false)]
    [TestCase("Dad", "For all family", false, "Child", "school", true)]
    public async Task UpdateProjectGroupPositiveTest(
        string newName, string newDescription, bool newIsFavorite,
        string originalName, string originalDescription, bool originalIsFavorite)
    {
        Guid id = Guid.NewGuid();

        ProjectGroup parent = new ProjectGroup
        {
            Id = Guid.NewGuid(),
            Name = "Parent",
            Description = "description",
            IsFavorite = false,
        };

        ProjectGroup entity = new ProjectGroup
        {
            Name = originalName,
            Description = originalDescription,
            IsFavorite = originalIsFavorite,
            ParentId = parent.Id
        };

        _groupRepository.GetById(id).Returns(entity);
        _groupRepository.GetParentByParentId(entity.ParentId).Returns(parent);

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
    public async Task UpdateProjectGroupWithNullParentPositiveTest(
        string newName, string newDescription, bool newIsFavorite,
        string originalName, string originalDescription, bool originalIsFavorite)
    {
        Guid id = Guid.NewGuid();

        ProjectGroup entity = new ProjectGroup
        {
            Name = originalName,
            Description = originalDescription,
            IsFavorite = originalIsFavorite,
            ParentId = default
        };

        _groupRepository.GetById(id).Returns(entity);
        _groupRepository.GetParentByParentId(default).Returns((ProjectGroup)null);

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
    public async Task UpdateProjectGroupCheckWithSameNameButSelfParentPositiveTest(
        string newDescription, bool newIsFavorite,
        string originalName, string originalDescription, bool originalIsFavorite)
    {
        Guid id = Guid.NewGuid();

        ProjectGroup parent = new ProjectGroup
        {
            Id = Guid.NewGuid(),
            Name = "Parent",
            Description = "description",
            IsFavorite = false,
        };

        ProjectGroup entity = new ProjectGroup
        {
            Name = originalName,
            Description = originalDescription,
            IsFavorite = originalIsFavorite,
            ParentId = parent.Id
        };

        _groupRepository.GetById(id).Returns(entity);
        _groupRepository.GetParentByParentId(entity.ParentId).Returns(parent);

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
    public void UpdateProjectGroupCheckNullParamNegativeTest()
    {
        Assert.ThrowsAsync<NullParameterException>(async () => await _service.Update(Guid.NewGuid(), null));
    }

    [Test]
    public void UpdateProjectGroupCheckNullParamNameNegativeTest()
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
    public void UpdateProjectGroupWithMissingEntityNegativeTest()
    {
        ProjectGroup entity = new ProjectGroup
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
    public void UpdateProjectGroupWithMissingParentNegativeTest()
    {
        _groupRepository.GetById(Arg.Any<Guid>()).Returns((ProjectGroup)null);

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
    public void UpdateProjectGroupCheckWithSameNameNegativeTest()
    {
        const string firstName = "First Name";
        const string secondName = "Second Name";

        ProjectGroup parent = new ProjectGroup
        {
            Id = Guid.NewGuid(),
            Name = "Group"
        };

        ProjectGroup child1 = new ProjectGroup { Id = Guid.NewGuid(), Name = firstName, ParentId = parent.Id };
        ProjectGroup child2 = new ProjectGroup { Id = Guid.NewGuid(), Name = secondName, ParentId = parent.Id };
        parent.Children.Add(child1);
        parent.Children.Add(child2);

        _groupRepository.GetById(child1.Id).Returns(child1);
        _groupRepository.GetById(child2.Id).Returns(child2);
        _groupRepository.GetParentByParentId(parent.Id).Returns(parent);

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