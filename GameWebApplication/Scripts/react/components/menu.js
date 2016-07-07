import React from 'react'
import AppBar from 'material-ui/AppBar';
import Drawer from 'material-ui/Drawer';
import MenuItem from 'material-ui/MenuItem';
import RaisedButton from 'material-ui/RaisedButton';


const drawerContainerStyle = {
    position: 'relative'
};

class Menu extends React.Component {
    constructor(props) {
        super(props);
        this.state = {open: false};
    }

    handleToggle() {
        this.setState({open: !this.state.open});
    }

    render() {
        return (
            <div>
                <AppBar
                    title="Menu"
                    iconClassNameRight="muidocs-icon-navigation-expand-more"
                    onTouchTap={ this.handleToggle.bind(this) }
                />
                <Drawer open={this.state.open} containerStyle={drawerContainerStyle}>
                    <MenuItem linkButton={true} href="/home">Game</MenuItem>
                    <MenuItem linkButton={true} href="/home/scores">Scores</MenuItem>
                </Drawer>
            </div>
        );
    }
}

export default Menu;