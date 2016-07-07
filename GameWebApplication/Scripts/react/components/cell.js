import React from 'react'
import Paper from 'material-ui/Paper';
import Popover from 'material-ui/Popover';
import Menu from 'material-ui/Menu';
import MenuItem from 'material-ui/MenuItem';

const baseCellStyle = {
    height: 100,
    width: 100,
    margin: 10,
    textAlign: 'center',
    display: 'inline-block',
    float: 'left'
};

const occupiedByBotStyle = Object.assign({}, baseCellStyle); //copy object es6
occupiedByBotStyle.backgroundColor = 'red';

const occupiedByPlayerStyle = Object.assign({}, baseCellStyle);
occupiedByPlayerStyle.backgroundColor = 'blue';

class Cell extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            open: false,
            isOccupying: false,
            id: props.model.id,
            occupyState: {
                isOccupiedByPlayer: props.model.isOccupiedByPlayer,
                isOccupiedByBot: props.model.isOccupiedByBot,
                resetOccupied: props.model.resetOccupied
            }
        };
    };

    cellHandler(event) {
        // This prevents ghost click.
        event.preventDefault();

        if (this.state.isOccupying)
        {
            console.log('Player. Cell ' + this.state.id + ' Clicking');
            //click on the server
            $.connection.serverGameHub.server.click(this.state.id);

            console.log('Player. Cell ' + this.state.id + ' Clicked');

            this.setState({ isOccupying: false });
            return;
        }

        console.log('Player. Cell ' + this.state.id + ' open Popover');
        this.setState({
            open: true,
            anchorEl: event.currentTarget
        });
    };

    componentWillReceiveProps(newProps) {
        this.setState({id: newProps.model.id,
                        occupyState: {
                            isOccupiedByPlayer: newProps.model.isOccupiedByPlayer,
                            isOccupiedByBot: newProps.model.isOccupiedByBot,
                            resetOccupied: newProps.model.resetOccupied
                        }});
    };

    getStyle() {
        if (this.state.occupyState.resetOccupied)
            return baseCellStyle;
        if (this.state.occupyState.isOccupiedByPlayer)
            return occupiedByPlayerStyle;
        if (this.state.occupyState.isOccupiedByBot)
            return occupiedByBotStyle;
        return baseCellStyle;
    }

    handlerRequestClose() {
        this.setState({
            open: false
        });
    };

    occupyHandler() {
        console.log('Player. Cell ' + this.state.id + ' occypying');

        $.connection.serverGameHub.server.setOccupied(this.state.id);
        console.log('Player. Cell ' + this.state.id + ' occypied');
        this.setState({
            open: false,
            isOccupying: true,
            occupyState: { isOccupiedByPlayer: true }});
    };

    render() {
        return (
            <div>
                <Paper zDepth={2} style={this.getStyle()} onTouchTap={this.cellHandler.bind(this)} className="hidden" />
                <Popover
                    open={this.state.open}
                    anchorEl={this.state.anchorEl}
                    anchorOrigin={{horizontal: 'left', vertical: 'bottom'}}
                    targetOrigin={{horizontal: 'left', vertical: 'top'}}
                    onRequestClose={this.handlerRequestClose.bind(this)}
                >
                    <Menu>
                        <MenuItem primaryText="Occupy cell" onTouchTap={this.occupyHandler.bind(this)} />
                    </Menu>
                </Popover>
            </div>
        );
    }
}

export default Cell;