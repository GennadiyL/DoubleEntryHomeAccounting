using GLSoft.DoubleEntryHomeAccounting.Business.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using NSubstitute;

namespace Business.UnitTests.CorrespondentGroupTests;

[TestFixture]
public class AddCorrespondentGroupTests
{
    private IUnitOfWorkFactory _unitOfWorkFactory;
    private IUnitOfWork _unitOfWork;
    private ICorrespondentGroupRepository _groupRepository;
    private CorrespondentGroupService _service;

    [SetUp]
    public void SetUp()
    {
        _groupRepository = Substitute.For<ICorrespondentGroupRepository>();

        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.GetRepository<IGroupEntityRepository<CorrespondentGroup, Correspondent>>().Returns(_groupRepository);

        _unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        _unitOfWorkFactory.Create().Returns(_unitOfWork);

        _service = new CorrespondentGroupService(_unitOfWorkFactory);
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork?.Dispose();
    }

    [TestCase("Name", "Description", true, 6)]
    [TestCase("Andy", "", false, 12001)]
    [TestCase("StringName", "Words about CorrespondentGroup", true, 0)]
    public async Task AddCorrespondentGroupPositiveTest(string name, string description, bool isFavorite, int maxOrder)
    {
        CorrespondentGroup parent = new CorrespondentGroup
        {
            Id = Guid.NewGuid(),
            Name = "Group"
        };
        CorrespondentGroup entity = null;

        _groupRepository.GetById(parent.Id).Returns(parent);
        _groupRepository.GetByParentId(parent.Id).Returns(parent);
        _groupRepository.GetMaxOrder(parent.Id).Returns(maxOrder);
        await _groupRepository.Add(Arg.Do<CorrespondentGroup>(p => entity = p));

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
    [TestCase("StringName", "Words about CorrespondentGroup", true, 0)]
    public async Task AddCorrespondentGroupWithNullParentPositiveTest(string name, string description, bool isFavorite, int maxOrder)
    {
        CorrespondentGroup entity = null;

        _groupRepository.GetByParentId(default).Returns((CorrespondentGroup)null);
        _groupRepository.GetMaxOrder(default).Returns(maxOrder);
        await _groupRepository.Add(Arg.Do<CorrespondentGroup>(p => entity = p));

        GroupParam param = new GroupParam
        {
            Name = name,
            Description = description,
            IsFavorite = isFavorite,
            ParentId = null,
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
    public void AddCorrespondentGroupCheckNullParamNegativeTest()
    {
        Assert.ThrowsAsync<MissingInputParameterException>(async () => await _service.Add(null));
    }

    [Test]
    public void AddCorrespondentGroupCheckNullParamNameNegativeTest()
    {
        GroupParam param = new GroupParam
        {
            Name = null,
            Description = "description",
            IsFavorite = true
        };
        Assert.ThrowsAsync<MissingNameException>(async () => await _service.Add(param));
    }

    [Test]
    public void AddCorrespondentGroupWithMissingParentNegativeTest()
    {
        _groupRepository.GetById(Arg.Any<Guid>()).Returns((CorrespondentGroup)null);

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
    public void AddCorrespondentGroupCheckEntityWithSameNameNegativeTest()
    {
        const string secondName = "Second Name";
        const string firstName = "First Name";

        CorrespondentGroup parent = new CorrespondentGroup
        {
            Id = Guid.NewGuid(),
            Name = "Group"
        };
        
        parent.Children.Add(new CorrespondentGroup() {Id = Guid.NewGuid() , Name = firstName});
        parent.Children.Add(new CorrespondentGroup() { Id = Guid.NewGuid(), Name = secondName });

        _groupRepository.GetById(parent.Id).Returns(parent);
        _groupRepository.GetByParentId(parent.Id).Returns(parent);

        var param = new GroupParam()
        {
            Name = secondName,
            Description = "description",
            IsFavorite = true,
            ParentId = parent.Id
        };
        Assert.ThrowsAsync<DuplicationNameException>(async () => await _service.Add(param));
    }
}