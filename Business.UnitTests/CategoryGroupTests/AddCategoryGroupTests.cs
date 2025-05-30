﻿using GLSoft.DoubleEntryHomeAccounting.Business.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using NSubstitute;

namespace Business.UnitTests.CategoryGroupTests;

[TestFixture]
public class AddCategoryGroupTests
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

        _groupRepository.GetById(parent.Id).Returns(parent);
        _groupRepository.GetParentWithChildrenByParentId(parent.Id).Returns(parent);
        _groupRepository.GetMaxOrderInParent(parent.Id).Returns(maxOrder);
        await _groupRepository.Add(Arg.Do<CategoryGroup>(p => entity = p));

        GroupParam param = new GroupParam
        {
            Name = name,
            Description = description,
            IsFavorite = isFavorite,
            ParentId = parent.Id,
        };
        Guid id = await _service.Add(param);

        Assert.That(entity, Is.Not.Null);
        Assert.That(entity.Id, Is.EqualTo(id));
        Assert.That(ReferenceEquals(parent, entity.Parent), Is.True);
        Assert.That(parent.Children.Contains(entity), Is.True);

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

        _groupRepository.GetParentWithChildrenByParentId(default).Returns((CategoryGroup)null);
        _groupRepository.GetMaxOrderInParent(default).Returns(maxOrder);
        await _groupRepository.Add(Arg.Do<CategoryGroup>(p => entity = p));

        GroupParam param = new GroupParam
        {
            Name = name,
            Description = description,
            IsFavorite = isFavorite,
            ParentId = default,
        };
        await _service.Add(param);

        Assert.That(entity, Is.Not.Null);
        Assert.That(entity.Parent, Is.EqualTo(null));

        Assert.That(name, Is.EqualTo(param.Name));
        Assert.That(entity.Description, Is.EqualTo(description));
        Assert.That(entity.IsFavorite, Is.EqualTo(isFavorite));
        Assert.That(entity.Order, Is.EqualTo(maxOrder + 1));
    }

    [Test]
    public void AddCategoryGroupCheckNullParamNegativeTest()
    {
        Assert.ThrowsAsync<NullParameterException>(async () => await _service.Add(null));
    }

    [Test]
    public void AddCategoryGroupCheckNullParamNameNegativeTest()
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
    public void AddCategoryGroupWithMissingParentNegativeTest()
    {
        _groupRepository.GetById(Arg.Any<Guid>()).Returns((CategoryGroup)null);

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
    public void AddCategoryGroupCheckWithSameNameNegativeTest()
    {
        const string secondName = "Second Name";
        const string firstName = "First Name";

        CategoryGroup parent = new CategoryGroup
        {
            Id = Guid.NewGuid(),
            Name = "Group"
        };

        parent.Children.Add(new CategoryGroup() { Id = Guid.NewGuid(), Name = firstName });
        parent.Children.Add(new CategoryGroup() { Id = Guid.NewGuid(), Name = secondName });

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