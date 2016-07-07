import React from 'react'
import RaisedButton from 'material-ui/RaisedButton';
import TextField from 'material-ui/TextField';
import Dialog from 'material-ui/Dialog';
import FlatButton from 'material-ui/FlatButton';

export default class CreateGame extends React.Component {
    constructor(props) {
        super(props);
        this.state = {open: false};
    };

    handleOpen() {
        this.setState({open: true});
    };

    handleClose() {
        this.setState({open: false});
    };

    handleClick() {
        var players = this.state.playersCount;
        var name = this.state.name;
        if (players && players > 1 && name && name.length > 0) {
            this.setState({class: 'hidden'});
            this.props.gameStarted({ name: name, playersCount: players });
            return;

        }
        this.handleOpen();
    };

    playersChanged(e) {
        this.setState({ playersCount: e.target.value});
    };

    nameChanged(e) {
        this.setState({ name: e.target.value});
    };

    render() {
        const actions = [
            <FlatButton
                label="OK"
                primary={true}
                onTouchTap={this.handleClose.bind(this)}
            />
        ];
        return (
            <div className={this.state.class}>
                <TextField hintText="Your name" onChange={this.nameChanged.bind(this)} />
                <TextField hintText="How many players?" onChange={this.playersChanged.bind(this)} />
                <RaisedButton label="Start game" primary={true} onTouchTap={this.handleClick.bind(this)}  />
                <Dialog
                    actions={actions}
                    modal={false}
                    open={this.state.open}
                    onRequestClose={this.handleClose.bind(this)}
                >
                    Name could not be empty and players should be more than 1.
                </Dialog>
            </div>
        )
    };
}