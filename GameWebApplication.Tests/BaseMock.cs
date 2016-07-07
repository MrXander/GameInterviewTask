using System;
using GameWebApplication.Hubs;
using GameWebApplication.Models;
using Moq;

namespace GameWebApplication.Tests
{
    public class BaseMock
    {
        protected static Mock<IClientGameHub> GetHubMock()
        {
            var hubMock = new Mock<IClientGameHub>();
            hubMock.Setup(x => x.ClickClient(0));
            hubMock.Setup(x => x.SetOccupiedCellClient(0));
            hubMock.Setup(x => x.GameOverClient(new Scores()));
            return hubMock;
        }

        protected static Action GetFirstEvent(GameTestable game)
        {
            Action a;
            game.Events.TryDequeue(out a);
            return a;
        }

    }
}
