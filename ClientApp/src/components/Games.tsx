import * as React from 'react';
import { connect } from 'react-redux';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { icon } from '@fortawesome/fontawesome-svg-core/import.macro'

export interface GameItem {
    gameId: string,
    firstPlayerId: string,
    secondPlayerId: string
}

export interface Cell {
    cellType: string;
    pos: Pos;
    isDestroyed: boolean;
}

export interface Pos {
    col: number;
    row: number;
}

export interface Game {
    currentPlayerId: string,
    ownField: Cell[],
    enemyField: Cell[]
}

export default class Games extends React.PureComponent<{}, { ownField: Cell[], enemyField: Cell[], games: GameItem[], playerId: string, gameId: string, currentPlayerId: string }> {
    public state = {
        ownField: [],
        enemyField: [],
        games: [],
        playerId: '',
        gameId: '',
        currentPlayerId: ''
    };

    public componentDidMount() {
        this.getAllGames()
    }

    public getAllGames() {
        fetch(`game/getall`, { method: 'GET' })
            .then(response => response.json() as Promise<GameItem[]>).then(data => this.setState({
                games: data
            }))
    }

    public newGame() {
        fetch(`game`, { method: 'POST' })
            .then(response => response.json() as Promise<Game>)
            .then(data => {
                // this.setState({
                //     ownField: data[0],
                //     enemyField: data[1]
                // });
                //console.log(this)
                
                this.getAllGames()
            });
    }

    public play(gameId: string, playerId: string) {
        this.setState({
            gameId: gameId,
            playerId: playerId
        });
        this.getGame(gameId, playerId)
    }

    public getGame(gameId: string, playerId: string) {
        fetch(`game/getgame`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ gameId: gameId, playerId: playerId })
        })
            .then(response => response.json() as Promise<Game>)
            .then(data => {
                this.setState({
                    currentPlayerId: data.currentPlayerId,
                    ownField: data.ownField,
                    enemyField: data.enemyField
                });
            });
    }

    public move(cell: Cell) {
        fetch(`game`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ gameId: this.state.gameId, playerId: this.state.playerId, pos: cell.pos })
        })
            .then(response => this.getGame(this.state.gameId, this.state.playerId))
            .then(data => {
                // this.setState({
                //     ownField: data[0],
                //     enemyField: data[1]
                // });

                this.getGame(this.state.gameId, this.state.playerId)
            });
    }

    public render() {
        return (
            <div>
                <div>{this.state.playerId==this.state.currentPlayerId ? 'Your turn' : 'Opponent turn'}</div>
                <div>
                    <button onClick={() => this.newGame()}>New Game</button>
                </div>
                <div>
                    <table>
                        {
                            this.state.games.map((game: GameItem) => 
                                <tr>
                                    <td><button className={game.firstPlayerId == this.state.playerId ? 'ship' : ''} 
                                        onClick={() => this.play(game.gameId, game.firstPlayerId)}>First player</button></td>
                                    <td><button className={game.secondPlayerId == this.state.playerId ? 'ship' : ''}
                                        onClick={() => this.play(game.gameId, game.secondPlayerId)}>Second player</button></td>
                                </tr>)
                        }
                    </table>
                </div>
                <div className='field'>
                    {
                        this.state.ownField.map((cell: Cell) =>
                            <div className={'cell ' + (cell.cellType == 'ship' ? 'ship' : '')}>
                                {cell.isDestroyed ? <FontAwesomeIcon size='xl' fixedWidth icon={icon({ name: 'xmark' })} /> : <></>}
                            </div>)
                    }
                </div>
                <div className='field'>
                    {
                        this.state.enemyField.map((cell: Cell) =>
                            <div onClick={() => this.move(cell)}
                                className={'cell '
                                    + (cell.cellType == 'ship' ? 'ship' : '')}>
                                {cell.isDestroyed ? <FontAwesomeIcon size='xl' fixedWidth icon={icon({ name: 'xmark' })} /> : <></>}
                            </div>)
                    }
                </div>
            </div>
        );
    }
}