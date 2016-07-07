import './style.css';
import React, { Component } from 'react'
import ReactDOM from 'react-dom'
import injectTapEventPlugin from 'react-tap-event-plugin';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import Menu from './components/menu'
import CreateGame from './components/createGame'
import Game from './components/game'

injectTapEventPlugin();

class App extends Component {
    constructor(props) {
        super(props);
        this.state = {showGame: false};
    };

    gameStarted(model) {
        this.setState({
            showGame: true,
            model: model
        });
    };

    render() {
        return (
                <div>
                    <MuiThemeProvider>
                        <Menu />
                    </MuiThemeProvider>
                    <MuiThemeProvider>
                        <CreateGame gameStarted={this.gameStarted.bind(this)}  />
                    </MuiThemeProvider>
                    <MuiThemeProvider>
                        <Game model={this.state.model} showGame={this.state.showGame} />
                    </MuiThemeProvider>
                </div>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('game'));