import { ICard } from './ICard';
import { IPlayer } from './IPlayer';

export interface IMatch {
  id: string;
  player: IPlayer;
  discardPiles: [ICard[], ICard[]];
  status: number;
  computerHandSize: number;
  playerScore: number;
  computerScore: number;
  round: number;
}
