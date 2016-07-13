import React from 'react'
import Cell from './Cell'
//import jquery from 'jquery'
//import Hubs from 'signalrjs/hub'
//import $ from 'signalrjs';

class Game extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            createGameModel: { playersCount: 0 },
            game: {
                CellsCount: 0,
                Players: [],
                Scores: { ScoreList: [] }
            },
            cell: {
                id: -1,
                isOccupiedByPlayer: false,
                isOccupiedByBot: false,
                resetOccupied: false
            }};
    };

    componentDidMount() {
        var client = $.connection.serverGameHub.client;
        var self = this;
        client.startGame = function(game) {
            console.log('Game started');
            console.log(game);
            self.setState({ game: game });
        };
        client.resetOccupied = function(cellId) {
            console.log('Server. Reset occupied cell ' + cellId);
            self.setState({ cell: { id: cellId, resetOccupied: true } });
        };
        client.setOccupied = function(cellId) {
            console.log('Server. Set occupied cell ' + cellId);
            self.setState({ cell: { id: cellId, isOccupiedByBot: true } });
        };
        client.updateScore = function(game) {
            console.log('Server. BotGotPoint. Updating score.');
            console.log(game);
            self.setState({ game: game });
        };
        client.gameOver = function(game) {
            console.log('Server. Game over');
            console.log(game);
            self.setState({ game: game });
            alert('Game over');
        };
    };

    componentWillReceiveProps(newProps) {
        this.setState(newProps);
        //initialize connection
        $.connection.hub.start().done(function () {
            //create game
            var name = newProps.model.name;
            var players = newProps.model.playersCount;
            $.connection.serverGameHub.server.createGame({ playerName: name, playersCount: players }).done(function() { console.log('Game created'); });
        });
    };

    gameClass() {
        return "game-field " + (this.props.showGame ? "block" : "hidden");
    };

    render() {
        var cells = [];
        for(var i = 0; i < this.state.game.CellsCount; i++) {
            var model = { id: i };
            if (this.state.cell.id === i){
                model = this.state.cell;
            }
            cells.push(<Cell model={model} key={'cell_' + i} />);
        }
        return (
            <div className={this.gameClass()}>
                <div className="statistic">
                    <span>Statistic:</span>
                    {this.state.game.Scores.ScoreList.map(score => (
                        <div key={score.Name}>{score.Name}: {score.Points}</div>
                    )) }
                </div>
                <div>
                    {cells}
                </div>
            </div>
        )
    };
}

export default Game;