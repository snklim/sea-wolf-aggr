import * as React from 'react';
import { connect } from 'react-redux';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { icon } from '@fortawesome/fontawesome-svg-core/import.macro'

export interface Game {
    gameId: string,
    firstPlayerId: string,
    secondPlayerId: string,
    playAs: string
}

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

export default class Games extends React.PureComponent<{}, { ownField: Cell[], enemyField: Cell[], games: Game[], player: string, gameId: string }> {
    public state = {
        ownField: [],
        enemyField: [],
        games: [],
        player: 'first',
        gameId: ''
    };

    public componentDidMount() {
        fetch(`game/getall`, { method: 'GET' })
            .then(response => response.json() as Promise<Game[]>).then(data => this.setState({
                games: data
            }))
    }

    public newGame() {
        fetch(`game`, { method: 'POST' })
            .then(response => response.json() as Promise<Cell[][]>)
            .then(data => {
                this.setState({
                    ownField: data[0],
                    enemyField: data[1]
                });
            });
    }

    public play(gameId: string, player: string) {
        this.setState({
            gameId: gameId,
            player: player
        });
        this.getGame(gameId, player)
    }

    public getGame(gameId: string, player: string) {
        fetch(`game/getgame`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ gameId: gameId, player: player })
        })
            .then(response => response.json() as Promise<Cell[][]>)
            .then(data => {
                this.setState({
                    ownField: data[0],
                    enemyField: data[1]
                });
            });
    }

    public move(cell: Cell) {
        fetch(`game`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ gameId: this.state.gameId, player: this.state.player, pos: cell.pos })
        })
            .then(response => response.json() as Promise<Cell[][]>)
            .then(data => {
                // this.setState({
                //     ownField: data[0],
                //     enemyField: data[1]
                // });

                this.getGame(this.state.gameId, this.state.player)
            });
    }

    public render() {
        return (
            <div>
                <div>{this.state.player}</div>
                <div>
                    <button onClick={this.newGame}>New Game</button>
                </div>
                <div>
                    <table>
                        {
                            this.state.games.map((game: Game) => 
                                <tr>
                                    <td><button onClick={() => this.play(game.gameId, 'first')}>First player</button></td>
                                    <td><button onClick={() => this.play(game.gameId, 'second')}>Second player</button></td>
                                </tr>)
                        }
                    </table>
                </div>
                <div className='field'>
                    {
                        this.state.ownField.map((cell: Cell) =>
                            <div className={'cell ' + (cell.cellType == 1 ? 'ship' : '')}>
                                {cell.isDestroyed ? <FontAwesomeIcon size='xl' fixedWidth icon={icon({ name: 'xmark' })} /> : <></>}
                            </div>)
                    }
                </div>
                <div className='field'>
                    {
                        this.state.enemyField.map((cell: Cell) =>
                            <div onClick={() => this.move(cell)}
                                className={'cell '
                                    + (cell.cellType == 1 ? 'ship' : '')}>
                                {cell.isDestroyed ? <FontAwesomeIcon size='xl' fixedWidth icon={icon({ name: 'xmark' })} /> : <></>}
                            </div>)
                    }
                </div>
            </div>
        );
    }
}