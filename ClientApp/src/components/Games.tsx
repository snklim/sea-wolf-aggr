import * as React from 'react';
import { connect } from 'react-redux';

export interface Cell {
    cellType: number;
}

export default class Games extends React.PureComponent<{}, { ownField: Cell[], enemyField: Cell[] }> {
    public state = {
        ownField: [],
        enemyField: []
    };

    public componentDidMount() {
        fetch(`game`, {method: 'POST'})
            .then(response => response.json() as Promise<Cell[][]>)
            .then(data => {
                this.setState({
                    ownField: data[0],
                    enemyField: data[1]
                });
            });
    }

    public newGame() {
        console.log('new game')
    }

    public render() {
        return (<div>
            <div className='field'>
            {
                this.state.ownField.map((cell: Cell) =>
                    <div className={'cell ' + (cell.cellType == 1 ? 'ship' : '')}></div>)
            }
            </div>
            <div className='field'>
            {
                this.state.enemyField.map((cell: Cell) =>
                    <div className={'cell ' + (cell.cellType == 1 ? 'ship' : '')}></div>)
            }
            </div>
            <button onClick={this.newGame}>New Game</button>
        </div>);
    }
}