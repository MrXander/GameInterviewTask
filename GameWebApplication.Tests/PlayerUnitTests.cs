using System;
using System.Linq;
using GameWebApplication.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameWebApplication.Tests
{
    [TestClass]
    public class PlayerUnitTests: BaseMock
    {
        [TestMethod]
        public void SetOccupiedTest()
        {
            const int cellId = 0;
            var hubMock = GetHubMock();

            var createGameModel = new CreateGameModel() { PlayerName = "TestPlayer", PlayersCount = 2 };
            var game = new GameTestable("myGame", createGameModel, hubMock.Object);

            var player = game.Player;
            player.SetOccupied(cellId);

            var a = GetFirstEvent(game);
            a(); //execute occupied event

            Assert.IsTrue(game.Cells[cellId].IsOccupied); 
        }

        private static Action GetFirstEvent(GameTestable game)
        {
            Action a;
            game.Events.TryDequeue(out a);
            return a;            
        }

        [TestMethod]
        public void ClickTest()
        {
            const int cellId = 0;
            var hubMock = GetHubMock();

            var createGameModel = new CreateGameModel() { PlayerName = "TestPlayer", PlayersCount = 2 };
            var game = new GameTestable("myGame", createGameModel, hubMock.Object);

            var player = game.Player;
            player.Click(cellId); // nothing should happen            
            Assert.IsFalse(game.Cells[cellId].IsOccupied); // check that cell is not occupied

            Assert.AreEqual(1, game.Events.Count); //click added to event queue

            var a = GetFirstEvent(game);
            a(); //execute click event

            Assert.IsFalse(game.Cells[cellId].IsOccupied); // check that cell is not occupied again
            var playerWithPoints = game.Scores.ScoreList.FirstOrDefault(x => x.Points > 0);
            Assert.IsNull(playerWithPoints);
        }

        [TestMethod]
        public void OccupyAndClickTest()
        {
            const int cellId = 0;
            var hubMock = GetHubMock();
            var createGameModel = new CreateGameModel() { PlayerName = "TestPlayer", PlayersCount = 2 };
            var game = new GameTestable("myGame", createGameModel, hubMock.Object);

            var player = game.Player;
            player.SetOccupied(cellId);
            
            Assert.AreEqual(1, game.Events.Count);

            var a = GetFirstEvent(game);
            a(); //execute set occupied event

            Assert.IsTrue(game.Cells[cellId].IsOccupied);

            player.Click(cellId);

            a = GetFirstEvent(game);
            a(); //execute click event

            Assert.IsFalse(game.Cells[cellId].IsOccupied);
                        
            var scores = game.Scores.ScoreList.FirstOrDefault(x => string.Equals(x.Name, createGameModel.PlayerName));

            Assert.IsNotNull(scores);
            Assert.AreEqual(1, scores.Points);
        }
    }
}
