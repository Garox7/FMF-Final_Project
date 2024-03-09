import { Injectable } from '@angular/core';
import { IMatch } from '../interfaces/IMatch';
import { ApiService } from './apiService.service';
import { Observable, ReplaySubject, map } from 'rxjs';

import { ICard } from '../interfaces/ICard';
import { LoginService } from './loginService.service';

@Injectable({
  providedIn: 'root',
})
export class MatchService {
  matchChange = new ReplaySubject<IMatch>(1);
  match!: IMatch;
  playerHand!: ICard[];

  errorMessageChange = new ReplaySubject<string>(1);

  playerEmail: string = this.loginService.player.Username;

  discardChange = new ReplaySubject<ICard[]>(1);
  discardPileIdChange = new ReplaySubject<number | null>(1);
  discardCards: ICard[] = [];
  discardPileId!: number;

  constructor(
    private api: ApiService<IMatch>,
    private loginService: LoginService
  ) {}

  newMatch(): Observable<IMatch> {
    return this.api
      .get<IMatch>(
        `/game/match/new-match/${this.loginService.player.Username}`,
        {
          withCredentials: true,
        }
      )
      .pipe(
        map((matchResponse: IMatch) => {
          this.match = matchResponse;
          this.matchChange.next(this.match);

          return matchResponse;
        })
      );
  }

  getMatch(matchId: string): Observable<IMatch> {
    return this.api
      .get<IMatch>(
        `/game/match/${this.loginService.player.Username}/${matchId}`,
        {
          withCredentials: true,
        }
      )
      .pipe(
        map((matchResponse: IMatch) => {
          this.match = matchResponse;
          this.matchChange.next(this.match);
          return matchResponse;
        })
      );
  }

  getCard(): Observable<ICard> {
    return this.api
      .get<ICard>(
        `/game/match/draw-card/${this.loginService.player.Username}/${this.match.id}`,
        {
          withCredentials: true,
        }
      )
      .pipe(
        map((cardRes: ICard) => {
          this.match.player.hand.push(cardRes);
          this.matchChange.next(this.match);

          // console.log(cardRes); // DEBUG

          return cardRes;
        })
      );
  }

  addDiscardCard(discardCard: ICard[]): void {
    if (this.discardCards.length <= 2) {
      this.discardCards = [];
      this.discardCards.push(...discardCard);
      this.discardChange.next(this.discardCards.slice());

      // console.log('selectedCard in matchService: ', this.discardCards); //DEBUG
    }
  }

  updatePileId(pileId: number): void {
    this.discardPileId = pileId;
    this.discardPileIdChange.next(this.discardPileId);

    // console.log(this.discardPileId); // DEBUG
  }

  playTurn(): Observable<IMatch> {
    let matchDto = {
      DiscardCards: this.discardCards,
      DiscardPileId: this.discardPileId,
    };

    return this.api
      .post<IMatch>(
        `/game/match/play/${this.loginService.player.Username}/${this.match.id}`,
        matchDto,
        {
          withCredentials: true,
        }
      )
      .pipe(
        map((matchRes: IMatch) => {
          this.match = matchRes;
          this.matchChange.next(this.match);

          this.discardCards = [];
          this.discardChange.next(this.discardCards);

          this.errorMessageChange.next('');

          // console.log('pile id in matchSercvice: ', this.discardPileId);

          return matchRes;
        })
      );
  }
}
