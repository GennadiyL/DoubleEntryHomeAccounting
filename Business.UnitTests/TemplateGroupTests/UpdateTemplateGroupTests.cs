using GLSoft.DoubleEntryHomeAccounting.Business.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using NSubstitute;

namespace Business.UnitTests.TemplateGroupTests;

[TestFixture]
public class UpdateTemplateGroupTests
{
    private IUnitOfWorkFactory _unitOfWorkFactory;
    private IUnitOfWork _unitOfWork;
    private ITemplateGroupRepository _groupRepository;
    private TemplateGroupService _service;

    [SetUp]
    public void SetUp()
    {
        _groupRepository = Substitute.For<ITemplateGroupRepository>();

        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.GetRepository<IGroupEntityRepository<TemplateGroup, Template>>().Returns(_groupRepository);

        _unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        _unitOfWorkFactory.Create().Returns(_unitOfWork);

        _service = new TemplateGroupService(_unitOfWorkFactory);
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork?.Dispose();
    }

    [TestCase("Name", "Description", true, "Mom", "All", false)]
    [TestCase("Dad", "For all family", false, "Child", "school", true)]
    public async Task UpdateTemplateGroupPositiveTest(
        string newName, string newDescription, bool newIsFavorite,
        string originalName, string originalDescription, bool originalIsFavorite)
    {
        Guid id = Guid.NewGuid();

        TemplateGroup parent = new TemplateGroup
        {
            Id = Guid.NewGuid(),
            Name = "Parent",
            Description = "description",
            IsFavorite = false,
        };

        TemplateGroup entity = new TemplateGroup
        {
            Name = originalName,
            Description = originalDescription,
            IsFavorite = originalIsFavorite,
            ParentId = parent.Id
        };

        _groupRepository.GetById(id).Returns(entity);
        _groupRepository.GetByParentId(entity.ParentId).Returns(parent);

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
    public async Task UpdateTemplateGroupWithNullParentPositiveTest(
        string newName, string newDescription, bool newIsFavorite,
        string originalName, string originalDescription, bool originalIsFavorite)
    {
        Guid id = Guid.NewGuid();

        TemplateGroup entity = new TemplateGroup
        {
            Name = originalName,
            Description = originalDescription,
            IsFavorite = originalIsFavorite,
            ParentId = default
        };

        _groupRepository.GetById(id).Returns(entity);
        _groupRepository.GetByParentId(default).Returns((TemplateGroup)null);

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
    public async Task UpdateTemplateGroupCheckWithSameNameButSelfParentPositiveTest(
        string newDescription, bool newIsFavorite,
        string originalName, string originalDescription, bool originalIsFavorite)
    {
        Guid id = Guid.NewGuid();

        TemplateGroup parent = new TemplateGroup
        {
            Id = Guid.NewGuid(),
            Name = "Parent",
            Description = "description",
            IsFavorite = false,
        };

        TemplateGroup entity = new TemplateGroup
        {
            Name = originalName,
            Description = originalDescription,
            IsFavorite = originalIsFavorite,
            ParentId = parent.Id
        };

        _groupRepository.GetById(id).Returns(entity);
        _groupRepository.GetByParentId(entity.ParentId).Returns(parent);

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
    public void UpdateTemplateGroupCheckNullParamNegativeTest()
    {
        Assert.ThrowsAsync<NullParameterException>(async () => await _service.Update(Guid.NewGuid(), null));
    }

    [Test]
    public void UpdateTemplateGroupCheckNullParamNameNegativeTest()
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
    public void UpdateTemplateGroupWithMissingEntityNegativeTest()
    {
        TemplateGroup entity = new TemplateGroup
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
    public void UpdateTemplateGroupWithMissingParentNegativeTest()
    {
        _groupRepository.GetById(Arg.Any<Guid>()).Returns((TemplateGroup)null);

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
    public void UpdateTemplateGroupCheckWithSameNameNegativeTest()
    {
        const string firstName = "First Name";
        const string secondName = "Second Name";

        TemplateGroup parent = new TemplateGroup
        {
            Id = Guid.NewGuid(),
            Name = "Group"
        };

        TemplateGroup child1 = new TemplateGroup { Id = Guid.NewGuid(), Name = firstName, ParentId = parent.Id };
        TemplateGroup child2 = new TemplateGroup { Id = Guid.NewGuid(), Name = secondName, ParentId = parent.Id };
        parent.Children.Add(child1);
        parent.Children.Add(child2);

        _groupRepository.GetById(child1.Id).Returns(child1);
        _groupRepository.GetById(child2.Id).Returns(child2);
        _groupRepository.GetByParentId(parent.Id).Returns(parent);

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