import * as React from 'react';
import { connect } from 'react-redux';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { icon } from '@fortawesome/fontawesome-svg-core/import.macro'

export interface Cell {
    cellType: number;
    pos: Pos;
    isDestroyed: boolean;
    playerId: string;
}

export interface Pos {
    col: number;
    row: number;
}

export default class Games extends React.PureComponent<{}, { ownField: Cell[], enemyField: Cell[] }> {
    public state = {
        ownField: [],
        enemyField: []
    };

    public componentDidMount() {
        fetch(`game`, { method: 'POST' })
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

    public move(cell: Cell) {
        fetch(`game`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ playerId: cell.playerId, pos: cell.pos })
        })
            .then(response => response.json() as Promise<Cell[][]>)
            .then(data => {
                console.log(data)
                this.setState({
                    ownField: data[0],
                    enemyField: data[1]
                });
            });
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
                        <div onClick={() => this.move(cell)} 
                            className={'cell ' 
                                + (cell.cellType == 1 ? 'ship' : '')}>
                                    {cell.isDestroyed ? <FontAwesomeIcon size='xl' fixedWidth icon={icon({name: 'xmark'})} /> : <></>}
                                </div>)
                }
            </div>
            <button onClick={this.newGame}>New Game</button>
        </div>);
    }
}