//using Business.Services;
//using Common.DataAccess;
//using Common.Models;
//using GLSoft.DoubleEntryHomeAccounting.Business.Services;
//using GLSoft.DoubleEntryHomeAccounting.Common.Models;
//using Moq;
//using NUnit.Framework;

//namespace Business.UnitTests.CategoryTests;

//[TestFixture]
//public class DeleteCategoryTests
//{
//    [Test]
//    public void DeleteCategoryPositiveTest()
//    {
//        var isDeleted = false;

//        var parent = new CategoryGroup { Id = Guid.NewGuid(), Name = "Parent" };
//        var deletedEntity = new Category
//            { Id = Guid.NewGuid(), Name = "Name", Description = "", IsFavorite = true, Order = 2, Parent = parent };
//        parent.Children.Add(deletedEntity);
//        parent.Children.Add(new Category
//        {
//            Id = Guid.NewGuid(), Name = "Kate", Description = "Vacation", IsFavorite = false, Order = 3, Parent = parent
//        });
//        parent.Children.Add(new Category
//        {
//            Id = Guid.NewGuid(), Name = "Mom", Description = "Health", IsFavorite = true, Order = 1, Parent = parent
//        });

//        var mockEntityDataAccess = new Mock<ICategoryDataAccess>();
//        mockEntityDataAccess.Setup(eda => eda.Get(It.IsAny<Guid>())).Returns(() => deletedEntity);
//        mockEntityDataAccess.Setup(eda => eda.LoadParent(It.IsAny<Category>()));
//        mockEntityDataAccess.Setup(eda => eda.Delete(It.IsAny<Category>())).Callback(() => isDeleted = true);

//        mockEntityDataAccess.Setup(eda => eda.UpdateList(It.IsAny<List<Category>>()))
//            .Callback(() => parent.Children.Remove(deletedEntity));

//        var mockParentEntityDataAccess = new Mock<ICategoryGroupDataAccess>();
//        mockParentEntityDataAccess.Setup(pda => pda.LoadChildren(It.IsAny<CategoryGroup>()));

//        var mockAccountDataAccess = new Mock<IAccountDataAccess>();
//        mockAccountDataAccess.Setup(ada => ada.GetAccountsByCategory(It.IsAny<Category>()))
//            .Returns(() => new List<Account>
//            {
//                new() { Id = Guid.NewGuid(), Name = "NO" },
//                new() { Id = Guid.NewGuid(), Name = "Yes" },
//                new() { Id = Guid.NewGuid(), Name = "For" }
//            });

//        mockAccountDataAccess.Setup(ada => ada.Update(It.IsAny<Account>()));

//        var service = new CategoryService(CreateMockGlobalDataAccess(),
//            mockEntityDataAccess.Object,
//            mockParentEntityDataAccess.Object,
//            mockAccountDataAccess.Object);
//        service.Delete(deletedEntity.Id);
//        Assert.IsTrue(isDeleted);
//        Assert.AreEqual(2, parent.Children.Count());
//        Assert.AreEqual(2, parent.Children[0].Order);
//    }

//    [Test]
//    public void DeleteCategoryCheckAndGetEntityByIdTest()
//    {
//        var mockEntityDataAccess = new Mock<ICategoryDataAccess>();
//        mockEntityDataAccess.Setup(eda => eda.Get(It.IsAny<Guid>())).Returns(() => null);
//        var service = new CategoryService(CreateMockGlobalDataAccess(),
//            mockEntityDataAccess.Object,
//            CreateMockParentEntityDataAccess(),
//            CreateMockAccountDataAccess());

//        Assert.ThrowsAsync<ArgumentNullException>(async () => await service.Delete(Guid.NewGuid()));
//    }

//    private IGlobalDataAccess CreateMockGlobalDataAccess()
//    {
//        var mockGlobalDataAccess = new Mock<IGlobalDataAccess>();
//        mockGlobalDataAccess.Setup(gda => gda.Load());
//        mockGlobalDataAccess.Setup(gda => gda.Save());
//        mockGlobalDataAccess.Setup(gda => gda.Get(It.IsAny<Guid>())).Returns(() => null);
//        return mockGlobalDataAccess.Object;
//    }

//    private ICategoryGroupDataAccess CreateMockParentEntityDataAccess()
//    {
//        var mockParentEntityDataAccess = new Mock<ICategoryGroupDataAccess>();
//        return mockParentEntityDataAccess.Object;
//    }

//    private IAccountDataAccess CreateMockAccountDataAccess()
//    {
//        var mockAccountDataAccess = new Mock<IAccountDataAccess>();
//        return mockAccountDataAccess.Object;
//    }
//}