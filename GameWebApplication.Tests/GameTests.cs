using System;
using GameWebApplication.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameWebApplication.Tests
{
    [TestClass]
    public class GameTests : BaseMock
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OnePlayerGameTest()
        {
            var hubMock = GetHubMock();

            var createGameModel = new CreateGameModel() { PlayerName = "TestPlayer", PlayersCount = 1 };
            var game = new GameTestable("myGame", createGameModel, hubMock.Object);
        }

        [TestMethod]        
        public void MoreThanOnePlayerGameTest()
        {
            var hubMock = GetHubMock();

            var createGameModel = new CreateGameModel() { PlayerName = "TestPlayer", PlayersCount = 2 };
            var game = new GameTestable("myGame", createGameModel, hubMock.Object);
        }
    }
}
