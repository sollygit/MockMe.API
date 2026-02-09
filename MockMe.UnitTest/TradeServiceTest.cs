using MockMe.API.Services;
using MockMe.API.ViewModels;
using MockMe.Model;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockMe.UnitTest
{
    [TestFixture]
    public class TradeServiceTest
    {
        private IQueryable<AssetTrade> trades;

        [SetUp]
        public void Setup()
        {
            trades = new List<AssetTrade>()
            {
                new AssetTrade { Id = Guid.NewGuid().ToString(), Asset = new Asset(1, "EUR/USD"), Amount = 100, Direction = 1 },
                new AssetTrade { Id = Guid.NewGuid().ToString(), Asset = new Asset(2, "JPY/USD"), Amount = 200, Direction = 2 },
                new AssetTrade { Id = Guid.NewGuid().ToString(), Asset = new Asset(3, "GBP/USD"), Amount = 300, Direction = 1 },
            }.AsQueryable();
        }

        [Test]
        public async Task GetAllAsync_Success()
        {
            // Arrange
            var mockService = new Mock<ITradeService>();
            mockService.Setup(x => x.GetAllAsync()).Returns(async () =>
            {
                await Task.Yield();
                return trades;
            });

            // Act
            var actual = await mockService.Object.GetAllAsync();

            // Assert
            Equals(trades.Count(), actual.Count());
        }

        [Test]
        public async Task GetAsync_Success()
        {
            // Arrange
            var trade = trades.FirstOrDefault();

            var mockService = new Mock<ITradeService>();
            mockService.Setup(x => x.GetAsync(Guid.Parse(trade.Id))).Returns(async () =>
            {
                await Task.Yield();
                return trade;
            });

            // Act
                
            var entity = new AssetTradeViewModel { Id = trade.Asset.Id, Name = trade.Asset.Name, Amount = trade.Amount, Direction = trade.Direction };
            await mockService.Object.SaveAsync(entity);
            var actual = await mockService.Object.GetAsync(Guid.Parse(trade.Id));

            // Assert
            Equals(trade, actual);
        }

        [Test]
        public async Task GetAsync_NotFound_Success()
        {
            // Arrange
            var tradeId = Guid.NewGuid();
            var mockService = new Mock<ITradeService>();

            mockService.Setup(x => x.GetAsync(tradeId)).Returns(async () =>
            {
                await Task.Yield();
                return null;
            });

            // Act
            var actual = await mockService.Object.GetAsync(tradeId);

            // Assert
            mockService.Verify(m => m.GetAsync(tradeId), Times.AtLeastOnce());
            Equals(null, actual);
        }

        [Test]
        public async Task SaveAsync_Success()
        {
            // Arrange
            var trade = trades.FirstOrDefault();

            var mockService = new Mock<ITradeService>();
            mockService.Setup(x => x.SaveAsync(It.IsAny<AssetTradeViewModel>())).Returns(async () =>
            {
                await Task.Yield();
                return trade;
            });

            // Act
            var entity = new AssetTradeViewModel { Id = trade.Asset.Id, Name = trade.Asset.Name, Amount = trade.Amount, Direction = trade.Direction };
            var actual = await mockService.Object.SaveAsync(entity);

            // Assert
            Equals(trade, actual);
        }

        [Test]
        public void SaveAsync_IsNull_Failure_Throws()
        {
            string errorMessage = "Entity cannot be null";

            // Arrange
            var trade = It.IsAny<AssetTrade>();

            // Act and Assert
            Assert.That(async () =>
                await SaveAsync_ThrowException(trade, errorMessage),
                Throws.Exception.TypeOf<ServiceException>().And.Message.EqualTo(errorMessage));
        }

        [Test]
        public async Task UpdateAsync_Success()
        {
            // Arrange
            var trade = trades.FirstOrDefault();

            var mockService = new Mock<ITradeService>();
            mockService.Setup(x => x.UpdateAsync(Guid.Parse(trade.Id), It.IsAny<AssetTradeViewModel>())).Returns(async () =>
            {
                await Task.Yield();
                return trade;
            });

            // Act
            var entity = new AssetTradeViewModel { Id = trade.Asset.Id, Name = trade.Asset.Name, Amount = trade.Amount, Direction = trade.Direction };
            await mockService.Object.UpdateAsync(Guid.Parse(trade.Id), entity);

            // Assert
            mockService.Verify(m => m.UpdateAsync(Guid.Parse(trade.Id), It.IsAny<AssetTradeViewModel>()), Times.AtLeastOnce());
            Assert.That(trade.Asset.Name, Is.EqualTo("EUR/USD"));
            Assert.That(trade.Amount, Is.EqualTo(100));
            Assert.That(trade.Direction, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteAsync_Success()
        {
            // Arrange
            var trade = trades.FirstOrDefault();

            var mockService = new Mock<ITradeService>();
            mockService.Setup(x => x.DeleteAsync(Guid.Parse(trade.Id))).Returns(async () =>
            {
                await Task.Yield();
                return trade;
            });

            // Act
            await mockService.Object.DeleteAsync(Guid.Parse(trade.Id));
            var actual = await mockService.Object.GetAsync(Guid.Parse(trade.Id));

            // Assert
            mockService.Verify(m => m.DeleteAsync(Guid.Parse(trade.Id)));
            mockService.Verify(m => m.GetAsync(Guid.Parse(trade.Id)));
            Equals(null, actual);
        }

        private static async Task SaveAsync_ThrowException(AssetTrade trade, string errorMessage)
        {
            var mockService = new Mock<ITradeService>();
            var entity = trade == null ? It.IsAny<AssetTradeViewModel>() : new AssetTradeViewModel();
            await mockService.Object.SaveAsync(entity).ConfigureAwait(false);
            throw new ServiceException(errorMessage);
        }
    }
}
